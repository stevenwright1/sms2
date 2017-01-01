/*
 * Copyright (c) 2007-2010 Citrix Systems, Inc.  All Rights Reserved.
 */
package com.citrix.wi.controls;
import com.citrix.wi.types.LayoutMode;

/**
 * Maintains presentation state for the Layout control.
 */
public class LayoutControl {

    public boolean isLoginPage = false;
	public boolean isLoggedOutPage = false;
    public boolean isAppListPage = false;
	public boolean isPreferencesPage = false;
    public boolean isSearchPage = false;
    public boolean embeddedLayout = false;
    public boolean hasLightbox = false;
    public boolean hasCancelButton = false;
    public boolean showApplistBox = true;
    public boolean showBackgroundGradient = false;

    public LayoutMode layoutMode = LayoutMode.ADVANCED;
    public String browserTitle = "";
    public String formAction = null;
}
