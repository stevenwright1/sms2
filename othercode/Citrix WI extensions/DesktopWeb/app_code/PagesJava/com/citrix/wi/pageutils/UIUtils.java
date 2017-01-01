/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils;

import java.io.IOException;
import java.io.StringWriter;
import java.util.Map;

import com.citrix.wi.controls.ResourceControl;
import com.citrix.wi.metrics.PerformanceMetrics;
import com.citrix.wi.mvc.StatusMessage;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.mvc.WebCookie;
import com.citrix.wi.types.AGEAccessMode;
import com.citrix.wi.types.LayoutType;
import com.citrix.wi.types.WIAuthType;
import com.citrix.wi.util.ClientInfoUtilities;
import com.citrix.wing.MessageType;
import com.citrix.wing.UserEnvironmentAdaptor;
import com.citrix.wing.util.Locales;
import com.citrix.wing.util.Strings;
import com.citrix.wing.util.WebUtilities;

/**
 * Provides a number of useful methods that are handy when working with
 * the UI or the browser.  E.g., obtaining and creating URLs, obtaining parts
 * of URLs, obtaining screen sizes, etc.
 */
public class UIUtils {

    public static final String TARGET_MAIN_FRAME = "WIMainFrame";
    public static final String TARGET_TOP        = "top";

    /**
     * Adds a named metric to a query string. Operation fails if the metric name
     * or value is invalid.
     *
     * @param qs the query string, everything following the "?"
     * @param metricName the parameter name that the metric will put under in
     * the resulting querystring.
     * @param metricValue the value of the metric, must be parsable as a long
     * for successful addition to the querystring
     * @return the update query string
     */
    public static String addMetricToQueryString(String qs, String metricName, String metricValue) {
        if (qs != null) {
            String separator = (qs.length() == 0) ? "" : "&";
            if (metricName != null && metricName.length() != 0 && Strings.isLong(metricValue, 10)) {
                qs = qs + separator + metricName + "=" + metricValue;
            }
        }
        return qs;
    }

    /**
     * Retrieve and validate the client window size from a cookie.
     *
     * @param request the current request
     * @return the screen resolution
     */
    public static int[] getCookieScreenRes(WIContext wiContext) {
        String width = "-1";
        String height = "-1";
        String scrRes = (String)wiContext.getUserEnvironmentAdaptor().getClientSessionState().get(
                        Constants.COOKIE_ICA_SCREEN_RESOLUTION);
        if (scrRes != null) {
            int pos = scrRes.indexOf("x");
            if (pos != -1) {
                width = scrRes.substring(0, pos);
                height = scrRes.substring(pos + 1);
            }
        }
        return validateClientWindowSize(width, height);
    }

    public static String getJavascript(String jscript) {
        return "<script type=\"text/javascript\">\n" + "<!--\n" + jscript + ";\n" + "// -->\n" + "</script>";
    }

    /**
     * Generates a URL for the Logged Out page with an appropriate query string.
     *
     * If a log message key is provided, the message is logged and the resulting
     * log event ID is included as part of the display message.
     *
     * @param wiContext the Web Interface context object
     * @param messageType the type of message to be displayed on the page
     * @param messageKey the key of the message to display
     * @param logMsgKey the key of the message to log
     */
    public static String getLoggedOutRedirectURL(WIContext wiContext, MessageType messageType,
        String messageKey, String logMsgKey)
    {
        return getLoggedOutRedirectURL(wiContext, messageType, messageKey, null, logMsgKey);
    }

    /**
     * Generates a URL for the Logged Out page with an appropriate query string.
     *
     * If a log message key is provided, the message is logged and the resulting
     * log event ID is included as part of the display message.
     *
     * @param wiContext the Web Interface context object
     * @param messageType the type of message to be displayed on the page
     * @param messageKey the key of the message to display
     * @param messageArg the argument for the display message, or null if not required
     * @param logMsgKey the key of the message to log
     */
    public static String getLoggedOutRedirectURL(WIContext wiContext, MessageType messageType,
        String messageKey, String messageArg, String logMsgKey) {

        String bLoggedOutCookie = (String)wiContext.getUserEnvironmentAdaptor().getClientSessionState().get(
                        Constants.COOKIE_SMC_LOGGED_OUT);
        boolean bSMCLoggedOut = Constants.VAL_TRUE.equalsIgnoreCase(bLoggedOutCookie);

        if (AGEUtilities.isAGEEmbeddedMode(wiContext)) {
            // Redirect straight to the login page when in embedded mode
            return "../auth/" + Constants.PAGE_LOGIN;
        }

        String prefix = "";
        if (bSMCLoggedOut) {
            prefix = Constants.QSTR_SMC_LOGGED_OUT + "=" + Constants.VAL_ON;
        }

        // Log the message
        String logEventID = (logMsgKey != null ? wiContext.log(messageType, logMsgKey) : null);

        String queryString = getMessageQueryStr(prefix, messageType, messageKey, messageArg, logEventID);
        return "../auth/" + Constants.PAGE_LOGGEDOUT + queryString;
    }

    /**
     * Generates a URL for displaying the given message on the given page.
     *
     * @param wiContext the Web Interface context object
     * @param redirectUrl the URL of the target page
     * @param msgType the type of message to be displayed on the page
     * @param displayMsgKey the key of the message to display
     */
    public static String getMessageRedirectUrl(WIContext wiContext, String redirectUrl,
        MessageType msgType, String displayMsgKey) {
        return getMessageRedirectUrl(wiContext, redirectUrl, msgType, displayMsgKey, null);
    }

    /**
     * Generates a URL for displaying the given message on the given page.
     *
     * If a log message key is provided, the message is logged and the resulting
     * log event ID is included as part of the display message.
     *
     * @param wiContext the Web Interface context object
     * @param redirectUrl the URL of the target page
     * @param msgType the type of message to be displayed on the page
     * @param displayMsgKey the key of the message to display
     * @param logMsgKey the key of the message to log
     */
    public static String getMessageRedirectUrl(WIContext wiContext, String redirectUrl,
        MessageType msgType, String displayMsgKey, String logMsgKey) {

        String logEventID = null;
        if (logMsgKey != null) {
            logEventID = wiContext.log(msgType, logMsgKey, null);
        }
        return redirectUrl += getMessageQueryStr(msgType, displayMsgKey, logEventID);
    }

    /**
     * Encode message as a query string.
     *
     * @param messageType constant describing the type of message
     * @param messageKey the key to the message in the resource bundle
     * @result the query string
     */
    public static String getMessageQueryStr(MessageType messageType, String messageKey) {
        return getMessageQueryStr(messageType, messageKey, null);
    }

    /**
     * Encode message as a query string.
     *
     * @param messageType constant describing the type of message
     * @param messageKey the key to the message in the resource bundle
     * @param logEventID the ID of the system log for the message
     * @result the query string
     */
    public static String getMessageQueryStr(MessageType messageType, String messageKey, String logEventID) {
        return getMessageQueryStr(messageType, messageKey, null, logEventID);
    }

    /**
     * Encode message as a query string.
     *
     * @param messageType constant describing the type of message
     * @param messageKey the key to the message in the resource bundle
     * @param messageArg the argument to be inserted into the message
     * @param logEventID the ID of the system log for the message
     * @result the query string
     */
    public static String getMessageQueryStr(MessageType messageType, String messageKey, String messageArg,
                    String logEventID) {
        return getMessageQueryStr(null, messageType, messageKey, messageArg, logEventID);
    }

    /**
     * Encode message as a query string.
     *
     * @param prefix existing query string fragment that should be prefixed to the result
     * @param messageType constant describing the type of message
     * @param messageKey the key to the message in the resource bundle
     * @param messageArg the argument to be inserted into the message
     * @param logEventID the ID of the system log for the message
     * @result the query string
     */
    public static String getMessageQueryStr(String prefix, MessageType messageType, String messageKey,
                    String messageArg, String logEventID) {
        // Initialising the string buffer to a reasonable size saves time
        StringBuffer queryStr = new StringBuffer(300);

        // If we add something, add this value first
        // this is used to prevent a trailing "&"
        String nextPrefix = "?";

        if (!Strings.isEmpty(prefix)) {
            // Strip out the question mark if the prefix includes one already
            if(!prefix.startsWith("?")){
                queryStr.append("?");
            }
            queryStr.append(prefix);
            nextPrefix = "&"; // next time we now need & not ?
        }

        if (messageType != null) {
            queryStr.append(nextPrefix);
            queryStr.append(Constants.QSTR_MSG_TYPE + "=" + WebUtilities.escapeURL(messageType.toString()));
            nextPrefix = "&";
        }

        if (!Strings.isEmpty(messageKey)) {
            queryStr.append(nextPrefix);
            queryStr.append(Constants.QSTR_MSG_KEY + "=" + WebUtilities.escapeURL(messageKey));
            nextPrefix = "&";
        }

        if (!Strings.isEmpty(messageArg)) {
            queryStr.append(nextPrefix);
            queryStr.append(Constants.QSTR_MSG_ARGS + "=" + WebUtilities.escapeURL(messageArg));
            nextPrefix = "&";
        }

        if (!Strings.isEmpty(logEventID)) {
            queryStr.append(nextPrefix);
            queryStr.append(Constants.QSTR_LOG_EVENT_ID + "=" + WebUtilities.escapeURL(logEventID));
        }

        return queryStr.toString();
    }

    /**
     * Gets a named performance metrics value from the query string.
     *
     * @param metricName the metric name
     * @returns the metric value, -1 if the metric is invalid
     */
    public static long getMetricFromQueryString(WIContext wiContext, String metricName) {
        String metric = wiContext.getWebAbstraction().getQueryStringParameter(metricName);
        long metricValue = PerformanceMetrics.INVALID_TIMING;

        // Underlying WING method does the null and empty check on the string.
        if (Strings.isLong(metric, 10)) {
            metricValue = Long.parseLong(metric);
        }
        return metricValue;
    }

    /**
     * Gets the value of a cookie which is not encoded. This method is used to
     * retrieve cookies that were set in client-side script.
     *
     * @param wiContext the request data object.
     * @param cookie the name of the cookie.
     * @return the value of the cookie; null indicates that the cookie does not
     * exist.
     */
    public static String getNonEncodedCookie(WebAbstraction web, String cookieName) {
        String cookieValue = null;
        if (cookieName != null) {
            WebCookie cookie = web.getCookieIgnoreCase(cookieName);
            if (cookie != null) {
                cookieValue = cookie.getValue();
            }
        }
        return cookieValue;
    }

    /**
     * Establish the screen height. This can come from different sources, it is
     * either in the ClientInfo (if the information was retreived from the
     * browser agent string) or from the values in the WIContext (if the
     * information originated from the cookies). The values from the agent
     * String take presidence if they are available
     *
     * @param wiContext Request info
     * @return <code>screenHeight</code>.
     */
    public static int getScreenHeight(WIContext wiContext) {
        int screenHeight = wiContext.getClientInfo().getScreenHeight();
        // if the ClientInfo does not contain a screen height look in the value
        // that was written into the cookies, and try that value.
        if (screenHeight == 0) {
            int[] screenSize = getCookieScreenRes(wiContext);
            screenHeight = screenSize[1];
        }
        return screenHeight;
    }

    /**
     * Establish the screen width. This can come from different sources, it is
     * either in the ClientInfo (if the information was retreived from the
     * browser agent string) or from the values in the WIContext (if the
     * information originated from the cookies). The values from the agent
     * String take presidence if they are available
     *
     * @param wiContext Request info
     * @return <code>screenWidth</code>.
     */
    public static int getScreenWidth(WIContext wiContext) {
        int screenWidth = wiContext.getClientInfo().getScreenWidth();
        // if the ClientInfo does not contain a screen width look in the value
        // that was written into the cookies, and try that value.
        if (screenWidth == 0) {
            int[] screenSize = getCookieScreenRes(wiContext);
            screenWidth = screenSize[0];
        }
        return screenWidth;
    }

    /**
     * Destroys the session and redirects the top frame to the login page. No
     * log message is written to the system log. The displayMsgKey key String is
     * passed to the target page so that a meaningful message can be displayed
     * to the user.
     *
     * @param wiContext the WIContext for the current request
     * @param msgType the type of the message Error|Warn|Info
     * @param displayMsgKey the key of the message to be displayed to the user
     * in the target page
     */
    public static void HandleLoginFailedMessage(WIContext wiContext, MessageType msgType, String displayMsgKey)
                    throws IOException {
        HandleLoginFailedMessage(wiContext, msgType, displayMsgKey, null, null, null);
    }

    /**
     * Destroys the session and redirects the top frame to the login page. The
     * log message String is written to the system log (if it is not null or
     * empty). The displayMsgKey key String is passed to the target page so that
     * a meaningful message can be displayed to the user.
     *
     * @param wiContext the WIContext for the current request
     * @param msgType the type of the message Error|Warn|Info
     * @param displayMsgKey the key of the message to be displayed to the user
     * in the target page
     * @param logMessage the log message to be written to the system log
     */
    public static void HandleLoginFailedMessage(WIContext wiContext, MessageType msgType, String displayMsgKey,
                    String msgArg, String logMessageKey, Object[] logMessageArgs) throws IOException {
        UserEnvironmentAdaptor envAdaptor = wiContext.getUserEnvironmentAdaptor();
        WebAbstraction web = wiContext.getWebAbstraction();

        // reset the logon type since this message is part of login - not a
        // session timeout
        Authentication.storeLogonType(null, envAdaptor);

        // Log the message
        String logEventID = null;
        if (logMessageArgs != null) {
            logEventID = wiContext.log(msgType, logMessageKey, logMessageArgs);
        } else {
            logEventID = wiContext.log(msgType, logMessageKey);
        }

        // Redirect
        String redirectURL = Constants.PAGE_LOGIN + getMessageQueryStr(msgType, displayMsgKey, msgArg, logEventID);

        // Store the state if needed
        if (!(envAdaptor.isCommitted())) {
            envAdaptor.commitState();
            envAdaptor.destroy();
        }

        // Destroy built-up session state
        web.abandonSession();

        web.clientRedirectToUrl(redirectURL);
    }

    /**
     * Destroys the session and redirects the top frame to the login page. The
     * log message String is written to the system log (if it is not null or
     * empty). The displayMsgKey key String is passed to the target page so that
     * a meaningful message can be displayed to the user.
     *
     * @param wiContext the WIContext for the current request
     * @param status the MessageStatus object describing the failure details
     * @throws IOException
     */
    public static void HandleLoginFailedMessage(WIContext wiContext, StatusMessage status) throws IOException {
        HandleLoginFailedMessage(wiContext,
                                 status.getType(),
                                 status.getDisplayMessageKey(),
                                 status.getDisplayMessageArg(),
                                 status.getLogMessageKey(),
                                 status.getLogMessageArgs());
    }

    /**
     * Destroys the session and redirects to the Logged Out page.
     *
     * This method is intended to be used from the WI main frame, i.e. UI
     * pages redirecting to other UI pages. Pages inside frames must deal with
     * logout via another mechanism.
     *
     * If a display message key is provided, the message is displayed to the
     * user.
     *
     * @param wiContext the Web Interface context object
     * @param msgType the type of message to be displayed on the page
     * @param displayMsgKey the key of the message to display
     */
    public static void handleLogout(WIContext wiContext, MessageType msgType, String displayMsgKey)
    {
        handleLogout(wiContext, msgType, displayMsgKey, null, true);
    }

    /**
     * Destroys the session and redirects to the Logged Out page.
     *
     * This method is intended to be used from the WI main frame, i.e. UI
     * pages redirecting to other UI pages. Pages inside frames must deal with
     * logout via another mechanism.
     *
     * If a display message key is provided, the message is displayed to the
     * user.
     *
     * @param wiContext the Web Interface context object
     * @param msgType the type of message to be displayed on the page
     * @param displayMsgKey the key of the message to display
     * @param endSession whether to destroy the session
     */
    public static void handleLogout(WIContext wiContext, MessageType msgType, String displayMsgKey, boolean endSession)
    {
        handleLogout(wiContext, msgType, displayMsgKey, null, endSession);
    }

    /**
     * Redirects to the Logged Out page and optionally destroys the session.
     *
     * This method is intended to be used from the WI main frame, i.e. UI
     * pages redirecting to other UI pages. Pages inside frames must deal with
     * logout via another mechanism.
     *
     * If a display message key is provided, the message is displayed to the
     * user.
     *
     * @param wiContext the Web Interface context object
     * @param msgType the type of message to be displayed on the page
     * @param displayMsgKey the key of the message to display
     * @param msgArg the argument for the display message, or null if no argument required
     * @param endSession whether to destroy the session
     */
    public static void handleLogout(WIContext wiContext, MessageType msgType,
        String displayMsgKey, String msgArg, boolean endSession) {

        // Set up the state in preparation for logout
        prepareLogout(wiContext);

        // Redirect
        String redirectURL = getLoggedOutRedirectURL(wiContext, msgType, displayMsgKey, msgArg, null);
        wiContext.getWebAbstraction().clientRedirectToUrl(redirectURL);

        // Destroy the session if required
        if (endSession) {
            wiContext.getWebAbstraction().abandonSession();
        }
    }

    /**
     * Prepares for logout.
     *
     * Sets up the state and performs the appropriate actions in
     * preparation for logging the user out. Actual logout and redirection to
     * the Logged Out page must be performed elsewhere.
     *
     * @param wiContext the Web Interface context object
     */
    public static void prepareLogout(WIContext wiContext) {
        UserEnvironmentAdaptor envAdaptor = wiContext.getUserEnvironmentAdaptor();
        WebAbstraction web = wiContext.getWebAbstraction();

        // If the site is in AGE mode, the user will be asked to close their
        // browser. If this site was accessed as the AGE Home Page, the
        // corresponding AGE session will be logged out, too.
        if (AGEUtilities.isAGEIntegrationEnabled(wiContext)) {
            envAdaptor.getClientSessionState().put(Constants.COOKIE_AGE_LOGGED_OUT, Constants.VAL_ON);

            AGEAccessMode accessMode = AGEUtilities.getAGEAccessMode(wiContext);

            if (accessMode == AGEAccessMode.DIRECT) {
                AGEUtilities.doAGELogout(wiContext);
            } else if (accessMode == AGEAccessMode.EMBEDDED) {
                // Clear the logon type cookie since the logged out page will
                // not be shown
                Authentication.setUntrustedLogonType(null, envAdaptor);
            }
        } else {
            // The user has logged out; if they used smartcard authentication,
            // and were successfully authenticated, then we need to make sure
            // that they close the browser for security reasons.
            WIAuthType logonMethod = Authentication.getUntrustedLogonType(envAdaptor);
            boolean authenticated = true;

            try {
                authenticated = Authentication.getAuthenticationState(web) != null
                                && Authentication.getAuthenticationState(web).isAuthenticated();
            } catch (java.lang.IllegalStateException e) {
                authenticated = false;
            }

            if (authenticated && WIAuthType.CERTIFICATE.equals(logonMethod)) {
                envAdaptor.getClientSessionState().put(Constants.COOKIE_SMC_LOGGED_OUT, Constants.VAL_ON);
                // we got here via a logout from an authentic user, allow silent
                envAdaptor.getClientSessionState().put(Constants.COOKIE_ALLOW_AUTO_LOGIN, Constants.VAL_ON);
            } else {
                // we got here from an authentication error, disable silent in this session
                envAdaptor.getClientSessionState().put(Constants.COOKIE_ALLOW_AUTO_LOGIN, Constants.VAL_OFF);
            }
        }

		// Clear the WINGSession cookie if the kiosk mode is on so that the silent detection will
		// run again if the user clicks the link to the login page.
		if (wiContext.getConfiguration().getKioskMode() == true)
		{
			Map clientSessionState = envAdaptor.getClientSessionState();
			clientSessionState.put(Constants.COOKIE_REMOTE_CLIENT_DETECTED,null);
			clientSessionState.put(Constants.COOKIE_STREAMING_CLIENT_DETECTED,null);
			clientSessionState.put(Constants.COOKIE_ALTERNATE_RESULT,null);
			clientSessionState.put(Constants.COOKIE_ICO_STATUS,null);
			clientSessionState.put(Constants.COOKIE_RDP_CLASS_ID,null);
		}		
    }

    /**
     * Redirects the current page to the specified target URL and causes a
     * message to be displayed.
     *
     * This method is intended to be used from the WI main frame, i.e. UI
     * pages redirecting to other UI pages. Pages inside frames must deal with
     * messages via another mechanism.
     *
     * This method does not cause anything to be written to the log.
     *
     * @param wiContext the Web Interface context object
     * @param redirectUrl the URL of the target page
     * @param messageType the type of message to be displayed on the page
     * @param displayMsgKey the key of the message to display
     */
    public static void handleMessage(WIContext wiContext, String redirectURL, MessageType msgType,
        String displayMsgKey) {
        handleMessage(wiContext, redirectURL, msgType, displayMsgKey, null);
    }

    /**
     * Redirects the current page to the specified target URL and causes a
     * message to be displayed.
     *
     * This method is intended to be used from the WI main frame, i.e. UI
     * pages redirecting to other UI pages. Pages inside frames must deal with
     * messages via another mechanism.
     *
     * If a log message key is provided, the message is logged and the resulting
     * log event ID is included as part of the display message.
     *
     * @param wiContext the Web Interface context object
     * @param redirectUrl the URL of the target page
     * @param msgType the type of message to be displayed on the page
     * @param displayMsgKey the key of the message to display
     * @param logMsgKey the key of the message to log
     */
    public static void handleMessage(WIContext wiContext, String redirectURL, MessageType msgType,
            String displayMsgKey, String logMsgKey) {

        wiContext.getWebAbstraction().clientRedirectToUrl(
            getMessageRedirectUrl(wiContext, redirectURL, msgType, displayMsgKey, logMsgKey));
     }

    /**
     * Verify that the client window size is within its limited range.
     *
     * @param width the window width
     * @param height the window height
     * @return true if the client window size is valid
     */
    public static boolean isValidClientSize(int width, int height) {
        boolean valid = true;
        if (width <= 0 || width > Constants.MAX_ICA_WINDOW_VRES || height <= 0
                        || height > Constants.MAX_ICA_WINDOW_HRES) {
            valid = false;
        }
        return valid;
    }

    /**
     * Verify that a client window size specified by two strings is valid.
     *
     * @param width the window width
     * @param height the window height
     * @return true if the client window size is valid
     */
    public static boolean isValidClientSize(String width, String height) {
        int[] size = new int[] { -1, -1 };
        try {
            size[0] = Integer.parseInt(width);
            size[1] = Integer.parseInt(height);
        } catch (Exception e) {
            return false;
        }
        return isValidClientSize(size[0], size[1]);
    }

    /**
     * Validates the range of the client window's width and
     * returns a pair of ints {width, height}
     *
     * @param width client window width
     * @param height client window height
     * @return the validated width and height in an int array
     */
    private static int[] validateClientWindowSize(int width, int height) {
        if (!isValidClientSize(width, height)) {
            width = Integer.parseInt(Constants.DEFAULT_ICA_WINDOW_HRES);
            height = Integer.parseInt(Constants.DEFAULT_ICA_WINDOW_VRES);
        }
        return new int[] { width, height };
    }

    /**
     * Validates the range of the client window's width from a
     * String and returns a pair of ints {width, height}
     *
     * @param width client window width
     * @param height client window height
     * @return the validated width and height in an int array
     */
    public static int[] validateClientWindowSize(String width, String height) {
        int[] size = new int[] { -1, -1 };
        try {
            size[0] = Integer.parseInt(width);
            size[1] = Integer.parseInt(height);
        } catch (Exception ignore) {}
        return validateClientWindowSize(size[0], size[1]);
    }

    /**
     * Generates the id for a popup associated with the given control.
     *
     * This allows javascript to automatically associate the control
     * and its popup.
     *
     * @param controlId the id of the control
     * @return the id for the popup associated with the control
     */
    public static String getPopupId(String controlId) {
        return "Popup_" + controlId;
    }

    /**
     * This gets the URL used to request the style sheet.
     * It contains a hash of the language file and the config file.
     *
     * @throws IOException
     */
    public static String getStyleSheetURL(WIContext wiContext) throws IOException {
        // The parameters will ensure that the cache is invalidated
        // when the language or config files change, or if the
        // display mode or language is changed.
        String url = Constants.PAGE_STYLE;
        url += "?cacheString=" + Locales.toString(wiContext.getCurrentLocale());
        url += "++";
        // NB this order is important
        if (AGEUtilities.isAGEEmbeddedMode(wiContext)) {
            url += "ageembedded";
        } else if (Include.isCompactLayout(wiContext)) {
            url += LayoutType.COMPACT.toString();
        } else {
            url += LayoutType.NORMAL.toString();
        }
        url += "++" + wiContext.getResourceBundleCryptoHash();
        url += "++" + wiContext.getConfiguration().getHash();
        return url;
    }

    /**
     * This gets the URL used to request the cached block of javascript.
     * It contains a hash of the language file, the config file and others
     *
     * @throws IOException
     */
    public static String getJavascriptURL(WIContext wiContext) throws IOException {
        // The parameters will ensure that the cache is invalidated
        // when the language or config files change, or if the
        // display mode or language is changed.
        String url = Constants.PAGE_JAVASCRIPT;
        url += "?cacheString=" + Locales.toString(wiContext.getCurrentLocale());
        url += "++";
        if (Include.isCompactLayout(wiContext)) {
            url += LayoutType.COMPACT.toString();
        } else {
            url += LayoutType.NORMAL.toString();
        }
        url += "++" + wiContext.getResourceBundleCryptoHash(); // languague file
        url += "++" + wiContext.getConfiguration().getHash(); // configuration file
        if (Include.isLoggedIn(wiContext.getWebAbstraction())) {
            url += "++" + "authenticated";
        }
        return url;
    }

    /**
     * Gets the mark-up for a assistive text link.
     *
     * @param id used for the id of the a tag.
     * @param cssClass used for the class attribute of the a tag.
     * @return the mark-up
     */
    public static String generateHelpLinkMarkup( WIContext wiContext, String id, String cssClass ) {
        // By specifying "alt" text but setting a blank value for the "title" attribute,
        // we can provide alternate text for accessibility purposes while preventing it from
        // displaying as a tooltip in Internet Explorer.
        String altText = wiContext.getString("HelpIconAltText");

        StringWriter writer = new StringWriter();
        writer.write("<a id='");
        writer.write(id);
        writer.write("' class='");
        writer.write(cssClass);
        writer.write("' href='#' ><img alt='" + altText + "' title='' src='");
        writer.write(ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/Help.png"));
        writer.write("' ></a>");
        return writer.toString();
    }

    /**
     * This returns our best estimate of the width of the wrapper.
     * It will return the minimum width, unless a cookie has been set
     * to tell us the exact calculated width.
     *
     * @param web the web abstraction
     * @return the predicted width of the wrapper around the page content
     */
    public static int guessWrapperWidth( WIContext wiContext ) {
        int width = 0;

        // see if we know what the real answer is
        Map clientInfoCookie = Include.getClientInfoCookie(wiContext.getWebAbstraction());
        String widthString = (String)clientInfoCookie.get(Constants.COOKIE_WRAPPER_WIDTH);
        if (widthString != null) {
            try {
                width = Integer.parseInt(widthString);
            } catch (NumberFormatException e) {
                // ignore
            }
        }

        // if we don't know what the answer is,
        // use the screen width (not the viewport width) to have a guess
        if (width == 0) {
            width = UIUtils.getScreenWidth(wiContext) - Constants.WRAPPER_BORDER_WIDTH - Constants.CE_WINDOW_BORDER_WIDTH; // allow for borders
        }

        // ensure the value we have is in range
        if (width < Constants.MIN_WRAPPER_WIDTH) {
            width = Constants.MIN_WRAPPER_WIDTH;
        }
        if (width > Constants.MAX_WRAPPER_WIDTH) {
            width = Constants.MAX_WRAPPER_WIDTH;
        }
        return width;
    }

    /**
     * Gets an HTML link fragment (either href or href + onclick handler) that will invoke
     * a confirmation for the desktop restart feature.
     *
     * In compact mode, this will result in a redirection to the restart confirmation page.
     *
     * In normal UI mode, this will result in the display of the confirmation lightbox.
     *
     * @param wiContext the Web Interface context
     * @param resourceControl UI control for the desktop resource
     * @param sessionToken the session token for CSRF
     * @return the HTML link fragment as a string
     */
    public static String getApplistConfirmRestartLinkFragment(WIContext wiContext, ResourceControl resourceControl, String sessionToken) {
        boolean startInProgress = (resourceControl.isDelayedLaunch) ? resourceControl.startInProgress : false;
        return getApplistConfirmRestartLinkFragment(
            wiContext,
            resourceControl.id,
            startInProgress,
            resourceControl.getName(wiContext),
            sessionToken);
    }

    /**
     * Gets an HTML link fragment (either href or href + onclick handler) that will invoke
     * a confirmation for the desktop restart feature.
     *
     * In compact mode, this will result in a redirection to the restart confirmation page.
     *
     * In normal UI mode, this will result in the display of the confirmation lightbox.
     *
     * This version of the method is for use after a launch error.
     *
     * @param wiContext the Web Interface context
     * @param appId the ID of the desktop resource
     * @param displayName the display name of the desktop resource
     * @param sessionToken the session token for CSRF
     * @return the HTML link fragment as a string
     */
    public static String getApplistConfirmRestartLinkFragment(WIContext wiContext, String appId, String displayName, String sessionToken) {
        return getApplistConfirmRestartLinkFragment(wiContext, appId, false, displayName, sessionToken);
    }

    /**
     * Gets an HTML link fragment (either href or href + onclick handler) that will invoke
     * a confirmation for the desktop restart feature.
     *
     * In compact mode, this will result in a redirection to the restart confirmation page.
     *
     * In normal UI mode, this will result in the display of the confirmation lightbox.
     *
     * @param wiContext the Web Interface context
     * @param appId the ID of the desktop resource
     * @param delayedLaunchInProgress whether this resource is currently starting (delayed launch)
     * @param displayName the display name of the desktop resource
     * @param sessionToken the session token for CSRF
     * @return the HTML link fragment as a string
     */
    private static String getApplistConfirmRestartLinkFragment(WIContext wiContext, String appId,
        boolean delayedLaunchInProgress, String displayName, String sessionToken) {

        String result;

        if (Include.isCompactLayout(wiContext)) {
            // Compact layout uses a straightforward link to the restart
            // confirmation page
            result = "href=\"" + Constants.PAGE_CONFIRM_RESTART_DESKTOP +
               "?" + Constants.QSTR_APPLICATION + "=" + WebUtilities.escapeURL(appId) +
               "&" + SessionToken.QSTR_TOKENNAME + "=" + sessionToken;

            if (delayedLaunchInProgress) {
                result += "&" + Constants.QSTR_RETRY_IN_PROGRESS +"=true";
            }

            result += "\"";
        } else {
            // This message is used by the javascript to update the lightbox text
            // when the delayed launch is finished.
            String lightboxTitle = wiContext.getString("RestartLightboxTitle", displayName);
            String lightboxMessage = wiContext.getString("RestartDesktopConfirmationText");

            String restartUrl = getRestartDesktopUrl(wiContext, appId, sessionToken);

            // This parameter needs to be passed in to the JS function if this is/was
            // a delayed launch desktop
            String delayedLaunchDesktopId = delayedLaunchInProgress ? "'" + WebUtilities.encodeForId(appId) + "'" : "null";

            result = "href=\"" + WebUtilities.encodeAmpersands(restartUrl) + "\" onclick=\"showLightboxWithMessage(this," +
                       "'" + WebUtilities.escapeJavascript(lightboxTitle) + "', " +
                       "'" + WebUtilities.escapeJavascript(lightboxMessage) + "'," +
                       delayedLaunchDesktopId + "); return false;\"";
        }

        return result;
    }

    /**
     * Gets an HTML link fragment (either href or href + onclick handler) that will invoke
     * a confirmation for the desktop restart feature when in direct launch mode.
     *
     * In compact mode, this will result in a redirection to the restart
     * confirmation page.
     *
     * In normal UI mode, this will result in the display of the confirmation
     * lightbox.
     *
     * @param wiContext the Web Interface context
     * @param appId the ID of the desktop resource
     * @param sessionToken the session token for CSRF
     * @return the HTML link fragment as a string
     */
    public static String getDirectLaunchConfirmRestartLinkFragment(WIContext wiContext, String appId, String sessionToken) {
        String result;

        if (Include.isCompactLayout(wiContext)) {
            result = "href=\"javascript:confirmRestart();\"";
        } else {
            String restartUrl = getRestartDesktopUrl(wiContext, appId, sessionToken);
            result = "href=\"" + WebUtilities.encodeAmpersands(restartUrl) + "\" onclick=\"setLightboxText(); displayLightbox(this); return false;\"";
        }

        return result;
    }

    /**
     * Gets a URL that can be used to restart the desktop with the given ID
     * (assuming that the desktop is restartable).
     *
     * Invoking this URL will cause a restart immediately.
     *
     * @param wiContext the Web Interface context
     * @param appId the ID of the desktop resource
     * @param sessionToken the session token for CSRF
     * @return the restart URL as a string
     */
    public static String getRestartDesktopUrl(WIContext wiContext, String appId, String sessionToken) {
        return Constants.PAGE_RESTART_DESKTOP +
              "?" + Constants.QSTR_APPLICATION + "=" + WebUtilities.escapeURL(appId) +
              "&" + SessionToken.QSTR_TOKENNAME + "=" + sessionToken;
    }

    /**
     * Gets the duration (in hours) that browsers are allowed to cache JavaScript
     * code that is generated by Web Interface.
     * 
     * @param web the WebAbstraction
     * @return the number of hours that JavaScript may be cached
     */
    public static int getJavaScriptCacheDuration(WebAbstraction web) {
        return getCacheDuration(web, KEY_JAVASCRIPT_CACHE_DURATION);
    }

    /**
     * Gets the duration (in hours) that browsers are allowed to cache CSS
     * markup that is generated by Web Interface.
     * 
     * @param web the WebAbstraction
     * @return the number of hours that CSS may be cached
     */
    public static int getCSSCacheDuration(WebAbstraction web) {
        return getCacheDuration(web, KEY_CSS_CACHE_DURATION);
    }

    /**
     * Gets the duration (in hours) that browsers are allowed to cache
     * application icons.
     * 
     * @param web the WebAbstraction
     * @return the number of hours that icons may be cached
     */
    public static int getAppIconCacheDuration(WebAbstraction web) {
        return getCacheDuration(web, KEY_APP_ICON_CACHE_DURATION);
    }

    private static final String KEY_JAVASCRIPT_CACHE_DURATION = "JAVASCRIPT_CACHE_HOURS";
    private static final String KEY_CSS_CACHE_DURATION = "CSS_CACHE_HOURS";
    private static final String KEY_APP_ICON_CACHE_DURATION = "APPLICATION_ICON_CACHE_HOURS";

    private static int getCacheDuration(WebAbstraction web, String settingName) {
        int duration = 0;

        String durationStr = web.getConfigurationAttribute(settingName);
        try {
            duration = Integer.parseInt(durationStr);
        } catch (NumberFormatException ignore) { }

        return duration;
    }

}
