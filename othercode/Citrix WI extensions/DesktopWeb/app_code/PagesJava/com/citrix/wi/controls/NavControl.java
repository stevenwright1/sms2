/*
 * Copyright (c) 2007 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.controls;

import com.citrix.wi.tabs.TabSet;

/**
 * Maintains presentation state for the navigation control.
 */
public class NavControl {
    private boolean showLoginSettings = false;
    private boolean loginSettingsLinkActive = true;
    private boolean showMessages = false;
    private boolean messagesLinkActive = true;
    private boolean showSettings = false;
    private boolean settingsLinkActive = true;
    private boolean showLayout = false;
    private boolean showUserName = false;
    private String userName = "";
    private boolean allowChangePassword = false;
    private boolean showLogout = false;
    private boolean showClientDetection = false;
    private String logoutToolTip = TIP_LOGOUT_WI_AND_APPS;
    private boolean showGraphicsMode = false;
    private boolean fullGraphics = true;
    private boolean showReconnect = false;
    private boolean showDisconnect = false;
    private boolean useButtonForLogout = false;
    private String extraQuery = "";
    private TabSet tabs = new TabSet();

    public boolean getShowNavBar() {
        boolean showNavBar = (tabs.getNumTabs() > 1);
        return showNavBar;
    }

    /**
     * Tests whether the pre-Login settings link should be shown.
     * @return <code>true</code> if shown, else <code>false</code>
     */
    public boolean getShowLoginSettings() {
        return showLoginSettings;
    }

    /**
     * Sets whether the pre-Login settings link should be shown.
     * @param value <code>true</code> if shown, else <code>false</code>
     */
    public void setShowLoginSettings(boolean value) {
        showLoginSettings = value;
    }

    /**
     * Tests whether the messages link should be shown.
     * @return <code>true</code> if shown, else <code>false</code>
     */
    public boolean getShowMessages() {
        return showMessages;
    }

    /**
     * Sets whether the messages link should be shown.
     * @param value <code>true</code> if shown, else <code>false</code>
     */
    public void setShowMessages(boolean value) {
        showMessages = value;
    }

    /**
     * Tests whether the settings link should be shown.
     * @return <code>true</code> if shown, else <code>false</code>
     */
    public boolean getShowSettings() {
        return showSettings;
    }

    /**
     * Sets whether the settings link should be shown.
     * @param value <code>true</code> if shown, else <code>false</code>
     */
    public void setShowSettings(boolean value) {
        showSettings = value;
    }

    /**
     * Gets whether the pre-login settings link should be active (clickable).
     * @return <code>true</code> if the link is active, else <code>false</code>
     */
    public boolean getLoginSettingsLinkActive() {
        return loginSettingsLinkActive;
    }

    /**
     * Sets whether the pre-login settings link should be active (clickable).
     * @param value <code>true</code> if active, else <code>false</code>
     */
    public void setLoginSettingsLinkActive(boolean value) {
        loginSettingsLinkActive = value;
    }

    /**
     * Gets whether the messages link should be active (clickable).
     * @return <code>true</code> if the link is active, else <code>false</code>
     */
    public boolean getMessagesLinkActive() {
        return messagesLinkActive;
    }

    /**
     * Sets whether the messages link should be active (clickable).
     * @param value <code>true</code> if active, else <code>false</code>
     */
    public void setMessagesLinkActive(boolean value) {
        messagesLinkActive = value;
    }

    /**
     * Gets whether the settings link should be active (clickable).
     * @return <code>true</code> if the link is active, else <code>false</code>
     */
    public boolean getSettingsLinkActive() {
        return settingsLinkActive;
    }

    /**
     * Sets whether the settings link should be active (clickable).
     * @param value <code>true</code> if active, else <code>false</code>
     */
    public void setSettingsLinkActive(boolean value) {
        settingsLinkActive = value;
    }

    /**
     * Tests whether the layout link should be shown.
     * @return <code>true</code> if shown, else <code>false</code>
     */
    public boolean getShowLayout() {
        return showLayout;
    }

    /**
     * Sets whether the layout link should be shown.
     * @param value <code>true</code> if shown, else <code>false</code>
     */
    public void setShowLayout(boolean value) {
        showLayout = value;
    }

    /**
     * Tests whether the logged in user's name and the logout link should be shown.
     * @return <code>true</code> if shown, else <code>false</code>
     */
    public boolean getShowUserName() {
        return showUserName;
    }

    /**
     * Sets whether the logged in user's name and the logout link should be shown.
     * @param value <code>true</code> if shown, else <code>false</code>
     */
    public void setShowUserName(boolean value) {
        showUserName = value;
    }

    /**
     * Gets the displayed user name.
     * @return The user name
     */
    public String getUserName() {
        return userName;
    }

    /**
     * Sets the displayed user name.
     * @param The user name
     */
    public void setUserName(String value) {
        userName = value;
    }

    /**
     * Tests whether the logout link should be shown.
     * @return <code>true</code> if shown, else <code>false</code>
     */
    public boolean getShowLogout() {
        return showLogout;
    }

    /**
     * Tests whether the client detection link should be shown.
     * @return <code>true</code> if shown, else <code>false</code>
     */
    public boolean getShowClientDetection() {
        return showClientDetection;
    }

    /**
     * Sets whether the client detection link should be shown.
     * @return <code>true</code> if shown, else <code>false</code>
     */
    public void setShowClientDetection(boolean value) {
        showClientDetection = value;
    }

    /**
     * Sets whether the logout link should be shown.
     * @param value <code>true</code> if shown, else <code>false</code>
     */
    public void setShowLogout(boolean value) {
        showLogout = value;
    }

    /**
     * Gets the key for the logout link tooltip string.
     * @return The key
     */
    public String getLogoutToolTip() {
        return logoutToolTip;
    }

    /**
     * Sets the key for the logout link tooltip string.
     * @param The key
     */
    public void setLogoutToolTip(String value) {
        logoutToolTip = value;
    }

    /**
     * Gets whether the graphics mode link should be shown.
     * @return <code>true</code> if shown, else <code>false</code>
     */
    public boolean getShowGraphicsMode() {
        return showGraphicsMode;
    }

    /**
     * Sets whether the graphics mode link should be shown.
     * @param value <code>true</code> if shown, else <code>false</code>
     */
    public void setShowGraphicsMode(boolean value) {
        showGraphicsMode = value;
    }

    /**
     * Gets whether we are in full graphics mode.
     * @return <code>true</code> if we are in full graphics mode, else <code>false</code>
     */
    public boolean getFullGraphics() {
        return fullGraphics;
    }

    /**
     * Sets whether we are in full graphics mode.
     * @param value <code>true</code> if we are in full graphics mode, else <code>false</code>
     */
    public void setFullGraphics(boolean value) {
        fullGraphics = value;
    }

    /**
     * Gets whether the reconnect button should be shown.
     * @return <code>true</code> if shown, else <code>false</code>
     */
    public boolean getShowReconnect() {
        return showReconnect;
    }

    /**
     * Sets whether the reconnect button should be shown.
     * @param value <code>true</code> if shown, else <code>false</code>
     */
    public void setShowReconnect(boolean value) {
        showReconnect = value;
    }

    /**
     * Gets whether the disconnect button should be shown.
     * @return <code>true</code> if shown, else <code>false</code>
     */
    public boolean getShowDisconnect() {
        return showDisconnect;
    }

    /**
     * Sets whether the disconnect button should be shown.
     * @param value <code>true</code> if shown, else <code>false</code>
     */
    public void setShowDisconnect(boolean value) {
        showDisconnect = value;
    }

    /**
     * Gets if the logout link should be represented as a button.
     * @return <code>true</code> if shown as a button, else <code>false</code>
     */
    public boolean getUseButtonForLogout() {
        return useButtonForLogout;
    }

    /**
     * Sets if the logout link should be represented as a button.
     * @param value <code>true</code> if shown as a button, else <code>false</code>
     */
    public void setUseButtonForLogout(boolean value) {
        useButtonForLogout = value;
    }

    /**
     * Gets whether the user is allowed to change password.
     * @return <code>true</code> if the user is allowed to.
     */
    public boolean getAllowChangePassword() {
        return allowChangePassword;
    }

    /**
     * Sets whether the user is allowed to change password or not
     * @param value <code>boolean</code>  whether to allow
     */
    public void setAllowChangePassword(boolean value) {
        allowChangePassword = value;
    }

    /**
     * Gets the set of tabs to display.
     * @return <code>TabSet</code> object representing the tabs to be displayed
     */
    public TabSet getTabs() {
        return tabs;
    }

    /**
     * Sets the tabs to display.
     * @param value <code>TabSet</code> object representing the tabs to be displayed. Cannot be null
     */
    public void setTabs(TabSet value) {
        if (value == null) {
            throw new IllegalStateException("tabSet cannot be null");
        }
        tabs = value;
    }

    /**
     * Gets an extra query string (determined by the page) for links in the control
     * @param value <code>String</code> query string fragment, excluding leading "?"
     */
    public String getExtraQuery() {
        return extraQuery;
    }

    /**
     * Sets an extra query string (determined by the page) for links in the control
     * @param value <code>String</code> query string fragment, excluding leading "?"
     */
    public void setExtraQuery(String value) {
        extraQuery = value;
    }

    // Possible values for the logout link tooltip string key
    public static String TIP_LOGOUT_WI_ONLY = "TipLogoutWIOnly";
    public static String TIP_LOGOUT_WI_AND_APPS = "TipLogoutWIAndApps";
}