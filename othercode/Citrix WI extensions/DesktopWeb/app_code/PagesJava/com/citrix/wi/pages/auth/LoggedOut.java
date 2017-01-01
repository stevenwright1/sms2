/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth;

import java.io.IOException;

import com.citrix.authentication.web.AuthenticationState;
import com.citrix.wi.controls.LoggedoutPageControl;
import com.citrix.wi.controlutils.FeedbackMessage;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pages.StandardLayout;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.LaunchUtilities;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wing.MessageType;
import com.citrix.wing.UserEnvironmentAdaptor;
import com.citrix.wing.util.Strings;

public class LoggedOut extends StandardLayout {

    protected LoggedoutPageControl viewControl = new LoggedoutPageControl();

    public LoggedOut(WIContext wiContext) {
        super(wiContext);
        wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
    }

    protected boolean performImp() throws IOException {

        boolean showForm = true;

        UserEnvironmentAdaptor envAdaptor = wiContext.getUserEnvironmentAdaptor();

        // Reset the logon type cookie
        Authentication.setUntrustedLogonType(null, envAdaptor);

        boolean smartCardLoggedOut = Constants.VAL_ON.equalsIgnoreCase((String)envAdaptor.getClientSessionState().get(Constants.COOKIE_SMC_LOGGED_OUT));
        boolean AGELoggedOut = Constants.VAL_ON.equalsIgnoreCase((String)envAdaptor.getClientSessionState().get(Constants.COOKIE_AGE_LOGGED_OUT));

        // Unlike Smart Card Logout, once we get to this logout page, and are aware we need to
        // display the 'close your browser' message, we will clear the cookie. We need to do this
        // because AGE child windows (e.g. spawned from its Navigation Page) share the same transient
        // cookie cache.
        if (AGELoggedOut) {
            envAdaptor.getClientSessionState().put(Constants.COOKIE_AGE_LOGGED_OUT, Constants.VAL_OFF);
        }

        String pageURL = wiContext.getConfiguration().getUIConfiguration().getErrorCallbackURL();

        // Remove application id from cookies to avoid getting into Direct Launch
        // mode by accident.
        LaunchUtilities.clearClientSessionLaunchData(wiContext);
        LaunchUtilities.clearSessionLaunchData(wiContext);
        AuthenticationState state =
            Authentication.getAuthenticationState(wiContext.getWebAbstraction());
        state.setInitialURL(null);

        envAdaptor.commitState();
        envAdaptor.destroy();
        if (!Strings.isEmptyOrWhiteSpace(pageURL)) {
            FeedbackMessage feedback = feedbackControl.getFeedback();
            MessageType msgType = (feedback == null) ? null : feedback.getType();
            String msgKey = (feedback == null) ? null : feedback.getKey();
            String logEventId = (feedback == null) ? null : feedback.getLogEventId();

            pageURL += UIUtils.getMessageQueryStr(msgType, msgKey, logEventId);

            wiContext.getWebAbstraction().clientRedirectToUrl(pageURL);
            showForm = false;
        } else {
			layoutControl.isLoggedOutPage = true;
            
            // warn the user to close the window
            if (smartCardLoggedOut || AGELoggedOut) {
                viewControl.setShowCloseWindow(true);
            } else {
                viewControl.setShowCloseWindow(false);
            }
        }

        return showForm;
    }

    protected String getBrowserPageTitleKey() {
        return "BrowserTitleLoggedout";
    }
}
