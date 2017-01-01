package com.citrix.wi.pages.auth;

import java.io.IOException;

import com.citrix.wi.mvc.ActionState;
import com.citrix.wi.mvc.StatusMessage;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pages.StandardLayout;
import com.citrix.wi.pageutils.UIUtils;

public abstract class AbstractAuthLayout extends StandardLayout {

    public AbstractAuthLayout(WIContext wiContext) {
        super(wiContext);
    }

    public abstract ActionState performInternal() throws IOException;

    protected boolean performImp() throws IOException
    {
        if (!prePerformChecks()) {
            return false;
        }

        ActionState actionState = performInternal();
        if (actionState.getStatusMessage() != null)
        {
            handleStatusMessage(actionState.getStatusMessage());
        }
        return actionState.getShowForm();
    }

    protected boolean prePerformChecks() throws IOException
    {
        return true;
    }

    protected void handleStatusMessage(StatusMessage statusMessage) throws IOException
    {
        UIUtils.HandleLoginFailedMessage(wiContext, statusMessage);
    }
}
