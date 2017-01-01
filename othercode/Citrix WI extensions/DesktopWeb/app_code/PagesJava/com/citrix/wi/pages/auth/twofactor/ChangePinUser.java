/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth.twofactor;

import java.io.IOException;
import java.util.Map;

import com.citrix.authenticators.IRADIUSAuthenticator;
import com.citrix.authenticators.ISecurIDAuthenticator;
import com.citrix.authenticators.RADIUSAuthenticator;
import com.citrix.wi.controls.DialogPageControl;
import com.citrix.wi.mvc.ActionState;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Features;
import com.citrix.wi.pageutils.TwoFactorAuth;
import com.citrix.wing.MessageType;
import com.citrix.wing.util.Strings;

/**
 * This page is used to allow to choose a PIN.
 * It is used for SecurID (including when using RADIUS).
 */
public class ChangePinUser extends AbstractTwoFactorAuthLayout {

    protected DialogPageControl viewControl = new DialogPageControl();

    public ChangePinUser(WIContext wiContext) {
        super(wiContext);
        getWebAbstraction().setRequestContextAttribute(VAL_VIEW_CONTROL, viewControl);
    }

    /**
     * If cancel is pressed, return to the login page with an error.
     * If OK is pressed, check the PIN has been entered, and they both match.
     * Once these checks are complete, the change pin is attempted.
     */
    protected ActionState doPostAction(Object authenticator, Map parameters, boolean isOK) throws IOException {
        ActionState actionState = null;
        if (isOK) {
            // The user has submitted their new PIN (and a second, confirmation PIN)
            String pin1 = getWebAbstraction().getFormParameter(TwoFactorAuth.ID_PIN1);
            String pin2 = getWebAbstraction().getFormParameter(TwoFactorAuth.ID_PIN2);

            // Check the PIN and the confirmation PIN match up
            if (pin1 == null || "".equals(pin1)) {
                setFeedback(MessageType.ERROR, TwoFactorAuth.KEY_MUST_ENTER_PIN);
                actionState = new ActionState(true);

            } else if(pin1 != null && pin1.equals(pin2)) {

                // Check this is a valid pin for a SecurID Server (using RADIUS or not)
                if ((pin1.length() < 4) || (pin1.length() > 8) || (!Strings.isValidString(pin1, false, true, true, ""))) {
                    // let the user try again if we find the pin to be invalid
                    setFeedback(MessageType.ERROR, TwoFactorAuth.KEY_PIN_REJECTED);
                    actionState = new ActionState(true);
                } else {
                    // if it is a valid pin, then continue...
                    if (authenticator instanceof IRADIUSAuthenticator) {
                        actionState = doRadius((IRADIUSAuthenticator)authenticator, pin1);
                    } else if (Features.isSecurIDSupported() && authenticator instanceof ISecurIDAuthenticator) {
                        actionState = doSecurID((ISecurIDAuthenticator)authenticator, pin1);
                    } else {
                        throw new IllegalArgumentException("Must be passed either a RADIUS or SecurID Authenticator");
                    }
                }

            } else {
                // pins do not match - have them try again
                setFeedback(MessageType.ERROR, TwoFactorAuth.KEY_PIN_NO_MATCH);
                actionState = new ActionState(true);
            }
        } else {
            if (Features.isSecurIDSupported() && authenticator instanceof ISecurIDAuthenticator) {
                ((ISecurIDAuthenticator)authenticator).Dispose();
            }
            actionState = new ActionState(PIN_NOT_CHANGED_STATUS);
        }
        return actionState;
    }

    /**
     * Set up the page controls to display the page
     */
    protected void doShowFormActions() {
        welcomeControl.setTitle(wiContext.getString("ScreenTitleChangePIN"));
        welcomeControl.setBody(wiContext.getString("NewUserPINText2"));
        layoutControl.formAction = Constants.FORM_POSTBACK;
    }

    protected String getBrowserPageTitleKey()
    {
        return "BrowserTitleChangePIN";
    }

    /**
     * Perform the change pin, then redirect to the login page
     * with the appropriate message
     */
    private ActionState doRadius(IRADIUSAuthenticator authenticator, String pin1) throws IOException {
        ActionState actionState = null;
        // Attempt to change the PIN
        String result = authenticator.authenticate(pin1);
        // Tell user if the PIN has been changed or not
        if (RADIUSAuthenticator.RET_SUCCESS.equalsIgnoreCase(result)) {
            // user must reauthenticate so back to login page
            actionState = new ActionState(PIN_CHANGED_STATUS);
        } else {
            // the pin was rejected by the system, go back to login page
            actionState = new ActionState(PIN_REJECTED_STATUS);
        }
        return actionState;
    }

    /**
     * Perform the change pin, then redirect to the login page
     * with the appropriate message
     */
    private ActionState doSecurID(ISecurIDAuthenticator authenticator, String pin1) throws java.io.IOException {
        ActionState actionState = null;
        String result = authenticator.ChangePIN(pin1);
        if (ISecurIDAuthenticator.RET_SUCCESS.equals(result)) {
            // user must reauthenticate so back to login page
            actionState = new ActionState(PIN_CHANGED_STATUS);
        } else {
            // the pin was rejected by the system, go back to login page
            actionState = new ActionState(PIN_REJECTED_STATUS);
        }
        return actionState;
    }
}
