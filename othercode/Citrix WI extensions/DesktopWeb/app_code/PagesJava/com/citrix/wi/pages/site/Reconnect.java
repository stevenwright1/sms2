/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.site;

import java.io.IOException;

import com.citrix.wi.controls.ReconnectViewControl;
import com.citrix.wi.metrics.PerformanceMetrics;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pages.StandardPage;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Embed;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.SessionUtils;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wi.pageutils.Utils;
import com.citrix.wi.types.AllowChangePassword;
import com.citrix.wi.types.MPSClientType;
import com.citrix.wi.types.ReconnectOption;
import com.citrix.wing.AccessTokenException;
import com.citrix.wing.MessageType;
import com.citrix.wing.PasswordExpiredException;
import com.citrix.wing.ResourceUnavailableException;
import com.citrix.wing.util.Strings;
import com.citrix.wing.util.WebUtilities;
import com.citrix.wing.webpn.ApplicationInfo;
import com.citrix.wing.webpn.SearchStatus;
import com.citrix.wing.webpn.SessionInfo;
import com.citrix.wing.webpn.SessionInfoSet;
import com.citrix.wing.webpn.UserContext;
import com.citrix.wi.pageutils.PageHistory;
import com.citrix.wi.pageutils.SessionToken;

/**
 * The reconnect page has no UI, but it may need to set feedback.
 */
public class Reconnect extends StandardPage {
    // This "view control" is used to set up the contents of the hidden
    // frame the reconnect page inhabits.
    private ReconnectViewControl viewControl = new ReconnectViewControl();

    public Reconnect(WIContext wiContext) {
        super(wiContext);
        wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
    }

    protected boolean performImp() throws IOException {
        WebAbstraction web = wiContext.getWebAbstraction();

        if (!SessionToken.checkQueryValid(web)) {
            return true;
        }

        String currentPage = PageHistory.getCurrentPageURL(wiContext.getWebAbstraction());

        PerformanceMetrics metrics = null;

        // If the client supports EUEM collect passed metrics
        if (Include.isOSEUEMSupport(wiContext)) {
            metrics = new PerformanceMetrics();

            // Store StartRECD is the reconnect "click" time
            String reconnectId = wiContext.getWebAbstraction().getQueryStringParameter(Constants.QSTR_METRIC_RECONNECT_ID);
            if (Strings.isLong(reconnectId, 10)) {
                long reconnectTime = Long.parseLong(reconnectId);
                metrics.addMilliTiming(Constants.METRIC_START_RECD, reconnectTime);
            }
        }

        // Buffer the output until the business logic is complete.
        String bufferedJavascript = "";
        String bufferedFrameset = "";

        // Get the user context (used to find the sessions for reconnection)
        UserContext userContext = SessionUtils.checkOutUserContext(wiContext);

        // Checks whether it is reconnect at login.
        boolean bAtLogin = Constants.VAL_ON.equalsIgnoreCase(web.getQueryStringParameter(Constants.QSTR_LOGINID));

        // Marks as done if it is reconnect at login.
        if (bAtLogin) {
            web.setSessionAttribute(Constants.SV_RECONNECT_AT_LOGIN, Boolean.FALSE);
        }

        if (Include.isWorkspaceControlEnabled(wiContext)) {
            // Variable to hold the application list for reconnection.
            SessionInfoSet sessionSet = null;

            if (!Authentication.isAnonUser(wiContext.getUserEnvironmentAdaptor())) {
                // Authentication is not Anonymous.

                ReconnectOption action = null;
                if (bAtLogin) {
                    action = wiContext.getUserPreferences().getReconnectAtLoginAction();
                } else {
                    action = wiContext.getUserPreferences().getReconnectButtonAction();
                }

                if ((action == ReconnectOption.DISCONNECTED_AND_ACTIVE)
                     || (action == ReconnectOption.DISCONNECTED)) {
                    // Now we know that we should reconnect, just do the job.
                    try {
                        if (Include.isOSEUEMSupport(wiContext) && metrics != null) {
                            metrics.addTiming(Constants.METRIC_START_REWD);
                        }
                        sessionSet = userContext.findReconnectableSessions(action == ReconnectOption.DISCONNECTED_AND_ACTIVE);
                        if (Include.isOSEUEMSupport(wiContext) && metrics != null) {
                            metrics.addTiming(Constants.METRIC_END_REWD);
                        }
                    } catch (PasswordExpiredException pee) {
                        if (Authentication.isChangePasswordAllowed(wiContext)) {
                            handleReconnectMessage(Constants.PAGE_CHANGE_PASSWD, MessageType.INFORMATION, "ChangePasswordNowOrAtLogin", null);
                        } else {
                            handleReconnectMessageAndLogout(MessageType.ERROR, "CredentialsExpired", null);
                        }
                    } catch (AccessTokenException ate) {
                        // Credentials problem
                        handleReconnectMessage(currentPage, MessageType.ERROR, Utils.getAuthErrorMessageKey(ate), null);
                    } catch (ResourceUnavailableException rue) {
                        handleReconnectMessage(currentPage, MessageType.ERROR, "ReconnectError", null);
                    }
                }
            } else {
                // We do not have login credentials or the client name, puts up an
                // internal error message.
                handleReconnectMessage(currentPage, MessageType.ERROR, "ReconnectError", "ReconnectInternalError");
            }

            if ((viewControl.getRedirectUrl() == null) && (sessionSet != null)) {
                SessionInfo[] sessions = sessionSet.getSessions();

                if (sessions.length == 0 && sessionSet.getStatus() == SearchStatus.OK) {
                    handleNoSessionsToReconnect(bAtLogin, currentPage);
                } else if (isReconnectDisabled() && sessions.length != 0) {
                    handleReconnectMessage(currentPage, MessageType.WARNING, "CannotReconnectApps", null);
                } else {
                    // Set session counter for reconnect metrics
                    if (Include.isOSEUEMSupport(wiContext) && metrics != null) {
                        metrics.setSessionCount(sessions.length + 1);
                    }

                    if (sessionSet.getStatus() == SearchStatus.PARTIAL_TEMP) {
                        handleReconnectMessage(currentPage, MessageType.INFORMATION, "WorkspaceControlReconnectPartialTemp", null);
                    } else if (sessionSet.getStatus() == SearchStatus.PARTIAL_PERSISTENT) {
                        handleReconnectMessage(currentPage, MessageType.INFORMATION, "WorkspaceControlReconnectPartialPersist", null);
                    }

                    // Tries to reconnect to the applications.
                    if (Embed.isScriptedHostedAppLaunch(wiContext)
                     && !Embed.ICA_ICOCLIENT.equals(Embed.getScriptedHostedAppLaunchClient(wiContext))) {
                        // Embedded launch is selected, generates the JavaScript to
                        // reconnect to the applications in the embedded mode.
                        bufferedJavascript += UIUtils.getJavascript("var win;\n");
                        for (int i = 0; i < sessions.length; i++) {
                            bufferedJavascript += getEmbedReconnectJavascript(sessions[i], metrics) + "\n";
                        }
                    } else {
                        // Native client launch (including ICO launch) is selected,
                        // generates the frames to reconnect to the applications.
                        String separator = "";
                        String reconnectRows = "";
                        int rowheight = 100;
                        if (sessions.length > 0) {
                            rowheight = 100 / sessions.length;
                        }
                        for (int i = 0; i < sessions.length; i++) {
                            reconnectRows += separator + rowheight;
                            separator = ",";
                        }

                        if (wiContext.getClientInfo().osPocketPC()) {
                            // Add in extra frameset to display a "return to applist" link
                            bufferedFrameset = "<frameset rows=\"99%, 1%\" border=\"0\" framespacing=\"0\" frameborder=\"no\">";

                            String frameName = "ReconnectUI";
                            bufferedFrameset += "<frame src=\"" + Constants.PAGE_RECONNECT_UI + "\"\n";
                            bufferedFrameset += "name=\"" + frameName + "\"\n";
                            bufferedFrameset += "title=\"" + frameName + "\"\n";
                            bufferedFrameset += "scrolling=\"yes\"\n";
                            bufferedFrameset += "frameborder=\"no\"\n";
                            bufferedFrameset += "border=\"0\">\n";
                        }

                        bufferedFrameset += "<frameset cols=\"" + reconnectRows + "\" border=\"5\" framespacing=\"20\" frameborder=\"yes\"";
                        if(Include.isOSEUEMSupport(wiContext)) {
                            bufferedFrameset += " onload=\"";
                            for (int i = 0; i < sessions.length; i++) {
                                String frName = "Reconnect" + i;

                                // Put the EndRECD and StartIFDCD metrics into the URL.
                                bufferedFrameset += "addCurrentTimeToWindowLocation(" +
                                                        frName + ", " +
                                                        "addPageLoadTimeToURL('" +
                                                            getReconnectUrl(sessions[i], metrics) + "', '" +
                                                            Constants.QSTR_METRIC_LAUNCH_ID +
                                                        "'), '" +
                                                        Constants.METRIC_START_IFDCD +
                                                    "');";
                            }
                            bufferedFrameset += "\"";
                        }

                        bufferedFrameset += ">\n";

                        for (int i = 0; i < sessions.length; i++) {
                            String frName = "Reconnect" + i;
                            if (Include.isOSEUEMSupport(wiContext)) {
                                bufferedFrameset += "<frame src=\"" + Constants.PAGE_DUMMY + "\"\n";
                            } else {
                                bufferedFrameset += "<frame src=\"" + getReconnectUrl(sessions[i], metrics) + "\"\n";
                            }
                            bufferedFrameset += "name=\"" + frName + "\"\n";
                            bufferedFrameset += "title=\"" + frName + "\"\n";
                            bufferedFrameset += "width=\"1\"\n";
                            bufferedFrameset += "height=\"1\"\n";
                            bufferedFrameset += "scrolling=\"yes\"\n";
                            bufferedFrameset += "frameborder=\"yes\"\n";
                            bufferedFrameset += "border=\"1\">\n";
                        }

                        bufferedFrameset += "</frameset>\n"; //Reconnect launch frames

                        if (wiContext.getClientInfo().osPocketPC()) {
                            bufferedFrameset += "</frameset>\n"; //Reconnect UI
                        }
                    }

                    // Put the reconnect metrics in the session so that
                    // they can be accessed from the launch page.
                    // this is keyed to the client reconnect time.
                    if (Include.isOSEUEMSupport(wiContext) && metrics != null) {
                        long reconnectTime = metrics.getTiming(Constants.METRIC_START_RECD);
                        if (reconnectTime >= 0) {
                            web.setSessionAttribute(String.valueOf(metrics.getTiming(Constants.METRIC_START_RECD)), metrics);
                        }
                    }
                }
            }
        }

        if (bufferedFrameset.equals("")) {
            bufferedFrameset = "<body></body>";
        }

        viewControl.setBufferedFrameset(bufferedFrameset);
        viewControl.setBufferedJavascript(bufferedJavascript);

        // Return the user context before any output is generated
        SessionUtils.returnUserContext(userContext);

        // This is required for PocketPC which does not support frames and so
        // must do a redirect back to the current page with a full 302 redirect.
        if (wiContext.getClientInfo().osPocketPC() && !Strings.isEmptyOrWhiteSpace(currentPage)) {
            web.clientRedirectToUrl(currentPage);
            return false;
        }

        return true;
    }

    /**
     * Handle the situation where their are no sessions to reconnect to.
     *
     * If at login & Pocket PC then this will redirect back the the AppList page as the pocket PC
     * is unable to use iframes for the normal reconnect senario.
     */
    private void handleNoSessionsToReconnect(boolean atLogin, String currentPage) {
        if (atLogin) {
            // If at login and pocket PC then redirect back to the AppList page.
            if (wiContext.getClientInfo().osPocketPC()) {
                // Redirect back to the AppsList page as the PocketPC has a dedicated
                // reconnect page and with no sessions to reconnect to this relevant.
                viewControl.setRedirectUrl(Constants.PAGE_APPLIST);
            }
        } else {
            // Don't display this message at login time, since it's confusing
            // and unnecessary information.
            handleReconnectMessage(currentPage, MessageType.INFORMATION, "NoAppToReconnect", null);
        }
    }

    /**
     * Construct the url for reconnecting to the given session using native ICA client.
     *
     * For ICO launch we need to go to the appembed page to allow the ICO to start
     * the native client.
     * @param wiContext the request data object
     * @param mfSession The session information object for the session to reconnect.
     * @param metrics The EUEM performance metrics for the reconnect.
     * @param selectedClient The currently selected client.
     * @param selectedAppAccessMethod The currently selected app access method.
     * @return the URL to used.
     */
    public String getReconnectUrl(SessionInfo mfSession, PerformanceMetrics metrics) {

        boolean useICOClient = Embed.ICA_ICOCLIENT.equals(Embed.getScriptedHostedAppLaunchClient(wiContext));
        String reconnectURL = (useICOClient ? Constants.PAGE_APPEMBED : Constants.PAGE_LAUNCH)
                                + getReconnectQueryString(mfSession, metrics) +
                                SessionToken.makeCsrfQueryToken(wiContext);
        // symbian clients have issues with resolving relative resources as they try to
        // resolve the path from the current frame source instead of frameset
        if (wiContext.getClientInfo().osSymbian()) {
            reconnectURL = wiContext.getWebAbstraction().getApplicationPath() + "/site/" + reconnectURL;
        }
        return reconnectURL;
    }

    /**
     * Construct the JavaScript for reconnecting to the given session using embedded client.
     * @param wiContext the request data object
     * @param mfSession the session to reconnect to
     * @param metrics The EUEM performance metrics for the reconnect.
     * @param userPrefs the UserPreferences object
     * @param currentLocale the currently selected locale
     * @return the JavaScript to be used.
     */
    public String getEmbedReconnectJavascript(SessionInfo mfSession, PerformanceMetrics metrics) {

        boolean useJICA = Embed.ICA_JAVACLIENT.equals(Embed.getScriptedHostedAppLaunchClient(wiContext));
        boolean isSeamless = wiContext.getUserPreferences().getUseSeamless();

        String title = "";
        String DesiredHRES = null;
        String DesiredVRES = null;

        if (useJICA && isSeamless) {
            // JICA seamless, need to use custom size and title
            title = WebUtilities.escapeURL(wiContext.getString("ICAConnectionCenter"));
            DesiredHRES = Constants.ICA_CONN_CENTER_HRES;
            DesiredVRES = Constants.ICA_CONN_CENTER_VRES;
        } else {
            ApplicationInfo initialApp = mfSession.getInitialAppInfo();
            // Title is simply the name of the application
            title = (initialApp == null) ? "Published Application" : initialApp.getInternalAppName();
            title = WebUtilities.escapeURL(title);
            Embed.WindowDimensions winDims
                = new Embed.WindowDimensions(wiContext, initialApp,
                                             UIUtils.getScreenWidth(wiContext),
                                             UIUtils.getScreenHeight(wiContext));
            DesiredHRES = winDims.getHRES();
            DesiredVRES = winDims.getVRES();
        }
        String queryStr = getReconnectQueryString(mfSession, metrics) + "&CTX_WindowWidth=" + DesiredHRES
            + "&CTX_WindowHeight=" + DesiredVRES + "&Title=" + title + SessionToken.makeCsrfQueryToken(wiContext);

        String embedURL = "'" + Constants.PAGE_APPEMBED + queryStr + "'";

        if (metrics != null) {
            // If client supports EUEM put the EndRECD metric into the URL
            embedURL = "addPageLoadTimeToURL(" +
                           embedURL + ", '" +
                           Constants.QSTR_METRIC_LAUNCH_ID +
                       "')";
        }

        String script = "win = appEmbed(" + embedURL + ", '" + DesiredHRES + "', '" + DesiredVRES + "');\n";

        return UIUtils.getJavascript(script);
    }

    /**
     * Constructs the query string for reconnect to the given session.
     * @param mfSession The session for reconnection.
     * @param metrics The EUEM performance metrics for the reconnect.
     * @return the query string.
     */
    private String getReconnectQueryString(SessionInfo mfSession, PerformanceMetrics metrics) {
        String qs = "?CTX_Application=" + WebUtilities.escapeURL(mfSession.getId());

        if (metrics != null) {
            long rctValue = metrics.getTiming(Constants.METRIC_START_RECD);
            if (rctValue >= 0) {
                qs += "&" + Constants.QSTR_METRIC_RECONNECT_ID + "=" + rctValue;
            }
        }

        return qs;
    }
    /**
     * Check if Workspace control reconnect should be disabled.  Under some circumstances, Workspace control
     * is enabled, however the reconnect option is not available.
     * @return true if Workspace control reconnect is disabled.
     */
    private boolean isReconnectDisabled() {
        MPSClientType client = Include.getSelectedRemoteClient(wiContext);
        return (Include.isICOSupportedPlatform(wiContext)
                        && (client == MPSClientType.LOCAL_ICA)
                        && !Include.doIcaLaunchViaScripting(wiContext))
              || (client == MPSClientType.EMBEDDED_RDP)
              || ((Include.getWorkspaceControlLevel(wiContext) == Constants.RC_JAVACLIENT_ONLY)
                      && (client != MPSClientType.JAVA));
    }

    /**
     * Displays a message in the main frame.
     *
     * The view control is updated with a URL that will cause the main frame to
     * display a message.
     *
     * @param targetUrl the target page that will display the message
     * @param msgType the type of the message to display
     * @param displayMsgKey the key to the localized message to display
     * @param logMsgKey the key to the localized message to log
     */
    private void handleReconnectMessage(String targetUrl, MessageType msgType, String displayMsgKey,
        String logMsgKey) {

        String redirectUrl = UIUtils.getMessageRedirectUrl(wiContext, targetUrl,
                msgType, displayMsgKey, logMsgKey);
        viewControl.setRedirectUrl(redirectUrl);
    }

    /**
     * Displays a message in the main frame and logs the user out.
     *
     * The user is redirected to the Logged Out page, where the message is
     * displayed.
     *
     * @param msgType the type of the message to display
     * @param displayMsgKey the key to the localized message to display
     * @param logMsgKey the key to the localized message to log
     */
    private void handleReconnectMessageAndLogout(MessageType msgType, String displayMsgKey, String logMsgKey) {
        UIUtils.prepareLogout(wiContext);
        String redirectUrl = UIUtils.getLoggedOutRedirectURL(wiContext, msgType, displayMsgKey, logMsgKey);
        viewControl.setRedirectUrl(redirectUrl);
        getWebAbstraction().abandonSession();
    }
}
