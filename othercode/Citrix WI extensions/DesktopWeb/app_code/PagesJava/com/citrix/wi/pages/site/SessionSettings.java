/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.site;

import com.citrix.wi.UserPreferences;
import com.citrix.wi.config.AppAccessMethodConfiguration;
import com.citrix.wi.config.ClientConnectionConfiguration;
import com.citrix.wi.config.WIConfiguration;
import com.citrix.wi.controls.SessionSettingsPageControl;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pages.site.preferences.policies.LocalResourcesSettingsPolicy;
import com.citrix.wi.pages.site.preferences.policies.PerformanceSettingsPolicy;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wi.types.AppDisplaySizePreference;
import com.citrix.wi.types.AudioQualityPreference;
import com.citrix.wi.types.BandwidthProfilePreference;
import com.citrix.wi.types.ColorDepthPreference;
import com.citrix.wing.types.AppAccessMethod;
import com.citrix.wing.types.KeyPassthroughOption;
import com.citrix.wing.webpn.UserContext;

/*
 * This page contains the following settings:
 * 
 * - Connection speed (bandwidth control) 
 * - Window colour 
 * - Sound quality 
 * - Printer mapping 
 * - Window size 
 * - Keyboard combos 
 * - PDA 
 * - Special folder redirection
 */
public class SessionSettings extends PreferencesSection {
    private LocalResourcesSettings       localResources;
    private LocalResourcesSettingsPolicy localResourcesPolicy;

    private PerformanceSettings          performanceSettings;
    private PerformanceSettingsPolicy    performanceSettingsPolicy;

    /**
     * Creates a new SessionSettings preferences section, generating its own
     * policy objects.
     * 
     * @param wiContext
     * @param viewControl The section's view control.
     */
    public SessionSettings(WIContext wiContext, SessionSettingsPageControl viewControl) {
        super(wiContext, viewControl);

        ClientConnectionConfiguration clientConnectionConfiguration = wiContext.getConfiguration()
                        .getClientConnectionConfiguration();

        localResources = new LocalResourcesSettings();
        localResourcesPolicy = new LocalResourcesSettingsPolicy(clientConnectionConfiguration);

        performanceSettings = new PerformanceSettings();
        performanceSettingsPolicy = new PerformanceSettingsPolicy(clientConnectionConfiguration);
    }

    private static AppDisplaySizePreference winSizeFromCustom(String desiredHRES, String desiredVRES)
                    throws ParseException {
        if (!UIUtils.isValidClientSize(desiredHRES, desiredVRES)) {
            throw new ParseException();
        }

        return AppDisplaySizePreference.fromString(desiredHRES + "x" + desiredVRES);
    }

    private static AppDisplaySizePreference parseWinSize(String winSize, String desiredHRES, String desiredVRES,
                    String screenPercent) throws ParseException {

        AppDisplaySizePreference result = null;

        if (AppDisplaySizePreference.CUSTOM.getType().equalsIgnoreCase(winSize)) {
            result = winSizeFromCustom(desiredHRES, desiredVRES);
        } else if (AppDisplaySizePreference.PERCENT.getType().equalsIgnoreCase(winSize)) {
            result = AppDisplaySizePreference.fromString(screenPercent + "%");
            if (result == null) {
                throw new ParseException(Constants.ID_LABEL_WINSIZE_PERCENT);
            }
        } else {
            result = AppDisplaySizePreference.fromString(winSize);
        }

        return result;
    }

    /**
     * Tests whether the bandwidth customization settings should be shown.
     * 
     * @return <code>true</code> if they should be shown, else
     * <code>false</code>
     */
    public static boolean getShowBandwidthCustom(WIContext wiContext) {
        ClientConnectionConfiguration config = wiContext.getConfiguration().getClientConnectionConfiguration();

        boolean showColorSettings = config.getAllowCustomizeWinColor();
        boolean showAudioSettings = config.getAllowCustomizeAudio();
        boolean showPrinterMapping = config.getAllowCustomizePrinterMapping();

        return (showColorSettings || showAudioSettings || showPrinterMapping);
    }

    public boolean isVisible() {
        AppAccessMethodConfiguration appAccessConfig = wiContext.getConfiguration().getAppAccessMethodConfiguration();
        if (!appAccessConfig.isEnabledAppAccessMethod(AppAccessMethod.REMOTE)) {
            // Don't show this page for a streaming-only site
            return false;
        }

        WIConfiguration config = wiContext.getConfiguration();
        return localResourcesPolicy.hasVisibleSettings(config) || performanceSettingsPolicy.hasVisibleSettings(config);
    }

    protected void getDataFromUserPreferences(UserPreferences userPreferences) {
        BandwidthProfilePreference bandwidthProfile = userPreferences.getBandwidthProfilePreference();
        performanceSettings.setBandwidthProfile(bandwidthProfile);

        if (bandwidthProfile == BandwidthProfilePreference.CUSTOM) {
            // Color depth
            if (performanceSettingsPolicy.canUserCustomizeColorDepth()) {        
                performanceSettings.setColorDepth(userPreferences.getColorDepthPreference());
            }

            // Audio
    		if (performanceSettingsPolicy.canUserCustomizeAudioQuality()) {
                performanceSettings.setAudioQuality(userPreferences.getAudioQualityPreference());
            }
    
            // Printer mapping
    		if (performanceSettingsPolicy.canUserCustomizePrinterMapping()) {
                performanceSettings.setPrinterMappingEnabled(!Boolean.FALSE.equals(userPreferences.getPrinterEnabled()));
            }
        }

        // Window size settings
        if (performanceSettingsPolicy.canUserCustomizeWindowSize()) {
            performanceSettings.setWindowSize(userPreferences.getAppDisplaySizePreference());
        }

        // Windows Key combination passthrough
        if (localResourcesPolicy.canUserCustomizeKeyPassthrough()) {
            localResources.setKeyPassthrough(userPreferences.getKeyPassthrough());
        }

        // PDA support
        if (localResourcesPolicy.canUserCustomizeVirtualCOMPort()) {
            localResources.setIsVirtualCOMPortEnabled(userPreferences.getVirtualCOMPortEnabled());
        }

        // Special folder redirection mapping
        if (localResourcesPolicy.canUserCustomizeSpecialFolderRedirection()) {
            localResources.setIsSpecialFolderRedirectionEnabled(userPreferences.getSpecialFolderRedirectionEnabled());
        }
    }

    protected boolean isDataValid() {
        // First make sure that we have the most up to date values from the user preferences
        // This will allow us to make decisions based on the current values
        getDataFromUserPreferences(wiContext.getUserPreferences());

        // Bandwidth control
        // "None" (meaning no bandwidth profile) will result in the bandwidth profile setting being set to null.
        if (performanceSettingsPolicy.canUserCustomizeBandwidth()) {
            String formBandwidthProfile = web.getFormParameter(Constants.ID_OPTION_BANDWIDTH);
            performanceSettings.setBandwidthProfile(BandwidthProfilePreference.fromString(formBandwidthProfile));
        }

        try {
            // The settings for colour depth, audio quality and printer mapping
            // are determined by the bandwidth setting and so should only be
            // saved if no profile has been selected.
			if (performanceSettings.getBandwidthProfile() == BandwidthProfilePreference.CUSTOM) {
                // Colour settings
                if (performanceSettingsPolicy.canUserCustomizeColorDepth()) {
                    String formColorDepth = web.getFormParameter(Constants.ID_OPTION_WINDOW_COLOR);
                    performanceSettings.setColorDepth(ColorDepthPreference.fromString(formColorDepth));
                }

                // Audio settings
                if (performanceSettingsPolicy.canUserCustomizeAudioQuality()) {
                    String formAudioQuality = web.getFormParameter(Constants.ID_OPTION_AUDIO);
                    if (formAudioQuality == null) {
                        throw new ParseException();
                    }
                    performanceSettings.setAudioQuality(AudioQualityPreference.fromString(formAudioQuality));
                    if (performanceSettings.getAudioQuality() == null) {
                        throw new ParseException();
                    }
                }

                // Printer mapping settings
                String formPrinterMapping = web.getFormParameter(Constants.ID_CHECK_PRINTER);
                if (performanceSettingsPolicy.canUserCustomizePrinterMapping()
                                && isCheckboxValueValid(formPrinterMapping)) {
                    performanceSettings.setPrinterMappingEnabled(Constants.VAL_ON.equalsIgnoreCase(formPrinterMapping));
                }
            }

            // Validate window size
            if (performanceSettingsPolicy.canUserCustomizeWindowSize()) {
                String formWinSize = web.getFormParameter(Constants.ID_OPTION_WINDOW_SIZE);
                String formDesiredHRES = web.getFormParameter(Constants.ID_TEXT_DESIRED_HRES);
                String formDesiredVRES = web.getFormParameter(Constants.ID_TEXT_DESIRED_VRES);
                String formScreenPercent = web.getFormParameter(Constants.ID_TEXT_SCREEN_PERCENT);
                performanceSettings.setWindowSize(parseWinSize(formWinSize, formDesiredHRES, formDesiredVRES,
                                formScreenPercent));
            }

            // Keyboard Pass-through settings
            if (localResourcesPolicy.canUserCustomizeKeyPassthrough()) {
                String formKeyPassthrough = web.getFormParameter(Constants.ID_OPTION_KEY_PASSTHROUGH);
                localResources.setKeyPassthrough(KeyPassthroughOption.fromString(formKeyPassthrough));
                if (localResources.getKeyPassthrough() == null) {
                    throw new ParseException();
                }
            }

            // Virtual COM Port
            String formVirtualCOMPortEnabled = web.getFormParameter(Constants.ID_CHECK_VIRTUAL_COM_PORT);
            if (localResourcesPolicy.canUserCustomizeVirtualCOMPort()
                            && isCheckboxValueValid(formVirtualCOMPortEnabled)) {
                localResources.setIsVirtualCOMPortEnabled(new Boolean(Constants.VAL_ON
                                .equalsIgnoreCase(formVirtualCOMPortEnabled)));
            }

            // Special Folder Redirection
            String formSpecialFolderRedirectionEnabled = web
                            .getFormParameter(Constants.ID_CHECK_SPECIALFOLDERREDIRECTION);
            if (localResourcesPolicy.canUserCustomizeSpecialFolderRedirection()
                            && isCheckboxValueValid(formSpecialFolderRedirectionEnabled)) {
                localResources.setIsSpecialFolderRedirectionEnabled(new Boolean(Constants.VAL_ON
                                .equalsIgnoreCase(formSpecialFolderRedirectionEnabled)));
            }
        } catch (ParseException e) {
            // If the parsing failed, present user with settings retrieved from
            // session.  Mark invalid fields, if any.
            String invalidField = e.getInvalidField();
            if (invalidField != null) {
                viewControl.addInvalidField(invalidField);
            }
            return false;
        }

        return true;
    }

    protected void savePreferences(UserContext userContext, UserPreferences userPreferences) {
        userPreferences.setAppDisplaySizePreference(null);

        if (performanceSettingsPolicy.canUserCustomizeWindowSize()) {
            userPreferences.setAppDisplaySizePreference(performanceSettings.getWindowSize());
        }

        if (localResourcesPolicy.canUserCustomizeKeyPassthrough()) {
            userPreferences.setKeyPassthrough(localResources.getKeyPassthrough());
        }

        if (performanceSettingsPolicy.canUserCustomizeBandwidth()) {
            userPreferences.setBandwidthBandwidthProfilePreference(performanceSettings.getBandwidthProfile());
        }

        // Doesn't have an associated policy check, because the settings for the
        // defined bandwidth profiles must be saved whatever.  This is to ensure
        // that the bandwidth profile's defined settings over-write any custom
        // settings held over from a previous configuration.  
        saveBandwidthSettings(userPreferences, performanceSettings.getBandwidthProfile());

        if (localResourcesPolicy.canUserCustomizeVirtualCOMPort()) {
            userPreferences.setVirtualCOMPortEnabled(localResources.getIsVirtualCOMPortEnabled());
        }

        if (localResourcesPolicy.canUserCustomizeSpecialFolderRedirection()) {
            userPreferences.setSpecialFolderRedirectionEnabled(localResources.getIsSpecialFolderRedirectionEnabled());
        }
    }

    private void saveBandwidthSettings(UserPreferences userPreferences, BandwidthProfilePreference bandwidthProfile) {
        
        if (bandwidthProfile == BandwidthProfilePreference.CUSTOM) {
            saveCustomBandwidthProfile(userPreferences);
        } else if (bandwidthProfile == BandwidthProfilePreference.HIGH) {
            saveHighBandwidthProfile(userPreferences);
        } else if (bandwidthProfile == BandwidthProfilePreference.MEDIUM_HIGH) {
            saveMediumHighBandwidthProfile(userPreferences);
        } else if (bandwidthProfile == BandwidthProfilePreference.MEDIUM) {
            saveMediumBandwidthProfile(userPreferences);
        } else if (bandwidthProfile == BandwidthProfilePreference.LOW) {
            saveLowBandwidthProfile(userPreferences);
        }
    }

    private void saveLowBandwidthProfile(UserPreferences userPreferences) {
        userPreferences.setColorDepthPreference(ColorDepthPreference.COLOR_HIGH);
        userPreferences.setAudioQualityPreference(AudioQualityPreference.OFF);
        userPreferences.setPrinterEnabled(Boolean.FALSE);
    }

    private void saveMediumBandwidthProfile(UserPreferences userPreferences) {
        userPreferences.setColorDepthPreference(ColorDepthPreference.COLOR_HIGH);
        userPreferences.setAudioQualityPreference(AudioQualityPreference.OFF);
        userPreferences.setPrinterEnabled(Boolean.FALSE);
    }

    private void saveMediumHighBandwidthProfile(UserPreferences userPreferences) {
        userPreferences.setColorDepthPreference(ColorDepthPreference.COLOR_HIGH);
        userPreferences.setAudioQualityPreference(AudioQualityPreference.OFF);
        userPreferences.setPrinterEnabled(Boolean.TRUE);
    }

    private void saveHighBandwidthProfile(UserPreferences userPreferences) {
        userPreferences.setColorDepthPreference(ColorDepthPreference.COLOR_TRUE);
        userPreferences.setAudioQualityPreference(AudioQualityPreference.HIGH);
        userPreferences.setPrinterEnabled(Boolean.TRUE);
    }

    private void saveCustomBandwidthProfile(UserPreferences userPreferences) {
        if (performanceSettingsPolicy.canUserCustomizeColorDepth()) {
            userPreferences.setColorDepthPreference(performanceSettings.getColorDepth());
        }

        if (performanceSettingsPolicy.canUserCustomizeAudioQuality()) {
            userPreferences.setAudioQualityPreference(performanceSettings.getAudioQuality());
        }

        if (performanceSettingsPolicy.canUserCustomizePrinterMapping()) {
            userPreferences.setPrinterEnabled(new Boolean(performanceSettings.isPrinterMappingEnabled()));
        }
    }

    protected void setupView() {
        SessionSettingsPageControl viewControl = (SessionSettingsPageControl)this.viewControl;

        // Bandwidth
        viewControl.setBandwidth(performanceSettings.getBandwidthProfile());
        viewControl.setShowBandwidthOptions(performanceSettingsPolicy.canUserCustomizeBandwidth());
        if (performanceSettingsPolicy.canUserCustomizeBandwidth()) {
            viewControl.setShowBandwidthCustom(getShowBandwidthCustom(wiContext));
        }

        // Only show the color depth, audio quality and printer mapping settings on the page
        // if the user is actually permitted to change them - i.e. if the user could change the
        // bandwidth level to "custom" or if the admin has forced it to be "custom".
        if (performanceSettingsPolicy.canUserCustomizeBandwidth() ||
            performanceSettings.getBandwidthProfile() == BandwidthProfilePreference.CUSTOM) {
            // Window color
            viewControl.setShowColorSettings(performanceSettingsPolicy.canUserCustomizeColorDepth());
            if (performanceSettingsPolicy.canUserCustomizeColorDepth()) {
                viewControl.setColorDepth(performanceSettings.getColorDepth());
            }

            // Audio quality
            viewControl.setShowAudioSettings(performanceSettingsPolicy.canUserCustomizeAudioQuality());
            if (performanceSettingsPolicy.canUserCustomizeAudioQuality()) {
                viewControl.setAudioQuality(performanceSettings.getAudioQuality());
            }

            // Printer mapping
            viewControl.setShowPrinterMapping(performanceSettingsPolicy.canUserCustomizePrinterMapping());
            if (performanceSettingsPolicy.canUserCustomizePrinterMapping()) {
                viewControl.setPrinterMapping(performanceSettings.isPrinterMappingEnabled());
            }
        }

        // Window size
        viewControl.setShowWindowSizeSettings(performanceSettingsPolicy.canUserCustomizeWindowSize());
        if (performanceSettingsPolicy.canUserCustomizeWindowSize()) {
            viewControl.setAppDisplaySize(performanceSettings.getWindowSize());
        }

        // Key Passthrough
        viewControl.setShowKeyPassthroughSettings(localResourcesPolicy.canUserCustomizeKeyPassthrough());
        if (localResourcesPolicy.canUserCustomizeKeyPassthrough()) {
            viewControl.setKeyPassthroughOption(localResources.getKeyPassthrough());
        }

        // PDA Support
        viewControl.setShowVirtualCOMPort(localResourcesPolicy.canUserCustomizeVirtualCOMPort());
        if (localResourcesPolicy.canUserCustomizeVirtualCOMPort()) {
            viewControl.setVirtualCOMPort(localResources.getIsVirtualCOMPortEnabled());
        }

        // Special folder redirection
        viewControl.setShowSpecialFolderRedirection(localResourcesPolicy.canUserCustomizeSpecialFolderRedirection());
        if (localResourcesPolicy.canUserCustomizeSpecialFolderRedirection()) {
            Boolean setting = localResources.getIsSpecialFolderRedirectionEnabled();
            if (setting != null) {
                viewControl.setSpecialFolderRedirection(setting.booleanValue());
            } else {
                viewControl.setSpecialFolderRedirection(false);
            }
        }
    }
}

class PerformanceSettings {
    private BandwidthProfilePreference bandwidthProfile        = null;
    private ColorDepthPreference       colorDepth              = null;
    private AudioQualityPreference     audioQuality            = null;
    private boolean                    isPrinterMappingEnabled = true;
    private AppDisplaySizePreference   windowSize              = null;

    /**
     * @return the bandwidthProfile
     */
    public BandwidthProfilePreference getBandwidthProfile() {
        return bandwidthProfile;
    }

    /**
     * @param bandwidthProfile the bandwidthProfile to set
     */
    public void setBandwidthProfile(BandwidthProfilePreference bandwidthProfile) {
        this.bandwidthProfile = bandwidthProfile;
    }

    /**
     * @return the colorDepth
     */
    public ColorDepthPreference getColorDepth() {
        return colorDepth;
    }

    /**
     * @param colorDepth the colorDepth to set
     */
    public void setColorDepth(ColorDepthPreference colorDepth) {
        this.colorDepth = colorDepth;
    }

    /**
     * @return the audioQuality
     */
    public AudioQualityPreference getAudioQuality() {
        return audioQuality;
    }

    /**
     * @param audioQuality the audioQuality to set
     */
    public void setAudioQuality(AudioQualityPreference audioQuality) {
        this.audioQuality = audioQuality;
    }

    /**
     * @return the isPrinterMappingEnabled
     */
    public boolean isPrinterMappingEnabled() {
        return isPrinterMappingEnabled;
    }

    /**
     * @param isPrinterMappingEnabled the isPrinterMappingEnabled to set
     */
    public void setPrinterMappingEnabled(boolean isPrinterMappingEnabled) {
        this.isPrinterMappingEnabled = isPrinterMappingEnabled;
    }

    /**
     * @return the windowSize
     */
    public AppDisplaySizePreference getWindowSize() {
        return windowSize;
    }

    /**
     * @param windowSize the windowSize to set
     */
    public void setWindowSize(AppDisplaySizePreference windowSize) {
        this.windowSize = windowSize;
    }
}

class LocalResourcesSettings {
    private KeyPassthroughOption keyPassthrough                    = null;
    private Boolean              isVirtualCOMPortEnabled           = null;
    private Boolean              isSpecialFolderRedirectionEnabled = null;

    /**
     * @return the keyPassthrough
     */
    public KeyPassthroughOption getKeyPassthrough() {
        return keyPassthrough;
    }

    /**
     * @param keyPassthrough the keyPassthrough to set
     */
    public void setKeyPassthrough(KeyPassthroughOption keyPassthrough) {
        this.keyPassthrough = keyPassthrough;
    }

    /**
     * @return the isVirtualCOMPortEnabled
     */
    public Boolean getIsVirtualCOMPortEnabled() {
        return isVirtualCOMPortEnabled;
    }

    /**
     * @param isVirtualCOMPortEnabled the isVirtualCOMPortEnabled to set
     */
    public void setIsVirtualCOMPortEnabled(Boolean isVirtualCOMPortEnabled) {
        this.isVirtualCOMPortEnabled = isVirtualCOMPortEnabled;
    }

    /**
     * @return the isSpecialFolderRedirectionEnabled
     */
    public Boolean getIsSpecialFolderRedirectionEnabled() {
        return isSpecialFolderRedirectionEnabled;
    }

    /**
     * @param isSpecialFolderRedirectionEnabled the
     * isSpecialFolderRedirectionEnabled to set
     */
    public void setIsSpecialFolderRedirectionEnabled(Boolean isSpecialFolderRedirectionEnabled) {
        this.isSpecialFolderRedirectionEnabled = isSpecialFolderRedirectionEnabled;
    }
}

class ParseException extends Exception {

    private String invalidField;

    /**
     * Default constructor.
     */
    public ParseException() {
        super();
    }

    /**
     * Constructor.
     * 
     * @param invalidField id of the field that failed to parse
     */
    public ParseException(String invalidField) {
        super();
        this.invalidField = invalidField;
    }

    /**
     * Gets the id of invalid field.
     * 
     * @return the id of invalid field.
     */
    public String getInvalidField() {
        return invalidField;
    }
}
