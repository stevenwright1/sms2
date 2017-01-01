/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth.twofactor;

import java.io.IOException;
import java.util.HashMap;
import java.util.Map;

import com.citrix.authenticators.IRADIUSAuthenticator;
import com.citrix.authenticators.ISecurIDAuthenticator;
import com.citrix.wi.controls.DialogPageControl;
import com.citrix.wi.mvc.ActionState;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Features;
import com.citrix.wi.pageutils.TwoFactorAuth;

/**
 * This page is used for SecurID Authentication.
 * This includes when SecurID uses RADIUS and is configured with well-known challenge values.
 * It asks the user if they want a system generated pin, or pick one themselves.
 * You can get to this page from the RADIUS page.
 */
public class ChangePinEither extends AbstractTwoFactorAuthLayout {

    protected DialogPageControl viewControl = new DialogPageControl();

    public ChangePinEither(WIContext wiContext) {
        super(wiContext);
        getWebAbstraction().setRequestContextAttribute(VAL_VIEW_CONTROL, viewControl);
    }

    /**
     * If OK has been clicked, process the user's response.
     * If OK wasn't clicked, then end this change pin attempts.
     */
    protected ActionState doPostAction(Object authenticator, Map parameters, boolean isOK) throws IOException {
        ActionState result = null;
        if (isOK) {
            // The user has submitted their choice
            String userResponse = getWebAbstraction().getFormParameter(TwoFactorAuth.ID_PIN_TYPE);
            if (authenticator instanceof IRADIUSAuthenticator) {
                result = doRadius((IRADIUSAuthenticator)authenticator, userResponse);
            } else if (Features.isSecurIDSupported() && authenticator instanceof ISecurIDAuthenticator) {
                result = doSecurID((ISecurIDAuthenticator)authenticator, userResponse);
            } else {
                throw new IllegalArgumentException("Must be passed either a RADIUS or SecurID Authenticator");
            }
        } else {
            // they cancelled
            if (Features.isSecurIDSupported() && authenticator instanceof ISecurIDAuthenticator) {
                ((ISecurIDAuthenticator)authenticator).Dispose();
            }
            result = new ActionState(PIN_NOT_CHANGED_STATUS);
        }
        return result;
    }

    /**
     * Redirect to the appropriate page, given whether the user
     * wanted a system generated pin or not.
     */
    private ActionState doSecurID(ISecurIDAuthenticator authenticator, String userResponse) {
        // Set up parameters for page queue
        Map parameters = new HashMap();
        parameters.put(TwoFactorAuth.VAL_AUTHENTICATOR, authenticator);

        if (Constants.VAL_SYSTEM.equalsIgnoreCase(userResponse)) {
            // Option of system generated PIN
            Authentication.addPageToQueueHead(wiContext, TwoFactorAuth.PAGE_CHANGE_PIN_WARNING, parameters);
        } else {
            // User chosen PIN
            Authentication.addPageToQueueHead(wiContext, TwoFactorAuth.PAGE_CHANGE_PIN_USER, parameters);
        }
        Authentication.redirectToNextAuthPage(wiContext);
        return new ActionState(false);
    }

    /**
     * Given the users response, this replies appropriately to the RADIUS
     * server, and then forwards to the appropriate page.
     */
    private ActionState doRadius(IRADIUSAuthenticator authenticator, String userResponse) throws java.io.IOException {
        ActionState actionState = null;
        // The semantics of the underlying RADIUS challenge is actually asking
        // whether we want a system PIN. Replying 'n' means the user wants
        // to supply their own PIN, replying 'y' means the user wants generated.
        if (Constants.VAL_USER.equals(userResponse)) {

            // Tell radius choose are own PIN and check response
            String result = authenticator.authenticate(TwoFactorAuth.RADIUS_NO);
            if (TwoFactorAuth.RADIUS_CHANGE_PIN_USER.equalsIgnoreCase(result)) {
                // Success, so go to next page
                Map userPinParameters = new HashMap();
                userPinParameters.put(TwoFactorAuth.VAL_AUTHENTICATOR, authenticator);
                Authentication.addPageToQueueHead(wiContext, TwoFactorAuth.PAGE_CHANGE_PIN_USER, userPinParameters);
                Authentication.redirectToNextAuthPage(wiContext);
                actionState = new ActionState(false);
            } else {
                // An error occured if not asked for user pin
                actionState = new ActionState(GENERAL_RADIUS_ERROR_STATUS);
            }

        } else if (Constants.VAL_SYSTEM.equals(userResponse)) {

            // Go to the warning page which may send a 'y' to RADIUS
            Map changePinWarningParameters = new HashMap();
            changePinWarningParameters.put(TwoFactorAuth.VAL_AUTHENTICATOR, authenticator);
            Authentication.addPageToQueueHead(wiContext, TwoFactorAuth.PAGE_CHANGE_PIN_WARNING, changePinWarningParameters);
            Authentication.redirectToNextAuthPage(wiContext);
            actionState = new ActionState(false);

        } else {
            // they did not choose system or user, which is invalid - cancel the operation
            actionState = new ActionState(PIN_NOT_CHANGED_STATUS);
        }
        return actionState;
    }

    /**
     * Setup the GUI if it is going to be displayed
     */
    protected void doShowFormActions() {
        welcomeControl.setTitle(wiContext.getString("ScreenTitleChangePIN"));
        welcomeControl.setBody(wiContext.getString("PINEither"));
        layoutControl.formAction = Constants.FORM_POSTBACK;
    }

    protected String getBrowserPageTitleKey()
    {
        return "BrowserTitleChangePIN";
    }
}
