/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils;

import java.util.Map;

import com.citrix.wi.config.auth.ADFSAuthPoint;
import com.citrix.wi.config.auth.AuthPoint;
import com.citrix.wi.config.auth.WebServerAuthPoint;
import com.citrix.wi.localization.ClientManager;
import com.citrix.wi.localization.ClientPackage;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.types.ClientPlatform;
import com.citrix.wi.util.ClientInfo;
import com.citrix.wing.UserEnvironmentAdaptor;
import com.citrix.wing.config.ConnectionRoute;
import com.citrix.wing.config.ConnectionRoutingPolicy;
import com.citrix.wing.config.SGConnectionRoute;

/**
 * This deals with the Utility methods related to Client Detection
 */
public class ClientUtils {

    private static String[] transferItems = new String[] {
                    Constants.COOKIE_ICA_SCREEN_RESOLUTION,
                    Constants.COOKIE_CLIENT_CONN_SECURE };

    private static String[] desiredItems = new String[] {
                    Constants.COOKIE_ICA_SCREEN_RESOLUTION };

    /**
     * Checks whether we should carry out client detection. Return true if
     * client detection is needed; false otherwise.
     */
    public static boolean clientDetectionRequired(UserEnvironmentAdaptor envAdaptor) {
        Map clientSessionState = envAdaptor.getClientSessionState();

        // Do not carry out client detection if we already have all the
        // information.
        for (int i = 0; i < desiredItems.length; i++) {
            String v = (String)clientSessionState.get(desiredItems[i]);
            if (v == null || "".equals(v)) {
                return true;
            }
        }

        // All information is present
        return false;
    }

    /**
     * Transfers the content of the client information cookie (set in
     * JavaScript) into the client session state. This method was introduced for
     * better compatibility with reverse/web proxies. It is necessary for the
     * client machine to set the client information cookie using JavaScript -
     * however, if the JavaScript cookie specifies a path then this may not get
     * rewritten when the site is being viewed through a web proxy. The solution
     * is to store the JavaScript cookie without a path and then intercept it on
     * the site login page and transfer its contents to the client session
     * state. The client session state has a path which can be rewritten
     * correctly by the web proxy.
     *
     * This function may be called several times per session. The session may change
     * from insecure to secure during the life time of a session (http: promoting to
     * https: retains existing cookies, https: to http: does not reveal secure cookies).
     *
     * @param clientInfoCookie <code>Map</code> containing the contents of the
     * client information cookie.
     */
    public static void transferClientInformationCookie(WebAbstraction web, UserEnvironmentAdaptor envAdaptor) {
        Map clientInfoCookie = Include.getClientInfoCookie(web);
        Map clientSessionState = envAdaptor.getClientSessionState();

        // Only transfer selected items to the client session state
        // Not all items are used by server-side scripts
        for (int i = 0; i < transferItems.length; i++) {
            String k = transferItems[i];
            String v = (String)clientInfoCookie.get(k);

            if ((v != null) && (!v.equals(""))) {
                clientSessionState.put(k, v);
            }
        }
    }

    /**
     * Tests whether the Java client is supported.
     */
    public static boolean isJavaClientSupported(WIContext wiContext) {
        AuthPoint authPoint = wiContext.getConfiguration().getAuthenticationConfiguration().getAuthPoint();
        ClientInfo clientInfo = wiContext.getClientInfo();

        // Supported if the client OS is not WinCE or Symbian
        boolean supported = !clientInfo.osWinCE() && !clientInfo.osSymbian();
        // If the Java client is available on the server
        supported &= isJavaClientAvailableOnServer(wiContext);
        // Supported if not using adfs
        supported &= !(authPoint instanceof ADFSAuthPoint);

        return supported;
    }

    /**
     * Tests whether the RDP client is supported.
     */
    public static boolean isRDPClientSupported(WIContext wiContext) {
        AuthPoint authPoint = wiContext.getConfiguration().getAuthenticationConfiguration().getAuthPoint();
        ClientInfo clientInfo = wiContext.getClientInfo();

        // Supported if the client OS is Win32 or Win64 and the browser is IE
        boolean supported = (clientInfo.osWin32() || clientInfo.osWin64()) && clientInfo.isIE();
        // Not supported if authentication is federated or handled by the web server (Adv Kerberos)
        supported &= !(authPoint instanceof WebServerAuthPoint) && !authPoint.isFederated();
        // Not supported if a secure gateway is being used
        supported &= !isUsingSecureGateway(wiContext);
        // RDP client does not support passthrough to server, but smartcard to server is supported
        // (see UserContextImpl - convertToRDPClientParams)
        supported &= !Authentication.isPassthroughUser(wiContext.getUserEnvironmentAdaptor());

        return supported;
    }

    /**
     * Determines if java client is available on server @return true if client
     * is available otherwise false
     */
    private static boolean isJavaClientAvailableOnServer(WIContext wiContext) {
        ClientManager clientManager = Include.getClientManager(wiContext.getStaticEnvironmentAdaptor());
        ClientPackage clientPackage = clientManager.getClientPackage(ClientPlatform.JAVA, wiContext.getCurrentLocale());
        return clientPackage.isOnServer();
    }

    /**
     * Determines if the client is connecting via Secure Gateway @return true if
     * connecting through Secure Gateway
     */
    private static boolean isUsingSecureGateway(WIContext wiContext) {
        ConnectionRoutingPolicy policy = wiContext.getConfiguration().getDMZRoutingPolicy();
        String clientAddress = Include.getClientAddress(wiContext);
        ConnectionRoute route = policy.evaluate(clientAddress);
        boolean isSgRoute = (route instanceof SGConnectionRoute);
        return isSgRoute;
    }

}
