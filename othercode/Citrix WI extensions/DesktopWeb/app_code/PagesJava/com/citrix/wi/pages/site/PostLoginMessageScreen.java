/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.site;

import java.io.IOException;

import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pages.StandardLayout;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.LocalisedText;

public class PostLoginMessageScreen extends StandardLayout {

    public PostLoginMessageScreen(WIContext wiContext) {
        super(wiContext);
        wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", messagesControl);
    }

    public final boolean performImp() throws IOException {
        setupWelcomeControl();

        layoutControl.hasCancelButton = true;
        super.setupNavControl();
        navControl.setMessagesLinkActive(false);

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

