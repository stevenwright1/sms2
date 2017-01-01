/*
 * Copyright (c) 2007 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.controls;

import com.citrix.wing.util.WebUtilities;
import com.citrix.wing.webpn.ResourceInfo;
import com.citrix.wing.webpn.DesktopInfo;
import com.citrix.wi.pageutils.DelayedLaunchUtilities;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.ApplistUtils;
import com.citrix.wi.pageutils.LaunchUtilities;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wing.webpn.UserContext;
import com.citrix.wing.types.DesktopAssignmentType;
import com.citrix.wing.util.Strings;


/**
 * Maintains presentation state for a single resource on the applist page.
 */
public class ResourceControl {
    public static ResourceControl fromResourceInfo(ResourceInfo resInfo,
                                                  WIContext wiContext,
                                                  UserContext userContext,
                                                  int appIconSize) {
        return fromResourceInfo(resInfo, wiContext, userContext, appIconSize, false, false, null);
    }

    /**
     * Builds a ResourceControl object based on specified data.
     * 
     * @param resInfo resource info
     * @param wiContext the wi context
     * @param appIconSize 
     */
    public static ResourceControl fromResourceInfo(ResourceInfo resInfo,
                                                   WIContext wiContext,
                                                   UserContext userContext,
                                                   int appIconSize,
                                                   boolean isTreeView,
                                                   boolean forceAppSize,
                                                   Boolean showDescription) {

        ResourceControl resource = new ResourceControl();
        resource.launchHref = Include.processAppLink(wiContext, resInfo);
        resource.publishedDisplayName = resInfo.getDisplayName();
        resource.id = resInfo.getId();
        resource.publishedDescription = resInfo.getDescription();
        resource.isRestartable = LaunchUtilities.canPowerOffResource(resInfo);
        resource.iconImg = ApplistUtils.getIconMarkup(wiContext, resInfo, appIconSize, isTreeView, forceAppSize, showDescription);

        if (resInfo instanceof DesktopInfo) {
            DesktopInfo desktopInfo = (DesktopInfo)resInfo;
            resource.requiresAssignment = (desktopInfo.getAssignmentType() == DesktopAssignmentType.ASSIGN_ON_FIRST_USE);
            resource.netbiosName = desktopInfo.getHostName();
        }

        fillInDelayedLaunchDetails(resource, wiContext, resInfo);

        return resource;
    }


    private static void fillInDelayedLaunchDetails(ResourceControl resource, WIContext wiContext, ResourceInfo resInfo) {
        DelayedLaunchControl delayedLaunchControl = DelayedLaunchUtilities.getDelayedLaunchControl(wiContext);

        RetryControl retryControl = delayedLaunchControl.getPendingResourceRetryControl(resource.id);
        if (retryControl != null) {
            resource.isDelayedLaunch = true;
            resource.startInProgress = true;
            resource.retryControl = retryControl;
        } else if (delayedLaunchControl.isBlockedLaunch(resource.id)) {
            resource.isDelayedLaunch = true;
            resource.startInProgress = false;
            resource.launchHtmlLink = DelayedLaunchUtilities.getLaunchLink(wiContext, resInfo);
        }
    }

    /**
     * Published display name as received from the XML service.
     */
    public String publishedDisplayName;

    /**
     * Published description as received from the XML service.
     */
    public String publishedDescription;

    /**
     * The URL used to launch the resource.
     */
    public String launchHref;

    /**
     * The URL for the icon representing the resource.
     */
    public String iconImg;

    /**
     * The URL used to open the folder containing the resource.
     */
    public String pathUrl;

    /**
     * The text used to describe the folder containing the resource.
     */
    public String pathText;

    /**
     * The index of the desktop within its group.
     * 0 means there is only a single desktop in the group.
     */
    public int desktopIndex = 0;

    /**
     * The desktop netbios name.
     */
    public String netbiosName = null;

    /**
     * Returns a resource description in format:
     * desktop name (n) [netbios name] - published description.
     * all parts but desktop name are optional.
     * 
     * @return a resource description
     */
    public String getDescription(WIContext wiContext) {
        String description = getName(wiContext);

        if (!Strings.isEmptyOrWhiteSpace(netbiosName)) {
            description += " [" + netbiosName + "]";
        }
        if (!Strings.isEmptyOrWhiteSpace(publishedDescription)) {
            description += " - " + publishedDescription;
        }

        return description;
    }

    public String getTruncatedName(WIContext wiContext) {
    	final int MAX_NAME_LENGTH = 28;
        final String TRUNCATION_STRING = "...";
        String desktopName = getName(wiContext);

        if (desktopName.length() > MAX_NAME_LENGTH) {
        	final int NUMBER_OF_SEGMENTS = 2;
            int segmentLength = (MAX_NAME_LENGTH - TRUNCATION_STRING.length()) / NUMBER_OF_SEGMENTS;
            desktopName = desktopName.substring(0, segmentLength) + TRUNCATION_STRING +
                          desktopName.substring(desktopName.length() - segmentLength);
        }

        return desktopName;
    }

    /**
     * The compound name of the resource.
     */
    public String getName(WIContext wiContext) {
        String fullDisplayName = DesktopGroupsController.getInstance(wiContext).getCompoundDesktopDisplayName(publishedDisplayName, id);
        return fullDisplayName;
    }

    /**
     * The id of the resource.
     */
    public String id;

    /**
     * Whether the resource is restartable (for desktops).
     */
    public boolean isRestartable;

    /**
     * Whether the resource needs to be assigned before used.
     */
    public boolean requiresAssignment = false;

    /**
     * Whether the resource represents a delayed launch control or not.
     */
    public boolean isDelayedLaunch;

    /**
     * Gets the id encoded string of the appId
     */
    public String getEncodedAppId() {
        return WebUtilities.encodeForId(id);
    }

    // html link to launch the resource
    public String launchHtmlLink;

    // flag to indicate whether a start is already in progress.
    public boolean startInProgress = false;

    // the retry control relevant to the corresponding delayed launch
    public RetryControl retryControl;

    /**
     * Icon to display for delayed launches.
     * @param isDesktopsTab indicates whether get images for desktops tab
     * @return url of the icon to display for delayed launches
     */
    public String getDelayedLaunchImgSrc(boolean isDesktopsTab) {
        String imgSrc = "../media/Transparent16.gif";
        if (isDelayedLaunch) {
            if (isDesktopsTab) {
                imgSrc = startInProgress ? "../media/Loader.gif" : "../media/DesktopReady.png";
            } else {
                imgSrc = startInProgress ? "../media/LaunchSpinner.gif" : "../media/LaunchReady.gif";
            }
        }
        return imgSrc;
    }

}