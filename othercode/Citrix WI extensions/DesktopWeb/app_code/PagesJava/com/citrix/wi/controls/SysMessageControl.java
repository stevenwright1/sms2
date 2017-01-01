/*
 * Copyright (c) 2007 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */
package com.citrix.wi.controls;

/**
 * Maintains presentation state for the system message control.
 */
public class SysMessageControl {
    private String message = null;

    /**
     * Gets the system message
     * @return the message
     */
    public String getMessage() {
        return message;
    }

    /**
     * Sets the system message
     * @param value the message
     */
    public void setMessage(String value) {
        message = value;
    }
}