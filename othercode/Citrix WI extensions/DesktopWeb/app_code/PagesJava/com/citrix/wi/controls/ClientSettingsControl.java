/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.controls;

import com.citrix.wi.pageutils.Markup;
import com.citrix.wi.types.MPSClientType;

/**
 * Maintains presentation state for Client Settings ie AppAccessMethod,
 * Remote client, Java packages and Client Summary table.
 *
 * This control is used by both the Connection Prefs and Login Prefs Pages.
 */
public class ClientSettingsControl extends PageControl {

    // Whether to show the remote client type selection
    public boolean showClientType = false;

    // What to show in the remote client type selection
    public boolean showICALocal = false;
    public boolean showICAJava = false;
    public boolean showRDPEmbedded = false;

    // The selected remote client
    public MPSClientType selectedClient = null;

    // Localized inline help for the remote client type
    public String clientTypeInlineHelp = "";;

    // Java packages.
    public boolean showJicaPackages = false;
    public boolean jicaAudioSelected = false;
    public boolean jicaCDMSelected = false;
    public boolean jicaClipboardSelected = false;
    public boolean jicaConfigUISelected = false;
    public boolean jicaPrinterSelected = false;
    public boolean jicaZeroSelected = false;

    // Whether to show a link to the wizard (advanced mode)
    public boolean showAdvancedModeWizardLink = false;

    // Whether to show a link to the wizard (auto mode)
    public boolean showAutoModeWizardLink = false;

    /**
     * Tests whether to show the Client Preferences.
     * @return <code>true</code> if it should be shown, else <code>false</code>
     */
    public boolean getShowClientSettings() {
        return (showClientType || showJicaPackages
            || showAdvancedModeWizardLink || showAutoModeWizardLink);
    }

    /**
     * Tests whether Local client is selected in the client type selection.
     * @return <code>selected</code> if selected, else an empty string
     */
    public String getClientICALocalSelectedStr() {
        return Markup.selectedStr(selectedClient == MPSClientType.LOCAL_ICA);
    }

    /**
     * Tests whether Citrix Presentation Server Client for Java is selected in the client type
     * selection.
     * @return <code>selected</code> if selected, else an empty string
     */
    public String getClientICAJavaSelectedStr() {
        return Markup.selectedStr(selectedClient == MPSClientType.JAVA);
    }

    /**
     * Tests whether RDPEmbedded client is selected in the client type
     * selection.
     * @return <code>selected</code> if selected, else an empty string
     */
    public String getClientRDPEmbeddedSelectedStr() {
        return Markup.selectedStr(selectedClient == MPSClientType.EMBEDDED_RDP);
    }

    /**
     * Tests whether the checkbox for Citrix Presentation Server Client for Java Audio package is selected.
     * @return <code>checked</code> if selected else an empty string
     */
    public String getJICAAudioCheckedStr() {
        return Markup.checkedStr( jicaAudioSelected );
    }

    /**
     * Tests whether the checkbox for Citrix Presentation Server Client for Java Client Drive Mapping package is selected.
     * @return <code>checked</code> if selected else an empty string
     */
    public String getJICACDMCheckedStr() {
        return Markup.checkedStr( jicaCDMSelected );
    }

    /**
     * Tests whether the checkbox for Citrix Presentation Server Client for Java Clipboard package is selected.
     * @return <code>checked</code> if selected else an empty string
     */
    public String getJICAClipboardCheckedStr() {
        return Markup.checkedStr( jicaClipboardSelected );
    }

    /**
     * Tests whether the checkbox for Citrix Presentation Server Client for Java ConfigUI package is selected.
     * @return <code>checked</code> if selected else an empty string
     */
    public String getJICAConfigUICheckedStr() {
        return Markup.checkedStr( jicaConfigUISelected );
    }

    /**
     * Tests whether the checkbox for Citrix Presentation Server Client for Java Printer Mapping package is selected.
     * @return <code>checked</code> if selected else an empty string
     */
    public String getJICAPrinterCheckedStr() {
        return Markup.checkedStr( jicaPrinterSelected );
    }

    /**
     * Tests whether the checkbox for Citrix Presentation Server Client for Java Zero-Latency package is selected.
     * @return <code>checked</code> if selected else an empty string
     */
    public String getJICAZeroCheckedStr() {
        return Markup.checkedStr( jicaZeroSelected );
    }
}
