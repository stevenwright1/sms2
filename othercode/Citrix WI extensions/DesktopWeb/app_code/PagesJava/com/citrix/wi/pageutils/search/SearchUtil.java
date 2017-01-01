/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils.search;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

import com.citrix.wing.AccessTokenException;
import com.citrix.wing.ResourceUnavailableException;
import com.citrix.wing.util.Strings;
import com.citrix.wing.webpn.ApplicationInfo;
import com.citrix.wing.webpn.DesktopInfo;
import com.citrix.wing.webpn.DocumentInfo;
import com.citrix.wing.webpn.ResourceInfo;
import com.citrix.wing.webpn.ResourceInfoSet;
import com.citrix.wing.webpn.SearchStatus;
import com.citrix.wi.controls.DesktopGroupsController;
import java.util.Iterator;

public class SearchUtil {
    /**
     * Searches the resource names in the resource list for a user provided search text and returns the results.
     *
     * The returned <code>ResourceInfo[]</code> contains the search results.
     *
     * @param searchString the text used for searching to find a match
     * @return The list of the resources found during search.
     *
     * The currently supported search criterias are exact match search, wild char search
     * and plain text search. The currently supported wild char search sequences  are
     *            *txt              -- Search for any application names which ends with the word txt
     *            txt*              -- Search for any application names which starts with the word txt.
     *            txt1*txt2         -- Search for any application names which starts with txt1 and ends with txt2
     *            *txt1*txt2*txt3*  -- Search for any application names which contains the words txt1, txt2 and txt3 in the specified order.
     *            *                 -- Displays all the application names.
     *
     * For an exact match you should specify the search string in double quotes like "txt1"
     *
     * Plain text search includes just specifying multiple texts with space in between
     * eg. txt1 txt2 txt3
     *
     * Also note that the above mentioned different types of searches should not be mixed.
     */
    private static ISearch determineSearchStrategy(String searchString) {
        ISearch searchStrategy = null;
        if (searchString.startsWith("\"") && searchString.endsWith("\"") ||
            searchString.startsWith("\'") && searchString.endsWith("\'")) { // eg. "txt" or 'txt'
            searchStrategy = new ExactMatchSearch(searchString);
        } else if (searchString.indexOf("*") != -1) {
            searchStrategy = new WildCardSearch(searchString);
        } else {
            searchStrategy = new PlainTextSearch(searchString);
        }
        return searchStrategy;
    }

    /**
     * Searches given resource set for matches with search string.
     * 
     * @param searchString search string
     * @param resourcesToSearchIn resources to search within
     * @param controller desktops controller used to obtain compund desktop name
     * @return An array of <code>ResourceInfo</code> mathing the search string.
     */
    public static ResourceInfo[] findSearchResults(String searchString,
                                                   ResourceInfoSet resourcesToSearchIn,
                                                   DesktopGroupsController controller)
        throws AccessTokenException,
               ResourceUnavailableException {

        if (searchString == null || searchString.equals("")) {
            return new ResourceInfo[0];
        }

        ResourceInfo[] resources = resourcesToSearchIn.getResources();
        if (resourcesToSearchIn.getResources().length == 0 &&
            resourcesToSearchIn.getStatus() != SearchStatus.OK) {
            return new ResourceInfo[0];
        }

        List searchResults = new ArrayList();

        searchString = searchString.trim().toLowerCase();
        ISearch searchStrategy = determineSearchStrategy(searchString);
        for (int i = 0; i < resources.length; i++) {
            if (resources[i] instanceof DocumentInfo || resources[i] instanceof ApplicationInfo) {
                String fullDisplayName = resources[i].getDisplayName();
                if (resources[i] instanceof DesktopInfo) {
                    // Retrieve compound name for desktops.
                    fullDisplayName = controller.getCompoundDesktopDisplayName(resources[i].getDisplayName(),
                                                                               resources[i].getId());
                }
                SearchRank rank = searchStrategy.doRank(resources[i], fullDisplayName);
                // Rank will be null if there was no match during the search.
                if (rank != null) {
                    searchResults.add(rank);
                }
            }
        }

        return getSortedSearchResults(searchResults);
    }

    private static ResourceInfo[] getSortedSearchResults(List searchResults) {
        Collections.sort(searchResults, new SearchComparator());

        ResourceInfo[] sortedSearchResults = new ResourceInfo[searchResults.size()];

        Iterator it = searchResults.iterator();
        for (int i = 0; it.hasNext(); i++) {
            SearchRank rank = (SearchRank)it.next();
            sortedSearchResults[i] = rank.getResourceInfo();
        }
        return sortedSearchResults;
    }

}

/**
 * Comparator for SearchRank objects.
 * Defines the order in which the search result is displayed.
 */
class SearchComparator implements Comparator {
    public int compare(Object o1, Object o2) {
        final int EQUAL = 0;

        if (!(o1 instanceof SearchRank) || !(o2 instanceof SearchRank)) {
            throw new IllegalArgumentException("Compared object must be of type ISearchRank!");
        }

        SearchRank r1 = ((SearchRank)o1);
        SearchRank r2 = ((SearchRank)o2);

        // First check for the weightage.
        // The higher the value, the better search rank.
        int compareValue = new Integer(r2.getTotalMatchWeighting()).compareTo(new Integer(r1.getTotalMatchWeighting()));

        // If both weights are are equal, check for the number of matches.
        // The higher the value, the better search rank.
        if (compareValue == EQUAL) {
            compareValue = new Integer(r2.getNumberOfMatches()).compareTo(new Integer(r1.getNumberOfMatches()));
        }

        // Now look for the folder hierarchy.
        // The lower the value, the better search rank.
        if (compareValue == EQUAL) {
            compareValue = new Integer(r1.getFolderLevel()).compareTo(new Integer(r2.getFolderLevel()));
        }

        // Finally, sort alpabetically.
        // The lower (lexicographically) the value, the better search rank.
        if (compareValue == EQUAL) {
            compareValue = Strings.compare(r1.getFullDisplayName(), r2.getFullDisplayName());
        }

        return compareValue;
    }
}
