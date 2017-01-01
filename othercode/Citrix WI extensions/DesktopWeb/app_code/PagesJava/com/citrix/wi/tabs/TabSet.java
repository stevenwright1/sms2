/*
 * TabSet.java
 * Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */

package com.citrix.wi.tabs;
import java.util.ArrayList;
import com.citrix.wing.util.Collections;
import com.citrix.wing.util.Strings;

/**
 * Set of tabs that are available to be displayed on the application list page.
 *
 * This can include resources tabs (which display resources for launch) and
 * special tabs such as the search results tab.
 */
public class TabSet {
    private ArrayList tabList = new ArrayList();
    private Tab selectedTab;
    // records the previous tab so that when a tab is closed
    // we can go back to the previous tab
    Tab previousTab;

    /**
     * Remove all tabs from the set.
     */
    public void clear() {
        tabList.clear();
        selectedTab = null;
        previousTab = null;
    }

    /**
     * Add a new tab to the set.
     *
     * If the new tab has the same ID as an existing tab in the set, the
     * new tab will be ignored.
     *
     * The new tab becomes the currently selected tab if no other tab is
     * currently selected.
     *
     * @param uniqueTab the new <code>Tab</code> to add to the set.
     */
    public void addTab( Tab uniqueTab ) {
        // Validate that the tab exists and is not already added
        if ((uniqueTab != null) && (getTabById(uniqueTab.getId()) == null)) {

            tabList.add(uniqueTab);
            if (selectedTab == null) {
                setSelected(uniqueTab);
            }
        }
    }

    /**
     * Add an array of tabs to the set.
     *
     * If any tab has the same ID as an existing tab in the set, the new tab
     * will be ignored. The tabs are added in the order specified by the array.
     *
     * @param uniqueTabs the array of <code>Tab</code> objects to add
     */
    public void addTabs( Tab[] uniqueTabs ) {
        for (int i = 0; i < uniqueTabs.length; i++) {
            addTab(uniqueTabs[i]);
        }
    }

    /**
     * Remove a tab from the set.
     *
     * @param tab the <code>Tab</code> object to remove
     */
    public void remove( Tab tab ) {
        tabList.remove( tab );
        if( selectedTab == tab ) {
            selectedTab = previousTab;
        }
    }

    /**
     * Remove a tab from the set.
     *
     * @param tabId the unique ID of the tab to select
     */
    public void remove(String tabId) {
        if (!Strings.isEmpty(tabId)) {
            Tab tab = getTabById(tabId);
            if (tab != null) {
                remove(tab);
            }
        }
    }

    /**
     * Gets the currently selected tab if it is visible.
     *
     * If no selection has been made, the first visible tab is returned.
     * If there are no visible tabs it returns null.
     *
     * @return the currently selected <code>Tab</code> object
     */
    public Tab getSelected() {
        if( selectedTab != null && selectedTab.isVisible() ) {
            return selectedTab;
        } else if ( tabList.size() < 1 ) {
            return null;
        } else {
            for( int i = 0; i < tabList.size(); i++ ) {
                Tab tab = (Tab)tabList.get( i );
                if( tab.isVisible() ) {
                    setSelected( tab );
                    return selectedTab;
                }
            }

            // If we got this far it means that there is no current
            // visible tab.
            selectedTab = null;
            return selectedTab;
        }
    }

    /**
     * Gets whether the given tab is selected.
     *
     * @param tab the <code>Tab</code> object to test
     * @return <code>true<code> if the given tab is selected, otherwise <code>false</code>
     */
    public boolean isSelected(Tab tab) {
        return tab == getSelected();
    }

    /**
     * Gets the previously selected tab.
     * This returns null if no previous tab
     * Note: Previous tab will never be same as currently selected tab
     */
    public Tab getPreviousTab() {
        return previousTab;
    }

    /**
     * Sets the currently selected tab.
     *
     * This method does not validate whether the tab is visible. It is possible
     * to select a tab with the expectation of it becoming visible.
     *
     * The tab must be a member of this tab set.
     *
     * @param tab the <code>Tab</code> object to select
     */
    public void setSelected( Tab tab ) {
        if ((tab != null) && tabList.contains(tab)) {
            // store the currently Selected tab as previous tab
            Tab lastTab = selectedTab;
            selectedTab = tab;
            setPreviousTab(lastTab);
        }
    }

    /**
     * Sets the currently selected tab.
     *
     * This method does not validate whether the tab is visible. It is possible
     * to select a tab with the expectation of it becoming visible.
     *
     * The tab must be a member of this tab set.
     *
     * @param tabId the unique ID of the tab to select
     */
    public void setSelected( String tabId ) {
        if (!Strings.isEmpty(tabId)) {
            Tab tab = getTabById( tabId );
            if (tab != null) {
                setSelected(tab);
            }
        }
    }

    /**
     * Sets the previous tab by ID
     * This method does not set previous tab if the tab id is
     * same as current tab
     */
    public void setPreviousTab(String tabId) {
        if (Strings.isEmpty(tabId)) {
            return;
        }
        Tab tab = getTabById(tabId);
        if (tab == null) {
            return;
        }
        setPreviousTab(tab);
    }

    /**
     * Sets the previous tab
     * This method does not set the previous tab if the given tab
     * is the currently selected tab
     */
    public void setPreviousTab(Tab tab) {
        if (tab == null) {
            return;
        }
        // If the current tab is same as the given tab
        // then do not save as previous tab since it is current tab
        if (selectedTab == tab) {
            return;
        }
        previousTab = tab;
    }

    /**
     * Gets all the tabs in the set which are of type <code>ResourcesTab</code>.
     * Tabs are returned whether visible or not.
     *
     * @return array of <code>ResourcesTab</code> objects
     */
    public ResourcesTab[] getResourceTabs() {
        ResourcesTab[] result = new ResourcesTab[0];

        ArrayList resourcesTabs = (ArrayList) Collections.filter(tabList, ResourcesTab.class);
        if (resourcesTabs != null) {
            result = (ResourcesTab[]) resourcesTabs.toArray(new ResourcesTab[0]);
        }

        return result;
    }

    /**
     * Gets all the tabs in the set.
     * Tabs are returned whether visible or not.
     *
     * @return array of <code>Tab</code> objects
     */
    public Tab[] getTabs() {
        return (Tab[])tabList.toArray(new Tab[0]);
    }

    /**
     * Gets the number of tabs in the set.
     *
     * @return number of <code>Tab</code> objects
     */
    public int getNumTabs() {
        return tabList.size();
    }

    /**
     * Gets the tab associated with the given unique ID.
     *
     * @return an <code>Tab</code> object or null if a matching tab could not be found
     */
    public Tab getTabById( String tabId ) {
        if( Strings.isEmpty( tabId ) ) {
            return null;
        }

        for( int i = 0; i < tabList.size(); i++ ) {
            Tab tab = (Tab)tabList.get( i );
            if( tab.getId().equals( tabId ) ) {
                return tab;
            }
        }
        return null;
    }
}
