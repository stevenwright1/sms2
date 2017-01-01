/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils;

import java.util.ArrayList;

import com.citrix.wi.config.ApplistTabConfig;
import com.citrix.wi.controls.ApplistPageControl;
import com.citrix.wi.controls.MessagesControl;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.tabs.Tab;
import com.citrix.wi.tabs.TabSet;
import com.citrix.wi.tabs.EmptyResourcesTab;
import com.citrix.wi.tabs.ResourcesTab;
import com.citrix.wi.types.UserInterfaceBranding;
import com.citrix.wing.AccessTokenException;
import com.citrix.wing.ResourceUnavailableException;
import com.citrix.wing.webpn.UserContext;

/**
 * Provides a number of useful methods that are handy when working with
 * the UI or the browser.  E.g., obtaining and creating URLs, obtaining parts
 * of URLs, obtaining screen sizes, etc.
 */
public class TabUtils {

    /*
     * Creates the set of available tabs for the user. Not all of the tabs may
     * be currently visible.
     *
     * @param viewControl the <code>ApplistPageControl</code> object
     * @param wiContext the Web Interface context
     * @param userContext the user context object
     * @param messagesControl the <code>MessagesControl</code>object
     * @return <code>TabSet</code> object containing the available tabs
     */
    public static TabSet createTabSet(ApplistPageControl viewControl,
                                               WIContext wiContext,
                                               UserContext userContext,
                                               MessagesControl messagesControl) {

        TabSet tabSet = new TabSet();
        ResourcesTab[] resources = createResourceTabs(viewControl, wiContext, userContext);
        tabSet.addTabs(resources);

        return tabSet;
    }

    /**
     * Loads the resource tabs with content.
     *
     * @param wiContext the Web Interface context
     * @param tabSet the set of tabs to load
     * @param viewControl the ApplistPageControl object
     * @param selectEmptyTab <code>true</code> if the empty resources tab should be selected, else <code>false</code>
     */
    public static void loadResourceTabs(WIContext wiContext, TabSet tabSet, ApplistPageControl viewControl, boolean selectEmptyTab) 
        throws AccessTokenException, ResourceUnavailableException {
        
        ResourcesTab[] resTabs = tabSet.getResourceTabs();
        for (int i = 0; i < resTabs.length; i++) {
            ResourcesTab resTab = resTabs[i];
            resTab.load();
        }

        // Remove any tabs that are not visible
        Tab[] tabs = tabSet.getTabs();
        for (int i = 0; i < tabs.length; i++) {
            if (!tabs[i].isVisible()) {
                tabSet.remove(tabs[i]);
            }
        }

        // If no resource tab is visible, create an empty resources tab
        if (!anyResourceTabVisible(tabSet)) {
            Tab emptyTab = new EmptyResourcesTab(viewControl, wiContext);
            tabSet.addTab(emptyTab);
            if (selectEmptyTab) {
                tabSet.setSelected(emptyTab);
            }
        }
    }

    // Creates the set of available resource tabs for the user. This is dependent
    // on the site configuration.
    private static ResourcesTab[] createResourceTabs(ApplistPageControl viewControl,
                                                      WIContext wiContext,
                                                      UserContext userContext) {

        ApplistTabConfig[] tabConfig = wiContext.getConfiguration().getUIConfiguration().getAccessPointTabs();

        ArrayList tabList = new ArrayList();
        for (int i = 0; i < tabConfig.length; i++) {
            ResourcesTab resourceTab = createResourceTabFromConfig(tabConfig[i], viewControl, wiContext, userContext);
            tabList.add(resourceTab);
        }

        return (ResourcesTab[])tabList.toArray(new ResourcesTab[0]);
    }

    // Creates a new resources tab from the configuration
    private static ResourcesTab createResourceTabFromConfig(ApplistTabConfig config, ApplistPageControl viewControl, WIContext wiContext, UserContext userContext) {
        String name = config.getUniqueName();
        String title;
        if (config.isCustom()) {
            title = LocalisedText.getAccessPointTabTitle(wiContext, name);
        } else if (ResourcesTab.ID_ALL_RESOURCES.equalsIgnoreCase(name)) {
            // This is the built-in "All Resources" tab
            // It has a different title depending on the site branding
            title = wiContext.getString(ResourcesTab.STR_APPS);

            if (Include.getSiteBranding(wiContext) == UserInterfaceBranding.DESKTOPS) {
                title = wiContext.getString(ResourcesTab.STR_DESKTOPS);
            }
        } else {
            // Another built-in tab - get the title from the resource file
            title = wiContext.getString(name + "TabTitle");
        }

        ResourcesTab tab = new ResourcesTab(viewControl,
                                             wiContext,
                                             userContext,
                                             title,
                                             name,
                                             config.getHomeFolder(),
                                             config.getAllowedResources(),
                                             config.getDisallowedResources());
        return tab;
    }

    /**
     * Checks whether at least one tab in the supplied tab set is visible.
     *
     * @param tabSet the set of tabs to check
     * @return <code>true</code> if at least one tab is visible, else <code>false</code>
     */
    private static boolean anyResourceTabVisible(TabSet tabSet) {
        ResourcesTab[] resTabs = tabSet.getResourceTabs();
        for (int i = 0; i < resTabs.length; i++) {
            ResourcesTab resTab = resTabs[i];
            if (resTab.isVisible()) {
                return true;
            }
        }
        return false;
    }

    /**
     * Saves the id of the currently selected tab in the session
     *
     * @param wiContext the Web Interface context
     * @param tab the tab id to save in the session
     */
    public static void saveCurrentTab(WIContext wiContext, String tab) {
        wiContext.getWebAbstraction().setSessionAttribute(Constants.SV_CURRENT_TAB, tab);
    }

    /**
     * Saves the id of the previously selected tab in the session.
     * This is required to restore the previous tab when the search tab is closed
     *
     * @param wiContext the Web Interface context
     * @param tab the tab id to save in the session
     */
    public static void savePreviousTab(WIContext wiContext, String tab) {
        wiContext.getWebAbstraction().setSessionAttribute(Constants.SV_PREVIOUS_TAB, tab);
    }

    /**
     * Retrieves the id of the currently selected tab from the session or user preferences.
     * First check the session and return the current tab if defined.
     * If it's not found in the session, check user preferences, and if it's not in user preferences either, return null.
     *
     * @param wiContext the Web Interface context
     * @return the tab id of the currently selected tab
     */
    public static String retrieveCurrentTab(WIContext wiContext) {
        String tab = (String)wiContext.getWebAbstraction().getSessionAttribute(Constants.SV_CURRENT_TAB);
        if (tab == null) {
            tab = wiContext.getUserPreferences().getCurrentTab();
        }
        return tab;
    }

    /**
     * Retrieves the id of the previously selected tab from the session.
     *
     * @param wiContext the Web Interface context
     * @return the tab id of the previously selected tab
     */
    public static String retrievePreviousTab(WIContext wiContext){
        return (String) wiContext.getWebAbstraction().getSessionAttribute(Constants.SV_PREVIOUS_TAB);
    }

    /**
     * Helper function to generate HTML markup for a given tab
     *
     * @param wiContext the Web Interface context
     * @param tabSet the tabSet containing the given tab
     * @param tab the tab
     * @return the HTML tab markup as a <code>String</code>
     */
    public static String generateTabMarkup(WIContext wiContext, TabSet tabSet, Tab tab) {
        String tabId = tab.getId();
        String tabClass = tabSet.isSelected(tab) ? "selectedTab" : "otherTab";
        String hrefTag = tabSet.isSelected(tab) ? "" : "href=\"" + tab.targetPage() + "?" + Constants.QSTR_CURRENT_TAB + "=" + tabId + "\"";
        String mouseOverHandler = "";
        String mouseOutHandler = "";
        String imageMarkup = null;
        String anchorClass = "tabText";

        if (tab.isBusy()) {
            tabClass += " imageTab";
            imageMarkup = "<img src=\"../media/DesktopTabLoader.gif\">";
        }

        String innerImageMarkup = "";
        StringBuffer markup = new StringBuffer();

        markup.append("<li id=\"" + tabId + "\" class=\"" + tabClass + "\"" + mouseOverHandler + mouseOutHandler + ">\n");
        markup.append("  <div class=\"leftDoor\">\n");
        markup.append("    <div class=\"rightDoor\">\n");
        markup.append("      <a id=\"" + tabId + "_Text\" class=\"" + anchorClass + "\" " + hrefTag + "><span>" + tab.getTitle() + innerImageMarkup + "</span></a>\n");
        if (imageMarkup != null) {
            markup.append("        " + imageMarkup + "\n");
        }
        markup.append("    </div>\n");
        markup.append("  </div>\n");
        markup.append("</li>\n");

        return markup.toString();
    }

}
