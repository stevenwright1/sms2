/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth.age;

import com.citrix.authentication.tokens.ChangeablePasswordBasedAccessToken;
import com.citrix.wi.controls.DialogPageControl;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pages.StandardLayout;
import com.citrix.wi.pageutils.AGEUtilities;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.TwoFactorAuth;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wing.MessageType;
import com.citrix.wing.util.Strings;

import java.io.IOException;
import java.util.Map;

public class Password extends StandardLayout {

    protected DialogPageControl viewControl = new DialogPageControl();

    public Password(WIContext wiContext) {
        super(wiContext);
        wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
    }

    protected boolean performImp() throws IOException {

        WebAbstraction web = wiContext.getWebAbstraction();

        // Check that AGE passthrough is allowed
        if (!AGEUtilities.isAGEIntegrationEnabled(wiContext)) {
            UIUtils.HandleLoginFailedMessage(wiContext, MessageType.ERROR, "NoAGELogin");
            return false;
        }

        if (web.isPostRequest()) {
            if (Constants.VAL_OK.equals(web.getFormParameter(Constants.ID_SUBMIT_MODE))) {

                // Retrieve the access token and recreate it with the new password
                Map parameters = (Map)Authentication.getAuthenticationState(web).getParameters();
                ChangeablePasswordBasedAccessToken accessToken =
                    (ChangeablePasswordBasedAccessToken)parameters.get(Constants.AGE_ACCESS_TOKEN);

                String password = web.getFormParameter(TwoFactorAuth.ID_PASSWORD_CHALLENGE);

                if ((password != null) && !Strings.hasControlChars(password) &&
                    (accessToken != null)) {

                    // Update the stored access token with the new password
                    accessToken.setSecret(password);

                    Authentication.forwardToNextAuthPage(wiContext);
                    return false;
                }
            }

            // Invalid input or user pressed Cancel - return an error response
            web.abandonSession();
            web.setResponseStatus(WebAbstraction.SC_UNAUTHORIZED);
            web.writeToResponse(wiContext.getString(AGEUtilities.KEY_AG_AUTH_ERROR));

            return false;
        }

        // Set up the page
        // The controls from the two-factor password challenge page are used
        // as the page is very similar

        // Prepare nav and welcome message controls with default values
        welcomeControl.setTitle(wiContext.getString("ScreenTitlePasswordChallenge"));
        welcomeControl.setBody(wiContext.getString("PasswordChallengeText2"));
        layoutControl.formAction = Constants.FORM_POSTBACK;
        // Nav bar should not be shown, so do nothing

        return true;
    }

    protected String getBrowserPageTitleKey() {
        return "BrowserTitlePasswordChallenge";
    }
}
