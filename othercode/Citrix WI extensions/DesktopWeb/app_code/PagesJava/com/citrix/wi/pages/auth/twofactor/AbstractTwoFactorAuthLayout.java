/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth.twofactor;

import java.io.IOException;
import java.util.Map;

import com.citrix.wi.mvc.ActionState;
import com.citrix.wi.mvc.StatusMessage;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pages.auth.AbstractAuthLayout;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.TwoFactorAuth;
import com.citrix.wing.MessageType;

public abstract class AbstractTwoFactorAuthLayout extends AbstractAuthLayout {

    protected final static String VAL_VIEW_CONTROL = "viewControl";

    // Status message constants
    protected final static String VAL_GENERAL_AUTHENTICATION_ERROR = "GeneralAuthenticationError";
    protected final static StatusMessage GENERAL_CREDENTIALS_FAILURE_STATUS = new StatusMessage("GeneralCredentialsFailure");
    protected final static StatusMessage GENERAL_AUTHENTICATION_ERROR_STATUS = new StatusMessage(VAL_GENERAL_AUTHENTICATION_ERROR);
    protected final static StatusMessage INVALID_CREDENTIALS_STATUS = new StatusMessage("InvalidCredentials");
    protected final static StatusMessage GENERAL_RADIUS_ERROR_STATUS = new StatusMessage(MessageType.ERROR, VAL_GENERAL_AUTHENTICATION_ERROR, null, "RadiusAuthenticatorError", null);
    protected final static StatusMessage PIN_NOT_CHANGED_STATUS = new StatusMessage(MessageType.INFORMATION, TwoFactorAuth.KEY_PIN_NOT_CHANGED);
    protected final static StatusMessage PIN_CHANGED_STATUS = new StatusMessage(MessageType.INFORMATION, TwoFactorAuth.KEY_PIN_CHANGED);
    protected final static StatusMessage PIN_REJECTED_STATUS = new StatusMessage(TwoFactorAuth.KEY_PIN_REJECTED);

    public AbstractTwoFactorAuthLayout(WIContext wiContext) {
        super(wiContext);
    }

    public ActionState performInternal() throws IOException
    {
        Map parameters = (Map)Authentication.getAuthenticationState(wiContext.getWebAbstraction()).getParameters();
        Object authenticator = parameters.get(TwoFactorAuth.VAL_AUTHENTICATOR);
        if (authenticator == null) {
            return new ActionState(GENERAL_AUTHENTICATION_ERROR_STATUS);
        }

        ActionState actionState = new ActionState(true);
        if (getWebAbstraction().isPostRequest())
        {
            String submitMode = wiContext.getWebAbstraction().getFormParameter(Constants.ID_SUBMIT_MODE);
            boolean isOK = Constants.VAL_OK.equalsIgnoreCase(submitMode);
            actionState = doPostAction(authenticator, parameters, isOK);
        } else
        {
            actionState = doGetAction(authenticator, parameters);
        }

        if (actionState.getShowForm())
        {
            doShowFormActions();
        }

        return actionState;
    }

    // Do actions for HTTP POST requests
    protected abstract ActionState doPostAction(Object authenticator, Map parameters, boolean isOK)
    throws IOException;

    // Default Get Action is do nothing
    protected ActionState doGetAction(Object authenticator, Map parameters)
    {
        return new ActionState(true);
    }

    // Do any actions required to set up the message center and other
    // controls associated with the page
    protected abstract void doShowFormActions() throws IOException;
}
