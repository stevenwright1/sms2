/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth.twofactor;

import java.io.IOException;
import java.util.Map;

import com.citrix.authentication.web.AuthenticationState;
import com.citrix.authenticators.IRADIUSAuthenticator;
import com.citrix.authenticators.ISecurIDAuthenticator;
import com.citrix.authenticators.RADIUSAuthenticator;
import com.citrix.wi.controls.DialogPageControl;
import com.citrix.wi.mvc.ActionState;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Features;
import com.citrix.wi.pageutils.TwoFactorAuth;
import com.citrix.wing.MessageType;

/**
 * This page is used for SecurID (including when using RADIUS)
 * and is used to ask the user for their next token code, when
 * required by the RSA SecurID server.
 */
public class NextTokencode extends AbstractTwoFactorAuthLayout {

    protected DialogPageControl viewControl = new DialogPageControl();

    public NextTokencode(WIContext wiContext) {
        super(wiContext);
        wiContext.getWebAbstraction().setRequestContextAttribute(VAL_VIEW_CONTROL, viewControl);
    }

    /**
     * If OK was clicked, and the user entered a tokencode, process the code.
     * If the tokencode wasn't entered, tell the user to enter the code.
     * If the user clicked on Cancel, then got to the login page.
     */
    protected ActionState doPostAction(Object authenticator, Map parameters, boolean isOK) throws IOException {
        ActionState actionState = null;
        if (isOK) {

            String tokencode = wiContext.getWebAbstraction().getFormParameter(TwoFactorAuth.ID_TOKENCODE);
            if (tokencode != null && !tokencode.trim().equals("")) {

                if (authenticator instanceof IRADIUSAuthenticator) {
                    actionState = doRadiusAuth((IRADIUSAuthenticator)authenticator, tokencode);
                } else if (Features.isSecurIDSupported() && authenticator instanceof ISecurIDAuthenticator) {
                    actionState = doSecurIDAuth((ISecurIDAuthenticator)authenticator, tokencode, parameters);
                } else {
                    throw new IllegalArgumentException("Must be passed either a RADIUS or SecurID Authenticator");
                }

            } else {
                setFeedback(MessageType.ERROR, TwoFactorAuth.KEY_MUST_ENTER_TOKENCODE);
                actionState = new ActionState(true);
            }

        } else {
            if (Features.isSecurIDSupported() && authenticator instanceof ISecurIDAuthenticator) {
                ((ISecurIDAuthenticator)authenticator).Dispose();
            }
            // they cancelled - back to login with an error indicating that authentication was not possible
            actionState = new ActionState(GENERAL_CREDENTIALS_FAILURE_STATUS);
        }
        return actionState;
    }

    /**
     * Setup the GUI
     */
    protected void doShowFormActions() throws IOException {
        welcomeControl.setTitle(wiContext.getString("ScreenTitleNextToken"));
        welcomeControl.setBody(wiContext.getString("NextTokenText2"));
        layoutControl.formAction = Constants.FORM_POSTBACK;
    }

    protected String getBrowserPageTitleKey()
    {
        return "BrowserTitleNextTokencode";
    }

    /**
     * This method sends the recieved token code.
     * If successful, then possibly deal with password integration.
     */
    private ActionState doSecurIDAuth(ISecurIDAuthenticator authenticator, String tokencode, Map parameters) throws IOException {
        ActionState actionState = null;
        String result = ISecurIDAuthenticator.RET_FAILED;

        if (authenticator != null) {
            result = authenticator.NextTokencode(tokencode.trim());
        }

        if (ISecurIDAuthenticator.RET_SUCCESS.equals(result)) {
            AuthenticationState authState = Authentication.getAuthenticationState(wiContext.getWebAbstraction());

            // Authentication succeeded, if password integration is enabled,
            // redirect to the get_password page to retrieve the cached password and
            // update the AccessToken for use at MPS authentication-time (explicit.aspx).
            if (TwoFactorAuth.isPasswordIntegrationEnabled(wiContext.getConfiguration())) {
                authState.addPageToQueueHead(TwoFactorAuth.PAGE_GET_PASSWORD, parameters);
            }

            Authentication.forwardToNextAuthPage(wiContext);
            actionState = new ActionState(false);

        } else {
            actionState = new ActionState(INVALID_CREDENTIALS_STATUS);
        }

        return actionState;
    }

    /**
     * Send the RADIUS server the users token code.
     * Inform the user if it fails, else to to the next authenication page
     */
    private ActionState doRadiusAuth(IRADIUSAuthenticator authenticator, String tokencode) throws IOException {
        ActionState actionState = null;

        // Give it to the RADIUS server
        String result = authenticator.authenticate(tokencode.trim());

        // authentication success
        if (RADIUSAuthenticator.RET_SUCCESS.equalsIgnoreCase(result)) {
            // success, so go to next auth page
            Authentication.redirectToNextAuthPage(wiContext);
            actionState = new ActionState(false);
        } else if (RADIUSAuthenticator.RET_FAILED.equalsIgnoreCase(result)) {
            // system failure, so log and return to login page
            actionState = new ActionState(GENERAL_RADIUS_ERROR_STATUS);
        } else {
            // invalid tokencode
            actionState = new ActionState(INVALID_CREDENTIALS_STATUS);
        }

        return actionState;
    }
}
