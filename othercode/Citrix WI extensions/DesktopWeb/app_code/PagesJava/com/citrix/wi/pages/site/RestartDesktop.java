// RestartDesktop.java
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// [NFuseVersionAndBuildNumber]
package com.citrix.wi.pages.site;

import java.io.IOException;

import com.citrix.wi.controls.DelayedLaunchControl;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pages.StandardPage;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.DelayedLaunchUtilities;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.SessionToken;
import com.citrix.wi.pageutils.SessionUtils;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wing.MessageType;
import com.citrix.wing.util.Strings;
import com.citrix.wing.util.WebUtilities;
import com.citrix.wing.webpn.UserContext;

/**
 * This page sets a session variable to indicate that the user's desktop should be restarted,
 * and then redirects to the home page to perform the launch.
 */
public class RestartDesktop extends StandardPage {

    public RestartDesktop(WIContext wiContext) {
        super(wiContext);
    }

    protected boolean performImp() throws IOException {
        if (!SessionToken.verifyCsrfSafe(wiContext)) {
            getWebAbstraction().clientRedirectToUrl(UIUtils.
                getMessageRedirectUrl(wiContext, Include.getHomePage(wiContext), MessageType.ERROR, "SessionTokenError"));
            return false;
        }

        String appId = getWebAbstraction().getQueryStringParameter(Constants.QSTR_APPLICATION);
        if (Strings.isEmpty(appId)) {
            getWebAbstraction().clientRedirectToUrl(UIUtils.
                getMessageRedirectUrl(wiContext, Include.getHomePage(wiContext), MessageType.ERROR, "GeneralAppLaunchError"));
            return false;
        }

        UserContext userContext = SessionUtils.checkOutUserContext(wiContext);

        // Set two session variables, one to trigger a power-off and another to allow a power-off-restart
        // to be distinguishable from a regular launch.

        // Add the restarting desktop to both the power off list and the restarting list, the latter to
        // allow a power-off-restart to be distinguishable from a regular launch.
        DelayedLaunchControl delayedLaunchControl = DelayedLaunchUtilities.getDelayedLaunchControl(wiContext);
        delayedLaunchControl.addPowerOffResource(appId);
        delayedLaunchControl.addRestartingResource(appId);

        String url = Include.getHomePage(wiContext);
        url += "?" + Constants.QSTR_RETRY_APPLICATION + "=" + WebUtilities.escapeURL(appId);
        url += SessionToken.copyCsrfQueryToken(wiContext);

        getWebAbstraction().clientRedirectToUrl(url);

        SessionUtils.returnUserContext(userContext);

        return false;
    }
}
