/*
 * Tab.java
 * Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */

package com.citrix.wi.tabs;
import com.citrix.wing.AccessTokenException;
import com.citrix.wing.ResourceUnavailableException;

/**
 * Represents a tab that can be displayed in the application list page.
 */
public abstract class Tab {

    /**
     * Gets the internal ID used to identify the tab.
     *
     * The tab ID is unique among all other tabs and remains constant (it is
     * not localized).
     *
     * @return the tab ID
     */
    public abstract String getId();

    /**
     * Gets the text to display on the tab header.
     *
     * @return the tab title
     */
    public abstract String getTitle();

    /**
     * Gets the target page to display when the tab is selected.
     *
     * @return the target page
     */
    public abstract String targetPage();

    /**
     * Gets whether the user should be shown the tab header.
     *
     * @return <code>true</code> if the tab is visible, else <code>false</code>
     */
    public boolean isVisible() {
        return true;
    }

    /**
     * Gets whether the tab should be shown with a visible indication that work is in progress.
     *@return <code>true</code> if the tab should be shown as busy.
     */
    public boolean isBusy() {
        return false;
    }

    /**
     * Populates view controls etc.
     * @return returns <code>true</code> if the tab could be shown
     */
    public boolean setupViewControl() throws AccessTokenException, ResourceUnavailableException {
        return true;
    }
}
