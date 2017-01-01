/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth;

import java.util.HashMap;

import com.citrix.wi.accountselfservice.AccountTask;
import com.citrix.wi.accountselfservice.IncorrectAnswerException;
import com.citrix.wi.accountselfservice.QuestionBasedContext;
import com.citrix.wi.accountselfservice.TaskDeniedException;
import com.citrix.wi.accountselfservice.UserLockedOutException;
import com.citrix.wi.controls.AccountSSQuestionPageControl;
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

import java.io.IOException;

public class AccountSSQBA extends AbstractAccountSSLayout {

    protected AccountSSQuestionPageControl viewControl = new AccountSSQuestionPageControl();

    public AccountSSQBA(WIContext wiContext) {
        super(wiContext, true, true);
        wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
    }

    public ActionState performInternal() throws IOException {

        WebAbstraction web = getWebAbstraction();

        // Retrieve parameters from the authentication state
        HashMap parameters = (HashMap) Authentication.getAuthenticationState(web).getParameters();
        QuestionBasedContext context =
            (QuestionBasedContext) parameters.get(AccountSelfService.VAL_ACCOUNT_SS_CONTEXT);
        AccountTask task = (AccountTask) parameters.get(AccountSelfService.VAL_TASK);

        if ( ( context == null ) || ( task == null ) ) {
            // Without these parameters, account self service cannot continue
            return new ActionState(new StatusMessage("SelfServiceNotAllowed"));
        }

        // Deal with form submission
        if (web.isPostRequest()) {
            String submitMode = web.getFormParameter(Constants.ID_SUBMIT_MODE);
            if (Constants.VAL_OK.equalsIgnoreCase(submitMode)) {

                // Retrieve the answer
                String answer = "";
                if (web.getFormParameter(AccountSelfService.ID_ANSWER) != null) {
                    // We do not trim the answer as the question may be sensitive to whitespace
                    answer = web.getFormParameter(AccountSelfService.ID_ANSWER);
                }

                // Retrieve the confirmed answer, if present
                String confirmedAnswer = "";
                if (web.getFormParameter(AccountSelfService.ID_CONFIRM_ANSWER) != null) {
                    confirmedAnswer = web.getFormParameter(AccountSelfService.ID_CONFIRM_ANSWER);
                }

                if (context.requireAnswerConfirmation() && !answer.equals(confirmedAnswer)) {
                    // The two answers need to be identical
                    setFeedback(MessageType.ERROR, "DifferentAnswers");
                }

                if ((answer == null) || (answer.length() == 0) || Strings.hasControlChars(answer))
                {
                    // Reload the page, displaying an error
                    setFeedback(MessageType.ERROR, "InvalidAnswer");
                    viewControl.addInvalidField(Constants.ID_ASS_ANSWER);
                }

                // Only do something with the results if there has not yet been
                // an error
                if (!isFeedbackSet()) {
                    // A valid answer was provided
                    try {
                        context.answerCurrentQuestion( answer );

                        if ( context.isUserAuthenticated() ) {
                            // The user has answered all of their questions
                            Authentication.redirectToNextAuthPage(wiContext);
                        } else {
                            // The user has more questions to answer
                            // Redirect to the same page in order to prevent the browser from caching the form
                            // parameters (thus allowing a re-post), if we directly display the resulting 200 OK page.
                            Authentication.redirectToCurrentAuthPage(wiContext);
                        }
                    } catch ( IncorrectAnswerException iae ) {
                        // The user provided an incorrect answer
                        if ( iae.getAttemptsRemaining() > 0 ) {
                            // Reload the page and notify them of the attempts remaining
                            setFeedback(MessageType.ERROR, "AnswerWasIncorrect");
                            viewControl.addInvalidField(Constants.ID_ASS_ANSWER);
                        } else {
                            // The user has been locked out of account self service
                            return new ActionState(new StatusMessage("AllowedAttemptsReached"));
                        }
                    } catch ( ServiceCommunicationException sce ) {
                        // There was some error in communicating with the web service
                        return new ActionState(new StatusMessage(MessageType.ERROR, "SelfServiceNotAllowed2", null, sce.getMessageKey(), sce.getMessageArguments()));
                    } catch ( UserLockedOutException uloe ) {
                        // The user has been locked out of account self service
                        return new ActionState(new StatusMessage("AllowedAttemptsReached"));
                    } catch ( TaskDeniedException tde ) {
                        // Some other error occurred e.g. the authentication token expired
                        return new ActionState(new StatusMessage("SelfServiceNotAllowed"));
                    }
                }
            } else {
                // The user has pressed the Cancel button
                return new ActionState(new StatusMessage(MessageType.INFORMATION, "SelfServiceNotComplete"));
            }
        }

        // Set up the page appearance
        welcomeControl.setTitle(wiContext.getString(getPageTitleKey()));
        if (context.getCurrentQuestionNumber() == 1) {
            welcomeControl.setBody( wiContext.getString("SecurityQuestionWelcome") );
        }
        layoutControl.formAction = Constants.FORM_POSTBACK;

        // Set up the page control
        viewControl.setTotalQuestionCount( context.getQuestionCount() );
        viewControl.setQuestionNumber( context.getCurrentQuestionNumber() );
        viewControl.setQuestionText( context.getCurrentQuestionText() );
        viewControl.setMaskAnswerFields( context.maskAnswerFields() );
        viewControl.setShowConfirmationField( context.requireAnswerConfirmation() );
        viewControl.continueButtonLabelKey = "Next";

        return new ActionState(true);
    }
}
