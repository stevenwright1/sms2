/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth;

import java.io.IOException;
import java.util.Map;

import com.citrix.wi.accountselfservice.AccountTask;
import com.citrix.wi.accountselfservice.QuestionBasedContext;
import com.citrix.wi.accountselfservice.TaskDeniedException;
import com.citrix.wi.mvc.ActionState;
import com.citrix.wi.mvc.StatusMessage;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pageutils.AccountSelfService;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.util.ServiceCommunicationException;
import com.citrix.wing.MessageType;

public class AccountSSUnlock extends AbstractAccountSSLayout {

    public AccountSSUnlock(WIContext wiContext) {
        super(wiContext, true, false);
    }

    public ActionState performInternal() throws IOException {

        WebAbstraction web = getWebAbstraction();

        // Check whether the user needs to be redirected to the login page
        if ( web.isGetRequest() &&
             ( web.getQueryStringParameter( Constants.QSTR_END_SELF_SERVICE ) != null ) ) {
             // No need to display a message on the login page
             return new ActionState(new StatusMessage(null, ""));
        }

        // user hit finish button
        if (web.isPostRequest()) {
            web.clientRedirectToUrl(Constants.PAGE_ACCOUNT_SS_UNLOCK + "?" + Constants.QSTR_END_SELF_SERVICE + "=" + Constants.VAL_ON);
            return new ActionState(false);
        }

        // Retrieve parameters from the authentication state
        Map parameters = (Map) Authentication.getAuthenticationState(web).getParameters();
        QuestionBasedContext context =
            (QuestionBasedContext) parameters.get(AccountSelfService.VAL_ACCOUNT_SS_CONTEXT);
        AccountTask task = (AccountTask) parameters.get(AccountSelfService.VAL_TASK);

        if ( ( context == null ) ||
             ( task == null ) ||
             ( !context.isUserAuthenticated() ) ||
             ( task != AccountTask.ACCOUNT_UNLOCK ) ) {
            // This page has been reached in error
            return new ActionState(new StatusMessage("SelfServiceNotAllowed"));
        }

        // Attempt to unlock the user account
        try {
            context.unlockAccount();
        } catch ( ServiceCommunicationException sce ) {
            // There was some error in communicating with the web service
            return new ActionState(new StatusMessage(MessageType.ERROR, "SelfServiceNotAllowed2", null, sce.getMessageKey(), sce.getMessageArguments()));
        } catch ( TaskDeniedException tde ) {
            // The service failed to unlock the account
            return new ActionState(new StatusMessage("AccountUnlockFailed"));
        }

        // At this point the unlock request must have succeeded, so show the page content

        // Set up the page appearance
        welcomeControl.setTitle(wiContext.getString(getPageTitleKey()));
        layoutControl.formAction = Constants.FORM_POSTBACK;

        // Clear the authorization cookie now the account self-service operation is complete
        // Note, doing this further down where the session is expired fails if the response has been committed by then.
        expireAuthCookie();

        return new ActionState(true);
    }
}