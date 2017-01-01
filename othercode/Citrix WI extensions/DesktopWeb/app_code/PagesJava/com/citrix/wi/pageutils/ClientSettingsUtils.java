/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils;

import java.util.HashSet;
import java.util.List;
import java.util.Set;

import com.citrix.wi.UserPreferences;
import com.citrix.wi.clientdetect.Mode;
import com.citrix.wi.controls.ClientSettingsControl;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pageutils.clientdetection.DetectionUtils;
import com.citrix.wi.types.JavaPackage;
import com.citrix.wi.types.MPSClientType;
import com.citrix.wing.util.Strings;
import com.citrix.wing.webpn.UserContext;

/**
 * Utilities for client settings (used by Connection Preferences and
 * Login Preferences pages).
 *
 * The general strategy is:
 * - taking into account, admin settings, platform etc, determine which settings are possible
 *   (eg is streaming possible? is remote access via the java client possible?)
 * - populate the view control so that items which are possible will have corresponding HTML
 * - let the Javascript hide/show elements as the user changes the values of dropdowns
 * - let the Javascript hide/show elements at page load time
 */
public class ClientSettingsUtils {

    private WIContext wiContext;
    private WebAbstraction web;

    // The following bCustomizeXXX flags take into account all the admin settings, the
    // platform, the browser etc.
    private boolean bCustomizeClients;
    private boolean bCustomizeJICAPackages;

    private MPSClientType remoteClientType; // the client which would be used if remote access was preferred
    private boolean autoClient; // Set to "true" if the user chooses "Auto" for the client

    // All the clients the user could select, taking admin settings, browser etc into account
    private List possibleRemoteClients;

    // Java packages
    private boolean bJicaAudio;
    private boolean bJicaCDM;
    private boolean bJicaClipboard;
    private boolean bJicaConfigUI;
    private boolean bJicaPrinter;
    private boolean bJicaZero;

    // Wizard link (advanced mode)
    private boolean bShowAdvancedModeWizardLink = false;

    // Wizard button (auto mode)
    private boolean bShowAutoModeWizardLink = false;

    // Flag used to enforce correct calling sequence of methods
    private boolean bProcessPostDone = false;

    public ClientSettingsUtils(WIContext wiContext) {
        this.wiContext = wiContext;
        web = wiContext.getWebAbstraction();

        // Client type
        remoteClientType = Include.getSelectedRemoteClient(wiContext);

        possibleRemoteClients = DetectionUtils.getAvailableRemoteClients(wiContext);
        bCustomizeClients = Include.getAllowUserToCustomizeClient(wiContext);

        // Java packages
        // only show if the wizard is not supported (i.e. we have a client dropdown)
        // or they have selected the Java client
        bCustomizeJICAPackages = Include.getAllowUserToCustomizeJavaPackages(wiContext)
            && (MPSClientType.JAVA.equals(Include.getSelectedRemoteClient(wiContext)) || !Include.clientWizardSupported(wiContext));
        Set jicaPackages = wiContext.getUserPreferences().getJavaClientPackages();
        if (jicaPackages != null) {
            bJicaAudio = jicaPackages.contains(JavaPackage.AUDIO);
            bJicaCDM = jicaPackages.contains(JavaPackage.CLIENT_DRIVE_MAPPING);
            bJicaClipboard = jicaPackages.contains(JavaPackage.CLIPBOARD);
            bJicaConfigUI = jicaPackages.contains(JavaPackage.CONFIG_UI);
            bJicaPrinter = jicaPackages.contains(JavaPackage.PRINTER_MAPPING);
            bJicaZero = jicaPackages.contains(JavaPackage.ZERO_LATENCY);
        }

        // Advanced mode wizard link
        bShowAdvancedModeWizardLink = Include.isWizardModeSupported(wiContext, Mode.ADVANCED)
            && DetectionUtils.clientsAvailableForDetection(wiContext, null);

        // Auto mode wizard link
        bShowAutoModeWizardLink = !bShowAdvancedModeWizardLink
            && DetectionUtils.clientsAvailableForDetection(wiContext, null)
            && Include.isWizardModeSupported(wiContext, Mode.AUTO)
            && Include.isLoggedIn(web);
    }

    /**
     * Determine if the client settings section(s) have anything to show.
     */
    public boolean anyCustomizableSettings() {
        return bCustomizeClients
            || bCustomizeJICAPackages
            || bShowAdvancedModeWizardLink
            || bShowAutoModeWizardLink;
    }

    /**
     * HTTP POST processing - populates this object with form parameters.
     */
    public void processPostRequest() {
        // Client type or "Auto"
        String clientString = web.getFormParameter(Constants.ID_OPTION_CLIENT_TYPE);
        remoteClientType = MPSClientType.fromString(clientString);
        autoClient = Strings.equalsIgnoreCase(Constants.VAL_CLIENT_TYPE_AUTO, clientString);


        bJicaAudio = Strings.equalsIgnoreCase(Constants.VAL_ON,
            web.getFormParameter(Constants.ID_CHECK_JICA_AUDIO));
        bJicaCDM = Strings.equalsIgnoreCase(Constants.VAL_ON,
            web.getFormParameter(Constants.ID_CHECK_JICA_CDM));
        bJicaClipboard = Strings.equalsIgnoreCase(Constants.VAL_ON,
            web.getFormParameter(Constants.ID_CHECK_JICA_CLIPBOARD));
        bJicaConfigUI = Strings.equalsIgnoreCase(Constants.VAL_ON,
            web.getFormParameter(Constants.ID_CHECK_JICA_CONFIGUI));
        bJicaPrinter = Strings.equalsIgnoreCase(Constants.VAL_ON,
            web.getFormParameter(Constants.ID_CHECK_JICA_PRINTER));
        bJicaZero = Strings.equalsIgnoreCase(Constants.VAL_ON,
            web.getFormParameter(Constants.ID_CHECK_JICA_ZERO));

        bProcessPostDone = true;
    }

    /**
     * Set up the given view control.
     */
    public void setupViewControl(ClientSettingsControl clientSettingsControl) {
        clientSettingsControl.selectedClient = remoteClientType;

        if (bShowAdvancedModeWizardLink) {
            clientSettingsControl.showAdvancedModeWizardLink = true;
        } else if (bShowAutoModeWizardLink) {
            clientSettingsControl.showAutoModeWizardLink = true;
        } else if (bCustomizeClients) {
            // Remote clients
            clientSettingsControl.showClientType = true;
            clientSettingsControl.showICALocal = possibleRemoteClients.contains(MPSClientType.LOCAL_ICA);
            clientSettingsControl.showICAJava = possibleRemoteClients.contains(MPSClientType.JAVA);
            clientSettingsControl.showRDPEmbedded = possibleRemoteClients.contains(MPSClientType.EMBEDDED_RDP);

            String inlineHelp = ""; // localized inline help string
            if (clientSettingsControl.showICALocal) {
                inlineHelp += wiContext.getString("Help_ClientType_Native");
            }
            if (clientSettingsControl.showICAJava) {
                inlineHelp += wiContext.getString("Help_ClientType_Java");
            }
            if (clientSettingsControl.showRDPEmbedded) {
                inlineHelp += wiContext.getString("Help_ClientType_RDP");
            }
            clientSettingsControl.clientTypeInlineHelp = wiContext.getString("Help_ClientType", inlineHelp);
        }

        // Java packages
        if (bCustomizeJICAPackages) {
            clientSettingsControl.showJicaPackages = true;
            clientSettingsControl.jicaAudioSelected = bJicaAudio;
            clientSettingsControl.jicaCDMSelected = bJicaCDM;
            clientSettingsControl.jicaClipboardSelected = bJicaClipboard;
            clientSettingsControl.jicaConfigUISelected = bJicaConfigUI;
            clientSettingsControl.jicaPrinterSelected = bJicaPrinter;
            clientSettingsControl.jicaZeroSelected = bJicaZero;
        }

    }

    /**
     * Use the stored wizard data to find out what the client the wizard automatically selected was.
     * Returns null if no data was stored.
     */
    private MPSClientType getAutoClientType() {
        MPSClientType autoClient = null;
        com.citrix.wi.clientdetect.Client client = Include.getWizardState(wiContext).getRemoteClientResult();
        if (client != null) {
            autoClient = client.getClientType().getMPSClientType();
        }
        return autoClient;
    }

    /**
     * Method for saving user prefs which works either pre- or post- login; userContext will
     * be null pre-login.
     */
    private void savePrefs(UserPreferences userPrefs, WIContext wiContext, UserContext userContext) {
        if (userContext == null) {
            // pre-login
            Include.saveUserPrefsPreLogin(userPrefs, wiContext);
        } else {
            // post-login
            Include.saveUserPrefs(userPrefs, wiContext, userContext);
        }
    }

    /**
     * Save the user's client preferences.
     */
    public void savePreferences(UserContext userContext, UserPreferences writableUserPrefs) {

        if (!bProcessPostDone) {
            throw new RuntimeException(
                "clientSettingsUtils: you must call processPostRequest() before calling savePreferences()");
        }

        // Client type.
        if (bCustomizeClients) {
            if (autoClient) {
                // if they select Auto, then use the info from the wizard
                remoteClientType = getAutoClientType();
                if (remoteClientType != null) {
                    writableUserPrefs.setClientType(remoteClientType);
                    writableUserPrefs.setForcedClient(new Boolean(false));
                }
            } else if (remoteClientType != null) {
                writableUserPrefs.setClientType(remoteClientType);
                writableUserPrefs.setForcedClient(new Boolean(true));
            }
        }

        // We need to work out what the current client is; so apply
        // the changes so far then read it out:
        savePrefs(writableUserPrefs, wiContext, userContext);
        remoteClientType = Include.getSelectedRemoteClient(wiContext);

        // JICA Packages.
        if (bCustomizeJICAPackages
            && (remoteClientType == MPSClientType.JAVA)) {
            HashSet packages = new HashSet();
            if (bJicaAudio) {
                packages.add(JavaPackage.AUDIO);
            }
            if (bJicaCDM) {
                packages.add(JavaPackage.CLIENT_DRIVE_MAPPING);
            }
            if (bJicaClipboard) {
                packages.add(JavaPackage.CLIPBOARD);
            }
            if (bJicaConfigUI) {
                packages.add(JavaPackage.CONFIG_UI);
            }
            if (bJicaPrinter) {
                packages.add(JavaPackage.PRINTER_MAPPING);
            }
            if (bJicaZero) {
                packages.add(JavaPackage.ZERO_LATENCY);
            }
            writableUserPrefs.setJavaClientPackages(packages);
        }

        savePrefs(writableUserPrefs, wiContext, userContext);
    }
}
