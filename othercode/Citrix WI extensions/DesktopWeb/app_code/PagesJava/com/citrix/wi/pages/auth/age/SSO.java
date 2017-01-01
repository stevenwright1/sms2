// Copyright (c) 2004 - 2010 Citrix Systems, Inc. All Rights Reserved.

package com.citrix.wi.pages.auth.age;

import java.io.IOException;
import java.util.HashMap;

import com.citrix.authentication.tokens.AccessToken;
import com.citrix.authentication.tokens.UPNCredentials;
import com.citrix.authentication.tokens.UserDomainPasswordCredentials;
import com.citrix.authentication.tokens.age.SAMIdentityToken;
import com.citrix.authentication.tokens.age.UPNIdentityToken;
import com.citrix.authentication.web.AuthUtilities;
import com.citrix.wi.config.AuthenticationConfiguration;
import com.citrix.wi.config.auth.AGAuthPoint;
import com.citrix.wi.config.auth.AuthPoint;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pages.StandardLayout;
import com.citrix.wi.pages.auth.age.types.HeaderParameters;
import com.citrix.wi.pageutils.AGEUtilities;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wi.types.AGAuthenticationMethod;
import com.citrix.wi.types.AllowChangePassword;
import com.citrix.wing.MessageType;
import com.citrix.wing.util.Strings;

/**
 * Handles single-sign-on authentication between the Access Gateway and Web
 * Interface.
 *
 * Access Gateway and Web Interface communicate via a proprietary HTTP
 * challenge-response protocol, which operates in a similar way to HTTP Basic
 * Authentication.
 */
public class SSO extends StandardLayout {
    // Components of the CitrixAGBasic challenge
    private static final String AUTH_HEADER = "CitrixAGBasic "; // the trailing space is required
    private static final String KEY_PASSWORD_REQUIRED = "password_required";
    private static final String KEY_LOGOUT_URL = "logout_url";

    // HTTP Headers used during challenge/response
    private static final String REQUEST_HEADER_NAME = "Authorization";
    private static final String RESPONSE_HEADER_NAME = "WWW-Authenticate";
    private static final String HEADER_CHANGE_PASSWORD_URL = "X-CitrixAG-ChangePassword";

    // Error keys
    private static final String KEY_NO_AG_LOGIN_ALLOWED = "NoAGELogin";

    // Log message strings
    private static final String MSG_CHANGE_PASSWORD_NOT_SUPPORTED = "AGChangePasswordNotSupported";

    public SSO(WIContext wiContext) {
        super(wiContext);
    }

    public boolean performImp() throws IOException {
        WebAbstraction web = wiContext.getWebAbstraction();
        AuthenticationConfiguration authConfig = wiContext.getConfiguration().getAuthenticationConfiguration();

        // Check that AGE passthrough is allowed
        AuthPoint authPoint = authConfig.getAuthPoint();
        if (authPoint == null || !(authPoint instanceof AGAuthPoint)) {
            UIUtils.HandleLoginFailedMessage(wiContext, MessageType.ERROR, KEY_NO_AG_LOGIN_ALLOWED);
            return false;
        }

        AGAuthPoint agAuthPoint = (AGAuthPoint)authPoint;

        boolean isPasswordRequiredFromAG = isPasswordRequiredFromAG(agAuthPoint);

        String authStr = web.getRequestHeader(REQUEST_HEADER_NAME);
        if (isResponseToChallenge(authStr)) {
            processResponseToChallenge(web, agAuthPoint, isPasswordRequiredFromAG, authStr.trim());
            return false;
        }

        createAndSendChallenge(web, isPasswordRequiredFromAG);

        return false;
    }

    private static boolean isPasswordRequiredFromAG(AGAuthPoint agAuthPoint) {
        return (agAuthPoint.getAuthenticationMethod() == AGAuthenticationMethod.EXPLICIT)
                && !agAuthPoint.isPromptPasswordEnabled();
    }

    private static boolean isResponseToChallenge(String authStr) {
        return authStr != null && authStr.length() > 0;
    }

    private void createAndSendChallenge(WebAbstraction web, boolean isPasswordRequiredFromAG) throws IOException {
        String logoutTicket = AuthUtilities.createRandomString();

        // Add mapping from ticket to session
        wiContext.getUtils().storeAGLogoutTicket(wiContext, logoutTicket);

        String passwordRequired = isPasswordRequiredFromAG ? Constants.VAL_YES : Constants.VAL_NO;

        // Send challenge
        String challenge = AUTH_HEADER +
            KEY_PASSWORD_REQUIRED + "=\"" + passwordRequired + "\";" +
            KEY_LOGOUT_URL + "=\"" + Constants.PAGE_AGE_LOGOUT + "?" +
            AGEUtilities.QSTR_LOGOUT_TICKET + "=" + logoutTicket + "\"";

        web.appendResponseHeader(RESPONSE_HEADER_NAME, challenge);
        web.setResponseStatus(WebAbstraction.SC_UNAUTHORIZED);
        web.writeToResponse(wiContext.getString(AGEUtilities.KEY_AG_AUTH_ERROR));
    }

    private void processResponseToChallenge(WebAbstraction web, AGAuthPoint agAuthPoint,
                    boolean isPasswordRequiredFromAG, String authStr) throws IOException {
        if (authStr.startsWith(AUTH_HEADER)) {
            HeaderParameters headerParams = HeaderParameters.fromAGBasicChallenge(
                            authStr.substring(AUTH_HEADER.length()), isPasswordRequiredFromAG);

            AccessToken accessToken = createAccessToken(agAuthPoint, headerParams);

            if (accessToken != null) {
                // Store in authentication state and forward to the next page
                saveHeaderParamsToAuthState(web, headerParams, accessToken);

                AllowChangePassword passwordPolicy = agAuthPoint.getPasswordChangePolicy();
                boolean passwordChangesAllowed = (passwordPolicy == AllowChangePassword.EXPIRED_ONLY) ||
                    (passwordPolicy == AllowChangePassword.ALWAYS);

                // Extract change password URL if required
                if (passwordChangesAllowed) {
                    processChangePasswordUrl(headerParams);
                }

                Authentication.forwardToNextAuthPage(wiContext);
                return;
            }
        }

        // Bad response to challenge
        web.abandonSession();
        web.setResponseStatus(WebAbstraction.SC_UNAUTHORIZED);
        web.writeToResponse(wiContext.getString(AGEUtilities.KEY_AG_AUTH_ERROR));
    }

    private void processChangePasswordUrl(HeaderParameters headerParams) {
        if (accessGatewaySupportsChangePassword(headerParams)) {
            // See if a change password URL has been provided
            // It is not necessarily an error if the URL is missing: AG might be configured to
            // only allow specific users to change their passwords
            String changePasswordUrl = wiContext.getWebAbstraction().getRequestHeader(HEADER_CHANGE_PASSWORD_URL);
            if (changePasswordUrl != null) {
                AGEUtilities.setAGEChangePasswordUrl(wiContext, changePasswordUrl);
            }
        } else {
            // WI admin has configured change password support, but the AG is not capable
            // of supporting it.
            wiContext.log(MessageType.WARNING, MSG_CHANGE_PASSWORD_NOT_SUPPORTED);
        }
    }

    private boolean accessGatewaySupportsChangePassword(HeaderParameters headerParams) {
        return (headerParams.getAgeProtocolRevision() != null) &&
            (headerParams.getAgeProtocolRevision().intValue() >= MIN_VERSION_FOR_CHANGE_PASSWORD);
    }

    private static final int MIN_VERSION_FOR_CHANGE_PASSWORD = 2;

    private static void saveHeaderParamsToAuthState(WebAbstraction web, HeaderParameters headerParams, AccessToken accessToken) {
        HashMap parameters = (HashMap)Authentication.getAuthenticationState(web).getParameters();
        parameters.put(Constants.AGE_USERNAME, headerParams.getUsername());
        parameters.put(Constants.AGE_DOMAIN, headerParams.getDomain());
        parameters.put(Constants.AGE_SESSION_ID, headerParams.getAgeSessionId());
        parameters.put(Constants.AGE_ACCESS_TOKEN, accessToken);
    }

    private static AccessToken createAccessToken(AGAuthPoint agAuthPoint, HeaderParameters headerParams) {
        try {
            AGAuthenticationMethod authMethod = agAuthPoint.getAuthenticationMethod();            
            
            if (isUPN(headerParams.getUsername())) {
                if (isExplicitAuthEnabled(authMethod)) {
                    return new UPNCredentials(headerParams.getUsername(), headerParams.getPassword());
                } else if (isSmartCardPassThroughAuthEnabled(authMethod)){
                    return new UPNIdentityToken(headerParams.getUsername());
                } else {
                    // UPNs are not supported for smart card prompt authentication.
                    return null;
                }
            } else {
                if (isExplicitAuthEnabled(authMethod)) {
                    return new UserDomainPasswordCredentials(headerParams.getUsername(),
                                    headerParams.getDomain(), headerParams.getPassword());
                } else {
                    return new SAMIdentityToken(headerParams.getUsername(), headerParams.getDomain());
                }
            }
        } catch (IllegalArgumentException ignore) {
            // In case the credentials were badly formatted, leave accessToken null
            return null;
        }
    }

    private static boolean isUPN(String username) {
        return username.indexOf('@') != -1;
    }

    private static boolean isExplicitAuthEnabled(AGAuthenticationMethod authMethod) {
        return AGAuthenticationMethod.EXPLICIT.equals(authMethod);
    }
    
    private static boolean isSmartCardPassThroughAuthEnabled(AGAuthenticationMethod authMethod)
    {
        return AGAuthenticationMethod.SMART_CARD_KERBEROS.equals(authMethod);        
    }

    public String getBrowserPageTitleKey() {
        // return null so only site name is used.
        // no UI associated with this class.
        return null;
    }
}