/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */

package com.citrix.wi.pages.site;

import java.io.IOException;

import com.citrix.wi.controls.DelayedLaunchControl;
import com.citrix.wi.controls.DirectLaunchPageControl;
import com.citrix.wi.controlutils.FeedbackMessage;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pages.StandardLayout;
import com.citrix.wi.pageutils.AGEUtilities;
import com.citrix.wi.pageutils.ApplistUtils;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.DelayedLaunchUtilities;
import com.citrix.wi.pageutils.Embed;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.LaunchUtilities;
import com.citrix.wi.pageutils.ResourceEnumerationUtils;
import com.citrix.wi.pageutils.SessionUtils;
import com.citrix.wi.pageutils.clientdetection.DetectionUtils;
import com.citrix.wi.types.UserInterfaceBranding;
import com.citrix.wing.AppLaunchInfo;
import com.citrix.wing.AppLaunchParams;
import com.citrix.wing.MessageType;
import com.citrix.wing.ResourceUseException;
import com.citrix.wing.RetryRequiredException;
import com.citrix.wing.LaunchOverride;
import com.citrix.wing.types.ClientType;
import com.citrix.wing.util.WebUtilities;
import com.citrix.wing.webpn.DesktopInfo;
import com.citrix.wing.webpn.ResourceInfo;
import com.citrix.wing.webpn.UserContext;
import com.citrix.wi.controls.ResourceControl;

public class DirectLaunch extends StandardLayout {
    protected DirectLaunchPageControl viewControl = new DirectLaunchPageControl();
    private UserContext userContext = null;
    private DelayedLaunchControl delayedLaunchControl = null;

    public DirectLaunch(WIContext wiContext) {
        super(wiContext);
        delayedLaunchControl = DelayedLaunchUtilities.getDelayedLaunchControl(wiContext);
        wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
    }

    protected String getBrowserPageTitleKey() {
        return null;
    }

    private boolean getShowDesktopUI() throws IOException {
        boolean showDesktopUI = false;
        ResourceControl resource = viewControl.getResourceControl();
        if (resource != null) {
            ResourceInfo resInfo = ResourceEnumerationUtils.getResource(resource.id, userContext);
            showDesktopUI = (resInfo instanceof DesktopInfo);
        }
        return showDesktopUI;
    }

    protected boolean performImp() throws IOException {
        super.setupNavControl();

        //This method accepts following query strings
        // QSTR_RETRY_APPLICATION - When the direct launch was not successful, launcher redirects to this page
        //                          with this query string to indicate that the delayed launch feedback UI needs to be displayed
        //                          and the desktop needs to be retried.

        // If we are not in direct launch mode, we should never see this page.
        if (!LaunchUtilities.getDirectLaunchModeInUse(wiContext)) {
            wiContext.getWebAbstraction().clientRedirectToUrl(Constants.PAGE_APPLIST);
            return false;
        }

        userContext = SessionUtils.checkOutUserContext(wiContext);

        // Note, the order of these tests is important!
        if (isPowerOffRequest()) {
            performPowerOffDesktop(getPowerOffDesktop());
        } else if (isRefreshDelayedRequest()) {
            performRefreshDelayedLaunch(getRefreshDelayedApp(wiContext));
        } else if (isLaunchRequest()) {
            clearRestartingDesktop(getLaunchApp());
            performLaunch(getLaunchApp());
        } else {
            performRefreshImmediateLaunch(getRefreshApp(wiContext));
        }

        boolean showDesktopUI = getShowDesktopUI();
        viewControl.setShowDesktopUI(showDesktopUI);
        if (showDesktopUI) {
            layoutControl.showApplistBox = false;
            layoutControl.showBackgroundGradient = true;
        }

        recordCurrentPageURL();

        SessionUtils.returnUserContext(userContext);

        return true;
    }

    private boolean isPowerOffRequest() {
        return getPowerOffDesktop() != null;
    }

    private String getPowerOffDesktop() {
        String appId = getRefreshDelayedApp(wiContext);
        String powerOffDesktop = null;

        if (appId != null && delayedLaunchControl.hasPowerOffResource(appId)) {
            powerOffDesktop = appId;
        }

        return powerOffDesktop;
    }

    private void clearRestartingDesktop(String appId) {
        if (appId != null) {
            delayedLaunchControl.removeRestartingResource(appId);
        }
    }

    public static String getRefreshDelayedApp(WIContext wiContext) {
        // a delayed retry should be specified by query string, which allows us to have concurrent launches
        String retryApp = wiContext.getWebAbstraction().getQueryStringParameter(Constants.QSTR_RETRY_APPLICATION);
        // in case we lose the query string, double check against the session state
        if (retryApp == null) {
            DelayedLaunchControl dlc = DelayedLaunchUtilities.getDelayedLaunchControl(wiContext);
            String sessionApp = getRefreshApp(wiContext);
            if (sessionApp != null) {
                if (dlc.isResourcePending(sessionApp)) {
                    retryApp = sessionApp;
                }
            }
        }
        return retryApp;
    }

    private boolean isRefreshDelayedRequest() {
        return getRefreshDelayedApp(wiContext) != null;
    }

    private String getLaunchApp() {
        return LaunchUtilities.getSessionLaunchApp(wiContext);
    }

    private boolean isLaunchRequest() {
        return getLaunchApp() != null;
    }

    private static String getRefreshApp(WIContext wiContext) {
        return LaunchUtilities.getSessionDirectLaunchedApp(wiContext);
    }

    private void setupLaunchScript(final ResourceInfo resInfo) {
        // Launch in the hidden frame using some javascript
        // by setting the auto launch javascript property

        // The final argument to getAppLinkRaw sets the 'launch only'
        // parameter for the launch, which means the app
        // will be launched, but tells the launcher page not to
        // save the appId into the session which stops the F5
        // case from re-launching it. The parameter requires a
        // non-zero length value to indicate valid usage.
        String launchURL = Include.getAppLinkRaw(wiContext, resInfo.getId(), null, true);

        String launchJs = LaunchUtilities.getDirectAutoLaunchJavaScript(wiContext, resInfo, launchURL);
        autoLaunchJavaScriptControl.autoLaunchJavaScript = launchJs;
    }

    private String getLaunchErrorReason(ResourceInfo resInfo) {
        String errorCode = null;
        if (!DetectionUtils.clientsAvailableForDetection(wiContext, null)) {
            errorCode = "NoEnabledClient";
        } else if (resInfo == null || !resInfo.isEnabled()) {
            errorCode = "AppRemoved";
        }
        return errorCode;
    }

    private boolean performPowerOffDesktop(String appId) {
        delayedLaunchControl.removePendingResource(appId);
        delayedLaunchControl.removePowerOffResource(appId);

        ResourceInfo resInfo = ResourceEnumerationUtils.getResource(appId, userContext);

        String errorCode = getLaunchErrorReason(resInfo);
        if (errorCode != null) {
            setFeedback(MessageType.ERROR, errorCode);
        } else {
            // Arrange for the desktop to be delay-launched, with an initial power-off
            DelayedLaunchUtilities.addPowerOffDelayedLaunch(wiContext, getClientType(), resInfo);

            FeedbackMessage message = new FeedbackMessage(MessageType.INFORMATION,
                                                              "RestartWait",
                                                              new String[] { resInfo.getDisplayName() }
                                                             );
            message.setTransient(true);
            String url = com.citrix.wi.controlutils.FeedbackUtils.getFeedbackUrl(wiContext, message, Constants.PAGE_DIRECT_LAUNCH);
            wiContext.getWebAbstraction().clientRedirectToUrl(url);
        }
        return false;
    }

    private void performLaunch(final String appId) {
        final ResourceInfo resInfo = ResourceEnumerationUtils.getResource(appId, userContext);

        String errorCode = getLaunchErrorReason(resInfo);
        if (errorCode != null) {
            setFeedback(MessageType.ERROR, errorCode);
            return;
        }

        clearLaunchTrigger();
        recordLaunchedApp(appId);

        if (resourceNeedsBooting(appId, resInfo)) {
            setupConnectingUI(resInfo);
        } else if (LaunchUtilities.canScriptLaunch(wiContext, resInfo)) {
            setupAutoLaunchUI(resInfo);
        } else {
            setupManualLaunchUI(resInfo);
        }
        // Setup script launch for all cases, even if the script auto launch cannot happen.
        // This is needed to show the desktop icon, spinner and play transitions.
        // Note this MUST be done after setting the UI.
        setupLaunchScript(resInfo);
    }

    private void clearLaunchTrigger() {
        LaunchUtilities.clearSessionLaunchData(wiContext);
    }

    private void recordLaunchedApp(String appId) {
        LaunchUtilities.setSessionDirectLaunchedApp(wiContext, appId);
    }

    private void performRefreshImmediateLaunch(final String appId) {

        final ResourceInfo resInfo = ResourceEnumerationUtils.getResource(appId, userContext);

        String errorCode = getLaunchErrorReason(resInfo);
        if (errorCode != null) {
            setFeedback(MessageType.ERROR, errorCode);
        } else if (LaunchUtilities.canScriptLaunch(wiContext, resInfo)) {
            setupAutoLaunchUI(resInfo);
        } else {
            setupManualLaunchUI(resInfo);
        }
    }

    private void performRefreshDelayedLaunch(final String appId) {
        final ResourceInfo resInfo = ResourceEnumerationUtils.getResource(appId, userContext);
        String errorCode = getLaunchErrorReason(resInfo);

        setRetryQueryOnNavLinks();

        if (errorCode != null) {
            setFeedback(MessageType.ERROR, errorCode);
        } else if (delayedLaunchControl.isResourcePending(appId)) {
            setupConnectingUI(resInfo);
        } else if (LaunchUtilities.canScriptLaunch(wiContext, resInfo)) {
            setupAutoLaunchUI(resInfo);
        } else {
            setupManualLaunchUI(resInfo);
        }
    }

    private void setRetryQueryOnNavLinks() {
        // Ensure that we don't lose the query string when changing graphics mode or
        // going to the wizard. Putting this in the session would invalidate concurrent launches.
        // The url will be decoded by the platform in the next page, so to make sure it stays
        // intact we need to encode it twice.
        navControl.setExtraQuery(Constants.QSTR_RETRY_APPLICATION + "=" +
            WebUtilities.escapeURL(getRefreshDelayedApp(wiContext)));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // methods for setup viewcontrol
    private boolean isErrorBeingDisplayed() {
        FeedbackMessage message = feedbackControl.getFeedback();
        return (message != null && (message.getType() == MessageType.ERROR));
    }






    private void setupCommonLaunchUI(final ResourceInfo resInfo) {
        boolean isTreeView = false;
        boolean forceAppSize = true;
        Boolean showDescription = Boolean.TRUE;
        viewControl.setResourceControl(ResourceControl.fromResourceInfo(resInfo,
                                                                        wiContext,
                                                                        userContext,
                                                                        Constants.ICON_SIZE_32,
                                                                        isTreeView,
                                                                        forceAppSize,
                                                                        showDescription));
        boolean isAutoLaunch = !LaunchUtilities.browserBlocksLaunch(wiContext) && !isErrorBeingDisplayed();
        String appLaunchLink = DelayedLaunchUtilities.getLaunchLink(wiContext, resInfo, isAutoLaunch);
        viewControl.setAppLaunchLink(appLaunchLink);
        viewControl.getResourceControl().launchHref = Include.processAppLink(wiContext, resInfo.getId(), null, true, false);

        boolean isXDBranded = (Include.getSiteBranding(wiContext) == UserInterfaceBranding.DESKTOPS);
        viewControl.setLogOffHelpString(wiContext.getString(Include.logOffApplications(wiContext) ? "TipLogoutWIAndApps" :
            (isXDBranded ? "TipLogoutWIOnlyXD" : "TipLogoutWIOnly")));

        welcomeControl.setBody("");
        layoutControl.hasLightbox = !Include.isCompactLayout(wiContext);

        // Show the problem connecting link when captions are on, and the wizard is supported.
        navControl.setShowClientDetection(!DetectionUtils.hideWizardHelpLinks(wiContext));
        boolean ageEmbeddedMode = AGEUtilities.isAGEEmbeddedMode(wiContext);
        navControl.setShowGraphicsMode(ageEmbeddedMode ? false : wiContext.getConfiguration().getUIConfiguration().getAllowCustomizeLayout());
        navControl.setFullGraphics(!Include.isCompactLayout(wiContext));

        // Show the wizard link if we are in the incorrect zone and the wizard links are not disabled
        if (Include.getWizardState(wiContext).getIncorrectZone() && !DetectionUtils.hideWizardHelpLinks(wiContext)) {
            viewControl.setWizardLink("href=\"" + Constants.PAGE_WIZARD_PRE_INPUT
                        + "?" + Constants.QSTR_DETECT_CURRENT + "=" + Constants.VAL_TRUE
                        + "&" + Constants.QSTR_SHOW_ZONE + "=" + Constants.VAL_TRUE + "\"");
        } else {
            viewControl.setWizardLink(null);
        }
    }

    private void setupConnectingUI(final ResourceInfo resInfo) {
        setupCommonLaunchUI(resInfo);

        String appNameStringKey = isErrorBeingDisplayed() ? "DesktopLaunchWithError" :
                                  delayedLaunchControl.hasRestartingResource(resInfo.getId()) ? "DesktopRestartEllipsis" :
                                                         "DesktopLaunchEllipsis";
        viewControl.setAppNameDisplayTextKey(appNameStringKey);
    }

    private void setupAutoLaunchUI(final ResourceInfo resInfo) {
        setupCommonLaunchUI(resInfo);

        String appNameStringKey = null;
        if (isErrorBeingDisplayed()) {
            appNameStringKey = "DesktopLaunchWithError";
        } else {
            appNameStringKey = isRefreshDelayedRequest() ? "ReadyToConnect" : "DesktopLaunch";
        }

        viewControl.setAppNameDisplayTextKey(appNameStringKey);
    }

    private void setupManualLaunchUI(final ResourceInfo resInfo) {
        setupCommonLaunchUI(resInfo);

        String appNameStringKey = null;
        if (isErrorBeingDisplayed()) {
            appNameStringKey = "DesktopLaunchWithError";
        } else {
            if (isRefreshDelayedRequest()) {
                appNameStringKey = "ReadyToConnect";
            } else {
                viewControl.setIsAppNameBold(true);
            }
        }

        viewControl.setAppNameDisplayTextKey(appNameStringKey);
    }

    // End methods for setup viewcontrol
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Methods for testing for a delayed desktop
    /**
     * Gets whether the resource is currently shutdown and needs to be booted.
     * The only way to tell whether a resource needs to be booted is to try launching it and see whether we get
     * the retry-required error. However, this also has the side-effect of booting it which could cause this
     * function to return different results on each call. To get around that the result of the first call are cached and
     * then the cache results are returned for subsequent calls.
     */
    private boolean resourceNeedsBooting(final String appId, final ResourceInfo resource) {
        if (appId.equals(cachedResourceNeedsBootingKey)) {
            return cachedResourceNeedsBootingResult;

        } else if (cachedResourceNeedsBootingKey == null) {
            cachedResourceNeedsBootingKey = appId;
            cachedResourceNeedsBootingResult = resourceNeedsBootingInternal(appId, resource);
            return cachedResourceNeedsBootingResult;

        } else {
            throw new IllegalArgumentException("not implemented for multiple invocations with different parameters");
        }
    }
    private String cachedResourceNeedsBootingKey = null;
    private boolean cachedResourceNeedsBootingResult = false;

    private boolean resourceNeedsBootingInternal(final String appId, final ResourceInfo resInfo) {
        boolean result = false;
        if (!isDelayedResource(appId) && !LaunchUtilities.hasCachedLaunchInfo(wiContext, appId)) {
            try {
                AppLaunchParams appLaunchParams = new AppLaunchParams(getClientType());

                if (ClientType.ICA_30.equals(getClientType())) {
                    LaunchOverride launchOverride = LaunchShared.getLaunchOverrideFromOverrideData(wiContext, userContext.getAccessPrefs().getLocale(),
                                                                                   appId, ClientType.ICA_30);
                    appLaunchParams.setLaunchOverride(launchOverride);
                }

                AppLaunchInfo launchInfo = (AppLaunchInfo)userContext.launchApp(appId, appLaunchParams);
                LaunchUtilities.putCachedLaunchInfo(wiContext, appId, launchInfo);
            } catch (ResourceUseException ruex) {
                if (exceptionIsRetryRequired(ruex) && DelayedLaunchUtilities.isDelayedLaunchSupported(wiContext)) {
                    DelayedLaunchUtilities.addDelayedLaunch(wiContext, (RetryRequiredException)ruex.getCause(), getClientType(), resInfo);
                    result = true;
                }
            } catch (Exception e) { }
        }
        return result;
    }

    private ClientType getClientType() {
        return Embed.toWingClientType(Embed.getScriptedHostedAppLaunchClient(wiContext));
    }

    private boolean exceptionIsRetryRequired(ResourceUseException ex) {
        return ((ResourceUseException)ex).getCause() instanceof RetryRequiredException;
    }

    private boolean isDelayedResource(String appId) {
        boolean result = delayedLaunchControl.isResourcePending(appId) ||
                         delayedLaunchControl.isBlockedLaunch(appId);
        return result;
    }

    // End Methods for testing for a delayed desktop
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public static String propagateDirectLaunchInfo(WebAbstraction web, String nextPage) {
        // Utility method to help other pages (e.g. client wizard) preserve the direct launch
        // app info in the query string. There may be several direct launch pages open in
        // a single browser so we can't use session state to hold this data.
        String retryQuery = web.getQueryStringParameter(Constants.QSTR_RETRY_APPLICATION);
        if (retryQuery != null) {
            return WebUtilities.addQueryStringToURL(nextPage, Constants.QSTR_RETRY_APPLICATION, WebUtilities.escapeURL(retryQuery));
        } else {
            return nextPage;
        }
    }
}
