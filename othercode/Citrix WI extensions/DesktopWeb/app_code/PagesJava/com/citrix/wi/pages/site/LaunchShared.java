/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.site;

import java.io.IOException;
import java.io.Reader;
import java.util.Date;
import java.util.Locale;
import java.util.Random;

import com.citrix.wi.controls.DelayedLaunchControl;
import com.citrix.wi.controls.DesktopGroupsController;
import com.citrix.wi.controls.LaunchControl;
import com.citrix.wi.controlutils.FeedbackMessage;
import com.citrix.wi.controlutils.FeedbackUtils;
import com.citrix.wi.metrics.PerformanceMetrics;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pages.StandardPage;
import com.citrix.wi.pageutils.AGEUtilities;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.DelayedLaunchUtilities;
import com.citrix.wi.pageutils.DesktopLaunchHistory;
import com.citrix.wi.pageutils.Embed;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.LaunchUtilities;
import com.citrix.wi.pageutils.ResourceEnumerationUtils;
import com.citrix.wi.pageutils.SessionToken;
import com.citrix.wi.pageutils.SessionUtils;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wi.types.BandwidthProfilePreference;
import com.citrix.wi.util.ClientInfo;
import com.citrix.wi.util.Platform;
import com.citrix.wi.util.Trace;
import com.citrix.wing.AppLaunchInfo;
import com.citrix.wing.AppLaunchParams;
import com.citrix.wing.ICAConstants;
import com.citrix.wing.ICAFile;
import com.citrix.wing.LaunchOverride;
import com.citrix.wing.MessageType;
import com.citrix.wing.RADFile;
import com.citrix.wing.StreamedAppLaunchInfo;
import com.citrix.wing.types.AppAccessMethod;
import com.citrix.wing.types.AppDisplaySize;
import com.citrix.wing.types.BandwidthProfile;
import com.citrix.wing.types.ClientType;
import com.citrix.wing.util.Locales;
import com.citrix.wing.util.Strings;
import com.citrix.wing.util.Throwables;
import com.citrix.wing.webpn.ResourceInfo;
import com.citrix.wing.webpn.UserContext;

/**
 * Page used for ICA and RADE launches
 */
public abstract class LaunchShared extends StandardPage
{
    protected Random random = new Random();
    protected LaunchControl viewControl = new LaunchControl();
    protected PerformanceMetrics metrics = null;
    private PerformanceMetrics reconnectMetrics = null;

    public LaunchShared(WIContext wiContext, PerformanceMetrics metrics) {
        super(wiContext);
        wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
        this.metrics = metrics;
    }

    // Method overridden to enable cache. Remove to disable.
    protected void setResponseCaching() {
    }

    private String getReconnectTime() {
        return wiContext.getWebAbstraction().getQueryStringParameter(Constants.QSTR_METRIC_RECONNECT_ID);
    }

    private void addMetrics(String nfuseUID) {
        WebAbstraction web = wiContext.getWebAbstraction();
        String reconnectTime = getReconnectTime();
        if (Strings.isLong(reconnectTime, 10)) {
            reconnectMetrics = (PerformanceMetrics)web.getSessionAttribute(reconnectTime);
        }

        long launchTime = UIUtils.getMetricFromQueryString(wiContext, Constants.QSTR_METRIC_LAUNCH_ID);

        if (reconnectMetrics != null) {
            // Reconnect and launch scenario

            // Add the reconnect metrics
            metrics.addAll(reconnectMetrics);

            // Set the session start metric to be the reconnect "click" time.
            metrics.addMilliTiming(Constants.METRIC_START_SCD, metrics.getTiming(Constants.METRIC_START_RECD));

            // Set the reconnect-end metric to be the
            // time the initial launch request is made (i.e. when the
            // reconnect page response is received.
            metrics.addMilliTiming(Constants.METRIC_END_RECD, launchTime);

            if (doIncludeIfdcd(nfuseUID)) {
                long startIFDCD = UIUtils.getMetricFromQueryString(wiContext, Constants.METRIC_START_IFDCD);
                metrics.addMilliTiming(Constants.METRIC_START_IFDCD, startIFDCD);
            }
        } else {
            // Launch scenario
            // Set the session start metric to be the launch "click" time.
            metrics.addMilliTiming(Constants.METRIC_START_SCD, launchTime);

            // Set the ICA File Download time to be the launch start time too.
            if (doIncludeIfdcd(nfuseUID)) {
                metrics.addMilliTiming(Constants.METRIC_START_IFDCD, launchTime);
            }
        }
    }

    // Methods that differ between ASP and JSP implementations
    protected abstract String getRequestPageUrl();
    protected abstract void writeICAFileContents(ICAFile icaFile) throws IOException;
    protected abstract void writeRadFileContents(RADFile radFile, ICAFile icaFile) throws IOException;
    protected abstract String getAutoRadeUrl();

    public boolean performImp() throws IOException {
        WebAbstraction web = getWebAbstraction();
        ClientInfo clientInfo = wiContext.getClientInfo();
        UserContext userContext = SessionUtils.checkOutUserContext(wiContext);
        boolean doEUEM = (metrics != null) && Include.isOSEUEMSupport(wiContext);
        String nfuseUID = web.getQueryStringParameter(Constants.QSTR_LAUNCH_UID);

        String launchMethod = getLaunchMethod();
        // Determine the app access method to use in this launch
        AppAccessMethod appAccessMethod = getAppAccessMethod(launchMethod);

        if (!SessionToken.verifyCsrfSafe(wiContext)) {
            handleLaunchError(new FeedbackMessage(MessageType.ERROR, "SessionTokenError"), appAccessMethod);
            SessionUtils.returnUserContext(userContext);
            return true;
        }

        setClientSpecificHeaders(web, clientInfo);
        String nfuseAppName = LaunchUtilities.getCurrentLaunchApp(wiContext);

        if (!LaunchUtilities.getDirectLaunchModeInUse(wiContext)) {
            LaunchUtilities.clearSessionLaunchData(wiContext);
        }

        //check if app name is empty
        if (Strings.isEmpty(nfuseAppName)) {
            handleLaunchError(new FeedbackMessage(MessageType.ERROR, "GeneralAppLaunchError"), appAccessMethod);
            SessionUtils.returnUserContext(userContext);
            return true;
        }

        // if URL isn't unique make redirect to prevent caching
        if (!isUrlUnique(nfuseUID, doEUEM)) {
            String pageUrl = getRequestPageUrl();
            String queryString = getLocalLaunchQueryString(nfuseAppName, launchMethod);
            web.clientRedirectToUrl(pageUrl + "?" + queryString + SessionToken.copyCsrfQueryToken(wiContext));
            SessionUtils.returnUserContext(userContext);
            return false;
        }

        try {
            if( userContext == null ) {
                throw new IllegalStateException( "Cannot launch with a null userContext" );
            }

            if( appAccessMethod == AppAccessMethod.REMOTE ) {
                // Parse all captured metrics out of the querystrings and store them into metrics object
                if (doEUEM) {
                    addMetrics(nfuseUID);
                }

                // REMOTE app access (ICA and RDP)
                AppLaunchInfo launchInfo = getDesktopLaunchInfo(nfuseAppName, userContext);

                if (launchInfo == null) {
                    AppLaunchParams appLaunchParams = new AppLaunchParams(ClientType.ICA_30);
                    LaunchOverride launchOverride = LaunchShared.getLaunchOverrideFromOverrideData(
                                                                            wiContext,
                                                                            userContext.getAccessPrefs().getLocale(),
                                                                            nfuseAppName,
                                                                            appLaunchParams.getLaunchClientType());
                    appLaunchParams.setLaunchOverride(launchOverride);
                    launchInfo = (AppLaunchInfo)userContext.launchApp(nfuseAppName, appLaunchParams);
                }

                ICAFile icaFile = generateIcaFile( userContext, launchInfo );

                if (doEUEM) {
                    metrics.addMilliTiming(Constants.METRIC_NRD, launchInfo.getNameResolutionDuration());
                    metrics.addMilliTiming(Constants.METRIC_TRD, launchInfo.getTicketRequestDuration());

                    metrics.addTiming(Constants.METRIC_END_LPWD);
                    metrics.write(icaFile);

                    String reconnectTime = getReconnectTime();
                    // If we have reconnect metrics and they have been written
                    // out for all ICA sessions, then null the object in the session.
                    if(reconnectMetrics != null &&
                        Strings.isLong(reconnectTime, 10) &&
                        reconnectMetrics.decrementSessionCount() == 0) {

                        web.setSessionAttribute(reconnectTime, null);
                    }
                }

                writeICAFileContents(icaFile);
            } else {

                // STREAMING app access (RADE)

                RADFile radFile = null; // RADE launch info
                ICAFile icaFile = null; // launch info for ICA fallback
                java.lang.Exception radeException = null;
                java.lang.Exception icaException = null;

                // Get the RADE launch info
                try {
                    StreamedAppLaunchInfo radeLaunchInfo = (StreamedAppLaunchInfo)userContext.launchApp(
                                                                                    nfuseAppName,
                                                                                    new AppLaunchParams(ClientType.RADE));
                    radFile = userContext.convertToRADFile(radeLaunchInfo);
                } catch (java.lang.Exception ex) {
                    // this resource could not be launched with RADE
                    radeException = ex;
                }

                // Get the ICA launch info
                try {
                    AppLaunchParams appLaunchParams = new AppLaunchParams(ClientType.ICA_30);
                    if (radFile != null) {
                        // this ICA launch data is for fallback, so set the primary client type
                        appLaunchParams.setPrimaryClientType(ClientType.RADE);
                    } else {
                        LaunchOverride launchOverride = LaunchShared.getLaunchOverrideFromOverrideData(wiContext, userContext.getAccessPrefs().getLocale(),
                                                                                       nfuseAppName, appLaunchParams.getLaunchClientType());
                        appLaunchParams.setLaunchOverride(launchOverride);
                    }
                    AppLaunchInfo icaLaunchInfo = (AppLaunchInfo)userContext.launchApp(nfuseAppName, appLaunchParams);
                    icaFile = generateIcaFile(userContext, icaLaunchInfo);
                } catch (java.lang.Exception ex) {
                    // this resource could not be launched with ICA
                    icaException = ex;
                }

                if ((radFile == null) && (icaFile == null)) {
                    // the resource could not be launched with either RADE or ICA; throw the exception
                    // from the RADE launch attempt (since this is primarily a RADE launch)
                    throw radeException;
                }

                if (radFile == null) {
                    // create an empty RAD file - the final RAD file will only contain embedded ICA information
                    radFile = new RADFile();
                } else {
                    // fill in the RADE session URL
                    String sessionURL = wiContext.getConfiguration().getAppAccessMethodConfiguration().getRADESessionUrl();
                    if ("auto".equalsIgnoreCase(sessionURL)) {
                        sessionURL = getAutoRadeUrl();
                    }

                    radFile.setValue(RADFile.VALNAME_SESSION_URL, sessionURL);
                }

                // Delete the RAD file after use
                radFile.setValue(RADFile.VALNAME_REMOVE_RAD_FILE, RADFile.VALUE_YES);

                writeRadFileContents(radFile, icaFile);
            }

        } catch (java.lang.Exception e) {
            if (Trace.enabled(Trace.CATEGORY_LAUNCH)) {
                Trace.trace(this, "performImp: caught exception while launching " + nfuseAppName + ": \n" + Throwables.getStackTrace(e));
            }

            ResourceInfo resInfo = ResourceEnumerationUtils.getResource(nfuseAppName, userContext);
            FeedbackMessage msg = LaunchUtilities.getLaunchErrorMessage(wiContext, e, resInfo);

            handleLaunchError(msg, appAccessMethod);
            SessionUtils.returnUserContext(userContext);
            return true;
        }

        DesktopLaunchHistory.getInstance(wiContext).addDesktop(nfuseAppName);
        SessionUtils.returnUserContext(userContext);
        return false;
    }

    /**
     * This method tries to get the launchInfo for the given appId from the session
     * or from the delayedLaunchControl.readyToLaunch list. Otherwise this returns null
     * NOTE: launch info may be deleted from session on retrieval
     */
    private AppLaunchInfo getDesktopLaunchInfo(String appId, UserContext userContext) {
        // LaunchInfo is stored in session if the desktop was ready to launch
        // otherwise it is stored in DelayedLaunchControl.
        AppLaunchInfo launchInfo = LaunchUtilities.getCachedLaunchInfo(wiContext, appId);
        if (launchInfo != null) {
            // remove it from the session
            LaunchUtilities.clearCachedLaunchInfo( wiContext, appId );
        } else {
            // if not stored in the session see if it is stored in the delayed launch control
            DelayedLaunchControl delayedLaunchControl = DelayedLaunchUtilities.getDelayedLaunchControl(wiContext);
            if (delayedLaunchControl.isResourceReadyToLaunch(appId)) {
                launchInfo = delayedLaunchControl.getResourceLaunchInfo(appId);
                ResourceInfo resInfo = ResourceEnumerationUtils.getResource(appId, userContext);
                if (LaunchUtilities.canScriptLaunch(wiContext, resInfo)) {
                    delayedLaunchControl.removeReadyToLaunchResource(appId);
                } else {
                    delayedLaunchControl.changeReadyStatusToBlocked(appId);
                }

            }
        }
        return launchInfo;
    }

    /**
     * This function checks if ICA file URL is unique. This is required in order to prevent problems with the
     * browser cache.
     * URL is considered unique if it contains EUEM timestamps or unique launch id.
     *
     * @param launchUID Launch unique id.
     * @param doEUEM Tells if launch uses EUEM metrics.
     * @return True if URL is unique, false if not.
     */
    private boolean isUrlUnique(String launchUID, boolean doEUEM) {
        boolean isUnique = (!Strings.isEmpty(launchUID) || doEUEM);
        return isUnique;
    }

    /**
     * Tells whether IFDCD value should be added to metrics.
     *
     * @param launchUID Unique launch identifier.
     * @return True if IFDCD metric should be added, false otherwise
     */
    private boolean doIncludeIfdcd(String launchUID) {
        // IFDCD should not be reported for embedded launches because the
        // client should be doing it and we may override their value.

        // Launch UID is set if:
        //    - it is an embedded (or ICO) launch OR
        //    - EUEM is not in use
        // In either case, we should *not* send IFDCD data.
        boolean addIFDCD = Strings.isEmpty(launchUID);
        return addIFDCD;
    }

    /**
     * Handles a launch error from a given error key.
     *
     * For embedded (including ICO/RCO) launches, sets appropriate headers to
     * indicate an error condition to the client. For non-embedded launches,
     * prepares the view control so that the response can redirect the
     * browser to an appropriate error page using JavaScript.
     *
     * @param feedbackMessage the feedback message that should be displayed
     * @param appAccessMethod the application access method used for the launch
     */
    public void handleLaunchError(FeedbackMessage feedbackMessage, AppAccessMethod appAccessMethod) {
        WebAbstraction web = getWebAbstraction();

        if ((appAccessMethod == AppAccessMethod.STREAMING) && Embed.isScriptedStreamingAppLaunch(wiContext)) {
            // This is an RCO launch
            // Set an error status code to indicate that a RAD file is not being returned.
            web.setResponseStatus(WebAbstraction.SC_INTERNAL_SERVER_ERROR, HTTP_STATUS_RADE_ERROR);

            // Use an HTTP header to specify an error message
            web.appendResponseHeader(HTTP_HEADER_RADE_ERROR, feedbackMessage.getMessageString(wiContext));

            // Error key can be retrieved on default page to display feedback to user with more
            // informative message then general message from the streaming client.
            web.setSessionAttribute(Constants.SV_LAUNCH_FILE_FEEDBACK_MSG, feedbackMessage);
        } else if ((appAccessMethod == AppAccessMethod.REMOTE) && Embed.isScriptedHostedAppLaunch(wiContext)) {
            // This is an embedded launch (including ICO launches)
            // We indicate a failure to the ICA client by returning an HTTP 500
            // status with an appropriate error message.
            // Set an error status code to indicate to the Java Client that an ICA file is not being returned.
            web.setResponseStatus(WebAbstraction.SC_INTERNAL_SERVER_ERROR, HTTP_STATUS_ICA_ERROR);

            // Use an HTTP header to specify an error message for the Java Client to display.
            web.appendResponseHeader(HTTP_HEADER_ICA_ERROR, feedbackMessage.getMessageString(wiContext));
            // Error key can be retrieved on default page to display feedback to user with more
            // informative message then general message from ICO.
            web.setSessionAttribute(Constants.SV_LAUNCH_FILE_FEEDBACK_MSG, feedbackMessage);
        } else {
            // This is a non-embedded launch
            // Since we are returning content to the browser, we are free to
            // return HTML that will cause a message to be displayed.
            viewControl.redirectUrl = FeedbackUtils.getFeedbackUrl(wiContext, feedbackMessage, Include.getHomePage(wiContext));
        }

        if (!LaunchUtilities.getDirectLaunchModeInUse(wiContext)) {
            LaunchUtilities.clearSessionLaunchData(wiContext);
        }
    }

    protected static final String HTTP_HEADER_ICA_ERROR  = "ICA-File-Error";
    protected static final String HTTP_HEADER_RADE_ERROR = "RAD-File-Error";
    protected static final String HTTP_STATUS_ICA_ERROR  = "ICA File Unavailable";
    protected static final String HTTP_STATUS_RADE_ERROR = "RAD File Unavailable";


    static public LaunchOverride getLaunchOverrideFromOverrideData(WIContext wiContext,
                                                                   Locale locale,
                                                                   String appId,
                                                                   ClientType clientType)
                                                                  throws IOException {
        if (appId == null) {
            throw new IllegalArgumentException("appId cannot be null");
        }

        LaunchOverride launchOverride = null;

        if (ClientType.ICA_30.equals(clientType)) {
            // apply override ICA file for ICA connections
            BandwidthProfilePreference bandwidthProfilePreference = wiContext.getUserPreferences().getBandwidthProfilePreference();
            BandwidthProfile bandwidthProfile = bandwidthProfilePreference == null ? null : bandwidthProfilePreference.toBandwidthProfile();
            ICAFile icaFile = getOverrideICA(wiContext, bandwidthProfile, locale);

            launchOverride = icaFile.convertToLaunchOverride(appId);
        } else {
            launchOverride = new LaunchOverride();
        }

        Include.modifyLaunchSettings(launchOverride, clientType);

        return launchOverride;
    }

    static private ICAFile getOverrideICA(WIContext wiContext, BandwidthProfile bp, Locale locale) throws IOException{
        String overridesFile = Constants.ICA_FILE_DEFAULT;
        if (bp == BandwidthProfile.HIGH) {
            overridesFile = Constants.ICA_FILE_BANDWIDTH_HIGH;
        } else if (bp == BandwidthProfile.MEDIUM_HIGH) {
            overridesFile = Constants.ICA_FILE_BANDWIDTH_MEDIUM_HIGH;
        } else if (bp == BandwidthProfile.MEDIUM) {
            overridesFile = Constants.ICA_FILE_BANDWIDTH_MEDIUM;
        } else if (bp == BandwidthProfile.LOW) {
            overridesFile = Constants.ICA_FILE_BANDWIDTH_LOW;
        }

        // Construct the override ICAFile object.
        ICAFile overrides = null;
        String configLocation = (Platform.isJava() ? "WEB-INF/" : "conf/");
        Reader overridesReader = wiContext.getWebAbstraction().contextPathResourceReader(configLocation + overridesFile);
        if (overridesReader != null) {
            try {
                overrides = ICAFile.fromReader(overridesReader, true);

                if (shouldSpecifyUILocale(wiContext)) {
                    overrides.setValue(ICAConstants.SECTION_APPLICATION, ICAConstants.VALNAME_UILOCALE, Locales.toString(locale, '-'));
                }
            } finally {
                overridesReader.close();
            }
        }

        return overrides;
    }

    // Generate an ICAFile from AppLaunchInfo
    protected ICAFile generateIcaFile(UserContext userContext, AppLaunchInfo launchInfo) throws IOException {
        if (launchInfo == null) {
            throw new IllegalArgumentException("launchInfo cannot be null");
        }
        DesktopGroupsController controller = DesktopGroupsController.getInstance(wiContext);
        String fullDisplayName = controller.getCompoundDesktopDisplayName(launchInfo.getFriendlyName(),
                                                                   launchInfo.getId());
        launchInfo.setFriendlyName(fullDisplayName);

        // Prevent seamless launches for published desktops.
        if (launchInfo.getIsDesktop() && launchInfo.getDisplaySize().isSeamless()) {
            launchInfo.setProtected(false);

            AppDisplaySize displaySize = launchInfo.getPublishedDisplaySize();
            if (displaySize == null || displaySize.isSeamless()) {
                displaySize = AppDisplaySize.fromFullScreen();
            }
            launchInfo.setDisplaySize(displaySize);
        }

        ICAFile overrides = getOverrideICA(wiContext, launchInfo.getBandwidthProfile(), launchInfo.getLocale());
        ICAFile icaFile = userContext.convertToICAFile(launchInfo, null, overrides);

        String currentSslProxyHost = icaFile.getValue(ICAConstants.SECTION_APPLICATION,
            ICAConstants.VALNAME_SSL_PROXY_HOST);
        String sslProxyHostFromAG = AGEUtilities.getSSLProxyHost(wiContext);

        if ((currentSslProxyHost != null) && (sslProxyHostFromAG != null)) {
            // Replace the SSLProxyHost in the ICA file with the host address provided by
            // Access Gateway.
            icaFile.setValue(ICAConstants.SECTION_APPLICATION,
                ICAConstants.VALNAME_SSL_PROXY_HOST, sslProxyHostFromAG);
        }

        return icaFile;
    }

    static private boolean shouldSpecifyUILocale(WIContext wiContext) {
        // The locale for the remote session is only specified by WI (via the "UILocale" ICA file setting) when either:
        //   a) the user's browser includes one of the WI-supported languages in its Accept-Language HTTP header.
        // or:
        //   b) the user has explicitly set a non-English WI language preference.
        //
        // The intention is to omit UILocale for languages that are not supported by WI (e.g., Chinese) and leave it to
        // the ICA client (which may support such languages) to use its built-in language fallback mechanism, typically
        // resulting in a "native" client UI where available.
        //
        // Note, if English were not considered "neutral" then (for a Chinese user) explicitly selecting English would
        // result in English WI UI and English client UI whereas selecting no language would result in English WI UI
        // and Chinese client UI. Given there is no way to "unset" a preferred language (except by clearing cookies)
        // English is considered neutral. This means the remote session UI can no longer be forced to match the
        // English WI UI in such situations, unless the user adds English to the browser's Accept-Languages list.

        boolean neutralLocale = true;
        WebAbstraction web = wiContext.getWebAbstraction();

        if (Include.userCanSelectLanguage(wiContext.getConfiguration())) {
            Locale preferredLocale = wiContext.getUserPreferences().getLocale();
            neutralLocale = (preferredLocale == null) || (Locale.ENGLISH.getLanguage().equals(preferredLocale.getLanguage()));
        }

        // Get the locales supplied by the browser in its Accept-Language HTTP header
        Locale[] userLocales = web.getLocalesFromRequest();
        // Attempt to match these to an installed language pack
        Locale locale = Include.getLanguageManager(wiContext.getStaticEnvironmentAdaptor()).getBestMatch(userLocales);

        return ((locale != null) || !neutralLocale);
    }

    protected void setClientSpecificHeaders(WebAbstraction web, ClientInfo clientInfo) {
        if (clientInfo.osPocketPC()) {
            web.appendResponseHeader("Content-Disposition", "attachment;filename=" + clientInfo.getUniqueICAFileName());
        }
    }

    protected String getLaunchMethod() {
        WebAbstraction web = wiContext.getWebAbstraction();
        String launchMethod = web.getQueryStringParameter(Constants.QSTR_LAUNCH_METHOD);
        if (!Constants.LAUNCH_METHOD_STREAMING.equals(launchMethod)) {
            launchMethod = "";
        }
        return launchMethod;
    }

    protected String getLocalLaunchQueryString(String nfuseAppName, String launchMethod) {
        String UID = Long.toString(new Date().getTime()) + Integer.toString(random.nextInt());
        String queryStr = Constants.QSTR_LAUNCH_UID + "=" + UID + "&";
        queryStr += LaunchUtilities.getLaunchAppQueryStringFragment(wiContext, nfuseAppName);

        // Add the launch method if specified
        if(!Strings.isEmpty(launchMethod)) {
            queryStr += "&" + Constants.QSTR_LAUNCH_METHOD + "=" + launchMethod;
        }

        return queryStr;
    }

    protected AppAccessMethod getAppAccessMethod(String launchMethod) {
        return (Constants.LAUNCH_METHOD_STREAMING.equals(launchMethod)? AppAccessMethod.STREAMING : AppAccessMethod.REMOTE);

    }
}
