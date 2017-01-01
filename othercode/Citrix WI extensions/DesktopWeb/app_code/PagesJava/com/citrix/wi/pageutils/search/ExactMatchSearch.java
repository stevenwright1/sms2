/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */

package com.citrix.wi.pageutils.search;

import com.citrix.wing.webpn.ResourceInfo;

/**
 * This class tries to find out if there is an exact match for the user
 * initiated search.
 */
public class ExactMatchSearch implements ISearch {
    // user specified search string.
    private String searchString = null;

    /**
     * Constructor which takes an array of resources and the search string.
     * @param resources, the ResourceInfo array
     * @param searchString, the string to search for.
     */
    public ExactMatchSearch(String searchString) {
        // Remove " or ' from both ends of the search string.
        if (searchString.startsWith("\"") || searchString.startsWith("\'")) {
            searchString = searchString.substring(1, searchString.length());
        }
        if (searchString.endsWith("\"") || searchString.endsWith("\'")) {
            searchString = searchString.substring(0, searchString.length() - 1);
        }
        this.searchString = searchString;
    }

    /**
     * Method to do the actual search and ranking.
     * see ISearch interface.
     */
    public SearchRank doRank(ResourceInfo resource, String fullDisplayName) {
        SearchRank rank = null;
        if (fullDisplayName.equalsIgnoreCase(searchString.trim())) {
            rank = new SearchRank(resource, fullDisplayName);
            rank.addMatchWeighting(1); // add a weight of 1 so that the isMatchFound() returns true.
        }
        return rank;
    }
}
