/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.controls;

/**
 * This class composes the {@link AccountSettingsPageControl}, 
 * {@link ClientSettingsControl}, {@link DisplaySettingsPageControl},
 * {@link SessionSettingsPageControl} and {@link WorkspaceControlSettingsViewControl} 
 * classes into a single object for display in a single page.  
 */
public class PreferencesPageControl extends PageControl {

    private AccountSettingsPageControl accountSettings = new AccountSettingsPageControl();
    private ClientSettingsControl clientSettings = new ClientSettingsControl();
    private DisplaySettingsPageControl displaySettings = new DisplaySettingsPageControl();
    private SessionSettingsPageControl sessionSettings = new SessionSettingsPageControl();
    private WorkspaceControlSettingsViewControl workspaceControlSettings = new WorkspaceControlSettingsViewControl();
    
    /**
     * @return the accountSettings
     */
    public AccountSettingsPageControl getAccountSettings() {
        return accountSettings;
    }
        
    /**
     * @return the displaySettings
     */
    public DisplaySettingsPageControl getDisplaySettings() {
        return displaySettings;
    }
    
    /**
     * @return the sessionSettings
     */
    public SessionSettingsPageControl getSessionSettings() {
        return sessionSettings;
    }
    
    /**
     * @return the clientSettings
     */
    public ClientSettingsControl getClientSettings() {
        return clientSettings;
    }
    
    /**
     * @return the workspaceControlSettings
     */
    public WorkspaceControlSettingsViewControl getWorkspaceControlSettings() {
        return workspaceControlSettings;
    }
    
    /**
     * Determines whether or not the General section should be displayed.  
     * 
     * @return <code>true</code> if the General section should be displayed; 
     * else <code>false</code>.
     */
    public boolean getShowGeneralSection() {
        return displaySettings.getShowDisplaySettings() ||
               workspaceControlSettings.getShowWorkspaceControlSettings() ||
               accountSettings.getShowAutoLoginSettings() ||
               clientSettings.getShowClientSettings();
    }
}
