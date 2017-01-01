/*
 * Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.clientdetect.controllers;

import java.io.IOException;

import com.citrix.wi.clientdetect.Mode;
import com.citrix.wi.clientdetect.WizardConstants;
import com.citrix.wi.clientdetect.WizardContext;
import com.citrix.wi.clientdetect.WizardUtil;
import com.citrix.wi.clientdetect.models.UpgradeViewModel;
import com.citrix.wi.clientdetect.util.WizardBrowserTitleBuilder;

public class UpgradeRadeController extends Controller {
    private UpgradeViewModel viewModel = new UpgradeViewModel();

    public UpgradeRadeController(WizardContext wizardContext) {
        super(wizardContext);
        webAbstraction.setRequestContextAttribute(WizardConstants.VIEW_MODEL, viewModel);
    }

    public boolean perform() throws IOException {
        viewModel.pageTitle = WizardBrowserTitleBuilder.createTitle(wizardContext, "UpgradeRadePageTitle");

        // HTML setup
        viewModel.pageHeadingKey = "UpgradeRadePageHeading";

        WizardUtil.setupCommonWizardLinks(wizardContext, viewModel);
        viewModel.showUpgradeLaterLink = wizardContext.getInputs().getMode() != Mode.ADVANCED;
        viewModel.showSkipLink = false;

        // JavaScript setup
        viewModel.downloadUrl = WizardUtil.getStreamingClientDownloadUrl(wizardContext);
        viewModel.downloadedUrl = WizardConstants.PAGE_RADE_DOWNLOADED + "?" + WizardConstants.QSTR_UPGRADE + "="
                        + WizardConstants.VAL_TRUE;

        return true;
    }

}
