/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.clientdetect.actions;

import java.util.Collection;
import java.util.Locale;

import com.citrix.wi.clientdetect.Client;
import com.citrix.wi.clientdetect.WizardConstants;
import com.citrix.wi.clientdetect.WizardContext;
import com.citrix.wi.clientdetect.WizardUtil;
import com.citrix.wi.clientdetect.models.WizardInput;
import com.citrix.wi.clientdetect.models.WizardModel;
import com.citrix.wi.clientdetect.models.WizardOutput;
import com.citrix.wi.clientdetect.type.ClientType;
import com.citrix.wi.clientdetect.util.MultiLanguage;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.types.ProductType;
import com.citrix.wing.util.HttpURLChecker;
import com.citrix.wing.util.Locales;
import com.citrix.wing.util.WebUtilities;

/**
 * This specifies the action of initializing the Client Detection Wizard.
 * This needs to be performed before the client wizard pages can be used.
 */
public class InitializeWizard {
    public InitializeWizard() { }

    /**
     * Performs the initialization of the wizard models.
     * This method initializes WizardModel, WizardInput, WizardOutput models.
     * @param controller Used to get the models and parameters from the web abstraction object
     */
    public void perform(WizardContext wizardContext) {
        WizardModel model = wizardContext.getModel();
        WizardInput inputs = wizardContext.getInputs();
        WizardOutput output = wizardContext.getOutput();

        //Initialize the models
        model.initialize();
        inputs.initialize();
        output.initialize();

        WebAbstraction webAbstraction = wizardContext.getWebAbstraction();

        // Input parameters to the client wizard
        // Use getRequestParameter rather than getFormParameter because
        // WISP uses query string parameters rather than form data to access this page
        String redirectUrl = webAbstraction.getRequestParameter(WizardConstants.PARAMETER_REDIRECT_URL);
        String csrfToken = webAbstraction.getRequestParameter(WizardConstants.PARAMETER_CSRF_TOKEN);
        String mode = webAbstraction.getRequestParameter(WizardConstants.PARAMETER_MODE);
        String remoteClients = webAbstraction.getRequestParameter(WizardConstants.PARAMETER_REMOTE_CLIENTS);
        String preferredClient = webAbstraction.getRequestParameter(WizardConstants.PARAMETER_PREFERRED_CLIENT);
        String streamingClients = webAbstraction.getRequestParameter(WizardConstants.PARAMETER_STREAMING_CLIENTS);
        String localeStr = webAbstraction.getRequestParameter(WizardConstants.PARAMETER_LOCALE);
        String allowLogout = webAbstraction.getRequestParameter(WizardConstants.PARAMETER_ALLOW_LOGOUT);
        String masterPage = webAbstraction.getRequestParameter(WizardConstants.PARAMETER_MASTER_PAGE);
        String showWelcomeScreen = webAbstraction.getRequestParameter(WizardConstants.PARAMETER_SHOW_WELCOME_SCREEN);
        String showZonePage = webAbstraction.getRequestParameter(WizardConstants.PARAMETER_SHOW_ZONE_PAGE);
        String showUpgradePage = webAbstraction.getRequestParameter(WizardConstants.PARAMETER_SHOW_UPGRADE_PAGE);
        String usingAccessGateway = webAbstraction.getRequestParameter(WizardConstants.PARAMETER_ACCESS_GATEWAY_PROXY);
        String productTypeStr = webAbstraction.getRequestParameter(WizardConstants.PARAMETER_PRODUCT_TYPE);
        String javaFallbackStr = webAbstraction.getRequestParameter(WizardConstants.PARAMETER_JAVA_FALLBACK);

        if (csrfToken == null ||
            !WizardUtil.initialiseCsrfToken(webAbstraction, csrfToken)) {
            model.setError(WizardConstants.INVALID_REDIRECT_URL);
        }

        if (redirectUrl == null) {
            redirectUrl = webAbstraction.getConfigurationAttribute(WizardConstants.WIZARD_DEFAULT_REDIRECT_URL);
        } else if (!WizardUtil.isValidRedirectUrl(webAbstraction, redirectUrl)) {
            model.setError(WizardConstants.INVALID_REDIRECT_URL);
        }

        if (!HttpURLChecker.isWellFormed(redirectUrl) ||
            !WebUtilities.getLastElementOfURL(redirectUrl).endsWith(WizardConstants.PAGE_EXTENSION)) {
            model.setError(WizardConstants.INVALID_REDIRECT_URL);
        }

        inputs.initialize(redirectUrl, mode);
        Locale locale = null;
        MultiLanguage mui = (MultiLanguage)webAbstraction.getApplicationAttribute(WizardConstants.APP_ATTRIBUTE_MULTI_LANGUAGE);
        // Try to get the locale from the input parameter
        if (localeStr != null) {
            locale = Locales.fromString(localeStr, '_');
            if (locale != null) {
                locale = mui.getLanguageManager().getBestMatch(locale);
            }
        }

        // If the locale was not passed in to the wizard or the language manager could not
        // find the user locale then try to get the locale from the user agent string
        if (locale == null) {
            Locale[] locales = webAbstraction.getLocalesFromRequest();
            locale = mui.getLanguageManager().getBestMatch(locales);
        }

        // If still does not have any valid locale then use the default locale
        if (locale == null) {
            locale = mui.getLanguageManager().getDefaultLocale();
        }
        inputs.setlocale(locale);

        Collection remoteClientsCollection = null;
        Collection streamingClientsCollection = null;
        if (remoteClients != null) {
            remoteClientsCollection = ClientType.getClientTypesFromString(remoteClients);
            if (!remoteClientsCollection.isEmpty()) {
                inputs.setRemoteClients(remoteClientsCollection);
            }
        }
        if (streamingClients != null) {
            streamingClientsCollection = ClientType.getClientTypesFromString(streamingClients);
            inputs.setStreamingClient(streamingClientsCollection.size() >= 1);
        }
        if ((remoteClients == null && streamingClients == null) ||
            (inputs.getRemoteClients().isEmpty() && !inputs.detectStreamingClient())) {
            output.setAlternateResult(WizardUtil.getError(WizardConstants.NO_CLIENT_TO_DETECT));
        } else {
            model.setCurrentStep(WizardConstants.PAGE_START);
            model.populateModel(inputs);
        }

        if (preferredClient != null) {
            inputs.setPreferredClient(Client.getClientFromString(preferredClient));
        }
        if (allowLogout != null) {
            inputs.setAllowLogout(allowLogout.equalsIgnoreCase(WizardConstants.ON));
        }

        if (masterPage != null && WizardUtil.isValidMasterPageUrl(masterPage)) {
            inputs.setMasterPage(masterPage);
        }

        if (showWelcomeScreen != null) {
            inputs.setShowWelcomeScreen(showWelcomeScreen.equalsIgnoreCase(WizardConstants.ON));
        }

        if (showZonePage != null) {
            inputs.setShowZonePage(showZonePage.equalsIgnoreCase(WizardConstants.ON));
        }

        if (showUpgradePage != null) {
            inputs.setShowUpgradePage(showUpgradePage.equalsIgnoreCase(WizardConstants.ON));
        }

        if (usingAccessGateway != null) {
            inputs.setUsingAccessGateway(usingAccessGateway.equalsIgnoreCase(WizardConstants.ON));
        }

        // fromString() returns null if input is null or does not match a valid type
        ProductType productType = ProductType.fromString(productTypeStr);
        if (productType != null) {
            inputs.setProductType(productType);
        }

        if (javaFallbackStr != null) {
            inputs.setJavaFallback(javaFallbackStr.equalsIgnoreCase(WizardConstants.ON));
        }

        // Clear out any wizard cookie items that may exist from earlier runs of the wizard
        WizardUtil.clearWizardCookieItems(webAbstraction, inputs.getUsingAccessGateway());
    }
}
