/*
 * Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils;

import java.util.ArrayList;

import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wing.util.Strings;

/**
 * Helper class to record pages the user has visited
 */
public class PageHistory {

    /**
     * Retrieves the page history and location in the session
     * If this is the first time we are here, creates an empty list.
     * If we have moved from pre-Login to post-Login pages then resets the list
     */
    public static void initPageHistory(WebAbstraction web) {
        ArrayList pageHistory = getPageStack(web, SV_PAGE_HISTORY);
        ArrayList startPages  = getPageStack(web, SV_START_PAGE);
        PageLocation lastLocation = (PageLocation)web.getSessionAttribute(SV_PAGE_LOCATION);

        PageLocation location = Include.isLoggedIn(web) ? PageLocation.SITE : PageLocation.AUTH;

        // Always clear out the page history when switching
        // from Auth to Site, but must leave the start pages

        if ((pageHistory == null) || (lastLocation != location))
        {
            pageHistory = new ArrayList();
            web.setSessionAttribute(SV_PAGE_HISTORY, pageHistory);
            web.setSessionAttribute(SV_PAGE_LOCATION, location);
        }

        // Only create the start pages if they are not there,
        // do not renew them when moving from Auth to Site, otherwise
        // the initial post login stack is gone!

        if (startPages == null)
        {
            startPages = new ArrayList();
            web.setSessionAttribute(SV_START_PAGE, startPages);
        }
    }

    /**
     * Specify the first and only page to present to the user after login is complete.
     */
    public static void setInitialPostLoginPage(WebAbstraction web, String pageUrl) {
        ArrayList pages = getPageStack(web, SV_START_PAGE);
        if (pages == null)
        {
            pages = new ArrayList();
            web.setSessionAttribute(SV_START_PAGE, pages);
        }
        else
        {
            pages.clear();
        }

        pages.add(pageUrl);
    }

    /**
     * Specify a page to present to the user after login is complete.
     */
    public static void addToPostLoginPages(WebAbstraction web, String pageURL) {
        ArrayList pages = getPageStack(web, SV_START_PAGE);
        String lastPage = "";
        if (pages.size() >= 1) {
            lastPage = pages.get(pages.size() - 1).toString();
        }

        if (!Strings.equalsIgnoreCase(pageURL, lastPage)) {
            pages.add(pageURL);
        }
    }

    /**
     * Returns the next post-login page and pops it off the page stack.
     * 
     * @param web WebAbstraction
     */
    public static String getNextPostLoginPage(WebAbstraction web) {
        String result = null;
        ArrayList pages = getPageStack(web, SV_START_PAGE);
        if (pages.size() >= 1) {
            result = pages.get(pages.size() - 1).toString();
            pages.remove(pages.size() - 1);
        }

        return result;
    }

    /**
     * Records the current page's URL where it can be accessed after a refresh
     * Also used to determine with which page to update the frameset after logged in
     */
    public static void recordCurrentPageURL(WebAbstraction web) {
        String path = web.getRequestPath();
        ArrayList pages = getPageStack(web, SV_PAGE_HISTORY);
        String lastPage = "";
        if (pages.size() >= 1) {
            lastPage = pages.get(pages.size() - 1).toString();
        }

        if (!Strings.equalsIgnoreCase(path, lastPage)) {
            pages.add(path);
        }
    }

    /**
     * Returns the last recorded page.
     * 
     * Callers who want to use this method to redirect to the last recorded page using Javascript will probably
     * also want to pop that page off the page history stack - so they should use the
     * getCurrentPageURL(WebAbstraction, boolean) form of this method instead.
     *
     * @param web WebAbstraction
     */
    public static String getCurrentPageURL(WebAbstraction web) {
        return getCurrentPageURL(web, false);
    }

    /**
     * Returns the last recorded page and optionally pops it off the page history stack.
     * 
     * This method is useful for callers who want to redirect to the last recorded
     * page using using Javascript.
     *
     * @param web WebAbstraction
     * @param pop if <code>true</code>, the current page is popped off the page history stack, otherwise
     * the page history stack is unaffected.
     */
    public static String getCurrentPageURL(WebAbstraction web, boolean pop) {
        String result = null;
        ArrayList pages = getPageStack(web, SV_PAGE_HISTORY);
        if (pages.size() >= 1) {
            result = pages.get(pages.size() - 1).toString();
            if (pop) {
                pages.remove(pages.size() - 1);
            }
        } else {
            if (Include.isLoggedIn(web)) {
                // We've logged just logged in
                result = getNextPostLoginPage(web);
            } else {
                // We have not yet logged in, just say it should be login page
                result = web.getApplicationPath() + "/auth/" + Constants.PAGE_LOGIN;
            }
        }

        return result;
    }

    /**
     * Redirects the user to the last visited page,
     * If we can not find the page, fallbacks to the Login page for pre-Login
     * and Applist after logged in
     *
     * @param web WebAbstraction
     * @param queryStr the query string to include in the redirect URL
     */
    public static void redirectToLastPage(WIContext wiContext, String queryStr) {
        WebAbstraction web = wiContext.getWebAbstraction();
        ArrayList pages = getPageStack(web, SV_PAGE_HISTORY);

        // We are redirecting so remove the current page (self) from the stack
        if (pages.size() >= 1) {
            pages.remove(pages.size() - 1);
        }
        String redirectURL = PageHistory.getCurrentPageURL(web);
        redirectToPage(wiContext, redirectURL, queryStr);
    }

    /**
     * Redirect to the current page (used for example after switching graphics mode).
     *
     * @param wiContext WIContext
     */
    public static void redirectToCurrentPage(WIContext wiContext) {
        WebAbstraction web = wiContext.getWebAbstraction();
        String redirectURL = PageHistory.getCurrentPageURL(web);

        redirectToPage(wiContext, redirectURL, "");
    }

    /**
     * Redirect to the Home page (used after saving settings in preferences page).
     * Pre-login: Logon screen
     * Post login: AccessPoint screen
     *
     * @param wiContext WIContext
     */
    public static void redirectToHomePage(WIContext wiContext, String queryStr) {
        redirectToPage(wiContext, null, queryStr);
    }

    /**
     * Redirect to the given page URL. If the supplied page URL is null, then
     * redirect to the home page.
     *
     * @param wiContext WIContext
     * @param redirectURL the URL to redirect to
     * @param queryStr the query string to append to the redirect URL
     * @param generateFrameset whether to redirect to the frameset page
     */
    private static void redirectToPage(WIContext wiContext, String redirectURL, String queryStr) {
        WebAbstraction web = wiContext.getWebAbstraction();
        if (Include.isLoggedIn(web)) {
            if (redirectURL == null) {
                if (LaunchUtilities.getDirectLaunchModeInUse(wiContext)) {
                    redirectURL = Constants.PAGE_DIRECT_LAUNCH;
                } else {
                    redirectURL = Constants.PAGE_APPLIST;
                }
            }
        } else {
            if (redirectURL == null) {
                redirectURL = Constants.PAGE_LOGIN;
            }
       }
        redirectURL += queryStr;
        web.clientRedirectToUrl(redirectURL);
    }

    private static ArrayList getPageStack(WebAbstraction web, String type) {
        ArrayList pageStack = (ArrayList)web.getSessionAttribute(type);
        if (pageStack != null) {
            while (pageStack.size() > MAX_PAGE_STACK_SIZE) {
                // Get rid of the older pages
                pageStack.remove(0);
            }
        }

        return pageStack;
    }

    // Don't let page stacks grow bigger than this
    private static int MAX_PAGE_STACK_SIZE = 8;

    // Session variables used by this class
    private static final String SV_PAGE_HISTORY = "CTX_PageHistory";
    private static final String SV_PAGE_LOCATION = "CTX_PageLocation";
    private static final String SV_START_PAGE = "CTX_StartPage";
}
