/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.clientdetect;

import java.io.StringWriter;
import java.net.MalformedURLException;
import java.net.URL;
import java.util.Iterator;
import java.util.Locale;
import java.util.Map;

import com.citrix.wi.clientdetect.models.WizardInput;
import com.citrix.wi.clientdetect.models.WizardViewModel;
import com.citrix.wi.clientdetect.util.WizardHelp;
import com.citrix.wi.config.ClientDeploymentConfiguration;
import com.citrix.wi.config.client.ClientPackageConfig;
import com.citrix.wi.localization.ClientManager;
import com.citrix.wi.localization.ClientPackage;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.mvc.WebCookie;
import com.citrix.wi.types.ClientPlatform;
import com.citrix.wi.util.ClientInfo;
import com.citrix.wi.util.ClientInfoUtilities;
import com.citrix.wi.util.Platform;
import com.citrix.wi.util.VersionNumber;
import com.citrix.wing.util.Cookies;
import com.citrix.wing.util.HttpURLChecker;
import com.citrix.wing.util.Strings;
import com.citrix.wing.util.WebUtilities;

/**
 * Static utility methods used for the wizard
 */
public class WizardUtil {

    private static final int NO_TABINDEX = -42;

    private WizardUtil() { }

    /**
     * Encodes the string as the error output of the wizard.
     * @param error the error to encode
     * @return the encoded error
     */
    public static String getError(String error) {
        return WizardConstants.ERROR + ":" + error;
    }

    /**
     * Gets the value of the cookie with the given name from the given string of all the cookies.
     * @param cookiesStr the string of all the cookies
     * @param name the name of the cookie
     * @return the value of the cookie
     */
    public static String getCookieValue(String cookiesStr, String name) {
        return getValueFromString(name, cookiesStr, ";");
    }

    /**
     * Gets the value of the given name from the name-value pairs str with the
     * given separator.
     */
    private static String getValueFromString(String name, String str, String sep) {
        String result = "";
        if (str != null) {
            int itemStart = str.indexOf(name + "=");
            if (itemStart != -1) {
                int valueStart = itemStart + name.length() + 1;
                int valueEnd = str.indexOf(sep, valueStart);
                if (valueEnd == -1) {
                    valueEnd = str.length();
                }
                result = str.substring(valueStart, valueEnd);
            }
        }

        return result;
    }

    /**
     * Gets the Wizard subcookie value of the given name from the
     * given string of all the cookies.
     *
     * @param cookiesStr string representing all the cookies
     * @param name the name of the Wizard subcookie
     *
     * @return wizard subcookie value
     */
    public static String getValueFromWizardCookie(String cookiesStr, String name) {
        String clientInfoCookie = getCookieValue(cookiesStr, WizardConstants.COOKIE_CLIENT_INFO);
        Map map = Cookies.parseCookieValue(clientInfoCookie);
        return (String)map.get(WizardConstants.COOKIE_WIZARD + name);
    }

    /**
     * Gets the value of the Wizard subcookie with the given name from the given <code>WebCookie</code>.
     *
     * @param cookie the cookie
     * @param name the name of the Wizard subcookie
     *
     * @return the wizard subcookie value
     */
    private static String getValueFromWizardCookie(WebCookie cookie, String name) {
        String result = null;
        Map subCookies = getSubCookies(cookie);
        if (subCookies != null) {
            result = (String) subCookies.get(WizardConstants.COOKIE_WIZARD + name);
        }
        return result;
    }


    /**
     * Sets the Wizard subcookie value into the given cookie.
     *
     * @param webAbstraction the web abstraction
     * @param cookie the cookie
     * @param name the name of the Wizard subcookie
     * @param value of the wizard subcookie
     */
    public static void setWizardCookie(WebAbstraction webAbstraction, WebCookie cookie,
                                       String name, String value) {
        Map map = getSubCookies(cookie);
        if (map != null && name != null){
            map.put(WizardConstants.COOKIE_WIZARD + name, value);
            cookie.setValue(Cookies.createCookieValue(map));
            String cookiePath = webAbstraction.getApplicationPath();
            if (!cookiePath.endsWith("/")) {
                cookiePath += "/";
            }
            cookie.setPath(cookiePath);
        }
    }

    private static Map getSubCookies(WebCookie cookie) {
        if (cookie == null) {
            return null;
        }
        return Cookies.parseCookieValue(cookie.getValue());
    }

    /**
     * Removes the Wizard subcookie from the given WebCookie.
     *
     * @param webAbstraction The web abstraction
     * @param cookie cookie that contains the wizard sub cookie
     * @param name of the wizard sub cookie to remove
     */
    public static void removeWizardCookie(WebAbstraction webAbstraction, WebCookie cookie, String name) {
        if (cookie != null && name != null) {
            Map cookies = Cookies.parseCookieValue(cookie.getValue());
            cookies.remove(WizardConstants.COOKIE_WIZARD + name);
            cookie.setValue(Cookies.createCookieValue(cookies));
            String cookiePath = webAbstraction.getApplicationPath();
            if (!cookiePath.endsWith("/")) {
                cookiePath += "/";
            }
            cookie.setPath(cookiePath);
        }
    }

    /**
     * Clears all the Wizard subcookie items from the Web Interface client information cookie.
     *
     * @param webAbstraction The web abstraction
     * @param usingAccessGateway <code>true</code> if the site is integrated with Access Gateway, otherwise <code>false</code>
     */
    public static void clearWizardCookieItems(WebAbstraction webAbstraction, boolean usingAccessGateway) {
        WebCookie cookieIn = webAbstraction.getCookie(WizardConstants.COOKIE_CLIENT_INFO);
        if (cookieIn == null) {
            return;
        }

        Map cookieItems = Cookies.parseCookieValue(cookieIn.getValue());

        Iterator iter = cookieItems.keySet().iterator();
        while (iter.hasNext()) {
            String name = (String)iter.next();
            if (name.startsWith(WizardConstants.COOKIE_WIZARD)) {
                iter.remove();
            }
        }

        // Create a new cookie to send in the response
        // Use the updated values
        WebCookie cookieOut = new WebCookie(cookieIn.getName(), Cookies.createCookieValue(cookieItems));
        cookieOut.setHttpOnly(false);

        // Do not set a cookie path if using Access Gateway as this can cause
        // problems
        if (!usingAccessGateway) {
            cookieOut.setPath(webAbstraction.getAbsoluteRequestDirectory());
        }

        webAbstraction.addCookie(cookieOut, false);
    }

    /**
     * Creates the markup for a download button
     *
     * @param page that the button goes to by default
     * @param enableButton Whether to enable to Download butto by default.  If true, the button is enabled.  Otherwise it is greyed out and a popup warning text is inserted.
     * @param wizardContext
     * @return the markup fragment
     */
    public static String getDownloadButtonMarkup(boolean enableButton, WizardContext wizardContext) {
        return getDownloadButtonMarkup(enableButton, wizardContext, NO_TABINDEX);
    }

    /**
     * Creates the markup for a download button
     *
     * @param page that the button goes to by default
     * @param enableButton Whether to enable the 'Download' button by default. If true, the button is enabled. Otherwise it is greyed out and a popup warning text is inserted.
     * @param wizardContext
     * @param tabIndex Tab index of tabbable elements within the markup.
     * @return the markup fragment
     */
    public static String getDownloadButtonMarkup(boolean enableButton, WizardContext wizardContext, int tabIndex) {
        StringWriter writer = new StringWriter();

        // download link
        writer.write("<p id=\"graphic_DownloadButton\"><a id=\"Download\" ");
        // Putting the onclick handler in the href seems to fix double click problems on Firefox
        writer.write("href=\"javascript:downloadButtonClicked();\" onmouseover=\"mouseOverDownloadButton()\" onmouseout=\"mouseOutDownloadButton()\"");
        if (tabIndex != NO_TABINDEX) {
            writer.write("tabindex=\"" + tabIndex + "\"");
        }
        writer.write(">");
        writer.write("<img id=\"downloadButtonImg\" src=\"");
        writer.write(WizardUtil.getDownloadButtonURL(enableButton, false, wizardContext));
        writer.write("\" alt=\"");
        writer.write(wizardContext.getString("DownloadButtonAltText"));
        writer.write("\" title=\"\"></a></p>");
        // This button is hidden by default and is shown only if high contrast mode is enabled.
        writer.write("<p id=\"highContrast_DownloadButton\"><input id=\"highContrast_Download\" type=\"submit\" ");
        writer.write("value=\"");
        writer.write(wizardContext.getString("Install"));
        writer.write("\" onClick=\"downloadButtonClicked(); return false;\"></p>");

        if (!enableButton) {
            // popup text; make it a link so that it can have focus and be read by a screenreader
            writer.write("<div id=\"downloadWarningText\" class=\"wiPopup invisiblePopup\">");
            writer.write("<a id=\"downloadWarningTextLink\" href=\"javascript:warningTextClicked();\"");
            if (tabIndex != NO_TABINDEX) {
                writer.write("tabindex=\"" + tabIndex + "\"");
            }
            writer.write(">");
            writer.write(wizardContext.getString("MustAgreeWithLicence"));
            writer.write("</a>");
            writer.write("</div>");
        }

        return writer.toString();
    }


    /**
     * Get the markup for a security more link with the given
     * message in the popup
     *
     * @param message
     * @param wizardContext
     * @return
     */
    public static String getSecurityMessageMarkup(String headingKey, String messageKey, WizardContext wizardContext) {
        return getSecurityMessageMarkup(headingKey, messageKey, wizardContext, NO_TABINDEX);
    }

    /**
     * Get the markup for a security more link with the given
     * message in the popup
     *
     * @param message
     * @param wizardContext the wizard context object
     * @param tabIndex Tab index of tabbable elements within the markup.
     * @return
     */
    public static String getSecurityMessageMarkup(String headingKey, String messageKey, WizardContext wizardContext, int tabIndex)
    {
        StringWriter writer = new StringWriter();
        writer.write("<div class=\"SecurityImplications\">");

        // the link
        writer.write("<p id=\"ShortSecurityContent\">");
        if (headingKey != null) {
            writer.write(wizardContext.getString(headingKey));
        }
        writer.write(" <a id=\"MoreSecurity\" class=\"inlineHelpLink\" href=\"#\"");
        if (tabIndex != NO_TABINDEX) {
            writer.write(" tabindex=\"" + tabIndex + "\"");
        }
        writer.write(">");
        writer.write(wizardContext.getString("LearnMoreSecurity"));
        writer.write("</a>");
        writer.write("</p>");

        // the popup
        writer.write("<div id=\"Popup_MoreSecurity\" class=\"wiPopup\"><p><b>");
        writer.write(wizardContext.getString("SecurityInformationPopupTitle"));
        writer.write("</b></p><p>");
        writer.write(wizardContext.getString(messageKey));
        writer.write("</p></div>");

        writer.write("</div>");
        return writer.toString();
    }

    /**
     * Get the URL for the download button
     * based on what is
     *
     * @param enabled if the button looks enabled
     * @param rollover if the button has a mouse over it
     * @param clientInfo
     * @return the URL for the download button
     */
    public static String getDownloadButtonURL(boolean enabled, boolean rollover, WizardContext wizardContext) {
        String result = null;
        if (!enabled) {
            result = WizardConstants.IMG_CODE_DOWNLOAD_DISABLED;
        } else {
            if (rollover) {
                result = WizardConstants.IMG_CODE_DOWNLOAD_ROLLOVER;
            } else {
                result = WizardConstants.IMG_CODE_DOWNLOAD_ENABLED;
            }
        }
        return getLocalizedImageName(result, wizardContext);
    }

    /**
     * This changes a string like image.png into
     * .../directory/en/image.gif (depending on what is required)
     *
     * @param imageName the image name that includes the <lang> tag.
     * @param wizardContext the wizard context object
     * @return
     */
    public static String getLocalizedImageName(String imageName, WizardContext wizardContext) {
        // do the gif/png conversion
        String result =  ClientInfoUtilities.getImageName(wizardContext.getClientInfo(), imageName);
        // get the url for the image requested in the correct locale
        result = wizardContext.getMui().getWizardImageUrl(result, wizardContext.getInputs().getLocale());
        return result;
    }

    /**
     * Gets the URL to used to download the client.
     *
     * @param wizardContext the wizard context object
     * @return String download url
     */
    public static String getClientDownloadUrl(WizardContext wizardContext) {
        return getClientDownloadUrl(wizardContext, wizardContext.getClientInfo().getClientPlatform());
    }

    /**
     * Gets the URL to use to download the client.
     *
     * @param wizardContext the wizard context object
     * @return String download url
     */
    public static String getStreamingClientDownloadUrl(WizardContext wizardContext) {
        return getClientDownloadUrl(wizardContext, ClientPlatform.STREAMING_WIN32);
    }


    /**
     * Gets the URL to use to download the client.
     *
     * @param wizardContext the wizard context object
     * @return String download url
     */
    private static String getClientDownloadUrl(WizardContext wizardContext, ClientPlatform platform) {
        ClientPackage clientPackage = getClientPackage(wizardContext, platform);
        return clientPackage.getUrl();
    }

    /**
     * Gets a client package using the supplied wizard context and client platform.
     *
     * @param wizardContext the wizard context object
     * @param platform the client platform
     * @return this never returns null
     */
    private static ClientPackage getClientPackage(WizardContext wizardContext, ClientPlatform platform) {
        ClientManager clientManager = wizardContext.getClientManager();
        Locale locale = wizardContext.getInputs().getLocale();
        ClientPackage clientPackage = clientManager.getClientPackage(platform, locale);
        return clientPackage;
    }

    /**
     * Get the configuration for the specified platform, from the client
     * deployment configuration
     *
     * @param the wizardContext
     * @return the <code>ClientPackageConfig</code>
     */
    private static ClientPackageConfig getClientPackageConfig(WizardContext wizardContext, ClientPlatform platform) {
        ClientDeploymentConfiguration config = WizardUtil.getClientDeploymentConfiguration(wizardContext);
        return config.getClientPackageConfig(platform);
    }

    /**
     * This decides if there is an ICA client on the server.
     * It returns false in the case that the user is taken to
     * citrix.com to download their client
     *
     * @param wizardContext
     * @return true if the client is on the server.
     */
    public static boolean isClientOnServer(WizardContext wizardContext) {
        ClientPackage clientPackage = getClientPackage(wizardContext,
                        wizardContext.getClientInfo().getClientPlatform());
        return clientPackage.isOnServer();
    }

    /**
    * Gets the custom caption defined for the given platform in the config file.
    *
    * @param cdConfig the client deployment configuration
    * @param sClientInfo client info
    *
    * @return the custom caption for the given platform otherwise return null if not defined.
    */
    public static String getCustomCaptionMessage(WizardContext wizardContext) {
        String customCaption = null;
        ClientPackageConfig config = WizardUtil.getClientPackageConfig(wizardContext, wizardContext.getClientInfo().getClientPlatform());
        if (config != null) {
            customCaption = config.getDescription();
        }
        return customCaption;
    }

    /**
     * Gets whether the browser is supported by the wizard
     * @param browser the browser
     * @return <code>true</code> if the browser is supported, otherwise <code>false</code>
     */
    public static boolean isSupportedBrowser(String browser) {
        return (browser == ClientInfo.BROWSER_IE ||
                browser == ClientInfo.BROWSER_FIREFOX ||
                browser == ClientInfo.BROWSER_SAFARI ||
                browser == ClientInfo.BROWSER_MOZILLA ||
                browser == ClientInfo.BROWSER_CHROME);
    }

    /**
     * Appends a query string name-value pair to a URL
     * @param url the URL to append to
     * @param queryStrKey the query string key
     * @param value the query string value to append
     * @return URL with query string appeneded
     */
    public static String getUrlWithQueryStr(String url, String queryStrKey, String value) {
        String result = "";
        String appendStr = "";
        if (queryStrKey == null || queryStrKey.length() == 0
            || value == null || value.length() == 0) {
            result = url;
        } else {
            //If the url already contains a query string then append the new one
            // at the end.
            if (url != null && url.indexOf("?") != -1){
                appendStr = "&";
            } else {
                appendStr = "?";
            }
            result = url + appendStr + queryStrKey + "=" + value;
        }
        return result;
    }

    /**
     * Appends the CSRF protection token to a URL
     * @param context the wizard context
     * @param url the URL
     * @return URL with CSRF token
     */
    public static String getUrlWithQueryStrWithCsrf(WizardContext context, String url)
    {
        // For URL with CSRF token.
        String token = getCsrfToken(context.getWebAbstraction());
        String finalUrl = getUrlWithQueryStr(url, WizardConstants.QSTR_TOKENNAME, token);

        return finalUrl;
    }

    /**
     * Appends a query string name-value pair to a URL and adds the CSRF protection token
     * @param context the wizard context
     * @param url the URL to append to
     * @param queryStrKey the query string key
     * @param value the query string value to append
     * @return URL with query string and CSRF token appeneded
     */
    public static String getUrlWithQueryStrWithCsrf(WizardContext context, String url, String queryStrKey, String value)
    {
        // Get the URL without CSRF token.
        String rootUrl = getUrlWithQueryStr(url, queryStrKey, value);

        // For URL with CSRF token.
        String finalUrl = getUrlWithQueryStrWithCsrf(context, rootUrl);

        return finalUrl;
    }

    /**
     * Attempt to initialse wizard's session CSRF token with query string parameter.
     * @param web The web abstraction.
     * @param receivedToken The token received in the request query string.
     * @return true iff the token was accepted (i.e. it matches that stored by Web Interface if
     * Web Interface shares session state with the wizard, otherwise the token is accepted
     * without validation).
     */
    public static boolean initialiseCsrfToken(WebAbstraction web,
        String receivedToken) {
        // The result.
        boolean accepted;

        // Recall any existing token.
        String token = (String)web.getSessionAttribute(WizardConstants.SESSION_CSRF_TOKEN_NAME);

        // If no existing token...
        if (token == null) {
            // ...store token.
            web.setSessionAttribute(WizardConstants.SESSION_CSRF_TOKEN_NAME_PRIVATE, receivedToken);

            // Stored token is the one we'll use.
            accepted = true;
        }
        else {
            accepted = token.equals(receivedToken);
        }

        return accepted;
    }

    /**
     * Get the session token. If the core appears present, use its value, otherwise use
     * own 'assumed' value.
     * @param web the web abstraction
     * @return the CSRF token, as stored in the session
     */
    public static String getCsrfToken(WebAbstraction web) {
        // Recall token from core Web Interface.
        String token = (String)web.getSessionAttribute(WizardConstants.SESSION_CSRF_TOKEN_NAME);

        // If no Web Interface token...
        if (token == null) {
            // ...use the token the Wizard started with.
            token = (String)web.getSessionAttribute(WizardConstants.SESSION_CSRF_TOKEN_NAME_PRIVATE);
        }

        return token;
    }

    /**
     * Check that current request's query string contains correct CSRF session token.
     * @param web the web abstraction
     * @return <code>true<code> if the token is correct, otherwise <code>false</code>
     */
    public static boolean validateCsrfQueryStr(WebAbstraction web) {
        String sessionToken = getCsrfToken(web);
        String queryToken = web.getQueryStringParameter(WizardConstants.QSTR_TOKENNAME);

        return sessionToken != null && queryToken != null &&
            sessionToken.compareToIgnoreCase(queryToken) == 0;
    }

    /**
     * Returns true if the two objects are both non null and equal.
     * @param o1 an object
     * @param o2 an object
     * @return <code>true</code> if the objects are the same and non-null, <code>false</code> otherwise
     */
    public static boolean isEquals(Object o1, Object o2) {
        boolean result = false;
        if (o1 != null && o2 != null) {
            result = o1.equals(o2);
        }
        return result;
    }

    /**
     * Checks for a non-null empty string
     * @param s the string
     * @return true if the supplied string is non null and not the empty string, otherwise false
     */
    public static boolean isNonEmptyString(String s) {
        return (s != null) && (s.length() > 0);
    }

    /*
     * Determines if rdp class id is supported/valid
     * @param rdpClientClassId the class id to be tested.
     * @return returns true if valid,otherwise false
     */
    public static boolean isValidRdpClassId(String rdpClientClassId) {
        boolean validRdpClassId = false;
        for (int i = 0; i < WizardConstants.VALID_RDP_CLASS_ID.length; i++) {
            if ( WizardConstants.VALID_RDP_CLASS_ID[i].equalsIgnoreCase(rdpClientClassId) ) {
                validRdpClassId = true;
                break;
            }
        }
        return validRdpClassId;
    }

    /**
     * Checks if the masterpage URL is valid.
     * @param masterPage the masterpage URL
     * @return true if valid, otherwise false.
     */
    public static boolean isValidMasterPageUrl(String masterPage) {
        boolean result = false;
        if (Platform.isDotNet()) {
            result = masterPage.endsWith(".master") && (masterPage.startsWith("/") || masterPage.startsWith("~")) ;
        } else if (Platform.isJava()) {
            result = masterPage.endsWith(".jsp") && masterPage.startsWith("/");
        }
        return result;
    }

    /**
     * Determine if client detection is possible on the given platform
     * @param sClientInfo client info
     * @returns true if client detection is possible, false otherwise
     */
    public static boolean isClientDetectablePlatform(ClientInfo sClientInfo) {
        boolean result = false;
        if (sClientInfo != null) {
            result = sClientInfo.osVista() || sClientInfo.osWin32() || sClientInfo.osWin64()
            ||
            isMacClientDetectablePlatform(sClientInfo);
        }
        return result;
    }

    /**
     * Determines whether the mac client is detectable on the given platform.
     * @param sClientInfo client info
     * @returns true if it is possible to detect the mac client on the platform.
     * Note that we do not know the OS minor version from the browser info and the mac
     * client plugin will only run on OSX from version 10.4 so it may be that the plugin
     * can never be installed, but this is our best guess.
     * Mac OSX versions pre 10.4 are no longer supported.
     */
    public static boolean isMacClientDetectablePlatform(ClientInfo sClientInfo) {
        // Once the mac client is available with the browser plugin we can enable this.
        // sClientInfo.osMacOSX() & (sClientInfo.isSafari() || sClientInfo.isFirefox());
        return false;
    }

    /**
     * Determines whether the client operating system is supported by the
     * client detection wizard.
     *
     * @param sClientInfo client info
     * @return <code>true</code> if OS is supported, else <code>false</code>.
     */
    public static boolean isWizardSupportedOS(ClientInfo sClientInfo) {
        boolean result = false;
        if (sClientInfo != null) {

            // Classic Mac OS is not supported by the wizard.
            boolean classicMacOS = sClientInfo.osMac() && !sClientInfo.osMacOSX();

            result = (sClientInfo.getPlatform() != ClientInfo.OS_UNKNOWN)
                    && !(sClientInfo.osSymbian() || sClientInfo.osWinCE() ||
                         sClientInfo.osPocketPC() || classicMacOS);
        }
        return result;
    }

    /**
     * Exits the wizard with an alternate result of "session expired".
     * @parameter webAbstraction the web abstraction
     */
    public static void handleSessionExpired(WebAbstraction webAbstraction) {
        String sessionExpiredUrl = webAbstraction.getConfigurationAttribute(WizardConstants.WIZARD_DEFAULT_REDIRECT_URL);
        sessionExpiredUrl = getUrlWithQueryStr(sessionExpiredUrl, WizardConstants.ALTERNATE_RESULT, WizardConstants.SESSION_EXPIRED);
        webAbstraction.clientRedirectToContextUrl(sessionExpiredUrl);
    }

    /**
     * Checks if a redirect URL is valid
     * @param webAbstraction the web abstraction
     * @param redirectUrl the URL to check
     * @return true if valid, otherwise false
     */
    public static boolean isValidRedirectUrl(WebAbstraction webAbstraction, String redirectUrl) {
        boolean valid = false;
        if (HttpURLChecker.isWellFormedRelative(redirectUrl)) {
            valid = true;
        } else if (HttpURLChecker.isWellFormedAbsolute(redirectUrl)) {
            URL url = null;
            URL wizardUrl = null;
            try {
                url = new URL(redirectUrl);
                wizardUrl = new URL(webAbstraction.getBaseURL());
                valid = url.getHost().equalsIgnoreCase(wizardUrl.getHost());
            } catch (MalformedURLException e) { }
        }
        return valid;
    }

    /**
     * Determines if streaming client installation is complete.
     * @param webAbstraction the web abstraction
     * @param inputs the wizard inputs model
     * @param sClientInfo client info
     * @return true if complete, otherwise false
     */
    public static boolean isRadeInstallationIncomplete(WebAbstraction webAbstraction, WizardInput inputs, ClientInfo sClientInfo) {
        WebCookie persistentCookie = webAbstraction.getCookie(WizardConstants.COOKIE_WI_USER);
        String radeRestartCookie = getValueFromWizardCookie(persistentCookie, WizardConstants.COOKIE_RADE_STARTED);

        return (inputs.getMode() == Mode.AUTO && inputs.detectStreamingClient() &&
               WizardUtil.isEquals(radeRestartCookie, WizardConstants.VAL_TRUE) &&
               sClientInfo.isIE() && (sClientInfo.osWin32() || sClientInfo.osWin64()));
    }

    /**
     * Returns true if polling for client detection is supported on the browser
     * and OS represented by the supplied ClientInfo object
     * @param clientInfo - the ClientInfo object
     * @return true if polling client detection is supported, false otherwise
     */
    public static boolean isClientDetectionPollingSupported(ClientInfo clientInfo) {
        // Currently polling is supported on IE on Windows, Mozilla Based browsers
        // and Safari on Mac OSX
        boolean isWindows = clientInfo.osWin32() || clientInfo.osWin64();
        boolean isIEOnWindows = isWindows && clientInfo.isIE();
        boolean isMozillaBased = clientInfo.isMozillaBased();
        boolean isSafariOnMacOSX = clientInfo.osMacOSX() && clientInfo.isSafari();
        return (isIEOnWindows || isMozillaBased || isSafariOnMacOSX);
    }

    /**
     * Returns the ICA client version from the web server clients directory
     * on the web server, or that override by the configuration.
     *
     * @param wizardContext - the current WizardContext object.
     * @return String representing the version in the form "ddd.ddd.ddd...." or "" if no client is found.
     */
    public static String getServerClientVersion(WizardContext wizardContext) {
        return getClientVersionNumber(wizardContext, wizardContext.getClientInfo().getClientPlatform());
    }

    /**
     * Returns the Streaming client version from the web server clients directory
     * on the web server, or that override by the configuration
     *
     * @param wizardContext - the current WizardContext object.
     * @return String representing the version in the form "ddd.ddd.ddd...." or "" if no client is found.
     */
    public static String getServerStreamingClientVersion(WizardContext wizardContext) {
        return getClientVersionNumber(wizardContext, ClientPlatform.STREAMING_WIN32);
    }

    /**
     * Helper for getting a client version number using the supplied wizard context and client platform.
     *
     * @param wizardContext the wizard context object.
     * @param clientPlatform the client platform.
     * @return the client version number string.
     */
    public static String getClientVersionNumber(WizardContext wizardContext, ClientPlatform clientPlatform) {
        return getClientVersionNumber(wizardContext.getClientManager(), clientPlatform, wizardContext.getInputs().getLocale());
    }

    /**
     * Helper for getting a client version number using the supplied client manager, client platform and locale.
     *
     * @param clientManager the client manager.
     * @param clientPlatofrm the client platform.
     * @param locale the locale to use for finding the client package.
     * @return the client version number string.
     */
    public static String getClientVersionNumber(ClientManager clientManager, ClientPlatform clientPlatform, Locale locale) {
        ClientPackage clientPackage = clientManager.getClientPackage(clientPlatform, locale);
        VersionNumber version = clientPackage.getVersion();
        if (version != null) {
            return version.toString();
        } else {
            return "";
        }
    }

    /**
     * Returns the ClientDeploymentConfiguration for the wizard
     * @param wizardContext - the WizardContext
     * @return the ClientDeploymentConfiguration
     */
    public static ClientDeploymentConfiguration getClientDeploymentConfiguration(WizardContext wizardContext) {
        return getClientDeploymentConfiguration(wizardContext.getWebAbstraction());
    }

    /**
     * Returns the ClientDeploymentConfiguration for the wizard
     * @param web - the WebAbstraction
     * @return the ClientDeploymentConfiguration
     */
    public static ClientDeploymentConfiguration getClientDeploymentConfiguration(WebAbstraction web) {
        return (ClientDeploymentConfiguration)web.getApplicationAttribute(WizardConstants.APP_ATTRIBUTE_CLIENT_DEPLOYMENT_CONFIGURATION);
    }

    /**
     * Get the Client Licence Agreement mark-up
     *
     * @param wizardContext
     * @return the html to show the licence agreement
     */
    public static String getClientLicenseAgreement(WizardContext wizardContext) {
        StringWriter writer = new StringWriter();
        writer.write("<p><strong>");
        writer.write(wizardContext.getString("LicenceTextTitle"));
        writer.write("</strong></p><p>");
        writer.write(wizardContext.getString("LicenceTextBody"));
        writer.write("</p><p>");
        writer.write(wizardContext.getString("LicenceTextCode"));
        writer.write("</p>");
        return writer.toString();
    }

    /**
     * Determines whether to display the ICA Client license agreement.
     *
     * @param wizardContext a <code>WizardContext</code> object
     * @return <code>true</code> if the license agreement should be displayed, otherwise <code>false</code>
     */
    public static boolean showClientLicenseAgreement(WizardContext wizardContext) {
        // Only show the license agreement if:
        // - either the client is on the disk, or there is a custom url to point
        // to the file
        // - the client is configured to show a eula
        boolean showEula = false;
        ClientPackageConfig config = getClientPackageConfig(wizardContext, wizardContext.getClientInfo().getClientPlatform());
        String downloadUrl = getClientDownloadUrl(wizardContext);
        if (config != null) {
            showEula = config.isShowEULA() && !Strings.isEmpty(downloadUrl);
        }
        return showEula;
    }

    /**
     * Sets up the Skip, Logout, Try Alternative and
     * return to client selection links
     *
     * @param wizardContext
     * @param model
     */
    public static void setupCommonWizardLinks(WizardContext wizardContext, WizardViewModel model) {
        // hide all but return to client selection in advanced mode
        if (wizardContext.getInputs().getMode() == Mode.ADVANCED) {
            model.showSkipLink = false;
            model.showLogoutLink = false;
            model.showReturnToClientSelectionLink = true;
        } else {
            model.showSkipLink = true;
            model.showLogoutLink = wizardContext.getInputs().allowLogout();
            model.showReturnToClientSelectionLink = false;
        }
    }

    /**
     * Get the markup for the help steps for the given section
     *
     * @param wizardContext
     * @param help
     * @param sectionNumber
     * @return the markup for that section
     */
    public static String getHelpStepsMarkup(WizardContext wizardContext, WizardHelp help, Integer sectionNumber) {
        StringWriter writer = new StringWriter();

        Iterator steps = help.getStepNumbers(sectionNumber);
        while(steps.hasNext()){
            java.lang.Integer stepNo = (java.lang.Integer) steps.next();
            String text = (String) help.getStepText(sectionNumber, stepNo);
            String imageUrl = (String) help.getStepImage(sectionNumber, stepNo);
            String imageCaption = (String) help.getStepImageCaption(sectionNumber, stepNo);

            writer.write("<div class=\"HelpStepNo\">");
            writer.write(stepNo.toString());
            writer.write(".</div>");

            if (text != null) {
                writer.write("<div class=\"HelpStepText\">");
                writer.write(text);
                writer.write("</div>");
            }

            if (imageUrl != null) {
                writer.write("<img class=\"HelpImage\" src=\"");
                writer.write(wizardContext.getMui().getHelpImageUrl(imageUrl, wizardContext.getInputs().getLocale()));
                writer.write("\" alt=\"");
                writer.write(imageCaption);
                writer.write("\">");
            }
        }

        return writer.toString();
    }

    /**
     * Gets the markup for the help in the center
     * of wizard pages such as ChangeZone and PopupHelp
     *
     * @param wizardContext
     * @param helpid the id of the help that needs to be fetched
     * @param idIfHelp the id of the string for the preamble if the help is available
     * @param idIfNoHelp the id of the string for the preamble if the help is not available
     * @return the help that was requested, with the appropriate preamble
     * dependent on if the help is available.
     */
    public static String getCentralHelpMarkup(WizardContext wizardContext, String helpID, String idIfHelp, String idIfNoHelp) {
        StringWriter writer = new StringWriter();

        WizardHelp help = wizardContext.getWizardHelp(helpID);
        if (help != null) {
            writer.write("<p>");
            writer.write(wizardContext.getString(idIfHelp));
            writer.write("</p>");
            writer.write("<div id=\"HelpContent\">");
            Iterator sections = help.getSectionNumbers();
            java.lang.Integer sectionNo = (java.lang.Integer) sections.next(); // only want the first one
            writer.write(WizardUtil.getHelpStepsMarkup(wizardContext, help, sectionNo));
            writer.write("</div>");
        } else {
            writer.write("<p>");
            writer.write(wizardContext.getString(idIfNoHelp));
            writer.write("</p>");
        }

        return writer.toString();
    }


    /**
     * Creates a link in a p tag that is used to
     * download the client instantly
     *
     * @param wizardContext
     * @param downlaodUrl the URL for the file to download
     * @return
     */
    public static String getProblemDownloadingLinkMarkup(WizardContext wizardContext, String downloadUrl) {
        return getProblemDownloadingLinkMarkup(wizardContext, downloadUrl, NO_TABINDEX);
    }
    /**
     * Creates a link in a p tag that is used to
     * download the client instantly
     *
     * @param wizardContext
     * @param downlaodUrl the URL for the file to download
     * @return
     */
    public static String getProblemDownloadingLinkMarkup(WizardContext wizardContext, String downloadUrl, int tabIndex) {
        // only include the dowloadurl if it is not empty
        String downloadUrlMarkup;
        if(Strings.isEmpty(downloadUrl)) {
            downloadUrlMarkup = "";
        } else {
            downloadUrlMarkup = "'" + WebUtilities.escapeJavascript(downloadUrl) + "'";
        }

        StringWriter writer = new StringWriter();
        writer.write("<p id=\"DownloadAgainLink\">");
        String linkAttributes = "href=\"#\" onclick=\"downloadClientNow(" + downloadUrlMarkup + "); return false;\"";
        if (tabIndex != NO_TABINDEX) {
            linkAttributes = linkAttributes + " tabindex=\"" + tabIndex + "\"";
        }
        writer.write(wizardContext.getString("TryDownloadAgain", linkAttributes));
        writer.write("</p>");
        return writer.toString();
    }
}
