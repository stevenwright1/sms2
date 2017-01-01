/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth;

import java.io.IOException;

import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pages.StandardLayout;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.PageLocation;

/**
 * This page is a super class for all the pages
 * that have UI and live in the auth directory.
 * These pages must run the silent detection before they
 * display anything.
 */
public abstract class PreLoginUIPage extends StandardLayout {

    public PreLoginUIPage(WIContext wiContext) {
        super(wiContext);
    }

    public boolean performImp() throws IOException {
        if (Include.isLoggedIn(wiContext.getWebAbstraction())) {
            wiContext.getWebAbstraction().clientRedirectToUrl("../site/" + Constants.PAGE_APPLIST);
            return false;
        }

        if (SilentDetection.isDetectionRequired(wiContext) && !isFeedbackSet()) {
            // we will new run the silent detection wizard
            recordCurrentPageURL();
            // we needed to record the current page url so we come back
            // to this page after the silent detection has completed
            getWebAbstraction().clientRedirectToContextUrl("/" + PageLocation.AUTH + "/" + Constants.PAGE_SILENT_DETECTION);
            return false;
        } else {
            // get on with the rest of the page now...
            return performInternal();
        }
    }

    protected boolean performGuard() throws IOException {
        return true;
    }


    protected abstract boolean performInternal() throws IOException;
}
