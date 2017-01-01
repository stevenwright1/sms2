/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.controls;

/**
 * Maintains presentation state for contents of the search results tab.
 */
public class SearchResultsControl extends PageControl {

    /**
     * The resources to display in the search results tab.
     */
    public ResourceControl[] resources = new ResourceControl[0];

    /**
     * Whether to show a Next page link
     */
    public boolean showNextLink = false;

    /**
     * Whether to show a Previous page link
     */
    public boolean showPreviousLink = false;

    /**
     * The current search results page number
     */
    public int currentPageNumber = 1;

    /**
     * The total number of search results
     */
    public String nTotal = "";

    /**
     * The start number of results currently displaying from the complete search results
     */
    public String nStartCurrentSearchResults = "";

    /**
     * The end number of results currently displaying from the complete search results
     */
    public String nEndCurrentSearchResults = "";

    /**
     * Gets the number of resource to display.
     */
    public int getNumResources() {
        return resources.length;
    }

    /**
     * The query these search results are for.
     */
    public String searchQuery;
}
