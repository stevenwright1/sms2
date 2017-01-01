/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.controls;

import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Markup;
import com.citrix.wi.types.AppDisplaySizePreference;
import com.citrix.wi.types.AudioQualityPreference;
import com.citrix.wi.types.BandwidthProfilePreference;
import com.citrix.wi.types.ColorDepthPreference;
import com.citrix.wing.types.KeyPassthroughOption;

/**
 * Maintains presentation state for the Session Settings page.
 */
public class SessionSettingsPageControl extends PageControl {
    // Bandwidth control
    private boolean showBandwidthOptions = false;
    private BandwidthProfilePreference bandwidthProfile = null;
    private boolean showBandwidthCustom = false;

    // Color depth settings.
    private boolean showColorSettings = false;
    private ColorDepthPreference colorDepth = null;

    // Audio quality settings.
    private boolean showAudioSettings = false;
    private AudioQualityPreference audioQuality = null;

    private boolean showWindowSizeSettings = false;
    private AppDisplaySizePreference selectedWindowSize = AppDisplaySizePreference.NO_PREFERENCE;

    // Printer mapping
    private boolean showPrinterMapping = false;
    private boolean printerMapping = true;

    // Windows Key combination pass through
    private boolean showKeyPassthroughSettings = false;
    private KeyPassthroughOption keyPassthroughOption = null;

    // Virtual COM Port
    private boolean showVirtualCOMPortSetting = false;
    private Boolean virtualCOMPortEnabled = Boolean.TRUE;

    // Special folder redirection
    private boolean showSpecialFolderRedirection = false;
    private boolean specialFolderRedirection = true;

    public void setAppDisplaySize(AppDisplaySizePreference displaySize) {
        selectedWindowSize = displaySize;
    }

    /**
     * Tests whether the Performance settings should be shown.
     * @return <code>true</code> if they should be shown, else <code>false</code>
     */
    public boolean getShowUserExperienceSection() {
        return (showBandwidthOptions ||
                showColorSettings ||
                showAudioSettings ||
                showPrinterMapping ||
                showWindowSizeSettings ||
                showKeyPassthroughSettings);
    }

    /**
     * Tests whether the Local Resources settings (KeyPassthrough, Virtual COM Port, SFR)
     * should be shown.
     * @return <code>true</code> if they should be shown, else <code>false</code>
     */
    public boolean getShowDevicesSection() {
        return (showVirtualCOMPortSetting || showSpecialFolderRedirection);
    }

    /**
     * Tests whether the window size settings should be shown.  Currently un-used as
     * getShowDisplaySection has superceded this method; could come back into use if
     * more settings are added to the Display Section.
     * @return <code>true</code> if they should be shown, else <code>false</code>
     */
    public boolean getShowWindowSizeSettings() {
        return showWindowSizeSettings;
    }

    /**
     * Sets whether the window size settings should be shown.
     * @param value <code>true</code> if they should be shown, else <code>false</code>
     */
    public void setShowWindowSizeSettings(boolean value) {
        showWindowSizeSettings = value;
    }

    /**
     * Tests whether the keyboard pass-thru settings should be shown.
     * @return <code>true</code> if they should be shown, else <code>false</code>
     */
    public boolean getShowKeyPassthroughSettings() {
        return showKeyPassthroughSettings;
    }

    /**
     * Sets whether the keyboard pass-thru settings should be shown.
     * @param value <code>true</code> if they should be shown, else <code>false</code>
     */
    public void setShowKeyPassthroughSettings(boolean value) {
        showKeyPassthroughSettings = value;
    }

    /**
     * Gets whether the color settings should be shown.
     * @return <code>true</code> if they should be shown, else <code>false</code>
     */
    public boolean getShowColorSettings() {
        return showColorSettings;
    }

    /**
     * Sets whether the color settings should be shown.
     * @param value <code>true</code> if they should be shown, else <code>false</code>
     */
    public void setShowColorSettings(boolean value) {
        showColorSettings = value;
    }

    /**
     * Gets whether the audio settings should be shown.
     * @return <code>true</code> if they should be shown, else <code>false</code>
     */
    public boolean getShowAudioSettings() {
        return showAudioSettings;
    }

    /**
     * Sets whether the audio settings should be shown.
     * @param value <code>true</code> if they should be shown, else <code>false</code>
     */
    public void setShowAudioSettings(boolean value) {
        showAudioSettings = value;
    }

    /**
    * Sets whether the bandwidth optimization options should be shown.
    * @param value <code>true</code> if they should be shown, else <code>false</code>
    */
    public void setShowBandwidthOptions(boolean value) {
        showBandwidthOptions = value;
    }

    /**
     * Sets the bandwidth settings.
     * @param value the <code>BandwidthProfile</code>
     */
    public void setBandwidth(BandwidthProfilePreference value) {
        bandwidthProfile = value;
    }

    /**
     * Tests whether the bandwidth options should be shown.
     * @return <code>true</code> if they should be shown, else <code>false</code>
     */
    public boolean getShowBandwidthOptions() {
        return showBandwidthOptions;
    }

    /**
     * Tests whether the bandwidth custom option should be shown.
     * @return <code>true</code> if they should be shown, else <code>false</code>
     */
    public boolean getShowBandwidthCustom() {
        return showBandwidthCustom;
    }

    /**
     * Sets whether the custom bandwidth option should be shown.
     * @param value <code>true</code> if it should be shown, else <code>false</code>
     */
    public void setShowBandwidthCustom(boolean value) {
        showBandwidthCustom = value;
    }

    /**
     * Tests whether the Printer Mapping settings should be shown.
     * @return <code>true</code> if they should be shown, else <code>false</code>
     */
    public boolean getShowPrinterMapping() {
        return showPrinterMapping;
    }

    /**
     * Sets whether the Printer Mapping settings should be shown.
     * @param value <code>true</code> if they should be shown, else <code>false</code>
     */
    public void setShowPrinterMapping(boolean value) {
        showPrinterMapping = value;
    }

    /**
     * Tests whether the Printer Mapping is enabled.
     * @return <code>true</code> if enabled, else <code>false</code>
     */
    public boolean getPrinterMapping() {
        return printerMapping;
    }

    /**
     * Sets whether the Printer Mapping is enabled.
     * @param value <code>true</code> if enabled, else <code>false</code>
     */
    public void setPrinterMapping(boolean value) {
        printerMapping = value;
    }

    /**
     * Tests whether the Special Folder Redirection settings should be shown.
     * @return <code>true</code> if they should be shown, else <code>false</code>
     */
    public boolean getShowSpecialFolderRedirection() {
        return showSpecialFolderRedirection;
    }

    /**
     * Sets whether the Special Folder Redirection settings should be shown.
     * @param value <code>true</code> if they should be shown, else <code>false</code>
     */
    public void setShowSpecialFolderRedirection(boolean value) {
        showSpecialFolderRedirection = value;
    }

    /**
     * Tests whether the Special Folder Redirection is enabled.
     * @return <code>true</code> if enabled, else <code>false</code>
     */
    public boolean getSpecialFolderRedirection() {
        return specialFolderRedirection;
    }

    /**
     * Sets whether the Special Folder Redirection is enabled.
     * @param value <code>true</code> if enabled, else <code>false</code>
     */
    public void setSpecialFolderRedirection(boolean value) {
        specialFolderRedirection = value;
    }

    /**
     * Tests whether the Special Folder Redirection settings should be shown.
     * @return <code>true</code> if they should be shown, else <code>false</code>
     */
    public boolean getShowVirtualCOMPort() {
        return showVirtualCOMPortSetting;
    }

    /**
     * Sets whether the Special Folder Redirection settings should be shown.
     * @param value <code>true</code> if they should be shown, else <code>false</code>
     */
    public void setShowVirtualCOMPort(boolean value) {
        showVirtualCOMPortSetting = value;
    }

    /**
     * Tests whether the Virtual COM Port is enabled.
     * @return <code>true</code> if enabled, else <code>false</code>
     */
    public Boolean getVirtualCOMPort() {
        return virtualCOMPortEnabled;
    }

    /**
     * Sets whether the Special Folder Redirection is enabled.
     * @param value <code>true</code> if enabled, else <code>false</code>
     */
    public void setVirtualCOMPort(Boolean value) {
        virtualCOMPortEnabled = value;
    }

    /**
     * Gets the window horizontal size.
     * @return the number of pixels
     */
    public String getWindowHRes() {
        return String.valueOf(selectedWindowSize.getWindowHRes());
    }

    /**
     * Gets the window vertical size.
     * @return the number of pixels
     */
    public String getWindowVRes() {
        return String.valueOf(selectedWindowSize.getWindowVRes());
    }

    /**
     * Gets the window size, as a screen percentage.
     * @return the percentage
     */
    public String getScreenPercent() { 
        return String.valueOf(selectedWindowSize.getScreenPercent());
    }

    /**
     * Gets the keyboard pass-thru setting.
     * @return the <code>KeyPassthroughOption</code>
     */
    public KeyPassthroughOption getKeyPassthroughOption() {
        return keyPassthroughOption;
    }

    /**
     * Sets the keyboard pass-thru setting.
     * @param value the <code>KeyPassthroughOption</code>
     */
    public void setKeyPassthroughOption(KeyPassthroughOption value) {
        keyPassthroughOption = value;
    }

    /**
     * Gets the color depth setting.
     * @return the <code>ColorDepth</code>
     */
    public ColorDepthPreference getColorDepth() {
        return colorDepth;
    }

    /**
     * Sets the color depth setting.
     * @param value the <code>ColorDepth</code>
     */
    public void setColorDepth(ColorDepthPreference value) {
        colorDepth = value;
    }

    /**
     * Gets the Audio settings.
     * @return the <code>AudioQuality</code>
     */
    public AudioQualityPreference getAudioQuality() {
        return audioQuality;
    }

    /**
     * Sets the Audio settings.
     * @param value the <code>AudioQuality</code>
     */
    public void setAudioQuality(AudioQualityPreference value) {
        audioQuality = value;
    }

    public String getWindowSizeSelectedStr(AppDisplaySizePreference windowSize) {
        boolean isEnabled = false;

        // Small hack to ensure the Percentage of screen option is chosen when 
        // an invalid percentage value has been supplied.
        boolean invalidPercentage = Constants.ID_LABEL_WINSIZE_PERCENT.equalsIgnoreCase(getFirstInvalidField()) &&
                                    windowSize.equals(AppDisplaySizePreference.PERCENT);
        
        isEnabled = invalidPercentage ? true : selectedWindowSize.equals(windowSize);        
        return Markup.selectedStr(isEnabled);
    }

    public String getKeyPassthroughSelectedStr(KeyPassthroughOption keyPassthroughStr) {
        boolean isEnabled = (keyPassthroughStr == keyPassthroughOption);
        return Markup.selectedStr(isEnabled);
    }

    /**
     * Tests whether the given color setting is selected.
     * @return <code>selected</code> if selected, else an empty string
     */
    public String getColorSelectedStr(ColorDepthPreference color) {
        boolean isColorSelected = false;
        if (bandwidthProfile == BandwidthProfilePreference.CUSTOM) {
            isColorSelected = (colorDepth == color);
        } else if (bandwidthProfile == BandwidthProfilePreference.HIGH) {
            isColorSelected = (color == ColorDepthPreference.COLOR_TRUE);
        } else {
            isColorSelected = (color == ColorDepthPreference.COLOR_HIGH);
        }

        return Markup.selectedStr(isColorSelected);
    }

    /**
     * Tests whether the given audio quality setting is selected.
     * @return <code>selected</code> if selected, else an empty string
     */
    public String getAudioQualitySelectedStr(AudioQualityPreference audio) {
        boolean isAudioQualitySelected = false;

        if (bandwidthProfile == BandwidthProfilePreference.CUSTOM) {
            isAudioQualitySelected = (audio == audioQuality);
        } else if (bandwidthProfile == BandwidthProfilePreference.HIGH) {
            isAudioQualitySelected = (audio == AudioQualityPreference.HIGH);
        } else {
            isAudioQualitySelected = (audio == AudioQualityPreference.OFF);
        }
        return Markup.selectedStr(isAudioQualitySelected);
    }

    /**
     * Tests whether the printer mapping is checked.
     * @return <code>checked</code> selected, else an empty string
     */
    public String getPrinterMappingCheckedStr() {
        boolean highBandwidth = bandwidthProfile == BandwidthProfilePreference.HIGH;
        boolean mediumHighBandwidth = bandwidthProfile == BandwidthProfilePreference.MEDIUM_HIGH;
        boolean customBandwidth = bandwidthProfile == BandwidthProfilePreference.CUSTOM;

        return Markup.checkedStr(highBandwidth ||
                                 mediumHighBandwidth ||
                                 (customBandwidth && printerMapping));
    }

    /**
     * Tests whether the Special Folder Redirection setting is selected.
     * @return <code>checked</code> selected, else an empty string
     */
    public String getSpecialFolderRedirectionCheckedStr() {
        return Markup.checkedStr(specialFolderRedirection);
    }

    /**
     * Tests whether the bandwidth setting is selected.
     * 
     * @param bandwidth a bandwidth profile preference.
     * @return <code>selected</code> if selected, else an empty string
     */
    public String getBandwidthSelectedStr(BandwidthProfilePreference bandwidth) {
        return Markup.selectedStr(bandwidth == bandwidthProfile);
    }

    /**
     * Tests whether the Virtual COM Port setting is selected.
     * @return <code>checked</code> if selected, else an empty string
     */
    public String getVirtualCOMPortCheckedStr() {
        boolean isEnabled = Boolean.TRUE.equals(virtualCOMPortEnabled);
        return Markup.checkedStr(isEnabled);
    }
}
