/*
 * Copyright (c) 2008 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */

package com.citrix.wi.pageutils;

import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.types.UserInterfaceBranding;
import com.citrix.wi.util.BrowserTitleBuilder;

/**
 * Responsible for constructing an appropriate browser page title.
 */
public class WIBrowserTitleBuilder {

    // Default branding product name key used as the first part of
    // the page title when site is configured as Applications.
    private static final String DEFAULT_PRODUCT_NAME_KEY = "DefaultProductName";

    // Desktop branding product name key used as the first part of
    // the page title when site is configured as Desktop.
    private static final String DESKTOP_PRODUCT_NAME_KEY = "DesktopProductName";

    /**
     * Get the string lookup key for the branding product name.
     * @param wiContext the WI context
     * @return Branding product name lookup key
     */
    public static String getProductNameKey(WIContext wiContext) {

        String productNameKey = DEFAULT_PRODUCT_NAME_KEY;

        // If we are branded as Desktops then change the site name to reflect this.
        if (Include.getSiteBranding(wiContext) == UserInterfaceBranding.DESKTOPS) {
            productNameKey = DESKTOP_PRODUCT_NAME_KEY;
        }

        return productNameKey;
    }

    /**
     * Get the branding product name.
     * @param wiContext the WI context
     * @return Branding product name
     */
    public static String getProductName(WIContext wiContext) {

        return wiContext.getString(getProductNameKey(wiContext));

    }

    /**
     * Create a page title from the default siteName and the pageBrowserTitle Key
     * @param wiContext the WI context
     * @param browserPageTitleKey string representing key for lookup of the browser page title
     * @return browser page title
     */
    public static String createTitle(WIContext wiContext, String browserPageTitleKey) {

        String productNameKey = getProductNameKey(wiContext);

        return createTitle(wiContext, productNameKey, browserPageTitleKey);
    }

    /**
     * Create a page title from the Product Name Key and the pageBrowserTitle Key
     * @param wiContext the WI context
     * @param productNameKey key for the lookup of the site name.
     * @param browserPageTitleKey key for lookup of the browser page title
     * @return browser page title
     */
    private static String createTitle(WIContext wiContext, String productNameKey, String browserPageTitleKey) {

        String title = wiContext.getString(productNameKey);

        // If a page name has been defined then add it onto the generic
        // page title.
        if (browserPageTitleKey != null) {
            title = BrowserTitleBuilder.buildBrowserTitle(title, wiContext.getString(browserPageTitleKey));
        }

        return title;
    }


    /**
     * Create a custom title using a specified string passed in which is combined
     * with product branding to make the final browser title
     * @param wiContext the WI context
     * @param browserPageTitle Page title to use in the browser title.
     * @return browser page title
     */
    public static String createCustomTitle(WIContext wiContext, String browserPageTitle) {
        return createCustomTitle(wiContext, DEFAULT_PRODUCT_NAME_KEY, browserPageTitle);
    }

    /**
     * Create a custom title using a specified string passed in which is combined
     * with product branding to make the final browser title.
     * @param wiContext the WI context
     * @param productNameKey  look up key for the product name to use
     * @param browserPageTitle Page title to use in the browser title.
     * @return browser page title
     */
    private static String createCustomTitle(WIContext wiContext, String productNameKey, String browserPageTitle) {

        String pageTitle = wiContext.getString(productNameKey);

        pageTitle = BrowserTitleBuilder.buildBrowserTitle(pageTitle, browserPageTitle);

        return pageTitle;
    }
}