/*
 * Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.clientdetect.controllers;

import java.io.IOException;

import com.citrix.wi.clientdetect.Mode;
import com.citrix.wi.clientdetect.WizardConstants;
import com.citrix.wi.clientdetect.WizardContext;
import com.citrix.wi.clientdetect.WizardUtil;
import com.citrix.wi.clientdetect.models.ChangeZoneViewModel;
import com.citrix.wi.clientdetect.type.ClientType;
import com.citrix.wi.clientdetect.util.WizardBrowserTitleBuilder;
import com.citrix.wing.util.WebUtilities;

/**
 * This controls the changeZone page
 */
public class ChangeZoneController extends Controller {

    private ChangeZoneViewModel viewModel = new ChangeZoneViewModel();

    public ChangeZoneController(WizardContext wizardContext) {
        super(wizardContext);
        webAbstraction.setRequestContextAttribute(WizardConstants.VIEW_MODEL, viewModel);
    }

    public boolean perform() throws IOException {
        viewModel.pageTitle = WizardBrowserTitleBuilder.createTitle(wizardContext, "ChangeZoneHelpPageTitle");

        String currentStep = wizardContext.getModel().getCurrentStep();
        ClientType currentStepClientType = ClientType.getClientTypeFromCurrentStep(currentStep);

        String siteUri = webAbstraction.getBaseURL();
        viewModel.siteURL = siteUri.substring(0, siteUri.indexOf(webAbstraction.getApplicationPath()));

        viewModel.trustedZoneTextKey = "AddSiteToTrustedZoneNative";
        if (currentStepClientType == ClientType.NATIVE) {
            viewModel.trustedZoneTextKey = "AddSiteToTrustedZoneNative";
        } else if (currentStepClientType == ClientType.RADE) {
            viewModel.trustedZoneTextKey = "AddSiteToTrustedZoneRade";
        } else if (currentStepClientType == ClientType.RDP) {
            viewModel.trustedZoneTextKey = "AddSiteToTrustedZoneRdp";
        }

        viewModel.urlSiteAddedLink = wizardContext.getModel().getCurrentStep();

        // hide the skip and logout links in advanced mode
        WizardUtil.setupCommonWizardLinks(wizardContext, viewModel);

        if (wizardContext.getInputs().getMode() == Mode.ADVANCED) {
            viewModel.urlCancelLink = WebUtilities.escapeHTML(wizardContext.getModel().getNextStepWithCsrf(wizardContext));
        } else {
            viewModel.urlCancelLink = WebUtilities.escapeHTML(WizardUtil.getUrlWithQueryStrWithCsrf(wizardContext, WizardConstants.PAGE_FINISH,
                WizardConstants.SKIPPED, WizardConstants.VAL_TRUE));
        }

        viewModel.securityMessageKey = currentStepClientType.getName() + "FurtherSecurityImplicationsZoneChange";

        return true;
    }

}
