package com.citrix.wi.controls;

public class LauncherControl {

    // Javascript launching tag
    public String launchTag = null;

    // Application title
    public String decodedTitle = null;

    // Link to favicon
    public String faviconLink = null;

    // Path to redirect to in case of error condition
    public String redirectUrl = null;

    // flag to indicate passthrough enabled
    public boolean isPassthroughEnabled = false;

    // Javascript error tag for no bookmark URLs
    public String passthroughErrorUrl = null;
}
