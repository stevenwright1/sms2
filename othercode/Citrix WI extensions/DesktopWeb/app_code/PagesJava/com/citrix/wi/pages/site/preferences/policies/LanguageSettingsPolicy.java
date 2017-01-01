/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.site.preferences.policies;

import com.citrix.wi.config.WIConfiguration;
import com.citrix.wi.localization.LanguagePack;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.Include;

/**
 * Defines the policy which governs how the language settings are
 * displayed.
 */
public class LanguageSettingsPolicy implements PreferencesSectionPolicy {

	private WIContext wiContext;

	/**
	 * Generates a new LanguageSettingsPolicy object.
	 */
	public LanguageSettingsPolicy(WIContext wiContext) {
		this.wiContext = wiContext;
	}

	/* (non-Javadoc)
	 * @see com.citrix.wi.pages.site.preferences.policies.PreferencesSectionPolicy#hasVisibleSettings(com.citrix.wi.config.WIConfiguration)
	 */
	public boolean hasVisibleSettings(WIConfiguration config) {
		return userCanCustomizeLanguage();
	}

	/**
	 * Whether the user is allowed to customize the Web Interface display language.
	 * 
	 * @return <code>true</code> if the setting can be customized, otherwise
	 * <code>false</code>
	 */
	public boolean userCanCustomizeLanguage() {
		boolean result = false;

		// User must be permitted to choose their language and there must be
		// more than one language available
		if (Include.userCanSelectLanguage(wiContext.getConfiguration())) {
			LanguagePack[] packs = Include.getLanguageManager(wiContext.getStaticEnvironmentAdaptor())
							.getLanguagePacks();
			result = (packs.length > 1);
		}

		return result;
	}
}
