/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils;

import java.util.Map;

import com.citrix.authentication.tokens.AccessToken;
import com.citrix.authentication.tokens.PasswordBasedToken;
import com.citrix.authentication.tokens.SIDBasedToken;
import com.citrix.authentication.tokens.UPNCredentials;
import com.citrix.authentication.tokens.UserDomainPasswordCredentials;
import com.citrix.authentication.web.AuthenticationState;
import com.citrix.wi.config.AuthenticationConfiguration;
import com.citrix.wi.config.WIConfiguration;
import com.citrix.wi.config.auth.AGAuthPoint;
import com.citrix.wi.config.auth.ExplicitUDPAuth;
import com.citrix.wi.config.auth.WIAuthPoint;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.types.CredentialFormat;
import com.citrix.wi.types.WIAuthType;
import com.citrix.wi.util.WIAccessTokenValidator;
import com.citrix.wing.AccessTokenValidator;
import com.citrix.wing.AccessTokenValidity;
import com.citrix.wing.MessageType;
import com.citrix.wing.SourceIOException;
import com.citrix.wing.UserEnvironmentAdaptor;
import com.citrix.wing.util.Strings;
import com.citrix.wi.types.AGEAccessMode;
import com.citrix.wi.types.AllowChangePassword;
import com.citrix.wi.config.auth.ExplicitAuth;
import com.citrix.wi.controls.LoginPageControl;

/**
 * This class contains some utility methods related to Authentication
 */
public class Authentication {

    // Constants used for authentication parameter entries
    public static final String VAL_ACCESS_TOKEN  = "AccessToken";
    public static final String VAL_EXPLICIT_AUTH = "ExplicitAuth";

    // This key is used in the session and client session state.
    // It is used to record the authentication method for the
    // session. The value stored in the client session state is
    // untrusted. It is set when the session is authenticated and reset when the
    // session is logged out. If the server session has expired (e.g. server
    // restart), further user action will be intercepted by the authentication
    // layer. In this case the login page will see that this cookie is set and
    // displays the logged out page with the session expire message.
    public static final String LOGON_TYPE_KEY    = "CTX_LogonType";

    /**
     * Creates and returns an access token of the appropriate type from the
     * given arguments.
     *
     * @param user string representing the user
     * @param domain the user domain
     * @param password the user password
     * @param udpAuth the explicit authentication configuration
     * @return an <code>AccessTokenResult</code> where the
     * <code>AccessToken</code> inside may be object or null if none could be
     * created
     */
    public static AccessTokenResult createAccessToken(String user, String domain, String password,
                    ExplicitUDPAuth udpAuth) {
        AccessTokenResult result;
        if (user == null || user.length() == 0) {
            // Check for blank username
            result = new AccessTokenResult(MessageType.ERROR, "BlankUsername", true, false, user, domain);

        } else if (user.indexOf("@") >= 0) {
            // User has entered UPN style credentials
            result = createUPNAccessToken(user, password, udpAuth);

        } else {
            // User has entered non-UPN style credentials (domain\name or
            // standard)
            result = createNTAccessToken(user, domain, password, udpAuth);
        }
        return result;
    }

    /**
     * Deals with user@domain style credentials
     */
    private static AccessTokenResult createUPNAccessToken(String user, String password, ExplicitUDPAuth udpAuth) {
        AccessTokenResult result;
        if ((udpAuth.getCredentialFormat() != CredentialFormat.UPN)
                        && (udpAuth.getCredentialFormat() != CredentialFormat.ALL)) {
            // UPN style is not allowed
            result = new AccessTokenResult(MessageType.ERROR, "UPNcredentialFormatNotAllowed", true, false, user, null);

        } else {
            // Validate there are username and domain parts to the UPN.
            String[] userElems = Strings.split(user, '@', 2);
            if (userElems[0] == null || userElems[0].trim().length() == 0) {
                // Avoid cases where user has entered '@domain'
                result = new AccessTokenResult(MessageType.ERROR, "BlankUsername", true, false, user, null);

            } else if (userElems.length < 2 || userElems[1] == null || userElems[1].trim().length() == 0) {
                // Avoid cases where user has entered 'user@'
                // Since UPN credentials were used, we will mark the username
                // field invalid
                // rather than the domain field
                result = new AccessTokenResult(MessageType.ERROR, "BlankDomain", true, false, user, null);

            } else {
                // All is well, create the AccessToken
                result = new AccessTokenResult(new UPNCredentials(user, password));
            }
        }
        return result;
    }

    /**
     * Deals with all non-upn credentials
     */
    private static AccessTokenResult createNTAccessToken(String user, String domain, String password,
                    ExplicitUDPAuth udpAuth) {
        AccessTokenResult result;
        if ((udpAuth.getCredentialFormat() != CredentialFormat.DOMAIN_USERNAME)
                        && (udpAuth.getCredentialFormat() != CredentialFormat.ALL)) {
            // DomanUsername format is not allowed
            result = new AccessTokenResult(MessageType.ERROR, "DomainUsernameCredentialFormatNotAllowed", true, true,
                            user, domain);

        } else if (user.indexOf("\\") >= 0) {
            // User has entered an NT-style 'domain \ user'
            result = createNTSlashAccessToken(user, password);

        } else {
            // User has entered domain separetly, or not at all
            result = createNTStandardAccessToken(user, domain, password, udpAuth);
        }
        return result;
    }

    /**
     * Process domain/username style credentials
     */
    private static AccessTokenResult createNTSlashAccessToken(String user, String password) {
        AccessTokenResult result;
        // validate the input
        String[] userElems = Strings.split(user, '\\', 2);
        if (userElems[0] == null || userElems[0].trim().length() == 0) {
            // Avoid cases where user has entered '\ user'
            // Since D\U-style credentials were entered, we will mark the
            // username field invalid
            // rather than the domain field
            result = new AccessTokenResult(MessageType.ERROR, "BlankDomain", true, false, user, null);

        } else if (userElems.length < 2 || userElems[1] == null || userElems[1].trim().length() == 0) {
            // Avoid cases where user has entered 'domain \'
            result = new AccessTokenResult(MessageType.ERROR, "BlankUsername", true, false, user, null);

        } else {
            // Create the AccessToken
            result = new AccessTokenResult(new UserDomainPasswordCredentials(userElems[1], userElems[0], password));
        }
        return result;
    }

    /**
     * Deals with normally entered credentials, and fills in the domain, if that
     * is configured to happen.
     */
    private static AccessTokenResult createNTStandardAccessToken(String user, String domain, String password,
                    ExplicitUDPAuth udpAuth) {
        AccessTokenResult result;
        if (domain.trim().length() != 0) {
            // User has entered User/Domain/Password
            result = new AccessTokenResult(new UserDomainPasswordCredentials(user, domain, password));

        } else {
            // The user has not entered a domain in any form, use the first
            // in the login domain list (if appropriate).
            if ((udpAuth.getDomainFieldHidden()) && !(udpAuth.getDomainSelection().isEmpty())) {
                // WI is configured to hide the domain field and the
                // user does not specify a domain in the username
                // field. We attempt to retrieve the first domain in
                // the DomainSelection list as the default domain.
                String d = (String)udpAuth.getDomainSelection().get(0);

                // Note: The domain can be empty if explicitly set by
                // the admin. This allows Unix-style credentials, with
                // no domain.
                result = new AccessTokenResult(new UserDomainPasswordCredentials(user, d, password));

            } else {
                // Can't find a domain to put in
                result = new AccessTokenResult(MessageType.ERROR, "BlankDomain", false, true, user, domain);
            }
        }
        return result;
    }

    /** Constant used for storing the username in the session */
    private static final String STORED_FAILED_USERNAME = "StoredFailedCredentialsUsername";

    /**
     * Store the failed username in the session
     *
     * @param username that is stored
     * @param web the <code>WebAbstraction</code>
     */
    public static void setFailedUsername(String username, WebAbstraction web) {
        web.setSessionAttribute(STORED_FAILED_USERNAME, username);
    }

    /**
     * Get the stored (failed) username from the session
     *
     * @param web the <code>WebAbstraction</code>
     * @return returns a null string when there is nothing stored.
     */
    public static String getFailedUsername(WebAbstraction web) {
        Object obj = web.getSessionAttribute(STORED_FAILED_USERNAME);
        return (obj == null) ? "" : (String)obj;
    }

    /** Constant used for storing the domain in the session */
    private static final String STORED_FAILED_DOMAIN = "StoredFailedCredentialsDomain";

    /**
     * Store the failed domain in the session
     *
     * @param domain to store
     * @param web the <code>WebAbstraction</code>
     */
    public static void setFailedDomain(String domain, WebAbstraction web) {
        web.setSessionAttribute(STORED_FAILED_DOMAIN, domain);
    }

    /**
     * Get the stored (failed) domain in the Session
     *
     * @param web the <code>WebAbstraction</code>
     * @return returns an empty string when there is nothing stored.
     */
    public static String getFailedDomain(WebAbstraction web) {
        Object obj = web.getSessionAttribute(STORED_FAILED_DOMAIN);
        return (obj == null) ? "" : (String)obj;
    }

    /** Constant used for storing the domain in the session */
    private static final String STORED_CONTEXT_LIST = "StoredContextLookupList";

    /**
     * Store the failed password in the session
     *
     * @param domain to store
     * @param web the <code>WebAbstraction</code>
     */
    public static void setContextList(String[] contexts, WebAbstraction web) {
        web.setSessionAttribute(STORED_CONTEXT_LIST, contexts);
    }

    /**
     * Get the stored (failed) password in the Session
     *
     * @param web the <code>WebAbstraction</code>
     * @return returns an empty string array when there is nothing stored.
     */
    public static String[] getContextList(WebAbstraction web) {
        Object obj = web.getSessionAttribute(STORED_CONTEXT_LIST);
        if (obj == null || !(obj instanceof String[])) {
            return new String[0]; // don't return null, as this upsets the
            // loginpagecontrol
        } else {
            return (String[])obj;
        }
    }

    /**
     * Process access token result error by storing the username and domain,
     * then redirects to the given page with the correct error string. The error
     * string is updated to reflect the invalid fields mentioned in the
     * accessToken result. The session is not distroyed due to the need to keep
     * the username and domain in the session state.
     *
     * @param web the <code>WebAbstraction</code>
     * @param accessTokenResult the result that is to be processed
     * @param url that we will redirect to
     */
    public static void processAccessTokenResultError(WebAbstraction web, AccessTokenResult accessTokenResult, String url) {
        // fail we are given a null
        if (accessTokenResult == null) {
            throw new IllegalArgumentException("Can't be past a null accessTokenResult");
        }

        // store the username and domain to be picked up by a login page control
        Authentication.setFailedUsername(accessTokenResult.getUsername(), web);
        Authentication.setFailedDomain(accessTokenResult.getDomain(), web);

        // redirect to the appropriate page showing the correct error
        String redirectURL = url
                        + UIUtils.getMessageQueryStr(accessTokenResult.getMessageType(), accessTokenResult
                                        .getMessageKey());

        // and highlighting the correct field as invalid
        if (accessTokenResult.isInvalidUsername()) {
            redirectURL += "&" + Constants.QSTR_INVALID_USERNAME + "=" + Constants.VAL_TRUE;
        }
        if (accessTokenResult.isInvalidDomain()) {
            redirectURL += "&" + Constants.QSTR_INVALID_DOMAIN + "=" + Constants.VAL_TRUE;
        }
        if (accessTokenResult.isInvalidContext()) {
            redirectURL += "&" + Constants.QSTR_INVALID_CONTEXT + "=" + Constants.VAL_TRUE;
        }

        // do the redirect
        web.clientRedirectToUrl(redirectURL);
    }

    /**
     * Override that redirects to the login page after processing the
     * <code>AccessTokenResult</code>
     *
     * @param web the <code>WebAbstraction</code>
     * @param accessTokenResult the result that is to be processed
     */
    public static void processAccessTokenResultError(WebAbstraction web, AccessTokenResult accessTokenResult) {
        processAccessTokenResultError(web, accessTokenResult, Constants.PAGE_LOGIN);
    }

    /**
     * Looks at the session state to see if and username, domain or context list
     * has been stored and so needs to be output on the login page. It also
     * looks at the query string to see if any of the fields have been marked as
     * invalid.
     *
     * @param viewControl that needs to be updated
     * @param web the <code>WebAbstraction</code>
     */
    public static void extractInvalidFieldData(LoginPageControl viewControl, WebAbstraction web) {
        // check for invalid fields
        if (Constants.VAL_TRUE.equals(web.getQueryStringParameter(Constants.QSTR_INVALID_USERNAME))) {
            viewControl.addInvalidField(Constants.ID_USER);
        }
        if (Constants.VAL_TRUE.equals(web.getQueryStringParameter(Constants.QSTR_INVALID_DOMAIN))) {
            viewControl.addInvalidField(Constants.ID_DOMAIN);
        }
        if (Constants.VAL_TRUE.equals(web.getQueryStringParameter(Constants.QSTR_INVALID_CONTEXT))) {
            viewControl.addInvalidField(Constants.ID_CONTEXT);
        }
        // use the stored credential if there is an invalid field
        viewControl.setUserName(Authentication.getFailedUsername(web));
        viewControl.setDomain(Authentication.getFailedDomain(web));
        // we have had an nds context lookup that has left us with a few options
        viewControl.setNDSContexts(Authentication.getContextList(web));
    }

    /**
     * Gets the logon mode of this session. Some WI functionality depends on the
     * 'mode' used to authenticate to WI. For instance, certain functionality is
     * disabled for guest users. This method indicates how the user
     * authenticated to Web Interface.
     *
     * @param request The HTTP request of the current page.
     * @return A String indicating the Web Interface authentication method.
     */
    public static WIAuthType getLogonType(UserEnvironmentAdaptor envAdaptor) {
        return WIAuthType.fromString((String)envAdaptor.getSessionState().get(LOGON_TYPE_KEY));
    }

    /**
     * Gets the user's primary access token. The primary access token is the
     * token holding the credentials the user used to authenticate to Web
     * Interface. A HttpServletRequest instance is passed to this method, so
     * that it's possible to pull the user's access token from the HTTP headers,
     * which can be useful for integrating with some technologies.
     *
     * @param request The HTTP request of the current page.
     * @return An instance of AccessToken, or <code>null</code>.
     */
    public static AccessToken getPrimaryAccessToken(WebAbstraction web) {
        AuthenticationState authState = getAuthenticationState(web);
        if (authState == null) {
            return null;
        } else {
            return authState.getAccessToken();
        }
    }

    /**
     * Retrieve the logon type from the session cookie. The returned value must
     * not be trusted as it is controlled by the client device.
     *
     * @return the logon type recorded in the session cookie.
     */
    public static WIAuthType getUntrustedLogonType(UserEnvironmentAdaptor envAdaptor) {
        return WIAuthType.fromString((String)envAdaptor.getClientSessionState().get(LOGON_TYPE_KEY));
    }

    /**
     * Indicates if the user is an anonymous (guest) user. This is a convenience
     * method, it simply checks if the logon type is ANONYMOUS.
     *
     * @param request The HTTP request of the current page.
     * @return <code>true</code> if the user is a guest user.
     */
    public static boolean isAnonUser(UserEnvironmentAdaptor envAdaptor) {
        return WIAuthType.ANONYMOUS.equals(getLogonType(envAdaptor));
    }

    /**
     * Indicates if the user is an explicitly authenticated user. This is a
     * convenience method, it simply checks if the logon type is EXPLICIT.
     *
     * @param request The HTTP request of the current page.
     * @return <code>true</code> if the user is an explicitly authenticated
     * user.
     */
    public static boolean isExplicitUser(UserEnvironmentAdaptor envAdaptor) {
        return WIAuthType.EXPLICIT.equals(getLogonType(envAdaptor));
    }

    /**
     * Indicates if the user has authenticated via Passthrough. This doesn't
     * include passthrough with smartcard as it simply checks if the logon type
     * is SINGLE_SIGN_ON.
     *
     * @param request The HTTP request of the current page.
     * @return <code>true</code> if the user is authenticated using passthrough
     * user.
     */
    public static boolean isPassthroughUser(UserEnvironmentAdaptor envAdaptor) {
        return WIAuthType.SINGLE_SIGN_ON.equals(getLogonType(envAdaptor));
    }

    /**
     * Set the logon type in the session cookie. This value will not be trusted
     * as it is controlled by the client device.
     *
     * @param the logon type to record; use <code>null</code> to clear the
     * cookie.
     */
    public static void setUntrustedLogonType(WIAuthType logonType, UserEnvironmentAdaptor envAdaptor) {
        saveLogonTypeToMap(logonType, envAdaptor.getClientSessionState());
    }

    /**
     * Save the logon type used by the user. The value is stored in both session
     * and client-session state. Storage in client session state allows the
     * logon type to be used by the session timeout mechanism and smartcard
     * logout handling, which need the value after a WI session is logged out.
     * Storage in session state allows for trusted storage of the value.
     *
     * @param logonType the logon type to record; use <code>null</code> to clear
     * the cookie.
     */
    public static void storeLogonType(WIAuthType logonType, UserEnvironmentAdaptor envAdaptor) {
        setUntrustedLogonType(logonType, envAdaptor);

        saveLogonTypeToMap(logonType, envAdaptor.getSessionState());
    }

    /*
     *
     */
    private static void saveLogonTypeToMap(WIAuthType logonType, Map map) {
        String logonTypeString = null;
        if (logonType != null) {
            logonTypeString = logonType.toString();
        }

        map.put(LOGON_TYPE_KEY, logonTypeString);
    }

    /**
     * Gets the AuthenticationState instance for this session. This is stored in
     * the request, not the session, as the AuthenticationFilter exposes the
     * session AuthenticationState via a request relevant wrapper. This enables
     * the AuthenticationState to perform request/response relevant operations
     * (such as writing out cookies). To isolate Web Interface from the
     * authentication layer, do not use this method directly outside of the
     * authentication pages.
     *
     * @param request the HttpServletRequest object
     * @return An instance of AuthenticationState.
     */
    public static AuthenticationState getAuthenticationState(WebAbstraction web) {
        Object o = web.getRequestContextAttribute(Constants.AUTHSTATE_VARIABLE_NAME);
        if (o instanceof AuthenticationState) {
            return (AuthenticationState)o;
        }
        return null;
    }

    /**
     * Tests whether the user's primary access token has an expired password.
     *
     * @param request the HTTP request
     * @return true if the password is considered to be expired, else false.
     */
    public static boolean hasPasswordExpired(WebAbstraction web) {
        boolean result = false;
        PasswordBasedToken pbt = null;

        AccessToken ac = getPrimaryAccessToken(web);
        if (ac != null && ac instanceof PasswordBasedToken) {
            // Only PasswordBasedTokens can have an expired password.
            pbt = (PasswordBasedToken)ac;
        }

        if (pbt != null && pbt.hasPasswordExpired()) {
            result = true;
        }

        return result;
    }

    /**
     * Verifies that the given access token meets domain restrictions and other
     * administrator-imposed conditions. Also authenticates the user against the
     * AccessTokenValidator that is provided (if any), propagating any password
     * expiry information.
     *
     * @param wiConfig The current WI Configuration
     * @param validator An AccessTokenValidator to call after preliminary validation.
     * @param token an access token to validate.
     * @return the result of the validation.
     */
    public static AccessTokenValidity validateAccessTokenWithExpiry(WIConfiguration wiConfig,
                    AccessTokenValidator validator, AccessToken token) throws SourceIOException {
        WIAccessTokenValidator wiatv = new WIAccessTokenValidator(wiConfig.getAuthenticationConfiguration(), validator);
        return wiatv.checkAccessTokenWithExpiry(token);
    }

    /**
     * Verifies that the given access token meets domain restrictions and other
     * administrator-imposed conditions, and potentially populates the token with
     * group SIDs. Also authenticates the user against the AccessTokenValidator that
     * is provided (if any), propagating any password expiry information and
     * populating the access token with SIDs corresponding to the Active Directory groups
     * the user belongs to.
     *
     * @param wiConfig The current WI Configuration
     * @param validator An AccessTokenValidator to call after preliminary validation.
     * @param token an access token to validate.
     * @return the result of the validation.
     */
    public static AccessTokenValidity validateAccessTokenWithAccountInfo(WIConfiguration wiConfig,
                    AccessTokenValidator validator, SIDBasedToken token) throws SourceIOException {
        WIAccessTokenValidator wiatv = new WIAccessTokenValidator(wiConfig.getAuthenticationConfiguration(), validator);
        return wiatv.checkAccessTokenWithAccountInfo(token);
    }

    /**
     * Marks the current auth page as completed then does a server forward to
     * the next page.
     *
     * @param wiContext
     */
    public static void forwardToNextAuthPage(WIContext wiContext) {
        WebAbstraction web = wiContext.getWebAbstraction();
        AuthenticationState authState = getAuthenticationState(web);
        authState.pageCompleted();
        web.serverForwardToContextUrl(getAuthenticationPageContextPath(wiContext, authState));
    }

    /**
     * Does a server forward to the next auth page.
     *
     * @param wiContext
     */
    public static void forwardToCurrentAuthPage(WIContext wiContext) {
        WebAbstraction web = wiContext.getWebAbstraction();
        AuthenticationState authState = getAuthenticationState(web);
        web.serverForwardToContextUrl(getAuthenticationPageContextPath(wiContext, authState));
    }

    /**
     * Commits the User Environment Adaptor then redirects to the next auth
     * page.
     *
     * @param wiContext
     */
    public static void redirectToCurrentAuthPage(WIContext wiContext) {
        WebAbstraction web = wiContext.getWebAbstraction();
        AuthenticationState authState = getAuthenticationState(web);
        wiContext.getUserEnvironmentAdaptor().commitState();
        wiContext.getUserEnvironmentAdaptor().destroy();
        web.clientRedirectToUrl(authState.getCurrentPage());
    }

    /**
     * Marks the current page as complete, commits the User Environment Adaptor,
     * then redirects to the next auth page.
     *
     * @param wiContext
     */
    public static void redirectToNextAuthPage(WIContext wiContext) {
        WebAbstraction web = wiContext.getWebAbstraction();
        AuthenticationState authState = getAuthenticationState(web);
        authState.pageCompleted();
        wiContext.getUserEnvironmentAdaptor().commitState();
        wiContext.getUserEnvironmentAdaptor().destroy();
        web.clientRedirectToUrl(authState.getCurrentPage());
    }

    /**
     * Adds the given page to the head of the queue, marks the current page as
     * complete, commits the User Environment Adaptor, then redirects to the
     * next auth page.
     *
     * @param wiContext the <code>WIContext</code>
     * @param page to redirect to
     * @param parameters to store
     */
    public static void redirectToAuthPage(WIContext wiContext, String page, Object parameters) {
        addPageToQueueHead(wiContext, page, parameters);
        redirectToNextAuthPage(wiContext);
    }

    /**
     * Gives the context path (used to do a server forward) for the given page
     */
    public static String getAuthenticationPageContextPath(WIContext wiContext, String authenticationPage) {
        return "/auth/" + authenticationPage;
    }

    /**
     * Gives the context path for the next authentication page.
     */
    public static String getAuthenticationPageContextPath(WIContext wiContext, AuthenticationState authState) {
        return getAuthenticationPageContextPath(wiContext, authState.getCurrentPage());
    }

    /**
     * Adds the given pages onto the stack of authentication pages
     */
    public static void addPageToQueueHead(WIContext wiContext, String page, Object properties) {
        getAuthenticationState(wiContext.getWebAbstraction()).addPageToQueueHead(page, properties);
    }

    /**
     * Gets whether the change password policy for the current user allows
     * password changes.
     * 
     * The possibility of a password change may still depend on whether the user's
     * password has expired.
     * 
     * @param wiContext the WI context
     * @return true if password changes are allowed at any time or when a password has expired;
     * false if password changes are never allowed.
     */
    public static boolean isChangePasswordAllowed(WIContext wiContext) {
        AllowChangePassword changePasswordPolicy = getChangePasswordPolicy(wiContext);
        return (changePasswordPolicy == AllowChangePassword.ALWAYS) || (changePasswordPolicy == AllowChangePassword.EXPIRED_ONLY);
    }

    /**
     * Gets the change password policy for the current user.
     * 
     * This method may be called pre or post-login.
     * 
     * @param wiContext the WI context
     * @return an <code>AllowChangePassword</code> indicating whether the user
     * is allowed to change password.
     */
    public static AllowChangePassword getChangePasswordPolicy(WIContext wiContext) {
        AuthenticationConfiguration authConfig = wiContext.getConfiguration().getAuthenticationConfiguration();

        if (authConfig.getAuthPoint() instanceof WIAuthPoint) {
            WIAuthPoint wiAuthPoint = (WIAuthPoint) authConfig.getAuthPoint();
            return getWIChangePasswordPolicy(wiContext, wiAuthPoint);
        } else if (authConfig.getAuthPoint() instanceof AGAuthPoint) {
            AGAuthPoint agAuthPoint = (AGAuthPoint) authConfig.getAuthPoint();
            return getAGChangePasswordPolicy(wiContext, agAuthPoint);
        } else {
            return AllowChangePassword.NEVER;
        }
    }

    /**
     * Gets the change password policy for the current user, when using an authentication
     * point of Web Interface.
     */
    private static AllowChangePassword getWIChangePasswordPolicy(WIContext wiContext, WIAuthPoint wiAuthPoint) {
        ExplicitAuth expAuth = (ExplicitAuth) wiAuthPoint.getAuthMethod(WIAuthType.EXPLICIT);
        if (expAuth == null) {
            // Configuration did not allow explicit authentication
            return AllowChangePassword.NEVER;
        }

        if (Include.isLoggedIn(wiContext.getWebAbstraction())) {
            if (!Authentication.isExplicitUser(wiContext.getUserEnvironmentAdaptor()) ||
                TwoFactorAuth.isPasswordIntegrationEnabled(wiContext.getConfiguration())) {
                // User did not authenticate using explicit authentication, or two-factor
                // password integration was enabled
                // We do not support password change after login when two-factor password
                // integration is in use, because password changes can only occur within the
                // context of a two-factor authentication attempt.
                return AllowChangePassword.NEVER;
            }
        }

        return expAuth.getPasswordChangePolicy();
    }

    /**
     * Gets the change password policy for the current user, when using an authentication
     * point of Access Gateway.
     */
    private static AllowChangePassword getAGChangePasswordPolicy(WIContext wiContext, AGAuthPoint agAuthPoint) {
        // See whether AG provided WI with a change password URL for this user
        boolean changePasswordUrlProvided = (AGEUtilities.getAGEChangePasswordUrl(wiContext) != null);

        // Change password through WI is only available in direct mode, because in
        // other modes (e.g. embedded) Access Gateway can provide its own UI.
        boolean usingDirectMode = (AGEUtilities.getAGEAccessMode(wiContext) == AGEAccessMode.DIRECT);

        if (changePasswordUrlProvided && usingDirectMode) {
            return agAuthPoint.getPasswordChangePolicy();
        } else {
            return AllowChangePassword.NEVER;
        }
    }
}
