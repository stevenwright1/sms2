/*
 * Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */

package com.citrix.wi.pageutils;

import java.util.Map;

import com.citrix.wi.controls.DesktopGroupsController;
import com.citrix.wi.controlutils.FeedbackMessage;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pageutils.clientdetection.DetectionUtils;
import com.citrix.wi.types.MPSClientType;
import com.citrix.wing.AccessTokenException;
import com.citrix.wing.AppLaunchInfo;
import com.citrix.wing.DesktopUnavailableException;
import com.citrix.wing.MessageType;
import com.citrix.wing.NoSuchResourceException;
import com.citrix.wing.UnsupportedResourceOperationException;
import com.citrix.wing.ResourceUnavailableException;
import com.citrix.wing.SessionLimitExceededException;
import com.citrix.wing.types.ErrorResolution;
import com.citrix.wing.util.Strings;
import com.citrix.wing.util.WebUtilities;
import com.citrix.wing.webpn.DesktopInfo;
import com.citrix.wing.webpn.ResourceInfo;

public class LaunchUtilities {

    private static final String SV_DIRECT_LAUNCH = "CTX_DirectLaunch";
    private static final String SV_DIRECT_LAUNCHED_APP = "CTX_DirectLaunchedApp";
    private static final String SV_LAUNCH_APP = "CTX_LaunchApp";
    private static final String SV_LAUNCH_APP_CSRF_TOKEN = "CTX_LaunchAppCsrfToken";

    private static final String QSTR_LAUNCH_APPLICATION = "CTX_Application";

    // Query string fragment to indicate not to save launched app data
    public static final String QSTR_LAUNCH_ONLY = "CTX_LaunchOnly";

    // Session key to indicate that a desktop auto launch has already been attempted
    //  or that it should not be attempted again for this session.
    public static final String PREVENT_AUTO_DESKTOP_LAUNCH = "CTX_AutoDesktoplaunchAttempted";

    ////////////////////////////////////////////////////////////////
    /// Methods for dealing with launch data in session state
    ////////////////////////////////////////////////////////////////

    public static void clearSessionLaunchData(WIContext wiContext) {
        Map state = getSessionState(wiContext);
        state.put(SV_LAUNCH_APP, null);
        state.put(SV_LAUNCH_APP_CSRF_TOKEN, null);
    }

    // The application to launch
    public static String getSessionLaunchApp(WIContext wiContext) {
        String appId = getString(getSessionState(wiContext), SV_LAUNCH_APP);
        return appId;
    }

    // The application to launch
    public static void setSessionLaunchApp(WIContext wiContext, String appId) {
        setString(getSessionState(wiContext), SV_LAUNCH_APP, appId);
    }

    // The saved CSRF token associated with the application to launch
    public static String getSessionLaunchAppCsrfToken(WIContext wiContext) {
        String token = getString(getSessionState(wiContext), SV_LAUNCH_APP_CSRF_TOKEN);
        return token;
    }

    // Save the to-be-launched application's associated CSRF token (may be null)
    public static void setSessionLaunchAppCsrfToken(WIContext wiContext, String token) {
        setString(getSessionState(wiContext), SV_LAUNCH_APP_CSRF_TOKEN, token);
    }

    public static boolean isSessionDirectLaunch(WIContext wiContext) {
        boolean value = getBool(getSessionState(wiContext), SV_DIRECT_LAUNCH);
        return value;
    }

    public static void setSessionDirectLaunch(WIContext wiContext, boolean value) {
        setBool(getSessionState(wiContext), SV_DIRECT_LAUNCH, value);
    }

    // The application that was last launched
    public static String getSessionDirectLaunchedApp(WIContext wiContext) {
        String value = getString(getSessionState(wiContext), SV_DIRECT_LAUNCHED_APP);
        return value;
    }

    // The application that was last launched
    public static void setSessionDirectLaunchedApp(WIContext wiContext, String value) {
        setString(getSessionState(wiContext), SV_DIRECT_LAUNCHED_APP, value);
    }

    ////////////////////////////////////////////////////////////////
    // Methods for dealing with whether to auto launch a desktop. //
    ////////////////////////////////////////////////////////////////

    /**
     * Sets a session flag to indicate that an automatic desktop launch has been
     * attempted for this session or the auto launch has been disabled.
     */
    public static void setAutoDesktopLaunchDisabled(WIContext wiContext) {
        setBool(getSessionState(wiContext), PREVENT_AUTO_DESKTOP_LAUNCH, true);
    }

    /**
     * Returns if an automatic desktop launch is still enabled for this session.
     * (i.e. The flag to prevent it has not yet been set)
     */
    public static boolean getAutoDesktopLaunchEnabled(WIContext wiContext) {
        Map sessionVals = getSessionState(wiContext);
        return !sessionVals.containsKey(PREVENT_AUTO_DESKTOP_LAUNCH);
    }

    /**
     * Indicates whether an automatic launch can be attempted in a direct or
     * delayed launch scenario.
     *
     * @param wiContext current WIContext for the launch attempt
     * @param resource the subject of the launch attempt
     * @return boolean true if the launch can be attempted automatically
     * @throws IllegalArgumentException if the resource parameter is null.
     */
    public static boolean canScriptLaunch(WIContext wiContext, ResourceInfo resource) {
        // WinCE clients cause DetectionUtils.hasClientForLaunch to return false
        // because the ICO doesn't run on WinCE (no ActiveX support).  WinCE's
        // IE does support scripted launches, however, and if the client is
        // missing from WinCE it can't be downloaded and installed, so this
        // method now assumes that if the OS is WinCE there is a client
        // available for launching apps.
        if (resource != null) {
            return (!LaunchUtilities.browserBlocksLaunch(wiContext) &&
              DetectionUtils.hasClientForLaunch(wiContext, resource)) ||
              wiContext.getClientInfo().osWinCE();
        } else {
            throw new IllegalArgumentException("ResourceInfo parameter cannot be null");
        }
    }

    ////////////////////////////////////////////////////////////////
    /// Methods for dealing with launch data in client session state
    ////////////////////////////////////////////////////////////////

    public static void clearClientSessionLaunchData(WIContext wiContext) {
        getClientSessionState(wiContext).put(SV_LAUNCH_APP, null);
    }

    public static String getClientSessionLaunchApp(WIContext wiContext) {
        return getString(getClientSessionState(wiContext), SV_LAUNCH_APP);
    }

    public static void setClientSessionLaunchApp(WIContext wiContext, String appId) {
        setString(getClientSessionState(wiContext), SV_LAUNCH_APP, appId);
    }

    ////////////////////////////////////////////////////////////////
    /// Methods for caching LaunchInfo in client session state
    ////////////////////////////////////////////////////////////////
    public static void putCachedLaunchInfo(WIContext wiContext, String appID, AppLaunchInfo launchInfo) {
        wiContext.getWebAbstraction().setSessionAttribute(Constants.SV_APP_LAUNCH_INFO + appID, launchInfo);
    }

    public static AppLaunchInfo getCachedLaunchInfo(WIContext wiContext, String appID) {
        return (AppLaunchInfo)wiContext.getWebAbstraction().getSessionAttribute(Constants.SV_APP_LAUNCH_INFO + appID);
    }

    public static boolean hasCachedLaunchInfo(WIContext wiContext, String appID) {
        return getCachedLaunchInfo(wiContext, appID) != null;
    }

    public static void clearCachedLaunchInfo(WIContext wiContext, String appID) {
        wiContext.getWebAbstraction().setSessionAttribute(Constants.SV_APP_LAUNCH_INFO + appID, null);
    }

    ////////////////////////////////////////////////////////////////
    /// Compound methods for dealing with launch data
    ////////////////////////////////////////////////////////////////

    /**
     * Determines whether the user is in direct launch mode. This happens when
     * the user launches a bookmark and then logs in.
     *
     * @return <code>true</code> if the user is in direct launch mode.
     */
    public static boolean getDirectLaunchModeInUse(WIContext wiContext) {
        return isSessionDirectLaunch(wiContext)
                || isRequestDirectLaunch(wiContext);
    }

    /**
     * Clear the ApplicationID from the client state and, if present and permitted,
     * transfer it into the session.
     *
     * @param wiContext the WI Context
     */
    public static void transferLaunchDataToSession(WIContext wiContext) {
        String appId = getClientSessionLaunchApp(wiContext);
        clearClientSessionLaunchData(wiContext);

        if ((appId != null) && wiContext.getConfiguration().getEnablePassthroughURLs()) {
            setSessionLaunchApp(wiContext, appId);
            setSessionDirectLaunch(wiContext, true);
        }
    }

    /**
     * Gets a feedback message associated with the given
     * ResourceUnavailableException and resource.
     *
     * @param wiContext the Web Interface context
     * @param e an exception to map to a feedback message
     * @param r the ResourceInfo object of the resource that was being launched
     * @return an appropriate FeedbackMessage object
     */
    public static FeedbackMessage getLaunchErrorMessage(WIContext wiContext,
        java.lang.Exception e, ResourceInfo r) {

        if (r == null) {
            return new FeedbackMessage(MessageType.ERROR, "GeneralAppLaunchError");
        }
        DesktopGroupsController controller = DesktopGroupsController.getInstance(wiContext);
        String displayName = controller.getCompoundDesktopDisplayName(r.getDisplayName(), r.getId());
        FeedbackMessage message = new FeedbackMessage(MessageType.ERROR, getLaunchErrorKey(e), new String[] { displayName });

        // Whether the exception might be resolved by a desktop restart
        boolean resolvableByRestart = (e instanceof ResourceUnavailableException) &&
                                      ((ResourceUnavailableException)e).hasSuggestedResolution(ErrorResolution.RESTART_DESKTOP);
        boolean resourceRestartable = (r instanceof DesktopInfo) && ((DesktopInfo)r).canPowerOff();

        if (resolvableByRestart && resourceRestartable) {
            // This is a launch error that may be solved by a restart.
            // We show a special message in this situation, with a link to restart
            // the desktop.
            // The message is different depending on whether or not we are in
            // direct launch mode.
            if (getDirectLaunchModeInUse(wiContext)) {
                String restartUrl = UIUtils.getDirectLaunchConfirmRestartLinkFragment(wiContext, r.getId(), SessionToken.get(wiContext));
                message = new FeedbackMessage(
                    MessageType.ERROR,
                    "DirectLaunchResolvableError",
                    new String[] { restartUrl });
            } else {
                String restartUrl = UIUtils.getApplistConfirmRestartLinkFragment(wiContext,
                    r.getId(), displayName, SessionToken.get(wiContext));
                message = new FeedbackMessage(
                    MessageType.ERROR,
                    "NormalLaunchResolvableError",
                    new String[] { restartUrl, displayName });
            }
        }

        return message;
    }


    /**
     * Gets the resource string key associated with a given exception.
     *
     * @param e the exception to map to a resource string key
     * @return a resource string key
     */
    public static String getLaunchErrorKey(java.lang.Exception e) {
        String errorKey = null;

        if (e instanceof AccessTokenException) {
            errorKey = Utils.getAuthErrorMessageKey((AccessTokenException)e);
        } else if (e instanceof SessionLimitExceededException) {
            // This occurs when launching desktop resources and means that
            // the user already has a desktop session running and is not allowed another.
            errorKey = "NoMoreActiveSessions";
        } else if (e instanceof UnsupportedResourceOperationException) {
            errorKey = "UnsupportedClientType";
        } else if (e instanceof NoSuchResourceException) {
            errorKey = "AppRemoved";
        } else if (e instanceof DesktopUnavailableException) {
            // This can occur when launching desktop resources.
            // It is a subclass of ResourceUnavailableException and so we check
            // for it first.
            errorKey = Utils.getDesktopErrorMessageKey((DesktopUnavailableException)e);
        } else if (e instanceof ResourceUnavailableException) {
            errorKey = Constants.CONST_RESOURCE_ERROR;
        } else {
            errorKey = "GeneralAppLaunchError";
        }

        return errorKey;
    }

    /**
     * Return the appId associated with the current context, either in
     * the request query string or in the session state
     *
     * @param wiContext
     * @return appId specified in the current request query string (if specified)
     * otherwise the value specified in the session state
     */
    public static String getCurrentLaunchApp(WIContext wiContext) {
        String appId = getQueryStringLaunchApp(wiContext);
        if (Strings.isEmpty(appId)) {
            appId = LaunchUtilities.getSessionLaunchApp(wiContext);
        }
        return appId;
    }

    ////////////////////////////////////////////////////////////////
    // Methods for accessing the request context for temporary
    // launch data
    ////////////////////////////////////////////////////////////////

    public static void setRequestDirectLaunch(WIContext wiContext, boolean value) {
        WebAbstraction web = wiContext.getWebAbstraction();
        web.setRequestContextAttribute(SV_DIRECT_LAUNCH, toBoolString(value));
    }

    public static boolean isRequestDirectLaunch(WIContext wiContext) {
        WebAbstraction web = wiContext.getWebAbstraction();
        return fromBoolString(web.getRequestContextAttribute(SV_DIRECT_LAUNCH));
    }

    ////////////////////////////////////////////////////////////////
    // Methods for accessing the launch data from query string
    // parameters
    ////////////////////////////////////////////////////////////////

    public static String getQueryStringLaunchApp(WIContext wiContext) {
        WebAbstraction web = wiContext.getWebAbstraction();
        String launchApp = web.getQueryStringParameter(QSTR_LAUNCH_APPLICATION);

        return launchApp;
    }

    public static String getLaunchAppQueryStringFragment(WIContext wiContext, String appId) {
        StringBuffer sb = new StringBuffer();
        sb.append(QSTR_LAUNCH_APPLICATION);
        sb.append("=");
        sb.append(WebUtilities.escapeURL(appId));
        return sb.toString();
    }

    public static String getAppIdFromInitialQueryString(WIContext wiContext, String queryString) {
        String appId = WebUtilities.getRawParameter(queryString, QSTR_LAUNCH_APPLICATION);
        return WebUtilities.unescapeURL(appId);
    }

    public static String getDirectAutoLaunchJavaScript(WIContext wiContext, ResourceInfo resInfo, String launchURL) {
        if (canScriptLaunch(wiContext, resInfo)) {
            return getAutoLaunchJavaScript(wiContext, resInfo.getId(), launchURL);
        } else {
            String encodedAppId = WebUtilities.encodeForId(resInfo.getId());
            boolean isRestarting = DelayedLaunchUtilities.getDelayedLaunchControl(wiContext).isResourcePending(resInfo.getId());

            String launchScript = "  var elt = document.getElementById('" + encodedAppId + "'); \n";
            launchScript += "  if (elt) { \n";
            if (isRestarting) {
                launchScript += "     setSpinnerVisible('" + encodedAppId + "', true); \n";
            } else {
                launchScript += "     showDesktopLaunchingUI(elt); \n";
                // Add some extra tile to be sure the play icon is set after the spinner was removed.
                launchScript += "     var launchTimeout = " + wiContext.getConfiguration().getMultiLaunchTimeout() + " * 1000 + 200;";
                launchScript += "     setTimeout(function() { setLaunchReadyIcon(document, elt.id); }, \n";
                launchScript += "                   launchTimeout); \n";
            }

            launchScript += "  } \n";

            return launchScript;
        }
    }

    public static String getAutoLaunchJavaScript(WIContext wiContext, String appId, String launchURL) {
        if (wiContext.getClientInfo().osPocketPC()) {
            // Delay for 5 seconds to allow images to download
            return "setTimeout(\"window.location.href = '" + launchURL + "'\", 5000);\n";
        }

        String launchScript = "if (document.getElementById('" + Constants.ID_DIV_LAUNCH + "') != null) { \n";

        if (Include.isOSEUEMSupport(wiContext)) {
            launchScript += "  var url = addCurrentTimeToURL('" + launchURL + "', '" + Constants.QSTR_METRIC_LAUNCH_ID + "'); \n";
        } else {
            launchScript += "  var url = '" + launchURL + "'; \n";
        }
        launchScript += "  var elt = document.getElementById('" + WebUtilities.encodeForId(appId) + "'); \n";
        launchScript += "  if (elt) { \n";
        launchScript += "     launch(elt); \n";
        launchScript += "  } else { \n";
        launchScript += "     autolaunch(url); \n";
        launchScript += "  } \n";
        launchScript += "} \n";

        return launchScript;
    }


    public static String getJavaScriptForReconnectAtLogin(WIContext wiContext, String reconnectURL) {
        String frameName = Constants.ID_FRAME_RECONNECT + Include.getFrameSuffix(wiContext.getUserEnvironmentAdaptor());
        String launchScript;
        if (wiContext.getClientInfo().osPocketPC()) {
            launchScript = "window.location.href = '" + reconnectURL + "';";
        } else {
            launchScript = "var reconnectFrame = window.frames['" + frameName + "']; \n"
                + "if(reconnectFrame != null && reconnectFrame.location.href) {\n"
                + "  reconnectFrame.location.href = addCurrentTimeToURL('" +
                reconnectURL + "', '" + Constants.QSTR_METRIC_RECONNECT_ID + "'); \n";
            launchScript += "}";
        }

        return launchScript;
    }

    ////////////////////////////////////////////////////////////////
    // Misc. helpers for launch related activities
    ////////////////////////////////////////////////////////////////

    /**
     * Internet Explorer's yellow security bar blocks downloads and
     * consequently may block launches when both:
     *   1) in IE (except WinCE); and
     *   2) using ICA client (not ICO)
     * because ica file is downloaded to the browser using javascript
     *
     * @param wiContext current WIContext for the launch attempt
     * @return true if the browser will prevent an ICA launch under the current configuration
     */
    public static boolean browserBlocksLaunch(WIContext wiContext) {
        boolean result = !wiContext.getClientInfo().osWinCE() &&
                         !wiContext.getClientInfo().osPocketPC() &&
                          wiContext.getClientInfo().isIE() &&
                          Include.getSelectedRemoteClient(wiContext) == MPSClientType.LOCAL_ICA &&
                         !Include.doIcaLaunchViaScripting(wiContext);
        return result;
    }


    /**
     * Checks to see if the given resource could be powered off.
     *
     * @param resource the resource to check for
     * @return if it can successfully power off
     */
    public static boolean canPowerOffResource(ResourceInfo resource) {
        if (resource instanceof DesktopInfo) {
            DesktopInfo desktopInfo = (DesktopInfo)resource;
            return desktopInfo.canPowerOff();
        }
        return false;
    }


    //////////////////////////////
    // Utilities
    //////////////////////////////

    private static Map getSessionState(WIContext wiContext) {
        return wiContext.getUserEnvironmentAdaptor().getSessionState();
    }

    private static Map getClientSessionState(WIContext wiContext) {
        return wiContext.getUserEnvironmentAdaptor().getClientSessionState();
    }

    //////////////////////////////////////////////////////
    // Utilities for dealing with maps and boolean types
    // represented as strings
    //////////////////////////////////////////////////////

    private static void setBool(Map state, String key, boolean value) {
        state.put(key, toBoolString(value));
    }

    private static boolean getBool(Map state, String key) {
        return fromBoolString(state.get(key));
    }

    private static void setString(Map state, String key, String value) {
        state.put(key, value);
    }

    private static String getString(Map state, String key) {
        return (String)state.get(key);
    }

    private static String toBoolString(boolean value) {
        return value ? Constants.VAL_TRUE : Constants.VAL_FALSE;
    }

    private static boolean fromBoolString(Object value) {
        return Constants.VAL_TRUE.equals(value);
    }

}
