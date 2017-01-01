/*
 * Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.clientdetect.controllers;

import java.io.IOException;

import com.citrix.wi.clientdetect.Mode;
import com.citrix.wi.clientdetect.WizardConstants;
import com.citrix.wi.clientdetect.WizardContext;
import com.citrix.wi.clientdetect.WizardUtil;
import com.citrix.wi.clientdetect.models.PopupHelpViewModel;
import com.citrix.wi.clientdetect.type.ClientType;
import com.citrix.wi.clientdetect.util.WizardBrowserTitleBuilder;
import com.citrix.wing.util.WebUtilities;

/**
 * This controls the popup help page
 */
public class PopupHelpController extends Controller {
    private PopupHelpViewModel viewModel = new PopupHelpViewModel();

    public PopupHelpController(WizardContext wizardContext) {
        super(wizardContext);
        webAbstraction.setRequestContextAttribute(WizardConstants.VIEW_MODEL, viewModel);
    }

    public boolean perform() throws IOException {
        viewModel.pageTitle = WizardBrowserTitleBuilder.createTitle(wizardContext, "PopupHelpPageTitle");
        WizardUtil.setupCommonWizardLinks(wizardContext, viewModel);

        if (wizardContext.getInputs().getMode() == Mode.ADVANCED) {
            viewModel.urlCancelLink = WebUtilities.escapeHTML(wizardContext.getModel().getNextStepWithCsrf(wizardContext));
        } else {
            viewModel.urlCancelLink = WebUtilities.escapeHTML(WizardUtil.getUrlWithQueryStrWithCsrf(wizardContext, WizardConstants.PAGE_FINISH, WizardConstants.SKIPPED, WizardConstants.VAL_TRUE));
        }

        String currentStep = wizardContext.getModel().getCurrentStep();
        ClientType currentStepClientType = ClientType.getClientTypeFromCurrentStep(currentStep);

        if (ClientType.JAVA.equals(currentStepClientType)) {
            viewModel.allowPopupsTextKey = "AllowPopupsJava";
        } else if (ClientType.RDP.equals(currentStepClientType)) {
            viewModel.allowPopupsTextKey = "AllowPopupsRDP";
        }
        return true;
    }
}
