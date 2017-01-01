/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.site;

import com.citrix.wi.UserPreferences;
import com.citrix.wi.config.AuthenticationConfiguration;
import com.citrix.wi.config.auth.WIAuthPoint;
import com.citrix.wi.controls.AccountSettingsPageControl;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.types.AllowChangePassword;
import com.citrix.wi.types.WIAuthType;
import com.citrix.wi.util.ClientInfo;
import com.citrix.wing.webpn.UserContext;

/*
 * This section handles the following settings:
 *
 * - Change password link 
 * - Silent authentication
 */
public class AccountSettings extends PreferencesSection {

    // User's preferences
    private boolean useSilentAuthentication = false;

    public AccountSettings(WIContext wiContext, AccountSettingsPageControl viewControl) {
        super(wiContext, viewControl);
    }

    /**
     * Whether the user is allowed to change their password.
     *
     * @param wiContext the Web Interface context object
     * @return <code>true</code> if the password can be changed, otherwise
     * <code>false</code>
     */
    public static boolean allowChangePassword(WIContext wiContext) {
        return (AllowChangePassword.ALWAYS == Authentication.getChangePasswordPolicy(wiContext));
    }

    /**
     * Whether the user is allowed to customize auto login.
     *
     * @param wiContext the Web Interface context object
     * @return <code>true</code> if the setting can be customized, otherwise
     * <code>false</code>
     */
    public static boolean allowCustomizeAutoLogin(WIContext wiContext) {
        AuthenticationConfiguration authConfig = wiContext.getConfiguration().getAuthenticationConfiguration();
        
        if (isAutoLoginConfigured(authConfig)) {            
            WIAuthPoint wiAuthPoint = (WIAuthPoint)authConfig.getAuthPoint();            
            ClientInfo clientInfo = wiContext.getClientInfo();
            
            return (isSupportedBrowserForAutoLogin(clientInfo) &&
                    isSupportedAuthMethodForAutoLogin(wiAuthPoint));
        }

        return false;
    }

	public static boolean hasVisibleSettings(WIContext wiContext) {
	    boolean userCanCustomizeSettings = wiContext.getConfiguration().getAllowCustomizeSettings();
	    return userCanCustomizeSettings &&
               (allowChangePassword(wiContext) || 
                allowCustomizeAutoLogin(wiContext));
	}

	public boolean isVisible() {
        return hasVisibleSettings(wiContext);
    }

    protected void getDataFromUserPreferences(UserPreferences userPreferences) {
        useSilentAuthentication = !java.lang.Boolean.FALSE.equals(userPreferences.getUseSilentAuth());
    }

    protected boolean isDataValid() {
        String silentAuthenticationValue = web.getFormParameter(Constants.ID_CHECK_SILENT_AUTHENTICATION);
        if (!isCheckboxValueValid(silentAuthenticationValue))  {
            return false;
        }
        
        return true;
    }

    protected void savePreferences(UserContext userContext, UserPreferences newUserPrefs) {
        if (allowCustomizeAutoLogin(wiContext)) {
            useSilentAuthentication = Constants.VAL_ON.equalsIgnoreCase(web.getFormParameter(Constants.ID_CHECK_SILENT_AUTHENTICATION));
            newUserPrefs.setUseSilentAuth(new java.lang.Boolean(useSilentAuthentication));
        }        
    }
    
    protected void setupView() {
        AccountSettingsPageControl viewControl = (AccountSettingsPageControl)this.viewControl;
        
        // Password section
        viewControl.setShowPasswordSection(allowChangePassword(wiContext));

        // Login/out options section
        boolean showAutoLogin = allowCustomizeAutoLogin(wiContext);
        viewControl.setShowAutoLoginSettings(showAutoLogin);
        if (showAutoLogin) {
            viewControl.setAutoLogin(useSilentAuthentication);
        }
    }

    private static boolean isAutoLoginConfigured(AuthenticationConfiguration authConfig) {
        return authConfig.getAuthPoint() instanceof WIAuthPoint &&
               authConfig.getAllowCustomizeAutoLogin();
    }

    private static boolean isSupportedAuthMethodForAutoLogin(WIAuthPoint wiAuthPoint) {
        return wiAuthPoint.isAuthMethodEnabled(WIAuthType.CERTIFICATE) ||
               wiAuthPoint.isAuthMethodEnabled(WIAuthType.CERTIFICATE_SINGLE_SIGN_ON) ||
               wiAuthPoint.isAuthMethodEnabled(WIAuthType.SINGLE_SIGN_ON);
    }

    private static boolean isSupportedBrowserForAutoLogin(ClientInfo browser) {
        return browser.isIE() || 
               browser.isFirefox();
    }
}