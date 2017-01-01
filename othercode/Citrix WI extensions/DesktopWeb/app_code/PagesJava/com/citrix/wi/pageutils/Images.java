/*
 * Copyright (c) 2008 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils;

import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.types.UserInterfaceBranding;
import com.citrix.wi.util.ClientInfoUtilities;

/**
 * Utility methods for working with images in the user interface.
 */
public class Images {
    private static final String FAVICON_APPS     = "IcaComboAll.ico";
    private static final String FAVICON_DESKTOPS = "XenDesktopComboAll.ico";

    /**
     * Gets the filename of the favorites icon for the site.
     *
     * The favorites icon varies according to site branding.
     *
     * @return filename of the favorites icon as a string
     */
    public static String getFavoritesIcon(WIContext wiContext) {
        String baseIcon = FAVICON_APPS;

        if (Include.getSiteBranding(wiContext) == UserInterfaceBranding.DESKTOPS) {
            baseIcon = FAVICON_DESKTOPS;
        }

        return baseIcon;
    }
}