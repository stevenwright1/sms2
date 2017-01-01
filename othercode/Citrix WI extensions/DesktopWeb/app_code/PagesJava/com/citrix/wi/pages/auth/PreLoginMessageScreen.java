/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth;

import java.io.IOException;

import com.citrix.wi.controlutils.MessageUtils;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.LocalisedText;
import com.citrix.wi.pageutils.Settings;
import com.citrix.wi.pageutils.TabUtils;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wi.types.UserInterfaceBranding;
import com.citrix.wing.MessageType;

public class PreLoginMessageScreen extends PreLoginUIPage {

    public PreLoginMessageScreen(WIContext wiContext) {
        super(wiContext);
        wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", messagesControl);
    }

    /**
     * Check we can access the messages page.
     *
     * Override in sub classes where needed to override access checking.
     */
    protected boolean canAccess() {

        if (!Settings.showPreLoginMessageScreen(wiContext)) {
            wiContext.getWebAbstraction().clientRedirectToUrl(
                    Constants.PAGE_LOGIN + UIUtils.getMessageQueryStr(MessageType.ERROR, "CustomSettingsNotAllowed")
                    );
            return false;
        }
        return true;
    }

    public boolean performInternal() throws IOException {

        // Check that this page can be accessed.
        if (!canAccess()) {
            return false;
        }

        layoutControl.hasCancelButton = true;
        navControl.setShowLoginSettings(true);
        navControl.setShowMessages(true);
        navControl.setMessagesLinkActive(false);
        MessageUtils.populate(wiContext, messagesControl, Include.isLoggedIn(wiContext.getWebAbstraction()));
        setupWelcomeControl();

        wiContext.getUserEnvironmentAdaptor().commitState();
        wiContext.getUserEnvironmentAdaptor().destroy();
        recordCurrentPageURL();
        return true;
    }

    /**
     * Sets up the welcome control
     * Overrides the welcome message if a customised text is specified
     */
    private void setupWelcomeControl() {
        String title = wiContext.getString("ScreenTitleMessages");
        
        welcomeControl.setTitle(title);
        welcomeControl.setTitleClass("messages");
    }

    protected String getBrowserPageTitleKey() {
        return "BrowserTitleMessages";
    }
}
