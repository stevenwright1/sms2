/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.controls;

import java.util.Set;
import java.util.HashSet;

import com.citrix.wi.pageutils.Markup;
import com.citrix.wi.types.CompactApplicationView;

/**
 * Maintains presentation state for the View Style Settings page. This page
 * is used to change view style in low graphics mode only.
 */
public class ViewStyleSettingsPageControl extends PageControl {

    /**
     * The selected view style.
     */
    public CompactApplicationView viewStyle = CompactApplicationView.LIST;

    /**
     * Low graphics view styles allowed by the admin.
     */
    public Set allowedViewStyles = new HashSet();

    /**
     * Tests whether list view is selected.
     * @return <code>checked</code> if selected else an empty string
     */
    public String getListViewCheckedStr() {
        return Markup.checkedStr(viewStyle == CompactApplicationView.LIST);
    }

    /**
     * Tests whether icons view is selected.
     * @return <code>checked</code> if selected else an empty string
     */
    public String getIconsViewCheckedStr() {
        return Markup.checkedStr(viewStyle == CompactApplicationView.ICONS);
    }
}
