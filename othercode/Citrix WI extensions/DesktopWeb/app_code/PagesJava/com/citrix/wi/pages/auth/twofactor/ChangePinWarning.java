/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth.twofactor;

import java.io.IOException;
import java.util.HashMap;
import java.util.Map;
import java.util.StringTokenizer;

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
 * This page gives the user the option of whether they wish to get
 * a system generated PIN or not. If success, then they are asked to accept
 * the PIN in the ChangePinSystem page.
 */
public class ChangePinWarning extends AbstractTwoFactorAuthLayout {

    protected DialogPageControl viewControl = new DialogPageControl();

    public ChangePinWarning(WIContext wiContext) {
        super(wiContext);
        getWebAbstraction().setRequestContextAttribute(VAL_VIEW_CONTROL, viewControl);
    }

    /**
     * Call the appropritate method to process the request
     */
    protected ActionState doPostAction(Object authenticator, Map parameters, boolean isOK) throws IOException {
        ActionState actionState = null;
        if (isOK) {

            // The user has accepted the warning that their system-generated PIN is
            // about to be displayed
            if (authenticator instanceof IRADIUSAuthenticator) {
                actionState = doRadiusOK((IRADIUSAuthenticator)authenticator);
            } else if (Features.isSecurIDSupported() && authenticator instanceof ISecurIDAuthenticator) {
                actionState = doSecurIDOK((ISecurIDAuthenticator)authenticator);
            } else {
                throw new IllegalArgumentException("Must be passed either a RADIUS or SecurID Authenticator");
            }

        } else {

            // They cancelled
            if (authenticator instanceof IRADIUSAuthenticator) {
                actionState = doRadiusCancel((IRADIUSAuthenticator)authenticator);
            } else if (Features.isSecurIDSupported() && authenticator instanceof ISecurIDAuthenticator) {
                actionState = doSecurIDCancel((ISecurIDAuthenticator)authenticator);

            } else {
                throw new IllegalArgumentException("Must be passed either a RADIUS or SecurID Authenticator");
            }
        }
        return actionState;
    }

    /**
     * Setup the page controls
     */
    protected void doShowFormActions() {
        welcomeControl.setTitle(wiContext.getString("ScreenTitleChangePIN"));
        layoutControl.formAction = Constants.FORM_POSTBACK;
    }

    protected String getBrowserPageTitleKey()
    {
        return "BrowserTitleChangePIN";
    }

    /**
     * The user has said they want to see their pin, so try and get the pin.
     * If we get the pin, then redirect to show the pin, otherwise error out.
     */
    private ActionState doRadiusOK(IRADIUSAuthenticator authenticator) throws IOException {
        // The user has accepted the warning that their system-generated PIN is
        // about to be displayed
        String result = authenticator.authenticate(TwoFactorAuth.RADIUS_YES);

        // so try and extract the new PIN
        String pin = null;

        try {
            pin = extractPinFromChallenge(result);
        } catch (IllegalArgumentException iae) {
            pin = null; // 'result' did not contain a system-pin
        }

        ActionState actionState = null;
        // No PIN was supplied/generated - system failure - kill the session and back to login
        if (pin == null) {
            actionState = new ActionState(GENERAL_RADIUS_ERROR_STATUS);
        } else {
            // we have a PIN, store it and move to the System PIN authentication stage
            Map systemPinParameters = new HashMap();
            systemPinParameters.put(TwoFactorAuth.VAL_AUTHENTICATOR, authenticator);
            systemPinParameters.put(TwoFactorAuth.VAL_SYSTEM_PIN, pin);

            Authentication.addPageToQueueHead(wiContext, TwoFactorAuth.PAGE_CHANGE_PIN_SYSTEM, systemPinParameters);
            Authentication.redirectToNextAuthPage(wiContext);
            actionState = new ActionState(false);
        }
        return actionState;
    }

    /**
     * They have decided they don't want a System pin after all,
     * so tell RADIUS and tell the user they havn't changed their pin.
     */
    private ActionState doRadiusCancel(IRADIUSAuthenticator authenticator) throws IOException {
        authenticator.authenticate(TwoFactorAuth.RADIUS_NO);
        return new ActionState(PIN_NOT_CHANGED_STATUS);
    }

    private ActionState doSecurIDOK(ISecurIDAuthenticator authenticator) throws IOException {
        String pin = authenticator.GetSystemPIN();

        Map systemPinParameters = new HashMap();
        systemPinParameters.put(TwoFactorAuth.VAL_AUTHENTICATOR, authenticator);
        systemPinParameters.put(TwoFactorAuth.VAL_SYSTEM_PIN, pin);

        Authentication.addPageToQueueHead(wiContext, TwoFactorAuth.PAGE_CHANGE_PIN_SYSTEM, systemPinParameters);
        Authentication.redirectToNextAuthPage(wiContext);
        return new ActionState(false);
    }

    /**
     * They have decided against getting a new system pin,
     * so redirect to the login page.
     */
    private ActionState doSecurIDCancel(ISecurIDAuthenticator authenticator) throws java.io.IOException {
        authenticator.Dispose();
        return new ActionState(PIN_NOT_CHANGED_STATUS);
    }

    /**
      * Given a String of the form 'str[n]' this method returns the
      * string 'n'. This is used to extract the system - generated PIN
      * from a RADIUS challenge of the form 'CHANGE_PIN_SYSTEM_[N]'where
      * N is the system-generated PIN.
      *
      * @param challenge The challenge string of the form 'str[n]'
      * @return 'n', or null if '[n]' was not found.
      * @throw IllegalArgumentException if the challenge parameter did not contain
      * a system - generated PIN.
      */
    private String extractPinFromChallenge(String challenge) throws IllegalArgumentException {
        if (challenge == null || challenge.length() < 1 || !(challenge.toUpperCase().startsWith("CHANGE_PIN_SYSTEM_["))) {
            throw new IllegalArgumentException();
        }

        String pin = "";
        StringTokenizer st = new StringTokenizer(challenge, "[]", false);

        // the pin should be the 2nd token
        for (int i = 0; i < 2; i++) {
            if (st.hasMoreTokens()) {
                String result = st.nextToken();
                if (i == 1) {
                    if (result != null && result.length() > 1) {
                        pin = result;
                    }
                }
            }
        }
        if (pin == null || pin.length() < 1) {
            throw new IllegalArgumentException();
        }

        return pin;
    }
}