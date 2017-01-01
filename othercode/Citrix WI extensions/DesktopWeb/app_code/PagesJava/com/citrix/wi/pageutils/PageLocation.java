/*
 * Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils;

import java.util.ArrayList;
import java.util.Iterator;

/**
 * Enumeration of possible page locations (auth, site,...)
 */
public class PageLocation {

    private static final ArrayList ALL_INSTANCES = new ArrayList();

    /**
     * The page is in auth
     */
    public static final PageLocation AUTH = new PageLocation("auth");

    /**
     * The page is in site
     */
    public static final PageLocation SITE = new PageLocation("site");

    private String location = null;

    private PageLocation(String location) {
        this.location = location;
        ALL_INSTANCES.add(this);
    }

    /**
     * Converts this enumeration to a string.
     * @return one of:
     * <ul>
     * <li>Auth</li>
     * <li>Site</li>
     * </ul>
     */
    public String toString() {
        return location;
    }

    /**
     * Creates an instance of this enumeration from a string.
     * @param str the string to parse
     * @return an instance of <code>PageLocation</code>, or <code>null</code>
     * if <code>str</code> is not a valid value
     */
    public static final PageLocation fromString(String str) {
        PageLocation match = null;
        for (Iterator it = ALL_INSTANCES.iterator(); it.hasNext(); ) {
            PageLocation pageLocation = (PageLocation)it.next();
            if (pageLocation.toString().equals(str)) {
                match = pageLocation;
                break;
            }
        }
        return match;
    }


}
