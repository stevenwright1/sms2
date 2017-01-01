/*
 * Copyright (c) 2007 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.controls;

import com.citrix.wi.pageutils.Markup;

/**
 * Maintains presentation state for the Accounts settings page.
 */
public class AccountSettingsPageControl extends PageControl {

    private boolean showPasswordSection = false;
    
    private boolean showAutoLoginSettings = false;
    private boolean autoLogin = false;

    /**
     * Tests whether the password section should be shown.
     * @return <code>true</code> if shown, else <code>false</code>
     */
    public boolean getShowPasswordSection() {
        return showPasswordSection;
    }

    /**
     * Sets whether the password section should be shown.
     * @param value <code>true</code> if shown, else <code>false</code>
     */
    public void setShowPasswordSection(boolean value) {
        showPasswordSection = value;
    }

    /**
     * Tests whether auto login settings should be shown.
     * @return <code>true</code> if shown, else <code>false</code>
     */
    public boolean getShowAutoLoginSettings() {
        return showAutoLoginSettings;
    }

    /**
    * Sets whether auto login settings should be shown.
    * @param value code>true/code> if shown, else code>false/code>
    */
    public void setShowAutoLoginSettings(boolean value) {
        showAutoLoginSettings = value;
    }

    /**
     * Tests whether auto login is enabled.
     * @return code>true/code> if enabled, else code>false/code>
     */
    public boolean getAutoLogin() {
        return autoLogin;
    }

    /**
     * Sets whether auto login is enabled.
     * @param value code>true/code> if enabled, else code>false/code>
     */
    public void setAutoLogin(boolean value) {
        autoLogin = value;
    }

    /**
       * Tests whether auto login is selected.
       * @return <code>checked</code> if selected else an empty string
       */
    public String getAutoLoginCheckedStr () {
        return Markup.checkedStr(autoLogin);
    }

    /**
     * Determine if the Save/Cancel buttons need to be shown
     * @return <code>true</code> if the save/cancel buttons need to be shown.
     */
    public boolean getShowSaveAndCancelButtons() {
        return getShowAutoLoginSettings();
    }
}