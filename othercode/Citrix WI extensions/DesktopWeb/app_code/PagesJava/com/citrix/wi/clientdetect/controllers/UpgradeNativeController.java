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
import com.citrix.wi.types.ProductType;

/**
 * This controls the upgrade native page
 */
public class UpgradeNativeController extends Controller {
    private UpgradeViewModel viewModel = new UpgradeViewModel();

    public UpgradeNativeController(WizardContext wizardContext) {
        super(wizardContext);
        webAbstraction.setRequestContextAttribute(WizardConstants.VIEW_MODEL, viewModel);
    }

    public boolean perform() throws IOException {
        // HTML setup
        viewModel.pageTitle = WizardBrowserTitleBuilder.createTitle(wizardContext, "UpgradeNativePageTitle");
        viewModel.pageHeadingKey = "UpgradeNativePageHeading";
        viewModel.licenseAgreementKey = "LicenseAgreement";

        viewModel.customCaption = WizardUtil.getCustomCaptionMessage(wizardContext);
        if ("".equals(viewModel.customCaption)) {
            viewModel.customCaption = null;
        }

        viewModel.showLicenceAgreement = WizardUtil.showClientLicenseAgreement(wizardContext);

        WizardUtil.setupCommonWizardLinks(wizardContext, viewModel);
        viewModel.showUpgradeLaterLink = wizardContext.getInputs().getMode() != Mode.ADVANCED;
        viewModel.showSkipLink = false;

        // JavaScript setup
        viewModel.downloadUrl = WizardUtil.getClientDownloadUrl(wizardContext);
        viewModel.downloadedUrl = WizardConstants.PAGE_NATIVE_DOWNLOADED + "?" + WizardConstants.QSTR_UPGRADE + "="
                        + WizardConstants.VAL_TRUE;

        return true;
    }

}
