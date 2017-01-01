/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.controls;

/**
 * Maintains presentation state for the Logged Out Page.
 */
public class LoggedoutPageControl extends PageControl {
    private boolean showCloseWindow = false;

    /**
     * Tests whether the browser window should be closed at logout.
     * @return <code>true</code> if it should be closed, else <code>false</code>
     */
    public boolean getShowCloseWindow() {
        return showCloseWindow;
    }

    /**
     * Sets whether the browser window should be closed at logout.
     * @param value <code>true</code> if it should be closed, else <code>false</code>
     */
    public void setShowCloseWindow( boolean value ) {
        showCloseWindow = value;
    }
}
