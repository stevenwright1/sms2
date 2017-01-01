/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth;

import java.io.IOException;

import com.citrix.wi.config.WIConfiguration;
import com.citrix.wi.controls.PreLoginMessagePageControl;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.AGEUtilities;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.LocalisedText;
import com.citrix.wi.pageutils.WIBrowserTitleBuilder;
import com.citrix.wing.util.Strings;

public class PreLoginMessage extends PreLoginUIPage {

    protected PreLoginMessagePageControl viewControl = new PreLoginMessagePageControl();

    public PreLoginMessage(WIContext wiContext) {
        super(wiContext);
        wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
        layoutControl.formAction = Constants.FORM_POSTBACK;
    }

    protected boolean performInternal() throws IOException {
        // return to the login page if there is nothing to display
        if(!isPreLoginMessageConfiguredToAppear(wiContext)) {
            getWebAbstraction().clientRedirectToUrl(Constants.PAGE_LOGIN);
            return false;
        }

        // If they have clicked to OK button
        // add the session variable so the login page knows not to some back here again
        if(getWebAbstraction().isPostRequest()) {
            getWebAbstraction().setSessionAttribute("SV_PRE_LOGIN_MESSAGE_VISITED", Boolean.TRUE);
            getWebAbstraction().clientRedirectToUrl(Constants.PAGE_LOGIN);
            return false;
        }

        // Populate the view control and welcome control
        viewControl.ButtonString = LocalisedText.getPreLoginMessageButton(wiContext);
        viewControl.MessageString = LocalisedText.getPreLoginMessageText(wiContext);
        welcomeControl.setTitle(LocalisedText.getPreLoginMessageTitle(wiContext));

        // Set whether to show the full/low graphics link
        boolean ageEmbeddedMode = AGEUtilities.isAGEEmbeddedMode(wiContext);
        WIConfiguration config = wiContext.getConfiguration();
        navControl.setShowGraphicsMode(ageEmbeddedMode ? false : config.getUIConfiguration().getAllowCustomizeLayout());
        navControl.setFullGraphics(!Include.isCompactLayout(wiContext));

        setupBrowserTitle();

        return true;
    }

    protected String getBrowserPageTitleKey() {
        // Don't use a title, or just adminitrator customised title.
        return null;
    }

    protected void setupBrowserTitle() {

        // Override the default browser title if the user has customised the title string.
        String browserTitle = wiContext.getConfiguration().getLocalisedTextConfiguration().getLocalizedPreLoginMessageTitle(wiContext.getCurrentLocale());

        // if the admin has specified a
        if (browserTitle != null)
        {
            layoutControl.browserTitle = WIBrowserTitleBuilder.createCustomTitle(wiContext, browserTitle);
        }
    }

    public static boolean isPreLoginMessageConfiguredToAppear(WIContext wiContext) {
        String buttonString = LocalisedText.getPreLoginMessageButton(wiContext);
        String messageString = LocalisedText.getPreLoginMessageText(wiContext);
        String titleString = LocalisedText.getPreLoginMessageTitle(wiContext);
        // Only show the pre-login message page if
        // - there is a string from the button
        // and
        // - there is at least a title or a message to display
        return (!Strings.isEmpty(buttonString))
            && !(Strings.isEmpty(messageString) && Strings.isEmpty(titleString));
    }
}
