/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.site.preferences.policies;

import com.citrix.wi.config.ClientConnectionConfiguration;
import com.citrix.wi.config.WIConfiguration;
import com.citrix.wing.types.AppAccessMethod;

/**
 * Defines the policy governing the display of the performance settings.
 */
public class PerformanceSettingsPolicy implements PreferencesSectionPolicy {
	private ClientConnectionConfiguration clientConnectionConfiguration;

	/**
	 * Creates a new PerformanceSettingsPolicy object from the provided
	 * ClientConnectionConfiguration.
	 *
	 * @param policyConfiguration the ClientConnectionConfiguration used to 
	 * configure the performance settings' policy.
	 */
	public PerformanceSettingsPolicy(ClientConnectionConfiguration policyConfiguration) {
		this.clientConnectionConfiguration = policyConfiguration;
	}

	/**
	 * Whether the user is allowed to customize bandwidth control.
	 *
	 * @return <code>true</code> if the setting can be customized, otherwise <code>false</code>
	 */
	public boolean canUserCustomizeBandwidth() {
		return clientConnectionConfiguration.getAllowCustomizeBandwidth();
	}

	/**
	 * Whether the user is allowed to customize window color.
	 *
	 * @return <code>true</code> if the setting can be customized, otherwise <code>false</code>
	 */
	public boolean canUserCustomizeColorDepth() {
		return clientConnectionConfiguration.getAllowCustomizeWinColor();
	}

	/**
	 * Whether the user is allowed to customize audio.
	 *
	 * @return <code>true</code> if the setting can be customized, otherwise <code>false</code>
	 */
	public boolean canUserCustomizeAudioQuality() {
		return clientConnectionConfiguration.getAllowCustomizeAudio();
	}

	/**
	 * Whether the user is allowed to customize printer mapping.
	 *
	 * @return <code>true</code> if the setting can be customized, otherwise <code>false</code>
	 */
	public boolean canUserCustomizePrinterMapping() {
		return clientConnectionConfiguration.getAllowCustomizePrinterMapping();
	}

	/**
	 * Whether the user is allowed to customize window size.
	 *
	 * @return <code>true</code> if the setting can be customized, otherwise <code>false</code>
	 */
	public boolean canUserCustomizeWindowSize() {
		return clientConnectionConfiguration.getAllowCustomizeWinSize();
	}

	/**
	 * Whether the policy allows any settings to be customized.  
	 * 
	 * @param config the Web Interface configuration
	 * @return <code>true</code> if any settings can be customized, otherwise
	 * <code>false</code>
	 */
	public boolean hasVisibleSettings(WIConfiguration config) {
		boolean isRemoteEnabled = config.getAppAccessMethodConfiguration()
						.isEnabledAppAccessMethod(AppAccessMethod.REMOTE);

		boolean canCustomizePerformanceSettings = canUserCustomizeWindowSize() || canUserCustomizeColorDepth()
						|| canUserCustomizeAudioQuality() || canUserCustomizePrinterMapping()
						|| canUserCustomizeBandwidth();

		boolean result = config.getAllowCustomizeSettings() && isRemoteEnabled
						&& canCustomizePerformanceSettings;
		return result;
	}
}
