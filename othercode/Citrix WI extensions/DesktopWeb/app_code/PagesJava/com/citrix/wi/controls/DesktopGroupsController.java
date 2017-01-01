/*
 * DesktopGroupsController.java
 * Copyright (c) 2010 Citrix Systems, Inc.  All Rights Reserved.
 */
package com.citrix.wi.controls;

import com.citrix.wi.controls.ResourceControl;

import java.util.Arrays;
import java.util.Comparator;
import java.util.Iterator;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.TreeMap;
import java.util.HashMap;

import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.ApplistUtils;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Include;
import com.citrix.wing.AccessTokenException;
import com.citrix.wing.NoSuchResourceException;
import com.citrix.wing.ResourceUnavailableException;
import com.citrix.wing.util.Strings;
import com.citrix.wing.types.FolderResourceTypeFlags;
import com.citrix.wing.webpn.DesktopInfo;
import com.citrix.wing.webpn.FolderContentInfo;
import com.citrix.wing.webpn.FolderInfo;
import com.citrix.wing.webpn.ResourceInfo;
import com.citrix.wing.webpn.UserContext;

/**
 * This class is responsible for managing desktop groups displayed on the desktops tab view.
 */
public class DesktopGroupsController {
    // Map of DesktopKey -> DesktopGroup
    // This sorted map keeps groups in the display order, see SortByFriendlyName.
    private Map desktopGroups = new TreeMap(new SortByFriendlyName());

	private Map idToDesktopGroupMapping = new HashMap();

    private static ResourceInfo[] getAllChildDesktopsFiltered(WIContext wiContext, UserContext userContext) throws AccessTokenException, ResourceUnavailableException {
        List desktops = new ArrayList();
        List foldersToTraverse = new ArrayList();
        foldersToTraverse.add("\\");
        
        while (!foldersToTraverse.isEmpty()) {
            String folderPath = (String)foldersToTraverse.remove(0);

            FolderContentInfo folderContent = userContext.findFolderContent(folderPath,
                                       new FolderResourceTypeFlags(FolderResourceTypeFlags.DESKTOPS | FolderResourceTypeFlags.FOLDERS));

            if (folderContent == null) {
                continue;
            }
            
            // All retrieved resources must be desktops.
            desktops.addAll(Arrays.asList(folderContent.getLeafResources()));

            // Traverse all sub-folders.
            FolderInfo[] subFolders = folderContent.getSubFolders(); 
            for (int i=0; i<subFolders.length; i++) {
                foldersToTraverse.add(subFolders[i].getDisplayPath() + subFolders[i].getDisplayName());
            }
        }

        ResourceInfo[] result = ApplistUtils.removeHiddenResources(wiContext, (ResourceInfo[])desktops.toArray(new ResourceInfo[0]));
        return result;
    }

    /**
     * Constructor. Builds the DesktopGroupsController and adds all desktops
     * from the root folder to appropriate groups.
     * 
     * @param wiContext the WIContext object
     * @param userContext user context
     */
    public static DesktopGroupsController createAndStoreInSession(WIContext wiContext, UserContext userContext) throws AccessTokenException, ResourceUnavailableException {
        if (userContext == null) {
            throw new IllegalArgumentException("User Context cannot be null!");
        }

        DesktopGroupsController controller = new DesktopGroupsController();
        ResourceInfo[] resInfos = getAllChildDesktopsFiltered(wiContext, userContext);

        for (int i = 0; i < resInfos.length; i++) {
            // The icon size set here is irrelevant. Compact mode always use small icons, full uses custom images, not the resource icons.
            ResourceControl rc = ResourceControl.fromResourceInfo(resInfos[i], wiContext, userContext, Constants.ICON_SIZE_16);
            controller.addDesktop(rc, (DesktopInfo)resInfos[i]);
        }
        controller.storeInSession(wiContext);
        return controller;
    }

    /**
     * Retrieves the stored DesktopGroupsController object from the session state.
     *
     * @param wiContext the WIContext object
     * @return retrieved DesktopGroupsController
     * @throws IllegalStateException if the controller is not already stored in the session
     */
    public static DesktopGroupsController getInstance(WIContext wiContext) {
        DesktopGroupsController controller = (DesktopGroupsController)wiContext.getUserEnvironmentAdaptor().getSessionState().get(SV_DESKTOP_GROUPS_CONTROLLER);

        if (controller == null) {
            throw new IllegalStateException("no stored controllers found!");
        }

        return controller;
    }

    /**
     * Returns an instance of the class used to generate HTML markup, depending on whether
     * displaying in low or full graphics mode.
     *
     * @param wiContext the WIContext object
     * @return a <code>DesktopGroupMarkup</code> object to generate the markup
     */
    public DesktopGroupMarkup getMarkupGenerator(WIContext wiContext) {
        if (Include.isCompactLayout(wiContext)) {
            return (new DesktopGroupCompactMarkup());
        } else {
            return (new DesktopGroupFullMarkup());
        }
    }

    /**
     * Returns a sorted list of desktop groups.
     * @return a sorted list of desktop groups.
     */
    public List getDesktopGroups() {
        ArrayList list = new ArrayList(desktopGroups.values());
        return list;
    }

    /**
     * Returns the display name for the desktop in format "display name (n)",
     * where n is the number of the desktop in the group.
     * Alternatively, return just "display name" if the desktop is the only desktop in its group.
     *
     * @param friendlyName friendly name of the desktop
     * @param appId the desktop's appId
     * @return the display name to be used in the ICA file
     */
    public String getCompoundDesktopDisplayName(String friendlyName, String appId) {
        String compoundDisplayName = friendlyName;
        int desktopIndex = getDesktopIndex(appId);
        if (desktopIndex > 0) {
            compoundDisplayName += " (" + desktopIndex + ")";
        }

        return compoundDisplayName;
    }

    /**
     * Returns the index of the desktop within its group.
     *
     * @param appId the desktop's appId
     * @return the desktop index, or 0 if not found, or desktop is not within the group
     */
    private int getDesktopIndex(String appId) {
        int desktopIndex = 0;
        DesktopGroup group = (DesktopGroup)idToDesktopGroupMapping.get(appId);
        if (group != null) {
            ResourceControl desktop = group.getDesktop(appId);
		    if (desktop != null) {
                desktopIndex = desktop.desktopIndex;
            }
        }
        		
		return desktopIndex;
    }

    /**
     * Stores the desktop into the appropriate group.
     *
     * @param resource ResourceControl of the desktop being added
     * @param desktopInfo DesktopInfo of the desktop being added
     */
    public void addDesktop(ResourceControl resource, DesktopInfo desktopInfo) {
        // The GroupKey creates a key based on data from desktopInfo.
        // Two desktops that belong to the same group always have the same GroupKey.
        GroupKey key = new GroupKey(desktopInfo);

        // If the group stored under generated key is not found, it means there
        // is no group for this desktop. Generate it.
        if (!desktopGroups.containsKey(key)) {
            desktopGroups.put(key, new DesktopGroup());
        }

        DesktopGroup group = (DesktopGroup)desktopGroups.get(key);

        // Store the resource into the selected group
        group.addDesktop(resource, desktopInfo);

        idToDesktopGroupMapping.put(desktopInfo.getId(), group);
    }

    /**
     * Returns the html markup for all desktops managed by this class.
     *
     * @param wiContext the WIContext object
     * @return the html markup of the desktops tab
     */
    public String getAllDesktopGroupsMarkup(WIContext wiContext) {
        StringBuffer markup = new StringBuffer(2048);

        Iterator groupIt = desktopGroups.values().iterator();
        while (groupIt.hasNext()) {
            DesktopGroup desktopGroup = (DesktopGroup)groupIt.next();
            markup.append(getMarkupGenerator(wiContext).getGroupMarkup(desktopGroup, wiContext));
        }

        return markup.toString();
    }

    /**
     * Updates the desktop group's resource control to reflect that the desktop
     * has now been assigned.
     * Also sets the desktop host name on that desktop and returns the markup
     * for the assigned desktop.
     *
     * @param wiContext the WIContext object
     * @param desktopHostName the desktop host name to set on the desktop
     * @param desktopInfo desktop to modify
     * @return the html markup of the newly assigned desktop
     * @throws NoSuchResourceException if desktop described by desktopInfo is not stored inside this DesktopGroupController
     * @throws IllegalArgumentException if desktopInfo is null
     */
    public String assignDesktopAndGetMarkup(WIContext wiContext, String desktopHostName, DesktopInfo desktopInfo) throws NoSuchResourceException {
        if (desktopInfo == null) {
            throw new IllegalArgumentException("desktopInfo cannot be null!");
        }

        GroupKey key = new GroupKey(desktopInfo);

        DesktopGroup group = (DesktopGroup)desktopGroups.get(key);
        if (group == null) {
            throw new NoSuchResourceException("cannot assign an aofu desktop", desktopInfo.getId());
        }
        ResourceControl assignedDesktop = group.assignDesktop(desktopInfo.getId(), desktopHostName, wiContext);

        return getMarkupGenerator(wiContext).getDesktopMarkup(assignedDesktop, wiContext);
    }

    /**
     * Comparator class. Determines the order in which groups are displayed
     * and whether two desktops belong to the same group. This class' "compare"
     * method is used each time a desktop is added into the desktopGroups map.
     */
    private class SortByFriendlyName implements Comparator {
        public int compare(Object o1, Object o2) {
            GroupKey k1 = (GroupKey)o1;
            GroupKey k2 = (GroupKey)o2;

            // Two desktops belong to the same desktop group if they have:
            // 1. Equal friendly names.
            // 2. Originate from the same farm.
            int fnameResult = k1.friendlyName.compareToIgnoreCase(k2.friendlyName);

            int result = (fnameResult == 0) ? k1.sourceFarm.compareTo(k2.sourceFarm) : fnameResult;
            return result;
        }
    }

    /**
     * Key to session state where the DesktopGroupsController is stored.
     */
    public static final String SV_DESKTOP_GROUPS_CONTROLLER = "DesktopGroupsController";

    /**
     * Stores this object in the session state.
     *
     * @param wiContext the WIContext object
     */
    public void storeInSession(WIContext wiContext) {
        wiContext.getUserEnvironmentAdaptor().getSessionState().put(SV_DESKTOP_GROUPS_CONTROLLER, this);
    }

    /**
     * This class serves as a key for the desktop groups map.
     * When two desktops have equal friendly name and originate
     * from the same farm, they belong to the same group.
     */
    private class GroupKey {
        public GroupKey(String sourceFarm, String friendlyName) {
            this.sourceFarm = sourceFarm;
            this.friendlyName = friendlyName;
        }

        public GroupKey(DesktopInfo desktopInfo) {
            this.sourceFarm   = desktopInfo.getNameOfSource();
            this.friendlyName = desktopInfo.getDisplayName();
        }

        public GroupKey() { }

        public String friendlyName;
        public String sourceFarm;
    }

}
