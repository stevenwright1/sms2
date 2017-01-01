/*
 * Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.site_auth;

import java.io.IOException;

import com.citrix.wi.controls.StyleControl;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pages.StandardLayout;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.UIUtils;

/**
 * This page deals with rendering the javascript
 * It uses StandardLayout so the feedback control has the
 * timeout settings in it for the feedback.js
 */
public class JavaScript extends StandardLayout {

    StyleControl viewControl = new StyleControl();

    public JavaScript(WIContext wiContext) {
        super(wiContext);
        wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
    }

    protected void setResponseCaching() {
        int cacheDuration = UIUtils.getJavaScriptCacheDuration(wiContext.getWebAbstraction());

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
        web.setResponseContentType("text/javascript; charset=UTF-8");

        // Ignore all the parameters, those
        // are just used to work with browser caching
        // We use the information in the Session to decide what to show

        // The parameters will ensure that the browser retrieves the correct javascirpt
        // when the language or config files change, or if the
        // display mode or language is changed.

        if (Include.isCompactLayout(wiContext)) {
            viewControl.currentStyle = StyleControl.Style.LOW;
        } else {
            // default to full graphics
            viewControl.currentStyle = StyleControl.Style.FULL;
        }

        return true; // always display the javascirpt
    }

    public String getBrowserPageTitleKey() {
        return null;
    }
}
