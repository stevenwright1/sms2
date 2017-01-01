/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.site;

import com.citrix.wi.UserPreferences;
import com.citrix.wi.controls.WorkspaceControlSettingsViewControl;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pages.site.preferences.policies.WorkspaceControlSettingsPolicy;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.types.ReconnectOption;
import com.citrix.wing.webpn.UserContext;

/**
 * Allows the user to configure Workspace Control settings. 
 */
public class WorkspaceControlSettings extends PreferencesSection {
    
    private WorkspaceControlSettingsPolicy policy;
    
    private String reconnectAtLogin;
    private ReconnectOption reconnectOption;
    private boolean logOffAppsWithWI;
    
    private int workspaceControlLevel;
    
    /**
     * Creates a new WorkspaceControlSettings object
     * 
     * @param wiContext The Web Interface Context object
     * @param viewControl A WorkspaceControlSettingsViewControl object, to display 
     * workspace control settings to the end user.
     */
    public WorkspaceControlSettings(WIContext wiContext, WorkspaceControlSettingsViewControl viewControl) {
        super(wiContext, viewControl);
        policy = new WorkspaceControlSettingsPolicy(wiContext, 
                        wiContext.getConfiguration().getWorkspaceControlConfiguration());
        
        workspaceControlLevel = Include.getWorkspaceControlLevel(wiContext);
    }

    /* (non-Javadoc)
     * @see com.citrix.wi.pages.site.PreferencesSection#isVisible()
     */
    public boolean isVisible() {
        return policy.hasVisibleSettings(wiContext.getConfiguration());
    }

    /* (non-Javadoc)
     * @see com.citrix.wi.pages.site.PreferencesSection#getDataFromUserPreferences(com.citrix.wi.UserPreferences)
     */
    protected void getDataFromUserPreferences(UserPreferences userPreferences) {
        reconnectAtLogin = userPreferences.getReconnectAtLoginAction().toString();
        reconnectOption = userPreferences.getReconnectButtonAction();
        logOffAppsWithWI = !java.lang.Boolean.FALSE.equals(userPreferences.getLogoffApps());        
    }

    /* (non-Javadoc)
     * @see com.citrix.wi.pages.site.PreferencesSection#isDataValid()
     */
    protected boolean isDataValid() {
        String reconnectAtLoginValue = web.getFormParameter(Constants.ID_CHECK_RECONNECT_LOGIN);
        if (!isCheckboxValueValid(reconnectAtLoginValue))  {
            return false;
        }

		String reconnectAtLoginAction = web.getFormParameter(Constants.ID_OPTION_RECONNECT_LOGIN);
		boolean reconnectAtLoginEnabled = Constants.VAL_ON.equalsIgnoreCase(reconnectAtLoginValue);
		if (!validateReconnectAction(reconnectAtLoginAction, reconnectAtLoginEnabled)) {
            return false;
        }
        
        String logoffAppsValue = web.getFormParameter(Constants.ID_CHECK_LOGOFF);
        if (!isCheckboxValueValid(logoffAppsValue)) {
            return false;
        }
        
        String reconnectButtonValue = web.getFormParameter(Constants.ID_CHECK_RECONNECT_BUTTON);
        if (!isCheckboxValueValid(reconnectButtonValue)) {
            return false;
        }

		String reconnectButtonAction = web.getFormParameter(Constants.ID_OPTION_RECONNECT_BUTTON);
		boolean reconnectButtonEnabled = Constants.VAL_ON.equalsIgnoreCase(reconnectButtonValue);
		if (!validateReconnectAction(reconnectButtonAction, reconnectButtonEnabled)) {
			return false;
		}
        
        return true;
    }

    /* (non-Javadoc)
     * @see com.citrix.wi.pages.site.PreferencesSection#savePreferences(com.citrix.wing.webpn.UserContext)
     */
    protected void savePreferences(UserContext userContext, UserPreferences userPreferences) {
        if (!Include.isWorkspaceControlEnabled(wiContext)) {
            return;
        }
        
        // Workspace control
        userPreferences.setReconnectAtLoginAction(null);
        userPreferences.setLogoffApps(null);

        ReconnectOption reconnectAtLoginAction = parseReconnectOption(policy.userCanCustomizeReconnectAtLogin(), 
                                                                      Constants.ID_OPTION_RECONNECT_LOGIN, 
                                                                      Constants.ID_CHECK_RECONNECT_LOGIN);
        userPreferences.setReconnectAtLoginAction(reconnectAtLoginAction);

        if (policy.userCanCustomizeLogOff()) {
            logOffAppsWithWI = Constants.VAL_ON.equalsIgnoreCase(web.getFormParameter(Constants.ID_CHECK_LOGOFF));
            userPreferences.setLogoffApps(new java.lang.Boolean(logOffAppsWithWI));
        }
        
        ReconnectOption reconnectButtonAction = parseReconnectOption(policy.userCanCustomizeReconnectButtonAction(), 
                                                                     Constants.ID_OPTION_RECONNECT_BUTTON, 
                                                                     Constants.ID_CHECK_RECONNECT_BUTTON);        
        userPreferences.setReconnectButtonAction(reconnectButtonAction);
    }

    /* (non-Javadoc)
     * @see com.citrix.wi.pages.site.PreferencesSection#setupView()
     */
    protected void setupView() {
        WorkspaceControlSettingsViewControl viewControl = (WorkspaceControlSettingsViewControl)this.viewControl;
        
        if (policy.userCanCustomizeWorkspaceControl()) {
            viewControl.setShowReconnectAtLoginSettings(policy.userCanCustomizeReconnectAtLogin());
            if (policy.userCanCustomizeReconnectAtLogin()) {
                if (Constants.VAL_NONE.equalsIgnoreCase(reconnectAtLogin)) {
                    viewControl.setReconnectAtLogin(ReconnectOption.NONE);
                } else if (Constants.VAL_DISCONNECTED_ACTIVE.equalsIgnoreCase(reconnectAtLogin)) {
                    viewControl.setReconnectAtLogin(ReconnectOption.DISCONNECTED_AND_ACTIVE);
                } else if (Constants.VAL_DISCONNECTED.equalsIgnoreCase(reconnectAtLogin)) {
                    viewControl.setReconnectAtLogin(ReconnectOption.DISCONNECTED);
                }
            }

            viewControl.setShowLogoffSettings(policy.userCanCustomizeLogOff());
            if (policy.userCanCustomizeLogOff()) {
                viewControl.setLogoffApps(logOffAppsWithWI);
            }
            
            // Reconnect button
            viewControl.setShowReconnectButtonSettings(policy.userCanCustomizeReconnectButtonAction());
            viewControl.workspaceControlLevel = workspaceControlLevel;
            viewControl.setReconnectButton(reconnectOption);            
        }
    }

    private boolean validateReconnectAction(String reconnectAction, boolean reconnectActionEnabled) {
        boolean isAnonymousUser = Authentication.isAnonUser(wiContext.getUserEnvironmentAdaptor());

		if (!isAnonymousUser && policy.userCanCustomizeReconnectAtLogin() && reconnectActionEnabled) {
            return Constants.VAL_DISCONNECTED_ACTIVE.equalsIgnoreCase(reconnectAction) ||
                   Constants.VAL_DISCONNECTED.equalsIgnoreCase(reconnectAction);
        }
        
        // The user could not specify the reconnect action, so it is valid by default.        
        return true;
    }

    private ReconnectOption parseReconnectOption(boolean userCanCustomizeReconnectAction, String reconnectActionId, String reconnectActionConfiguredId) {
        ReconnectOption reconnectOption = null;
        
        boolean isAnonymousUser = Authentication.isAnonUser(wiContext.getUserEnvironmentAdaptor());
        if (!isAnonymousUser && userCanCustomizeReconnectAction) {
            String reconnectAtLoginAction = web.getFormParameter(reconnectActionId);
            boolean isReconnectAtLoginConfigured = Constants.VAL_ON.equalsIgnoreCase(web.getFormParameter(reconnectActionConfiguredId));
            
            reconnectOption = isReconnectAtLoginConfigured ? ReconnectOption.fromString(reconnectAtLoginAction) : ReconnectOption.NONE;            
        }
        
        return reconnectOption;
    }
}
