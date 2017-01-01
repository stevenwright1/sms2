/*
 * Copyright (c) 2005 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */
package com.citrix.wi.clientdetect.models;

public class NativeDownloadedViewModel extends WizardViewModel {

    public boolean IEPollingMode = false;

    // non ie mode
    public String userSuccessPage = null;
    public String autoSuccessPage = null;

    // ie mode
    public String enableActivexImageUrl = null;

    // both
    public boolean showUseJavaLink = false;
    public String skipWizardLink = null;
    public String skipWizardTextKey = null;
    public String skipWizardTooltipKey = null;

    // upgrade
    public boolean upgrade = false;

}
