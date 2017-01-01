/*
 * Copyright (c) 2005 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.controls;

/**
 * Maintains presentation state for the Account Self Service Password Reset Page.
 */
public class AccountSSPasswordResetPageControl extends PageControl {
    private boolean showSuccessMessage = false;

    /**
     * Tests whether the page should display a success message.
     * @return <code>true</code> if message should be displayed, else <code>false</code>
     */
    public boolean getShowSuccessMessage() {
        return showSuccessMessage;
    }

    /**
     * Sets whether the page should display a success message.
     * @param value <code>true</code> if message should be displayed, else <code>false</code>
     */
    public void setShowSuccessMessage( boolean value ) {
        showSuccessMessage = value;
    }
}
