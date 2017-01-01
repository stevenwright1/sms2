/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */

package com.citrix.wi.pages.site;

import java.io.IOException;

import com.citrix.wi.controls.RetryControl;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pages.StandardPage;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.DelayedLaunchUtilities;
import com.citrix.wi.pageutils.PageHistory;
import com.citrix.wi.pageutils.SessionToken;
import com.citrix.wi.pageutils.SessionUtils;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wing.MessageType;
import com.citrix.wing.webpn.ResourceInfo;
import com.citrix.wing.webpn.UserContext;
import com.citrix.wi.controls.*;
import com.citrix.wing.util.*;
import com.citrix.wi.pageutils.*;
/**
 * This page does not have any UI but needs to set the javascript to delay the launch
 * This page sets the javascript timer to retry the desktop launch after a delay calling launcher.java
 * after the set time
 */
public class DelayLaunchTimer extends StandardPage{
    private DelayLaunchTimerPageViewControl viewControl = new DelayLaunchTimerPageViewControl();

    public DelayLaunchTimer(WIContext wiContext) {
        super(wiContext);
        wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);

    }

    public boolean performImp() throws IOException {
        WebAbstraction web = wiContext.getWebAbstraction();

        UserContext userContext = SessionUtils.checkOutUserContext(wiContext);
        DelayedLaunchControl delayedLaunchControl = DelayedLaunchUtilities.getDelayedLaunchControl(wiContext);

        String errorKey = null;

        // Get the app name for this retry and verify, otherwise redirect with an error.
        String appName = web.getQueryStringParameter(Constants.QSTR_RETRY_APPLICATION);
        if (Strings.isEmpty(appName) || !delayedLaunchControl.isResourcePending(appName)) {
            UIUtils.handleMessage(wiContext, PageHistory.getCurrentPageURL(web), MessageType.ERROR, "RetryError");
            return false;
        }

        String launchUrl = "";
        ResourceInfo resInfo = null;

        RetryControl retryControl = delayedLaunchControl.getPendingResourceRetryControl(appName);

        // get the resource info for the app
        resInfo = ResourceEnumerationUtils.getResource(appName, userContext);
        launchUrl = Constants.PAGE_RETRY + "?"
                  + Constants.QSTR_RETRY_APPLICATION + "=" + WebUtilities.escapeURL(appName)
                  + SessionToken.copyCsrfQueryToken(wiContext);

        if ( (resInfo == null) || !resInfo.isEnabled() ) {
            errorKey = "AppRemoved";
        } else if (retryControl == null){
            errorKey = "RetryError";
        }

        SessionUtils.returnUserContext( userContext );

        if (errorKey != null){
            UIUtils.handleMessage(wiContext, PageHistory.getCurrentPageURL(web), MessageType.ERROR, errorKey);
            return false;
        }

        viewControl.setLaunchUrl(launchUrl);
        viewControl.setRetryTime(retryControl.getRetryDelayHint());
        return true;
    }

}
