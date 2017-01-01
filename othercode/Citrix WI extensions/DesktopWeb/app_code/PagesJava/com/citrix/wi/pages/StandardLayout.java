/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages;

import java.io.IOException;

import com.citrix.authentication.tokens.AccessToken;
import com.citrix.authentication.tokens.GuestToken;
import com.citrix.wi.config.WIConfiguration;
import com.citrix.wi.controls.AutoLaunchJavaScriptPageControl;
import com.citrix.wi.controls.FeedbackControl;
import com.citrix.wi.controls.FooterControl;
import com.citrix.wi.controls.HeaderControl;
import com.citrix.wi.controls.LayoutControl;
import com.citrix.wi.controls.MessagesControl;
import com.citrix.wi.controls.NavControl;
import com.citrix.wi.controls.SearchBoxControl;
import com.citrix.wi.controls.SysMessageControl;
import com.citrix.wi.controls.WelcomeControl;
import com.citrix.wi.controlutils.FeedbackMessage;
import com.citrix.wi.controlutils.FeedbackUtils;
import com.citrix.wi.controlutils.LoginSettingsUtils;
import com.citrix.wi.controlutils.MessageUtils;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pages.site.AccountSettings;
import com.citrix.wi.pageutils.AGEUtilities;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.SessionToken;
import com.citrix.wi.pageutils.Settings;
import com.citrix.wi.pageutils.WIBrowserTitleBuilder;
import com.citrix.wi.types.UserInterfaceBranding;
import com.citrix.wing.MessageType;

/**
 * This forms the base class for any pages which *do* have UI.
 */
public abstract class StandardLayout extends StandardPage {

    protected LayoutControl layoutControl = new LayoutControl();
    protected FeedbackControl feedbackControl = new FeedbackControl();
    protected HeaderControl headerControl = new HeaderControl();
    protected NavControl navControl = new NavControl();
    protected WelcomeControl welcomeControl = new WelcomeControl();
    protected SysMessageControl sysMessageControl = new SysMessageControl();
    protected FooterControl footerControl = new FooterControl();
    protected AutoLaunchJavaScriptPageControl autoLaunchJavaScriptControl = new AutoLaunchJavaScriptPageControl();

    // The search box control is here because it can form part of the Welcome
    // HTML in full graphics mode (only on the applist page though).
    protected SearchBoxControl searchBoxControl = new SearchBoxControl();

    // The navbar messages link indicates the number of waiting messages, and
    // provides message detail on mouseover, so we need the MessagesControl
    // as part of the StandardLayout.
    protected MessagesControl messagesControl = new MessagesControl();


    public StandardLayout(WIContext wiContext) {
        super(wiContext);

        WebAbstraction web = wiContext.getWebAbstraction();

        web.setRequestContextAttribute(Constants.CTRL_LAYOUT, layoutControl);
        web.setRequestContextAttribute(Constants.CTRL_FEEDBACK, feedbackControl);
        web.setRequestContextAttribute(Constants.CTRL_HEADER, headerControl);
        web.setRequestContextAttribute(Constants.CTRL_NAV, navControl);
        web.setRequestContextAttribute(Constants.CTRL_WELCOME, welcomeControl);
        web.setRequestContextAttribute(Constants.CTRL_MESSAGES, messagesControl);
        web.setRequestContextAttribute(Constants.CTRL_SYSMESSAGE, sysMessageControl);
        web.setRequestContextAttribute(Constants.CTRL_FOOTER, footerControl);
        web.setRequestContextAttribute(Constants.CTRL_SEARCH_BOX, searchBoxControl);
        web.setRequestContextAttribute(Constants.CTRL_AUTO_LAUNCH_JAVASCRIPT, autoLaunchJavaScriptControl);

        feedbackControl.setTimeoutAlert(wiContext.getWebAbstraction());

        // Tweak defaults when in NavUI
        if (AGEUtilities.isAGEEmbeddedMode(wiContext)) {
            layoutControl.embeddedLayout = true;
            feedbackControl.setShowTimeoutWarning(false);
        }

        setupLayoutControl();

        // Set up the feedback control
        // This adds any messages from the query string as well as any
        // persistent feedback.
        FeedbackUtils.setupFeedback(wiContext, feedbackControl);
    }

    /**
     * Convenience method for setting feedback.
     * 
     * @param type the message type - cannot be null
     * @param key the message key - cannot be null
     * @param eventLogId associated event log ID
     */
    public void setFeedback(MessageType type, String key, String eventLogId) {
        FeedbackMessage message = new FeedbackMessage(type, key, null, eventLogId, null);
        feedbackControl.setFeedback(message);
    }

    /**
     * Convenience method for setting feedback.
     * 
     * @param type the message type - cannot be null
     * @param key the message key - cannot be null
     */
    public void setFeedback(MessageType type, String key) {
        FeedbackMessage message = new FeedbackMessage(type, key);
        feedbackControl.setFeedback(message);
    }

    /**
     * Convenience method for determining whether any feedback has been set.
     */
    public boolean isFeedbackSet() {
        return feedbackControl.isFeedbackSet();
    }

    protected void setupNavControl() {

        WIConfiguration config = wiContext.getConfiguration();

        boolean loggedIn = Include.isLoggedIn(wiContext.getWebAbstraction());
        boolean ageEmbeddedMode = AGEUtilities.isAGEEmbeddedMode(wiContext);

        MessageUtils.populate(wiContext, messagesControl, loggedIn);
        navControl.setShowMessages(true);
        navControl.setShowGraphicsMode( ageEmbeddedMode ? false : config.getUIConfiguration().getAllowCustomizeLayout());
        navControl.setFullGraphics(!Include.isCompactLayout(wiContext));

        if (loggedIn) {
            // User is logged in

            // Only show the user name for full graphics XA branded sites
            if (Include.getSiteBranding(wiContext) == UserInterfaceBranding.APPLICATIONS && !Include.isCompactLayout(wiContext)) {
                // Set up the user's friendly name
                String username = getUsernameFromAccessToken();
                if (username != null) {
                    navControl.setUserName(username);
                    navControl.setShowUserName(true);
                }
            }

            // Show account management option in the navbar?  Also include a shortcut in the Settings sub menu if Settings exists
            navControl.setAllowChangePassword(AccountSettings.allowChangePassword(wiContext));

            // Always hide the client detection link, by default
            navControl.setShowClientDetection(false);

            // Hide the Logoff button in AGE embedded/indirect mode
            navControl.setShowLogout(!AGEUtilities.isAGEEmbeddedOrIndirectMode(wiContext));

            navControl.setUseButtonForLogout(Include.isCompactLayout(wiContext));

            // Set up the tooltip for the logout link
            navControl.setLogoutToolTip(Include.logOffApplications(wiContext) ? NavControl.TIP_LOGOUT_WI_AND_APPS : NavControl.TIP_LOGOUT_WI_ONLY);

            // The settings link is shown by default, but can be hidden
            // by the administrator, or if no settings are available to be
            // configured by the user.
            navControl.setShowSettings(Settings.showPreferences(wiContext));

            // Show the reconnect and disconnect links?
            navControl.setShowReconnect(Settings.getShowReconnectLink(wiContext));
            navControl.setShowDisconnect(Settings.getShowDisconnectLink(wiContext));

        } else {
            // Set up the nav bar for when the User is not logged in

            // Show the login settings tab in the navbar
            navControl.setShowLoginSettings(LoginSettingsUtils.hasVisibleSettings(wiContext));
        }
    }

    /**
     * Gets the username from the current primary access token, or provides a
     * suitable substitute if no username is available.
     *
     * @return the username as a string
     */
    private String getUsernameFromAccessToken() {
        String username = null;

        AccessToken accessToken = Authentication.getPrimaryAccessToken(wiContext.getWebAbstraction());
        if (accessToken != null) { // defensive - this should never be null
            if (accessToken instanceof GuestToken) {
                // Anonymous user so use a localised string
                username = wiContext.getString("Guest");
            } else {
                // Can get the actual username for all other tokens
                username = accessToken.getShortUserName();
            }
        }

        return username;
    }

    /**
     * Perform any common guard action on the page. In this instance check that the session token
     * copies a posted page and in the session state match.
     */
    protected boolean performGuard() throws IOException
    {
        return SessionToken.guard(wiContext) && super.performGuard();
    }

    /**
     * Get the Key name for the browser page title.  This is used to construct the page title.
     */
    protected abstract String getBrowserPageTitleKey();

    /**
     * Setup the basics of the layout control (i.e. Page Title).
     */
    protected void setupLayoutControl()
    {

        layoutControl.browserTitle = WIBrowserTitleBuilder.createTitle(wiContext, getBrowserPageTitleKey());
    }
}
