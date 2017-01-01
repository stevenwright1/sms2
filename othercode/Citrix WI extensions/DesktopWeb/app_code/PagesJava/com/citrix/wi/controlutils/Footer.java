/*
 * Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.controlutils;

import com.citrix.wi.controls.FooterControl;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.LocalisedText;

/**
 * Utility class for the footer control
 */
public class Footer {

    /**
     * Sets the configuration values to the control
     *
     * @param wiContext The WI context.
     * @param viewControl The <code>FooterControl</code>.
     */
    public static void populate(WIContext wiContext, FooterControl viewControl) {
        viewControl.setFooterText(LocalisedText.getFooterText(wiContext));
    }
}