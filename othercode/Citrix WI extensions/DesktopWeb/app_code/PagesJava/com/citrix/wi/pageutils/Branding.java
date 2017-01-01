/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils;

import com.citrix.wi.config.WIConfiguration;

/**
 * Utility class for the branding configuration
 * It also contains default branding colors and images
 */
public class Branding {

    /**
     * Header and footer defaults
     */
    public static final String DEFAULT_BRANDING_COLOR = "#95A2AC";
    public static final String DEFAULT_BRANDING_IMAGE = Constants.MEDIA_LOCATION + "HeaderGradient.png";
    public static final String DEFAULT_HEADING_IMAGE = Constants.MEDIA_LOCATION + "CitrixLogoHeader.png";
    public static final String DEFAULT_COMPACT_HEADER_IMAGE = Constants.MEDIA_LOCATION + "CitrixLogoHeader.png";
    private static final String DEFAULT_HEADER_FONT_COLOR = "#FFFFFF";
    
    /**
     * Gets the branding color.
     *
     * @return a string representing the branding color. If it is not
     * configured, the default branding color is returned.
     */
    public static String getBrandingColor(WIConfiguration wiConfig) {
        return getStringWithDefault(wiConfig.getBrandingConfiguration().getBrandingColor(), DEFAULT_BRANDING_COLOR);
    }

    /**
     * Returns the URL of the branding image if we configure to display it
     * @return a string of the file, empty string if branding image should not be displayed
     */
    public static String getBrandingImageURL(WIConfiguration wiConfig) {
        String brandingImage = "";
        if (wiConfig.getBrandingConfiguration().getDisplayBrandingImage()) {
            brandingImage = getStringWithDefault(wiConfig.getBrandingConfiguration().getBrandingImageURL(), DEFAULT_BRANDING_IMAGE);
        }
        return brandingImage;
    }

    /**
     * Returns the URL of the heading image
     * @return a string of the file
     */
    public static String getHeadingImage(WIConfiguration wiConfig) {
        return getStringWithDefault(wiConfig.getBrandingConfiguration().getHeadingImageURL(), DEFAULT_HEADING_IMAGE);
    }

    /**
     * Returns the URL of the heading image for low graphics
     * @return a string of the file
     */
    public static String getCompactHeaderImage(WIConfiguration wiConfig) {
        return getStringWithDefault(wiConfig.getBrandingConfiguration().getCompactHeaderImageURL(), DEFAULT_COMPACT_HEADER_IMAGE);
    }

    /**
     * Gets the URL for the heading home page.
     *
     * @return a string containing the home page URL; or <code>null</code> if
     * not configured.
     */
    public static String getHeadingHomePage(WIConfiguration wiConfig) {
        return getStringWithDefault(wiConfig.getBrandingConfiguration().getHeadingHomePageURL(), null);
    }    
    
    /**
     * Gets the font color for text displayed in the header. 
     * @return an HTML color string
     */
    public static String getHeaderFontColor(WIConfiguration wiConfig) {
        return getStringWithDefault(wiConfig.getBrandingConfiguration().getHeaderFontColor(), DEFAULT_HEADER_FONT_COLOR);
    }

    /*
     * If the str is null or empty or only contains whitespace
     * return defaultStr, otherwise return str.
     */
    private static String getStringWithDefault(String str, String defaultStr) {
        return ((str == null) || (str.trim().length() == 0)) ? defaultStr : str;
    }
}
