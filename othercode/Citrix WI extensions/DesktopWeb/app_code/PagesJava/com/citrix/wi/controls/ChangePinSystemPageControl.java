/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.controls;

/**
 * Maintains presentation state for the change system PIN page.
 */
public class ChangePinSystemPageControl extends DialogPageControl {

    private String systemPin = "";

    /**
     * Gets the system PIN for this page.
     * @return the system PIN
     */
    public String getSystemPin() {
        return systemPin;
    }

    /**
     * Sets the system PIN for this page.
     * @param systemPin the system PIN
     */
    public void setSystemPin( String systemPin ) {
        this.systemPin = systemPin;
    }
}
