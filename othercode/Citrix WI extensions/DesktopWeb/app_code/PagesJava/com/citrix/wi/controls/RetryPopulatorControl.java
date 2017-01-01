/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */

package com.citrix.wi.controls;

/**
 * Maintains the presentation state of the RetryPopulator page
 */
public class RetryPopulatorControl {
    // This holds the html for hidden iframes for each desktop that needs to be retried.
    String retryIframesHtml = "";
    // Redirect Url for any error message
    public String redirectUrl;

    /**
     * Sets the html for hidden iframes for each desktop that needs to be retried.
     */
    public void setRetryIframesHtml(String value) {
        retryIframesHtml = value;
    }

    /**
     * Gets the html for creating hidden iframes for each desktop that needs to be retried.
     */
    public String getRetryIframesHtml() {
        return retryIframesHtml;
    }
}
