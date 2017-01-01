/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth.twofactor;

import java.io.IOException;
import java.util.Map;

import com.citrix.authentication.tokens.AccessToken;
import com.citrix.authentication.tokens.ChangeablePasswordBasedAccessToken;
import com.citrix.authentication.tokens.PasswordBasedToken;
import com.citrix.authentication.web.AuthenticationState;
import com.citrix.authenticators.AuthenticatorInitializationException;
import com.citrix.authenticators.PasswordCache;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pages.StandardPage;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.TwoFactorAuth;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wing.MessageType;

/**
 * This page is used to get hold of the password
 * when SecurID password integration is used.
 */
public class GetPassword extends StandardPage {

    public GetPassword(WIContext wiContext) {
        super(wiContext);
    }

    protected boolean performImp() throws IOException {

        // If 'Password Integration' is configured, and we don't have a password yet
        // try and ascertain the password from the password cache
        if(TwoFactorAuth.isPasswordIntegrationEnabled(wiContext.getConfiguration())) {
            WebAbstraction web = wiContext.getWebAbstraction();
            AuthenticationState authState = Authentication.getAuthenticationState(web);
            Map parameters = (Map) authState.getParameters();
            AccessToken credentials = (AccessToken)parameters.get(Authentication.VAL_ACCESS_TOKEN);

            PasswordBasedToken passwordToken = (PasswordBasedToken) credentials;
            if(passwordToken != null) {

                //If we don't already have a password, look it up in the PasswordCache
                if(passwordToken.getPassword() == null || passwordToken.getPassword().length() < 1) {

                    MessageType messageType = null;
                    String messageKey = "";

                    String password = "";

                    try {
                        PasswordCache passwordCache = (PasswordCache) parameters.get(TwoFactorAuth.VAL_AUTHENTICATOR);
                        String username = (String)parameters.get(TwoFactorAuth.VAL_USER);

                        if(passwordCache != null) {
                            password = passwordCache.getPassword(username);
                        } else {
                            throw new AuthenticatorInitializationException("RSAGetPasswordFailed");
                        }

                        if (password == null || password.length() < 1) {
                            password = "";
                            messageType = MessageType.INFORMATION;
                            messageKey = "PasswordChallengeReasonFirstTime";
                        }
                    } catch (AuthenticatorInitializationException pwException) {
                        // The 'GetPassword' call failed - log a warning, but continue
                        // execution so that the user is taken to the 'password challenge'
                        // page.
                        wiContext.log(MessageType.ERROR, pwException.getMessage() );
                        password = "";
                        messageType = MessageType.WARNING;
                        messageKey = "PasswordChallengeReasonLookupFailure";
                    }

                    if(password.length() < 1) { // could not lookup password, redirect to password challenge page.
                        Authentication.addPageToQueueHead(wiContext, TwoFactorAuth.PAGE_PASSWORD_CHALLENGE, parameters);
                        authState.pageCompleted();
                        web.clientRedirectToUrl(authState.getCurrentPage() + UIUtils.getMessageQueryStr(messageType, messageKey));
                    } else { //lookup was a success, update the access token
                        ChangeablePasswordBasedAccessToken changeableToken = (ChangeablePasswordBasedAccessToken) credentials;
                        if(changeableToken != null) {
                            changeableToken.setSecret(password);
                        }
                    }
                }
            }  else { //We don't have a PasswordBasedToken
                throw new IllegalArgumentException("get_password.aspx requires a PasswordBasedToken.");
            }

            // We're done here, move onto the next step, which will be to perform
            // the explicit authentication.
            Authentication.redirectToNextAuthPage(wiContext);

        } else {
            throw new IllegalArgumentException("get_password.aspx requires that password integration is configured.");
        }

        return false;
    }
}
