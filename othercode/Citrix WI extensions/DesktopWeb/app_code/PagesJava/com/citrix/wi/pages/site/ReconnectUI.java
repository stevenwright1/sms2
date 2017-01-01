package com.citrix.wi.pages.site;

import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pages.StandardLayout;

/**
 * Dummy class that does nothing.  Required to get the Reconnect
 * UI page (a branded page with a link back to the Applist) to
 * work properly.
 */
public class ReconnectUI extends StandardLayout
{
    public ReconnectUI(WIContext wiContext)
    {
        super(wiContext);
    }

    protected boolean performImp() {
        welcomeControl.setTitle(wiContext.getString("ReturnToApplistTitle"));
        return true;
    }

    protected String getBrowserPageTitleKey() {
        return "BrowserTitleReconnectPage";
    }
}
