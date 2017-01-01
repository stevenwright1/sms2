// LaunchApplication.java
// Copyright (c) 2009 - 2010 Citrix Systems, Inc. All Rights Reserved.
// [NFuseVersionAndBuildNumber]
package com.citrix.wi.pages.site;

import com.citrix.wi.mvc.WIContext;
import com.citrix.wing.webpn.ApplicationInfo;
import com.citrix.wing.webpn.ResourceInfo;


/**
 * Class responsible for handling application launching.
 */
public class LaunchApplication extends LaunchResource
{
    LaunchApplication(WIContext wiContext, ApplicationInfo appInfo, String appID)
    {
        super(wiContext, (ResourceInfo)appInfo, appID);
    }
}
