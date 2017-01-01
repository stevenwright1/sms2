/*
 * Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.clientdetect.controllers;

import java.io.IOException;

import com.citrix.wi.clientdetect.WizardConstants;
import com.citrix.wi.clientdetect.WizardContext;
import com.citrix.wi.clientdetect.models.ViewModel;
import com.citrix.wi.clientdetect.models.WizardModel;
import com.citrix.wi.clientdetect.util.WizardBrowserTitleBuilder;

/**
 * This controls the radeDetection page
 */
public class RadeDetectionController extends Controller {
    private ViewModel viewModel = new ViewModel();

    public RadeDetectionController(WizardContext wizardContext) {
        super(wizardContext);
        webAbstraction.setRequestContextAttribute(WizardConstants.VIEW_MODEL, viewModel);
    }

    public boolean perform() throws IOException {
        viewModel.pageTitle = WizardBrowserTitleBuilder.createTitle(wizardContext, "RadeClientDetectionPageTitle");

        WizardModel model = wizardContext.getModel();
        model.setCurrentStep(WizardConstants.PAGE_RADE);

        return true;
    }

}
