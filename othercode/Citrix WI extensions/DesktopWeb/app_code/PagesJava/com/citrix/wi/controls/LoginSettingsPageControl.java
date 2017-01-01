/*
 * Copyright (c) 2007 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.controls;

import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Markup;
import com.citrix.wi.types.BandwidthProfilePreference;
import com.citrix.wi.types.ReconnectOption;

/**
 * Maintains presentation state for the pre-Login Settings page.
 */
public class LoginSettingsPageControl {
    public boolean showDisplaySection = false;

    // Connection Preferences section
    public boolean showBandwidthOptions = false;
    public boolean showReconnectOptions = false;
    public BandwidthProfilePreference selectedBandwidth = null;
    public boolean showBandwidthCustom = false;
    public ReconnectOption reconnectAtLogin = ReconnectOption.NONE;
    public int workspaceControlLevel = Constants.RC_DISABLED; // Needed for the Javascript logic

    // Client settings and client summary table
    public ClientSettingsControl clientSettingsControl = new ClientSettingsControl();
    
    // Display settings
    private CommonDisplaySettingsControl displaySettings = new CommonDisplaySettingsControl();
    
    /**
     * Determines whether or not the General section is to be displayed.
     * 
     * @return <code>true</code> if the General section should be displayed,
     * else <code>false</code>.
     */
    public boolean getShowGeneralSection() {
       return showDisplaySection || 
              displaySettings.showLayoutSelection ||
              showReconnectOptions || 
              clientSettingsControl.getShowClientSettings(); 
    }

    /**
        * Tests whether the no bandwidth option selected.
        * @return <code>selected</code> or an empty string
        */
    public String getCustomBandwidthSelectedStr() {
        return Markup.selectedStr(selectedBandwidth == BandwidthProfilePreference.CUSTOM);
    }

    /**
     * Tests whether the high bandwidth option selected.
     * @return <code>selected</code> or an empty string
     */
    public String getHighBandwidthSelectedStr() {
        return Markup.selectedStr(selectedBandwidth == BandwidthProfilePreference.HIGH);
    }

    /**
     * Tests whether the medium high bandwidth option selected.
     * @return <code>selected</code> or an empty string
     */
    public String getMediumHighBandwidthSelectedStr() {
        return Markup.selectedStr(selectedBandwidth == BandwidthProfilePreference.MEDIUM_HIGH);
    }

    /**
     * Tests whether the medium bandwidth option selected.
     * @return <code>selected</code> or an empty string
     */
    public String getMediumBandwidthSelectedStr() {
        return Markup.selectedStr(selectedBandwidth == BandwidthProfilePreference.MEDIUM);
    }
    
    /**
     * Tests whether the no reconnection option selected.
     * @return <code>selected</code> or an empty string
     */
    public String getReconnectCheckedStr() {
        return Markup.checkedStr(reconnectAtLogin == ReconnectOption.DISCONNECTED_AND_ACTIVE
            || reconnectAtLogin == ReconnectOption.DISCONNECTED);
    }

    /**
     * Tests whether the active and disconnected reconnection option selected.
     * @return <code>selected</code> or an empty string
     */
    public String getAllSelectedStr() {
        return Markup.selectedStr(reconnectAtLogin == ReconnectOption.DISCONNECTED_AND_ACTIVE);
    }

    /**
     * Tests whether the low bandwidth option selected.
     * @return <code>selected</code> or an empty string
     */
    public String getLowBandwidthSelectedStr() {
        return Markup.selectedStr(selectedBandwidth == BandwidthProfilePreference.LOW);
    }

    /**
     * Tests whether the disconnected reconnection option selected.
     * @return <code>selected</code> or an empty string
     */
    public String getDisconnectedSelectedStr() {
        return Markup.selectedStr(reconnectAtLogin != ReconnectOption.DISCONNECTED_AND_ACTIVE);
    }

    /**
     * Retrieves an object encapsulating the display settings such as the 
     * display language and site layout.  
     * 
     * @return An object encapsulating the display settings such as the 
     * display language and site layout.
     */
    public CommonDisplaySettingsControl getDisplaySettings() {
        return displaySettings;
    }
}
