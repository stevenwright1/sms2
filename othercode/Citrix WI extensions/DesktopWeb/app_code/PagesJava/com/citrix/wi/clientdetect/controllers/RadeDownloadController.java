/*
 * Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.clientdetect.controllers;

import java.io.IOException;

import com.citrix.wi.clientdetect.WizardConstants;
import com.citrix.wi.clientdetect.WizardContext;
import com.citrix.wi.clientdetect.WizardUtil;
import com.citrix.wi.clientdetect.models.DownloadViewModel;
import com.citrix.wi.clientdetect.util.WizardBrowserTitleBuilder;

/**
 * This controls the radeDownload page
 */
public class RadeDownloadController extends Controller {
    private DownloadViewModel viewModel = new DownloadViewModel();

    public RadeDownloadController(WizardContext wizardContext) {
        super(wizardContext);
        webAbstraction.setRequestContextAttribute(WizardConstants.VIEW_MODEL, viewModel);
    }

    public boolean perform() throws IOException {
        viewModel.pageTitle = WizardBrowserTitleBuilder.createTitle(wizardContext, "DownloadRadePageTitle");

        // HTML setup
        viewModel.pageHeadingKey = "DownloadRadePageHeading";
        WizardUtil.setupCommonWizardLinks(wizardContext, viewModel);
        viewModel.skipWizardTextKey = wizardContext.getInputs().allowLogout() ? "SkipWizardLinkText" : "SkipToLoginText";
        viewModel.skipWizardTooltipKey = wizardContext.getInputs().allowLogout() ? "SkipWizardLinkTooltip" : "SkipToLoginTooltip";

        // JavaScript setup
        viewModel.downloadUrl = WizardUtil.getStreamingClientDownloadUrl(wizardContext);
        viewModel.downloadedUrl = WizardConstants.PAGE_RADE_DOWNLOADED;

        return true;
    }

}
