/*
 * Copyright (c) 2007-2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth;

import java.io.IOException;

import com.citrix.wi.UserPreferences;
import com.citrix.wi.config.WorkspaceControlConfiguration;
import com.citrix.wi.controls.LoginSettingsPageControl;
import com.citrix.wi.controlutils.LoginSettingsUtils;
import com.citrix.wi.controlutils.MessageUtils;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.ClientSettingsUtils;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.PageHistory;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wi.types.BandwidthProfilePreference;
import com.citrix.wi.types.LayoutType;
import com.citrix.wi.types.ReconnectOption;
import com.citrix.wing.MessageType;
import com.citrix.wing.util.Locales;
import com.citrix.wing.util.Strings;

/*
 * This page contains the following settings:
 *
 * - Language
 * - Workspace control logon behaviour
 * - Connection speed (bandwidth control)
 * - Client settings
 */
public class LoginSettings extends PreLoginUIPage {
    protected LoginSettingsPageControl viewControl = new LoginSettingsPageControl();
    private ClientSettingsUtils clientSettingsUtils;

    private WorkspaceControlConfiguration wscConfig;
    private boolean bShowReconnectOptions;


    public LoginSettings (WIContext wiContext) {
        super(wiContext);
        wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
        navControl.setShowLoginSettings(true);
        navControl.setShowMessages(true);
        navControl.setLoginSettingsLinkActive(false);
        layoutControl.formAction = Constants.FORM_POSTBACK;
        layoutControl.isPreferencesPage = true;
        MessageUtils.populate(wiContext, messagesControl, Include.isLoggedIn(wiContext.getWebAbstraction()));
    }

    protected boolean performInternal() throws IOException {

        if (!LoginSettingsUtils.hasVisibleSettings(wiContext)) {
            wiContext.getWebAbstraction().clientRedirectToUrl(
                Constants.PAGE_LOGIN + UIUtils.getMessageQueryStr(MessageType.ERROR, "CustomSettingsNotAllowed")
                );
            return false;
        }

        clientSettingsUtils = new ClientSettingsUtils(wiContext);
        wscConfig = wiContext.getConfiguration().getWorkspaceControlConfiguration();
        bShowReconnectOptions = LoginSettingsUtils.allowCustomizeReconnectAtLogin(wiContext);

        boolean showForm = true;
        recordCurrentPageURL();
        String redirectQueryStr = "";
        if (wiContext.getWebAbstraction().isPostRequest()) {
            // Post request, process the submission.
            String submitMode = wiContext.getWebAbstraction().getFormParameter(Constants.ID_SUBMIT_MODE);
            if (Constants.VAL_OK.equalsIgnoreCase(submitMode)) {
                // Retrieve last saved values
                // Display section
                String languageStr = wiContext.getWebAbstraction().getFormParameter(Constants.ID_OPTION_LANGUAGE);
                String layoutTypeStr = wiContext.getWebAbstraction().getFormParameter(Constants.ID_OPTION_LAYOUT_TYPE);

                // Connection preferences
                String reconnectStr = wiContext.getWebAbstraction().getFormParameter(Constants.ID_CHECK_RECONNECT_LOGIN);
                String sessionTypeStr = wiContext.getWebAbstraction().getFormParameter(Constants.ID_OPTION_RECONNECT_LOGIN);
                String bandwidthStr = wiContext.getWebAbstraction().getFormParameter(Constants.ID_BANDWIDTH);

                // If any parameters are invalid (unknown values or values disallowed by the
                // admin) we simply ignore the parameter rather than displaying an error message.
                // This is acceptable since an invalid parameter should never arise if this
                // page is accessed via the UI.

                BandwidthProfilePreference bandwidth = null;
                if (bandwidthStr != null) {
                    bandwidth = BandwidthProfilePreference.fromString(bandwidthStr);
                }

                ReconnectOption reconnectAtLogin = null;
                if (bShowReconnectOptions) {
                    if (Strings.equalsIgnoreCase(Constants.VAL_ON, reconnectStr)) {
                        reconnectAtLogin = ReconnectOption.fromString(sessionTypeStr);
                    } else {
                        reconnectAtLogin = ReconnectOption.NONE;
                    }
                }

                java.util.Locale locale = null;
                if (languageStr != null) {
                    locale = Locales.fromString(languageStr);
                }
                
                LayoutType layoutType = null;
                if (layoutTypeStr != null) {
                    layoutType = LayoutType.fromString(layoutTypeStr);
                }

                clientSettingsUtils.processPostRequest();

                // If there were any invalid items, we will just ignore them rather than reporting
                // an error

                showForm = false;

                setLoginOptions(bandwidth, reconnectAtLogin, locale, layoutType);
                redirectQueryStr += UIUtils.getMessageQueryStr(MessageType.SUCCESS, "SettingsSaved");
            } else {
                // Cancel clicked
                showForm = false;
            }
            // Redirect to the home page irrespective of the settings are saved or cancelled.
            PageHistory.redirectToHomePage(wiContext, redirectQueryStr);

        }

        if (showForm) {
            // Prepare welcome control with default values
            welcomeControl.setTitle(wiContext.getString("ScreenTitleSettings"));
            LoginSettingsUtils.populate(wiContext, viewControl);
        }

        wiContext.getUserEnvironmentAdaptor().commitState();
        wiContext.getUserEnvironmentAdaptor().destroy();

        return showForm;
    }

    private void setLoginOptions(BandwidthProfilePreference bandwidth, ReconnectOption reconnectAtLogin, 
                                 java.util.Locale locale, LayoutType layoutType) {
        // Save any updated logon preferences.

        UserPreferences newUserPrefs = Include.getRawUserPrefs(wiContext.getUserEnvironmentAdaptor());

        clientSettingsUtils.savePreferences(null /* no context since pre-login */, newUserPrefs);

        // The client settings have been updated so we can use this utility method
        // to check if the workspace control setting should be applied to the user prefs.
        if (Include.isWorkspaceControlEnabled(wiContext)) {
            if(wscConfig.getAllowCustomizeReconnectAtLogin() && (reconnectAtLogin != null)) {
                newUserPrefs.setReconnectAtLoginAction(reconnectAtLogin);
            }
        }

        if (bandwidth != null) {
            newUserPrefs.setBandwidthBandwidthProfilePreference(bandwidth);
        }

        if (locale != null) {
            if (Include.getLanguageManager(wiContext.getStaticEnvironmentAdaptor()).isLanguageAvailable(locale)) {
                newUserPrefs.setLocale(locale);
            }
        }
        
        if (layoutType != null) {
            newUserPrefs.setLayoutType(layoutType);
        }

        Include.saveUserPrefsPreLogin(newUserPrefs, wiContext);
    }

    protected boolean performGuard() throws IOException {
        // LoginSettings page not protected against CSRF.
        return true;
    }

    protected String getBrowserPageTitleKey() {
        return "BrowserTitleLoginSettings";
    }
}
