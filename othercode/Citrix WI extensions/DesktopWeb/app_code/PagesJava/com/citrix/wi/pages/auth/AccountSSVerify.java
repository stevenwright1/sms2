/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth;

import java.io.IOException;
import java.util.HashMap;

import com.citrix.authentication.tokens.UserDomainPasswordCredentials;
import com.citrix.wi.accountselfservice.AccountTask;
import com.citrix.wi.accountselfservice.ContextFactory;
import com.citrix.wi.accountselfservice.QuestionBasedContext;
import com.citrix.wi.accountselfservice.TaskDeniedException;
import com.citrix.wi.accountselfservice.UserLockedOutException;
import com.citrix.wi.controls.AccountSSEntryPageControl;
import com.citrix.wi.mvc.ActionState;
import com.citrix.wi.mvc.StatusMessage;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.AccountSelfService;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.util.ServiceCommunicationException;
import com.citrix.wing.MessageType;
import com.citrix.wi.pageutils.Constants;

public class AccountSSVerify extends AbstractAccountSSLayout {

    protected AccountSSEntryPageControl viewControl = new AccountSSEntryPageControl();

    public AccountSSVerify(WIContext wiContext) {
        super(wiContext, false, false);
        getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
    }

    public ActionState performInternal() throws IOException {

        // Get the base URL of the Password Manager service
        String baseURL = wiContext.getConfiguration().getAccountSelfServiceConfiguration().getServiceBaseURL();

        HashMap parameters = (HashMap) Authentication.getAuthenticationState(wiContext.getWebAbstraction()).getParameters();
        UserDomainPasswordCredentials credentials = (UserDomainPasswordCredentials) parameters.get(Authentication.VAL_ACCESS_TOKEN);
        AccountTask task = (AccountTask) parameters.get(AccountSelfService.VAL_TASK);

        if ( ( credentials != null ) && ( task != null ) ) {

            try {
                ContextFactory factory = wiContext.getUtils().getAccountSSContextFactory();

                // Attempt to create a Context for the current user
                QuestionBasedContext context =
                    factory.getQuestionBasedContext( baseURL,
                                                     credentials.getUser(),
                                                     credentials.getDomain(),
                                                     task );

                // Store the context in the authentication state
                parameters.put(AccountSelfService.VAL_ACCOUNT_SS_CONTEXT, context);

                // Advance to the question-based authentication page
                Authentication.redirectToNextAuthPage(wiContext);
                return new ActionState(false);
            } catch ( ServiceCommunicationException sce ) {
                // There was some error in communicating with the web service
                return new ActionState(new StatusMessage(MessageType.ERROR, "SelfServiceNotAllowed2", null, sce.getMessageKey(), sce.getMessageArguments()));
            } catch ( UserLockedOutException uloe ) {
                // The user has been temporarily locked out of account self service
                return new ActionState(new StatusMessage("SelfServiceLockedOut"));
            } catch ( TaskDeniedException tde ) {
                // The web service denied the user access to account self service
                // redirect to input credentials page with an error message
                Authentication.addPageToQueueHead(wiContext, "account_ss_verify", parameters);
                parameters.put(Constants.CONST_ERROR_MESSAGE, "MistypedCredentialsOrNotAllowed");
                Authentication.addPageToQueueHead(wiContext, "account_ss_user", parameters);

                Authentication.redirectToNextAuthPage(wiContext);
                return new ActionState(false);
            }
        } else {
            // Important parameters are missing - account self service cannot continue
            return new ActionState(new StatusMessage("SelfServiceNotAllowed"));
        }
    }

    protected String getBrowserPageTitleKey() {
        return null;
    }
}
