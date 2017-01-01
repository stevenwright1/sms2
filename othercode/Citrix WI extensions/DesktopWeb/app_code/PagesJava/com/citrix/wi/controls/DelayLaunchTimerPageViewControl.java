/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */

package com.citrix.wi.controls;

/**
 * Maintains the presentation state of the delay launch timer page
 */
public class DelayLaunchTimerPageViewControl {

    private int retryTime;
    private String launchUrl;

    /**
     * Sets the retry time in seconds
     * @param value in seconds
     */
    public void setRetryTime(int value) {
        retryTime = value;
    }

    /**
     * Gets the retry time in secods
     * @param retryTime in seconds
     */
    public int getRetryTime() {
        return retryTime;
    }

    /**
     * Sets the launch Url
     * @param launch URL
     */
    public void setLaunchUrl(String value) {
        launchUrl = value;
    }

    /**
     * Gets the launch url
     * @param launch URL
     */
    public String getLaunchUrl() {
        return launchUrl;
    }

}
