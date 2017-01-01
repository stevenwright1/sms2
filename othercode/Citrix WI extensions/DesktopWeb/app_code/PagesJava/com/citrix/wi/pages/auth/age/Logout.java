// Copyright (c) 2004 - 2010 Citrix Systems, Inc. All Rights Reserved.

package com.citrix.wi.pages.auth.age;

import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pages.StandardLayout;
import com.citrix.wi.pageutils.AGEUtilities;

public class Logout extends StandardLayout {

    public Logout(WIContext wiContext) {
        super(wiContext);
    }

    public boolean performImp() {
        if (AGEUtilities.isAGEIntegrationEnabled(wiContext)) {
            String logoutTicket = wiContext.getWebAbstraction().getQueryStringParameter(AGEUtilities.QSTR_LOGOUT_TICKET);
            wiContext.getUtils().abandonSession(wiContext, logoutTicket);
        }

        return false;
    }

    public String getBrowserPageTitleKey() {
        // return null so only site name is used.
        // no UI associated with this class.
        return null;
    }
}