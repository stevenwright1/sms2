/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth.twofactor;

import java.io.IOException;
import java.util.HashMap;
import java.util.Map;

import com.citrix.authentication.tokens.AccessToken;
import com.citrix.authenticators.AuthenticatorInitializationException;
import com.citrix.authenticators.RADIUSAuthenticator;
import com.citrix.authenticators.RADIUSAuthenticatorFactory;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pages.StandardPage;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.LaunchUtilities;
import com.citrix.wi.pageutils.TwoFactorAuth;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wing.MessageType;

/**
 *
 * This class processes a RADIUS authentication.
 *
 * The outcome of the authentication can be one of the following, returned by
 * the RADIUSAuthenticator :
 *
 * RET_SUCCESS - The authentication succeeded, the user will be directed to the
 * next stage of the authentication process, which will be explicit
 * authentication against the Presentation server.
 *
 * RET_INVALID - The wrong username or passcode was supplied, and so the
 * authentication fails and the user is presented with the appropriate error.
 *
 * RET_FAILED - A system error occurred trying to authenticate, and so the
 * authentication fails and the user is presented with the appropriate error.
 *
 * NEXT_TOKENCODE - The RADIUS server represents an RSA SecurID system, which
 * has been configured to return this challenge when the user is required to
 * provide the next tokencode that appears on their token. The user is directed
 * to the next_tokencode.jsp page.
 *
 * CHANGE_PIN_USER - The RADIUS server represents an RSA SecurID system, which
 * has been configured to return this challenge when the user is required to
 * provide a new PIN. The user is directed to the change_pin_user.jsp page.
 *
 * SYSTEM_PIN_READY - The RADIUS server represents an RSA SecurID system, which
 * has been configured to return this challenge when the server wants supply the
 * user with a new PIN. The user is directed to the change_pin_warning.jsp page.
 *
 * CHANGE_PIN_EITHER - The RADIUS server represents an RSA SecurID system which
 * has been configured to return this challenge when the user is required to
 * choose whether to provide a new PIN for themself, or have the system generate
 * one for them. The user is directed to the change_pin_either.jsp page.
 *
 * If some other value is returned, it is assumed to be a RADIUS challenge from
 * a server that has not been configured to provide RSA SecurID challenges that
 * are well-known to Web Interface. In this case, the challenge is stored for
 * display in the challenge page, to which the authentication process is
 * directed.
 */
public class RADIUS extends StandardPage {

    public RADIUS(WIContext wiContext) {
        super(wiContext);
    }

    protected boolean performImp() throws IOException {
        Map parameters = (Map)Authentication.getAuthenticationState(wiContext.getWebAbstraction()).getParameters();
        AccessToken accessToken = (AccessToken)parameters.get(Authentication.VAL_ACCESS_TOKEN);
        String username = TwoFactorAuth.getUserName(accessToken, TwoFactorAuth
                        .useTwoFactorFullyQualifiedUserNames(wiContext.getConfiguration()));
        String passcode = (String)parameters.get(TwoFactorAuth.VAL_PASSCODE);

        RADIUSAuthenticator authenticator = null;
        try {
            // Get hold of a RADIUS Authenticator for this user
            RADIUSAuthenticatorFactory factory = (RADIUSAuthenticatorFactory)wiContext.getWebAbstraction()
                            .getApplicationAttribute(TwoFactorAuth.VAL_RADIUS_AUTHENTICATOR_FACTORY);
            authenticator = factory.createAuthenticator(username);

            // Attempt to authenticate the user with the specified Passcode.
            performAuthentication(passcode, authenticator);

        } catch (AuthenticatorInitializationException aie) {
            // we could not get an authenticator -- fail the authentication
            UIUtils.HandleLoginFailedMessage(wiContext, MessageType.ERROR,
                            AbstractTwoFactorAuthLayout.VAL_GENERAL_AUTHENTICATION_ERROR, null, aie.getMessage(), null);
        }

        return false;
    }

    /**
     * @param passcode
     * @param authenticator
     * @throws IOException
     */
    private void performAuthentication(String passcode, RADIUSAuthenticator authenticator) throws IOException {
        if (authenticator != null) {
            // perform authentication
            String result = authenticator.authenticate(passcode);

            // store result for futher pages
            Map authenticationParameters = new HashMap();
            authenticationParameters.put(TwoFactorAuth.VAL_AUTHENTICATOR, authenticator);
            authenticationParameters.put(TwoFactorAuth.VAL_CHALLENGE, result);

            // redirect to the approprate page given the result
            processAuthenticatorResult(result, authenticationParameters);
        } else {
            // we need an authenticator to proceed
            UIUtils.HandleLoginFailedMessage(wiContext, MessageType.ERROR,
                            AbstractTwoFactorAuthLayout.VAL_GENERAL_AUTHENTICATION_ERROR);
        }
    }

    /**
     * Redirects to the correct page given the authenticator result
     *
     * @param authenticator
     * @param result
     * @throws IOException
     */
    private void processAuthenticatorResult(String result, Object authenticationParameters) throws IOException {
        if (RADIUSAuthenticator.RET_SUCCESS.equalsIgnoreCase(result)) {
            // authentication succeeded - move onto the next stage in the
            // authentication state
            LaunchUtilities.transferLaunchDataToSession(wiContext);
            Authentication.redirectToNextAuthPage(wiContext);

        } else if (RADIUSAuthenticator.RET_INVALID.equalsIgnoreCase(result)) {
            // authentication failed - kill session and back to login page
            UIUtils.HandleLoginFailedMessage(wiContext, AbstractTwoFactorAuthLayout.INVALID_CREDENTIALS_STATUS);

        } else if (result == null || RADIUSAuthenticator.RET_FAILED.equalsIgnoreCase(result)) {
            // system failure - kill session and back to login page
            UIUtils.HandleLoginFailedMessage(wiContext, AbstractTwoFactorAuthLayout.GENERAL_RADIUS_ERROR_STATUS);

        } else if (TwoFactorAuth.RADIUS_NEXT_TOKENCODE.equalsIgnoreCase(result)) {
            // If the response was not a success or fail, check for well-known
            // RSA SecurID challenges (this requires some custom RSA RADIUS
            // server configuration to update challenge strings to something
            // that we recognise).

            // SecurID wants the user to supply the next token code that is
            // displayed on their token - go to the next_tokencode page
            Authentication.redirectToAuthPage(wiContext, TwoFactorAuth.PAGE_NEXT_TOKENCODE, authenticationParameters);

        } else if (TwoFactorAuth.RADIUS_CHANGE_PIN_USER.equalsIgnoreCase(result)) {
            // SecurID wants the user to supply a new PIN - go to the
            // change_pin_user page
            Authentication.redirectToAuthPage(wiContext, TwoFactorAuth.PAGE_CHANGE_PIN_USER, authenticationParameters);

        } else if (TwoFactorAuth.RADIUS_SYSTEM_PIN_READY.equalsIgnoreCase(result)) {
            // SecurID wants to supply a new PIN - go to the
            // change_pin_warning page
            Authentication.redirectToAuthPage(wiContext, TwoFactorAuth.PAGE_CHANGE_PIN_WARNING, authenticationParameters);

        } else if (TwoFactorAuth.RADIUS_CHANGE_PIN_EITHER.equalsIgnoreCase(result)) {
            // SecurID wants the user to change their PIN, but needs to know
            // if the user wants to choose the new PIN themselves or whether
            // they want the SecurID server to generate one for them - go to the
            // change_pin_either page so the user can decide what to do
            Authentication.redirectToAuthPage(wiContext, TwoFactorAuth.PAGE_CHANGE_PIN_EITHER, authenticationParameters);

        } else if (result != null) {
            // We received an unrecognised RADIUS Challenge, redirect to the
            // challenge page, supplying the challenge string to be displayed.
            Authentication.redirectToAuthPage(wiContext, TwoFactorAuth.PAGE_CHALLENGE, authenticationParameters);

        }
    }
}
