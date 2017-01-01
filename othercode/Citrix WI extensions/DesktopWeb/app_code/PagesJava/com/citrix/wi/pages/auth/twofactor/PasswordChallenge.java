/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth.twofactor;

import java.io.IOException;
import java.util.Map;

import com.citrix.authenticators.ISecurIDAuthenticator;
import com.citrix.wi.controls.DialogPageControl;
import com.citrix.wi.mvc.ActionState;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Features;
import com.citrix.wi.pageutils.TwoFactorAuth;
import com.citrix.wing.MessageType;
import com.citrix.wing.util.Strings;

/**
 * This is used with SecurID and when a Password Challenge is used.
 */
public class PasswordChallenge extends AbstractTwoFactorAuthLayout {

    protected DialogPageControl viewControl = new DialogPageControl();

    public PasswordChallenge(WIContext wiContext) {
        super(wiContext);
        getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
    }

    protected ActionState doPostAction(Object authenticator, Map parameters, boolean isOK) throws IOException {
        if (isOK)
        {
            String password = wiContext.getWebAbstraction().getFormParameter(TwoFactorAuth.ID_PASSWORD_CHALLENGE);

            if (!Strings.isEmpty(password) &&
                 (!Strings.hasControlChars(password)) &&
                 (password.length() <= Constants.PASSWORD_ENTRY_MAX_LENGTH)) {

                parameters.put(TwoFactorAuth.VAL_PASSWORD_FROM_CHALLENGE, password);

                // then continue login which will use the updated AccessToken
                Authentication.forwardToNextAuthPage(wiContext);
                return new ActionState(false);

            } else {
                setFeedback(MessageType.ERROR, "InvalidPasswordChallengeResponse");
                return new ActionState(true);
            }

        } else {
            if (Features.isSecurIDSupported() && authenticator instanceof ISecurIDAuthenticator) {
                ((ISecurIDAuthenticator)authenticator).Dispose();
            }
            // they cancelled - back to login with an error indicating that authentication was not possible
            return new ActionState(GENERAL_CREDENTIALS_FAILURE_STATUS);
        }
    }

    /**
     * Set up the GUI
     */
    protected void doShowFormActions() throws IOException {
        welcomeControl.setTitle(wiContext.getString("ScreenTitleChallengeText"));
        welcomeControl.setBody(wiContext.getString("PasswordChallengeText2"));
        layoutControl.formAction = Constants.FORM_POSTBACK;
    }

    protected String getBrowserPageTitleKey()
    {
        return "BrowserTitleChallenge";
    }
}
