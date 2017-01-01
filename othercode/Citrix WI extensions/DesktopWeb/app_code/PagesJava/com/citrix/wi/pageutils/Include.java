/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils;

import java.util.HashMap;
import java.util.Locale;
import java.util.Map;

import com.citrix.authentication.web.AuthenticationState;
import com.citrix.wi.IconCache;
import com.citrix.wi.UserPreferences;
import com.citrix.wi.UserPreferencesFilter;
import com.citrix.wi.clientdetect.ClientDetectionWizardState;
import com.citrix.wi.clientdetect.Mode;
import com.citrix.wi.clientdetect.WizardUtil;
import com.citrix.wi.clientdetect.type.IcoStatus;
import com.citrix.wi.config.AppAccessMethodConfiguration;
import com.citrix.wi.config.ApplistTabConfig;
import com.citrix.wi.config.ClientDeploymentConfiguration;
import com.citrix.wi.config.WIConfiguration;
import com.citrix.wi.controls.ResourceControl;
import com.citrix.wi.localization.ClientManager;
import com.citrix.wi.localization.LanguageManager;
import com.citrix.wi.mvc.PlatformSpecificUtils;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pageutils.clientdetection.DetectionUtils;
import com.citrix.wi.types.InstallCaptionState;
import com.citrix.wi.types.JavaFallbackMode;
import com.citrix.wi.types.LayoutMode;
import com.citrix.wi.types.LayoutType;
import com.citrix.wi.types.MPSClientType;
import com.citrix.wi.types.UserInterfaceBranding;
import com.citrix.wi.util.AppAttributeKey;
import com.citrix.wi.util.ClientInfo;
import com.citrix.wing.AppLaunchInfo;
import com.citrix.wing.MessageType;
import com.citrix.wing.StaticEnvironmentAdaptor;
import com.citrix.wing.UserEnvironmentAdaptor;
import com.citrix.wing.types.AppAccessMethod;
import com.citrix.wing.types.ClientType;
import com.citrix.wing.types.DesktopAssignmentType;
import com.citrix.wing.util.Cookies;
import com.citrix.wing.util.LocalizableString;
import com.citrix.wing.util.MPSClientNames;
import com.citrix.wing.util.Objects;
import com.citrix.wing.util.ResourceBundleFactory;
import com.citrix.wing.util.WebUtilities;
import com.citrix.wing.webpn.DesktopInfo;
import com.citrix.wing.webpn.ResourceInfo;
import com.citrix.wing.webpn.UserContext;

/**
 * Bucket class for utility methods that don't have a better home yet.
 */
public class Include {

    /**
     * Session Variable key for the ClientInfo singleton object.
     */
    public static final String SV_CLIENT_INFO = "CTX_ClientInfo";

    /**
     * Session Variable key defining a flag to display the Internal Error page
     * when a null user agent string is received.
     */
    public static final String SV_NULL_USER_AGENT = "NullUserAgent";

    /**
     * Resource key for the message logged when a null user agent string is
     * received.
     */
    private static final String KEY_NULL_USER_AGENT = "NullUserAgent";

    /**
     * A whitelist of pages that will accept a null user agent string.  (The
     * default is to reject all requests with a null User-Agent header).
     */
    private static final String[] PAGES_ACCEPTING_NULL_USER_AGENT = new String[] {
        "agelogout"
    };

    private static final String CLIENT_INFO_KEY = "com.citrix.wi.util.ClientInfo";

    /**
     * Determines whether the given wizard mode is supported by the current
     * configuration.
     *
     * @param wiContext the WI context
     * @param wizardMode the mode of the wizard
     * @return true if the mode is supported, false otherwise
     */
    public static boolean isWizardModeSupported(WIContext wiContext, Mode wizardMode) {
        boolean result = false;
        ClientDeploymentConfiguration clientDConfig = wiContext.getConfiguration().getClientDeploymentConfiguration();
        AppAccessMethodConfiguration aamConfig = wiContext.getConfiguration().getAppAccessMethodConfiguration();
        boolean streamingOnly = !aamConfig.isEnabledAppAccessMethod(AppAccessMethod.REMOTE);
        boolean streamingSupported = Include.getOsRadeCapable(wiContext)
            && Include.getBrowserRadePluginCapable(wiContext);

        // First check if the wizard is supported at all
        if (clientWizardSupported(wiContext)) {
            if (wizardMode == Mode.SILENT) {
                // always run the silent wizard
                result = true;
            } else if (wizardMode == Mode.AUTO) {
                // Auto mode is controlled by a configuration setting
                result = clientDConfig.getEnableClientWizardAuto();
                if (streamingOnly && !streamingSupported) {
                    // Don't allow wizard
                    result = false;
                }
            } else if (wizardMode == Mode.ADVANCED) {
                // Advanced mode is controlled by the customize clients setting
                result = Include.getAllowUserToCustomizeClient(wiContext)
                    && aamConfig.isEnabledAppAccessMethod(AppAccessMethod.REMOTE);
            }
        }

        return result;
    }


    /**
     * This method gets the wizard state from the session.
     * If that is not available it creates one, and if available,
     * it populates it with the result stored in the cookies
     *
     * @param wiContext
     * @return the wizard state
     */
    public static ClientDetectionWizardState getWizardState(WIContext wiContext) {
        return getWizardState(wiContext.getUserEnvironmentAdaptor());
    }

    /**
     * This method gets the wizard state from the session.
     * If that is not available it creates one.
     * If there are result available, it populates the new object with
     * the result stored in the cookies, otherwise the object will be left
     * without any detection results stored in it.
     *
     * @param uea the user environment adaptor to get the state from
     * @return the wizard state
     */
    public static ClientDetectionWizardState getWizardState(UserEnvironmentAdaptor uea) {
        Map clientSessionState = uea.getClientSessionState();

        // get from the session state
        ClientDetectionWizardState sWizardInfo =
            (ClientDetectionWizardState)uea.getSessionState().get("sWizardInfo");

        // if it is missing, build it from the cookies
        if (sWizardInfo == null) {
            sWizardInfo = new ClientDetectionWizardState();

            // populate the info from the cookie, if available
            String remoteClientDetected = (String)clientSessionState.get(Constants.COOKIE_REMOTE_CLIENT_DETECTED);
            String streamingClientDetected = (String)clientSessionState.get(Constants.COOKIE_STREAMING_CLIENT_DETECTED);
            String alternateResult = (String)clientSessionState.get(Constants.COOKIE_ALTERNATE_RESULT);
            String icoStatusResult = (String)clientSessionState.get(Constants.COOKIE_ICO_STATUS);
            String rdpClassID = (String)clientSessionState.get(Constants.COOKIE_RDP_CLASS_ID);
            if ((remoteClientDetected != null && streamingClientDetected != null && icoStatusResult != null)
                            || alternateResult != null) {
                sWizardInfo.setDetectionResult(remoteClientDetected, streamingClientDetected, alternateResult, rdpClassID, icoStatusResult);
            }

            // add into the session to be used later on
            uea.getSessionState().put("sWizardInfo", sWizardInfo);
        }
        return sWizardInfo;
    }

    /**
     * Determines the appropriate wizard input URL given current conditions.
     *
     * @param wiContext the WI context
     * @return the wizard input URL as a String, or null if the wizard does not need to be run.
     */
    public static String getWizardInputUrl(WIContext wiContext) {
        String url = null;

        if (DetectionUtils.getRadeDetectionStartCookie(wiContext)) {
            url = Constants.PAGE_WIZARD_PRE_INPUT + "?" + Constants.QSTR_CLIENT_TYPE + "=" + Constants.VAL_ACCESS_METHOD_STREAMING;
        } else if (isNextPageWizard(wiContext)) {
            url = Constants.PAGE_WIZARD_PRE_INPUT;
        }

        return url;
    }

    /**
     * Determines if next page should be client detection wizard given correct
     * conditions.
     *
     * This method assumes that auto mode is supported. To check whether auto
     * mode is supported, use isWizardModeSupported().
     *
     * @param wiContext the WI context
     * @return bool, true if next page should be client detection wizard, false
     * otherwise.
     */
    public static boolean isNextPageWizard(WIContext wiContext) {
        ClientDetectionWizardState sWizardInfo = getWizardState(wiContext);
        boolean runWizard = false;
        if (DetectionUtils.getRemoteAccessPresent(wiContext)) {
            // dual and remote only sites
            // run wizard if no remote client available
            // and there are some we can detect
            runWizard = !sWizardInfo.hasRemoteClientResult()
                            && (DetectionUtils.createRemoteClientList(wiContext, Mode.AUTO).size() > 0);
        } else {
            // streaming sites
            // run wizard if no streaming client available
            // and we can look for the streaming client
            runWizard = !sWizardInfo.hasStreamingClientResult()
                            && (DetectionUtils.createStreamingClientList(wiContext, Mode.AUTO).size() > 0);
        }

        // also run the wizard if we can upgrade either the streaming or remote
        // client
        // or the result is not valid for the current configuration
        runWizard = runWizard || DetectionUtils.promptForStreamingUpgrade(wiContext)
                        || DetectionUtils.promptForLocalIcaUpgrade(wiContext)
                        || !DetectionUtils.isValidClientDetectionResult(wiContext);

        return runWizard;
    }

    /**
     * Checks if the Wizard can be used on the detected OS
     * This is used to determine if client detection should be carried out
     * and if the wizard advanced mode should be used to select client for launch
     * @param wiContext the WI context
     * @return bool, true if os is supported by client detection wizard false otherwise
     */
    public static boolean clientWizardSupported(WIContext wiContext) {
        ClientInfo osInfo = wiContext.getClientInfo();
        // Defer to the client detection wizard to determine unsupported
        // platforms.
        boolean supportedPlatform = osSupported(wiContext) && WizardUtil.isWizardSupportedOS(osInfo);

        return supportedPlatform;
    }

    /**
     * Checks if the detected OS is supported by WI
     */
    public static boolean osSupported(WIContext wiContext) {
        return !(wiContext.getClientInfo().getPlatform() == ClientInfo.OS_UNKNOWN);
    }

    /**
     * Creates a unique random String that is alphanumeric and '_' only for
     * appending to the frame names
     */
    private static String createFrameSuffix() {
        String name = MPSClientNames.generateUnique();
        return name.replace('-', '_');
    }

    /**
     * Determine whether to launch using client site scripting
     * (i.e. using ICO)
     *
     * @param wiContext
     * @return true if you should use ICO for ICA launches
     */
    public static boolean doIcaLaunchViaScripting(WIContext wiContext) {
        ClientDetectionWizardState wizardInfo = getWizardState(wiContext);
        return isICOSupportedPlatform(wiContext) && wizardInfo.isScriptedLocalIcaLaunchPossible();
    }

    /**
     * Check if ICO is possible on this platform-browser
     * ICO is only available for IE on Win32 or Win64 (32 bit IE)
     */
    public static boolean isICOSupportedPlatform(WIContext wiContext) {
        ClientInfo clientInfo = wiContext.getClientInfo();
        boolean isIeOnWindows = clientInfo.isIE()
            && (clientInfo.osWin32() || clientInfo.osWin64());
        return isIeOnWindows;
    }

    /**
     * Determines whether the user should be able to choose their client. The
     * logic factors in the config settings and the user's browser and OS.
     *
     * Note that for a dual mode or streaming site, this always returns false, as we
     * don't parse any launch clients in those modes.
     *
     * This method call is not affected by the JavaFallbackMode setting.
     *
     * @param wiConfig
     * @param sClientInfo
     * @return <code>true</code> if the user can customize the client,
     * otherwise <code>false</code>.
     */
    public static boolean getAllowUserToCustomizeClient(WIContext wiContext) {
        boolean isRemoteOnlySite = DetectionUtils.getRemoteAccessPresent(wiContext) &&
                                   !DetectionUtils.getStreamingAccessPresent(wiContext);
        int numClients = DetectionUtils.getAvailableRemoteClients(wiContext).size();

        return isRemoteOnlySite && (numClients > 1);
    }

    /**
     * Determines whether the user should be able to choose the Java client
     * packages. The logic factors in the config settings and the user's browser
     * and OS.
     *
     * @param wiContext Request info.
     * @param userPrefs User preferences.
     * @return <code>true</code> if the user can customize the Java client
     * packages, otherwise <code>false</code>.
     */
    public static boolean getAllowUserToCustomizeJavaPackages(WIContext wiContext) {
        ClientDeploymentConfiguration cdConfig = wiContext.getConfiguration().getClientDeploymentConfiguration();

        boolean allowCustomizeJavaPackages = cdConfig.getAllowCustomizeJavaPackages();
        boolean canUseJavaClient = (cdConfig.isEnabledClient(MPSClientType.JAVA)
                                        || DetectionUtils.isJavaFallbackModeEnabled(wiContext, JavaFallbackMode.MANUAL)
                                        || DetectionUtils.isJavaFallbackModeEnabled(wiContext, JavaFallbackMode.AUTO));
        boolean isWinCE = wiContext.getClientInfo().osWinCE();
        boolean isSymbian = wiContext.getClientInfo().osSymbian();
        boolean canUserCustomizeJavaClient = getAllowUserToCustomizeClient(wiContext);
        boolean userForcedToUseJava = (getDefaultRemoteClient(wiContext) == MPSClientType.JAVA);

        return allowCustomizeJavaPackages &&
               canUseJavaClient &&
               !isWinCE &&
               !isSymbian &&
               (canUserCustomizeJavaClient || userForcedToUseJava);
    }

    /**
     * Escapes the double quotes in the given string by adding backslashes.
     * e.g. <a href="www"> would become <a href=\"www\">
     *
     * @param string String to escape.
     * @return String with double quotes escaped.
     */
    static public String escapeDoubleQuotes(String string) {
        String result = new String();
        int e, s = 0;

        while ((e = string.indexOf("\"", s)) >= 0) {
            result = result + string.substring(s, e) + '\\' + '"';
            s = e + 1;
        }
        result = result + string.substring(s);
        return result;
    }

    /**
     * Gets the default remote access client, taking into account the user's
     * platform.
     *
     * @param wiContext WIContext.
     * @param userPrefs userPreferences.
     * @return the default remote access client.
     */
    private static MPSClientType getDefaultRemoteClient(WIContext wiContext) {
        UserPreferences wiDefaults = UserPreferences.getDefaults();
        UserPreferences adminDefaults = wiDefaults.overriddenBy(new UserPreferences(wiContext.getConfiguration()));

        // Modify the admin default according to the user's platform
        return modifyClientForPlatform(wiContext, adminDefaults.getClientType());
    }


    /**
     * Determines if RADE is supported on the user's browser.
     *
     * @param wiContext WIContext.
     * @return <code>true</code> if RADE is supported, otherwise
     * <code>false</code>.
     */
    public static boolean getBrowserRadePluginCapable(ClientInfo clientInfo, UserEnvironmentAdaptor uea) {
        return (clientInfo.isIE() && (!clientInfo.isX64Browser())) || clientInfo.isNetscape() || clientInfo.isFirefox()
                        || Include.getWizardState(uea).isRADEClientAvailable();
        // if the RADE client is available, this browser must be supported!
    }

    /**
     * Determines if RADE is supported on the user's browser.
     */
    public static boolean getBrowserRadePluginCapable(WIContext wiContext) {
        ClientInfo clientInfo = wiContext.getClientInfo();
        UserEnvironmentAdaptor uae = wiContext.getUserEnvironmentAdaptor();
        return getBrowserRadePluginCapable(clientInfo, uae);
    }

    /**
     * Returns the IP address of the client.
     *
     * @return the client IP address as a string
     */
    public static String getClientAddress(WIContext wiContext) {
        String ageClientAddress = AGEUtilities.getAGEClientIPAddress(wiContext);
        return (ageClientAddress != null
                    ? ageClientAddress
                    : wiContext.getWebAbstraction().getUserHostAddress());
    }

    /**
     * This is method is used to get hold fo the Client Info
     * cookie that is set during client detection
     *
     * @param web
     * @return
     */
    public static Map getClientInfoCookie(WebAbstraction web) {
        String clientInfoCookie = UIUtils.getNonEncodedCookie(web, Constants.COOKIE_CLIENT_INFO);
        if ((clientInfoCookie != null) && (!clientInfoCookie.equals(""))) {
            return Cookies.parseCookieValue(clientInfoCookie);
        } else {
            return new HashMap();
        }
    }

    /**
     * Gets the current locale, i.e. the locale that should be used to localize
     * the site.
     * 
     * If the user is permitted to choose their locale, the locale is taken from
     * the user preferences. Otherwise the locale is taken from the HTTP request.
     * In both cases, validation and fallback ensure that only a valid locale is
     * used.
     *
     * @param userPrefs the current user preferences
     * @return the current locale
     */
    private static Locale getCurrentLocale(UserPreferences userPrefs,
                                          WIConfiguration wiConfig,
                                          UserEnvironmentAdaptor userAdaptor,
                                          StaticEnvironmentAdaptor staticAdaptor,
                                          WebAbstraction web) {
        Locale l = null;

        if (userCanSelectLanguage(wiConfig)) {
            l = userPrefs.getLocale();
        }

        if (l == null) {
            // No locale was found in the user preferences
            // Get the user's preferred locales from the HTTP request instead
            Locale[] userLocales = web.getLocalesFromRequest();

            // Attempt to match these to an installed language pack
            l = getLanguageManager(staticAdaptor).getBestMatch(userLocales);
            if (l == null) {
                l = (Locale)userAdaptor.getSessionState().get(Constants.SV_BROWSER_LOCALE);
            } else {
                userAdaptor.getSessionState().put(Constants.SV_BROWSER_LOCALE, l);
            }
        } else {
            // Found a locale in the user preferences
            // Verify that it is available
            l = getLanguageManager(staticAdaptor).getBestMatch(l);
        }

        if (l == null) {
            // The desired locale was not available in any form, so revert to
            // the default locale
            l = wiConfig.getDefaultLocale();
        }

        // Store and return the locale
        if (userCanSelectLanguage(wiConfig)) {
            userPrefs.setLocale(l);
        }

        return l;
    }

    /**
     * Determines whether the user is allowed to select their preferred display
     * language through the site preferences.
     * 
     * @param wiConfig the WI configuration
     * @return true if the user is allowed to select their display language, otherwise false
     */
    public static boolean userCanSelectLanguage(WIConfiguration wiConfig) {
        // When integrated with Access Gateway, locale is always obtained from the HTTP
        // request and not from the user preferences. This is so that the UI is consistent
        // with that of Access Gateway (which uses the same approach).
        return !AGEUtilities.isAGEIntegrationEnabled(wiConfig);
    }

    public static WIContext constructWIContext(WIConfiguration wiConfig,
                                               WebAbstraction web,
                                               StaticEnvironmentAdaptor staticAdaptor,
                                               UserEnvironmentAdaptor userAdaptor,
                                               PlatformSpecificUtils platformSpecificUtils) {
        PageHistory.initPageHistory(web);

        // These are expensive to create, so only do it once per request
        ClientInfo clientInfo = (ClientInfo)web.getRequestContextAttribute(CLIENT_INFO_KEY);
        if (clientInfo == null) {
            clientInfo = (ClientInfo)web.getSessionAttribute(SV_CLIENT_INFO);
            if (clientInfo == null) {
                clientInfo = new ClientInfo();
                try {
                    clientInfo.initialize(web.getRequestHeader("User-Agent"));
                } catch (IllegalArgumentException e) {
                    handleNullUserAgent(web, staticAdaptor);
                }
                web.setSessionAttribute(SV_CLIENT_INFO, clientInfo);
            }
            web.setRequestContextAttribute(CLIENT_INFO_KEY, clientInfo);
        }

        String USER_PREFS_KEY = "com.citrix.wi.UserPreferences";
        UserPreferences userPrefs = (UserPreferences)web.getRequestContextAttribute(USER_PREFS_KEY);
        if (userPrefs == null) {
            userPrefs = Include.getUserPrefs(clientInfo, userAdaptor, wiConfig);
            web.setRequestContextAttribute(USER_PREFS_KEY, userPrefs);
        }

        String CURRENT_LOCALE_KEY = "java.util.Locale.Current";
        Locale currentLocale = (Locale)web.getRequestContextAttribute(CURRENT_LOCALE_KEY);
        if (currentLocale == null) {
            currentLocale = Include.getCurrentLocale(userPrefs, wiConfig, userAdaptor, staticAdaptor, web);
            web.setRequestContextAttribute(CURRENT_LOCALE_KEY, currentLocale);
        }


        return new WIContext(wiConfig,
                             web,
                             userAdaptor,
                             staticAdaptor,
                             clientInfo,
                             userPrefs,
                             currentLocale,
                             platformSpecificUtils);
    }


    /**
     * Handles a null user agent string.  Null user agents are not permitted for
     * most page requests.  See PAGES_ACCEPTING_NULL_USER_AGENT for a list of
     * pages that explicitly allow null user agent strings in their requests.
     *
     * @param web The web abstraction
     * @param staticAdaptor The Static Environment Adaptor
     */
    public static void handleNullUserAgent(WebAbstraction web, StaticEnvironmentAdaptor staticAdaptor) {
        boolean acceptsNullUserAgent = checkRequestAcceptsNullUserAgent(web.getAbsoluteRequestDirectory());

        if (!acceptsNullUserAgent) {
            // Log message
            logNullUserAgentMessage(staticAdaptor, web.getUserHostAddress());

            // Cause a redirect to the internal error page.
            web.setSessionAttribute(SV_NULL_USER_AGENT, Boolean.TRUE);
        }
    }

    /**
     * Writes a message to the event log indicating that a request was received
     * without a user agent string specified.
     *
     * @param staticEnvAdaptor
     * @param ipAddress The IP Address of the client, included to help the admin
     * with troubleshooting the error.
     */
    public static void logNullUserAgentMessage(StaticEnvironmentAdaptor staticEnvAdaptor, String ipAddress) {
        if (staticEnvAdaptor == null) return;
        ResourceBundleFactory bundleFactory = (ResourceBundleFactory)
                staticEnvAdaptor.getApplicationAttribute(AppAttributeKey.RESOURCE_BUNDLE_FACTORY);
        if (bundleFactory == null) return;
        staticEnvAdaptor.getDiagnosticLogger().log(
            MessageType.ERROR,
            new LocalizableString(bundleFactory, KEY_NULL_USER_AGENT, ipAddress));
    }

    /**
     * Checks a request URL against the whitelist of pages accepting a null user
     * agent to see whether the null user agent should be tolerated.
     *
     * @param requestPath The URL to lookup
     * @return <code>true</code> if the requested path can accept a null user
     * agent, else <code>false</code>.
     */
    public static boolean checkRequestAcceptsNullUserAgent(String requestPath) {
        boolean acceptsNullUserAgent = false;
        for (int i = 0; i < PAGES_ACCEPTING_NULL_USER_AGENT.length; i++) {
            String pageAcceptingNullUserAgent = PAGES_ACCEPTING_NULL_USER_AGENT[i] + Constants.PAGE_EXTENSION;
            if (requestPath.endsWith(pageAcceptingNullUserAgent)) {
                acceptsNullUserAgent = true;
                break;
            }
        }
        return acceptsNullUserAgent;
    }

    /**
     * Returns the String that needs to be appended to the frame names. If the
     * frame suffix does not exist in the client session state then a new one is
     * created and added to the client session state to be used in subsequent
     * requests.
     *
     * @param wiContext The WIContext
     */
    public static String getFrameSuffix(UserEnvironmentAdaptor uae) {
        String frameSuffix = (String)uae.getSessionState().get(Constants.SV_FRAME_SUFFIX);
        if (frameSuffix == null) {
            frameSuffix = createFrameSuffix();
            uae.getSessionState().put(Constants.SV_FRAME_SUFFIX, frameSuffix);
        }
        return WebUtilities.escapeHTML(frameSuffix);
    }

    /**
     * Get the home page for the current site. This will usually be the applist
     * page but will be the direct launch page if launching a persistent URL.
     *
     * @param wiContext Request info.
     * @return the home page for the current site
     */
    public static String getHomePage(WIContext wiContext) {
        return LaunchUtilities.getDirectLaunchModeInUse(wiContext)
                    ? Constants.PAGE_DIRECT_LAUNCH
                    : Constants.PAGE_APPLIST;
    }

    /**
     * Get the IconCache, creating a new one if necessary.
     *
     * @return the icon cache.
     */
    public static IconCache getIconCache(StaticEnvironmentAdaptor adaptor) {
        IconCache iconCache = (IconCache)adaptor.getApplicationAttribute(AppAttributeKey.ICON_CACHE);
        if (iconCache == null) {
            iconCache = new IconCache();
            adaptor.setApplicationAttribute(AppAttributeKey.ICON_CACHE, iconCache);
        }
        return iconCache;
    }

    public static LanguageManager getLanguageManager(StaticEnvironmentAdaptor adaptor) {
        return (LanguageManager)adaptor.getApplicationAttribute(AppAttributeKey.LANGUAGE_MANAGER);
    }

    public static ClientManager getClientManager(StaticEnvironmentAdaptor adaptor) {
        return (ClientManager)adaptor.getApplicationAttribute(AppAttributeKey.CLIENT_MANAGER);
    }

    /**
     * Determines if RADE is supported on the user's operating system.
     *
     * @param wiContext WIContext.
     * @return <code>true</code> if RADE is supported, otherwise
     * <code>false</code>.
     */
    public static boolean getOsRadeCapable(ClientInfo clientInfo, UserEnvironmentAdaptor uea) {
        boolean isWindows2kOrLater = (clientInfo.osVersionMajor() >= 5) && (clientInfo.osWin32() || clientInfo.osWin64());
        boolean isRADEClientAvailable = Include.getWizardState(uea).isRADEClientAvailable();

        return isWindows2kOrLater || isRADEClientAvailable;
    }

    /**
     * Determines if RADE is supported on the user's operating system.
     */
    public static boolean getOsRadeCapable(WIContext wiContext) {
        return getOsRadeCapable(wiContext.getClientInfo(), wiContext.getUserEnvironmentAdaptor());
    }

    /**
     * Gets the raw User Preferences. The returned preferences in no way be
     * filtered or modified according to the admin's configuration. These
     * preferences must not be trusted.
     *
     * @param envAdaptor The environment adaptor for this request.
     * @return the unmodified user preferences
     */
    public static UserPreferences getRawUserPrefs(UserEnvironmentAdaptor envAdaptor) {
        UserPreferences userPersistentPrefs = new UserPreferences(envAdaptor.getUserState());
        UserPreferences userSessionPrefs = new UserPreferences(envAdaptor.getClientSessionState());
        return userPersistentPrefs.overriddenBy(userSessionPrefs);
    }

    /**
     * This returns the URL to use for reconnecting after logon.
     * If it returns null, you shouldn't do the reconnect.
     *
     * @param wiContext
     * @return null or the URL to reconnect to
     */
    public static String getReconnectAtLoginUrl(WIContext wiContext) {
        String result = null;
        if (!Authentication.isAnonUser(wiContext.getUserEnvironmentAdaptor())
                        && isWorkspaceControlEnabled(wiContext) && isReconnectAtLogin(wiContext)
                        && DetectionUtils.clientsAvailableForDetection(wiContext, null)) {
            result = Constants.PAGE_RECONNECT + "?" + Constants.QSTR_LOGINID + "="
                            + WebUtilities.escapeURL(Constants.VAL_ON) + "&" +
                            SessionToken.QSTR_TOKENNAME + "=" + SessionToken.get(wiContext);
            if (wiContext.getClientInfo().osSymbian()) {
                // Symbian clients have issues with resolving relative resources
                // as they try
                // to resolve the path from the current frame source instead of
                // frameset
                String sitePath = wiContext.getWebAbstraction().getRequestPath();
                sitePath = sitePath.substring(0, sitePath.lastIndexOf('/'));
                result = sitePath + "/" + result;
            }

        }
        return result;
    }

    private static boolean isReconnectAtLogin(WIContext wiContext) {
        WebAbstraction web = wiContext.getWebAbstraction();
        Boolean b = (Boolean)web.getSessionAttribute(Constants.SV_RECONNECT_AT_LOGIN);
        return (b != null ? b.booleanValue() : true);
    }

    /**
     * Returns whether logging off WI also logs off the user's remote applications.
     *
     * @return true if apps are also logged off.
     */
    public static boolean logOffApplications(WIContext wiContext) {
        return !Authentication.isAnonUser(wiContext.getUserEnvironmentAdaptor()) &&
            Include.isWorkspaceControlEnabled(wiContext) && Boolean.TRUE.equals(wiContext.getUserPreferences().getLogoffApps());
    }

    /**
     * Gets the selected client for launching remote applications, taking into
     * account the user's preferences and the user's platform. If streaming app
     * access is preferred then the LOCAL_ICA client is returned as this is used
     * for launching ICA-fallback.
     *
     * @param wiContext WIContext.
     * @param userPrefs userPreferences.
     * @return The client to be used.
     */
    public static MPSClientType getSelectedRemoteClient(WIContext wiContext) {

        MPSClientType result = modifyClientForPlatform(wiContext, wiContext.getUserPreferences().getClientType());
        return result;
    }

    /**
     * Gets the effective access method.
     *
     * Sometimes the effective access method is REMOTE, even though the access method in
     * the user preferences is STREAMING.
     */
    public static AppAccessMethod getEffectiveAppAccessMethod(WIContext wiContext) {

        AppAccessMethod effectiveAccessMethod = AppAccessMethod.REMOTE;

        // if streaming is an available app access method
        // and the streaming client is installed, use streaming
        AppAccessMethodConfiguration aamConfig = wiContext.getConfiguration().getAppAccessMethodConfiguration();
        if (aamConfig.isEnabledAppAccessMethod(AppAccessMethod.STREAMING)
                        && getWizardState(wiContext).isRADEClientAvailable()) {
            effectiveAccessMethod = AppAccessMethod.STREAMING;
        }

        return effectiveAccessMethod;
    }

    /**
     * Returns a resource bundle key which describes the reason why the a
     * streaming-only application could not be launched.
     *
     * @return The resource bundle key.
     */
    public static String getStreamingLaunchFailureReasonKey(UserEnvironmentAdaptor envAdaptor, ClientInfo sClientInfo) {
        String key = "StreamingAppLaunchFailureStreamingNotEnabled";

        if (!getOsRadeCapable(sClientInfo, envAdaptor)) {
            key = "StreamingAppLaunchNoOsSupport";
        } else if (!getBrowserRadePluginCapable(sClientInfo, envAdaptor)) {
            key = "StreamingAppLaunchFailureNoBrowserSupport";
        } else if (!Include.getWizardState(envAdaptor).isRADEClientAvailable()) {
            key = "StreamingAppLaunchFailureNoClientInstalled";
        }

        return key;
    }

    // ASP only...
    public static String getSuffixFromMessageKey(String messageKey, WIContext wiContext) {
        return getSuffixFromMessageKey(wiContext, messageKey);
    }

    /**
     * Returns a suffix to be appended onto a particular message in the Message
     * Center.
     *
     * @param messageKey the resource bundle key to the message that will be
     * displayed in the Message Center.
     * @param currentLocale the user's current locale
     */
    public static String getSuffixFromMessageKey(WIContext wiContext, String messageKey) {
        String result = null;

        // Check if the wizard captions are enabled
        InstallCaptionState installCaptions = wiContext.getConfiguration().getClientDeploymentConfiguration().getShowInstallCaption();
        boolean installCaptionEnabled = !InstallCaptionState.OFF.equals(installCaptions);

        if ("CannotReconnectApps".equals(messageKey) || "FailedToFindRDPClient".equals(messageKey)) {
            // show a link only if wizard captions are enabled
            // and enable the zone change
            // and only look at the currently selected client
            if (installCaptionEnabled) {
                result = " <a id=\"wizardLink\" href=\"" + Constants.PAGE_WIZARD_PRE_INPUT
                       + "?" + Constants.QSTR_DETECT_CURRENT + "=" + Constants.VAL_TRUE
                       + "&" + Constants.QSTR_SHOW_ZONE + "=" + Constants.VAL_TRUE
                       + "\">" + wiContext.getString("WizardAssitance") + "</a>";
            }
        } else if ("StreamingAppLaunchFailureNoClientInstalled".equals(messageKey)) {
            // show a link only if wizard captions are enabled
            // and run the wizard in streaming mode
            if (installCaptionEnabled) {
                result = " <a id=\"wizardLink\" href=\"" + Constants.PAGE_WIZARD_PRE_INPUT
                       + "?" + Constants.QSTR_CLIENT_TYPE + "=" + Constants.VAL_ACCESS_METHOD_STREAMING
                       + "\">" + wiContext.getString("WizardAssitance") + "</a>";
            }
        } else if ("AlwaysUseJavaClient".equals(messageKey)) {
            // always allow the user to choose the java client
            result = " <a id=\"wizardLink\" href=\"" + Constants.PAGE_WIZARD_PRE_INPUT
                    + "?" + Constants.QSTR_FORCE_ICA_JAVA + "=" + Constants.VAL_TRUE
                    + "\">" + wiContext.getString("AlwaysUseJavaClientSuffix") + "</a>";
        } else {
            result = AccountSelfService.getAccountSelfServiceSuffix(wiContext, messageKey);
        }

        return result;
    }

    /**
     * Gets the User Preferences. The returned preferences have been modified
     * according to the current admin configuration and appropriate defaults
     * have been applied.
     *
     * Note: This is only used while setting up the WIContext as
     * this call is expensive and creates a NEW user preferences object
     *
     * @param wiContext the current WIContext object
     * @param envAdaptor The environment adaptor for this request.
     * @return the admin-settings modified user preferences
     */
    public static UserPreferences getUserPrefs(ClientInfo clientInfo, UserEnvironmentAdaptor envAdaptor,
                    WIConfiguration wiConfig) {
        UserPreferencesFilter adminPolicy = new UserPreferencesFilter(wiConfig);
        UserPreferences rawUserPrefs = Include.getRawUserPrefs(envAdaptor);
        UserPreferences wiDefaults = UserPreferences.getDefaults();
        UserPreferences adminDefaults = wiDefaults.overriddenBy(new UserPreferences(wiConfig));

        UserPreferences result = adminDefaults;
        result = result.overriddenBy(rawUserPrefs);
        result = result.restrictedBy(adminPolicy, adminDefaults);

        return result;
    }

    /**
     * The returned preferences have been modified according to the current admin
     * configuration and appropriate defaults have been applied.
     * Note: This is only used while setting up the WIContext.
     *
     * @param envAdaptor The environment adaptor for this request.
     * @return the user preferences
     */
    public static UserPreferences getUserPrefs(WIContext wiContext) {
        return getUserPrefs(wiContext.getClientInfo(), wiContext.getUserEnvironmentAdaptor(), wiContext
                        .getConfiguration());
    }

    /**
     * Determine whether the client connection is secure.
     */
    public static boolean isClientConnectionSecure(UserEnvironmentAdaptor envAdaptor) {
        boolean isSecure = false;
        if (envAdaptor != null) {
            isSecure = Constants.VAL_TRUE.equalsIgnoreCase((String)envAdaptor.getClientSessionState().get(
                                                           Constants.COOKIE_CLIENT_CONN_SECURE));
        }
        return isSecure;
    }

    public static boolean isCompactLayout(WIContext wiContext) {
        boolean bCompactLayout = false;

        if (AGEUtilities.isAGEEmbeddedMode(wiContext)) {
            // AGE embedded mode uses a tweaked compact layout
            bCompactLayout = true;
        } else {
            LayoutType layoutType = wiContext.getUserPreferences().getLayoutType();

            if (layoutType == LayoutType.AUTO) {
                int screenWidth = UIUtils.getScreenWidth(wiContext);
                // The auto-switch for compact mode is dependent
                // upon the device screen width.
                bCompactLayout = (screenWidth > 0) && (screenWidth <= 500);
            } else {
                bCompactLayout = (layoutType == LayoutType.COMPACT);
            }
        }

        return bCompactLayout;
    }

    /**
     * Test for layout mode
     */
    public static LayoutMode getLayoutMode(WIContext wiContext) {
        return wiContext.getConfiguration().getUIConfiguration().getSiteLayoutMode();
    }

    /**
     * Tests whether the user is logged in
     * @return <code>true</code> if logged in, else <code>false</code>
     */
    public static boolean isLoggedIn(WebAbstraction web) {
        AuthenticationState authenticationState = Authentication.getAuthenticationState(web);
        return (authenticationState != null) && authenticationState.isAuthenticated();
    }

    public static boolean isOSEUEMSupport(WIContext wiContext) {
        return (!wiContext.getClientInfo().osPocketPC() && !wiContext.getClientInfo().osMac());
    }

    /**
     * Checks whether Workspace Control is enabled.
     */
    public static boolean isWorkspaceControlEnabled(WIContext wiContext) {
        boolean result = false;
        int rc = getWorkspaceControlLevel(wiContext);
        switch (rc) {
            case Constants.RC_ENABLED:
                result = (Include.getSelectedRemoteClient(wiContext) != MPSClientType.EMBEDDED_RDP);
                break;
            case Constants.RC_DISABLED:
                result = false;
                break;
            case Constants.RC_JAVACLIENT_ONLY:
                result = (Include.getSelectedRemoteClient(wiContext) == MPSClientType.JAVA);
                break;
            default:
        }
        return result;
    }

    /**
     * Apply rules to modify the user's prefered remote access client depending
     * on: - the os - the browser
     *
     * @param wiContext WIContext.
     * @param client The user's prefered client.
     * @param userPrefs userPreferences.
     * @return The client to be used.
     */
    private static MPSClientType modifyClientForPlatform(WIContext wiContext, MPSClientType client) {
        MPSClientType result = client;

        ClientInfo clientInfo = wiContext.getClientInfo();
        if (clientInfo.osPocketPC() || clientInfo.osWinCE()) {
            result = MPSClientType.LOCAL_ICA;
        } else if (clientInfo.osMac() && clientInfo.isSafari()) {
            result = MPSClientType.JAVA;
            if (!clientInfo.isSafariOne() && client == MPSClientType.LOCAL_ICA) {
                // Always use Java client for Safari on Mac
                // Except for Safari newer than v1, which can use local client
                result = MPSClientType.LOCAL_ICA;
            }
        }

        return result;
    }

    /**
     * Modify the application launch info.
     *
     * @param appLaunchInfo The launch info.
     * @param clientType the client type used for launch.
     */
    public static void modifyLaunchSettings(AppLaunchInfo appLaunchInfo, ClientType clientType) {
        // CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP
        //
        // CUSTOMIZATION POINT
        //
        // Modifying the appLaunchInfo object here will affect the sessions for the specified client types.
        //
        // CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP
        return;
    }

    /**
     * Saves the User Preferences and, if necessary, updates the access prefs in the supplied
     * UserContext
     *
     * @param rawNewPrefs The preferences to write - this should have been originally retrieved using getRawUserPrefs as we do not
     *  want default values to have been filled in, since that would cause the default values to become set as if the user had explicitly requested them.
     * @param envAdaptor The environment adaptor for this request.
     * @param userContext The current UserContext
     */
    public static void saveUserPrefs(UserPreferences rawNewPrefs, WIContext wiContext, UserContext userContext) {
        if (userContext == null) {
            throw new IllegalArgumentException("Non-null UserContext required when updating user prefs post-login");
        }
        saveUserPrefsInternal(rawNewPrefs, wiContext, userContext);
    }

    /**
     * Saves the User Preferences if changed pre-login (ie with no UserContext).
     *
     * @param userPrefs The preferences to write - this should have been originally retrieved using getRawUserPrefs as we do not
     *  want default values to have been filled in, since that would cause the default values to become set as if the user had explicitly requested them.
     * @param envAdaptor The environment adaptor for this request.
     */
    public static void saveUserPrefsPreLogin(UserPreferences rawNewPrefs, WIContext wiContext) {
        if (isLoggedIn(wiContext.getWebAbstraction())) {
            throw new RuntimeException("saveUserPrefsPreLogin should not be called once logged in");
        }
        saveUserPrefsInternal(rawNewPrefs, wiContext, null);
    }

    /**
     * Saves the User Preferences and, if necessary, updates the access prefs in the supplied
     * UserContext
     *
     * @param rawNewPrefs The preferences to write - this should have been originally retrieved using getRawUserPrefs as we do not
     *  want default values to have been filled in, since that would cause the default values to become set as if the user had explicitly requested them.
     * @param envAdaptor The environment adaptor for this request.
     * @param userContext The current UserContext (may by null if no current UserContext ie pre-login)
     */
    private static void saveUserPrefsInternal(UserPreferences rawNewPrefs, WIContext wiContext, UserContext userContext) {

        // Find the preference defaults and the settings the user is allowed to override
        UserPreferences adminDefaults = UserPreferences.getDefaults().overriddenBy(new UserPreferences(wiContext.getConfiguration()));
        UserPreferencesFilter adminPolicy = new UserPreferencesFilter(wiContext.getConfiguration());

        // ensure only contains non-null values for prefs which the user is currently allowed to set
        UserPreferences rawNewFilteredPrefs = rawNewPrefs.restrictedBy(adminPolicy, new UserPreferences());

        // Now generate the 'processed' new prefs incorporating any default values
        UserPreferences processedNewPrefs = adminDefaults.overriddenBy(rawNewFilteredPrefs);

        // record whether access prefs and/or locale will need updating before we overwrite the current user prefs
        boolean localeChanged = !Utils.safeEquals(wiContext.getUserPreferences().getLocale(), processedNewPrefs.getLocale());

        // Save the raw prefs (ie without defaults) to the user state
        rawNewFilteredPrefs.saveTo(wiContext.getUserEnvironmentAdaptor().getUserState());

        // Update the user prefs with the new prefs (including defaults)
        wiContext.getUserPreferences().copyFrom(processedNewPrefs);

        // Note don't bother checking whether AccessPrefs changed as it would
        // cost as much as updating
        if (userContext != null) {
            wiContext.getUserPreferences().updateAccessPrefs(userContext.getAccessPrefs());
        }

        if (localeChanged) {
            wiContext.setCurrentLocale(
                    getCurrentLocale(wiContext.getUserPreferences(),
                                     wiContext.getConfiguration(),
                                     wiContext.getUserEnvironmentAdaptor(),
                                     wiContext.getStaticEnvironmentAdaptor(),
                                     wiContext.getWebAbstraction()));
        }
    }

    public static int getWorkspaceControlLevel(WIContext wiContext) {
        WIConfiguration config = wiContext.getConfiguration();
        int result = Constants.RC_DISABLED;

        if (config.getAppAccessMethodConfiguration().isEnabledAppAccessMethod(AppAccessMethod.REMOTE) &&
            config.getWorkspaceControlConfiguration().getEnabled() &&
            !AGEUtilities.isAGEEmbeddedOrIndirectMode(wiContext)) {

            IcoStatus icoStatus = IcoStatus.NOT_PRESENT;
            if (!Include.clientWizardSupported(wiContext)) {
                // If we can't detect run the wizard
                // just default to not pass through
                icoStatus = IcoStatus.NOT_PASSTHROUGH;
            } else {
                // get the ICO from the wizard
                ClientDetectionWizardState sWizardInfo = Include.getWizardState(wiContext);
                if (sWizardInfo.getIcoStatusResult() != null) {
                    icoStatus = sWizardInfo.getIcoStatusResult();
                }
            }

            if (IcoStatus.NOT_PRESENT.equals(icoStatus)) {
                // ICO is not present and we are using Java Client then enable Workspace Control
                result = Constants.RC_JAVACLIENT_ONLY;
            } else {
                // If we are not running in pass-through, enable Workspace Control
                result = IcoStatus.NOT_PASSTHROUGH.equals(icoStatus) ? Constants.RC_ENABLED : Constants.RC_DISABLED;
            }
        }

        return result;
    }

    /**
     * This method generates the application link to be displayed in html from the
     * ResourceInfo object. This method also adds the required javascript functions for
     * EUEMSupport to the link. This method also adds the CSRF Query Token
     *
     * @param wiContext WIContext
     * @param resInfo ResourceInfo to generate the link for that resource.
     *
     * @return string for the application link, null if resInfo is null.
     */
    public static String processAppLink(WIContext wiContext, ResourceInfo resInfo) {
        if (resInfo == null) {
            return null;
        }

        boolean isAssignOnFirstUse = false;
        if (resInfo instanceof DesktopInfo) {
            isAssignOnFirstUse = ((DesktopInfo)resInfo).getAssignmentType() == DesktopAssignmentType.ASSIGN_ON_FIRST_USE;
        }

        return processAppLink(wiContext, resInfo.getId(), null, false, isAssignOnFirstUse);
    }

    /**
     * This method generates the application link to be displayed in html from the
     * AppId. This method also adds the required javascript functions for
     * EUEMSupport to the link. This method also adds the CSRF Query Token
     *
     * @param wiContext WIContext
     * @param appId appId to generate the link for that resource.
     *
     * @return string for the application link, null if resInfo is null.
     */
    public static String processAppLink(WIContext wiContext, String appId) {
        return processAppLink(wiContext, appId, null, false, false);
    }

    /**
     * This method generates the application link to be displayed in html from the
     * appId String with the given csrfQueryToken. This method also adds the required javascript functions for
     * EUEMSupport to the link. This adds the given CSRF Query Token to the link if not null otherwise it gets
     * the token and adds it.
     *
     * @param wiContext WIContext
     * @param appId String to generate the link for that resource.
     * @param csrfQueryToken String to add as CSRF Query token. If this is null then it gets the token from the session
     * @param isLaunchOnly boolean <code>true</code> if it creates a link which will not save launched app data and
     *                             so on the direct launch page it stops a page refresh from repeating the launch,
     *                             but it's not suitable for creating bookmarks.
     * @param requiresDesktopAssignment boolean <code>true</code> if the link's onclick handler should call the 'assignDesktop'
     *                             javascript function instead of 'launch'.
     * @return string for the application link, null if resInfo is null.
     */
    public static String processAppLink(WIContext wiContext, String appId, String csrfQueryToken, boolean isLaunchOnly, boolean requiresDesktopAssignment) {
        if (appId == null) {
            return null;
        }
        String result = "";
        String appClick = "";
        String appMouseDown = "";

        String appHref = Include.getAppLinkRaw(wiContext, appId, csrfQueryToken, isLaunchOnly);

        // As the string is being used as an href it must
        // have all '&' characters encoded as '&amp;'

        appHref = WebUtilities.encodeAmpersands(appHref);

        // Add javascript click time handler - needs to be on
        // mousedown too in order to catch rt-click open/save-as
        // events (which don't generate a click in all browsers).
        //
        // Note that QSTR_METRIC_LAUNCH_ID will be added to bookmarked URL's, but it is harmless as
        // it is discarded when launch is effected via a bookmark (default.aspx will create a
        // new QSTR_METRIC_LAUNCH_ID).

        if (isOSEUEMSupport(wiContext) && !requiresDesktopAssignment) {
            // Add click time to anchor.
            appClick = "addCurrentTimeToHref(this,\'" + appHref + "\',\'" + Constants.QSTR_METRIC_LAUNCH_ID + "\');";
            appMouseDown = "addCurrentTimeToHref(this,\'" + appHref + "\',\'" + Constants.QSTR_METRIC_LAUNCH_ID + "\');";
        }

        appClick += requiresDesktopAssignment ? "assignDesktop(this);" : "launch(this);";
        appClick += "return false;";

        if (appHref.length() > 0) {
            result = "href=" + "\"" + appHref + "\"";
        }

        result = result + " onClick=\"resetSessionTimeout();clearFeedback();";
        result = result + appClick + "\"";

        if (appMouseDown.length() > 0) {
            result = result + " onMouseDown=\" " + appMouseDown + "\"";
        }

        return result;
    }

    /**
     * This builds the initial href for a launch.
     * @param wiContext WIContext
     * @param resInfo the resource info for the application the link is for
     * @param csrfQueryToken String to add as CSRF Query token. If this is null then it gets the token from the session
     * @return the initial href for a launch of the given resource
     */
    public static String getAppLinkRaw(WIContext wiContext, ResourceInfo resInfo, String csrfQueryToken) {
        if (resInfo == null) {
            return null;
        }
        return getAppLinkRaw(wiContext, resInfo.getId(), csrfQueryToken, false);
    }

    /**
     * This builds the initial href for a launch
     * @param wiContext WIContext
     * @param appId the appID of the application the link is for
     * @param csrfQueryToken String to add as CSRF Query token. If this is null then it gets the token from the session
     * @param isLaunchOnly boolean if <code>true</code> it creates a link which will not save launched app data and
     *                             so on the direct launch page it stops a page refresh from repeating the launch,
     *                             but it's not suitable for creating bookmarks.
     * @return the initial href for a launch of the given resource
     */
    public static String getAppLinkRaw(WIContext wiContext, String appId, String csrfQueryToken, boolean isLaunchOnly) {
        String appLink = Constants.PAGE_LAUNCHER + "?" +
                         LaunchUtilities.getLaunchAppQueryStringFragment(wiContext, appId);

        if (csrfQueryToken == null) {
            appLink += SessionToken.makeCsrfQueryToken(wiContext);
        } else {
            appLink += csrfQueryToken;
        }

        if (isLaunchOnly) {
            appLink += "&" + LaunchUtilities.QSTR_LAUNCH_ONLY + "=y";
        }

        return appLink;
    }

    /**
     * Gets the branding mode of the site, i.e. whether the site is branded for
     * * applications or desktops.
     *
     * @param wiContext the Web Interface context object
     * @return <code>UserInterfaceBranding</code> object specifying the branding mode
     */
    public static UserInterfaceBranding getSiteBranding(WIContext wiContext) {
        return wiContext.getConfiguration().getUIConfiguration().getUIBranding();
    }

    /**
     * This method returns whether the site has a single tab with AllResources selected.
     */
    public static boolean isSingleAllResourcesTab(WIContext wiContext) {
        ApplistTabConfig[] tabConfig = wiContext.getConfiguration().getUIConfiguration().getAccessPointTabs();
        if (tabConfig.length == 1) {
            if (isAllResourcesTab(tabConfig[0])) {
                return true;
            }
        }
        return false;
    }

    /**
     * Determines whether a given object represents the AllResources Tab.
     *
     * @param obj the object with which to compare
     * @return <code>true</code> if its an AllResources tab, else <code>false</code>
     */
    private static boolean isAllResourcesTab(ApplistTabConfig obj) {
        boolean result = false;
        if (obj != null) {
            result = Objects.equals(ApplistTabConfig.ALL_RESOURCES.getUniqueName(), obj.getUniqueName());
        }
        return result;
    }

    /**
     * Builds up markup for the div containing the delayed launch icon.
     *
     * @param wiContext the WI context
     * @param desktop ResourceControl object for the desktop
     * @param spinnerId the id of the spinner div
     * @return markup for the div containing the delayed launch icon
     */
    public static String getDelayedLaunchImg(WIContext wiContext, ResourceControl desktop, String spinnerId) {
        String imgClass = "delayedImageNone";

        if (desktop.isDelayedLaunch) {
            if (desktop.startInProgress) {
                imgClass = "delayedImageSpinner";
            } else {
                imgClass = "delayedImagePlay";
            }
        }

        return "<div class='" + imgClass + "'  id='" + spinnerId + "'><!-- --></div>";
    }

}

