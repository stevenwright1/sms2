/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.site;

import com.citrix.wi.UserPreferences;
import com.citrix.wi.controls.ClientSettingsControl;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.ClientSettingsUtils;
import com.citrix.wing.webpn.UserContext;

/**
 * Provides a mechanism for the end user to customise their Citrix client(s).
 */
public class ClientSettings extends PreferencesSection {

	private ClientSettingsUtils clientSettingsUtils;

	public ClientSettings(WIContext wiContext, ClientSettingsControl viewControl) {
		super(wiContext, viewControl);

		clientSettingsUtils = new ClientSettingsUtils(wiContext);
	}

	/**
	 * Whether the user is allowed to customize any client settings.
	 *
	 * @return <code>true</code> if the settings can be customized, otherwise <code>false</code>
	 */
	public boolean userCanCustomizeClientSettings() {
		return clientSettingsUtils.anyCustomizableSettings();
	}

    /* (non-Javadoc)
     * @see com.citrix.wi.pages.site.PreferencesSection#isVisible()
     */
    public boolean isVisible() {
        return userCanCustomizeClientSettings();
    }

	/* (non-Javadoc)
	 * @see com.citrix.wi.pages.site.PreferencesSection#getDataFromUserPreferences(com.citrix.wi.UserPreferences)
	 */
	protected void getDataFromUserPreferences(UserPreferences userPreferences) {
		// Client data is not retrieved from user preferences as it is not displayed
		// to the user.
	}

	/* (non-Javadoc)
	 * @see com.citrix.wi.pages.site.PreferencesSection#isDataValid()
	 */
	protected boolean isDataValid() {
		// The data is automatically calculated so is assumed to be valid.
		return true;
	}

	/* (non-Javadoc)
	 * @see com.citrix.wi.pages.site.PreferencesSection#savePreferences()
	 */
	protected void savePreferences(UserContext userContext, UserPreferences userPreferences) {
		clientSettingsUtils.processPostRequest();
		clientSettingsUtils.savePreferences(userContext, userPreferences);
	}

	protected void setupView() {
		clientSettingsUtils.setupViewControl((ClientSettingsControl)viewControl);
	}
}
