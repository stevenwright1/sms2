/*
 * Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.clientdetect.controllers;

import java.io.IOException;

import com.citrix.wi.clientdetect.WizardConstants;
import com.citrix.wi.clientdetect.WizardContext;
import com.citrix.wi.clientdetect.WizardUtil;
import com.citrix.wi.clientdetect.models.RadeDownloadedViewModel;
import com.citrix.wi.clientdetect.util.WizardBrowserTitleBuilder;
import com.citrix.wi.mvc.WebCookie;

/**
 * This controls the radeDownloaded controler
 */
public class RadeDownloadedController extends Controller {
    private RadeDownloadedViewModel viewModel = new RadeDownloadedViewModel();

    public RadeDownloadedController(WizardContext wizardContext) {
        super(wizardContext);
        webAbstraction.setRequestContextAttribute(WizardConstants.VIEW_MODEL, viewModel);
    }

    public boolean perform() throws IOException {
        // add the rade cookie in case user restarts the machine as part of
        // installation
        WebCookie persistentCookie = wizardContext.getWebAbstraction().getCookie(WizardConstants.COOKIE_WI_USER);
        WizardUtil.setWizardCookie(wizardContext.getWebAbstraction(), persistentCookie,
                        WizardConstants.COOKIE_RADE_STARTED, WizardConstants.VAL_TRUE);
        wizardContext.getWebAbstraction().addCookie(persistentCookie, true);

        boolean upgrade = WizardConstants.VAL_TRUE.equals(webAbstraction.getQueryStringParameter(WizardConstants.QSTR_UPGRADE));
        viewModel.skipWizardTextKey = upgrade ? "UpgradeLater" : "SkipToLoginText";
        viewModel.skipWizardTooltipKey = upgrade ? "UpgradeLaterButtonTooltip" : "SkipToLoginTooltip";

        viewModel.pageTitle = WizardBrowserTitleBuilder.createTitle(wizardContext, "RadeClientDownloadedPageTitle");

        if (wizardContext.getClientInfo().isIE()) {
            viewModel.nextPage = "location.href='" + WizardConstants.PAGE_ENABLE_ACTIVEX + "'";
        } else if (WizardUtil.isClientDetectablePlatform(wizardContext.getClientInfo())
                        && !wizardContext.getClientInfo().isIE()) {
            viewModel.nextPage = "location.href='" + WizardConstants.PAGE_RADE + "'";
        }

        viewModel.showLogoutLink = wizardContext.getInputs().allowLogout();

        return true;
    }
}
