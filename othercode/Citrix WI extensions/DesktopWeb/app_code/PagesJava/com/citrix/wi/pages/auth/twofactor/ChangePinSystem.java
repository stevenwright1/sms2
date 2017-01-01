/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth.twofactor;

import java.io.IOException;
import java.util.Map;

import com.citrix.authenticators.IRADIUSAuthenticator;
import com.citrix.authenticators.ISecurIDAuthenticator;
import com.citrix.authenticators.RADIUSAuthenticator;
import com.citrix.wi.controls.ChangePinSystemPageControl;
import com.citrix.wi.mvc.ActionState;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Features;
import com.citrix.wi.pageutils.TwoFactorAuth;

/**
 * This page gives the user their system generated PIN
 * and then asks the user if they accept the new PIN,
 * informing the SecurID server appropriatly.
 * This page can be accessed by a SecurID server using RADIUS with
 * known challenge values.
 */
public class ChangePinSystem extends AbstractTwoFactorAuthLayout {

    protected ChangePinSystemPageControl viewControl = new ChangePinSystemPageControl();

    public ChangePinSystem(WIContext wiContext) {
        super(wiContext);
        getWebAbstraction().setRequestContextAttribute(VAL_VIEW_CONTROL, viewControl);
    }

    /**
     * Calls the appropriate method to handle the post.
     */
    protected ActionState doPostAction(Object authenticator, Map parameters, boolean isOK) throws IOException {
        ActionState result = null;
        if (authenticator instanceof IRADIUSAuthenticator) {
            IRADIUSAuthenticator iRadiusAuthenticator = (IRADIUSAuthenticator)authenticator;
            if (isOK) {
                result = doRadiusOK(iRadiusAuthenticator);
            } else {
                result = doRadiusCancel(iRadiusAuthenticator);
            }
        } else if (Features.isSecurIDSupported() && authenticator instanceof ISecurIDAuthenticator) {
            ISecurIDAuthenticator iSecurIDAuthenticator = (ISecurIDAuthenticator)authenticator;
            if (isOK) {
                result = doSecurIDOK(iSecurIDAuthenticator, (String)parameters.get(TwoFactorAuth.VAL_SYSTEM_PIN));
            } else {
                result = doSecurIDCancel(iSecurIDAuthenticator);
            }
        } else {
            // this should have been passed an authenticator
            throw new IllegalArgumentException("Must be passed either a RADIUS or SecurID Authenticator");
        }
        return result;
    }

    /**
     * Populate the view state with the system generated PIN
     */
    protected ActionState doGetAction(Object authenticator, Map parameters) {
        viewControl.setSystemPin((String)parameters.get(TwoFactorAuth.VAL_SYSTEM_PIN));
        return new ActionState(true);
    }

    /**
     * Setup the GUI if it needs to be shown.
     */
    protected void doShowFormActions() {
        welcomeControl.setTitle(wiContext.getString("ScreenTitleChangePIN"));
        welcomeControl.setBody(wiContext.getString("PINChangingMessage2"));
        layoutControl.formAction = Constants.FORM_POSTBACK;
    }

    protected String getBrowserPageTitleKey()
    {
        return "BrowserTitleChangePIN";
    }

    /**
     * Try and accept the system PIN. Report to the user the outcome
     */
    private ActionState doRadiusOK(IRADIUSAuthenticator authenticator) throws java.io.IOException {
        // The user chose to accept the system-generated PIN, so let the RADIUS server know.
        String result = authenticator.authenticate(TwoFactorAuth.RADIUS_YES);

        // Report to the user if the PIN has actually changed correctly or not.
        if (RADIUSAuthenticator.RET_SUCCESS.equalsIgnoreCase(result)) {
            // success, user must reathenticate
            return new ActionState(PIN_CHANGED_STATUS);
        } else {
            // should always return success, warn user if not
            return new ActionState(GENERAL_RADIUS_ERROR_STATUS);
        }
    }

    /**
     * Tell radius the user has reject the pin, and confirm this to the user
     */
    private ActionState doRadiusCancel(IRADIUSAuthenticator authenticator) throws java.io.IOException {
        // The user chose not to accept the system-generated PIN, so let the
        // RADIUS server know and fail the authentication.
        authenticator.authenticate(TwoFactorAuth.RADIUS_NO);
        return new ActionState(PIN_NOT_CHANGED_STATUS);
    }

    /**
     * Try and change to the new system generated PIN. Report to the user the outcome
     */
    private ActionState doSecurIDOK(ISecurIDAuthenticator authenticator, String pin) throws java.io.IOException {
        if (ISecurIDAuthenticator.RET_SUCCESS.equalsIgnoreCase(authenticator.ChangePIN(pin))) {
            // user must reauthenticate
            return new ActionState(PIN_CHANGED_STATUS);
        } else {
            // the system rejected the pin it generated
            return new ActionState(PIN_REJECTED_STATUS);
        }
    }

    /**
     * Don't change to the new pin if the user rejects it.
     */
    private ActionState doSecurIDCancel(ISecurIDAuthenticator authenticator) throws java.io.IOException {
        authenticator.Dispose();
        return new ActionState(PIN_NOT_CHANGED_STATUS);
    }
}