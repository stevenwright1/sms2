/*
 * Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.clientdetect.controllers;

import java.io.IOException;

import com.citrix.wi.clientdetect.WizardConstants;
import com.citrix.wi.clientdetect.WizardContext;
import com.citrix.wi.clientdetect.WizardUtil;
import com.citrix.wi.clientdetect.models.WizardViewModel;
import com.citrix.wi.clientdetect.type.ClientType;
import com.citrix.wi.clientdetect.util.WizardBrowserTitleBuilder;

/**
 * This controls the enable activeX page
 */
public class EnableActiveXController extends Controller {

    private WizardViewModel viewModel = new WizardViewModel();

    public EnableActiveXController(WizardContext wizardContext) {
        super(wizardContext);
        webAbstraction.setRequestContextAttribute(WizardConstants.VIEW_MODEL, viewModel);
    }

    public boolean perform() throws IOException {

        String clientName = ClientType.getClientNameFromCurrentStep(wizardContext.getModel().getCurrentStep());
        viewModel.pageTitle = WizardBrowserTitleBuilder.createTitle(wizardContext, clientName
                        + "EnableActiveXPageTitle");

        WizardUtil.setupCommonWizardLinks(wizardContext, viewModel);

        return true;
    }

}
