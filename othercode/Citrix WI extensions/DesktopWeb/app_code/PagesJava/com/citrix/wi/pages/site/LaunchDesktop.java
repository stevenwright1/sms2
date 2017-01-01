// LaunchDesktop.java
// Copyright (c) 2009 - 2010 Citrix Systems, Inc. All Rights Reserved.
// [NFuseVersionAndBuildNumber]
package com.citrix.wi.pages.site;

import com.citrix.wi.mvc.WIContext;
import com.citrix.wing.webpn.DesktopInfo;
import com.citrix.wing.webpn.ResourceInfo;

/**
 * Class responsible for handling desktop launching.
 */
public class LaunchDesktop extends LaunchResource
{
    LaunchDesktop(WIContext wiContext, DesktopInfo desktopInfo, String appID)
    {
        super(wiContext, (ResourceInfo)desktopInfo, appID);
    }

}
