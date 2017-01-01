// LaunchResource.java
// Copyright (c) 2009 - 2010 Citrix Systems, Inc. All Rights Reserved.
// [NFuseVersionAndBuildNumber]
package com.citrix.wi.pages.site;

import com.citrix.wi.controls.DelayedLaunchControl;
import com.citrix.wi.controlutils.FeedbackMessage;
import com.citrix.wi.controlutils.FeedbackUtils;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.DelayedLaunchUtilities;
import com.citrix.wi.pageutils.Embed;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.LaunchUtilities;
import com.citrix.wi.pageutils.ResourceEnumerationUtils;
import com.citrix.wi.pageutils.SessionToken;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wi.pageutils.clientdetection.DetectionUtils;
import com.citrix.wing.AppLaunchInfo;
import com.citrix.wing.AppLaunchParams;
import com.citrix.wing.ICAFile;
import com.citrix.wing.MessageType;
import com.citrix.wing.ResourceUnavailableException;
import com.citrix.wing.RetryRequiredException;
import com.citrix.wing.LaunchOverride;
import com.citrix.wing.types.AppAccessMethod;
import com.citrix.wing.types.ClientType;
import com.citrix.wing.util.WebUtilities;
import com.citrix.wing.webpn.AccessMethod;
import com.citrix.wing.webpn.ApplicationInfo;
import com.citrix.wing.webpn.DesktopInfo;
import com.citrix.wing.webpn.DocumentInfo;
import com.citrix.wing.webpn.ResourceInfo;
import com.citrix.wing.webpn.UserContext;


/**
 * Class offers common functionality for launching resources.
 */
abstract public class LaunchResource
{
    private final ResourceInfo resInfo;
    protected WIContext wiContext;
    protected String appID;

    static public LaunchResource CreateLauncher(String appID, WIContext wiContext, UserContext userContext)
    {
        LaunchResource launcher;
        ResourceInfo resInfo = ResourceEnumerationUtils.getResource(appID, userContext);

        if (resInfo instanceof DocumentInfo || (resInfo != null && resInfo.isAccessMethodAvailable(AccessMethod.LOCATION)))
        {
            launcher = new LaunchDocument(wiContext, (DocumentInfo)resInfo, appID);
        }
        else if (resInfo instanceof DesktopInfo)
        {
            launcher = new LaunchDesktop(wiContext, (DesktopInfo)resInfo, appID);
        }
        else
        {
            launcher = new LaunchApplication(wiContext, (ApplicationInfo)resInfo, appID);
        }
        return launcher;
    }

    LaunchResource(WIContext wiContext, ResourceInfo resInfo, String appID)
    {
        this.wiContext = wiContext;
        this.resInfo = resInfo;
        this.appID = appID;
    }

    protected boolean useRade()
    {
        boolean bStreamingApp = resInfo.isAccessMethodAvailable(AccessMethod.STREAM);
        boolean bRemoteApp = resInfo.isAccessMethodAvailable(AccessMethod.DISPLAY);

        boolean useRade = ((Include.getEffectiveAppAccessMethod(wiContext) == AppAccessMethod.STREAMING) && bStreamingApp)
                        || !bRemoteApp;
        return useRade;
    }

    public boolean isDelayedLaunchSupported()
    {
        boolean isSupported = DelayedLaunchUtilities.isDelayedLaunchSupported(wiContext) && !useRade();
        return isSupported;
    }

    /**
     * Gets the url to redirect when retry-required exception is received from WING.
     * In Direct Launch Mode, query string is added with the given appId to indicate delayed launch.
     * This is because when you launch a resource we dont want to show all the resources that are being retried.
     * For normal launch we use session variable and since the launch happen straight away there is no problem,
     * whereas with resource there might be several delayed launches so we use query string for each direct launch
     * to handle that resource.
     *
     * @param appID application ID
     */
    protected String getRetryRedirectUrl()
    {
        String url = null;
        if (LaunchUtilities.getDirectLaunchModeInUse(wiContext))
        {
            url = Constants.PAGE_DIRECT_LAUNCH + "?" + Constants.QSTR_RETRY_APPLICATION + "=" + WebUtilities.escapeURL(appID);
        }
        else
        {
            FeedbackMessage message = new FeedbackMessage(MessageType.INFORMATION,
                                                          "DelayedLaunchWait",
                                                          new String[] { resInfo.getDisplayName() }
                                                         );
            message.setTransient(true);
            url = FeedbackUtils.getFeedbackUrl(wiContext, message, Constants.PAGE_APPLIST);
        }
        return url;
    }


    /* This method clears the given appId from the delayedLaunchControl from both
     * awaiting resource list and ready to launch resource list.
     */
    protected void clearDelayedLaunchSessionData()
    {
        DelayedLaunchControl delayedLaunchControl = DelayedLaunchUtilities.getDelayedLaunchControl(wiContext);
        delayedLaunchControl.removePendingResource(appID);
        delayedLaunchControl.removeReadyToLaunchResource(appID);
    }

    protected LaunchResult tryDelayedLaunch(UserContext userContext)
    {
        ClientType client = Embed.toWingClientType(Embed.getScriptedHostedAppLaunchClient(wiContext));
        LaunchResult result = new LaunchResult();

        DelayedLaunchControl delayedLaunchControl = DelayedLaunchUtilities.getDelayedLaunchControl(wiContext);

        // Need to get the AppLaunchInfo when launching because it may return
        // a retry required exception. So we only want to launch it if successful
        try
        {
            // Store the launchInfo in the session so it doesn't need to be get again on launch.ica request.
            // LaunchInfo will be removed when we have generated the ica file in launch.ica.
            // If this is a successful launch after retry then don't add it since it's already been added while retrying.
            if (!delayedLaunchControl.isResourceReadyToLaunch(appID) && !LaunchUtilities.hasCachedLaunchInfo(wiContext, appID))
            {
                AppLaunchParams appLaunchParams = new AppLaunchParams(client);
                if (ClientType.ICA_30.equals(client)) {
                    LaunchOverride launchOverride = LaunchShared.getLaunchOverrideFromOverrideData(wiContext, userContext.getAccessPrefs().getLocale(),
                                                                                   appID, client);
                    appLaunchParams.setLaunchOverride(launchOverride);
                }
                // ^ may already have cached launch info if coming from the direct launch page where it tries
                //   launching the resource in the background just to find out if it's running
                // calling the next line may return retry-required exception
                AppLaunchInfo launchInfo = (AppLaunchInfo)userContext.launchApp(appID, appLaunchParams);

                // if successfully got launchInfo then store it in the session
                LaunchUtilities.putCachedLaunchInfo(wiContext, appID, launchInfo);
            }
            result.resultCode = LaunchResult.SUCCESS;
        }
        catch (ResourceUnavailableException rue)
        {
            Exception ex = (Exception)rue.getCause();
            if (ex instanceof RetryRequiredException)
            {
                result = handleDelayedLaunch(client, (RetryRequiredException)ex);
                return result;
            }
            else
            {
                result = getLaunchErrorUrl(rue);
            }
        }
        catch (Exception e)
        {
            result = getLaunchErrorUrl(e);
        }

        return result;
    }

    private LaunchResult getLaunchErrorUrl(Exception ex) {
        LaunchResult result = new LaunchResult();

        FeedbackMessage message = LaunchUtilities.getLaunchErrorMessage(wiContext, ex, resInfo);
        result.resultCode = LaunchResult.ERR_LAUNCH_ERROR;
        result.redirectUrl = FeedbackUtils.getFeedbackUrl(wiContext, message, Include.getHomePage(wiContext));

        return result;
    }

    private LaunchResult appAndClientsAvailable()
    {
        LaunchResult result = new LaunchResult();
        result.resultCode = LaunchResult.SUCCESS;

        // application is removed
        if (resInfo == null || !resInfo.isEnabled())
        {
            result.resultCode = LaunchResult.ERR_APP_REMOVED;
            result.redirectUrl = UIUtils.getMessageRedirectUrl(wiContext, Include.getHomePage(wiContext), MessageType.ERROR, "AppRemoved");
        }
        else if (!DetectionUtils.clientsAvailableForDetection(wiContext, null))
        {
            // None of the admin-enabled clients can be run on this platform
            result.resultCode = LaunchResult.ERR_NO_CLIENTS;
            result.redirectUrl = UIUtils.getMessageRedirectUrl(wiContext, Include.getHomePage(wiContext), MessageType.ERROR, "NoEnabledClient");
        }
        else if (useRade() && !Include.getWizardState(wiContext).isRADEClientAvailable())
        {
            result.resultCode = LaunchResult.ERR_NO_RADE_CLIENTS;
            String errorKey = Include.getStreamingLaunchFailureReasonKey(wiContext.getUserEnvironmentAdaptor(), wiContext.getClientInfo());
            result.redirectUrl = UIUtils.getMessageRedirectUrl(wiContext, Include.getHomePage(wiContext), MessageType.ERROR, errorKey);
        }

        return result;

    }


    private void cleanUpOnFailedLaunch(LaunchResult launchResult, boolean isDirectLaunch)
    {
        // DELAYED_LAUNCH indicates delayed launch in progress. Do not clean delayed launch control in this case
        if (launchResult.resultCode != LaunchResult.DELAYED_LAUNCH)
        {
            // clear the app id from the delayed launch control since there was an error
            clearDelayedLaunchSessionData();
        }

        if (!isDirectLaunch)
        {
            LaunchUtilities.clearSessionLaunchData(wiContext);
        }
    }

    public LaunchResult launch(boolean isDirectLaunch, UserContext userContext, String sitePath, long EUEMlaunchTime)
    {
        LaunchResult result = appAndClientsAvailable();

        if (!result.isSuccess())
        {
            // The launch has failed because the client is not available.
            // If in direct launch mode, need to set the application ID into
            // the launched app variable to record the launch attempt, as this
            // render of launcher may be the result of a bookmark being used
            // in a session where a launch has already taken place. Launcher
            // runs first for a bookmark launch so the direct launch code may
            // not have been reached yet for this launch attempt. Failing to
            // set this launch attempt will cause direct launch to display the
            // application info from the previous direct mode bookmark launch.
            if (isDirectLaunch)
            {
                LaunchUtilities.setSessionDirectLaunchedApp(wiContext, appID);
            }
            cleanUpOnFailedLaunch(result, isDirectLaunch);
            return result;
        }

        result.resultCode = LaunchResult.SUCCESS;
        if (isDelayedLaunchSupported())
        {
            result = tryDelayedLaunch(userContext);
        }

        if (result.isSuccess())
        {
            String launchQueryString = buildLaunchQueryString(sitePath, EUEMlaunchTime);

            result = (useRade()) ? launchRade(sitePath, launchQueryString) : launchHosted(sitePath, launchQueryString);

        }
        // If it was a continuation of launch, remove the appId from the blocked launches list.
        DelayedLaunchControl delayedLaunchControl = DelayedLaunchUtilities.getDelayedLaunchControl(wiContext);
        if (!isDirectLaunch && delayedLaunchControl.isBlockedLaunch(appID))
        {
            delayedLaunchControl.removeBlockedLaunch(appID);
        }

        // some clean up is needed in fail cases
        if (!result.isSuccess()) {
            cleanUpOnFailedLaunch(result, isDirectLaunch);
        }

        return result;
    }

    private String buildLaunchQueryString(String sitePath, long EUEMlaunchTime)
    {
        String queryStr = "?" + LaunchUtilities.getLaunchAppQueryStringFragment(wiContext, appID) +
                          "&" + Constants.QSTR_APP_FRIENDLY_NAME_URLENCODED +
                          "=" + WebUtilities.escapeURL(resInfo.getDisplayName()) + SessionToken.copyCsrfQueryToken(wiContext);

        // Add the EUEM data from the query string
        if (EUEMlaunchTime != -1)
        {
            queryStr += "&" + Constants.QSTR_METRIC_LAUNCH_ID + "=" + EUEMlaunchTime;
        }

        return queryStr;
    }

    protected LaunchResult launchRade(String sitePath, String queryStr)
    {
        // Indicate to subsequent pages that this launch should be done via the RADE client
        queryStr += "&" + Constants.QSTR_LAUNCH_METHOD + "=" + Constants.LAUNCH_METHOD_STREAMING;

        String launchUrl = sitePath + Constants.PAGE_STREAMING_LAUNCH + queryStr;

        // determine whether to launch via the RCO or by download the .RAD launch file
        boolean bEmbed = Embed.isScriptedStreamingAppLaunch(wiContext);
        LaunchResult result = (bEmbed) ? getEmbedLaunchTag("", sitePath, queryStr, false) : getFTAlaunchTag(launchUrl);
        return result;
    }

    protected LaunchResult launchHosted(String sitePath, String queryStr)
    {
        String client = Embed.getScriptedHostedAppLaunchClient(wiContext);
        String launchUrl = sitePath + Constants.PAGE_LAUNCH + queryStr;

        boolean bEmbed = Embed.isScriptedHostedAppLaunch(wiContext);
        boolean launchInNewWindow = !Embed.ICA_ICOCLIENT.equals(client);
        LaunchResult result = (bEmbed) ? getEmbedLaunchTag(client, sitePath, queryStr, launchInNewWindow) : getFTAlaunchTag(launchUrl);
        return result;
    }

	private String getLaunchTag(String launchUrl) {
		String launchCodeTag = "document.location.replace('" + launchUrl + "');";

	    // Older versions of Opera were blocking ICA file download due to overly restrictive security settings.
        // The issue has been confirmed on Opera Mobile 9.7 and Windows Opera 9.6. They both use Presto 2.1.1 rendering engine.
        // Applying workaround for all versions < 10.
		boolean isOperaVersionPriorTo10 = wiContext.getClientInfo().isOpera() && wiContext.getClientInfo().getBrowserVersionMajor() < 10;
		// This is to work around the silent launch fails on Safari mobile (e.g. iPad and iPhone versions).
		boolean isSafariMobile = wiContext.getClientInfo().isSafariMobile();
        if (isOperaVersionPriorTo10 || isSafariMobile) {
            launchCodeTag = "redirectToMainFrame('" + launchUrl + "');";
        }
        
		return launchCodeTag;
	}

    protected LaunchResult getFTAlaunchTag(String launchUrl)
    {
	    // Launch by downloading a launch file; desktop file-type-association
		// or mime type settings should cause the file to be opened with the client in most instances.
        LaunchResult result = new LaunchResult();

        if (wiContext.getClientInfo().isIE()) {
            // Stop IE's annoying popups, if doing FTA launch.
            // The popup would also stop reconnect at login working
            result.resultCode = LaunchResult.REDIRECTED;
            wiContext.getWebAbstraction().clientRedirectToUrl(launchUrl);
        } else {
            result.launchTag = getLaunchTag(launchUrl);
            result.resultCode = LaunchResult.SUCCESS;
        }
        return result;
    }

    private LaunchResult getEmbedLaunchTag(String client, String sitePath, String queryStr, boolean launchInNewWindow)
    {
        // If JICA seamless, need to use custom size and title
        String encodedTitle = null;
        boolean isSeamless = wiContext.getUserPreferences().getUseSeamless();
        String desiredHRES = null;
        String desiredVRES = null;

        if (Embed.ICA_JAVACLIENT.equals(client) && isSeamless)
        {
            encodedTitle = WebUtilities.escapeURL(wiContext.getString("ICAConnectionCenter"));
            desiredHRES = Constants.ICA_CONN_CENTER_HRES;
            desiredVRES = Constants.ICA_CONN_CENTER_VRES;
        }
        else
        {
            encodedTitle = WebUtilities.escapeURL(resInfo.getDisplayName());
            ApplicationInfo appInfo = (resInfo instanceof DocumentInfo) ? ((DocumentInfo)resInfo).getLaunchingAppInfo()
                                                                        : (ApplicationInfo)resInfo;
            Embed.WindowDimensions winDims = new Embed.WindowDimensions(wiContext,
                                                                        appInfo,
                                                                        UIUtils.getScreenWidth(wiContext),
                                                                        UIUtils.getScreenHeight(wiContext));
            desiredHRES = winDims.getHRES();
            desiredVRES = winDims.getVRES();
        }
        queryStr += "&" + Constants.QSTR_LAUNCH_WINDOW_WIDTH + "=" + desiredHRES
                  + "&" + Constants.QSTR_LAUNCH_WINDOW_HEIGHT + "=" + desiredVRES
                  + "&" + Constants.QSTR_TITLE + "=" + encodedTitle;

        String appEmbedPageUrl = sitePath + Constants.PAGE_APPEMBED + queryStr;

        LaunchResult result = new LaunchResult();
        result.resultCode = LaunchResult.SUCCESS;

        if (launchInNewWindow)
        {
            // client to be launched via an activeX control or plugin in a new browser window
            result.launchTag = "appEmbed('" + appEmbedPageUrl + "', '" + desiredHRES + "', '" + desiredVRES + "');";
        }
        else
        {
            // client to be launched via an activeX control or plugin, but *not* in a new browser window
            result.launchTag = "document.location.replace('" + appEmbedPageUrl + "');";
        }

        return result;
    }

    /**
     * This method handles the retry required exception when trying to launch a resource.
     * It also adds the appID to the awaiting resources list in the delayedLaunchControl.
     * The method returns the URL to the main window to display launching tab.
     *
     * @param client Client to launch with.
     * @param ex RetryRequiredException
     */
    protected LaunchResult handleDelayedLaunch(ClientType client, RetryRequiredException ex)
    {
        LaunchResult result = new LaunchResult();
        // Redirect the main window for two reasons. One to display the delayed launch tab
        // and secondly to load the retryPopulator page in the hidden frame which creates
        // iframe for each delayed launch

        // copy the csrf token from the launcher query string to be included in the retry.
        result.redirectUrl = getRetryRedirectUrl() + SessionToken.copyCsrfQueryToken(wiContext);

        DelayedLaunchUtilities.addDelayedLaunch(wiContext, ex, client, resInfo);
        result.resultCode = LaunchResult.DELAYED_LAUNCH;
        return result;
    }
}
