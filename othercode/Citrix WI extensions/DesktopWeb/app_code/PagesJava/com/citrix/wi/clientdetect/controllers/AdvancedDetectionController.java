/*
 * Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.clientdetect.controllers;

import java.io.IOException;

import com.citrix.wi.clientdetect.WizardConstants;
import com.citrix.wi.clientdetect.WizardContext;
import com.citrix.wi.clientdetect.WizardUtil;
import com.citrix.wi.clientdetect.models.ViewModel;
import com.citrix.wi.clientdetect.util.WizardBrowserTitleBuilder;

/**
 * This controls the advancedDetection page
 */
public class AdvancedDetectionController extends Controller {

    private ViewModel viewModel = new ViewModel();

    public AdvancedDetectionController(WizardContext wizardContext) {
        super(wizardContext);
        webAbstraction.setRequestContextAttribute(WizardConstants.VIEW_MODEL, viewModel);
    }

    public boolean perform() throws IOException {
        viewModel.pageTitle = WizardBrowserTitleBuilder.createTitle(wizardContext, "AdvancedDetectionPageTitle");
        viewModel.transientPage = true;

        // start detection afresh
        WizardUtil.clearWizardCookieItems(webAbstraction, wizardContext.getInputs().getUsingAccessGateway());

        return false;
    }

}
