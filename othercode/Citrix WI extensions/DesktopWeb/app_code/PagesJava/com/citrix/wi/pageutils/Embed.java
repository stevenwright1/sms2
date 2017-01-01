/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils;

import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.types.MPSClientType;
import com.citrix.wing.types.AppDisplaySize;
import com.citrix.wing.types.ClientType;
import com.citrix.wing.webpn.ApplicationInfo;
import com.citrix.wi.types.AppDisplaySizePreference;

/**
 * This class has some utility methods to deal with embeded launches
 */
public class Embed {

    public static final String ICA_JAVACLIENT = "ICA-JavaClient";
    public static final String RDP_ACTIVEX    = "RDP-ActiveX";
    public static final String ICA_ICOCLIENT  = "ICA-ICOClient";

    // Used by AppEmbed to signify a Streaming Launch
    // but is not used by the methods below
    public static final String RADE_RCOCLIENT = "rade-rcoclient";

    /**
     * Function to determine the client required Returns "" if no scripted
     * launch client could be found
     *
     * @param wiContext request data object
     * @return "ICA-ICOClient", "ICA-JavaClient", "RDP-ActiveX" or ""
     */
    public static String getScriptedHostedAppLaunchClient(WIContext wiContext) {
        MPSClientType selectedClient = Include.getSelectedRemoteClient(wiContext);

        if (selectedClient == MPSClientType.JAVA) {
            return ICA_JAVACLIENT;
        } else if (selectedClient == MPSClientType.EMBEDDED_RDP) {
            return RDP_ACTIVEX;
        } else if (selectedClient == MPSClientType.LOCAL_ICA && Include.doIcaLaunchViaScripting(wiContext)) {
            return ICA_ICOCLIENT;
        } else {
            return "";
        }
    }

    /**
     * Checks whether hosted applications should be launched
     * using JavaScript rather than a file download
     *
     * @return true if hosted apps will be launched using JavaScript
     */
    public static boolean isScriptedHostedAppLaunch(WIContext wiContext) {
        return !getScriptedHostedAppLaunchClient(wiContext).equals("");
    }

    /**
     * Checks whether streaming applications should be launched
     * using JavaScript rather than a file download
     *
     * @return true if streaming apps will be launched using JavaScript
     */
    public static boolean isScriptedStreamingAppLaunch(WIContext wiContext) {
        boolean useRCO = wiContext.getClientInfo().isIE();
        return useRCO;
    }

    /**
     * Convert the Embed style client string representation into one that WING
     * launch understands
     *
     * @param clientStr the Embed style client string
     * @return the Wing ClientType for the given string
     */
    public static ClientType toWingClientType(String clientStr) {
        ClientType client = ClientType.ICA_30;
        if (RDP_ACTIVEX.equals(clientStr)) {
            client = ClientType.RDP;
        } else if (RADE_RCOCLIENT.equals(clientStr)) {
            client = ClientType.RADE;
        }
        return client;
    }

    /**
     * This class encapsulated the desired horizontal and vertical dimensions
     */
    public static class WindowDimensions {

        private String desiredHRES = null;
        private String desiredVRES = null;

        public WindowDimensions(WIContext wiContext, ApplicationInfo res, int screenHRES, int screenVRES) {

            AppDisplaySize displaySize = null;
            if (wiContext.getConfiguration().getAllowCustomizeSettings()) {
                AppDisplaySizePreference displaySizePref = wiContext.getUserPreferences().getAppDisplaySizePreference();
                if (displaySizePref != null) {
                    displaySize = displaySizePref.toAppDisplaySize();
                }
            }

            if (displaySize == null && res != null) {
                displaySize = res.getDisplaySize();
            }

            desiredHRES = Constants.DEFAULT_ICA_WINDOW_HRES;
            desiredVRES = Constants.DEFAULT_ICA_WINDOW_VRES;
            if (displaySize != null && screenHRES != -1 && screenVRES != -1) {
                desiredHRES = "" + displaySize.calcPixelWidth(screenHRES);
                desiredVRES = "" + displaySize.calcPixelHeight(screenVRES);
            }
        }

        public String getHRES() {
            return desiredHRES;
        }

        public String getVRES() {
            return desiredVRES;
        }
    }
}
