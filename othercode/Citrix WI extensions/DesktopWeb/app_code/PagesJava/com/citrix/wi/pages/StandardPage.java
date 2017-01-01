/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages;

import java.io.IOException;

import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pageutils.PageHistory;

/**
 * This forms the base class for any pages which have no UI.
 */
public abstract class StandardPage {

    protected WIContext wiContext;

    public StandardPage(WIContext wiContext) {
        this.wiContext = wiContext;
        setResponseCaching();
    }

    /**
     * Sets up the response headers for no caching.
     *
     * Pages which have different caching requirements should override this method.
     */
    protected void setResponseCaching() {
        wiContext.getWebAbstraction().setNoCaching();
    }

    protected WebAbstraction getWebAbstraction()
    {
        return wiContext.getWebAbstraction();
    }

    /**
     * Record the current page's URL where it can be accessed after a refresh
     * to determine with which page to update the frameset.
     */
    protected void recordCurrentPageURL()
    {
        PageHistory.recordCurrentPageURL(wiContext.getWebAbstraction());
    }

    /**
     * Specify the first page to present to the user after login is complete.
     */
    protected void setInitialPostLoginPage(String pageURL) {
        PageHistory.setInitialPostLoginPage(wiContext.getWebAbstraction(), pageURL);
    }

    /**
     * Run the logic of the page, primarily as implemented by performImp.
     */
    public final boolean perform() throws IOException {
        boolean incomplete = performGuard();
        if (incomplete) {
            incomplete = performImp();
        }
        return incomplete;
    }

    /**
     * The per-page logic.
     */
    protected abstract boolean performImp() throws IOException;

    /**
     * Perform any common guard action on the page.
     */
    protected boolean performGuard() throws IOException {
        return true;
    }
}
