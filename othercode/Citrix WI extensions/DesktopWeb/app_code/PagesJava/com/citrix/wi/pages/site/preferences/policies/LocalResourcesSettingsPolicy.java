/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.site.preferences.policies;

import com.citrix.wi.config.ClientConnectionConfiguration;
import com.citrix.wi.config.WIConfiguration;

/**
 * Defines the policy which governs how the local resources settings are
 * displayed.
 */
public class LocalResourcesSettingsPolicy implements PreferencesSectionPolicy {
	private ClientConnectionConfiguration clientConnectionConfiguration;

	/**
	 * Creates a new LocalResourcesSettingsPolicy object from the provided
	 * ClientConnectionConfiguration.
	 *
	 * @param policyConfiguration the ClientConnectionConfiguration used to 
	 * configure the local resources settings' policy.
	 */
	public LocalResourcesSettingsPolicy(ClientConnectionConfiguration policyConfiguration) {
		this.clientConnectionConfiguration = policyConfiguration;
	}

	/**
	 * Whether the user is allowed to customize key passthrough.
	 * 
	 * @return <code>true</code> if the setting can be customized, otherwise
	 * <code>false</code>
	 */
	public boolean canUserCustomizeKeyPassthrough() {
		return clientConnectionConfiguration.getAllowCustomizeKeyPassthrough();
	}

	/**
	 * Whether the user is allowed to customize virtual COM ports.
	 * 
	 * @return <code>true</code> if the setting can be customized, otherwise
	 * <code>false</code>
	 */
	public boolean canUserCustomizeVirtualCOMPort() {
		return clientConnectionConfiguration.getAllowCustomizeVirtualCOM();
	}

	/**
	 * Whether the user is allowed to customize special folder redirection.
	 * 
	 * @return <code>true</code> if the setting can be customized, otherwise
	 * <code>false</code>
	 */
	public boolean canUserCustomizeSpecialFolderRedirection() {
		return clientConnectionConfiguration.getAllowCustomizeSpecialFolderRedirection();
	}

	/**
	 * Whether the policy allows any settings to be customized.  
	 * 
	 * @param config the Web Interface configuration
	 * @return <code>true</code> if any settings can be customized, otherwise
	 * <code>false</code>
	 */
	public boolean hasVisibleSettings(WIConfiguration config) {
		return canUserCustomizeKeyPassthrough() ||
			   canUserCustomizeVirtualCOMPort() ||
			   canUserCustomizeSpecialFolderRedirection();
	}
}
