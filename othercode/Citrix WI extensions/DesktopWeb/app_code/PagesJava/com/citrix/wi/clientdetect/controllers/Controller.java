/*
 * Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.clientdetect.controllers;

import java.io.IOException;

import com.citrix.wi.clientdetect.WizardContext;
import com.citrix.wi.mvc.WebAbstraction;

/**
 * Abstract controller used by all the wizard page controllers
 */
abstract class Controller {

    protected WizardContext  wizardContext;
    protected WebAbstraction webAbstraction;

    /**
     * Default Constructor, stops the pages being cached
     * 
     * @param wizardContext
     */
    Controller(WizardContext wizardContext) {
        this.wizardContext = wizardContext;
        this.webAbstraction = wizardContext.getWebAbstraction();
        setResponseCaching();
    }

    /**
     * Sets up the response headers for no caching.
     * 
     * Pages which have different caching requirements should override this
     * method.
     */
    protected void setResponseCaching() {
        wizardContext.getWebAbstraction().setNoCaching();
    }

    /**
     * Run the logic of the page
     */
    public abstract boolean perform() throws IOException;
}
