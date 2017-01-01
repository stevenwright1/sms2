/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils.clientdetection;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;

import com.citrix.wi.clientdetect.Client;
import com.citrix.wi.clientdetect.ClientDetectionWizardState;
import com.citrix.wi.clientdetect.Mode;
import com.citrix.wi.clientdetect.type.ClientType;
import com.citrix.wi.config.AppAccessMethodConfiguration;
import com.citrix.wi.config.ClientDeploymentConfiguration;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.AGEUtilities;
import com.citrix.wi.pageutils.ClientUtils;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.types.InstallCaptionState;
import com.citrix.wi.types.JavaFallbackMode;
import com.citrix.wi.types.MPSClientType;
import com.citrix.wing.types.AppAccessMethod;
import com.citrix.wing.webpn.AccessMethod;
import com.citrix.wing.webpn.ResourceInfo;

/**
 * Helper routines for the Client Detection Wizard.
 */
public class DetectionUtils {
    private DetectionUtils() {
    }

    /*
     * If interactive RADE client detection has started but not completed due to termination (for example re-boot)
     * then we need to continue rade detection.
     *
     * We use a persistent cookie to achieve this.
     */

    private static final String COOKIE_RADE_DETECTION_START = "CTX_RadeDetectionStart";

    /**
     * Sets a cookie to indicate that interactive RADE detection
     * is in progress.
     */
    public static void setRadeDetectionStartCookie(WIContext wiContext, ClientDetectionWizardState sWizardInfo) {
        // RADE detection is only ever run interactively if we are in AUTO mode and
        // RADE is the only client to be detected.
        if((sWizardInfo.getMode()==Mode.AUTO) && sWizardInfo.detectRadeOnly()) {
            wiContext.getUserEnvironmentAdaptor().getUserState().put(COOKIE_RADE_DETECTION_START, Constants.VAL_TRUE);
        }
    }

    /**
     * Clear the cookie indicating that interactive RADE
     * detection is in progress.
     */
    public static void clearRadeDetectionStartCookie(WIContext wiContext, ClientDetectionWizardState sWizardInfo) {
        Mode mode = sWizardInfo.getMode();
        if(sWizardInfo.detectRadeOnly()) {
            if(
                (mode==Mode.AUTO) // RADE client detection is interactive in this case
                ||
                ((mode == Mode.SILENT) && sWizardInfo.hasStreamingClientResult()) // streaming client available
              ) {
                wiContext.getUserEnvironmentAdaptor().getUserState().remove(COOKIE_RADE_DETECTION_START);
            }
        }
    }

    /**
     * Finds out if interactive RADE detection is in progress by reading the cookie and
     * verifying streaming is possible for the client.
     */
    public static boolean getRadeDetectionStartCookie(WIContext wiContext) {
        return getStreamingAccessPresent(wiContext) &&
            Constants.VAL_TRUE.equals(wiContext.getUserEnvironmentAdaptor().getUserState().get(COOKIE_RADE_DETECTION_START));
    }

    /*
     * Utils to determine if the site is effectively remote, streaming or dual.
     */

    /**
     * Get value that represents if access method of both streaming and remote is possible
     * @return <code>boolean</code> true or false
     */
    public static boolean getDualAccessMethod(WIContext wiContext) {
        return getStreamingAccessPresent(wiContext) && getRemoteAccessPresent(wiContext);
    }

    /**
     * Get value that represents if an access method of streaming is possible.
     * @return <code>boolean</code> true or false
     */
    public static boolean getStreamingAccessPresent(WIContext wiContext) {
        return wiContext.getConfiguration().getAppAccessMethodConfiguration().isEnabledAppAccessMethod(AppAccessMethod.STREAMING)
               && Include.getOsRadeCapable(wiContext) && Include.getBrowserRadePluginCapable(wiContext);
    }

    /**
     * Get value that represents if an access method of remote is possible.
     * @return <code>boolean</code> true or false
     */
    public static boolean getRemoteAccessPresent(WIContext wiContext) {
        return wiContext.getConfiguration().getAppAccessMethodConfiguration().isEnabledAppAccessMethod(AppAccessMethod.REMOTE);
    }

    /*
     * Other helper methods
     */

    /**
     * Gets the first remote client given by enabledRemoteClients or null if
     * no clients are available.
     */
    public static ClientType getPreferredRemoteClientType(WIContext wiContext) {
        ClientType result = null;
        List clients = DetectionUtils.createRemoteClientList(wiContext, null);
        if (!clients.isEmpty()) {
            result = ClientType.getClientTypeFromString(clients.get(0).toString());
        }

        return result;
    }

    /**
     * Create a list of all permissible streaming clients to pass to the wizard
     * for detection. The list consists of <code>ClientType</code> objects.
     * @param mode - the wizard mode, or null if the mode should not be used to restrict
     * the streaming client list.
     */
    public static List createStreamingClientList(WIContext wiContext, Mode mode) {
        List clients = new ArrayList();
        if (
            DetectionUtils.getStreamingAccessPresent(wiContext) // make sure the config and platform allow streaming
            && (mode != Mode.ADVANCED) // advanced mode is only for remote clients
          ) {
            clients.add(ClientType.RADE);
        }

        return clients;
    }

    /**
     * This list is from the clients that are configured
     * for use by the Administrator, and removing those that are not supported.
     * If Java fallback is one, Java is not included in this list, it will only be
     * included if it is configured by the administrator as an available client
     *
     * @param wiContext
     * @return the list of available clients
     */
    public static List getAvailableRemoteClients(WIContext wiContext) {
        ClientDeploymentConfiguration cdConfig = wiContext.getConfiguration().getClientDeploymentConfiguration();
        List result = new ArrayList(cdConfig.getEnabledClients()); // make copy so we don't change the config
        if (!ClientUtils.isJavaClientSupported(wiContext)) {
            result.remove(MPSClientType.JAVA);
        }
        if (!ClientUtils.isRDPClientSupported(wiContext)) {
            result.remove(MPSClientType.EMBEDDED_RDP);
        }
        return result;
    }

    /**
     * Create a <code>List</code> of remote clients
     * to pass to the wizard.
     * The list is in preference order (which is determined by the order
     * the clients are specified in the site's configuration).
     * For Silent and Auto mode it is generally only the preferred client,
     * and advanced mode includes all the clients.
     * @param mode - the wizard mode, or null if the mode should not be used to restrict
     * the remote client list.
     */
    public static List createRemoteClientList(WIContext wiContext, Mode mode) {

        // Get the allowed list of clients
        List mpsClients;
        if (wiContext.getConfiguration().getAppAccessMethodConfiguration().isEnabledAppAccessMethod(AppAccessMethod.REMOTE)) {
            if (Boolean.TRUE.equals(wiContext.getUserPreferences().getForcedClient())
                && (mode == Mode.SILENT)) {
                // If client is forced, we only want to consider that client in the silent detection process,
                // (If Advanced or Auto mode, all possible clients need to be
                // detected and displayed for the user to choose between).
                mpsClients = new ArrayList();
                mpsClients.add(Include.getSelectedRemoteClient(wiContext));
            } else {
                // get the list of clients that are enabled
                mpsClients = getAvailableRemoteClients(wiContext);
            }
        } else {
            mpsClients = new ArrayList();
        }

        // and it needs to be a ClientType object
        List clients = new ArrayList();
        Iterator it = mpsClients.iterator();

        if (mode == Mode.ADVANCED) {
            // pass in all the clients for advanced mode
            while(it.hasNext()) {
                clients.add(ClientType.getClientTypeFromMPSClientType((MPSClientType)it.next()));
            }
        } else {
            // only normally pass the first in the list
            // when not advanced mode
            if (it.hasNext()) {
                clients.add(ClientType.getClientTypeFromMPSClientType((MPSClientType)it.next()));
            }
        }

        return clients;
    }

    /**
     * Check whether there are any configured clients available to the user.
     * e.g. RDP can't be used with Firefox but it could be the only option
     * enabled in WI config.
     * @return true if at least one client is available, otherwise false
     */
    public static boolean clientsAvailableForDetection(WIContext wiContext, Mode mode) {
        return (createRemoteClientList(wiContext, mode).size()
            + createStreamingClientList(wiContext, mode).size())
            > 0;
    }

    /**
     * Gets whether the user is known to have a client that can launch the resource,
     * based on the access methods of the resource and the client wizard findings.
     *
     * @param wiContext current WIContext for launch attempt
     * @param resource the Resource we're trying launch
     * @return true if we think the user has a client.
     */
    public static boolean hasClientForLaunch( WIContext wiContext, ResourceInfo resource ) {
        ClientDetectionWizardState wizard = Include.getWizardState( wiContext );
        boolean remoteOk = resource.isAccessMethodAvailable(AccessMethod.DISPLAY) &&
                           wizard.hasRemoteClientResult();
        boolean streamOk = resource.isAccessMethodAvailable(AccessMethod.STREAM) &&
                           wizard.hasStreamingClientResult();
        return remoteOk || streamOk;
    }

    /**
     * Gets whether any allowed remote client is known to be installed, based on the client wizard findings.
     * Useful method for desktop launches because they can only use the remote client.
     *
     * @param wiContext current WIContext for launch attempt
     * @return true if we think the user has a client.
     */
    public static boolean hasClientForRemoteLaunch( WIContext wiContext ) {
        return Include.getWizardState( wiContext ).hasRemoteClientResult();
    }

    /**
     * The Problem connecting and direct launch help links,
     * should be hidden when the captions are turned off.
     *
     * @return true if the wizard help links should be hidden
     */
    public static boolean hideWizardHelpLinks(WIContext wiContext) {
        return wiContext.getConfiguration().getClientDeploymentConfiguration().getShowInstallCaption()
            == InstallCaptionState.OFF
            || !Include.clientWizardSupported(wiContext);
    }


    /**
     * Check if a Java fallback mode is enabled
     * Checks the administrator choice, and if the Java client
     * is supported.
     *
     * @param mode
     * @param wiContext
     * @return true if the mode is enabled
     */
    public static boolean isJavaFallbackModeEnabled(WIContext wiContext, JavaFallbackMode mode) {
        ClientType preferredRemoteClient = DetectionUtils.getPreferredRemoteClientType(wiContext);
        JavaFallbackMode javaFallback = wiContext.getConfiguration().getClientDeploymentConfiguration().getJavaFallbackMode();
        return javaFallback != null && javaFallback.equals(mode)
                && ClientUtils.isJavaClientSupported(wiContext)
                && ClientType.NATIVE.equals(preferredRemoteClient);
    }

    /**
     * Use the results from the Client Wizard, and the Admin settings,
     * to decide if the user should be given an upgrade for the
     * Streaming Client
     *
     * @param wiContext
     * @return true if the user should be given the upgrade
     */
    public static boolean promptForStreamingUpgrade(WIContext wiContext) {
        return Include.getWizardState(wiContext).getStreamingCanUpgrade()
                && wiContext.getConfiguration().getClientDeploymentConfiguration().isUpgradeClientsAtLogin();
    }

    /**
     * Use the results from the Client Wizard, and the Admin settings,
     * to decide if the user should be given an upgrade for the
     * Local ICA Client
     *
     * @param wiContext
     * @return true if the user should be given the upgrade
     */
    public static boolean promptForLocalIcaUpgrade(WIContext wiContext) {
        return Include.getWizardState(wiContext).getRemoteCanUpgrade()
                && wiContext.getConfiguration().getClientDeploymentConfiguration().isUpgradeClientsAtLogin();
    }

    /**
     * Check that the stored wizard result respects
     * the current configuration.
     *
     * @param wiContext
     * @return
     */
    public static boolean isValidClientDetectionResult(WIContext wiContext) {
        boolean wizardResultValid = true;

        // Streaming sites can only ever have the streaming client
        // so the result is always valid
        // For Remote and Dual sites, we need to check the remote
        // client is the correct one
        AppAccessMethodConfiguration aamConfig = wiContext.getConfiguration().getAppAccessMethodConfiguration();
        if (aamConfig.isEnabledAppAccessMethod(AppAccessMethod.REMOTE)) {
            // check the remote result respects the configuration
            ClientDetectionWizardState sWizardInfo = Include.getWizardState(wiContext);
            Client remoteclient = sWizardInfo.getRemoteClientResult();
            if (remoteclient == null) {
                // no result, is always a valid result
                // although it may not be up to date
                wizardResultValid = true;
            } else {
                // find the list of all possible clients
                List validClients = DetectionUtils.createRemoteClientList(wiContext, Mode.ADVANCED);
                if (DetectionUtils.isJavaFallbackModeEnabled(wiContext, JavaFallbackMode.MANUAL)
                                || DetectionUtils.isJavaFallbackModeEnabled(wiContext, JavaFallbackMode.AUTO)) {
                    validClients.add(ClientType.JAVA);
                }

                // see if the remote client the wizard has selected is in this list
                ClientType wizardClientType = remoteclient.getClientType();
                wizardResultValid = validClients.contains(wizardClientType);
            }
        }

        return wizardResultValid;
    }
}
