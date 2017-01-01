/*
 * Copyright (c) 2008 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils;

import com.citrix.wing.AccessTokenException;
import com.citrix.wing.ResourceUnavailableException;
import com.citrix.wing.webpn.ResourceInfo;
import com.citrix.wing.webpn.UserContext;

/**
 * Provides a set of useful utility methods for resource enumeration
 */
public class ResourceEnumerationUtils {

    /**
     * Returns a resource give the resource's Resource ID (usually passed in
     * through a browser query string), and the user context.
     * @param resourceId An identifier for a published resource
     * @param userContext
     * @return A ResourceInfo object used for launches of varying types.
     */
    public static ResourceInfo getResource(String resourceId, UserContext userContext) {
        ResourceInfo resInfo = null;
        try {
            resInfo = userContext.findResourceInfo(resourceId);
        } catch(AccessTokenException ignore) { } catch(ResourceUnavailableException ignore) { }
        return resInfo;
    }
}
