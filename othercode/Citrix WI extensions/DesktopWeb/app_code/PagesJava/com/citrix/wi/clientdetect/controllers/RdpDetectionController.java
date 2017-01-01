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
 * This controls the RdpDetection page
 */
public class RdpDetectionController extends Controller {
    private ViewModel model = new ViewModel();

    public RdpDetectionController(WizardContext wizardContext) {
        super(wizardContext);
        webAbstraction.setRequestContextAttribute(WizardConstants.VIEW_MODEL, model);
    }

    public boolean perform() throws IOException {
        model.pageTitle = WizardBrowserTitleBuilder.createTitle(wizardContext, "RadeClientDetectionPageTitle");

        WizardModel model = wizardContext.getModel();
        model.setCurrentStep(WizardConstants.PAGE_RDP);

        return true;
    }

}
