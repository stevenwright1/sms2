// LaunchResult.java
// Copyright (c) 2009 - 2010 Citrix Systems, Inc. All Rights Reserved.
// [NFuseVersionAndBuildNumber]

package com.citrix.wi.pages.site;

/**
 * Helper class used for indicating launch result.
 */
public class LaunchResult
{
    // failure codes are negative
    public static final int ERR_APP_REMOVED = -1;
    public static final int ERR_NO_CLIENTS  = -2;
    public static final int ERR_NO_RADE_CLIENTS = -3;
    public static final int ERR_LAUNCH_ERROR = -4;
    public static final int ERR_NOT_SUPPORTED = -5;
    public static final int DELAYED_LAUNCH = -6;

    // success codes are positive
    public static final int SUCCESS = 1;
    public static final int REDIRECTED = 2;

    public int resultCode = LaunchResult.SUCCESS;
    public String redirectUrl = null;
    public String launchTag = null;

    public boolean isSuccess()
    {
        return resultCode > 0;
    }

    public boolean isRedirected()
    {
        return resultCode == LaunchResult.REDIRECTED;
    }

    LaunchResult() { };
}
