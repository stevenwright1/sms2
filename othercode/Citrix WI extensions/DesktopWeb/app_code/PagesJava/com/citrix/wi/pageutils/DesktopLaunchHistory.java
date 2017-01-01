/*
 * Copyright (c) 2010 Citrix Systems, Inc. All Rights Reserved.
 */

package com.citrix.wi.pageutils;

import com.citrix.wi.mvc.WIContext;
import java.util.Set;
import java.util.HashSet;


/**
 * Summary description for DesktopLaunchHistory.
 */
public class DesktopLaunchHistory {

    private static final String SV_DESKTOP_LAUNCH_HISTORY = "DesktopLaunchHistory";

    private Set launchedDesktops = new HashSet();

    private DesktopLaunchHistory() {
    }

    /**
     * Gets the DesktopLaunchHistory object from the session.
     * @param wiContext the WIContext
     * @return DelayedLaunchControl stored in the session
     */
    public static DesktopLaunchHistory getInstance(WIContext wiContext) {
        DesktopLaunchHistory desktopLaunchHistory = (DesktopLaunchHistory)wiContext.getWebAbstraction().getSessionAttribute(SV_DESKTOP_LAUNCH_HISTORY);
        if (desktopLaunchHistory == null) {
            desktopLaunchHistory = new DesktopLaunchHistory();
            wiContext.getWebAbstraction().setSessionAttribute(SV_DESKTOP_LAUNCH_HISTORY, desktopLaunchHistory);
        }
        return desktopLaunchHistory;
    }

    public void addDesktop(String appId) {
        launchedDesktops.add(appId);
    }

    public boolean containsDesktop(String appId) {
        boolean result = launchedDesktops.contains(appId);
        return result;
    }

}
