/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.controls;

import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Markup;
import com.citrix.wi.types.ReconnectOption;

/**
 * Displays a number of Workspace Control settings to the end user.
 */
public class WorkspaceControlSettingsViewControl extends PageControl {

    private boolean showReconnectAtLoginSettings = false;
    private boolean showLogoffSettings = false;
    private ReconnectOption reconnectAtLogin = ReconnectOption.NONE;
    private boolean logoffApps = false;
    
    // Reconnection button section
    private boolean showReconnectButtonSettings = false;
    private ReconnectOption reconnectButton = ReconnectOption.NONE;
    public int workspaceControlLevel = Constants.RC_DISABLED; // Needed for the Javascript logic    

    /**
     * Determines whether the workspace control settings should be displayed at all. 
     * 
     * @return <code>true</code> if the workspace control settings should be 
     * displayed, else <code>false</code>.
     */
    public boolean getShowWorkspaceControlSettings() {
        return getShowReconnectAtLoginSettings() ||
               getShowReconnectButtonSettings() || 
               getShowLogoffSettings();
    }
    
    /**
     * Tests whether the Reconnect at Login settings should be shown.
     * @return <code>true</code> if shown, else <code>false</code>
     */
    public boolean getShowReconnectAtLoginSettings() {
        return showReconnectAtLoginSettings;
    }

    /**
     * Sets whether the Reconnect at Login settings should be shown.
     * @param value <code>true</code> if shown, else <code>false</code>
     */
    public void setShowReconnectAtLoginSettings(boolean value) {
        showReconnectAtLoginSettings = value;
    }

    /**
     * Tests whether the Logoff settings should be shown.
     * @return <code>true</code> if shown, else <code>false</code>
     */
    public boolean getShowLogoffSettings() {
        return showLogoffSettings;
    }

    /**
     * Sets whether the Logoff settings should be shown.
     * @param value <code>true</code> if shown, else <code>false</code>
     */
    public void setShowLogoffSettings(boolean value) {
        showLogoffSettings = value;
    }

    /**
     * Gets the selected Reconnect at Logon setting.
     * @return the <code>ReconnectOption</code>
     */
    public ReconnectOption getReconnectAtLogin() {
        return reconnectAtLogin;
    }

    /**
     * Sets the selected Reconnect at Logon setting.
     * @param value the <code>ReconnectOption</code>
     */
    public void setReconnectAtLogin(ReconnectOption value) {
        reconnectAtLogin = value;
    }

    /**
     * Tests whether applications should be disconnected at Log off.
     * @return <code>true</code> if disconnected, else <code>false</code>
     */
    public boolean getLogoffApps() {
        return logoffApps;
    }

    /**
     * Sets whether applications should be disconnected at Log off.
     * @param value <code>true</code> if disconnected, else <code>false</code>
     */
    public void setLogoffApps(boolean value) {
        logoffApps = value;
    }

    /**
     * Tests whether the reconnection option is checked
     * @return <code>checked</code> or an empty string
     */
    public String getReconnectCheckedStr() {
        return Markup.checkedStr(reconnectAtLogin == ReconnectOption.DISCONNECTED_AND_ACTIVE
            || reconnectAtLogin == ReconnectOption.DISCONNECTED);
    }

    /**
     * Tests whether the disconnected reconnection option selected.
     * @return <code>selected</code> or an empty string
     */
    public String getReconnectAtLoginDisconnectedSelectedStr() {
        return Markup.selectedStr(reconnectAtLogin != ReconnectOption.DISCONNECTED_AND_ACTIVE);
    }

    /**
     * Tests whether the reconnect disconnected and active at login setting is selected.
     * @return <code>selected</code> if selected, else an empty string
     */
    public String getReconnectAtLoginAllSelectedStr() {
        return Markup.selectedStr(reconnectAtLogin == ReconnectOption.DISCONNECTED_AND_ACTIVE);
    }

    /**
     * Tests whether the log off apps setting is selected.
     * @return <code>checked</code> if selected, else an empty string
     */
    public String getLogoffAppsCheckedStr() {
        return Markup.checkedStr(logoffApps);
    }

    /**
     * Tests whether the Reconnect Button Settings should be shown.
     * @return <code>true</code> if shown, else <code>false</code>
     */
    public boolean getShowReconnectButtonSettings() {
        return showReconnectButtonSettings;
    }

    /**
     * Sets whether the Reconnect Button Settings should be shown.
     * @param value <code>true</code> if shown, else <code>false</code>
     */
    public void setShowReconnectButtonSettings(boolean value) {
        showReconnectButtonSettings = value;
    }

    /**
     * Gets the selected Reconnect Button setting.
     * @return the <code>ReconnectOption</code>
     */
    public ReconnectOption getReconnectButton() {
        return reconnectButton;
    }

    /**
     * Sets the selected Reconnect Button setting.
     * @param value the <code>ReconnectOption</code>
     */
    public void setReconnectButton(ReconnectOption value) {
        reconnectButton = value;
    }

    /**
     * Tests whether the reconnect button setting is checked
     * @return <code>checked</code> if selected, else an empty string
     */
    public String getReconnectButtonCheckedStr() {
        return Markup.checkedStr(reconnectButton == ReconnectOption.DISCONNECTED ||
            reconnectButton == ReconnectOption.DISCONNECTED_AND_ACTIVE);
    }

    /**
     * Tests whether the reconnect disconnected button setting is selected.
     * @return <code>selected</code> if selected, else an empty string
     */
    public String getReconnectButtonDisconnectedSelectedStr() {
        return Markup.selectedStr(reconnectButton != ReconnectOption.DISCONNECTED_AND_ACTIVE);
    }

    /**
     * Tests whether the reconnect disconnected and active button setting is selected.
     * @return <code>selected</code> if selected, else an empty string
     */
    public String getReconnectButtonAllSelectedStr() {
        return Markup.selectedStr(reconnectButton == ReconnectOption.DISCONNECTED_AND_ACTIVE);
    }

    /**
     * Determine if the Save/Cancel buttons need to be shown
     * @return <code>true</code> if the save/cancel buttons need to be shown.
     */
    public boolean getShowSaveAndCancelButtons() {

        // if any of these sections are shown then we need to show the save/cancel buttons.
        if (getShowReconnectAtLoginSettings()
            || getShowLogoffSettings())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
