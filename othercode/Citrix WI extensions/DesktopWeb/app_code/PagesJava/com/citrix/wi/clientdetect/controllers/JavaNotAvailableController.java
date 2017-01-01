/*
 * Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.clientdetect.controllers;

import java.io.IOException;

import com.citrix.wi.clientdetect.Mode;
import com.citrix.wi.clientdetect.WizardConstants;
import com.citrix.wi.clientdetect.WizardContext;
import com.citrix.wi.clientdetect.WizardUtil;
import com.citrix.wi.clientdetect.models.JavaNotAvailableViewModel;
import com.citrix.wi.clientdetect.util.WizardBrowserTitleBuilder;
import com.citrix.wing.util.WebUtilities;

/**
 * Controls the JavaNotAvalible page
 */
public class JavaNotAvailableController extends Controller {

    private JavaNotAvailableViewModel viewModel = new JavaNotAvailableViewModel();

    public JavaNotAvailableController(WizardContext wizardContext) {
        super(wizardContext);
        webAbstraction.setRequestContextAttribute(WizardConstants.VIEW_MODEL, viewModel);
    }

    public boolean perform() throws IOException {
        viewModel.pageTitle = WizardBrowserTitleBuilder.createTitle(wizardContext, "JavaNotAvailablePageTitle");
        viewModel.showContinueText = wizardContext.getModel().getNextStep() != WizardConstants.PAGE_FINISH;
        WizardUtil.setupCommonWizardLinks(wizardContext, viewModel);

        if (wizardContext.getInputs().getMode() == Mode.ADVANCED) {
            viewModel.urlCancelLink = WebUtilities.escapeHTML(wizardContext.getModel().getNextStepWithCsrf(wizardContext));
        } else {
            viewModel.urlCancelLink = WebUtilities.escapeHTML(WizardUtil.getUrlWithQueryStrWithCsrf(wizardContext, WizardConstants.PAGE_FINISH, WizardConstants.SKIPPED, WizardConstants.VAL_TRUE));
        }

        return true;
    }

}
