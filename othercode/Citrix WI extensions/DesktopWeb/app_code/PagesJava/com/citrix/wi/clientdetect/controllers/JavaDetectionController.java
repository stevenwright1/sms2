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
 * This controls the javaDetection page
 */
public class JavaDetectionController extends Controller {
    private ViewModel viewModel = new ViewModel();

    public JavaDetectionController(WizardContext wizardContext) {
        super(wizardContext);
        webAbstraction.setRequestContextAttribute(WizardConstants.VIEW_MODEL, viewModel);
    }

    public boolean perform() throws IOException {
        viewModel.pageTitle = WizardBrowserTitleBuilder.createTitle(wizardContext, "JavaClientDetectionPageTitle");

        WizardModel model = wizardContext.getModel();
        model.setCurrentStep(WizardConstants.PAGE_JAVA);

        return true;
    }

}
