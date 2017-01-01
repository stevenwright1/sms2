/*
 * Copyright (c) 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.controls;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wing.util.WebUtilities;
import com.citrix.wi.pageutils.DelayedLaunchUtilities;
import com.citrix.wi.pageutils.DesktopLaunchHistory;

/**
 * Class responsible for generating markup for desktops tab.
 */
public abstract class DesktopGroupMarkup {
    /**
     * Generates markup for a given desktop group.
     *
     * @param desktopGroup group to generate markup for
     * @param wiContext the wi context
     * @return markup for a given desktop group.
     */
    public abstract String getGroupMarkup(DesktopGroup desktopGroup, WIContext wiContext);

	/**
     * Generates markup for a single resource.
     * @param desktop desktop to generate markup for
     * @param wiContext the wi context
     */
    public abstract String getDesktopMarkup(ResourceControl desktop, WIContext wiContext);

    /**
     * Tells whether to display restart button.
     * 
     * @param resource the resource
     * @return true to show button, false otherwise
     */
    protected boolean canShowRestartButton(ResourceControl resource) {
        // The decision whether to show restart button is based on two flags:
        // - resource.isRestartable - show button only for restartable desktops
        // - resource.requiresAssignment - show button only for desktops that are already assigned

		boolean result = resource.isRestartable && !resource.requiresAssignment;
        return result;
    }

}