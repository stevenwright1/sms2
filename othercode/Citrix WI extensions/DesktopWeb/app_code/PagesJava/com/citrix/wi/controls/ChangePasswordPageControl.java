/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.controls;

/**
 * Maintains presentation state for the change password page.
 */
public class ChangePasswordPageControl extends DialogPageControl {
    private boolean confirmOnly = false;

    /**
     * Returns whether the page is for changing the password, or
     * giving the user successful change confirmation
     */
    public boolean isConfirmationOnly() { return confirmOnly; }

    /**
     * Sets whether the page is for changing the password, or
     * to give the user successful change confirmation.
     * 
     * @param how true to indicate confirmation only, false for the
     * full change password UI
     */
    public void setConfirmOnly(boolean how) { confirmOnly = how; }
}
