/*
 * Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.clientdetect.controllers;

import java.io.IOException;

import com.citrix.wi.clientdetect.WizardConstants;
import com.citrix.wi.clientdetect.WizardContext;
import com.citrix.wi.clientdetect.WizardUtil;
import com.citrix.wi.clientdetect.models.WizardViewModel;
import com.citrix.wi.clientdetect.util.WizardBrowserTitleBuilder;

/**
 * This controls the radeDownload page
 */
public class RdpDownloadController extends Controller {
    private WizardViewModel viewModel = new WizardViewModel();

    public RdpDownloadController(WizardContext wizardContext) {
        super(wizardContext);
        webAbstraction.setRequestContextAttribute(WizardConstants.VIEW_MODEL, viewModel);
    }

    public boolean perform() throws IOException {
        viewModel.pageTitle = WizardBrowserTitleBuilder.createTitle(wizardContext, "EnableRdpPageHeading");
        WizardUtil.setupCommonWizardLinks(wizardContext, viewModel);
        return true;
    }

}
