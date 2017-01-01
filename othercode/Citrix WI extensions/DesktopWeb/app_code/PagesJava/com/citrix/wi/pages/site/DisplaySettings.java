/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.site;

import java.util.Locale;

import com.citrix.wi.UserPreferences;
import com.citrix.wi.config.WIConfiguration;
import com.citrix.wi.controls.CommonDisplaySettingsControl;
import com.citrix.wi.controls.DisplaySettingsPageControl;
import com.citrix.wi.localization.LanguagePack;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pages.site.preferences.policies.LanguageSettingsPolicy;
import com.citrix.wi.pages.site.preferences.policies.UISettingsPolicy;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.types.LayoutType;
import com.citrix.wing.util.Locales;
import com.citrix.wing.util.Strings;
import com.citrix.wing.webpn.UserContext;

/*
 * This page contains the following settings:
 * 
 * - Language - Layout - Hints - Remember last folder - Search
 */
public class DisplaySettings extends PreferencesSection {

    SiteSettings           settings         = new SiteSettings();
    LanguageSettings       languageSettings = new LanguageSettings();
    UISettingsPolicy       uiSettingsPolicy;
    LanguageSettingsPolicy languageSettingsPolicy;

    public DisplaySettings(WIContext wiContext, CommonDisplaySettingsControl viewControl) {
        super(wiContext, viewControl);

        uiSettingsPolicy = new UISettingsPolicy(wiContext, wiContext.getConfiguration().getUIConfiguration());

        languageSettings.packs = Include.getLanguageManager(wiContext.getStaticEnvironmentAdaptor()).getLanguagePacks();
        languageSettingsPolicy = new LanguageSettingsPolicy(wiContext);
    }

    public boolean isVisible() {
        WIConfiguration config = wiContext.getConfiguration();
        return config.getAllowCustomizeSettings() && 
               languageSettingsPolicy.hasVisibleSettings(config) && 
               uiSettingsPolicy.hasVisibleSettings(config);
    }

    protected void getDataFromUserPreferences(UserPreferences userPreferences) {
        settings.rememberLastFolder = (!(java.lang.Boolean.FALSE.equals(userPreferences.getRememberFolder())));

        if (uiSettingsPolicy.userCanCustomizeLayout()) {
            settings.layoutType = userPreferences.getLayoutType().toString();
        }

        if (uiSettingsPolicy.userCanCustomizeSearchDisplay()) {
            settings.showSearch = (!(java.lang.Boolean.FALSE.equals(userPreferences.getShowSearch())));
        }

        if (uiSettingsPolicy.userCanCustomizeHintsDisplay()) {
            settings.showHints = (!(java.lang.Boolean.FALSE.equals(userPreferences.getShowHints())));
        }
    }

    protected boolean isDataValid() {
        String rememberLastFolder = web.getFormParameter(Constants.ID_CHECK_REMEMBER_FOLDER);
        if (!isCheckboxValueValid(rememberLastFolder)) {
            return false;
        }

        String showSearch = web.getFormParameter(Constants.ID_CHECK_SHOW_SEARCH);
        if (!isCheckboxValueValid(showSearch)) {
            return false;
        }

        String showHints = web.getFormParameter(Constants.ID_CHECK_SHOW_HINTS);
        if (!isCheckboxValueValid(showHints)) {
            return false;
        }

        String layoutType = web.getFormParameter(Constants.ID_OPTION_LAYOUT_TYPE);
        if (uiSettingsPolicy.userCanCustomizeLayout() && !validateInputLayoutType(layoutType)) {
            return false;
        }

        String language = web.getFormParameter(Constants.ID_OPTION_LANGUAGE);
        Locale locale = Locales.fromString(language);
        if (languageSettingsPolicy.userCanCustomizeLanguage() && !validateLocale(locale)) {
            return false;
        }

        return true;
    }

    protected void savePreferences(UserContext userContext, UserPreferences userPreferences) {
        if (uiSettingsPolicy.userCanCustomizeRememberLastFolderOption()) {
            boolean rememberLastFolder = Constants.VAL_ON.equalsIgnoreCase(web
                            .getFormParameter(Constants.ID_CHECK_REMEMBER_FOLDER));
            userPreferences.setRememberFolder(new java.lang.Boolean(rememberLastFolder));
        }

        if (uiSettingsPolicy.userCanCustomizeLayout()) {
            String layoutType = web.getFormParameter(Constants.ID_OPTION_LAYOUT_TYPE);
            userPreferences.setLayoutType(LayoutType.fromString(layoutType));
        }

        if (languageSettingsPolicy.userCanCustomizeLanguage()) {
            String language = web.getFormParameter(Constants.ID_OPTION_LANGUAGE);
            Locale locale = Locales.fromString(language);
            userPreferences.setLocale(locale);
        }

        if (uiSettingsPolicy.userCanCustomizeSearchDisplay()) {
            boolean showSearch = Constants.VAL_ON
                            .equalsIgnoreCase(web.getFormParameter(Constants.ID_CHECK_SHOW_SEARCH));
            userPreferences.setShowSearch(new java.lang.Boolean(showSearch));
        }

        if (uiSettingsPolicy.userCanCustomizeHintsDisplay()) {
            boolean showHints = Constants.VAL_ON.equalsIgnoreCase(web.getFormParameter(Constants.ID_CHECK_SHOW_HINTS));
            userPreferences.setShowHints(new java.lang.Boolean(showHints));
        }
    }

    protected void setupView() {
        DisplaySettingsPageControl viewControl = (DisplaySettingsPageControl)this.viewControl;
        viewControl.setShowRememberFolderOption(uiSettingsPolicy.userCanCustomizeRememberLastFolderOption());
        if (uiSettingsPolicy.userCanCustomizeRememberLastFolderOption()) {
            viewControl.setRememberFolder(settings.rememberLastFolder);
        }

        viewControl.setShowLayoutSelection(uiSettingsPolicy.userCanCustomizeLayout());
        if (uiSettingsPolicy.userCanCustomizeLayout()) {
            viewControl.setLayoutType(LayoutType.fromString(settings.layoutType));
        }

        viewControl.setShowLanguageOptions(languageSettingsPolicy.userCanCustomizeLanguage());
        if (languageSettingsPolicy.userCanCustomizeLanguage()) {
            viewControl.setLanguages(languageSettings.packs);
            viewControl.setSelectedLanguage(Include.getLanguageManager(wiContext.getStaticEnvironmentAdaptor())
                            .getLanguagePack(wiContext.getCurrentLocale()));
        }

        viewControl.setShowSearchDisplayOption(uiSettingsPolicy.userCanCustomizeSearchDisplay());
        if (uiSettingsPolicy.userCanCustomizeSearchDisplay()) {
            viewControl.setShowSearch(settings.showSearch);
        }

        viewControl.setShowHintsDisplayOption(uiSettingsPolicy.userCanCustomizeHintsDisplay());
        if (uiSettingsPolicy.userCanCustomizeHintsDisplay()) {
            viewControl.setShowHints(settings.showHints);
        }
    }

    private boolean validateInputLayoutType(String layoutType) {
        if (Strings.isEmptyOrWhiteSpace(layoutType)) {
            // If the value is null, empty or whitespace, the user didn't want
            // to change the setting. Exit early and return true to indicate
            // that this is a valid response.
            return true;
        }

        return Constants.VAL_LAYOUT_TYPE_AUTO.equals(layoutType) || Constants.VAL_LAYOUT_TYPE_NORMAL.equals(layoutType)
                        || Constants.VAL_LAYOUT_TYPE_COMPACT.equals(layoutType);
    }

    private boolean validateLocale(Locale locale) {
        if (locale == null) {
            // If the value is null, the user didn't want to change the setting.
            // Exit early and return true to indicate that this is a valid
            // response.
            return true;
        }

        return Include.getLanguageManager(wiContext.getStaticEnvironmentAdaptor()).isLanguageAvailable(locale);
    }
}

class SiteSettings {
    public boolean rememberLastFolder = false;
    public String  layoutType         = null;
    public boolean showSearch         = false;
    public boolean showHints          = false;
}

class LanguageSettings {
    public String         languageStr = null;
    public Locale         locale      = null;
    public LanguagePack[] packs       = null;
}