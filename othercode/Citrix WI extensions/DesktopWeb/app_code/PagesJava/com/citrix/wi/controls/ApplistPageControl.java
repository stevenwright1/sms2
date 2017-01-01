/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.controls;

import java.util.Set;
import java.util.HashSet;

import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.types.ApplicationView;
import com.citrix.wi.types.CompactApplicationView;
import com.citrix.wing.webpn.FolderInfo;
import com.citrix.wi.controls.ResourceGroupControl;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.ApplistUtils;

/**
 * Maintains presentation state for the applications page.
 */
public class ApplistPageControl extends PageControl {

    // Default maximum resource name lengths above which truncation will occur
    private static int ICON_VIEW_DEFAULT_MAX_RESOURCE_NAME_LENGTH = 36;
    private static int LIST_VIEW_DEFAULT_MAX_RESOURCE_NAME_LENGTH = 25;
    private static int LOW_GRAPHICS_ICON_VIEW_DEFAULT_MAX_RESOURCE_NAME_LENGTH = 36;

    // Actual maximum resource name lengths above which truncation will occur
    private int iconViewMaxResourceNameLength;
    private int listViewMaxResourceNameLength;
    private int lowGraphicsIconViewMaxResourceNameLength;

    public ApplistPageControl() {
    }

    // Read in maximum resource name lengths
    public void setupResourceNameTruncation(WIContext wiContext) {
        iconViewMaxResourceNameLength = ApplistUtils.parsePositiveInteger(
            wiContext.getString("IconViewMaxResourceNameLength"), ICON_VIEW_DEFAULT_MAX_RESOURCE_NAME_LENGTH);
        listViewMaxResourceNameLength = ApplistUtils.parsePositiveInteger(
            wiContext.getString("ListViewMaxResourceNameLength"), LIST_VIEW_DEFAULT_MAX_RESOURCE_NAME_LENGTH);
        lowGraphicsIconViewMaxResourceNameLength = ApplistUtils.parsePositiveInteger(
            wiContext.getString("LGIconViewMaxResourceNameLength"), LOW_GRAPHICS_ICON_VIEW_DEFAULT_MAX_RESOURCE_NAME_LENGTH);
    }

    // Array of controls containing an item for each app or doc shown
    public ResourceControl[] resources = null;

    // Array of groups of ResourceControls
    public ResourceGroupControl[] resourceGroups;

    // Array containing an item for each folder shown
    public FolderInfo[] folders = null;

    // Used to force the size of the icons
    public int appIconSize = Constants.ICON_SIZE_32;

    // Path to the folder icon
    public String folderIconPath = null;

    // Full graphics view styles to show in the view style dropdown
    public Set allowedViewStyles = new HashSet();

    // The current full graphics view style
    public ApplicationView currentViewStyle;

    // In full graphics mode, whether to show the Change View link (not shown for Desktops tab anymore)
    public boolean showChangeView = true;

    // In low graphics mode, whether to show the Change View link
    public boolean showLowGraphicsChangeView = true;

    // Low graphics view styles to determine if the selected view style is allowed
    public Set allowedCompactViewStyles = null;

    // The current low graphics view style
    public CompactApplicationView currentCompactViewStyle;

    // If this key is set, a message is shown rather than a list of applications;
    // used when there are no applications to view.
    public String messageKey = null;
    public String treeViewInitialFolder = null;

    // If true, the refresh button will be displayed
    public boolean showRefresh = false;

    /**
     * Gets the number of folders contained in the current folder.
     * @return Number of folders
     */
    public int getNumFolders() {
        return folders.length;
    }

    /**
     * Gets the number of applications, desktops and docucments in the current folder.
     * @return Number of resources
     */
    public int getNumResources() {
        return resources.length;
    }

    /**
     * Gets the number of groups when displaying with the Groups view style
     * @return Number of different groups.
     */
    public int getNumResourceGroups() {
        return (resourceGroups== null) ? 0 : resourceGroups.length;
    }

    // Control for holding the search results display state
    public SearchResultsControl searchControl = new SearchResultsControl();

    // Used to alter the appname truncation logic
    public boolean lowGraphics = false;

    // Localised hint text
    public String hintText = null;

    // Whether to show the hint text
    public boolean showHint = false;

    // Whether to show the hint close button
    public boolean showCloseHint = false;

    // The full graphics breadcrumb trail
    public String breadCrumbTrail = null;

    // Contains the markup (HTML) for tree view
    public String treeViewMarkup = null;

    // The heading shown above the applist; either the folder name or the search string
    public String lowGraphicsHeading = null;

    // These two fields are for low graphics folder navigation
    public String lowGraphicsParentFolderPath = null;
    public String lowGraphicsParentFolderName = null;

    /**
     * Gets the truncated name if the application name is longer than a predefined length.
     * @return <code>String</code> truncated string if the name is very long
     */
    public String getTruncatedName(String appName) {

        int truncatedLength = -1; // don't truncate by default

        if (lowGraphics) {
            if (currentCompactViewStyle == CompactApplicationView.ICONS) {
                // We only truncate the resouce name in icon view
                truncatedLength = lowGraphicsIconViewMaxResourceNameLength;
            }
        } else {
            // We truncate in icon, list and group view
            if (currentViewStyle == ApplicationView.LIST) {
                truncatedLength = listViewMaxResourceNameLength;
            } else if (currentViewStyle == ApplicationView.ICONS) {
                truncatedLength = iconViewMaxResourceNameLength;
            } else if (currentViewStyle == ApplicationView.GROUPS) {
                truncatedLength = iconViewMaxResourceNameLength;
            }
        }

        if ((truncatedLength > 0) && (appName.length() > truncatedLength)) {
            return appName.substring(0, truncatedLength) + "...";
        } else {
            return appName;
        }
    }

    /**
     * The markup for the desktops tab view.
     */
    public String desktopTabViewMarkup;

}
