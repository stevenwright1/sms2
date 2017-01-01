/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.site;

import java.io.IOException;
import java.util.Iterator;
import com.citrix.wi.IconCache;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.controls.ApplistPageControl;
import com.citrix.wi.controls.ResourceControl;
import com.citrix.wi.controls.RetryControl;
import com.citrix.wi.controls.SearchResultsControl;
import com.citrix.wi.pages.StandardLayout;
import com.citrix.wi.pageutils.ApplistUtils;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.DelayedLaunchUtilities;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.LaunchUtilities;
import com.citrix.wi.pageutils.ResourceEnumerationUtils;
import com.citrix.wi.pageutils.SessionUtils;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wi.pageutils.Utils;
import com.citrix.wi.pageutils.search.SearchUtil;
import com.citrix.wing.AccessTokenException;
import com.citrix.wing.MessageType;
import com.citrix.wing.ResourceUnavailableException;
import com.citrix.wing.util.Strings;
import com.citrix.wing.util.WebUtilities;
import com.citrix.wing.webpn.ApplicationInfo;
import com.citrix.wing.webpn.DesktopInfo;
import com.citrix.wing.webpn.DocumentInfo;
import com.citrix.wing.webpn.ResourceInfo;
import com.citrix.wing.webpn.UserContext;
import com.citrix.wi.controls.DesktopGroupsController;

public class SearchResults extends StandardLayout {

    protected SearchResultsControl viewControl = new SearchResultsControl();

    public SearchResults(WIContext wiContext) {
        super(wiContext);
        wiContext.getWebAbstraction().setRequestContextAttribute("searchControl", viewControl);
    }

    private void setupWelcomeControl() {
        welcomeControl.setTitle(wiContext.getString("SearchResults"));
    }

    private void setupSearchBoxControl() {
        searchBoxControl.show = ApplistUtils.isSearchEnabled(wiContext);
        searchBoxControl.query = ApplistUtils.retrieveSearchQuery(wiContext);
    }

    protected void setupNavControl() {
        super.setupNavControl();
    }

    protected String getBrowserPageTitleKey() {
        return "BrowserTitleSearchResults";
    }

    protected boolean performImp() throws IOException {
        // processQueryStrings saves the current search query in the session. It is called
        // prior to setting up the search box control, which retrieves the query from the session.
        processQueryStrings();

        layoutControl.isSearchPage = true;
        layoutControl.hasCancelButton = true;
        setupWelcomeControl();
        setupSearchBoxControl();
        setupNavControl();

        // Get the number of results to display on each page
        int resultsPerPage = 0;
        String resultsPerPageString = wiContext.getString(
            Include.isCompactLayout(wiContext) ? "LGDefaultSearchResultsPerPage" : "DefaultSearchResultsPerPage");
        resultsPerPage = ApplistUtils.parsePositiveInteger(resultsPerPageString, 20 /* default */ );

        UserContext userContext = SessionUtils.checkOutUserContext(wiContext);
        ResourceInfo[] resourceInfos = retrieveSearchResults();
        if (resourceInfos == null) {
            // We don't have the search results for the current query string saved in the session, so find them
            // then save them away in the session. By saving them in the session, it means we don't
            // have to re-calculate eg. if the user hits the "next" link.
            try {
                DesktopGroupsController controller = DesktopGroupsController.getInstance(wiContext);
                resourceInfos = ApplistUtils.removeHiddenResources(wiContext,
                    SearchUtil.findSearchResults(retrieveSearchQuery().trim(), userContext.findAllVisibleResources(), controller));
                saveSearchResults(resourceInfos);
            } catch (AccessTokenException ate) {
                UIUtils.handleLogout(wiContext, MessageType.ERROR, Utils.getAuthErrorMessageKey(ate));
                return false;
            } catch (ResourceUnavailableException rue) {
                UIUtils.handleLogout(wiContext, MessageType.ERROR, "AppEnumerationError");
                return false;
            }
        }

        // Retrieve the current page number from the session - it must be a positive integer
        int pageNumber = retrievePageNumber();

        //Check for a huge page number value
        if (resourceInfos.length < (pageNumber - 1) * resultsPerPage) {
            // There isn't anything to display on this pagenumber, so revert to the first page
            pageNumber = 1;
            savePageNumber(pageNumber);
        }

        // Work out which resources we are going to display on the current page

        // The index of the first item on the page
        int firstItem = 0;
        // The index of the last item on the page
        int lastItem = 0;
        // The number of resources on the page
        int numViewableResources = 0;

        if (resourceInfos.length > 0) {
            // The index of the first item on the page
            firstItem = (pageNumber - 1) * resultsPerPage;
            // The index of the last item on the page
            lastItem = (pageNumber * resultsPerPage) - 1;
            if (lastItem >= resourceInfos.length) {
                lastItem = resourceInfos.length - 1;
            }

            viewControl.showPreviousLink = (firstItem > 0);
            viewControl.showNextLink = (lastItem < (resourceInfos.length - 1));
            numViewableResources = lastItem - firstItem + 1;
        }

        viewControl.resources = new ResourceControl[numViewableResources];

        // Populate content arrays
        for (int i = 0; i < numViewableResources; i++) {
            ResourceInfo resInfo = resourceInfos[i + firstItem];
            ResourceControl resource = ResourceControl.fromResourceInfo(resInfo,
                                                                        wiContext,
                                                                        userContext,
                                                                        Constants.ICON_SIZE_16);

            String resourceType = getResourceType(resInfo);
            resource.pathUrl = getSearchPathUrl(resInfo.getDisplayPath(), resourceType);
            resource.pathText = getSearchPathText(resInfo.getDisplayPath(), resourceType);

            viewControl.resources[i] = resource;
        }
        viewControl.currentPageNumber = pageNumber;
        viewControl.nStartCurrentSearchResults = Integer.toString(firstItem + 1);
        viewControl.nEndCurrentSearchResults = Integer.toString(lastItem + 1);
        viewControl.nTotal = Integer.toString(resourceInfos.length);
        viewControl.searchQuery = retrieveSearchQuery();

        SessionUtils.returnUserContext(userContext);

        return true; // always display
    }

    // processes query strings related to the search page - namely search query and results page number
    private void processQueryStrings() {
        // if new query terms received - save terms, clear existing
        String searchQuery = getQueryFromQueryString(wiContext.getWebAbstraction());
        if (validateSearchQuery(searchQuery)) {
            saveSearchQuery(searchQuery);
        }

        // if new page number received - save page number.
        int queryPageNumber = getPageNumberFromQueryString(wiContext.getWebAbstraction());
        if (validatePageNumQuery(queryPageNumber)) {
            savePageNumber(queryPageNumber);
        }
    }

    // Retrieve the search query string from the session.
    // If there is no item in the session, default to an empty string.
    private String retrieveSearchQuery() {
        String searchQuery = (String)wiContext.getWebAbstraction().getSessionAttribute(Constants.SV_SEARCH_QUERY);
        if (searchQuery != null) {
            return searchQuery;
        }

        return "";
    }

    // Save the search results in the session
    private void saveSearchResults(ResourceInfo[] resources) {
        wiContext.getWebAbstraction().setSessionAttribute(Constants.SV_CURRENT_SEARCH_RESULTS, resources);
    }

    // Retrieve the current search results page number from the session.
    // Default to page 1 if no item is in the session.
    private int retrievePageNumber() {

        Integer pageNumerInt = (Integer)wiContext.getWebAbstraction().getSessionAttribute(Constants.SV_CURRENT_PAGE_NUMBER);
        if (pageNumerInt != null) {
            int pageNumber = pageNumerInt.intValue();
            return pageNumber;
        }

        // default
        return 1;
    }

    // Save the current search results page number in the session.
    private void savePageNumber(int pageNumber) {
        wiContext.getWebAbstraction().setSessionAttribute(Constants.SV_CURRENT_PAGE_NUMBER, new Integer(pageNumber));
    }

    // Retrieve the search results from the session. If there are no search results in
    // the session, a null is returned.
    private ResourceInfo[] retrieveSearchResults() {
        return (ResourceInfo[])wiContext.getWebAbstraction().getSessionAttribute(Constants.SV_CURRENT_SEARCH_RESULTS);
    }

    // This function constructs the link to the page containing all the contents inside the passed in folder
    // This is used with the search results so that the user can click on the path link in order to get to the
    // corresponding folder contents.
    private String getSearchPathUrl(String folderPath, String resourceType) {
        String path = Constants.PAGE_APPLIST + "?" + Constants.QSTR_CURRENT_FOLDER + "=" + WebUtilities.escapeURL(folderPath)
                                             + "&amp;" + Constants.QSTR_SWITCH_TO_RESOURCE_VIEW + "=" + WebUtilities.escapeURL(resourceType);
        return path;
    }

    // Constuct the text to go in a folder link in the search results page
    private String getSearchPathText(String folderPath, String resourceType) {
        int numTabs = wiContext.getConfiguration().getUIConfiguration().getAccessPointTabs().length;

        // Stick the root folder name on the front if there is only one tab
        String rootFolderPrefix = wiContext.getString("RootFolderName");

        if (numTabs > 1) {
            // If there are multiple tabs, stick the resource type on the front
            // instead of root folder name.
            if (resourceType.equals(Constants.QSTR_RESOURCE_APP)) {
                rootFolderPrefix = wiContext.getString("Applications");
            } else if (resourceType.equals(Constants.QSTR_RESOURCE_CONTENT)) {
                rootFolderPrefix = wiContext.getString("Content");
            } else if (resourceType.equals(Constants.QSTR_RESOURCE_DESKTOP)) {
                rootFolderPrefix = wiContext.getString("Desktops");
                // For the case of desktops, return just the root folder name,
                // as we do not display folders in multiple desktop case.
                return rootFolderPrefix;
            }
        }

        // Get rid of trailing slash
        String searchPathText = folderPath.substring(0, folderPath.length() - 1);

        // Stick prefix on the front
        searchPathText = rootFolderPrefix + searchPathText;

        // Substitute backslashes with more user-friendly seperator
        searchPathText = Strings.replace(searchPathText, "\\", " > ");

        return searchPathText;
    }

    /**
     * Gets whether a valid search query was received with the current page request.
     * @param web the web abstraction which can be used to access the query strings
     * @return <code>true</code> if a valid search query string exists, otherwise <code>false</code>.
     */
    public static boolean newQueryReceived(WebAbstraction web) {
        return validateQueryStrings(web);
    }

    // Checks whether the search query received is valid, checking for non-null search term and non-negative page number
    private static boolean validateQueryStrings(WebAbstraction web) {
        String searchQuery = getQueryFromQueryString(web);
        int pageNumber = getPageNumberFromQueryString(web);

        return validateSearchQuery(searchQuery) || validatePageNumQuery(pageNumber);
    }

    // Checks whether the given search query is valid for processing - (non null & non empty)
    private static boolean validateSearchQuery(String query) {
        return !Strings.isEmptyOrWhiteSpace(query);
    }

    // Checks whether the given page number is valid for processing - (greater than zero)
    private static boolean validatePageNumQuery(int pageNum) {
        return pageNum > 0;
    }

    private static String getQueryFromQueryString(WebAbstraction web) {
        // no need to check input here, as it is html encoded before being output
        return web.getQueryStringParameter(Constants.QSTR_SEARCH_STRING);
    }

    private static int getPageNumberFromQueryString(WebAbstraction web) {
        String pageString = web.getQueryStringParameter(Constants.QSTR_SEARCH_RESULTS_PAGE_NO);
        int pageNumber = ApplistUtils.parsePositiveInteger(pageString, -1 /* default to an invalid value */);
        return pageNumber;
    }

    // Save the search query string in the session.
    // If the query string has changed from last time, clear out the old search results.
    private void saveSearchQuery(String searchQuery) {
        WebAbstraction web = wiContext.getWebAbstraction();
        String previoiusSearchQuery = (String)web.getSessionAttribute(Constants.SV_SEARCH_QUERY);
        if (!searchQuery.equals(previoiusSearchQuery)) {
            // As its a new search clear out the old results.
            web.setSessionAttribute(Constants.SV_SEARCH_QUERY, searchQuery);
            web.setSessionAttribute(Constants.SV_CURRENT_SEARCH_RESULTS, null);
            web.setSessionAttribute(Constants.SV_CURRENT_PAGE_NUMBER, null);
        }
    }

    // Gets the query string value corresponding to the actual type of the given ResourceInfo object.
    // Returns the empty string if the appropriate query string value isn't defined.
    private static String getResourceType(ResourceInfo resInfo) {
        if (resInfo instanceof DocumentInfo) {
            return Constants.QSTR_RESOURCE_CONTENT;
        }
        if (resInfo instanceof DesktopInfo) {
            return Constants.QSTR_RESOURCE_DESKTOP;
        }
        if (resInfo instanceof ApplicationInfo) {
            return Constants.QSTR_RESOURCE_APP;
        }
        return "";
    }

}
