/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils.search;

import com.citrix.wing.webpn.ResourceInfo;

/**
 * This interface does a search and return the results in an array list
 */
public interface ISearch {
    /**
     * Marker method for doing a search
     * 
     * This method takes a resource object and returns a search rank for the
     * particular object after doing a user specified string search.
     *
     * @param resource resource to rank
     * @param fullDisplayName full display name of the resource
     * @return a search rank for the corresponding resource if a match is found,
     *          else return null.
     */
    public SearchRank doRank(ResourceInfo resource, String fullDisplayName);
}
