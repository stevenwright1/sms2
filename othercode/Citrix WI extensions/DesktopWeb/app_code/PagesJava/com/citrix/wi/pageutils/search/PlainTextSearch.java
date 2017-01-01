/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */

package com.citrix.wi.pageutils.search;

import com.citrix.wing.webpn.ResourceInfo;
import com.citrix.wing.util.Strings;

/**
 * This class is used for plain text searches where the search string can be a
 * single or multiple word texts.
 */
public class PlainTextSearch implements ISearch {
    // user specified search tokens separated by white space.
    private String[] searchTokens = null;
    // the number of tokens in the search string is given the maximum weight.
    private final int maxWeighting;

    /**
     * Constructor which takes a search string.
     * @param searchString, the string to search for.
     */
    public PlainTextSearch(String searchString) {
        // Store the tokens in an array for looping through the resource for a match
        searchTokens = Strings.split(searchString, ' ');
        // Assign the number of tokens as the maximum weighting.
        maxWeighting = searchTokens.length;
    }

    /**
     * Method to do the actual search and ranking.
     * @see ISearch.
     */
    public SearchRank doRank(ResourceInfo resource, String fullDisplayName) {
        int weighting = maxWeighting;
        SearchRank rank = new SearchRank(resource, fullDisplayName);
        String lowercaseFullDisplayName = fullDisplayName.trim().toLowerCase();
        for (int j = 0; j < searchTokens.length; j++) { // iterate through the multiple word search query
            String token = searchTokens[j];
            if (lowercaseFullDisplayName.indexOf(token) != -1) {
                rank.addMatchWeighting(weighting);
                rank.incrementMatches();
            }
            // decrement the weight by one for the next token
            weighting--;
        }
        return rank.isMatchFound() ? rank : null;
   }

}
