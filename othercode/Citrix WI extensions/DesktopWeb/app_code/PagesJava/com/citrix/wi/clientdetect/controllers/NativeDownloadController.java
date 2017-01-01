/*
 * Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.clientdetect.controllers;

import java.io.IOException;

import com.citrix.wi.clientdetect.Mode;
import com.citrix.wi.clientdetect.WizardConstants;
import com.citrix.wi.clientdetect.WizardContext;
import com.citrix.wi.clientdetect.WizardUtil;
import com.citrix.wi.clientdetect.models.DownloadViewModel;
import com.citrix.wi.clientdetect.type.ClientType;
import com.citrix.wi.clientdetect.util.WizardBrowserTitleBuilder;
import com.citrix.wi.types.ProductType;

/**
 * This controls the NativeDownload page
 */
public class NativeDownloadController extends Controller {
    private DownloadViewModel viewModel = new DownloadViewModel();

    public NativeDownloadController(WizardContext wizardContext) {
        super(wizardContext);
        webAbstraction.setRequestContextAttribute(WizardConstants.VIEW_MODEL, viewModel);
    }

    public boolean perform() throws IOException {
        // HTML setup
        if (wizardContext.getInputs().getProductType() == ProductType.XEN_DESKTOP) {
            viewModel.pageHeadingKey = "DownloadNativePageHeadingXD";
        } else {
            viewModel.pageHeadingKey = "DownloadNativePageHeading";
        }
        viewModel.pageTitle = WizardBrowserTitleBuilder.createTitle(wizardContext, "DownloadNativePageTitle");
        viewModel.licenseAgreementKey = "LicenseAgreement";
        viewModel.skipWizardTextKey = wizardContext.getInputs().allowLogout() ? "SkipWizardLinkText" : "SkipToLoginText";
        viewModel.skipWizardTooltipKey = wizardContext.getInputs().allowLogout() ? "SkipWizardLinkTooltip" : "SkipToLoginTooltip";
        viewModel.customCaption = WizardUtil.getCustomCaptionMessage(wizardContext);
        if ("".equals(viewModel.customCaption)) {
            viewModel.customCaption = null;
        }

        viewModel.showLicenceAgreement = WizardUtil.showClientLicenseAgreement(wizardContext);

        boolean isAdvancedMode = (wizardContext.getInputs().getMode() == Mode.ADVANCED);
        viewModel.showAlreadyInstalledLink = !isAdvancedMode;

        // update the wizard model
        String cookies = (String)wizardContext.getWebAbstraction().getRequestHeader("Cookie");
        wizardContext.getModel().updateClientsResult(cookies);

        // use the wizard model to
        // show the link to use Java, when Java is available and in the remote client list
        viewModel.showUseJavaLink = !isAdvancedMode
                && wizardContext.getInputs().isJavaFallback()
                && wizardContext.getModel().isClientDetected(ClientType.JAVA);
        viewModel.forceClientUrl = wizardContext.getModel().getNextStepWithCsrf(wizardContext, true);

        WizardUtil.setupCommonWizardLinks(wizardContext, viewModel);

        // JavaScript setup
        viewModel.downloadUrl = WizardUtil.getClientDownloadUrl(wizardContext);
        viewModel.downloadedUrl = WizardConstants.PAGE_NATIVE_DOWNLOADED;

        return true;
    }

}
