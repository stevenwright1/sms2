/*
 * EmptyResourcesTab.java
 * Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */

package com.citrix.wi.tabs;

import com.citrix.wi.controls.ApplistPageControl;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.ApplistUtils;
import com.citrix.wi.pageutils.Constants;

/**
 * Represents a tab containing no resources.
 *
 * The content of the tab is a message telling the user that there are no
 * published resources available.
 */
public class EmptyResourcesTab extends Tab {

    private ApplistPageControl viewControl;
    private WIContext wiContext;

    /**
     * Creates a new <code>EmptyResourcesTab</code>.
     *
     * @param viewControl the view control for the application list page
     * @param wiContext the Web Interface Context object
     */
    public EmptyResourcesTab( ApplistPageControl viewControl, WIContext wiContext ) {
        this.viewControl = viewControl;
        this.wiContext = wiContext;
    }

    /* See Tab interface. */
    public String getId() {
        return "EmptyTab";
    }

    /* See Tab interface. */
    public String getTitle() {
        return wiContext.getString( "ScreenTitleApp" );
    }

    /* See Tab interface. */
    public String targetPage() {
        return Constants.PAGE_APPLIST;
    }

    /* See Tab interface. */
    public boolean setupViewControl() {
        viewControl.messageKey = ApplistUtils.getEmptyMessageKeyForUser( wiContext );
        return true;
    }
}
