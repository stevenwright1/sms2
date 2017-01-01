/*
 * DesktopGroup.java
 * Copyright (c) 2010 Citrix Systems, Inc.  All Rights Reserved.
 */
package com.citrix.wi.controls;

import com.citrix.wi.controls.ResourceControl;

import java.util.Collections;
import java.util.Comparator;
import java.util.Iterator;
import java.util.List;
import java.util.ArrayList;

import com.citrix.wing.types.DesktopAssignmentType;
import com.citrix.wing.util.Strings;
import com.citrix.wing.webpn.DesktopInfo;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.LaunchUtilities;
import com.citrix.wi.mvc.WIContext;
import java.util.TreeMap;
import java.util.Collection;
import java.util.Comparator;

/**
 * This class represents a group of desktops. It is used on the desktop tabs view
 * to display groups in a particular order.
 */
public class DesktopGroup {
    // List of desktops that are not aofu.
	private TreeMap desktops = new TreeMap(new SortByAppId());

    /**
     * Returns the list of  all desktops.
     * @return the list of  all desktops
     */
    public List getDesktops() {
        return new ArrayList(desktops.values());
    }

    /**
     * Assigns an aofu desktop to the user.
     * The method moves the desktop from unassignedDesktops to otherDesktops,
     * changing its type.
     * 
     * @param appId id of the desktop to assign
     * @param vmName the name of vm to set on desktop for display purposes
     * @param wiContext the wi context
     * @throw IllegalArgumentException if appId is not an id of an aofu desktop
     * that belongs to this group or if vmName is null
     * @return the desktop after the assignment
     */
    public ResourceControl assignDesktop(String appId, String vmName, WIContext wiContext) {
        if (appId == null) {
            throw new IllegalArgumentException("appId cannot be null");
        }

        if (vmName == null) {
            throw new IllegalArgumentException("vmName cannot be null");
        }

		ResourceControl rc = (ResourceControl)desktops.get(appId);
        if (rc == null) {
            throw new IllegalArgumentException("appId " + appId + " is not an aofu desktop");
        }

        // Regenerate the link markup so that the onclick handler performs a launch rather than
        // attempting another assignment.
        boolean requiresDesktopAssignment = false;
        rc.launchHref = Include.processAppLink(wiContext, rc.id, null, false, requiresDesktopAssignment);
        rc.netbiosName = vmName;
        rc.requiresAssignment = false;

        return rc;
    }

	public ResourceControl getDesktop(String appId) {
		return (ResourceControl)desktops.get(appId);
	}

    /**
     * Adds a desktop to the group, sorted alphabetically.
     * 
     * @param resource ResourceControl of the desktop being added
     * @param desktopInfo DesktopInfo of the desktop being added
     * @throws IllegalArgumentException if either resource or desktopInfo parameter is null
     */
    public void addDesktop(ResourceControl resource, DesktopInfo desktopInfo) {
        if (resource == null) {
            throw new IllegalArgumentException("resource cannot be null");
        }

        if (desktopInfo == null) {
            throw new IllegalArgumentException("desktopInfo cannot be null");
        }

        desktops.put(resource.id, resource);

        // (Re)generate the desktop names.
		Collection desktopsList = getDesktops();
		// Set a number that will be displayed as a part of the desktop name,
        // if there is more than one desktop in the group.
		if (desktopsList.size() > 1) {
			Iterator it = desktopsList.iterator();

			for (int i = 1; it.hasNext(); i++) {
				ResourceControl rc = (ResourceControl)it.next();
                rc.desktopIndex = i;
			}
		}
    }

    /**
     * Comparator class. Defines a sorting order, which is needed to keep
     * the order of shared desktops the same regardless of the adding order.
     */
    private class SortByAppId implements Comparator {
        public int compare(Object o1, Object o2) {
			int result = Strings.compare((String)o1, (String)o2);
            return result;
        }
    }

    /**
     * Returns ResourceControl object for a desktop from the group.
     * 
     * @return ResourceControl object for a desktop from the group
     */
    public ResourceControl getFirstResource() {
        ResourceControl result = null;

        if (!desktops.isEmpty()) {
            result = (ResourceControl)desktops.get((String)desktops.firstKey());
        }
        return result;
    }

}

