// Retry.java
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// [NFuseVersionAndBuildNumber]
package com.citrix.wi.pages.site;

import java.io.IOException;

import com.citrix.wi.controls.DelayedLaunchControl;
import com.citrix.wi.controls.RetryControl;
import com.citrix.wi.controls.RetryPageViewControl;
import com.citrix.wi.controlutils.FeedbackMessage;
import com.citrix.wi.controlutils.FeedbackUtils;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pages.StandardPage;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.DelayedLaunchUtilities;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.LaunchUtilities;
import com.citrix.wi.pageutils.PageHistory;
import com.citrix.wi.pageutils.ResourceEnumerationUtils;
import com.citrix.wi.pageutils.SessionToken;
import com.citrix.wi.pageutils.SessionUtils;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wi.pageutils.Utils;
import com.citrix.wing.AppLaunchInfo;
import com.citrix.wing.AppLaunchParams;
import com.citrix.wing.DesktopUnavailableException;
import com.citrix.wing.MessageType;
import com.citrix.wing.ResourceUnavailableException;
import com.citrix.wing.RetryRequiredException;
import com.citrix.wing.LaunchOverride;
import com.citrix.wing.types.ClientType;
import com.citrix.wing.types.DesktopUnavailableErrorType;
import com.citrix.wing.util.Strings;
import com.citrix.wing.util.WebUtilities;
import com.citrix.wing.webpn.ApplicationInfo;
import com.citrix.wing.webpn.DesktopInfo;
import com.citrix.wing.webpn.ResourceInfo;
import com.citrix.wing.webpn.UserContext;


/**
 * This page retries the desktop which mean it tries to get the AppLaunchInfo.
 * If succeeds this page updates the delayed launch feedback UI using javascript
 * and then redirects to the launcher page to do the actual launch except in the case
 * when this is the last desktop to retry and ica file will not be blocked in which
 * case it redirects to the home page and the retry populator will handle the launch.
 * If retry-required exception is received again, it will redirect to DelayLaunchTimer to wait further.
 */
public class Retry extends StandardPage {
    private DelayedLaunchControl delayedLaunchControl;
    private RetryPageViewControl viewControl = new RetryPageViewControl();
    public Retry(WIContext wiContext) {
        super(wiContext);
        wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
    }

    protected boolean performImp() throws IOException {
        // This page accepts following query string
        // QSTR_RETRY_APPLICATION - the application id of the desktop that needs to be retried

        boolean showForm = true;

        UserContext userContext = SessionUtils.checkOutUserContext(wiContext);
        delayedLaunchControl = DelayedLaunchUtilities.getDelayedLaunchControl(wiContext);

        String retryApp = wiContext.getWebAbstraction().getQueryStringParameter(Constants.QSTR_RETRY_APPLICATION);
        ResourceInfo resInfo = ResourceEnumerationUtils.getResource(retryApp, userContext);

        // check if resource is available
        if ((resInfo == null) || !resInfo.isEnabled())
        {
            viewControl.redirectUrl = UIUtils.getMessageRedirectUrl(wiContext,
                Include.getHomePage(wiContext), MessageType.ERROR, "AppRemoved");
            viewControl.redirectMainWindow = true;
            // remove the resource from the awaiting list
            delayedLaunchControl.removePendingResource(retryApp);
            SessionUtils.returnUserContext(userContext);
            return true;
        }

        // check if resource is awaiting
        if (Strings.isEmpty(retryApp) || !delayedLaunchControl.isResourcePending(retryApp)) {
            // if no retryApp or delayedLaunchControl does not contain this app then return error
            viewControl.redirectUrl = UIUtils.getMessageRedirectUrl(wiContext,
                Include.getHomePage(wiContext), MessageType.ERROR, "RetryError");
            SessionUtils.returnUserContext(userContext);
            return true;
        }

        viewControl.desktopDisplayName = resInfo.getDisplayName();

        RetryControl retryControl = delayedLaunchControl.getPendingResourceRetryControl(retryApp);
        String retryKey = retryControl.getRetryKey();
        try {
            // Whether the desktop needs to first be powered off. When the launch is subsequently attempted
            // it will almost certainly need to be retried as the desktop will just be starting to boot up.
            boolean powerOff = retryControl.getPowerOff();
            if (powerOff) {
                retryControl.setPowerOff(false);

                boolean powerOffResult = doPowerOff(userContext, resInfo, delayedLaunchControl);
                if (!powerOffResult) {
                    // The power off didn't go as expected so stop the
                    // execution of the page here.
                    SessionUtils.returnUserContext(userContext);
                    return true;
                }
            }

            // Whether we need to keep a list of desktops that are ready to launch but cannot be auto-launched.
            // At present we only keep a history if we are using ICA file download to launch
            // and we expect an automatic download would be blocked.
            boolean canAutoLaunch = LaunchUtilities.canScriptLaunch(wiContext, resInfo);
            viewControl.canScriptLaunch = canAutoLaunch;

            AppLaunchParams appParams = new AppLaunchParams(retryControl.getClientType());
            appParams.setLaunchRetryKey(retryKey);

            if (ClientType.ICA_30.equals(retryControl.getClientType())) {
                LaunchOverride launchOverride = LaunchShared.getLaunchOverrideFromOverrideData(wiContext, userContext.getAccessPrefs().getLocale(),
                                                                               resInfo.getId(), ClientType.ICA_30);
                appParams.setLaunchOverride(launchOverride);
			}

            AppLaunchInfo launchInfo = (AppLaunchInfo)userContext.launchApp(retryApp, appParams);

            // Make sure the desktop is no longer on the list of restarting desktops
            delayedLaunchControl.removeRestartingResource(retryApp);
            
            // Store the launchInfo so that we don't need to get it again when we actually do the launch
            delayedLaunchControl.changePendingResourceToReady(retryApp, launchInfo);

            if (canAutoLaunch) {
                // If the browser doesn't block auto launches, then redirect to the launcher frame.
                viewControl.redirectUrl = Constants.PAGE_LAUNCHER
                               + "?" + LaunchUtilities.getLaunchAppQueryStringFragment(wiContext, retryApp)
                               + SessionToken.copyCsrfQueryToken(wiContext);
                if (isCurrentPageDirectLaunch()) {
                    // Indicate that the launcher page should skip adding the app to the SV_LAUNCH_APP session variable,
                    // to avoid triggering a further launch if the directlaunch page is refreshed.
                    viewControl.redirectUrl += "&" + LaunchUtilities.QSTR_LAUNCH_ONLY + "=y";
                }
            } else {
                delayedLaunchControl.changeReadyStatusToBlocked(retryApp);
            }

            String lightboxMessage = wiContext.getString("SwitchOff", WebUtilities.escapeHTML(viewControl.desktopDisplayName));
            viewControl.retrySuccessfulTag = getRetrySuccessfulTag(resInfo, canAutoLaunch, lightboxMessage);

            if (!isCurrentPageDirectLaunch())
            {
                // Add javascript to remove the connecting indicator from the tab header.

                // remove spinner from all resources tab if no resources are launching
                if (Include.isSingleAllResourcesTab(wiContext)) {
                    if (!delayedLaunchControl.isResourcePendings()) {
                        viewControl.retrySuccessfulTag += getUpdateHeaderTag(retryApp, Constants.TAB_NAME_ALL_RESOURCES);
                    }
                }
                else
                {
                    // if there are no waiting desktops...
                    if (!delayedLaunchControl.hasPendingDesktops()) {
                        viewControl.retrySuccessfulTag += getUpdateHeaderTag(retryApp, Constants.TAB_NAME_DESKTOPS);
                    }
                    // ...apps...
                    if (!delayedLaunchControl.hasPendingApps()) {
                        viewControl.retrySuccessfulTag += getUpdateHeaderTag(retryApp, Constants.TAB_NAME_APPS);
                    }
                    // ... or content
                    if (!delayedLaunchControl.hasPendingContent()) {
                        viewControl.retrySuccessfulTag += getUpdateHeaderTag(retryApp, Constants.TAB_NAME_CONTENT);
                    }
                }
            }
        } catch (ResourceUnavailableException rue) {
            Exception ex = (Exception)rue.getCause();
            if (ex instanceof RetryRequiredException) {
                handleDelay(resInfo, retryControl.getClientType(), (RetryRequiredException)ex);
            } else {
                handleFailedRetry(rue, retryApp, resInfo);
            }
        } catch (java.lang.Exception e) {
            handleFailedRetry(e, retryApp, resInfo);
        }
        
        SessionUtils.returnUserContext(userContext);

        return showForm;
    }

    private void handleFailedRetry(Exception e, String retryApp, ResourceInfo resInfo)
    {
    	delayedLaunchControl.removeRestartingResource(retryApp);
        delayedLaunchControl.removePendingResource(retryApp);
        FeedbackMessage message = LaunchUtilities.getLaunchErrorMessage(wiContext, e, resInfo);
        viewControl.redirectUrl = FeedbackUtils.getFeedbackUrl(wiContext, message, Include.getHomePage(wiContext));
        viewControl.redirectMainWindow = true;
    }

    /**
     * Performs the power off operation and handles any errors.
     *
     * @param uc the user context
     * @param resInfo the resource info of the desktop to power off
     * @param dlc the delayed launch control
     * @return <code>true</code> if the page should continue executing, otherwise <code>false</code>
     */
    private boolean doPowerOff(UserContext uc, ResourceInfo resInfo, DelayedLaunchControl dlc) {
        boolean continueProcessing = true;

        String desktopId = resInfo.getId();

        try {
            // Note, this call blocks until XD has completed the power-off operation.
            uc.powerOffDesktop(desktopId);
        } catch (DesktopUnavailableException due) {
            if (due.getErrorType() == DesktopUnavailableErrorType.NO_AVAILABLE_WORKSTATION) {
                // This implies that the power off operation was not a valid
                // thing to try at this point (i.e. nothing found to power off).

                // Ensure that the status text for the desktop reads
                // "starting" rather than "restarting"
                dlc.removeRestartingResource(desktopId);

                // Display an informational message
                // Use FeedbackUtils.getFeedbackUrl() so that the display name
                // cannot be tampered with via the query string
                FeedbackMessage message = new FeedbackMessage(MessageType.INFORMATION,
                    "RestartNotAllowedYet", new String[] { resInfo.getDisplayName() });

                // Append the retry flag to the home page to ensure that the
                // delayed launch continues to process
                String homePage = Include.getHomePage(wiContext) + "?" +
                                  Constants.QSTR_RETRY_APPLICATION + "=" +
                                  WebUtilities.escapeURL(desktopId);

                viewControl.redirectUrl = FeedbackUtils.getFeedbackUrl(wiContext, message, homePage);
                viewControl.redirectMainWindow = true;
                continueProcessing = false;
            } else if (due.getErrorType() == DesktopUnavailableErrorType.OPERATION_IN_PROGRESS) {
                // The server is already busy with another operation for this desktop.
                // Ignore this error (it may be caused by repeatedly pressing the Restart button)
            } else {
                handlePowerOffError(due, desktopId, dlc);
                continueProcessing = false;
            }
        } catch (Exception e) {
            handlePowerOffError(e, desktopId, dlc);
            continueProcessing = false;
        }

        return continueProcessing;
    }

    // Cancels the restart and displays a suitable error message
    private void handlePowerOffError(Exception e, String desktopId, DelayedLaunchControl dlc) {
        dlc.removePendingResource(desktopId);
        dlc.removeRestartingResource(desktopId);
        viewControl.redirectUrl = UIUtils.getMessageRedirectUrl(wiContext,
            Include.getHomePage(wiContext), MessageType.ERROR, Utils.getPowerOffErrorMessageKey(e));
        viewControl.redirectMainWindow = true;
    }

    /**
     * Handles the retry-required exception. It sets the redirect url to the delay launch timer page
     * and redirects in the hidden frame. It also adds the new retry-required exception in the awaiting desktop
     */
    private void handleDelay(ResourceInfo resInfo, ClientType client, RetryRequiredException rre) {
        String appId = resInfo.getId();
        viewControl.redirectUrl = Constants.PAGE_DELAY_LAUNCH_TIMER
                                + "?" + Constants.QSTR_RETRY_APPLICATION + "=" + WebUtilities.escapeURL(appId)
                                + SessionToken.copyCsrfQueryToken(wiContext);
        viewControl.redirectMainWindow = false;
        delayedLaunchControl.addPendingResource(resInfo, new RetryControl(appId, client, rre, false));
    }

    /**
     * Gets the javascript method tag when the given appId resource is successfully retried.
     * 
     * @param resInfo of the resource that was successful
     * @param canAutoLaunch whether the resource can be auto launched
     * @param lightboxMessage the updated message to be shown in the lightbox, if its already showing.
     * @return javascript method tag to update the delayed launch entry
     */
    private String getRetrySuccessfulTag(ResourceInfo resInfo, boolean canAutoLaunch, String lightboxMessage) {
        String launchLink = Include.escapeDoubleQuotes(DelayedLaunchUtilities.getLaunchLink(wiContext, resInfo));
        String javascript = "";

        String encodedAppId = WebUtilities.encodeForId(resInfo.getId());
        if (isCurrentPageAppList()) {
            if (canAutoLaunch) {
                javascript = "updateDelayedAutoLaunchUI(\"" + encodedAppId + "\",\"" + lightboxMessage + "\");\n";
            } else {
                javascript = "updateDelayedManualLaunchUI(\"" + encodedAppId + "\");\n";
            }
        } else if (isCurrentPageDirectLaunch()) {
            launchLink = Include.escapeDoubleQuotes("<p class=\"firstParagraph\">") + launchLink;
            javascript = "updateDelayedDirectLaunchUI(\"" + encodedAppId + "\",\"" + launchLink + "</p>\");\n";
        }
        return javascript;
    }

    /**
     * Gets the javascript method tag to update the delayed launch tab's header
     * @param appId the name of the retry frame will be the given app Id
     * @param tabName name of the tab to update
     * @return javascript method tag
     */
    private String getUpdateHeaderTag(String appId, String tabName) {
        return "hideTabHeaderConnectingIcon(\"" + WebUtilities.encodeForId(appId) + "\", \"" + tabName + "\");\n";
    }

    /**
     * Checks if the current page is applist page
     */
    private boolean isCurrentPageAppList() {
        String currentUrl = PageHistory.getCurrentPageURL(wiContext.getWebAbstraction());
        return currentUrl.endsWith(Constants.PAGE_APPLIST);
    }

    /**
     * Checks if the current page is direct launch page
     */
    private boolean isCurrentPageDirectLaunch() {
        String currentUrl = PageHistory.getCurrentPageURL(wiContext.getWebAbstraction());
        return currentUrl.endsWith(Constants.PAGE_DIRECT_LAUNCH);
    }

}
