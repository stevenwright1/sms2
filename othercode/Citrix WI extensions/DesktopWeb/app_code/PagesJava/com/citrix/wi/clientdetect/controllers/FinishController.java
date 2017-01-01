/*
 * Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.clientdetect.controllers;

import java.io.IOException;

import com.citrix.wi.clientdetect.Client;
import com.citrix.wi.clientdetect.Mode;
import com.citrix.wi.clientdetect.WizardConstants;
import com.citrix.wi.clientdetect.WizardContext;
import com.citrix.wi.clientdetect.WizardUtil;
import com.citrix.wi.clientdetect.models.FinishViewModel;
import com.citrix.wi.clientdetect.models.WizardInput;
import com.citrix.wi.clientdetect.models.WizardModel;
import com.citrix.wi.clientdetect.models.WizardOutput;
import com.citrix.wi.clientdetect.type.ClientType;
import com.citrix.wi.clientdetect.type.IcoStatus;
import com.citrix.wi.clientdetect.util.WizardBrowserTitleBuilder;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.mvc.WebCookie;

/**
 * This controls the finish page
 */
public class FinishController extends Controller {

    private FinishViewModel model = new FinishViewModel();

    public FinishController(WizardContext wizardContext) {
        super(wizardContext);
        webAbstraction.setRequestContextAttribute(WizardConstants.VIEW_MODEL, model);
    }

    public boolean perform() throws IOException {
        model.pageTitle = WizardBrowserTitleBuilder.createTitle(wizardContext, "FinishPageTitle");
        model.transientPage = true;

        String reloaded = (String)wizardContext.getModel().getAttribute(WizardConstants.RELOADED);
        if (reloaded != WizardConstants.VAL_TRUE) {
            // detect the ico status, set the appropriate cookies, then reload
            // the page
            // we also get rid of the infobar in IE by reloading
            wizardContext.getModel().setAttribute(WizardConstants.RELOADED, WizardConstants.VAL_TRUE);
            model.reloadingPage = true;
        } else {
            wizardContext.getModel().setAttribute(WizardConstants.RELOADED, WizardConstants.VAL_FALSE);
            model.reloadingPage = false;
        }

        if (!finishLogicInc()) {
            return false;
        }

        populatePageModel();

        return false;
    }

    private void populatePageModel() {
        model.redirectURL = wizardContext.getInputs().getRedirectUrl();

        WizardOutput output = wizardContext.getOutput();

        Client remoteClientDetected = output.getRemoteClientDetected();
        model.remoteClientResult = remoteClientDetected != null ? remoteClientDetected.getClientAsString() : "";

        Client streamingClientDetected = output.getStreamingClientDetected();
        model.streamingClientResult = streamingClientDetected != null ? streamingClientDetected.getClientAsString()
                        : "";

        IcoStatus icoStatusResult = output.getIcoStatus();
        model.icoStatusResult = icoStatusResult != null ? icoStatusResult.getIcoStatusAsString() : "";

        model.showAlternateResult = output.getAlternateResult() != null;
        model.alternateResult = output.getAlternateResult();

        model.showRDPClassID = remoteClientDetected != null && remoteClientDetected.getClientType() == ClientType.RDP;
        model.RDPClassID = output.getRdpClientClassId();
    }

    private boolean finishLogicInc() {
        // Quit processing if GET url does not contain CSRF token
        if (!WizardUtil.validateCsrfQueryStr(wizardContext.getWebAbstraction())) {
            return false;
        }

        String reload = (String)wizardContext.getModel().getAttribute(WizardConstants.RELOADED);

        if (reload != WizardConstants.VAL_TRUE) {
            WebAbstraction webAbstraction = wizardContext.getWebAbstraction();
            WizardInput inputs = wizardContext.getInputs();
            WizardModel model = wizardContext.getModel();
            model.populateModel(inputs);
            WizardOutput output = wizardContext.getOutput();
            String logout = (String)webAbstraction.getQueryStringParameter(WizardConstants.LOGOUT);
            String skipped = (String)webAbstraction.getQueryStringParameter(WizardConstants.SKIPPED);
            if (logout != null && WizardUtil.isEquals(logout, WizardConstants.VAL_TRUE)) {
                output.setAlternateResult(WizardConstants.LOGOUT);
            } else if (skipped != null && WizardUtil.isEquals(skipped, WizardConstants.VAL_TRUE)) {
                output.setAlternateResult(WizardConstants.SKIPPED);
            } else if (output.getAlternateResult() == null) {
                if (inputs.getMode() == Mode.ADVANCED) {
                    String submitMode = (String)webAbstraction.getQueryStringParameter(WizardConstants.ID_SUBMIT_MODE);
                    if (submitMode != null && !WizardUtil.isEquals(submitMode, WizardConstants.VAL_OK)) {
                        // return skipped, but don't include a client result, it
                        // will get ignored
                        output.setAlternateResult(WizardConstants.SKIPPED);
                    } else {
                        String cookies = (String)webAbstraction.getRequestHeader("Cookie");
                        model.updateClientsResult(cookies);
                        String preferredClient = (String)webAbstraction
                                        .getQueryStringParameter(WizardConstants.ID_DEFAULT_CLIENT);

                        if (preferredClient != null && WizardUtil.isEquals(preferredClient, WizardConstants.AUTO)) {
                            // pick the first client
                            model.setUserPreferredClient(null);
                            if (inputs.getRemoteClients().size() > 0) {
                                ClientType preferred = (ClientType)inputs.getRemoteClients().getFirst();
                                if (model.isClientAvailable(preferred)) {
                                    model.setUserPreferredClient(new Client(preferred));
                                } else if (ClientType.NATIVE.equals(preferred) && inputs.isJavaFallback()) {
                                    // if we have Java fallback, try java
                                    if (model.isClientAvailable(ClientType.JAVA)) {
                                        model.setUserPreferredClient(new Client(ClientType.JAVA));
                                    }
                                }
                            }

                            // set the client as auto detected
                            Client userClient = model.getUserPreferredClient();
                            if (userClient != null) {
                               userClient.setAutoDetected(true);
                            }
                        } else {
                            Client userPreferred = new Client(ClientType.getClientTypeFromString(preferredClient));
                            model.setUserPreferredClient(userPreferred);
                        }
                        output.update(model, inputs);
                    }
                } else {
                    String cookies = (String)webAbstraction.getRequestHeader("Cookie");
                    model.updateClientsResult(cookies);
                    output.update(model, inputs);
                }
            }

            // clear the rade cookie
            if (inputs.getMode() != Mode.SILENT
                            || (inputs.getMode() == Mode.SILENT && output.getStreamingClientDetected() != null)) {
                WebCookie persistentCookie = webAbstraction.getCookie(WizardConstants.COOKIE_WI_USER);
                WizardUtil.removeWizardCookie(webAbstraction, persistentCookie, WizardConstants.COOKIE_RADE_STARTED);
                webAbstraction.addCookie(persistentCookie, true);
            }
        }

        return true; // keep going
    }

}
