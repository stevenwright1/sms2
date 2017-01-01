/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth;

import java.io.IOException;
import java.util.Map;

import com.citrix.authentication.tokens.AccessToken;
import com.citrix.authentication.tokens.UPNCredentials;
import com.citrix.wi.config.AuthenticationConfiguration;
import com.citrix.wi.config.auth.ExplicitUDPAuth;
import com.citrix.wi.config.auth.TwoFactorAuthMethod;
import com.citrix.wi.config.auth.WIAuthPoint;
import com.citrix.wi.controls.LoginPageControl;
import com.citrix.wi.mvc.ActionState;
import com.citrix.wi.mvc.StatusMessage;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pageutils.AccessTokenResult;
import com.citrix.wi.pageutils.AccountSelfService;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.TwoFactorAuth;
import com.citrix.wi.types.WIAuthType;
import com.citrix.wing.MessageType;
import com.citrix.wing.util.Strings;

public class AccountSSUser extends AbstractAccountSSLayout {

    protected LoginPageControl viewControl = new LoginPageControl();

    public AccountSSUser(WIContext wiContext) {
        super(wiContext, false, false);
        getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
    }

    public ActionState performInternal() throws IOException {
        WebAbstraction web = getWebAbstraction();
        // Get the configuration for explicit authentication
        // The call to isAccountSelfServiceEnabled() will have ensured that this
        // is ExplicitUDPAuth

        AuthenticationConfiguration authConfig = wiContext.getConfiguration().getAuthenticationConfiguration();
        ExplicitUDPAuth udpAuth = null;
        if (authConfig.getAuthPoint() instanceof WIAuthPoint) {
            WIAuthPoint wiAuthPoint = (WIAuthPoint)authConfig.getAuthPoint();
            udpAuth = (ExplicitUDPAuth)wiAuthPoint.getAuthMethod(WIAuthType.EXPLICIT);
        }
        TwoFactorAuthMethod twoFactorMethod = TwoFactorAuth.getTwoFactorAuthMethod(udpAuth);

        Map parameters = (Map)Authentication.getAuthenticationState(web).getParameters();

        String errorMessage = (String)parameters.get(Constants.CONST_ERROR_MESSAGE);
        if (errorMessage != null) {
            setFeedback(MessageType.ERROR, errorMessage);
            parameters.remove(Constants.CONST_ERROR_MESSAGE);
        }

        // Deal with form submission
        if (web.isPostRequest()) {
            return processPost(web, udpAuth, twoFactorMethod);
        } else {
            return processGet(web, udpAuth, twoFactorMethod);
        }
    }

    /**
     * Process the get request. This involves populating the viewControl
     *
     * @param web
     * @param udpAuth
     * @param twoFactorMethod
     * @return
     */
    private ActionState processGet(WebAbstraction web, ExplicitUDPAuth udpAuth, TwoFactorAuthMethod twoFactorMethod) {
        // Set up the page appearance
        welcomeControl.setTitle(wiContext.getString(getPageTitleKey()));
        welcomeControl.setBody(wiContext.getString("SelfServiceAccountWelcome"));
        layoutControl.formAction = Constants.FORM_POSTBACK;

        // Set up the page control

        // Check whether 2-factor authentication is enabled
        viewControl.setShowPasscode(twoFactorMethod != null);

        int numDomains = udpAuth.getDomainSelection().size();
        // Hide the domain field only if this has been configured and:
        // there is at most one entry in the list of domains, or
        // login is restricted to the entries in the domain restriction list
        boolean hideDomain = (udpAuth.getDomainFieldHidden() && ((numDomains <= 1) || udpAuth.getDomainsRestricted()));
        viewControl.setShowDomain(!hideDomain);

        if (numDomains != 0) {
            viewControl.setLoginDomainSelection(udpAuth.getDomainSelectionArray());
        }

        if (udpAuth.getDomainsRestricted()) {
            viewControl.setLoginDomains(udpAuth.getDomainsArray());
        }

        viewControl.setRestrictDomains(udpAuth.getDomainsRestricted());

        // Ensure password field is not displayed
        viewControl.setShowPassword(false);

        // check if we have any invalid fields passed through a query string
        Authentication.extractInvalidFieldData(viewControl, web);

        return new ActionState(true);
    }

    /**
     * Process Post request. This involves validating the credentials, and
     * redirect to the next page as appropriate.
     *
     * @param web
     * @param udpAuth
     * @param twoFactorMethod
     * @return
     */
    private ActionState processPost(WebAbstraction web, ExplicitUDPAuth udpAuth, TwoFactorAuthMethod twoFactorMethod) {
        String submitMode = web.getFormParameter(Constants.ID_SUBMIT_MODE);
        if (Constants.VAL_OK.equalsIgnoreCase(submitMode)) {

            // Pull out the fields we have interest in
            String user = Strings.ensureNonNull(web.getFormParameter(Constants.ID_USER));
            String domain = Strings.ensureNonNull(web.getFormParameter(Constants.ID_DOMAIN));
            String passcode = Strings.ensureNonNull(web.getFormParameter(Constants.ID_PASSCODE));

            // Keep track of any errors
            AccessTokenResult accessTokenResult = null;

            // Make sure none of the credential fields contain control
            // characters
            if (Strings.hasControlChars(user) || Strings.hasControlChars(domain) || Strings.hasControlChars(passcode)) {
                return new ActionState(new StatusMessage(MessageType.ERROR, "InvalidCredentials"));
            } else {
                // Attempt to create an access token
                accessTokenResult = Authentication.createAccessToken(user, domain, "", udpAuth);

                // UPN credentials are not allowed with account self service
                AccessToken token = accessTokenResult.getAccessToken();
                if (token != null && (token instanceof UPNCredentials)) {
                    accessTokenResult = new AccessTokenResult(MessageType.ERROR, "UPNcredentialFormatNotAllowed", true,
                                    false, user, domain);
                }
            }

            if (accessTokenResult.isError()) {
                // we had an error building the AccessToken
                // show the appropriate messages to the users
                Authentication.processAccessTokenResultError(web, accessTokenResult, Constants.PAGE_ACCOUNT_SS_USER);
            } else {
                goToNextPage(web, twoFactorMethod, passcode, accessTokenResult);
            }
            // don't show the form, we will be redirecting somewhere
            return new ActionState(false);

        } else {
            // The user has pressed the Cancel button
            return new ActionState(new StatusMessage(MessageType.INFORMATION, "SelfServiceNotComplete"));
        }
    }

    /**
     * Process valid credentials by redirectiing to the next page the auth
     * filter says we should visit.
     *
     * @param web
     * @param twoFactorMethod
     * @param passcode
     * @param accessTokenResult
     */
    private void goToNextPage(WebAbstraction web, TwoFactorAuthMethod twoFactorMethod, String passcode,
                    AccessTokenResult accessTokenResult) {
        // Store credentials in the session if they are not invalid
        Map parameters = (Map)Authentication.getAuthenticationState(web).getParameters();
        parameters.put(Authentication.VAL_ACCESS_TOKEN, accessTokenResult.getAccessToken());
        parameters.put(TwoFactorAuth.VAL_PASSCODE, passcode);

        // If two-factor authentication is in use, add it to the queue head so
        // that it
        // executes before the next page
        if (twoFactorMethod != null) {
            Authentication.addPageToQueueHead(wiContext, twoFactorMethod.getName().toLowerCase(), parameters);
        }

        Authentication.redirectToNextAuthPage(wiContext);
        // Add an authorization cookie (used by WI to identify the user for the
        // rest of the account self-service
        // process) and also store the cookie's value in the session object.
        AccountSelfService.recordAuthId(wiContext, addAuthCookie());
    }
}
