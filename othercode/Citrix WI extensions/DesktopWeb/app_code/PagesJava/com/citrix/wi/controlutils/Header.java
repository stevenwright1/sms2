/*
 * Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.controlutils;

import com.citrix.wi.controls.HeaderControl;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.Branding;

/**
 * Utility class for the header control
 */
public class Header {

    /**
     * Sets the configuration values to the control
     *
     * @param wiContext The WI context.
     * @param viewControl The <code>HeaderControl</code>.
     */
    public static void populate (WIContext wiContext, HeaderControl viewControl) {
        viewControl.setHeadingImage(Branding.getHeadingImage(wiContext.getConfiguration()));
        viewControl.setCompactHeaderImage(Branding.getCompactHeaderImage(wiContext.getConfiguration()));
        viewControl.setHeadingHomePage(Branding.getHeadingHomePage(wiContext.getConfiguration()));
    }
}