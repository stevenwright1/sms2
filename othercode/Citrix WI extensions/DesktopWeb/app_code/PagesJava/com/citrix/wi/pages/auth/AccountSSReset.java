/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth;

import java.io.IOException;
import java.util.HashMap;

import com.citrix.wi.accountselfservice.AccountTask;
import com.citrix.wi.accountselfservice.InvalidNewPasswordException;
import com.citrix.wi.accountselfservice.QuestionBasedContext;
import com.citrix.wi.accountselfservice.TaskDeniedException;
import com.citrix.wi.controls.AccountSSPasswordResetPageControl;
import com.citrix.wi.mvc.ActionState;
import com.citrix.wi.mvc.StatusMessage;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pageutils.AccountSelfService;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.util.ServiceCommunicationException;
import com.citrix.wing.MessageType;
import com.citrix.wing.util.Strings;

public class AccountSSReset extends AbstractAccountSSLayout {

    protected AccountSSPasswordResetPageControl viewControl = new AccountSSPasswordResetPageControl();

    public AccountSSReset(WIContext wiContext) {
        super(wiContext, true, true);
        wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
    }

    public ActionState performInternal() throws IOException {

        WebAbstraction web = getWebAbstraction();

        // Check whether the user needs to be redirected to the login page
        if ( web.isGetRequest() &&
             ( web.getQueryStringParameter( Constants.QSTR_END_SELF_SERVICE ) != null ) ) {
            // No need to display a message on the login page
            return new ActionState(new StatusMessage(null, ""));
        }

        // Retrieve parameters from the authentication state
        HashMap parameters = (HashMap) Authentication.getAuthenticationState(web).getParameters();
        QuestionBasedContext context =
            (QuestionBasedContext) parameters.get(AccountSelfService.VAL_ACCOUNT_SS_CONTEXT);
        AccountTask task = (AccountTask) parameters.get(AccountSelfService.VAL_TASK);

        if ( ( context == null ) ||
             ( task == null ) ||
             ( !context.isUserAuthenticated() ) ||
             ( task != AccountTask.PASSWORD_RESET ) ) {
            // This page has been reached in error
            return new ActionState(new StatusMessage("SelfServiceNotAllowed"));
        }

        // Deal with form submission
        if (web.isPostRequest()) {
            String submitMode = web.getFormParameter(Constants.ID_SUBMIT_MODE);
            if (Constants.VAL_OK.equalsIgnoreCase(submitMode)) {

                // Retrieve the first and second passwords
                String newPassword
                    = Strings.ensureNonNull(web.getFormParameter(AccountSelfService.ID_NEW_PASSWORD));
                String confirmPassword
                    = Strings.ensureNonNull(web.getFormParameter(AccountSelfService.ID_CONFIRM_PASSWORD));

                if ( Strings.hasControlChars( newPassword ) || Strings.hasControlChars( confirmPassword ) ) {
                    setFeedback(MessageType.ERROR, "ChangePasswordNewInvalid");
                } else if ( !newPassword.equals( confirmPassword ) ) {
                    setFeedback(MessageType.ERROR, "ChangePasswordConfirmDifferent");
                } else {
                    // The passwords were identical
                    try {
                        context.resetPassword( newPassword );
                        AccountSelfService.recordPasswordWasReset(wiContext);

                        // Redirect to the same page in order to prevent the browser from caching the form
                        // parameters (thus allowing a re-post), if we directly display the resulting 200 OK page.
                        Authentication.redirectToCurrentAuthPage(wiContext);

                        // Return to end the execution of the script
                        return new ActionState(false);

                    } catch ( InvalidNewPasswordException inpe ) {
                        // The account authority refused the new password
                        // Inform the user and allow them to try again
                        setFeedback(MessageType.ERROR, "ChangePasswordNewInvalid");
                    } catch ( ServiceCommunicationException sce ) {
                        // There was some error in communicating with the web service
                        return new ActionState(new StatusMessage(MessageType.ERROR, "SelfServiceNotAllowed2", null, sce.getMessageKey(), sce.getMessageArguments()));
                    } catch ( TaskDeniedException tde ) {
                        // The service failed to reset the password
                        return new ActionState(new StatusMessage("ResetPasswordFailed"));
                    }
                }
            } else {
                // The user has pressed the Cancel button
                return new ActionState(new StatusMessage(MessageType.INFORMATION, "SelfServiceNotComplete"));
            }
        }

        // Set up the page appearance
        welcomeControl.setTitle(wiContext.getString(getPageTitleKey()));
        layoutControl.formAction = Constants.FORM_POSTBACK;

        if (AccountSelfService.wasPasswordReset(wiContext)) {
            // The password was reset successfully

            // Set the page to show a success message
            viewControl.setShowSuccessMessage(true);

            // Clear the authorization cookie now the account self-service operation is complete
            // Note, doing this further down where the session is expired fails if the response has been committed by then.
            wiContext.getUtils().expireAuthCookie(wiContext, AccountSelfService.COOKIE_ACCOUNT_SS_AUTH);
        } else {
            // set the password change guideline text
            welcomeControl.setBody(wiContext.getString("ChangePassword3"));
        }

        return new ActionState(true);
    }
}