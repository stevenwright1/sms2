/*
 * Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;

import com.citrix.wi.config.ClientConnectionConfiguration;
import com.citrix.wi.config.WIConfiguration;
import com.citrix.wi.controls.AccountSettingsPageControl;
import com.citrix.wi.controls.ClientSettingsControl;
import com.citrix.wi.controls.DisplaySettingsPageControl;
import com.citrix.wi.controls.SessionSettingsPageControl;
import com.citrix.wi.controls.WorkspaceControlSettingsViewControl;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pages.site.AccountSettings;
import com.citrix.wi.pages.site.ClientSettings;
import com.citrix.wi.pages.site.DisplaySettings;
import com.citrix.wi.pages.site.PreferencesSection;
import com.citrix.wi.pages.site.SessionSettings;
import com.citrix.wi.pages.site.WorkspaceControlSettings;
import com.citrix.wi.pageutils.clientdetection.DetectionUtils;
import com.citrix.wi.types.LayoutMode;
import com.citrix.wi.types.ReconnectOption;

/**
 * Helper class to determine which Settings pages should be shown
 */
public class Settings
{
    /**
     * Checks whether the user can see the Settings page
     * 
     * @param wiContext
     * @return <code>true</code> if shown, else <code>false</code>
     */
    public static boolean showPreferences(WIContext wiContext) {
        WIConfiguration config = wiContext.getConfiguration();
        
        return config.getAllowCustomizeSettings() && 
            isAnyPreferencesSectionVisible(wiContext);
    }

    /**
     * Checks whether the user can see the MessageScreen page
     *
     * @param wiContext WIContext.
     * @return <code>true</code> if shown, else <code>false</code>
     */
    public static boolean showPreLoginMessageScreen(WIContext wiContext)
    {
        boolean canShowMessagesScreen = true;

        // Check to see if the site is in Advanced layout mode.
        if (isSimpleLayoutMode(wiContext)) {
            canShowMessagesScreen = false;
        }

        // Ensure that the site is in full mode so that we have been shown the login settings
        // page.
        return canShowMessagesScreen;
    }

    /**
     * Detemine if the site is in Simple layout mode.
     *
     * i.e. Navigation bar is not visible on the Login page and pre-login Messages/Preferences
     * are not available.
     *
     * @param wiContext WIContext.
     * @return <code>true</code> if in simple layout mode, else <code>false</code>
     */
    public static boolean isSimpleLayoutMode(WIContext wiContext)
    {
        return (LayoutMode.SIMPLE == wiContext.getConfiguration().getUIConfiguration().getSiteLayoutMode());
    }

    /**
     * Tests whether the bandwidth customization settings should be shown.
     * @return <code>true</code> if they should be shown, else <code>false</code>
     */
    public static boolean getShowBandwidthCustom(WIContext wiContext) {
        ClientConnectionConfiguration config = wiContext.getConfiguration().getClientConnectionConfiguration();
        boolean showColorSettings = config.getAllowCustomizeWinColor();
        boolean showAudioSettings = config.getAllowCustomizeAudio();
        boolean showPrinterMapping = config.getAllowCustomizePrinterMapping();
        return (showColorSettings || showAudioSettings || showPrinterMapping);
    }

    /**
     * Tests whether the reconnect link should be shown (assuming the user is logged in).
     * @return <code>true</code> if it should be shown, else <code>false</code>
     */
    public static boolean getShowReconnectLink(WIContext wiContext) {
        boolean anonymousUser = Authentication.isAnonUser(wiContext.getUserEnvironmentAdaptor());
    	boolean clientsAvailable = DetectionUtils.clientsAvailableForDetection(wiContext, null);
    	boolean workspaceControlEnabled = Include.isWorkspaceControlEnabled(wiContext);
    	boolean isDirectLaunch = LaunchUtilities.getDirectLaunchModeInUse(wiContext);
    	boolean reconnectButtonHasAction = (ReconnectOption.NONE != wiContext.getUserPreferences().getReconnectButtonAction());
    	
    	return !anonymousUser && 
    	       clientsAvailable &&
               workspaceControlEnabled &&
               !isDirectLaunch && 
               reconnectButtonHasAction;
    }

    /**
     * Tests whether the disconnect link should be shown (assuming the user is logged in).
     * @return <code>true</code> if it should be shown, else <code>false</code>
     */
    public static boolean getShowDisconnectLink(WIContext wiContext) {
        boolean showLink = false;
        if (!Authentication.isAnonUser(wiContext.getUserEnvironmentAdaptor()) && 
            !LaunchUtilities.getDirectLaunchModeInUse(wiContext)) {
            showLink = Include.isWorkspaceControlEnabled(wiContext);
        }

        return showLink;
    }

    /**
     * Tests whether the "Change View" dropdown is shown.
     * @return <code>true</code> if it should be shown, else <code>false</code>
     */
    public static boolean getShowChangeViewDropdown(WIContext wiContext) {
        return (wiContext.getConfiguration().getUIConfiguration().getViewStyles().size() > 1);
    }

    /**
     * Tests whether Change View link can be shown for the low graphics version.
     * @return <code>true</code> if it should be shown, else <code>false</code>
     */
    public static boolean getShowChangeViewLink(WIContext wiContext) {
        return (wiContext.getConfiguration().getUIConfiguration().getCompactViewStyles().size() > 1);
    }
    
    private static List getPreferencesSections(WIContext wiContext) {
        List preferencesSections = new ArrayList();
        
        preferencesSections.add(new AccountSettings(wiContext, new AccountSettingsPageControl()));
        preferencesSections.add(new ClientSettings(wiContext, new ClientSettingsControl()));
        preferencesSections.add(new DisplaySettings(wiContext, new DisplaySettingsPageControl()));
        preferencesSections.add(new SessionSettings(wiContext, new SessionSettingsPageControl()));
        preferencesSections.add(new WorkspaceControlSettings(wiContext, new WorkspaceControlSettingsViewControl()));   
    
        return preferencesSections;
    }
    
    private static boolean isAnyPreferencesSectionVisible(WIContext wiContext) {
        Iterator preferencesSections = getPreferencesSections(wiContext).iterator();
        
        boolean anySectionVisible = false;
        while (preferencesSections.hasNext() && !anySectionVisible) {
            anySectionVisible = ((PreferencesSection)preferencesSections.next()).isVisible();
        }
        
        return anySectionVisible;
    }
}
