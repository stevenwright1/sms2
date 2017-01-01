/*
 * Copyright (c) 2007 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */
package com.citrix.wi.controls;

import com.citrix.wi.pageutils.Branding;

/**
 * Maintains presentation state for the header control.
 */
public class HeaderControl {
    private String headingImage = Branding.DEFAULT_HEADING_IMAGE;
    private String compactHeaderImage = Branding.DEFAULT_COMPACT_HEADER_IMAGE;
    private String headingHomePage = "";
    
    /**
     * Gets the heading image
     */
    public String getHeadingImage() {
        return headingImage;
    }

    /**
     * Sets the heading image
     * @param value the heading image
     */
    public void setHeadingImage(String value) {
        headingImage = value;
    }

    /**
    * Gets the heading image for low graphics
    */
    public String getCompactHeaderImage() {
        return compactHeaderImage;
    }

    /**
     * Sets the heading image for low graphics
     * @param value the heading image
     */
    public void setCompactHeaderImage(String value) {
        compactHeaderImage = value;
    }

    /**
     * Gets the heading homepage
     */
    public String getHeadingHomePage() {
        return headingHomePage;
    }

    /**
     * Sets the heading homepage
     * @param value the heading homepage
     */
    public void setHeadingHomePage(String value) {
        headingHomePage = value;
    }
}

