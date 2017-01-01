/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.site.preferences.policies;

import com.citrix.wi.config.WIConfiguration;
import com.citrix.wi.config.WorkspaceControlConfiguration;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.Include;

/**
 * Defines a policy governing the display of the Workspace Control settings.  
 */
public class WorkspaceControlSettingsPolicy implements PreferencesSectionPolicy {
    WorkspaceControlConfiguration workspaceControlConfiguration;
    WIContext wiContext;
    
    public WorkspaceControlSettingsPolicy(WIContext wiContext, WorkspaceControlConfiguration wscConfig) {
        this.wiContext = wiContext;
        workspaceControlConfiguration = wscConfig;
    }

    /**
     * Whether the user is allowed to customize reconnect at login behaviour.
     *
     * @return <code>true</code> if the behaviour can be customized, otherwise
     * <code>false</code>
     */
    public boolean userCanCustomizeReconnectAtLogin() {
        return (Include.isWorkspaceControlEnabled(wiContext) && 
                !Authentication.isAnonUser(wiContext.getUserEnvironmentAdaptor()) && 
                workspaceControlConfiguration.getAllowCustomizeReconnectAtLogin());
    }

    /**
     * Whether the user is allowed to customize logoff behaviour.
     *
     * @return <code>true</code> if the behaviour can be customized, otherwise
     * <code>false</code>
     */
    public boolean userCanCustomizeLogOff() {
        return Include.isWorkspaceControlEnabled(wiContext) && 
               workspaceControlConfiguration.getAllowCustomizeLogOff();
    }
    
    /**
     * Whether the user can customize any workspace control settings.
     * 
     * @return <code>true</code> if workspace control can be customized at all,
     * else <code>false</code>.
     */
    public boolean userCanCustomizeWorkspaceControl() {
        return userCanCustomizeLogOff() || userCanCustomizeReconnectAtLogin();
    }

    /**
     * Whether the user is allowed to customize the behaviour of the reconnect
     * button.
     *
     * The button is shown when workspace control is enabled, or when
     * the client wizard is not supported (i.e. there is a client drop down).
     *
     * @return <code>true</code> if the behaviour can be customized, otherwise <code>false</code>
     */
    public boolean userCanCustomizeReconnectButtonAction() {
        return workspaceControlConfiguration.getAllowCustomizeReconnectButton() && 
               !Authentication.isAnonUser(wiContext.getUserEnvironmentAdaptor()) && 
               (Include.isWorkspaceControlEnabled(wiContext) || 
                !Include.clientWizardSupported(wiContext));
    }
    
    /* (non-Javadoc)
     * @see com.citrix.wi.pages.site.preferences.policies.PreferencesSectionPolicy#hasVisibleSettings(com.citrix.wi.config.WIConfiguration)
     */
    public boolean hasVisibleSettings(WIConfiguration config) {
        return userCanCustomizeReconnectAtLogin() ||
               userCanCustomizeLogOff() ||
               userCanCustomizeReconnectButtonAction();
    }
}
