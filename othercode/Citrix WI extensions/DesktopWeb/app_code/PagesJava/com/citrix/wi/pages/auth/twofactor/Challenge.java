/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth.twofactor;

import java.io.IOException;
import java.util.HashMap;
import java.util.Map;

import com.citrix.authenticators.RADIUSAuthenticator;
import com.citrix.wi.controls.ChallengePageControl;
import com.citrix.wi.mvc.ActionState;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.TwoFactorAuth;
import com.citrix.wing.util.WebUtilities;

/**
 * This page is used to give a users and arbitary challenge during RADIUS authentication.
 * NB:
 * The challenge page is only invoked if Web Interface does not recognise the
 * challenge; it is possible to configure RSA SecurID to send challenges that
 * are recognised by Web Interface (see the Web Interface
 * documentation for more information).
 */
public class Challenge extends AbstractTwoFactorAuthLayout {

    protected ChallengePageControl viewControl = new ChallengePageControl();

    public Challenge(WIContext wiContext) {
        super(wiContext);
        getWebAbstraction().setRequestContextAttribute(VAL_VIEW_CONTROL, viewControl);
    }

    /**
     * If the OK button was clicked, this sends the user response to the RADIUS server,
     * and then takes the appropriate action given the response from the RADIUS server.
     * If they click Cancel, then go back the the login page with an error.
     */
    protected ActionState doPostAction(Object authenticator, Map parameters, boolean isOK) throws IOException {
        // Get the correct authenticator
        RADIUSAuthenticator radiusAuthenticator = null;
        if (authenticator instanceof RADIUSAuthenticator) {
            radiusAuthenticator = (RADIUSAuthenticator)authenticator;
        } else {
            throw new IllegalArgumentException("Must be passed a RADIUS Authenticator");
        }

        ActionState actionState = new ActionState(true);

        // The user POSTed a response to the RADIUS Challenge
        if (isOK) {

            // Extract the user's response ...
            String challengeResponse = getWebAbstraction().getFormParameter(TwoFactorAuth.ID_CHALLENGE_RESPONSE);

            if (challengeResponse != null) {

                // ... and send it to the RADIUS server
                String result = radiusAuthenticator.authenticate(challengeResponse);

                if (TwoFactorAuth.RADIUS_SUCCESS.equalsIgnoreCase(result)) {

                    // authentication success - proceed to the next authentication state
                    Authentication.redirectToNextAuthPage(wiContext);
                    actionState = new ActionState(false);

                } else if (TwoFactorAuth.RADIUS_INVALID.equalsIgnoreCase(result)) {

                    // authentication failure - kill the session and back to login page
                    actionState = new ActionState(INVALID_CREDENTIALS_STATUS);

                } else if (result == null || TwoFactorAuth.RADIUS_FAILED.equalsIgnoreCase(result)) {

                    // system failure - kill the session and back to login page
                    actionState = new ActionState(GENERAL_RADIUS_ERROR_STATUS);

                } else {
                    // assume another challenge -- store it and make the 'challenge'
                    // page the next authentication state
                    Map challengeParameters = new HashMap();
                    challengeParameters.put(TwoFactorAuth.VAL_AUTHENTICATOR, authenticator);
                    challengeParameters.put(TwoFactorAuth.VAL_CHALLENGE, result);
                    Authentication.addPageToQueueHead(wiContext, TwoFactorAuth.VAL_CHALLENGE, challengeParameters);
                    Authentication.redirectToNextAuthPage(wiContext);
                    // This is a redirect, so don't show any UI
                    actionState = new ActionState(false);
                }

            } else {
                // The form wasn't filled in, just reload the page
                actionState = new ActionState(true);
            }

        } else {
            // they cancelled - back to login with an error indicating that authentication was not possible
            actionState = new ActionState(GENERAL_CREDENTIALS_FAILURE_STATUS);
        }

        return actionState;
    }

    /**
     * This is a GET, so set up the challenge string to be displayed,
     * then show the page.
     */
    protected ActionState doGetAction(Object authenticator, Map parameters) {
        viewControl.setRadiusChallenge(WebUtilities.escapeHTML((String)parameters.get(TwoFactorAuth.VAL_CHALLENGE)));
        return new ActionState(true);
    }

    /**
     * Set up the page controls to display the page
     */
    protected void doShowFormActions() throws IOException {
        welcomeControl.setTitle(wiContext.getString("ScreenTitleChallengeText"));
        welcomeControl.setBody(wiContext.getString("ChallengeText2"));
        layoutControl.formAction = Constants.FORM_POSTBACK;
    }

    protected String getBrowserPageTitleKey()
    {
        return "BrowserTitleChallenge";
    }
}