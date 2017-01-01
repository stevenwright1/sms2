/*
 * Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.clientdetect.controllers;

import java.io.IOException;

import com.citrix.wi.clientdetect.Mode;
import com.citrix.wi.clientdetect.WizardConstants;
import com.citrix.wi.clientdetect.WizardContext;
import com.citrix.wi.clientdetect.WizardUtil;
import com.citrix.wi.clientdetect.models.ViewModel;
import com.citrix.wi.clientdetect.models.WizardInput;
import com.citrix.wi.clientdetect.models.WizardModel;
import com.citrix.wi.clientdetect.models.WizardOutput;
import com.citrix.wi.clientdetect.util.WizardBrowserTitleBuilder;
import com.citrix.wi.config.ConfigurationProvider;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.util.ClientInfo;

/**
 * This controls the start page
 */
public class StartPageController extends Controller {
    private ViewModel viewModel = new ViewModel();

    public StartPageController(WizardContext wizardContext) {
        super(wizardContext);
        webAbstraction.setRequestContextAttribute(WizardConstants.VIEW_MODEL, viewModel);
    }

    public boolean perform() throws IOException {
        viewModel.pageTitle = WizardBrowserTitleBuilder.createTitle(wizardContext, "StartPageTitle");

        if (!initalizeStartPage()) {
            return false;
        }

        return true;
    }

    private boolean initalizeStartPage() {
        WizardInput inputs = wizardContext.getInputs();
        WizardModel model = wizardContext.getModel();
        WizardOutput output = wizardContext.getOutput();
        WebAbstraction abstraction = wizardContext.getWebAbstraction();

        ClientInfo sClientInfo = wizardContext.getClientInfo();
        ConfigurationProvider configProvider = (ConfigurationProvider)abstraction
                        .getApplicationAttribute(WizardConstants.APP_ATTRIBUTE_CONFIGURATION_PROVIDER);
        if (configProvider != null) {
            configProvider.setSiteInfo(abstraction.getBaseURL(), abstraction.getApplicationPath());
        }

        if (!WizardUtil.isWizardSupportedOS(sClientInfo)) {
            output.setAlternateResult(WizardUtil.getError(WizardConstants.UNSUPPORTED_OS));
        }

        String redirectUrl = null;

        String error = model.getError();
        if (output.getAlternateResult() != null && error == null) {
            redirectUrl = WizardUtil.getUrlWithQueryStrWithCsrf(wizardContext, WizardConstants.PAGE_FINISH);
        }
        // if rade is restarted due to machine reboot then start from enable
        // Active X page if IE
        else if (wizardContext.getInputs().detectStreamingClient()
                        && WizardUtil.isRadeInstallationIncomplete(abstraction, inputs, sClientInfo)) {
            model.setCurrentStep(WizardConstants.PAGE_RADE);
            redirectUrl = WizardConstants.PAGE_ENABLE_ACTIVEX;
        } else {
            Mode mode = wizardContext.getInputs().getMode();

            boolean showUI = error != null
                            || (mode == Mode.AUTO && (wizardContext.getInputs().getShowWelcomeScreen() || !WizardUtil
                                            .isSupportedBrowser(wizardContext.getClientInfo().getBrowser())));
            if (!showUI) {
                if (mode == Mode.ADVANCED) {
                    redirectUrl = WizardConstants.PAGE_ADVANCED_DETECT;
                } else {
                    redirectUrl = wizardContext.getModel().getNextStepWithCsrf(wizardContext);
                }
            }
        }

        if (redirectUrl != null) {
            wizardContext.getWebAbstraction().clientRedirectToUrl(redirectUrl);
            return false; // finish processing and show no UI
        }
        return true;
    }

}
