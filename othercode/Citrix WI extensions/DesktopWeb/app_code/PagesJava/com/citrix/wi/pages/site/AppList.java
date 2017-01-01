/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.site;

import java.io.IOException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;

import com.citrix.wi.UserPreferences;
import com.citrix.wi.controls.ApplistPageControl;
import com.citrix.wi.controls.DelayedLaunchControl;
import com.citrix.wi.controlutils.FeedbackMessage;
import com.citrix.wi.controlutils.FeedbackUtils;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pages.StandardLayout;
import com.citrix.wi.pageutils.AGEUtilities;
import com.citrix.wi.pageutils.ApplistUtils;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.DelayedLaunchUtilities;
import com.citrix.wi.pageutils.Embed;
import com.citrix.wi.pageutils.Hints;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.LaunchUtilities;
import com.citrix.wi.pageutils.LocalisedText;
import com.citrix.wi.pageutils.PageHistory;
import com.citrix.wi.pageutils.ResourceEnumerationUtils;
import com.citrix.wi.pageutils.SessionUtils;
import com.citrix.wi.pageutils.TabUtils;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wi.pageutils.Utils;
import com.citrix.wi.pageutils.clientdetection.DetectionUtils;
import com.citrix.wi.tabs.EmptyResourcesTab;
import com.citrix.wi.tabs.ResourcesTab;
import com.citrix.wi.tabs.Tab;
import com.citrix.wi.tabs.TabSet;
import com.citrix.wi.types.UserInterfaceBranding;
import com.citrix.wing.AccessTokenException;
import com.citrix.wing.MessageType;
import com.citrix.wing.PasswordExpiredException;
import com.citrix.wing.ResourceUnavailableException;
import com.citrix.wing.types.ClientType;
import com.citrix.wing.types.FolderResourceTypeFlags;
import com.citrix.wing.util.Strings;
import com.citrix.wing.webpn.DesktopInfo;
import com.citrix.wing.webpn.PublishedItemInfo;
import com.citrix.wing.webpn.ResourceInfo;
import com.citrix.wing.webpn.UserContext;

/**
 * Business logic for the application list page.
 *
 * This class is responsible for creating, initialising and configuring the
 * display of the various tabs that the user may see on the application list
 * page, e.g. tabs for applications, the search results tab, an empty tab, etc.
 *
 * Most of the business logic for the contents of the tabs is contained within
 * the tab classes themselves.
 */
public class AppList extends StandardLayout {

    protected ApplistPageControl viewControl;

    private TabSet tabSet;

    private WebAbstraction web;

    private UserContext userContext;

    // Indicates whether the user has been logged out - if true, we should *not* call returnUserContext
    // since this can re-create the session data on JSP
    private boolean loggedOut = false;

    private static final String KEY_APP_ENUMERATION_ERROR = "AppEnumerationError";

    /**
     * Create a new instance.
     *
     * @param wiContext the Web Interface Context object
     */
    public AppList(WIContext wiContext) {
        super(wiContext);
        viewControl = new ApplistPageControl();
        viewControl.setupResourceNameTruncation(wiContext);
        wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
        web = wiContext.getWebAbstraction();
    }

    protected boolean performImp() throws IOException {

        userContext = SessionUtils.checkOutUserContext(wiContext);

        // Check whether we should stop processing after handling the special cases
        // as some of the special case code may have requested redirection to another URL.
        // handleSpecialCases returns false to indicate to stop processing the page.
        if (!handleSpecialCases()) {
            if (!loggedOut) {
                // return the user context and return from this method.
                SessionUtils.returnUserContext(userContext);
            }
            return false;
        }

        // Ensure no attempt to auto launch a desktop is made again in this session.
        LaunchUtilities.setAutoDesktopLaunchDisabled(wiContext);

        tabSet = TabUtils.createTabSet(viewControl, wiContext, userContext, messagesControl);
        tabSet.setSelected(TabUtils.retrieveCurrentTab(wiContext));
        tabSet.setPreviousTab(TabUtils.retrievePreviousTab(wiContext));
        processQueryStrings();

        boolean showForm = true;

        try {
            TabUtils.loadResourceTabs(wiContext, tabSet, viewControl, true);

            // If the selected tab should be displayed on another page, then redirect to that page.
            // This can occur, for example, when closing the search tab when the previously selected
            // tab was the Messages or Preferences tab.
            if (!Constants.PAGE_APPLIST.equals(tabSet.getSelected().targetPage())) {
                String url = tabSet.getSelected().targetPage();
                String queryString = web.getRequestQueryString();
                if (queryString != null) {
                    url += "?" + queryString;
                }

                getWebAbstraction().clientRedirectToUrl(url);
                showForm = false;
            } else {

                if (tabSet.getSelected() != null) {
                    TabUtils.saveCurrentTab(wiContext, tabSet.getSelected().getId());
                }

                Tab selected = tabSet.getSelected();
                if (selected != null) {
                    recordCurrentPageURL();
                    // setupViewControl() should be called before setupCommon() for the search text history to work properly.
                    showForm = selected.setupViewControl();
                    setupCommon(tabSet);
                }

                cleanupTabPrefs();
            }
        } catch (PasswordExpiredException pee) {
            if (Authentication.isChangePasswordAllowed(wiContext)) {
                UIUtils.handleMessage(wiContext, Constants.PAGE_CHANGE_PASSWD, MessageType.INFORMATION, "ChangePasswordNowOrAtLogin");
            } else {
                UIUtils.handleLogout(wiContext, MessageType.ERROR, "CredentialsExpired");
                loggedOut = true;
            }
            showForm = false;
        } catch (AccessTokenException ate) {
            UIUtils.handleLogout(wiContext, MessageType.ERROR, Utils.getAuthErrorMessageKey(ate));
            showForm = false;
            loggedOut = true;
        } catch (ResourceUnavailableException rue) {
            UIUtils.handleLogout(wiContext, MessageType.ERROR, KEY_APP_ENUMERATION_ERROR);
            showForm = false;
            loggedOut = true;
        }

        if (!loggedOut) {
            SessionUtils.returnUserContext(userContext);
			layoutControl.hasLightbox = !Include.isCompactLayout(wiContext);
        }
        
        return showForm;
    }

    /**
     * Handle special cases which require special actions before the AppList
     * page can be rendered.
     *
     * 1) Post login pages (change password etc.).
     * 2) Bookmark launches.
     * 3) Failed bookmark launches.
     * 4) Auto desktop launch.
     * 5) Redirect to reconnect for mobile devices.
     * 6) Desktop restart/power off
     *
     * @return <code>true</code> if the page should continue processing, otherwise <code>false</code>
     */
    private boolean handleSpecialCases() {

        // see if we need to go to the wizard, or wherever
        String startPage = PageHistory.getNextPostLoginPage(web);
        // will get an appId here if its a power off request.
        String appId = web.getQueryStringParameter(Constants.QSTR_RETRY_APPLICATION);
        DelayedLaunchControl delayedLaunchControl = DelayedLaunchUtilities.getDelayedLaunchControl(wiContext);

        if (startPage != null) {
            // Check post logon pages incase we need change password etc.
            getWebAbstraction().clientRedirectToUrl(startPage);
            return false;
        } else if (LaunchUtilities.getDirectLaunchModeInUse(wiContext)) {
            // If we are in direct launch, go straight to direct launch and do a launch.
            getWebAbstraction().clientRedirectToUrl(Constants.PAGE_DIRECT_LAUNCH);
            return false;
        } else if (LaunchUtilities.getSessionLaunchApp(wiContext) != null) {
            // This handles a bookmark launch where a session was already active.
            handleAutoLaunch();
            // Allow continue processing to render the required JavaScript if set.
        } else if (appId != null && delayedLaunchControl.hasPowerOffResource(appId)) {
            ResourceInfo resInfo = ResourceEnumerationUtils.getResource(appId, userContext);
            boolean continueProcessing = performPowerOffDesktop(resInfo);

            FeedbackMessage message = new FeedbackMessage(MessageType.INFORMATION,
                                                          "RestartWait",
                                                          new String[] { resInfo.getDisplayName() }
                                                         );
            message.setTransient(true);
            String url = FeedbackUtils.getFeedbackUrl(wiContext, message, Constants.PAGE_APPLIST);
            wiContext.getWebAbstraction().clientRedirectToUrl(url);
            return continueProcessing;
        } else if (wiContext.getConfiguration().getAutoLaunchDesktop()) {
            // Auto desktop launch is enabled
            // If the user has only one published resource available, and it is a desktop, then
            // we just automatically launch the desktop (using the direct launch mechanism)
            // rather than displaying the application list page.
            if (!processAutoDesktopLaunch(userContext)) {
                return false;
            }
        }

        // Attempt to setup the IFrames for reconnect
        // If it failes we need to redirect and exit this page without rendering.
        if (!setupPostLoginReconnect()) {
            return false;
        }

        return true;
    }

    // Call the Wing to do the power off for the given resInfo.
    private boolean performPowerOffDesktop(ResourceInfo resInfo) {
        DelayedLaunchControl delayedLaunchControl = DelayedLaunchUtilities.getDelayedLaunchControl(wiContext);
        delayedLaunchControl.removePowerOffResource(resInfo.getId());

        // Arrange for the desktop to be delay-launched, with an initial power-off
        DelayedLaunchUtilities.addPowerOffDelayedLaunch(wiContext, getClientType(), resInfo);
        return true;
    }

    private ClientType getClientType() {
        return Embed.toWingClientType(Embed.getScriptedHostedAppLaunchClient(wiContext));
    }


    /**
     * Configure the IFrames for reconnect or redirect to the reconnect page.
     */
    private boolean setupPostLoginReconnect() {

        // setup the reconnect after login, if needed
        String reconnectURL = Include.getReconnectAtLoginUrl(wiContext);

        if (reconnectURL != null) {
            if (wiContext.getClientInfo().osPocketPC()) {
                // Set the current page URL before doing the redirect so that
                // the reconnect page knows where to come back to.
                PageHistory.recordCurrentPageURL(web);

                // Redirect to the Reconnect page.
                getWebAbstraction().clientRedirectToUrl(reconnectURL);

                // Return false to indicate that we couldn't setup the iframes and need to redirect.
                return false;
            } else {
                autoLaunchJavaScriptControl.reconnectJavaScript = LaunchUtilities.getJavaScriptForReconnectAtLogin(wiContext, reconnectURL);
            }
        }

        // Setup OK so no need for any other actions
        return true;
    }

    /**
     * Handle launch situation where the launcher page was not opened in an IFRAME.
     * 
     * Typically the launcher page is not opened in an IFRAME if the user visits
     * a bookmarked URL while logged in to WI already.
     * 
     * In rare cases, this situation might also occur if a JavaScript error
     * occurs during an application link "onclick" event which prevents the launch
     * from happening in the usual way. In this situation the browser will redirect
     * to the launcher page in the main WI window, replacing the app list page.
     */
    private void handleAutoLaunch() {
        FeedbackMessage message = feedbackControl.getFeedback();

        // The presence of the shortcut disabled error tells us that a launch was
        // already attempted and should not be attempted again.
        // Note we cannot distinguish between failed bookmark launches and normal
        // launches that suffered from JavaScript errors.
        boolean shortCutDisabledError = ((message != null) &&
                                         (message.getType() == MessageType.WARNING) &&
                                         (message.getKey().equals("ShortcutDisabled")));

        if (!shortCutDisabledError) {
            String nfuseAppName = LaunchUtilities.getSessionLaunchApp(wiContext);
            String csrfToken = LaunchUtilities.getSessionLaunchAppCsrfToken(wiContext);
            String launchURL = Constants.PAGE_LAUNCHER + "?"
                + LaunchUtilities.getLaunchAppQueryStringFragment(wiContext, nfuseAppName)
                + csrfToken;
            autoLaunchJavaScriptControl.autoLaunchJavaScript = LaunchUtilities.getAutoLaunchJavaScript(wiContext, nfuseAppName, launchURL);
        }
        LaunchUtilities.clearSessionLaunchData(wiContext);
    }

    // Reads, validates and handles query strings.
    private void processQueryStrings() {
        // This page accepts the following query strings:
        //
        // - QSTR_REFRESH - indicates we should talk to the server to refresh the list of apps
        // - QSTR_CURRENT_TAB - indicates the current (ie selected) tab
        // - QSTR_SEARCH_STRING - a search query string (processed elsewhere)
        // - QSTR_SEARCH_RESULTS_PAGE_NO - the search page number to display (processed elsewhere)
        // - QSTR_CURRENT_FOLDER - the current folder (processed elsewhere for efficiency reasons)
        // - QSTR_CURRENT_VIEW_STYLE - the current full graphics view style (processed elsewhere)
        // - QSTR_SWITCH_TO_RESOURCE_VIEW - command to switch to a tab which can display the current resource type

        // Clear the session cache on an explicit refresh
        if (Constants.VAL_ON.equalsIgnoreCase(web.getQueryStringParameter(Constants.QSTR_REFRESH))) {
            userContext.getEnvironmentAdaptor().getSessionCache().clear();
        }

        // close hints
        String closeHints = web.getQueryStringParameter(Constants.QSTR_CLOSE_HINTS_AREA);
        if (Constants.CONST_CLOSE_HINTS_AREA.equals(closeHints)) {
            // Save the close hints action of the user in user preferences.
            UserPreferences newUserPrefs = Include.getRawUserPrefs(wiContext.getUserEnvironmentAdaptor());
            newUserPrefs.setShowHints(Boolean.FALSE);
            Include.saveUserPrefs(newUserPrefs, wiContext, userContext);

        }

        String switchTo = null;

        // change current tab
        String currentTab = web.getQueryStringParameter(Constants.QSTR_CURRENT_TAB);
        if (!Strings.isEmpty(currentTab)) {
            switchTo = currentTab;
        }

        // switch to folder from search results
        String currentResourceType = web.getQueryStringParameter(Constants.QSTR_SWITCH_TO_RESOURCE_VIEW);
        if (currentResourceType != null) {
            currentTab = findTabIdForResource(currentResourceType, tabSet);
            if (!Strings.isEmpty(currentTab)) {
                switchTo = currentTab;
            }
        }

        if (switchTo != null) {
            currentTab = switchTo;
            tabSet.setSelected(switchTo);
            // save the previous tab in the session
            if (tabSet.getPreviousTab() != null) {
                TabUtils.savePreviousTab(wiContext, tabSet.getPreviousTab().getId());
            }
        }

        // change current folder
        String currentFolder = web.getQueryStringParameter(Constants.QSTR_CURRENT_FOLDER);
        if (currentFolder != null) {
            if (currentTab == null) {
                currentTab = TabUtils.retrieveCurrentTab(wiContext);
            }
            Tab selected = tabSet.getTabById(currentTab);
            if (selected != null && selected instanceof ResourcesTab) {
                ((ResourcesTab)selected).setCurrentFolder(currentFolder);
            }
        }


    }

    // View control setup code which is common to all tabs
    private void setupCommon(TabSet tabSet) {
        // Layout control
        layoutControl.isAppListPage = true;

        //
        // Welcome control
        //
        // Only show title in low graphics and not AAC NavUI
        if (Include.isCompactLayout(wiContext) && !AGEUtilities.isAGEEmbeddedMode(wiContext)) {
            Tab selected = tabSet.getSelected();
            welcomeControl.setTitle(selected.getTitle());
        }

        String customMsg = LocalisedText.getAppWelcomeMessage(wiContext);
        if (customMsg != null) {
            welcomeControl.setBody(customMsg);
        }

        //
        // Search control
        //
        searchBoxControl.show = ApplistUtils.isSearchEnabled(wiContext);
        // The search box is in the welcome HTML for full graphics mode, and in the
        // applist HTML for low graphics mode
        searchBoxControl.query = ApplistUtils.retrieveSearchQuery(wiContext);

        //
        // Nav control
        //
        super.setupNavControl();
        navControl.setTabs(tabSet);

        // show the wizard link when captions are on,
        // and the wizard is supported
        navControl.setShowClientDetection(!DetectionUtils.hideWizardHelpLinks(wiContext));

        if (tabSet.getSelected().getId().equals(ResourcesTab.STR_DESKTOPS)) {
            layoutControl.showApplistBox = false;
            if (!navControl.getShowNavBar()) {
                layoutControl.showBackgroundGradient = true;
            }
        }

        //
        // System message control
        //
        String customText = LocalisedText.getAppSysMessage(wiContext);
        if (customText != null) {
            sysMessageControl.setMessage(customText);
        }

        // Used to determine how to truncate app names
        viewControl.lowGraphics = Include.isCompactLayout(wiContext);

        //
        // Hints
        //
        if (!Boolean.FALSE.equals(wiContext.getUserPreferences().getShowHints())) {
            viewControl.hintText = Hints.getHint(wiContext, tabSet.getSelected().getId());
        }
        viewControl.showHint = (viewControl.hintText != null && !(tabSet.getSelected() instanceof EmptyResourcesTab));
        viewControl.showCloseHint = wiContext.getConfiguration().getUIConfiguration().getAllowCustomizeShowHints();

        //
        // Refresh Button
        //
        viewControl.showRefresh = wiContext.getConfiguration().getUIConfiguration().getShowRefresh();
    }

    // Find the ID of the resources tab to which the given resource belongs.
    private static String findTabIdForResource(String resourceType, TabSet tabSet) {
        FolderResourceTypeFlags flags = makeFolderResourceTypeFlags(resourceType);
        if (flags.getValue() > 0) {
            ResourcesTab tab = findTabForResource(flags, tabSet);
            if (tab != null) {
                return tab.getId();
            }
        }
        return null;
    }

    // Find the resources tab to which the given resource belongs
    private static ResourcesTab findTabForResource(FolderResourceTypeFlags flags, TabSet tabSet) {
        ResourcesTab[] resourceListTabs = tabSet.getResourceTabs();

        for (int i = 0; i < resourceListTabs.length; i++) {
            ResourcesTab tab = resourceListTabs[i];
            if (tab != null && tab.containsResource(flags)) {
                return tab;
            }
        }
        return null;
    }

    // Build a <code>FolderResourceTypeFlags</code> object from the given
    // resource type string.
    private static FolderResourceTypeFlags makeFolderResourceTypeFlags(String type) {
        if (type == null) {
            return new FolderResourceTypeFlags(0);
        }

        if (type.equals(Constants.QSTR_RESOURCE_APP)) {
            return new FolderResourceTypeFlags(FolderResourceTypeFlags.APPS);
        }

        if (type.equals(Constants.QSTR_RESOURCE_CONTENT)) {
            return new FolderResourceTypeFlags(FolderResourceTypeFlags.CONTENT);
        }

        if (type.equals(Constants.QSTR_RESOURCE_DESKTOP)) {
            return new FolderResourceTypeFlags(FolderResourceTypeFlags.DESKTOPS);
        }

        return new FolderResourceTypeFlags(0);
    }

    // Ensures that the user's tab preferences do not refer to any non-existent
    // tabs.
    private void cleanupTabPrefs() {
        ResourcesTab[] tabs = tabSet.getResourceTabs();
        String[] liveTabIds = new String[tabs.length];
        for (int i = 0; i < tabs.length; i++) {
            liveTabIds[i] = tabs[i].getId();
        }
        ApplistUtils.scrubTabPreferences(liveTabIds, wiContext, userContext);
    }

    /*
     * Creates a sorted list of AppListTabs to align with a given list of TabIds.
     * The original Map of tabs passed in is not altered by this method.
     * Tabs not in the new ordering list are put at the end of the sort with the same relative order that
     * they're returned by an enumeratiion of the map.
     * @oaram tabList map of string tab ids to Tab objects, to be sorted into a list of tabs
     * @param newOrderingById the order in which to sort the tabs
     * @return sorted list of AppListTabs
     */
    public static List sortTabs(final Map tabsToSort, final List requiredTabOrder) {

        List sortedTabList = new ArrayList();

        if (tabsToSort != null) {
            // take a copy to avoid altering the object passed in
            Map copyTabsToSort = new HashMap(tabsToSort);

            if (requiredTabOrder != null) {
                Iterator iter = requiredTabOrder.iterator();
                while (iter.hasNext()) {
                    // note, tabs are removed from the ToSort map as they're sorted
                    Object tab = copyTabsToSort.remove((String)iter.next());
                    if (tab != null) {
                        sortedTabList.add(tab);
                    }
                }
            }
            // tabs not listed in requiredTabOrder heven't been removed from the ToSort map and should be added un-sorted
            sortedTabList.addAll(copyTabsToSort.values());
        }

        return sortedTabList;
    }

    /**
     * Creates a string List by extracting the tab ids from the given list of AppListTabs.
     * @param appTabList List of AppListTabs from which to take the tab ids
     * @return List of tab ids
     */
    public static List tabListToTabIdList(final List appTabList) {
        List idList = new ArrayList();
        if (appTabList != null) {
            Iterator iter = appTabList.iterator();
            while (iter.hasNext()) {
                idList.add(((Tab)iter.next()).getId());
            }
        }
        return idList;
    }

    protected String getBrowserPageTitleKey() {
        if (Include.getSiteBranding(wiContext) == UserInterfaceBranding.DESKTOPS) {
            return "BrowserTitleDesktop";
        } else {
            return "BrowserTitleApplications";
        }
    }

    /**
     * Identify whether it is possible to automatically launch the user's
     * desktop, and prepare the launch if so.
     *
     * Automatic desktop launch occurs if the user has only a single launchable
     * published resource, and this is a desktop.
     *
     * @param userContext the user context object
     * @return <code>true</code> if the page should continue processing, otherwise <code>false</code>
     */
    private boolean processAutoDesktopLaunch(UserContext userContext) {

        // If we have already auto launched the desktop or don't want to for this session
        // then just display the AppList page without launching.
        if (!LaunchUtilities.getAutoDesktopLaunchEnabled(wiContext)) {
            return true;
        }

        // Ensure no attempt to auto launch a desktop is made again in this session.
        LaunchUtilities.setAutoDesktopLaunchDisabled(wiContext);

        boolean continuePageProcessing = true;

        // Obtain the list of visible resources
        try {
            ResourceInfo[] resources = userContext.findAllVisibleResources().getResources();

            // Go through looking for published items (i.e. not folders or sessions)
            // Keep a record of the first published item we find
            // Stop as soon as we find another (since we know auto-launch cannot happen)
            PublishedItemInfo singlePublishedItem = null;

            for (int i = 0; i < resources.length; i++) {
                ResourceInfo r = resources[i];

                if (r instanceof PublishedItemInfo) {
                    if (singlePublishedItem == null) {
                        // This is the first published item
                        singlePublishedItem = (PublishedItemInfo)r;
                    } else {
                        // There is more than one published item
                        // Stop looking for any more
                        singlePublishedItem = null;
                        break;
                    }
                }
            }

            // Check whether there is only a single launchable resource, and it
            // is a desktop.
            if ((singlePublishedItem != null) && (singlePublishedItem instanceof DesktopInfo)) {
                // Auto desktop launch is going ahead
                // Set up a direct launch of the desktop resource
                LaunchUtilities.setSessionLaunchApp(wiContext, singlePublishedItem.getId());
                LaunchUtilities.setSessionDirectLaunch(wiContext, true);

                // Pass on any query string parameters that were intended
                // for the app list page. They should apply to the direct
                // launch page instead.
                String directLaunchUrl = Constants.PAGE_DIRECT_LAUNCH;
                String queryString = getWebAbstraction().getRequestQueryString();
                if (queryString != null) {
                    directLaunchUrl += "?" + queryString;
                }

                getWebAbstraction().clientRedirectToUrl(directLaunchUrl);
                continuePageProcessing = false;
            }
        } catch (AccessTokenException ate) {
            UIUtils.handleLogout(wiContext, MessageType.ERROR, Utils.getAuthErrorMessageKey(ate));
            continuePageProcessing = false;
            loggedOut = true;
        } catch (ResourceUnavailableException rue) {
            UIUtils.handleLogout(wiContext, MessageType.ERROR, KEY_APP_ENUMERATION_ERROR);
            continuePageProcessing = false;
            loggedOut = true;
        }

        return continuePageProcessing;
    }
}
