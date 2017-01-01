/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.site.preferences.policies;

import com.citrix.wi.config.WIConfiguration;
import com.citrix.wi.config.WIUIConfiguration;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.AGEUtilities;

/**
 * Defines the policy governing the display of UI settings.  
 */
public class UISettingsPolicy implements PreferencesSectionPolicy {
	private WIUIConfiguration uiConfig;
	private WIContext wiContext;

	/**
	 * Generates a new UISettingsPolicy from the provided configuration.
	 * @param wiContext
	 * @param uiConfig The UI configuration for the site.
	 */
	public UISettingsPolicy(WIContext wiContext, WIUIConfiguration uiConfig) {
		this.uiConfig = uiConfig;
		this.wiContext = wiContext;
	}

	/**
	 * Whether the policy allows any settings to be customized.  
	 * 
	 * @param config the Web Interface configuration
	 * @return <code>true</code> if any settings can be customized, otherwise
	 * <code>false</code>
	 */
	public boolean hasVisibleSettings(WIConfiguration config) {
		return canUserCustomizeRememberFolderOption() ||
			   userCanCustomizeLayout() ||
			   userCanCustomizeRememberLastFolderOption() ||
			   userCanCustomizeSearchDisplay() ||
			   userCanCustomizeHintsDisplay();
	}

	/**
	 * Whether the user is allowed to customize the remember folder option.
	 * 
	 * @return <code>true</code> if the setting can be customized, otherwise
	 * <code>false</code>
	 */
	public boolean canUserCustomizeRememberFolderOption() {
		return uiConfig.getAllowCustomizePersistFolderLocation();
	}

	/**
	 * Whether the user is allowed to customize layout.
	 * 
	 * @return <code>true</code> if the setting can be customized, otherwise
	 * <code>false</code>
	 */
	public boolean userCanCustomizeLayout() {
		return !AGEUtilities.isAGEEmbeddedMode(wiContext)
						&& uiConfig.getAllowCustomizeLayout();
	}

	/**
	 * Whether the user is allowed to customize the display of the search
	 * feature.
	 * 
	 * @return <code>true</code> if the setting can be customized, otherwise
	 * <code>false</code>
	 */
	public boolean userCanCustomizeSearchDisplay() {
		return !AGEUtilities.isAGEEmbeddedMode(wiContext)
						&& uiConfig.getAllowCustomizeShowSearch();
	}

	/**
	 * Whether the user is allowed to customize the display of the hints
	 * feature.
	 * 
	 * @return <code>true</code> if the setting can be customized, otherwise
	 * <code>false</code>
	 */
	public boolean userCanCustomizeHintsDisplay() {
		return !AGEUtilities.isAGEEmbeddedMode(wiContext)
						&& uiConfig.getAllowCustomizeShowHints();
	}

	/**
	 * Whether the user is allowed to customize the remember last folder option.
	 * 
	 * @return <code>true</code> if the setting can be customized, otherwise
	 * <code>false</code>
	 */
	public boolean userCanCustomizeRememberLastFolderOption() {
		return uiConfig.getAllowCustomizePersistFolderLocation();
	}
}
