/*
 * Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.site_auth;

import java.io.IOException;

import com.citrix.wi.controls.StyleControl;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pages.StandardPage;
import com.citrix.wi.pageutils.AGEUtilities;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.UIUtils;

/**
 * This page deals with rendering the style sheet
 *
 * The parameters that this is taken with
 */
public class Style extends StandardPage {

    StyleControl viewControl = new StyleControl();

    public Style(WIContext wiContext) {
        super(wiContext);
        wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
    }

    protected void setResponseCaching() {
        int cacheDuration = UIUtils.getCSSCacheDuration(wiContext.getWebAbstraction());

        if (cacheDuration > 0) {
            wiContext.getWebAbstraction().setResponseCacheDuration(cacheDuration);

            // Content may be per-user so only allow private caching
            wiContext.getWebAbstraction().setResponseCacheControl(WebAbstraction.CACHE_CONTROL_PRIVATE);
        } else {
            wiContext.getWebAbstraction().setNoCaching();
        }
    }

    protected boolean performImp() throws IOException {

        WebAbstraction web = wiContext.getWebAbstraction();
        web.setResponseContentType("text/css");

        // Ignore all the parameters, those
        // are just used to work with browser caching
        // We use the information in the Session to decide what to show

        // The parameters will ensure that the browser retrieves the correct style
        // when the language or config files change, or if the
        // display mode or language is changed.

        // Look at the layout type
        // NB: the order of these is important
        // because AGEEmbeded is also in compact mode
        if (AGEUtilities.isAGEEmbeddedMode(wiContext)) {
            viewControl.currentStyle = StyleControl.Style.EMBEDDED;
        } else if (Include.isCompactLayout(wiContext)) {
            viewControl.currentStyle = StyleControl.Style.LOW;
        } else {
            // default to full graphics
            viewControl.currentStyle = StyleControl.Style.FULL;
        }

        return true; // always display the style
    }
}
