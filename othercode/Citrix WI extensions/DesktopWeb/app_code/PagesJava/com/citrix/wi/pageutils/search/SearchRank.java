/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */

package com.citrix.wi.pageutils.search;

import java.util.StringTokenizer;
import com.citrix.wing.webpn.ResourceInfo;

/**
 * This class determines the ranking of the searched items.
 * The items with greatest ranks will be displayed first.
 */
public class SearchRank {

    // This variable decides the weight according to the position of a particular string in the
    // search pattern. For example if user queries for a b c, the weights are (the last word always has 1):
    // a=3 b=2 c=1.
    // The resources named: Will get rank:
    // "a c d e"            3 + 1 = 4 (a+c)
    // "c d e w b"          1 + 2 = 3 (c+b)
    private int totalMatchWeighting = 0;

    // The resource object which corresponds to all the ranks.
    private ResourceInfo resourceInfo = null;

    // Variable for deciding how deep the item is in the folder hierarchy.
    private int folderLevel = 1; // assume root level by default

    // If there are multiple text strings to search for, this variable holds the total number of matches.
    private int numberOfMatches = 0;

    private String fullDisplayName = null;

    /**
     * Returns the total match weight.
     * @return the total match weight
     */
    public int getTotalMatchWeighting() {
        return totalMatchWeighting;
    }

    /**
     * Returns folder level indicating how deep the item is in the folder hierarchy.
     * @return folder level indicating how deep the item is in the folder hierarchy.
     */
    public int getFolderLevel() {
        return folderLevel;
    }

    /**
     * Returns the number of matching words.
     * @return the number of matching words
     */
    public int getNumberOfMatches() {
        return numberOfMatches;
    }

    /**
     * Constructor which takes a ResourceInfo object.
     * @param resourceInfo, the ResourceInfo object
     */
    public SearchRank(ResourceInfo resourceInfo, String fullDisplayName) {
        this.resourceInfo = resourceInfo;
        
        // Determine folder depth level.
        String folderPath = resourceInfo.getDisplayPath();
        this.folderLevel = new StringTokenizer(folderPath, "\\").countTokens();
        this.fullDisplayName = fullDisplayName;
    }

    public String getFullDisplayName() {
        return fullDisplayName;
    }

    /**
     * Retrieves the resource for this rank.
     * @return the corresponding resource.
     */
    public ResourceInfo getResourceInfo() {
        return this.resourceInfo;
    }

    /**
     * Tells whether this resource name has any matches for the searched string.
     * @return true or false.
     */
    public boolean isMatchFound() {
        return this.totalMatchWeighting > 0;
    }

    /**
     * This method increments the number of matches when the search string
     * has more than 1 word to search for.
     */
    public void incrementMatches() {
        numberOfMatches++;
    }
    /**
     * This method adds the weight to the current rank.
     * @param weight the weight to add.
     */
    public void addMatchWeighting(int weighting) {
        totalMatchWeighting += weighting;
    }
}
