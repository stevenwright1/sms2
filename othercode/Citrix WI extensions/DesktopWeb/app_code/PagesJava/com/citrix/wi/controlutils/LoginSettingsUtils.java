/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.controlutils;

import com.citrix.wi.config.AppAccessMethodConfiguration;
import com.citrix.wi.config.ClientConnectionConfiguration;
import com.citrix.wi.config.UIConfiguration;
import com.citrix.wi.config.WorkspaceControlConfiguration;
import com.citrix.wi.controls.CommonDisplaySettingsControl;
import com.citrix.wi.controls.LoginSettingsPageControl;
import com.citrix.wi.localization.LanguageManager;
import com.citrix.wi.localization.LanguagePack;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pages.site.SessionSettings;
import com.citrix.wi.pages.site.preferences.policies.PerformanceSettingsPolicy;
import com.citrix.wi.pageutils.ClientSettingsUtils;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.LaunchUtilities;
import com.citrix.wi.pageutils.Settings;
import com.citrix.wing.types.AppAccessMethod;

/**
 * Utility class for the Login Settings functionality.
 */
public class LoginSettingsUtils {

    /**
     * Set up a <code>LoginSettingsPageControl</code>.
     *
     * @param wiContext The <code>WIContext</code>
     * @param viewControl The <code>LoginSettingsPageControl</code> to populate
     */
    static public void populate(WIContext wiContext, LoginSettingsPageControl viewControl) {
        viewControl.showBandwidthOptions = allowCustomizeBandwidth(wiContext);
        viewControl.selectedBandwidth = wiContext.getUserPreferences().getBandwidthProfilePreference();
        viewControl.showBandwidthCustom = SessionSettings.getShowBandwidthCustom(wiContext);

        boolean bShowReconnectOptions = allowCustomizeReconnectAtLogin(wiContext);
        viewControl.showReconnectOptions = bShowReconnectOptions;
        viewControl.workspaceControlLevel = Include.getWorkspaceControlLevel(wiContext);
        if (bShowReconnectOptions) {
            viewControl.reconnectAtLogin = wiContext.getUserPreferences().getReconnectAtLoginAction();
        }

        // Display Section
        // Language selection box
        LanguageManager langmgr = Include.getLanguageManager(wiContext.getStaticEnvironmentAdaptor());
        LanguagePack[] packs = langmgr.getLanguagePacks();
        boolean bShowLanguageOptions = allowCustomizeLanguages(wiContext);
        
        CommonDisplaySettingsControl displaySettings = viewControl.getDisplaySettings();
        displaySettings.setShowLanguageOptions(bShowLanguageOptions);
        displaySettings.setLanguages(packs);
        displaySettings.setSelectedLanguage(langmgr.getLanguagePack(wiContext.getCurrentLocale()));
        displaySettings.setShowLayoutSelection(allowCustomizeLayout(wiContext.getConfiguration().getUIConfiguration()));
        displaySettings.setLayoutType(wiContext.getUserPreferences().getLayoutType());
        
        viewControl.showDisplaySection = bShowLanguageOptions;

        ClientSettingsUtils clientSettingsUtils = new ClientSettingsUtils(wiContext);
        clientSettingsUtils.setupViewControl(viewControl.clientSettingsControl);
    }

    /**
     * Whether the user is allowed to customize any settings on this page.
     *
     * @param wiContext the Web Interface context object
     * @return <code>true</code> if any settings can be customized, otherwise <code>false</code>
     */
    public static boolean hasVisibleSettings(WIContext wiContext) {
        // Calculate which settings are available, but only if the user
        // is allowed to change settings, and we are not in simple layout mode
        return
            wiContext.getConfiguration().getAllowCustomizeSettings() &&
            !Settings.isSimpleLayoutMode(wiContext) &&
            (allowCustomizeReconnectAtLogin(wiContext) ||
             allowCustomizeLanguages(wiContext) ||
             allowCustomizeLayout(wiContext.getConfiguration().getUIConfiguration()) ||
             allowCustomizeBandwidth(wiContext));
    }

    /**
     * Whether the user is allowed to customize reconnect at login behaviour.
     *
     * The button is shown when workspace control is enabled, or when
     * the client wizard is not supported (i.e. there is a client drop down).
     *
     * @param wiContext the Web Interface context object
     * @return <code>true</code> if the setting can be customized, otherwise <code>false</code>
     */
    public static boolean allowCustomizeReconnectAtLogin(WIContext wiContext) {
        WorkspaceControlConfiguration wscConfig = wiContext.getConfiguration().getWorkspaceControlConfiguration();
        return
            (wscConfig.getAllowCustomizeReconnectAtLogin() &&
             !LaunchUtilities.getDirectLaunchModeInUse(wiContext) &&
             (Include.isWorkspaceControlEnabled(wiContext) || !Include.clientWizardSupported(wiContext)));
    }

    /**
     * Whether the user is allowed to customize the display language.
     *
     * @param wiContext the Web Interface context object
     * @return <code>true</code> if the setting can be customized, otherwise <code>false</code>
     */
    public static boolean allowCustomizeLanguages(WIContext wiContext) {
        LanguageManager langmgr = Include.getLanguageManager(wiContext.getStaticEnvironmentAdaptor());
        LanguagePack[] packs = langmgr.getLanguagePacks();
        return (packs.length > 1);
    }
    
    /**
     * Whether the user is allowed to customize the display language.
     *
     * @param uiConfig the UI configuration
     * @return <code>true</code> if the setting can be customized, otherwise <code>false</code>
     */
    public static boolean allowCustomizeLayout(UIConfiguration uiConfig) {
        return uiConfig.getAllowCustomizeLayout();
    }

    /**
     * Whether the user is allowed to customize bandwidth.
     *
     * @param wiContext the Web Interface context object
     * @return <code>true</code> if the setting can be customized, otherwise <code>false</code>
     */
    public static boolean allowCustomizeBandwidth(WIContext wiContext) {
        // Bandwidth control comes from session settings
        AppAccessMethodConfiguration aamconfig = wiContext.getConfiguration().getAppAccessMethodConfiguration();
        ClientConnectionConfiguration clientConnectionConfiguration = wiContext.getConfiguration().getClientConnectionConfiguration();
        PerformanceSettingsPolicy performancePolicy = new PerformanceSettingsPolicy(clientConnectionConfiguration);
        
        return aamconfig.isEnabledAppAccessMethod(AppAccessMethod.REMOTE) &&
            performancePolicy.canUserCustomizeBandwidth();
    }
}
