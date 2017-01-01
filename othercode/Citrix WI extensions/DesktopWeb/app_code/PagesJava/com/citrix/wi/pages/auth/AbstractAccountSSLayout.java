package com.citrix.wi.pages.auth;

import java.util.Map;
import com.citrix.wi.mvc.StatusMessage;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.AccountSelfService;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wi.accountselfservice.AccountTask;

import java.io.IOException;

public abstract class AbstractAccountSSLayout extends AbstractAuthLayout {

    private boolean checkForSessionFixation;
    private boolean resetCookieOnError;

    public AbstractAccountSSLayout(WIContext wiContext, boolean checkForSessionFixation, boolean resetCookieOnError) {
        super(wiContext);
        this.checkForSessionFixation = checkForSessionFixation;
        this.resetCookieOnError = resetCookieOnError;
    }

    protected boolean prePerformChecks() throws IOException
    {
        boolean isValid = true;

        if (!super.prePerformChecks())
        {
            isValid = false;
        } else if (checkForSessionFixation && wiContext.getUtils().checkForSessionFixation(wiContext))
        {
            isValid = false;
        } else if (!AccountSelfService.isAccountSelfServiceEnabled(wiContext)) {
            // User is not allowed to perform account self service
            UIUtils.HandleLoginFailedMessage(wiContext, new StatusMessage("SelfServiceNotAllowed"));
            isValid = false;
        }

        return isValid;
    }

    protected void handleStatusMessage(StatusMessage statusMessage) throws IOException
    {
        if (resetCookieOnError)
        {
            expireAuthCookie();
        }
        super.handleStatusMessage(statusMessage);
    }

    protected void expireAuthCookie()
    {
        wiContext.getUtils().expireAuthCookie(wiContext, AccountSelfService.COOKIE_ACCOUNT_SS_AUTH);
    }

    protected String addAuthCookie()
    {
        return wiContext.getUtils().addAuthCookie(wiContext, AccountSelfService.COOKIE_ACCOUNT_SS_AUTH);
    }

    /**
     * Get the task the account self service is being requested to do.
     */
    protected AccountTask getSelfServiceTask() {

        WebAbstraction webAbstraction = getWebAbstraction();
        Map parameters = (Map)Authentication.getAuthenticationState(webAbstraction).getParameters();
        return (AccountTask)parameters.get(AccountSelfService.VAL_TASK);
    }

    /**
     * Get the page title string lookup key to use, based on the current task.
     */
    protected String getPageTitleKey() {

        AccountTask task = getSelfServiceTask();

        // Return the appropriate key for the page title based on the task.
        if (task == AccountTask.ACCOUNT_UNLOCK) {
            return "ScreenTitleUnlockAccount";
        } else if (task == AccountTask.PASSWORD_RESET) {
            return "ScreenTitleResetAccountPassword";
        } else {
            // default to returning the generic account self service title.
            return "AccountSelfService";
        }
    }

    /**
     * Get the browser title string lookup key to use, based on the current task.
     */
    protected String getBrowserPageTitleKey() {

        AccountTask task = getSelfServiceTask();

        // Return the appropriate key for the browser page title based on the task.
        if (task == AccountTask.ACCOUNT_UNLOCK) {
            return "BrowserTitleUnlockAccount";
        } else if (task == AccountTask.PASSWORD_RESET) {
            return "BrowserTitleResetAccountPassword";
        } else         {
            // default to returning the generic account self service title.
            return "BrowserTitleAccountSelfService";
        }
    }
}
