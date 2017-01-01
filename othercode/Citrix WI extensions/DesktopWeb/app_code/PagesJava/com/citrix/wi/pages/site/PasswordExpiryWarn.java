/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.site;

import java.io.IOException;

import com.citrix.wi.controls.PasswordExpiryWarnPageControl;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pages.StandardLayout;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.PasswordExpiryWarningUtils;

public class PasswordExpiryWarn extends StandardLayout {

    protected PasswordExpiryWarnPageControl viewControl = new PasswordExpiryWarnPageControl();

    public PasswordExpiryWarn(WIContext wiContext) {
        super(wiContext);
        wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
    }

    protected boolean performImp() throws IOException {
        WebAbstraction web = wiContext.getWebAbstraction();

        if (web.isPostRequest()) {
            String submitMode = web.getFormParameter(Constants.ID_SUBMIT_MODE);
            if (Constants.VAL_OK.equals(submitMode)) {
                // The user chose to change password
                web.clientRedirectToUrl(Constants.PAGE_CHANGE_PASSWD);
            } else {
                // No changing, so go display the applist (the applist
                // page also looks after processing any further pages
                // on the post-login page stack
                web.clientRedirectToUrl(Constants.PAGE_APPLIST);
            }
            return false;

        } else { // GET

            // Check if we really want to display this page
            if (!PasswordExpiryWarningUtils.warnUser(wiContext)) {
                wiContext.getWebAbstraction().clientRedirectToUrl(Constants.PAGE_APPLIST);
                return false;
            }

            welcomeControl.setTitle(wiContext.getString("PwdExpWarn"));
            welcomeControl.setBody(""); // No body is necessary for this page
            layoutControl.formAction = Constants.FORM_POSTBACK;
            setupNavControl();

            // Remove most of the elements of the nav bar, since we don't want the
            // user to navigate away from the warning screen
            navControl.setShowDisconnect(false);
            navControl.setShowMessages(false);
            navControl.setShowReconnect(false);
            navControl.setShowSettings(false);
            navControl.setShowGraphicsMode(false);
            navControl.setAllowChangePassword(false);

            String expiryMessage = PasswordExpiryWarningUtils.getExpiryDaysAsSentence(wiContext);

            expiryMessage += " " + wiContext.getString("PwdExpWarnBodyPost");

            viewControl.expiryMessage = expiryMessage;
        }

        return true;
    }

    protected String getBrowserPageTitleKey()
    {
        return "PwdExpWarn";
    }
}
