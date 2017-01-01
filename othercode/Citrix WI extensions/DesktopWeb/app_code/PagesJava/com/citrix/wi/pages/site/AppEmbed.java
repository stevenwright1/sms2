/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.site;

import java.io.IOException;

import com.citrix.wi.config.ClientDeploymentConfiguration;
import com.citrix.wi.controls.AppEmbedControl;
import com.citrix.wi.controls.DelayedLaunchControl;
import com.citrix.wi.localization.ClientManager;
import com.citrix.wi.localization.ClientPackage;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.mvc.WebCookie;
import com.citrix.wi.pages.StandardLayout;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.DelayedLaunchUtilities;
import com.citrix.wi.pageutils.Embed;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.LaunchUtilities;
import com.citrix.wi.pageutils.ResourceEnumerationUtils;
import com.citrix.wi.pageutils.SessionUtils;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wi.pageutils.Utils;
import com.citrix.wi.types.BandwidthProfilePreference;
import com.citrix.wi.types.ClientPlatform;
import com.citrix.wi.types.JavaPackage;
import com.citrix.wi.ui.JICAPackages;
import com.citrix.wi.util.ClientInfo;
import com.citrix.wi.util.Platform;
import com.citrix.wing.AccessTokenException;
import com.citrix.wing.AppLaunchInfo;
import com.citrix.wing.AppLaunchParams;
import com.citrix.wing.LaunchConversionException;
import com.citrix.wing.MessageType;
import com.citrix.wing.NoSuchResourceException;
import com.citrix.wing.UnsupportedResourceOperationException;
import com.citrix.wing.ResourceUseException;
import com.citrix.wing.types.BandwidthProfile;
import com.citrix.wing.types.ClientType;
import com.citrix.wing.types.LaunchConversionErrorType;
import com.citrix.wing.util.Strings;
import com.citrix.wing.webpn.ResourceInfo;
import com.citrix.wing.webpn.UserContext;
import com.citrix.wi.pageutils.DesktopLaunchHistory;
import com.citrix.wi.pageutils.SessionToken;
import com.citrix.wing.*;

public class AppEmbed extends StandardLayout {

    protected AppEmbedControl viewControl = new AppEmbedControl();

    public AppEmbed(WIContext wiContext) {
        super(wiContext);
        wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
    }

    // Constants required for dealing with cookie propagation to the Java Client
    private static final String AUTH_COOKIE = "WIAuthId";
    private static final String SESSION_COOKIE = (Platform.isDotNet() ? "ASP.NET_SessionId" : "JSESSIONID");
    private static final String[] JAVA_CLIENT_COOKIES = { SESSION_COOKIE, AUTH_COOKIE, Constants.USER_COOKIE_NAME };

    protected boolean performImp() throws IOException {
        WebAbstraction web = wiContext.getWebAbstraction();
        ClientDeploymentConfiguration cdconfig = wiContext.getConfiguration().getClientDeploymentConfiguration();
        ClientManager clientManager = Include.getClientManager(wiContext.getStaticEnvironmentAdaptor());

        String appId = LaunchUtilities.getQueryStringLaunchApp(wiContext);

        if (!LaunchUtilities.getDirectLaunchModeInUse(wiContext)) {
            // Clear any launch data from the session because we have
            //made it to launching the application
            LaunchUtilities.clearSessionLaunchData(wiContext);
        }

        String launchMethod = web.getQueryStringParameter(Constants.QSTR_LAUNCH_METHOD);
        viewControl.rdpClientClassID = Include.getWizardState(wiContext).getRdpClassID();
        viewControl.icaClientClassID = cdconfig.getClientPackageConfig(wiContext.getClientInfo().getClientPlatform()).getClassId();

        viewControl.radeClientClassID = cdconfig.getClientPackageConfig(ClientPlatform.STREAMING_WIN32).getClassId();

        viewControl.homePage = Include.getHomePage(wiContext);

        viewControl.QS = Constants.QSTR_LAUNCH_UID + "=1&" + LaunchUtilities.getLaunchAppQueryStringFragment(wiContext, appId) +
                            SessionToken.copyCsrfQueryToken(wiContext);
        viewControl.appName = appId;
        if (Include.isOSEUEMSupport(wiContext)) {
            // Pass on EUEM launch data into the launch URL.
            viewControl.QS = UIUtils.addMetricToQueryString(viewControl.QS, Constants.QSTR_METRIC_RECONNECT_ID,
                    web.getQueryStringParameter(Constants.QSTR_METRIC_RECONNECT_ID));
            viewControl.QS = UIUtils.addMetricToQueryString(viewControl.QS, Constants.QSTR_METRIC_LAUNCH_ID,
                    web.getQueryStringParameter(Constants.QSTR_METRIC_LAUNCH_ID));
        }

        int[] screenSize = UIUtils.validateClientWindowSize(
                web.getQueryStringParameter(Constants.QSTR_LAUNCH_WINDOW_WIDTH),
                web.getQueryStringParameter(Constants.QSTR_LAUNCH_WINDOW_HEIGHT));
        viewControl.desiredHRES = Integer.toString(screenSize[0]);
        viewControl.desiredVRES = Integer.toString(screenSize[1]);

        if (Constants.LAUNCH_METHOD_STREAMING.equals(launchMethod)) {
            viewControl.client = Embed.RADE_RCOCLIENT;
            viewControl.QS += "&" + Constants.QSTR_LAUNCH_METHOD + "=" + Constants.LAUNCH_METHOD_STREAMING;
        } else {
            // default to REMOTE launch method (ICA or RDP)
            viewControl.client = Embed.getScriptedHostedAppLaunchClient(wiContext);
        }

        viewControl.appSizeString = "WIDTH=" + viewControl.desiredHRES + " HEIGHT=" + viewControl.desiredVRES;

        boolean launchSuccess = true;
        if (Embed.ICA_JAVACLIENT.equals(viewControl.client)) {
            launchSuccess = handleJavaClientLaunch(clientManager, cdconfig);
        } else if (Embed.RDP_ACTIVEX.equals(viewControl.client)) {
            launchSuccess = handleRdpLaunch(appId);
        }

        viewControl.sessionDisconnected = wiContext.getString("SessionDisconnected");
        viewControl.sessionQuestionDisconnect = wiContext.getString("SessionQuestionDisconnect");

        if (Embed.ICA_JAVACLIENT.equals(viewControl.client)
                && Platform.isDotNet()
                && browserPropagatesCookiesToJRE(wiContext.getClientInfo())) {
            // Note, the cookies are added here (rather than earlier, as in JSP) to prevent
            // the user cookie from being added to the response twice.
            setCookies(wiContext);
        }

        if (launchSuccess) {
            DesktopLaunchHistory.getInstance(wiContext).addDesktop(appId);
        }

        return true;
    }

    private boolean handleJavaClientLaunch(ClientManager clientManager, ClientDeploymentConfiguration cdconfig) throws IOException {
        // Before we do anything else, check that the Java client is available on the server
        // The Java client's embedding tags separate path and package files, so the path here
        // points to the Java client directory
        ClientPackage clientPackage = clientManager.getClientPackage(ClientPlatform.JAVA, wiContext.getCurrentLocale());
        viewControl.clientPath = clientPackage.getUrl();

        if (viewControl.clientPath == null) {
            // The client is not available and launch cannot continue.
            // Set the URL that will display an error message.
            viewControl.redirectUrl = UIUtils.getMessageRedirectUrl(wiContext,
                Include.getHomePage(wiContext), MessageType.ERROR,
                "GeneralAppLaunchError", "NoJavaClientOnServer");

            // Ensure the default page doesn't try to launch the application again and just shows the error.
            UserContext userContext = SessionUtils.checkOutUserContext(wiContext);
            if (userContext != null) {
                LaunchUtilities.clearSessionLaunchData(wiContext);
                SessionUtils.returnUserContext(userContext);
            }

            return false;
        }

        // the url includes JICA-coreN.jar, this needs to be removed for the object tag
        viewControl.clientPath = Strings.replace(viewControl.clientPath, "JICA-coreN.jar", "");

        viewControl.initTag = (wiContext.getClientInfo().getBrowser() == ClientInfo.BROWSER_IE)
                        ? "onload=\"onInitIE();\""
                        : "onload=\"onInit();\"";

        if (Platform.isJava() && browserPropagatesCookiesToJRE(wiContext.getClientInfo())) {
            setCookies(wiContext);
        }

        // Determine whether we are using Jica Seamless
        boolean seamless = wiContext.getUserPreferences().getUseSeamless();

        //Fixed size for JICA ICA Connection Center
        if (seamless) {
            viewControl.desiredHRES = Constants.ICA_CONN_CENTER_HRES;
            viewControl.desiredVRES = Constants.ICA_CONN_CENTER_VRES;
        }

        // Obtain the list of packages
        JICAPackages jica = new JICAPackages(cdconfig, wiContext.getUserPreferences());
        BandwidthProfilePreference bandwidthProfilePreference = wiContext.getUserPreferences().getBandwidthProfilePreference();
        BandwidthProfile bandwidth = bandwidthProfilePreference == null ? null : bandwidthProfilePreference.toBandwidthProfile();
        if (bandwidth != null) {
            // For high bandwidth, audio is on.
            // For low, medium and medium-high bandwidth, audio is off.
            jica.setPackageEnabled(JavaPackage.AUDIO, (bandwidth == BandwidthProfile.HIGH));

            // For high and medium-high bandwidth, client printing is on.
            // For low and medium bandwidth, client printing is off.
            jica.setPackageEnabled(JavaPackage.PRINTER_MAPPING, (bandwidth == BandwidthProfile.HIGH || bandwidth == BandwidthProfile.MEDIUM_HIGH));
        }
        jica.setUseSeamless(seamless);

        viewControl.jicaCode = jica.getCode();
        viewControl.jicaPackages = jica.getArchives();
        viewControl.jicaUseZeroLatency = jica.isPackageEnabled(JavaPackage.ZERO_LATENCY);

        // Set the language for the applet
        viewControl.jicaLang = wiContext.getCurrentLocale().getLanguage();

        // Generate a URL pointing to CloseThisWindow.html. This will close the HTML
        // window after the app is terminated:
        WebAbstraction web = wiContext.getWebAbstraction();
        String currPath = web.getRequestPath();
        int pos = currPath.lastIndexOf("/");
        String file = currPath.substring(0, pos + 1);
        viewControl.closeURL = file + "../html/CloseThisWindow.html";

        // Check if the client platform is MAC OS and the client is using firefox
        // We are sending the java client the cookie information in a string as an applet parameter
        // in order to workaround a firefox cookie handling issue.
        if (!browserPropagatesCookiesToJRE(wiContext.getClientInfo())) {
            String cookieString = getJavaClientCookieString(web);
            if (!Strings.isEmpty(cookieString)) {
                viewControl.JICACookie = "<param name=HttpCookie value='" + cookieString + "'>";
            }
        }
        return true;
    }

    private AppLaunchInfo getLaunchInfoForRdpLaunch(String appId, UserContext userContext)
            throws IOException, AccessTokenException, UnsupportedResourceOperationException, NoSuchResourceException, ResourceUseException, LaunchConversionException
        {
        AppLaunchInfo launchInfo = LaunchUtilities.getCachedLaunchInfo(wiContext, appId);
        if (launchInfo != null) {
            LaunchUtilities.clearCachedLaunchInfo(wiContext, appId);
            return launchInfo;
        }

        // This launch may have been the result of a delayed launch
        DelayedLaunchControl delayedLaunchControl = DelayedLaunchUtilities.getDelayedLaunchControl(wiContext);
        if (delayedLaunchControl.isResourceReadyToLaunch(appId)) {
            launchInfo = delayedLaunchControl.getResourceLaunchInfo(appId);
            ResourceInfo resInfo = ResourceEnumerationUtils.getResource(appId, userContext);
            if (LaunchUtilities.canScriptLaunch(wiContext, resInfo)) {
                delayedLaunchControl.removeReadyToLaunchResource(appId);
            } else {
                delayedLaunchControl.changeReadyStatusToBlocked(appId);
            }
        } else {
            AppLaunchParams appLaunchParams = new AppLaunchParams(ClientType.RDP);
            LaunchOverride launchOverride = LaunchShared.getLaunchOverrideFromOverrideData(wiContext,
                                                               wiContext.getCurrentLocale(),
                                                               appId,
                                                               appLaunchParams.getLaunchClientType());
            appLaunchParams.setLaunchOverride(launchOverride);

            launchInfo = (AppLaunchInfo)userContext.launchApp(appId, appLaunchParams);
        }
        return launchInfo;
    }

    private boolean handleRdpLaunch(String appId) throws IOException {
        boolean launchSuccess = false;
        if ("".equals(viewControl.rdpLaunchErrorKey)) {
            UserContext userContext = SessionUtils.checkOutUserContext(wiContext);
            if (userContext != null) {
                try {
                    AppLaunchInfo launchInfo = getLaunchInfoForRdpLaunch(appId, userContext);

                    viewControl.rdpParams = userContext.convertToRDPClientParams(launchInfo, null);
                    viewControl.desiredHRES = viewControl.rdpParams.getParam("DesktopWidth");
                    viewControl.desiredVRES = viewControl.rdpParams.getParam("DesktopHeight");
                    launchSuccess = true;
                } catch (AccessTokenException ate) {
                    viewControl.rdpLaunchErrorKey = Utils.getAuthErrorMessageKey(ate);
                } catch (UnsupportedResourceOperationException pue) {
                    viewControl.rdpLaunchErrorKey = "UnsupportedClientType";
                } catch (NoSuchResourceException nsre) {
                    viewControl.rdpLaunchErrorKey = "AppRemoved";
                } catch (ResourceUseException rue) {
                    viewControl.rdpLaunchErrorKey = Constants.CONST_RESOURCE_ERROR;
                } catch (LaunchConversionException ace) {
                    if (ace.getErrorType() == LaunchConversionErrorType.UNSUPPORTED_ACCESS_TOKEN) {
                        viewControl.rdpLaunchErrorKey = "RDPUnsupportedAuthentication";
                    }
                    if (ace.getErrorType() == LaunchConversionErrorType.UNSUPPORTED_CONNECTION_ROUTE) {
                        viewControl.rdpLaunchErrorKey = "RDPUnsupportedConnection";
                    }
                }

                // Return the user context even during an error otherwise the apps will just keep launching
                // (Session variable is not cleared correctly) and you get and infinite loop.
                SessionUtils.returnUserContext(userContext);
            }

            if (viewControl.rdpParams == null && "".equals(viewControl.rdpLaunchErrorKey)) {
                viewControl.rdpLaunchErrorKey = "RDPLaunchError";
            }
        }
        return launchSuccess;
    }

    /**
     * Set the appropriate session cookies so that they are accessible from client script on both platforms.
     * This is necessary so that the Java client JRE can access these values.
     */
    private static void setCookies(WIContext wiContext) {
        WebAbstraction web = wiContext.getWebAbstraction();
        String path = web.getApplicationPath();

        // The JSP session cookie (JSESSIONID) is specified without a trailing slash
        // The ASP session cookie (ASP.NET_SessionId) is specified as just a slash
        addScriptAccessibleCookie(web, SESSION_COOKIE, (Platform.isDotNet() ? "/" : path), false);
        addScriptAccessibleCookie(web, AUTH_COOKIE, path + "/", false);
        addScriptAccessibleCookie(web, Constants.USER_COOKIE_NAME, path + "/", true);
        addScriptAccessibleCookie(web, Constants.COOKIE_WING_SESSION_NAME, path + "/", false);
        addScriptAccessibleCookie(web, Constants.COOKIE_DEVICE_NAME, path + "/", true);
    }

    /*
     * Set cookie in response, if supplied in request, as a cookie accessible from client script
     */
    private static void addScriptAccessibleCookie(WebAbstraction web, String name, String path, boolean persistent) {
        WebCookie cookie = web.getCookie(name);
        if (cookie != null) {
            WebCookie responseCookie = new WebCookie();
            responseCookie.setName(name);
            responseCookie.setValue(cookie.getValue());
            responseCookie.setPath(path);
            responseCookie.setHttpOnly(false);
            web.addCookie(responseCookie, persistent);
        }
    }

    /**
     * Collect session-id / auth-id / etc cookie values required by Java client
     */
    private static String getJavaClientCookieString(WebAbstraction web) {
        StringBuffer cookieString = new StringBuffer();

        for (int i = 0; i < JAVA_CLIENT_COOKIES.length; i++) {
            String name = JAVA_CLIENT_COOKIES[i];
            WebCookie cookie = web.getCookie(name);
            if (cookie != null) {
                cookieString.append(name);
                cookieString.append("=");
                cookieString.append(cookie.getValue());
                cookieString.append(";");
            }
        }

        // Backslash out double quotes since this will be given as a parameter to
        // the javascript CreateJICAApplet function.
        return Strings.replace(cookieString.toString(), "\"", "\\\"");
    }

    private static boolean browserPropagatesCookiesToJRE(ClientInfo clientInfo) {
        return !(clientInfo.osMac() && clientInfo.isFirefox());
    }

    protected String getBrowserPageTitleKey() {
        return null;
    }
}
