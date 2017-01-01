/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.site.preferences.policies;

import com.citrix.wi.config.WIConfiguration;
import com.citrix.wi.pages.site.PreferencesSection;

/**
 * Defines a policy object associated with a {@link PreferencesSection}.  
 * Implementing classes can encapsulate the logic associated with deciding 
 * whether or not to display a particular preference setting, or a whole 
 * preferences section.  
 */
public interface PreferencesSectionPolicy {
    
    /**
     * Whether the policy allows any settings to be customized.  
     * 
     * @param config the Web Interface configuration
     * @return <code>true</code> if any settings can be customized, otherwise
     * <code>false</code>
     */
    public boolean hasVisibleSettings(WIConfiguration config);
}
