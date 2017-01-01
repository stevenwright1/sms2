/*
 * Copyright (c) 2005 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */
package com.citrix.wi.clientdetect.models;

public class DownloadViewModel extends WizardViewModel {

    // for the JavaScript
    public String downloadUrl = null;
    public String downloadedUrl = null;

    // for the HTML
    public String pageHeadingKey = null;
    public String customCaption = null;
    public String licenseAgreementKey = null;
    public boolean showLicenceAgreement = false;
    public String skipWizardTextKey = null;
    public String skipWizardTooltipKey = null;

    public boolean showAlreadyInstalledLink = false;
    public String forceClientUrl = null;
    public boolean showUseJavaLink = false;

}
