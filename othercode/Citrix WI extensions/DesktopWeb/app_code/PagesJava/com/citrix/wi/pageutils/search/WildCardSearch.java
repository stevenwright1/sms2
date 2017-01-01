/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */

package com.citrix.wi.pageutils.search;

import com.citrix.wing.webpn.ResourceInfo;
import com.citrix.wing.util.Strings;

/**
 * This class is used for searches where the search string contains wildcard(s).
 */
public class WildCardSearch implements ISearch {

    // eg. txt*
    private boolean firstWordMatch = true;
    // eg. *txt
    private boolean lastWordMatch = true;
    // String array containing the user specified search tokens.
    private String[] searchTokens;

    /**
     * Constructor which takes an array of resources and the search string.
     * @param resources, the ResourceInfo array
     * @param searchString, the string to search for.
     */
    public WildCardSearch(String searchString) {
        this.searchTokens = Strings.split(searchString, '*');

        // eg. *txt
        if (searchString.startsWith("*")) {
            firstWordMatch = false;
        }
        // eg. txt*
        if (searchString.endsWith("*")) {
            lastWordMatch = false;
        }
    }

    /**
     * Method to do the actual search and ranking.
     * @see ISearch.
     */
    public SearchRank doRank(ResourceInfo resource, String fullDisplayName) {
        SearchRank rank = null;
        if (performWildCardSearch(fullDisplayName.trim().toLowerCase())) {
            rank = new SearchRank(resource, fullDisplayName);
            rank.addMatchWeighting(1);
        }
        return rank;

    }
    // performs a wild card search.The supported search sequences are
    // *txt              -- Search for any application names which ends with the word txt
    // txt*              -- Search for any application names which starts with the word txt.
    // txt1*txt2         -- Search for any application names which starts with txt1 and ends with txt2
    // *txt1*txt2*txt3*  -- Search for any application names which contains the words txt1, txt2 and txt3 in the specified order.
    // *                 -- Displays all the application names.
    private boolean performWildCardSearch(String appName) {
        int count = 0;
        int startIndex = 0;
        int numTokens = searchTokens.length;

        for (int i = 0; i < numTokens; i++) {
            count++;
            String token = searchTokens[i].trim();
            if (count == 1 && firstWordMatch && !appName.startsWith(token)) {
                return false;
            }

            if (count == numTokens && lastWordMatch && !appName.endsWith(token)) {
                return false;
            }

            if (appName.equals(token)) {
                return true;
            }

            int index = appName.indexOf(token, startIndex);
            if (index < 0) {
                return false;
            }

            startIndex = appName.indexOf(token) + token.length();
        }

        return true;
    }
}
