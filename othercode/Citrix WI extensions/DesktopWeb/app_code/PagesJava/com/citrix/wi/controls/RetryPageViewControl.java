/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */

package com.citrix.wi.controls;

/**
 * This does not have any UI and is only used to update the delayed launch feedback
 * UI then redirect to the launcher page and also to redirect the main frame if an error occurs
 */
public class RetryPageViewControl {

    // Url to redirect to
    public String redirectUrl;

    // Flag to indicate whether to redirect the main page or just the hidden iframe
    public boolean redirectMainWindow;

    // Javascript for the successful retry of the delayed launch
    public String retrySuccessfulTag = null;

    // The display name for the desktop resource being retried
    public String desktopDisplayName = "";

    // Flag to indicate whether the launch can be automatically initiated via javascript
    public boolean canScriptLaunch = false;
}
