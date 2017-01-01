/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */

package com.citrix.wi.pageutils;

import java.util.Iterator;

import com.citrix.wi.controls.DelayedLaunchControl;
import com.citrix.wi.controls.ResourceControl;
import com.citrix.wi.controls.RetryControl;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pages.site.DirectLaunch;
import com.citrix.wi.types.UserInterfaceBranding;
import com.citrix.wing.RetryRequiredException;
import com.citrix.wing.types.ClientType;
import com.citrix.wing.util.WebUtilities;
import com.citrix.wing.webpn.ApplicationInfo;
import com.citrix.wing.webpn.DesktopInfo;
import com.citrix.wing.webpn.ResourceInfo;
import com.citrix.wing.webpn.UserContext;

public class DelayedLaunchUtilities {

    // session variable for storing desktops that requires retry after a delay
    private static final String SV_DELAYED_LAUNCH_CONTROL = "DelayedLaunchControl";

    // Constants for the progress indicator images
    // note currently the same image is used for both low and full graphics
    public static final String IMAGE_PROGRESS_INDICATOR = "ProgressLoaderLightBar.gif";
    public static final String IMAGE_PROGRESS_INDICATOR_LOW_GRAPHICS = "ProgressLoaderLightBar.gif";

    /**
     * Gets the html link for launching the delayed resource,
     * with appropriate text for when the browser blocks the launch.
     *
     * @param wiContext WIContext
     * @param resInfo resourceInfo of the resource
     * @return html launch link
     */
    public static String getLaunchLink(WIContext wiContext, ResourceInfo resInfo) {
        return getLaunchLink(wiContext, resInfo, !LaunchUtilities.browserBlocksLaunch(wiContext));
    }

    /**
     * Gets the html link for launching the delayed desktop
     *
     * @param wiContext WIContext
     * @param resInfo - used to calculate the link for the desktop
     * @param isAutoLaunch - if true, have a launch if doesn't appear message,
     * otherwise just have a launch message.
     *
     * @return the HTML markup for the link
     */
    public static String getLaunchLink(WIContext wiContext, ResourceInfo resInfo, boolean isAutoLaunch) {
        if (wiContext == null || resInfo == null) {
            return null;
        }

        boolean isDirectLaunchPage = LaunchUtilities.getDirectLaunchModeInUse(wiContext);
        String appLink = "id=\"launchDesktopLink\" " + Include.processAppLink(wiContext, resInfo.getId(), null, isDirectLaunchPage, false);

        String link = wiContext.getString("LaunchManually", appLink);
        if (isAutoLaunch && isDirectLaunchPage) {

            String key = "LaunchApplicationManuallyIfDoesNotAppear";
            if (resInfo instanceof DesktopInfo) {
                key = "LaunchDesktopManuallyIfDoesNotAppear";
            }
            link = wiContext.getString(key, appLink);
        }
        return link;
    }

    /**
     * This returns the url for the hidden retry iframe. If there are any awaiting desktops
     * then this returns the retry populator page url otherwise it returns the dummy page
     *
     * @param wiContext WIContext
     * @return url of the hidden retry frame.
     */
    public static String getRetryFramePage(WIContext wiContext) {
        String url = null;
        if (isRetryFrameRequired(wiContext)) {

            url = Constants.PAGE_RETRY_POPULATOR + "?" + SessionToken.makeCsrfQueryToken(wiContext);
            if (LaunchUtilities.getDirectLaunchModeInUse(wiContext)) {
                String appId = DirectLaunch.getRefreshDelayedApp(wiContext);
                url += "&" + Constants.QSTR_RETRY_APPLICATION + "=" + WebUtilities.escapeURL(appId);
            }
        } else {
            url = Constants.PAGE_DUMMY;
        }
        return url;
    }

    /**
     * Gets whether a retry frame is required for the current page because
     * there are desktops waiting to connect.
     *
     * @param wiContext WIContext
     * @return <code>true</code> if the frame is required.
     */
    public static boolean isRetryFrameRequired(WIContext wiContext) {
        DelayedLaunchControl delayedLaunchControl = DelayedLaunchUtilities.getDelayedLaunchControl(wiContext);
        return delayedLaunchControl.isResourcePendings() || delayedLaunchControl.isResourceReadyToLaunchs();
    }

    /**
     * Gets the delayedLaunchControl from the session. Creates and stores new control if none was present.
     * @param wiContext WIContext
     * @return DelayedLaunchControl object
     */
    public static DelayedLaunchControl getDelayedLaunchControl(WIContext wiContext) {
        DelayedLaunchControl delayedLaunchControl = (DelayedLaunchControl)wiContext.getWebAbstraction().getSessionAttribute(SV_DELAYED_LAUNCH_CONTROL);
        if (delayedLaunchControl == null) {
            delayedLaunchControl = new DelayedLaunchControl();
            wiContext.getWebAbstraction().setSessionAttribute(SV_DELAYED_LAUNCH_CONTROL, delayedLaunchControl);
        }
        return delayedLaunchControl;
    }

    /**
     * Returns a circular progress image in Compact Layout otherwise returns progress bar
     * @param wiContext the WIContext
     * @return file name of the progress image
     */
    public static String getDelayedLaunchProgressImg(WIContext wiContext) {
        return (Include.isCompactLayout(wiContext) || AGEUtilities.isAGEEmbeddedMode(wiContext)) ? IMAGE_PROGRESS_INDICATOR_LOW_GRAPHICS : IMAGE_PROGRESS_INDICATOR;
    }

    /**
     * Sets up the session info to keep track of a a delayed-launch resource
     *
     * @param wiContext the WIContext
     * @param rrex the RetryRequiredException for the failed resource launch attempt
     * @param client the WING ClientType for the failed resource launch attempt
     * @param resInfo the resource info used to identify the resource when retrieving the info later
     */
    public static void addDelayedLaunch(WIContext wiContext, RetryRequiredException rrex, ClientType client, ResourceInfo resInfo) {
        DelayedLaunchControl delayedLaunchControl = DelayedLaunchUtilities.getDelayedLaunchControl(wiContext);

        // store the Retry Control in the delayedLaunchControl so that they can be re-populated when the user
        // navigates to another page
        boolean powerOff = false;
        String appId = resInfo.getId();
        delayedLaunchControl.addPendingResource(resInfo, new RetryControl(appId, client, rrex, powerOff));
    }

    /**
     * Sets up the session to keep track of a delayed-launch resource, with an initial power-off
     *
     * @param wiContext the WIContext
     * @param client the WING ClientType for the failed resource launch attempt
     * @param resInfo the resource info used to identify the resource when retrieving the info later
     */
    public static void addPowerOffDelayedLaunch(WIContext wiContext, ClientType client, ResourceInfo resInfo) {
        DelayedLaunchControl delayedLaunchControl = DelayedLaunchUtilities.getDelayedLaunchControl(wiContext);

        // Add the desktop to the delayed launch list, using a "synthetic" retry exception and specifying that a power-off
        // is required. No time delay is set in the exception so that the delayLaunchTimer page immediately redirects to
        // the retry page. The retry page (requested from a hidden iframe) then performs the power-off operation, which will
        // block for some time.
        // Note, if the power-off were done while processing the DirectLaunch/Applist page, the page would not be able to render
        // its UI until the power-off completed.
        RetryRequiredException rrex = new RetryRequiredException(null);
        boolean powerOff = true;
        String appId = resInfo.getId();
        delayedLaunchControl.addPendingResource(resInfo, new RetryControl(appId, client, rrex, powerOff));
    }

    /**
     * Gets whether the current platform (OS/Browser) can support using delayed launches
     * @param wiContext the WIContext
     * @return <code>true</code> if delayed launches are supported
     */
    public static boolean isDelayedLaunchSupported(WIContext wiContext) {
        // Not supported on Windows Mobile due to lack of iframe support
        return !wiContext.getClientInfo().osPocketPC();
    }

}
