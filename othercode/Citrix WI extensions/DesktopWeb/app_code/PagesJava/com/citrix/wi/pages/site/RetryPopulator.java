/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */

package com.citrix.wi.pages.site;

import java.io.IOException;
import java.util.Iterator;

import com.citrix.wi.controls.DelayedLaunchControl;
import com.citrix.wi.controls.RetryPopulatorControl;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pages.StandardPage;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.DelayedLaunchUtilities;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.LaunchUtilities;
import com.citrix.wi.pageutils.SessionToken;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wing.MessageType;
import com.citrix.wing.util.WebUtilities;

/**
 * This does not have any UI but needs to set the hidden iframes for each delayed launch
 * This page is called from inside the hidden iframe. This page creates iframe for each desktop
 * that is present in the awaitingDesktops table and the source of those hidden iframes is delayLaunchTimer.aspx/jsp
 * which then sets the javascript timer
 * This page also creates iframe for each desktop that is ready to launch whose source is the launcher page
 */
public class RetryPopulator extends StandardPage {
    RetryPopulatorControl viewControl = new RetryPopulatorControl();
    DelayedLaunchControl delayedLaunchControl;

    public RetryPopulator(WIContext wiContext) {
        super(wiContext);
        wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
    }

    public boolean performImp() throws IOException {
        delayedLaunchControl = DelayedLaunchUtilities.getDelayedLaunchControl(wiContext);
        if (!delayedLaunchControl.isResourcePendings() && !delayedLaunchControl.isResourceReadyToLaunchs()) {
            // we shouldn't be here if it is null or there are no awaiting desktops nor ready to launch desktops
            viewControl.redirectUrl = UIUtils.getMessageRedirectUrl(wiContext,
                Include.getHomePage(wiContext), MessageType.ERROR, "RetryError");
        }

        String bufferedHtml = "";
        String appName = null;
        if (LaunchUtilities.getDirectLaunchModeInUse(wiContext)) {
            bufferedHtml = processQueryString();
        } else {
            // for each awaiting desktop create an iframe and delayLaunchTimer as the src
            for (Iterator it = delayedLaunchControl.getPendingResourceIds().iterator(); it.hasNext(); ) {
                appName = (String)it.next();
                String src = Constants.PAGE_DELAY_LAUNCH_TIMER + "?" + Constants.QSTR_RETRY_APPLICATION + "=" + WebUtilities.escapeURL(appName)
                           + SessionToken.copyCsrfQueryToken(wiContext);
                bufferedHtml += getFrameHtml(appName, src);
            }
            // for each desktop ready to launch create an iframe and launcher as the src
            for (Iterator it = delayedLaunchControl.getReadyToLaunchResourceIds().iterator(); it.hasNext(); ) {
                appName = (String)it.next();
                String src = Constants.PAGE_LAUNCHER + "?" + LaunchUtilities.getLaunchAppQueryStringFragment(wiContext, appName)
                           + SessionToken.copyCsrfQueryToken(wiContext);
                bufferedHtml += getFrameHtml(appName, src);
            }
        }


        viewControl.setRetryIframesHtml(bufferedHtml);
        return true;
    }

    /**
     * Gets the iframe html with frame id as appName and src as the given src
     */
    private String getFrameHtml(String appName, String src) {
        String bufferedHtml = "<iframe width=\"0\" height=\"0\" name=\"" + WebUtilities.encodeForId(appName) + "\" class=\"HiddenIframe\" id=\"" + WebUtilities.encodeForId(appName) + "\" src=\"" + src + "\"></iframe>\n";
        return bufferedHtml;
    }

    private String processQueryString(){
        //This page only accepts QSTR_RETRY_APPLICATION which is validated against the awaiting
        // desktops list of the delayed launch control.
        String bufferedHtml = "";
        String retryApp = wiContext.getWebAbstraction().getQueryStringParameter(Constants.QSTR_RETRY_APPLICATION);
        // check that the retryApp is a valid awaiting desktop
        if (delayedLaunchControl.isResourcePending(retryApp)) {
            String retryUrl = Constants.PAGE_DELAY_LAUNCH_TIMER + "?" + Constants.QSTR_RETRY_APPLICATION + "=" + WebUtilities.escapeURL(retryApp)
                + SessionToken.copyCsrfQueryToken(wiContext);
            bufferedHtml = getFrameHtml(retryApp, retryUrl);
        } else if (delayedLaunchControl.isResourceReadyToLaunch(retryApp)) {
            String src = Constants.PAGE_LAUNCHER + "?" + LaunchUtilities.getLaunchAppQueryStringFragment(wiContext, retryApp)
                           + SessionToken.copyCsrfQueryToken(wiContext);
            bufferedHtml = getFrameHtml(retryApp, src);
        }
        return bufferedHtml;
    }




}
