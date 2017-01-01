/*
 * ResourceGroupControl.java
 * Copyright (c) 2007 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.controls;

import com.citrix.wi.controls.ResourceControl;

/**
 * Control representing sections in the Groups applist view
 */
public class ResourceGroupControl {

    public ResourceControl[] contents;
    public String title;
    private boolean isDesktopGroup;

    /**
     * Constructor.
     * @param title The display title of this group.
     * @param isDesktopGroup whether the group is a desktop group or not.
     * @param contents the <code>ResourceControl</code> objects contained in this group.
     */
    public ResourceGroupControl( String title, boolean isDesktopGroup, ResourceControl[] contents) {
        this.title = title;
        this.isDesktopGroup = isDesktopGroup;
        this.contents = contents;
    }

    /**
     * Gets the number of resources in the group.
     * @return number of resources
     */
    public int getNumResources() {
        return ( contents == null ) ? 0 : contents.length;
    }

    /**
     * Checks whether its a desktop group or not.
     * @return true if its a desktop group
     */
    public boolean isDesktopGroup() {
        return isDesktopGroup;
    }
}

