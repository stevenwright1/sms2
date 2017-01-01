/*
 * Copyright (c) 2005 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */
package com.citrix.wi.clientdetect;

import com.citrix.wi.clientdetect.models.WizardInput;
import com.citrix.wi.clientdetect.models.WizardModel;
import com.citrix.wi.clientdetect.models.WizardOutput;
import com.citrix.wi.clientdetect.util.MultiLanguage;
import com.citrix.wi.clientdetect.util.WizardHelp;
import com.citrix.wi.localization.ClientManager;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.util.ClientInfo;

/**
 * Holds useful items needed for processing a wizard page.
 */
public class WizardContext {

    WizardModel   model         = null;
    WizardInput   inputs        = null;
    WizardOutput  output        = null;
    MultiLanguage mui           = null;
    ClientInfo    sClientInfo   = null;
    ClientManager clientManager = null;
    WebAbstraction webAbstraction = null;

    /**
     * Creates a wizard context.
     *
     * @param webAbstraction the web abstraction
     */

    public WizardContext(WebAbstraction webAbstraction) {
        if (webAbstraction == null) throw new IllegalArgumentException("Can't create a wizard context with a null web abstraction");
        this.webAbstraction = webAbstraction;
        model = (WizardModel)webAbstraction.getSessionAttribute("WizardModel");
        if (model == null) {
            model = new WizardModel();
            webAbstraction.setSessionAttribute("WizardModel", model);
        }
        inputs = (WizardInput)webAbstraction.getSessionAttribute("WizardInput");
        if (inputs == null) {
            inputs = new WizardInput();
            webAbstraction.setSessionAttribute("WizardInput", inputs);
        }
        output = (WizardOutput)webAbstraction.getSessionAttribute("WizardOutput");
        if (output == null) {
            output = new WizardOutput();
            webAbstraction.setSessionAttribute("WizardOutput", output);
        }

        model.populateModel(inputs);
        mui = (MultiLanguage)webAbstraction.getApplicationAttribute(WizardConstants.APP_ATTRIBUTE_MULTI_LANGUAGE);
        clientManager = (ClientManager)webAbstraction.getApplicationAttribute(WizardConstants.APP_ATTRIBUTE_CLIENT_MANAGER);
        sClientInfo = (ClientInfo)webAbstraction.getSessionAttribute(WizardConstants.SV_CLIENT_INFO);
    }

    /**
     * Gets the localized string corresponding to the given resource bundle key.
     * @param key the resouce bundle key
     * @return the localized string
     */
    public String getString(String key) {
        return mui.getString(key, inputs.getLocale());
    }

    /**
     * Gets the localized string corresponding to the given resource bundle key.
     * @param key the resouce bundle key
     * @param arg parameter to insert into the localized string
     * @return the localized string
     */
    public String getString(String key, String arg) {
        return mui.getString(key, arg, inputs.getLocale());
    }

    /**
     * Gets the localized string corresponding to the given resource bundle key.
     * @param key the resouce bundle key
     * @param args array of parameters to insert into the localized string
     * @return the localized string
     */
    public String getString(String key, Object[] args) {
        return mui.getString(key, args, inputs.getLocale());
    }

    /**
     * Gets the wizard model.
     * @return the model
     */
    public WizardModel getModel() {
        return model;
    }

    /**
     * Gets the wizard input model.
     * @return the inputs
     */
    public WizardInput getInputs() {
        return inputs;
    }

    public WizardOutput getOutput() {
        return output;
    }

    /**
     * Gets the multi-language support object.
     * @return the mui
     */
    public MultiLanguage getMui() {
        return mui;
    }

    /**
     * Gets the client manager.
     * @return the client manager
     */
    public ClientManager getClientManager() {
        return clientManager;
    }

    /**
     * Gets the client info.
     * @return the client info
     */
    public ClientInfo getClientInfo() {
        return sClientInfo;
    }

    /**
     * Gets the web abstraction
     * @return the web abstraction
     */
    public WebAbstraction getWebAbstraction() {
        return webAbstraction;
    }

    /**
     * Gets the help for the give page id for the current locale
     *
     * @param pageID the page to get the help for
     * @return the help for the given page
     */
    public WizardHelp getWizardHelp(String pageID) {
        String key = pageID
                + "_" + getClientInfo().getBrowser()
                + "_" + getClientInfo().getBrowserVersionMajor()
                + "_" + getClientInfo().getPlatform();
        WizardHelp help = getMui().getHelp(key, getInputs().getLocale());
        return help;
    }
}
