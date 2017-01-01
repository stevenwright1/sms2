/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth;

import java.io.IOException;
import java.net.UnknownHostException;
import java.util.Map;

import com.citrix.authentication.tokens.AccessToken;
import com.citrix.authentication.tokens.SIDBasedToken;
import com.citrix.authenticators.AuthenticatorInitializationException;
import com.citrix.authenticators.PasswordCache;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.DisasterRecoveryUtils;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.LaunchUtilities;
import com.citrix.wi.pageutils.PageHistory;
import com.citrix.wi.pageutils.SessionUtils;
import com.citrix.wi.pageutils.TwoFactorAuth;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wing.AccessTokenValidity;
import com.citrix.wing.MessageType;
import com.citrix.wing.SourceIOException;
import com.citrix.wing.SourceUnavailableException;
import com.citrix.wing.webpn.WebPN;

public class ChangePassword extends com.citrix.wi.pages.site.ChangePassword {

    public ChangePassword(WIContext wiContext) {
        super(wiContext);
        // view control dealt with in the super class
    }

    /**
     * When you cancel, always return to the login page telling the
     * user to change their password.
     */
    protected void redirectToNextPage() throws IOException {
        UIUtils.HandleLoginFailedMessage(wiContext, MessageType.ERROR, "MustChangePasswordToLogin");
    }

    /**
     * User is always allowed to change password when not authenticated
     */
    protected boolean checkUserAllowedToChangePassword() throws IOException {
        return true;
    }

    /**
     * Get the provisional access token,
     * if we have not logged in yet.
     */
    protected AccessToken getCredentials() throws IOException {
        Map parameters = (Map)Authentication.getAuthenticationState(wiContext.getWebAbstraction()).getParameters();
        return (AccessToken)parameters.get(Authentication.VAL_ACCESS_TOKEN);
    }

    protected void changePassword(WebAbstraction web, AccessToken credentials, String passwordOld, String passwordNew) throws IOException {
        Map parameters = (Map)Authentication.getAuthenticationState(wiContext.getWebAbstraction()).getParameters();

        // Retrieve the required WebPN from the authentication state, rather
        // than cycling over the available WebPNs, to save a bit of time/effort.
        // The Explicit authentication code does this loop as part of the sign-
        // in process, so it caches the WebPN it found to be usable (iff the
        // password needs changing).
        WebPN webPN = (WebPN)parameters.get("WebPN");

        try {
            // Do change password performs the change password,
            // and redirects with the appropriate error if it fails
            doChangePassword(webPN, credentials, passwordOld, passwordNew);
            return;
        } catch(SourceUnavailableException ignore) { }

        handleError(MessageType.ERROR, "ChangePasswordFailed");
    }

    /**
     * Redirect appropriately on success,
     * including updating the password if we are using password integration
     */
    protected boolean redirectOnSuccess(WIContext wiContext, AccessToken credentials, String passwordNew, WebPN webPN) throws UnknownHostException {
        // When the change password succeeds and roaming user is enabled and the primary WebPN is being used,
        // populate the access token with the user's group SIDs as these are required to determine home farm bindings.
        if (wiContext.getConfiguration().isRoamingUserEnabled()) {
            java.util.List webPNs = DisasterRecoveryUtils.getWebPNs(wiContext);

            if (webPNs.indexOf(webPN) == 0) {
                try {
                    // This should not normally fail since the password has just been successfully changed.
                    AccessTokenValidity atv = Authentication.validateAccessTokenWithAccountInfo(wiContext.getConfiguration(), webPN, (SIDBasedToken)credentials);
                    if (atv == null || !atv.getValidationResult().isSuccess()) {
                        handleError(MessageType.ERROR, "GeneralCredentialsFailure");
                        return false;
                    }
                } catch (SourceIOException sioe) {
                    handleError(MessageType.ERROR, "GeneralCredentialsFailure");
                    return false;
                }
            }
        }

        Map parameters = (Map)Authentication.getAuthenticationState(wiContext.getWebAbstraction()).getParameters();

        if(TwoFactorAuth.isPasswordIntegrationEnabled(wiContext.getConfiguration())) {
            // if change password succeeded, update the 2-factor cached password
            PasswordCache passwordCache = (PasswordCache)parameters.get("authenticator");
            String username = (String)parameters.get("User");
            try {
                if(passwordCache != null) {
                    passwordCache.setPassword(username, passwordNew);
                } else {
                    throw new AuthenticatorInitializationException("RSASetPasswordFailed");
                }
            } catch(AuthenticatorInitializationException pwException) {

                wiContext.log(MessageType.WARNING, pwException.getMessage());
            }
        }

        Authentication.getAuthenticationState(wiContext.getWebAbstraction()).setAuthenticated(credentials);

        // Create a UserContext to hold user-specific app state; this
        // is null if the creation failed, which indicates some horrible
        // problem has arisen somewhere.
        DisasterRecoveryUtils.setWebPN(wiContext, webPN);
        SessionUtils.createNewUserContext(wiContext);

        // Set the flag so that the authenticated site change password
        // page shows the confirmation section only, then add the
        // page to the initial post login stack, adding it after the wizard
        // or homepage means it will be displayed first

        wiContext.getWebAbstraction().setSessionAttribute(CHANGE_PASSWORD_CONFIRM, Constants.VAL_TRUE);
        PageHistory.addToPostLoginPages(wiContext.getWebAbstraction(), Constants.PAGE_CHANGE_PASSWD);

        // If there was a direct launch request pending
        // ensure that it is passed along to the site pages
        LaunchUtilities.transferLaunchDataToSession(wiContext);

        // User is logged in, so redirect them to the site page (default.aspx)
        Authentication.redirectToCurrentAuthPage(wiContext);

        return true;
    }
}