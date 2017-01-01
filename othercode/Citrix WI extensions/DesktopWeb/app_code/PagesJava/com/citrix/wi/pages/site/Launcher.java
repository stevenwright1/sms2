// Launcher.java
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// [NFuseVersionAndBuildNumber]
package com.citrix.wi.pages.site;

import java.io.IOException;

import com.citrix.wi.IconCache;
import com.citrix.wi.controls.LauncherControl;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pages.StandardPage;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.LaunchUtilities;
import com.citrix.wi.pageutils.ResourceEnumerationUtils;
import com.citrix.wi.pageutils.SessionToken;
import com.citrix.wi.pageutils.SessionUtils;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wing.MessageType;
import com.citrix.wing.webpn.ResourceInfo;
import com.citrix.wing.webpn.UserContext;
import com.citrix.wing.util.WebUtilities;


public class Launcher extends StandardPage {

    protected LauncherControl viewControl = new LauncherControl();
    UserContext userContext;

    public Launcher(WIContext wiContext) {
        super(wiContext);
        wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
    }

    private void flagDirectLaunch(String appID, String launchOnly) {
        if (launchOnly == null || launchOnly.length() < 1) {
            LaunchUtilities.setSessionLaunchApp(wiContext, appID);

            // Save the Csrf token parameter for later (may be empty)
            LaunchUtilities.setSessionLaunchAppCsrfToken(wiContext, SessionToken.copyCsrfQueryToken(wiContext));
        }
    }

    /** Get the site path, which will be used as a base for constructing
     * links within this page.
     * Note, the code originally called UtilsASP.getWebAppURL (which returns
     * a full URL including protocol and authority), but this
     * caused a "mixed content" browser warning when connecting via SG, as
     * the page was requested via https but included
     * http links.
     */
    private String getSitePath() {
        String sitePath = wiContext.getWebAbstraction().getApplicationPath() + "/site/";
        return sitePath;
    }

    private void setupDefaultViewControl(String appID) {
        // no launch tag obtained yet
        viewControl.launchTag = "";

        // Is passthrough enabled?
        viewControl.isPassthroughEnabled = wiContext.getConfiguration().getEnablePassthroughURLs();

        // just in case the launcher Javascript finds out it is not in an iframe
        // construct a suitable error URL
        viewControl.passthroughErrorUrl = UIUtils.getMessageRedirectUrl(
                                            wiContext,
                                            Include.getHomePage(wiContext),
                                            MessageType.WARNING,
                                            "ShortcutDisabled");

        ResourceInfo resInfo = ResourceEnumerationUtils.getResource(appID, userContext);
        if (resInfo != null) {
            IconCache iconCache = Include.getIconCache(wiContext.getStaticEnvironmentAdaptor());
            String iconHash = iconCache.putIcon(resInfo.getIcon());
            String encodedIconId = WebUtilities.encodeForId(iconHash);

            viewControl.faviconLink = getSitePath() + Constants.PAGE_ICON + "?size=small&amp;id=" + encodedIconId;
            viewControl.decodedTitle = resInfo.getDisplayName();
        }
    }

    protected boolean performImp() throws IOException {
        boolean showForm = true;

        WebAbstraction web = wiContext.getWebAbstraction();
        web.setResponseContentType("text/html; charset=UTF-8");

        userContext = SessionUtils.checkOutUserContext(wiContext);

        // getCurrentLaunchApp() calls through to getQueryStringLaunchApp()
        // which does all the necessary decoding for us.
        String appID = LaunchUtilities.getCurrentLaunchApp(wiContext);

        setupDefaultViewControl(appID);

        // Unless told not to, store the launching appId in the session.
        // Storing the app in the session will cause the the applist or
        // the direct launch page to launch that app.


        boolean isDirectLaunch = LaunchUtilities.getDirectLaunchModeInUse(wiContext);
        if (isDirectLaunch) {
            // When refreshing the direct launch page via F5, there must not
            // be an app saved in the session, otherwise it will be launched.
            // Therefore, when the direct launch page fills in the launcher URL
            // in the launch iframe, the LAUNCH_ONLY parameter must be added
            // to stop the app being saved in the session.
            flagDirectLaunch(appID, web.getQueryStringParameter(LaunchUtilities.QSTR_LAUNCH_ONLY));
        }

        LaunchResource launcher = LaunchResource.CreateLauncher(appID, wiContext, userContext);
        LaunchResult result = launcher.launch(isDirectLaunch,
                                        userContext,
                                        getSitePath(),
                                        UIUtils.getMetricFromQueryString(wiContext, Constants.QSTR_METRIC_LAUNCH_ID));

        // In Direct launch mode do not clear launch session data if this is not a successfull
        // otherwise it will also remove the directlaunch session variable and switch to normal mode
        if (isDirectLaunch && !result.isSuccess()) {
            LaunchUtilities.clearSessionLaunchData(wiContext);
        }

        // do not show UI, as page will be redirected
        if (result.isRedirected()) {
            showForm = false;
        }
        viewControl.redirectUrl = result.redirectUrl;
        viewControl.launchTag = result.launchTag;

        SessionUtils.returnUserContext(userContext);
        return showForm;
    }

}

