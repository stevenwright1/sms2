/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth.twofactor;

import java.io.IOException;
import java.util.Map;

import com.citrix.authentication.tokens.AccessToken;
import com.citrix.authenticators.AuthenticatorInitializationException;
import com.citrix.authenticators.ISecurIDAuthenticator;
import com.citrix.wi.mvc.StatusMessage;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pages.StandardPage;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.TwoFactorAuth;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wing.MessageType;

/**
 * This class processes a SecurID authentication
 * (when not using RADIUS).
 */
public class SecurID extends StandardPage {

    private final static StatusMessage SECURID_ERROR_STATUS = new StatusMessage(MessageType.ERROR, AbstractTwoFactorAuthLayout.VAL_GENERAL_AUTHENTICATION_ERROR, null, "SecurIDAuthenticatorError", null);

    public SecurID(WIContext wiContext) {
        super(wiContext);
    }

    protected boolean performImp() throws IOException {

        Map parameters = (Map)Authentication.getAuthenticationState(getWebAbstraction()).getParameters();

        AccessToken accessToken = (AccessToken)parameters.get(Authentication.VAL_ACCESS_TOKEN);

        String username = TwoFactorAuth.getUserName(accessToken, TwoFactorAuth.useTwoFactorFullyQualifiedUserNames(wiContext.getConfiguration()));
        String passcode = (String)parameters.get(TwoFactorAuth.VAL_PASSCODE);

        parameters.put(TwoFactorAuth.VAL_USER, username);

        try {

            ISecurIDAuthenticator authenticator = null;

            if (TwoFactorAuth.isPasswordIntegrationEnabled(wiContext.getConfiguration())) {
                authenticator = wiContext.getUtils().getNewPasswordCachingSecurIDAuthenticator();
            } else {
                authenticator = wiContext.getUtils().getNewSecurIDAuthenticator();
            }

            parameters.put(TwoFactorAuth.VAL_AUTHENTICATOR, authenticator);

            String result = authenticator.Authenticate(username, passcode);
            if( result.equals(TwoFactorAuth.SECURID_NEXT_TOKENCODE)) {
                    Authentication.addPageToQueueHead(wiContext, TwoFactorAuth.PAGE_NEXT_TOKENCODE, parameters);
                    Authentication.redirectToNextAuthPage(wiContext);
            } else if( result.equals(TwoFactorAuth.SECURID_CHANGE_PIN_USER)) {
                    Authentication.addPageToQueueHead(wiContext, TwoFactorAuth.PAGE_CHANGE_PIN_USER, parameters);
                    Authentication.redirectToNextAuthPage(wiContext);
            } else if( result.equals(TwoFactorAuth.SECURID_CHANGE_PIN_SYSTEM)) {
                    Authentication.addPageToQueueHead(wiContext, TwoFactorAuth.PAGE_CHANGE_PIN_WARNING, parameters);
                    Authentication.redirectToNextAuthPage(wiContext);
            } else if( result.equals(TwoFactorAuth.SECURID_CHANGE_PIN_EITHER)) {
                    Authentication.addPageToQueueHead(wiContext, TwoFactorAuth.PAGE_CHANGE_PIN_EITHER, parameters);
                    Authentication.redirectToNextAuthPage(wiContext);
            } else if( result.equals(TwoFactorAuth.SECURID_SUCCESS)){
                    // Authentication succeeded, if password integration is enabled,
                    // redirect to the get_password page to retrieve the cached password and
                    // update the AccessToken for use at MPS authentication-time (explicit.aspx).
                    if (TwoFactorAuth.isPasswordIntegrationEnabled(wiContext.getConfiguration())) {
                        Authentication.addPageToQueueHead(wiContext, TwoFactorAuth.PAGE_GET_PASSWORD, parameters);
                    }

                    Authentication.forwardToNextAuthPage(wiContext);
            } else if (result.equals(TwoFactorAuth.SECURID_INVALID)) {
                UIUtils.HandleLoginFailedMessage(wiContext, AbstractTwoFactorAuthLayout.INVALID_CREDENTIALS_STATUS);
            } else {
                UIUtils.HandleLoginFailedMessage(wiContext, SECURID_ERROR_STATUS);
            }

        } catch (AuthenticatorInitializationException aie) {
            UIUtils.HandleLoginFailedMessage(wiContext, MessageType.ERROR, AbstractTwoFactorAuthLayout.VAL_GENERAL_AUTHENTICATION_ERROR, "", aie.getMessage(), null);
        }

        return false;
    }
}
