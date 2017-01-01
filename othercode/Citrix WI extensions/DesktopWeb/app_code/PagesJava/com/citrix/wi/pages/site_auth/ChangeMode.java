/*
 * Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.site_auth;

import java.io.IOException;

import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pages.StandardPage;
import com.citrix.wi.pages.site.DirectLaunch;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.types.LayoutType;
import com.citrix.wi.pageutils.PageHistory;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.SessionUtils;
import com.citrix.wi.UserPreferences;
import com.citrix.wing.webpn.UserContext;

/**
 * Flip between full and low graphics mode.
 *
 * This page could also be used to change other "modes" should such a need arise in the future.
 *
 * This page has no UI - it changes layout then displays the current page.
 */
public class ChangeMode extends StandardPage {

    public ChangeMode(WIContext wiContext) {
        super(wiContext);
    }

    protected boolean performImp() throws IOException {

        WebAbstraction web = wiContext.getWebAbstraction();

        // Get the input
        LayoutType layoutType = LayoutType.fromString(web.getQueryStringParameter(Constants.QSTR_LAYOUT_TYPE));

        // Validate the input
        if (layoutType == LayoutType.NORMAL || layoutType == LayoutType.COMPACT) {
            UserPreferences newUserPrefs = Include.getRawUserPrefs(wiContext.getUserEnvironmentAdaptor());
            newUserPrefs.setLayoutType(layoutType);

            // Now save the changes in the appropriate way depending on whether
            // the user is logged in or pre-login.
            if (Include.isLoggedIn(web)) {
                UserContext userContext = SessionUtils.checkOutUserContext(wiContext);
                Include.saveUserPrefs(newUserPrefs, wiContext, userContext);
                SessionUtils.returnUserContext(userContext);
            } else {
                Include.saveUserPrefsPreLogin(newUserPrefs, wiContext);
                wiContext.getUserEnvironmentAdaptor().commitState();
                wiContext.getUserEnvironmentAdaptor().destroy();
            }
        }

        // Re-display the current page in the new graphics mode
        String nextPage = PageHistory.getCurrentPageURL(web);
        // Direct launch query string (if any) needs to be passed through
        nextPage = DirectLaunch.propagateDirectLaunchInfo(web, nextPage);

        web.clientRedirectToUrl(nextPage);

        return false; // this page has no UI
    }
}
