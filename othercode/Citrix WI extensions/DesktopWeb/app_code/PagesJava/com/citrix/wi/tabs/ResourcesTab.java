/*
 * ResourcesTab.java
 * Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */

package com.citrix.wi.tabs;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.Comparator;
import java.util.Iterator;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.StringTokenizer;
import java.util.TreeMap;

import com.citrix.wing.AccessTokenException;
import com.citrix.wing.ResourceUnavailableException;
import com.citrix.wing.types.DesktopAssignmentType;
import com.citrix.wing.types.FolderResourceTypeFlags;
import com.citrix.wing.util.FolderPath;
import com.citrix.wing.util.Strings;
import com.citrix.wing.util.WebUtilities;
import com.citrix.wing.webpn.ApplicationInfo;
import com.citrix.wing.webpn.DesktopInfo;
import com.citrix.wing.webpn.DocumentInfo;
import com.citrix.wing.webpn.FolderContentInfo;
import com.citrix.wing.webpn.FolderInfo;
import com.citrix.wing.webpn.ResourceInfo;
import com.citrix.wing.webpn.ResourceInfoDisplayComparator;
import com.citrix.wing.webpn.UserContext;

import com.citrix.wi.UserPreferences;
import com.citrix.wi.controls.ApplistPageControl;
import com.citrix.wi.controls.DelayedLaunchControl;
import com.citrix.wi.controls.ResourceControl;
import com.citrix.wi.controls.ResourceGroupControl;
import com.citrix.wi.controls.RetryControl;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.AGEUtilities;
import com.citrix.wi.pageutils.ApplistUtils;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.DelayedLaunchUtilities;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.LaunchUtilities;
import com.citrix.wi.pageutils.ResourceEnumerationUtils;
import com.citrix.wi.pageutils.Settings;
import com.citrix.wi.types.ApplicationView;
import com.citrix.wi.types.CompactApplicationView;
import com.citrix.wi.types.UserInterfaceBranding;
import com.citrix.wi.util.ClientInfoUtilities;
import com.citrix.wi.controls.DesktopGroupsController;

/**
 * Tab that displays published resources to the user.
 *
 * The published resources may be organised into folders and the tab may be
 * configured to display only certain types of published resource.
 */
public class ResourcesTab extends Tab {

    private ApplistPageControl viewControl;
    private WIContext wiContext;
    private UserContext userContext;
    private DelayedLaunchControl delayedLaunchControl;
    private WebAbstraction web;
    private String tabTitle;
    private String tabId;
    private ApplicationView currentViewStyle;

    private FolderResourceTypeFlags allowedResourceTypes;
    private FolderContentInfo folderContent;
    private String currentFolder;
    private boolean loaded = false;

    /** The Id for the tab showing all published resources. */
    public static final String ID_ALL_RESOURCES = "AllResources";

    // Keys to resource file strings
    /** Key for folder string. */
    public static final String STR_FOLDERS = "Folders";

    /** Key for content string. */
    public static final String STR_CONTENT = "Content";

    /** Key for desktops string. */
    public static final String STR_DESKTOPS = "Desktops";

    /** Key for applications string. */
    public static final String STR_APPS = "Applications";

    /** Key for "other" string. */
    public static final String STR_OTHER = "Other";

    /**
     * Creates a new instance.
     *
     * @param viewControl the control for the application list page
     * @param wiContext the Web Interface Context object
     * @param userContext the user context object
     * @param tabTitle the localized title for the tab
     * @param tabId the unique identifier for the tab
     * @param homeFolder the starting folder for the tab
     * @param allowedContent flags describing the resource types that the folder is allowed to display
     * @param disallowedContent flags decribing the resource types that the folder cannot display
     */
    public ResourcesTab(ApplistPageControl viewControl, WIContext wiContext, UserContext userContext, String tabTitle, String tabId, String homeFolder, FolderResourceTypeFlags allowedContent, FolderResourceTypeFlags disallowedContent) {
        this.viewControl = viewControl;
        this.wiContext = wiContext;
        this.web = wiContext.getWebAbstraction();
        this.userContext = userContext;
        this.tabId = tabId;
        this.tabTitle = WebUtilities.escapeHTML((!Strings.isEmpty(tabTitle)) ? tabTitle : tabId);
        this.allowedResourceTypes = ApplistUtils.resolveAllowedResourceTypes(allowedContent, disallowedContent);
        this.delayedLaunchControl = DelayedLaunchUtilities.getDelayedLaunchControl(wiContext);
    }

    /**
     * Creates a new skeleton instance.
     */
    protected ResourcesTab() {
    }

    /* See Tab interface. */
    public String getTitle() {
        return tabTitle;
    }

    /* See Tab interface. */
    public boolean isBusy() {
        boolean isBusy = showConnectingIcon();
        return isBusy;
    }

    /**
     * Decides whether to show a connecting icon or not
     */
    private boolean showConnectingIcon() {
        boolean hasAwaitingApps = ResourcesTab.STR_APPS.equalsIgnoreCase(tabId) && delayedLaunchControl.hasPendingApps();
        boolean hasAwaitingDesktops = ResourcesTab.STR_DESKTOPS.equalsIgnoreCase(tabId) && delayedLaunchControl.hasPendingDesktops();
        boolean hasAwaitingContent = ResourcesTab.STR_CONTENT.equalsIgnoreCase(tabId) && delayedLaunchControl.hasPendingContent();
        boolean hasAwaitingResources = ResourcesTab.ID_ALL_RESOURCES.equalsIgnoreCase(tabId) && delayedLaunchControl.isResourcePendings();
        return hasAwaitingApps || hasAwaitingDesktops || hasAwaitingContent || hasAwaitingResources;
    }

    /* See Tab interface. */
    public String getId() {
        return tabId;
    }

    /* See Tab interface. */
    public boolean isVisible() {
        if (!loaded) {
            throw new IllegalStateException("Resource tab must be loaded before testing visibility");
        }
        boolean isVisible = folderContent != null && folderContent.getNumChildren() > 0;
        return isVisible;
    }

    /* See Tab interface. */
    public String targetPage() {
        return Constants.PAGE_APPLIST;
    }

    /**
     * Loads the current folder content for the tab.
     * This is needed before we can determine whether the tab is visible
     */
    public void load() throws AccessTokenException, ResourceUnavailableException {
        initCurrentFolder();
        initFolderContent();
        loaded = true;
    }

    private void initFolderContent() throws AccessTokenException, ResourceUnavailableException {
        if (folderContent != null) {
            return;
        }

        initCurrentFolder();
        folderContent = userContext.findFolderContent(currentFolder, allowedResourceTypes);
        // check folder really exists
        if (folderContent == null || folderContent.getNumChildren() < 1) {
            if (!isRootFolder(currentFolder)) {
                currentFolder = "\\";
                folderContent = userContext.findFolderContent(currentFolder, allowedResourceTypes);
            }
        }
        DesktopGroupsController.createAndStoreInSession(wiContext, userContext);
    }

    /**
     * Sets the current folder which this tab should display
     * @param folder folder to set as current
     */
    public void setCurrentFolder(String folder) {
        currentFolder = folder;
    }

    // Sets up the current folder (if not already set) by reading from the session or user preferences.
    private void initCurrentFolder() {
        if (currentFolder == null) {
            currentFolder = getCurrentFolderFromSession(web);
            if (currentFolder == null && getRememberFolderOption()) {
                Map currentFolderMap = wiContext.getUserPreferences().getCurrentFolders();
                if (currentFolderMap != null) {
                    currentFolder = (String)currentFolderMap.get(tabId);
                }
            }
        }
        currentFolder = FolderPath.normalize(currentFolder);
    }

    // Gets the current folder for this tab from the user's session. Returns null if not set.
    private String getCurrentFolderFromSession(WebAbstraction web) {
        Map folderMap = (Map)web.getSessionAttribute(Constants.SV_CURRENT_FOLDER);
        String currentFolder = null;
        if (folderMap != null) {
            currentFolder = (String)folderMap.get(tabId);
        }
        return currentFolder;
    }

    // Writes the current folder of this tab to the session.
    private void putCurrentFolderInSession(WebAbstraction web, String folder) {
        Map folderMap = (Map)web.getSessionAttribute(Constants.SV_CURRENT_FOLDER);
        if (folderMap == null) {
            folderMap = new HashMap();
        }

        folderMap.put(tabId, folder);
        web.setSessionAttribute(Constants.SV_CURRENT_FOLDER, folderMap);
    }

    // Writes the current folder of this tab to the session and user preferences.
    private void saveCurrentFolder(String currentFolder) {
        putCurrentFolderInSession(web, currentFolder);
        putCurrentFolderInUserPrefs(currentFolder);
    }

    // Writes the current folder of this tab to user preferences.
    private void putCurrentFolderInUserPrefs(String currentFolder) {
        // store current folder in user prefs
        if (getRememberFolderOption()) {
            UserPreferences rawUserPrefs = Include.getRawUserPrefs(userContext.getEnvironmentAdaptor());
            HashMap currentFolderMap = rawUserPrefs.getCurrentFolders();
            if (currentFolderMap == null) {
                currentFolderMap = new HashMap();
            }

            currentFolderMap.put(tabId, currentFolder);
            rawUserPrefs.setCurrentFolders(currentFolderMap);
            Include.saveUserPrefs(rawUserPrefs, wiContext, userContext);
        }
    }

    /* See Tab interface. */
    public boolean setupViewControl() throws AccessTokenException, ResourceUnavailableException {
        initCurrentFolder();
        initFolderContent();
        saveCurrentFolder(currentFolder);

        // Check if there are no apps
        if (folderContent == null || folderContent.getNumChildren() < 1) {
            // Display a message instead of the applist
            viewControl.messageKey = ApplistUtils.getEmptyMessageKeyForUser(wiContext);
        } else {
            setupViewStyle();
            if (viewControl.currentViewStyle == ApplicationView.TREE) {
                setupTreeView(currentFolder);
            } else {
                buildBreadCrumbTrail();
                setupNonTreeViews(folderContent);
            }
        }

        return true;
    }

    // Gets whether the current folder should be persisted according the config settings.
    private boolean getRememberFolderOption() {
        if (wiContext.getConfiguration().getUIConfiguration().getAllowCustomizePersistFolderLocation()) {
            return (!Boolean.FALSE.equals(wiContext.getUserPreferences().getRememberFolder()));
        }
        return wiContext.getConfiguration().getUIConfiguration().getPersistFolderLocation();
    }


    // Return true if the specified folder is the root folder.
    private boolean isRootFolder(String folder) {
        // The folder name is "normalized", so we only need to check for a back-slash
        return "\\".equals(folder);
    }

    // Determine the current view style
    private void setupViewStyle() {
        if (Include.isCompactLayout(wiContext)) {
            // Force some settings in embedded mode
            if (AGEUtilities.isAGEEmbeddedMode(wiContext)) {
                viewControl.currentCompactViewStyle = CompactApplicationView.LIST;
                viewControl.showLowGraphicsChangeView = false;
            } else {
                viewControl.currentCompactViewStyle = wiContext.getUserPreferences().getCompactViewStyle();

                if (Settings.getShowChangeViewLink(wiContext)) {
                    viewControl.showLowGraphicsChangeView = true;
                }
            }
        } else if (currentViewStyle == null) {
            viewControl.allowedViewStyles = wiContext.getConfiguration().getUIConfiguration().getViewStyles();

            // First try to get the view style from the query string and validate it
            currentViewStyle = ApplicationView.fromString(web.getQueryStringParameter(Constants.QSTR_CURRENT_VIEW_STYLE));
            if (currentViewStyle == null || !(viewControl.allowedViewStyles.contains(currentViewStyle))) {
                //Get it from the user preferences
                currentViewStyle = getViewStyle(wiContext, tabId);
            }
        }

        if (ResourcesTab.STR_DESKTOPS.equalsIgnoreCase(tabId)) { // desktops tab for both XA and XD branded sites
            this.currentViewStyle = ApplicationView.DESKTOPS; // the default view style for the desktops tab
            viewControl.currentCompactViewStyle = CompactApplicationView.DESKTOPS;
        }

        viewControl.currentViewStyle = currentViewStyle;

        // Store it inside the user preferences as well. the filter will be carried out when we save the user prefs
        UserPreferences rawUserPrefs = Include.getRawUserPrefs(userContext.getEnvironmentAdaptor());
        if (currentViewStyle != null) {
            HashMap currentViewStyles = rawUserPrefs.getViewStyles();
            if (currentViewStyles == null) {
                currentViewStyles = new HashMap();
            }
            if (currentViewStyles.containsKey(tabId)) {
                currentViewStyles.remove(tabId);
            }
            currentViewStyles.put(tabId, currentViewStyle);
            rawUserPrefs.setViewStyles(currentViewStyles);
        }


        // Do not give change view style options if the user has a desktops view style
        if (currentViewStyle == ApplicationView.DESKTOPS) {
            viewControl.showChangeView = false;
        }
        if (viewControl.currentCompactViewStyle == CompactApplicationView.DESKTOPS) {
            viewControl.showLowGraphicsChangeView = false;
        }
        Include.saveUserPrefs(rawUserPrefs, wiContext, userContext);

    }

    // Gets the current view style for this tab.
    private ApplicationView getViewStyle(WIContext wiContext, String tabId) {
        ApplicationView view = null;

        HashMap viewStyles = wiContext.getUserPreferences().getViewStyles();
        if (viewStyles != null) {
            view = (ApplicationView)viewStyles.get(tabId);
        }

        // Fall back to default view style
        if (view == null) {
            view = wiContext.getConfiguration().getUIConfiguration().getDefaultViewStyle();
        }

        return view;
    }

    // Set up the tree view html with all the root and leaf nodes
    private void setupTreeView(String currentFolder) throws AccessTokenException, ResourceUnavailableException {
        folderContent = userContext.findFolderContent("\\", allowedResourceTypes);
        viewControl.treeViewInitialFolder = currentFolder;
        StringBuffer markup = new StringBuffer(1024);
        markup.append("<ul id=\"treeView\">");
        getTreeViewFolders(folderContent, markup);
        markup.append("</ul>");
        viewControl.treeViewMarkup = markup.toString();
    }

    // index of current folder link
    private int folderLinkIndex = 0;

    // This helper function goes through each folder and create the html equivalent for tree view for the corresponding folders
    private void getTreeViewFolders(FolderContentInfo folderContentInfo, StringBuffer markup) throws AccessTokenException, ResourceUnavailableException {
        if (folderContentInfo != null) {
            FolderInfo[] subfolders = folderContentInfo.getSubFolders();
            if (subfolders != null && subfolders.length > 0) {
                for (int i = 0; i < subfolders.length; i++) {
                    String displayName = subfolders[i].getDisplayName();
                    String folderPath = subfolders[i].getDisplayPath();
                    String encodedFolderDisplayName = WebUtilities.escapeHTML(displayName);
                    markup.append("<li class=\"folder\"><a id=\"folderLink_");
                    markup.append(folderLinkIndex++);
                    markup.append("\" href=\"#\" onClick=\"toggleTreeNode(this);return false;\" onmouseover=\"updateMouseoverTreeNodePicture(this);\" onmouseout=\"updateMouseoutTreeNodePicture(this);\" class=\"folderClose\" title=\"");
                    markup.append(encodedFolderDisplayName);
                    markup.append("\">");
                    markup.append("<img src=\"../media/FolderClosedArrow.gif\" alt=\"");
                    markup.append(encodedFolderDisplayName);
                    markup.append("\">");
                    markup.append(encodedFolderDisplayName);
                    markup.append("</a><ul>");
                    getTreeViewFolders(userContext.findFolderContent(folderPath + displayName, allowedResourceTypes), markup);
                    markup.append("</ul></li>");
                }
            }

            getTreeViewLeafResources(ApplistUtils.removeHiddenResources(wiContext, folderContentInfo.getLeafResources()), markup);
        }
    }

    // This helper function goes through the each leaf node inside a folder  and create the html equivalent for tree view for the corresponding leaf nodes
    private void getTreeViewLeafResources(ResourceInfo[] resources, StringBuffer markup) {
        if (resources != null && resources.length > 0) {
            for (int i = 0; i < resources.length; i++) {
                String appId = resources[i].getId();
                String encodedAppId = WebUtilities.encodeForId(appId);
                String displayName = DesktopGroupsController.getInstance(wiContext).getCompoundDesktopDisplayName(resources[i].getDisplayName(), appId);
                String iconMarkup = ApplistUtils.getIconMarkup(wiContext, resources[i], Constants.ICON_SIZE_16, true, false, null);

                String delayedLaunchImgSrc = "../media/Transparent16.gif";
                if (delayedLaunchControl.isResourcePending(appId) || delayedLaunchControl.hasPowerOffResource(appId)) {
                    delayedLaunchImgSrc = "../media/LaunchSpinner.gif";
                } else if (delayedLaunchControl.isBlockedLaunch(appId)) {
                    delayedLaunchImgSrc = "../media/LaunchReady.gif";
                }

                markup.append("<li><a id=\"");
                markup.append(encodedAppId);
                markup.append("\" class=\"iconLink\" title=\"");
                markup.append(WebUtilities.escapeHTML(displayName));
                markup.append("\" ");
                markup.append(Include.processAppLink(wiContext, resources[i]));
                markup.append(">");
                markup.append("<img id=\"spinner_");
                markup.append(encodedAppId);
                markup.append("\" class=\"spinner\" width=\"11\" height=\"11\" src=\"");
                markup.append(delayedLaunchImgSrc);
                markup.append("\" alt=\"\" >");
                markup.append(iconMarkup);
                markup.append("<span>");
                markup.append(WebUtilities.escapeHTML(displayName));
                markup.append("</span></a></li>");
            }
        }
    }

    // Sets up the breadcrumb trail
    private void buildBreadCrumbTrail() {
        String breadCrumbTrail = null;

        if (isRootFolder(currentFolder)) {
            breadCrumbTrail = makeEndBreadCrumb(wiContext.getString("RootFolderName"));
        } else {
            String folderPath = "\\";
            breadCrumbTrail = makeLinkBreadCrumb(wiContext.getString("RootFolderName"), folderPath, 0);
            StringTokenizer st = new StringTokenizer(currentFolder, "\\");
            int numTokens = st.countTokens();
            for (int i = 0; i < numTokens - 1; i++) {
                String token = st.nextToken();
                folderPath += token;
                breadCrumbTrail += makeLinkBreadCrumb(token, folderPath, i + 1);
                folderPath += "\\";
            }
            //the last token
            breadCrumbTrail += makeEndBreadCrumb(st.nextToken());
        }

        viewControl.breadCrumbTrail = breadCrumbTrail;
    }

    private static final String CSS_CLASS_BREADCRUMB = "breadcrumb";
    private static final String CSS_CLASS_BREADCRUMB_LONG = "breadcrumbLong";
    private static final String CSS_CLASS_LAST_CRUMB = "lastBreadcrumb";
    private static final int DEFAULT_CRUMB_WRAP_LIMIT = 80;
    private static final int DEFAULT_CRUMB_WRAP_LIMIT_LG = 36;

    private String getBreadcrumbCssClass(final int crumbCharCount) {
        final boolean isLowGraphics = Include.isCompactLayout(wiContext);
        final int defaultWrapLimit = isLowGraphics ? DEFAULT_CRUMB_WRAP_LIMIT_LG : DEFAULT_CRUMB_WRAP_LIMIT;
        final String wrapLimitStringKey = isLowGraphics ? "LGBreadcrumbWrapLimit" : "BreadcrumbWrapLimit";
        final int crumbWrapLimit = ApplistUtils.parsePositiveInteger(wiContext.getString(wrapLimitStringKey), defaultWrapLimit);

        return crumbCharCount < crumbWrapLimit ? CSS_CLASS_BREADCRUMB : CSS_CLASS_BREADCRUMB_LONG;
    }

    private String makeEndBreadCrumb(String unEncodedText) {
        String encodedText = WebUtilities.escapeHTML(unEncodedText);
        String crumb = "<span class=\"" + getBreadcrumbCssClass(unEncodedText.length()) + " " + CSS_CLASS_LAST_CRUMB + "\">" +
                           encodedText +
                       "</span>";
        return crumb;
    }

    private String makeLinkBreadCrumb(String unEncodedText, String unEncodedPath, int crumbNum) {
        String encodedText = WebUtilities.escapeHTML(unEncodedText);
        String encodedHref = makeBreadCrumbHref(unEncodedPath);

        String crumb = "<span class=\"" + getBreadcrumbCssClass(unEncodedText.length()) + "\">" +
                           "<a id=\"breadcrumbLink_" + crumbNum + "\" href='" + encodedHref + "'>" +
                                encodedText +
                            "</a>" +
                            " &gt;" +
                       "</span> ";
        return crumb;
    }

    private String makeBreadCrumbHref(String unEncodedFolderPath) {
        return Constants.PAGE_APPLIST + "?" + Constants.QSTR_CURRENT_FOLDER + "=" + WebUtilities.escapeURL(unEncodedFolderPath);
    }

    // Set up code which is used for all view styles except tree view
    private void setupNonTreeViews(FolderContentInfo folderContent) {
        // we have already checked that folderContent is non-null
        viewControl.folders = folderContent.getSubFolders();
        ResourceInfo[] resourceInfos = ApplistUtils.removeHiddenResources(wiContext, folderContent.getLeafResources());

        if (viewControl.currentViewStyle == ApplicationView.GROUPS) {
            resourceInfos = sortByType(resourceInfos);
        }

        viewControl.appIconSize = getAppIconSize(Include.isCompactLayout(wiContext),
                                                viewControl.currentViewStyle,
                                                viewControl.currentCompactViewStyle);

        viewControl.folderIconPath = ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/FolderClosed" + viewControl.appIconSize + ".png");
        viewControl.resources = buildResourceControls(resourceInfos);

        if (viewControl.currentViewStyle == ApplicationView.GROUPS) {
            setupAppGroupsView(viewControl, resourceInfos, viewControl.resources);
        } else if (viewControl.currentViewStyle == ApplicationView.DESKTOPS ||
            viewControl.currentCompactViewStyle == CompactApplicationView.DESKTOPS) {
            setupDesktopGroupsView(viewControl);
        }
    }

    private int getAppIconSize(boolean isCompactLayout,
                               ApplicationView viewStyle,
                               CompactApplicationView compactViewStyle) {
        int result = Constants.ICON_SIZE_32;

        if (isCompactLayout) {
            if (compactViewStyle == CompactApplicationView.DESKTOPS ||
                compactViewStyle == CompactApplicationView.LIST) {
                result = Constants.ICON_SIZE_16;
            }
        } else {
            if (viewStyle == ApplicationView.DETAILS ||
                viewStyle == ApplicationView.LIST ||
                viewStyle == ApplicationView.TREE) {
                result = Constants.ICON_SIZE_16;
            }
        }

        return result;
    }

    private ResourceControl[] buildResourceControls(ResourceInfo[] resourceInfos) {
        ResourceControl[] resources = new ResourceControl[resourceInfos.length];

        // Populate the resources
        for (int i = 0; i < resourceInfos.length; i++) {
            resources[i] = ResourceControl.fromResourceInfo(resourceInfos[i],
                                                            wiContext,
                                                            userContext,
                                                            viewControl.appIconSize);
        }

        return resources;
    }

    private void setupDesktopGroupsView(ApplistPageControl viewControl) {
        DesktopGroupsController groupsController = DesktopGroupsController.getInstance(wiContext);
        viewControl.desktopTabViewMarkup = groupsController.getAllDesktopGroupsMarkup(wiContext);
    }

    // Sorts the ResourceInfo array by types into the order which they should be displayed in the Groups view.
    private ResourceInfo[] sortByType(ResourceInfo[] resArray) {
        List resList = Arrays.asList(resArray);
        Collections.sort(resList, new ResourceInfoDisplayComparator());
        return (ResourceInfo[])resList.toArray(new ResourceInfo[0]);
    }

    // Preconditions:
    //  - the ResourceInfo array is already sorted by type
    //  - the elements in the two arrays correspond
    private void setupAppGroupsView(ApplistPageControl viewControl, ResourceInfo[] resInfos, ResourceControl[] resControls) {

        if (resInfos.length != resControls.length) {
            throw new IllegalArgumentException("Array length mismatch - resource arrays must correspond.");
        }

        if (resInfos.length < 1) {
            viewControl.resourceGroups = new ResourceGroupControl[0];
            return;
        }

        ArrayList allGroups = new ArrayList();

        ArrayList currentGroup = new ArrayList();
        ResourceInfo currentResource = resInfos[0];

        for (int i = 0; i < resInfos.length; i++) {
            ResourceInfo resInf = resInfos[i];
            ResourceControl resCtrl = resControls[i];

            if (resInf == null || resCtrl == null) {
                continue;
            }

            if (!(resInf.getClass() == currentResource.getClass())) {
                ResourceGroupControl control = createResourceGroup(currentResource, currentGroup);
                if (control != null && !(currentResource instanceof FolderInfo)) {  // folders shouldn't be in this list in the first place
                    // but exclude them here just in case
                    allGroups.add(control);
                }
                currentResource = resInf;
                currentGroup = new ArrayList();
            }
            currentGroup.add(resCtrl);
        }
        // add final group
        ResourceGroupControl control = createResourceGroup(currentResource, currentGroup);
        if (control != null) {
            allGroups.add(control);
        }

        viewControl.resourceGroups = (ResourceGroupControl[])allGroups.toArray(new ResourceGroupControl[0]);
    }

    // Create a ResourceGroupControl from the list of ResourceInfos.
    // Returns null if the list is empty.
    private ResourceGroupControl createResourceGroup(ResourceInfo resInfo, List resources) {
        String title;
        boolean isDesktopGroup = false;

        if (resInfo instanceof FolderInfo) {
            title = wiContext.getString(STR_FOLDERS);
        } else if (resInfo instanceof DocumentInfo) {
            title = wiContext.getString(STR_CONTENT);
        } else if (resInfo instanceof DesktopInfo) {
            title = wiContext.getString(STR_DESKTOPS);
            isDesktopGroup = true; // to determine whether the resource group is a desktop group or not.
        } else if (resInfo instanceof ApplicationInfo) {
            title = wiContext.getString(STR_APPS);
        } else {
            title = wiContext.getString(STR_OTHER);
        }

        return new ResourceGroupControl(title, isDesktopGroup, (ResourceControl[])resources.toArray(new ResourceControl[0]));
    }

    /**
     * Gets whether this tab is capable of displaying the given resource type(s).
     * @param type Type(s) to test.
     * @return <code>true</code> if this tab can display the given resource type, otherwise returns <code>false</code>.
     */
    public boolean containsResource(FolderResourceTypeFlags type) {
        return allowedResourceTypes.includes(type);
    }

}
