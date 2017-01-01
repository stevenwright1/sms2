/*
 * Copyright (c) 2008 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.clientdetect.util;

import com.citrix.wi.clientdetect.WizardContext;
import com.citrix.wi.types.ProductType;
import com.citrix.wi.util.BrowserTitleBuilder;

/**
 * Responsible for constructing an appropriate browser page title.
 * Specific to client detection wizard.
 */
public class WizardBrowserTitleBuilder
{
    // Default branding product name key used as the first part of
    // the page title when site is configured as Applications.
    private static final String DEFAULT_PRODUCT_NAME_KEY = "DefaultProductName";

    // Desktop branding product name key used as the first part of
    // the page title when site is configured as Desktop.
    private static final String DESKTOP_PRODUCT_NAME_KEY = "DesktopProductName";

    /**
     * Create a page title from the WizardContext's Product name
     * and the pageBrowserTitle Key.
     *
     * @param wizardContext the Wizard Context
     * @param browserPageTitleKey key for lookup of the browser page title
     * @return browser page title
     */
    public static String createTitle(WizardContext wizardContext, String browserPageTitleKey) {
        String productName = getProductName(wizardContext);
        return BrowserTitleBuilder.buildBrowserTitle(productName, wizardContext.getString(browserPageTitleKey));
    }

    /**
     * Get the string lookup key for the branding product name.
     * @param wiContext the WI context
     * @return Branding product name lookup key
     */
    private static String getProductName(WizardContext wizardContext) {
        String productNameKey = DEFAULT_PRODUCT_NAME_KEY;
        // If we are branded as desktops then change the site name to reflect this.
        if (ProductType.XEN_DESKTOP.equals(wizardContext.getInputs().getProductType())) {
            productNameKey = DESKTOP_PRODUCT_NAME_KEY;
        }
        return wizardContext.getString(productNameKey);
    }
}
