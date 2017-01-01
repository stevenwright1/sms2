/*
 * Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.clientdetect.controllers;

import java.io.IOException;

import com.citrix.wi.clientdetect.Mode;
import com.citrix.wi.clientdetect.WizardConstants;
import com.citrix.wi.clientdetect.WizardContext;
import com.citrix.wi.clientdetect.WizardUtil;
import com.citrix.wi.clientdetect.models.NativeDownloadedViewModel;
import com.citrix.wi.clientdetect.type.ClientType;
import com.citrix.wi.clientdetect.WizardConstants;
import com.citrix.wi.clientdetect.util.WizardBrowserTitleBuilder;
import com.citrix.wi.types.ProductType;
import com.citrix.wing.util.WebUtilities;

/**
 * This controls the NativeDownloaded page
 */
public class NativeDownloadedController extends Controller {
    private NativeDownloadedViewModel viewModel = new NativeDownloadedViewModel();

    public NativeDownloadedController(WizardContext wizardContext) {
        super(wizardContext);
        webAbstraction.setRequestContextAttribute(WizardConstants.VIEW_MODEL, viewModel);
    }

    public boolean perform() throws IOException {

        viewModel.upgrade = WizardConstants.VAL_TRUE.equals(webAbstraction.getQueryStringParameter(WizardConstants.QSTR_UPGRADE));

        viewModel.skipWizardLink = viewModel.upgrade ? "javascript:upgradeLater()" :
            WebUtilities.escapeHTML(WizardUtil.getUrlWithQueryStrWithCsrf(wizardContext, WizardConstants.PAGE_FINISH, WizardConstants.SKIPPED, WizardConstants.VAL_TRUE));
        if (viewModel.upgrade) {
            viewModel.skipWizardTextKey = "UpgradeLater";
            viewModel.skipWizardTooltipKey = "UpgradeLaterButtonTooltip";
        } else {
            viewModel.skipWizardTextKey = wizardContext.getInputs().allowLogout() ? "SkipWizardLinkText" : "SkipToLoginText";
            viewModel.skipWizardTooltipKey = wizardContext.getInputs().allowLogout() ? "SkipWizardLinkTooltip" : "SkipToLoginTooltip";
        }

        String pageTitle = wizardContext.getClientInfo().isIE() ? "Ica-LocalEnableActiveXPageTitle"
                        : "NativeClientDownloadedPageTitle";
        viewModel.pageTitle = WizardBrowserTitleBuilder.createTitle(wizardContext, pageTitle);

        viewModel.IEPollingMode = WizardUtil.isClientDetectionPollingSupported(wizardContext.getClientInfo())
                        && wizardContext.getClientInfo().isIE();

        if (viewModel.IEPollingMode) {
            // IE polling mode
            String enableActivexImage = (wizardContext.getInputs().getProductType() == ProductType.XEN_DESKTOP ? WizardConstants.IMG_ACTIVEX_XD
                            : WizardConstants.IMG_ACTIVEX_XA_NATIVE);
            viewModel.enableActivexImageUrl = WizardUtil.getLocalizedImageName(enableActivexImage, wizardContext);

        } else {
            // Firefox (and others) mode
            viewModel.userSuccessPage = WebUtilities.escapeHTML("alreadyInstalled('"
                            + wizardContext.getModel().getNextStepWithCsrf(wizardContext,
                                            wizardContext.getInputs().getMode() != Mode.ADVANCED) + "')");
        }

        viewModel.autoSuccessPage = WebUtilities.escapeHTML("location.href='"
                        + wizardContext.getModel().getCurrentStep() + "'");

        setupTroubleshootingLinks();

        // use the wizard model to
        // show the link to use Java, when Java is available and in the remote client list
        boolean isAdvancedMode = (wizardContext.getInputs().getMode() == Mode.ADVANCED);
        viewModel.showUseJavaLink = !isAdvancedMode
                && wizardContext.getInputs().isJavaFallback()
                && wizardContext.getModel().isClientDetected(ClientType.JAVA);

        return true;
    }

    private void setupTroubleshootingLinks() {
        WizardUtil.setupCommonWizardLinks(wizardContext, viewModel);
    }

}
