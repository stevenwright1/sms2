/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth;

import java.io.IOException;

import com.citrix.wi.accountselfservice.AccountTask;
import com.citrix.wi.controls.AccountSSEntryPageControl;
import com.citrix.wi.mvc.ActionState;
import com.citrix.wi.mvc.StatusMessage;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pageutils.AccountSelfService;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wing.MessageType;

public class AccountSSEntry extends AbstractAccountSSLayout {

    protected AccountSSEntryPageControl viewControl = new AccountSSEntryPageControl();

    public AccountSSEntry(WIContext wiContext) {
        super(wiContext, false, false);
        getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
    }

    public ActionState performInternal() throws IOException {

        WebAbstraction web = getWebAbstraction();

        boolean accountUnlockEnabled = AccountSelfService.isAccountUnlockEnabled(wiContext.getConfiguration());
        boolean passwordResetEnabled = AccountSelfService.isPasswordResetEnabled(wiContext);

        // Deal with form submission
        if (web.isPostRequest()) {
            String submitMode = web.getFormParameter(Constants.ID_SUBMIT_MODE);
            if (Constants.VAL_OK.equalsIgnoreCase(submitMode)) {

                AccountTask task = null;

                // The user has selected a task or chosen to continue to the next page
                if ( accountUnlockEnabled && passwordResetEnabled ) {
                    // Get the user's choice and keep it in the authentication parameters
                    task = AccountTask.fromString( web.getFormParameter(AccountSelfService.ID_TASK) );
                } else if ( accountUnlockEnabled ) {
                    task = AccountTask.ACCOUNT_UNLOCK;
                } else if ( passwordResetEnabled ) {
                    task = AccountTask.PASSWORD_RESET;
                }

                if ( task != null ) {

                    AccountSelfService.BuildAuthenticationFilterQueue(wiContext, task);

                    Authentication.redirectToNextAuthPage(wiContext);
                } else {
                    // Form feedback was absent or invalid
                    // Do nothing here so that page is re-displayed
                }
            } else {
                // The user has pressed the Cancel button
                return new ActionState(new StatusMessage(MessageType.INFORMATION, "SelfServiceNotComplete"));
            }
        }

        // Set up the page appearance
        welcomeControl.setTitle(wiContext.getString("AccountSelfService"));
        welcomeControl.setBody(wiContext.getString("SelfServiceEntryWelcome"));
        layoutControl.formAction = Constants.FORM_POSTBACK;

        // Appearance changes depending on whether one or both tasks are allowed
        if (accountUnlockEnabled) {
            viewControl.setTaskAllowed(AccountTask.ACCOUNT_UNLOCK);
        }
        if (passwordResetEnabled) {
            viewControl.setTaskAllowed(AccountTask.PASSWORD_RESET);
        }
        return new ActionState(true);
    }

    protected String getBrowserPageTitleKey() {
        return "BrowserTitleAccountSelfService";
    }
}
