// DelayedLaunchControl.java
// Copyright (c) 2009 - 2010 Citrix Systems, Inc. All Rights Reserved.
// [NFuseVersionAndBuildNumber]
package com.citrix.wi.controls;

import java.util.Enumeration;
import java.util.Map;
import java.util.HashMap;
import java.util.Set;
import java.util.HashSet;
import com.citrix.wing.AppLaunchInfo;
import com.citrix.wing.webpn.ResourceInfo;
import com.citrix.wing.webpn.ApplicationInfo;
import com.citrix.wing.webpn.DesktopInfo;
import com.citrix.wing.webpn.DocumentInfo;

/**
 * This maintains the following collections:
 * 1. Pending Applications, Desktops and Content - Those are for storing a RetryControl for each resource that needs to be retried and
 *    is pending to launch. The key is the appId of the resource and value is the RetryControl
 * 2. Ready To Launch - This is for storing all the resources that are ready to launch. It stores a launchInfo object
 *    for each resource that is ready to launch and hasn't yet had an ica file generated. This is needed to cater for cases
 *    when launcher succeeds but before the ica file can be generated the page refreshes. The key is the appId of the
 *    resource and the value is the LaunchInfo
 * 3. Blocked Launches - This is to store a list of appIds for resources which are ready, but cannot be auto launched.
 * 4. Power Off Resources - This is for storing the resources that are to be powered off as the first step of a delayed launch.
 * 5. Restarting Resources - This is for storing the resources that are restarting after being powered off.
 */
public class DelayedLaunchControl {
    private Map pendingDesktops = new HashMap();
    private Map pendingApps = new HashMap();
    private Map pendingContent = new HashMap();
    private Map readyToLaunchResources = new HashMap();
    private Set blockedLaunches = new HashSet();
    private Set powerOffResources = new HashSet();
    private Set restartingResources = new HashSet();

    public DelayedLaunchControl() {
    }

    public boolean hasPendingDesktops() {
        return !pendingDesktops.isEmpty();
    }

    public boolean hasPendingContent() {
        return !pendingContent.isEmpty();
    }

    public boolean hasPendingApps() {
        return !pendingApps.isEmpty();
    }

    /* Checks whether there are any pending resources.
     * @return boolean true if there are pending resources, false otherwise
     */
    public boolean isResourcePendings() {
        return (hasPendingApps() || hasPendingDesktops() || hasPendingContent());
    }

    /**
     * Checks whether the given appId is in the pending resources list.
     * @param appId of the resource that needs to be checked
     * @return boolean true if the resource with the given appId is pending.
     */
    public boolean isResourcePending(String appId) {
        if (appId == null) {
            throw new IllegalArgumentException();
        }
        boolean hasApp = pendingApps.containsKey(appId);
        boolean hasDesktop = pendingDesktops.containsKey(appId);
        boolean hasContent = pendingContent.containsKey(appId);
        return (hasApp || hasDesktop || hasContent);
    }

    /**
     * Checks if the given appId is in the delayed launch history
     */
    public boolean isBlockedLaunch(String appId) {
        if (appId == null) {
            throw new IllegalArgumentException();
        }
        return blockedLaunches.contains(appId);
    }

    /**
     * Checks whether there are any resource that are ready to launch
     */
    public boolean isResourceReadyToLaunchs() {
        return !readyToLaunchResources.isEmpty();
    }

    /**
     * Checks whether the given appId is in the ready to launchResources list
     */
    public boolean isResourceReadyToLaunch(String appId) {
        if (appId == null) {
            throw new IllegalArgumentException();
        }
        return readyToLaunchResources.containsKey(appId);
    }

    /**
     * Gets the keys of pending resources.
     * @return Set of pending resources id's.
     */
    public Set getPendingResourceIds() {
        Map pendingResources = new HashMap();
        pendingResources.putAll(pendingApps);
        pendingResources.putAll(pendingDesktops);
        pendingResources.putAll(pendingContent);

        return pendingResources.keySet();
    }

    /**
     * Gets the keys of ready to launch resources.
     * @return Set of ready to launch resources id's.
     */
    public Set getReadyToLaunchResourceIds() {
        return readyToLaunchResources.keySet();
    }

    /**
     * Adds the given RetryControl to the pending collection with the appId as the key.
     * The type of the resource is determined using resInfo.
     * If the appId or retryControl is null then this throws IllegalArgumentException.
     */
    public void addPendingResource(ResourceInfo resInfo, RetryControl retryControl) {
        if (retryControl == null || resInfo == null) {
            throw new IllegalArgumentException();
        }
        String appId = resInfo.getId();
        blockedLaunches.remove(appId);

        if (resInfo instanceof DesktopInfo) {
            pendingDesktops.put(appId, retryControl);
        } else if (resInfo instanceof ApplicationInfo) {
            pendingApps.put(appId, retryControl);
        } else if (resInfo instanceof DocumentInfo) {
            pendingContent.put(appId, retryControl);
        }
    }

    /**
     * Gets the RetryControl for the given appId.
     * 
     * @return null if appId is null or there is no pending resource with the given appId
     */
    public RetryControl getPendingResourceRetryControl(String appId) {
        if (appId == null) {
            throw new IllegalArgumentException();
        }
        RetryControl result = null;
            
        if (pendingApps.containsKey(appId)) {
            result = (RetryControl)pendingApps.get(appId);
        } else if (pendingDesktops.containsKey(appId)) {
            result = (RetryControl)pendingDesktops.get(appId);
        } else if (pendingContent.containsKey(appId)) {
            result = (RetryControl)pendingContent.get(appId);
        }

        return result;
    }

    /**
     * This method removes the RetryControl of the given appId and stores the launchInfo
     * with the given appId in the readyToLaunchResource list.
     * This method does not do anything if there are no pending resource with the given appId.
     */
    public void changePendingResourceToReady(String appId, AppLaunchInfo launchInfo) {
        if (appId == null || launchInfo == null) {
            throw new IllegalArgumentException();
        }
        boolean foundResource = false;
        if (launchInfo.getIsDesktop() && pendingDesktops.containsKey(appId)) {
            foundResource = true;
            pendingDesktops.remove(appId);
        } else if (pendingApps.containsKey(appId)) {
            foundResource = true;
            pendingApps.remove(appId);
        } else if (pendingContent.containsKey(appId)) {
            foundResource = true;
            pendingContent.remove(appId);
        }

        if (foundResource) {
            // Remove from the power off resources list if present.
            powerOffResources.remove(appId);
            readyToLaunchResources.put(appId, launchInfo);
        }
    }

    /**
     * Adds the launchInfo for the given appId in the ready to launch resource list.
     */
    public void addReadyToLaunchResource(String appId, AppLaunchInfo launchInfo) {
        if (appId == null || launchInfo == null) {
            throw new IllegalArgumentException();
        }
        readyToLaunchResources.put(appId, launchInfo);
    }

    /**
     * This method removes the RetryControl for the given appId from the pending resources list
     * This method does not add appId to the readyToLaunch list. See <code>changePendingResourceToReady</code>
     * for moving the pending resource to the ready to launch list
     */
    public void removePendingResource(String appId) {
        if (appId == null) {
            throw new IllegalArgumentException();
        }
        pendingApps.remove(appId);
        pendingDesktops.remove(appId);
        pendingContent.remove(appId);
    }

    /**
     * Gets the launchInfo stored in the ready to launch resource list for the given appId
     */
    public AppLaunchInfo getResourceLaunchInfo(String appId) {
        if (appId == null) {
            throw new IllegalArgumentException();
        }
        AppLaunchInfo launchInfo = (AppLaunchInfo)readyToLaunchResources.get(appId);
        return launchInfo;
    }

    /**
     * Removes the ready to launch resource from the list.
     */
    public void removeReadyToLaunchResource(String appId) {
        if (appId == null) {
            throw new IllegalArgumentException();
        }

        readyToLaunchResources.remove(appId);
    }

    /**
     * Move the resource appId from the readyToLaunchResources to blockedLaunches list.
     */
    public void changeReadyStatusToBlocked(String appId) {
        if (appId == null) {
            throw new IllegalArgumentException();
        }

        if (readyToLaunchResources.containsKey(appId)) {
            blockedLaunches.add(appId);
            readyToLaunchResources.remove(appId);
        }
    }

    /**
     * Removes the resource with the given appId from the history
     */
    public void removeBlockedLaunch(String appId) {
        if (appId == null) {
            throw new IllegalArgumentException();
        }
        blockedLaunches.remove(appId);
    }

    /**
     * Adds the resource with the given appId to the power off resources list
     */
    public void addPowerOffResource(String appId) {
        if (appId == null) {
            throw new IllegalArgumentException();
        }
        powerOffResources.add(appId);
    }

    /**
     * Removes the resource with the given appId from the power off resources list
     */
    public void removePowerOffResource(String appId) {
        if (appId == null) {
            throw new IllegalArgumentException();
        }
        powerOffResources.remove(appId);
    }

    /**
     * Checks if the given appId is in the power off resources list.
     */
    public boolean hasPowerOffResource(String appId) {
        if (appId == null) {
            throw new IllegalArgumentException();
        }
        return powerOffResources.contains(appId);
    }

    /**
     * Adds the resource with the given appId to the restarting resources list
     */
    public void addRestartingResource(String appId) {
        if (appId == null) {
            throw new IllegalArgumentException();
        }
        restartingResources.add(appId);
    }

    /**
     * Removes the resource with the given appId from the restarting resources list
     */
    public void removeRestartingResource(String appId) {
        if (appId == null) {
            throw new IllegalArgumentException();
        }

        restartingResources.remove(appId);
    }

    /**
     * Checks if the given appId is in the restarting resources list.
     */
    public boolean hasRestartingResource(String appId) {
        if (appId == null) {
            throw new IllegalArgumentException();
        }
        return restartingResources.contains(appId);
    }
}

