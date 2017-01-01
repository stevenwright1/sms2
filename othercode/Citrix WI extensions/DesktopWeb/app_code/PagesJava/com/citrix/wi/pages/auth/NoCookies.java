/*
 * Copyright (c) 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth;

import java.io.IOException;

import com.citrix.wi.pages.StandardPage;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wing.MessageType;
import com.citrix.wing.util.Strings;
import com.citrix.wi.mvc.WIContext;

/**
 * Class handles situation where the user browser cannot store cookies.
 */
public class NoCookies extends StandardPage {

    private final static String MSGKEY_UNDERSORE_IN_DOMAIN_NAME = "UnderscoreInDomainName";
    
    public NoCookies(WIContext wiContext) {
        super(wiContext);
    }

    protected boolean performImp() throws IOException {
        boolean showForm = true;

        // If the host name contains an underscore, IE fails to set cookies.
        if (serverHostnameContainsUnderscore() && wiContext.getClientInfo().isIE()) {
            wiContext.log(MessageType.WARNING, MSGKEY_UNDERSORE_IN_DOMAIN_NAME);
            wiContext.getWebAbstraction().clientRedirectToUrl(Constants.PAGE_SERVER_ERROR);
            showForm = false;
        }

        return showForm;
    }

    private boolean serverHostnameContainsUnderscore() {
        String url = wiContext.getWebAbstraction().getRequestURL().toLowerCase();

        // Strip "http[s]://".
        if (url.startsWith("http")) {
            url = url.substring(url.indexOf("//") + 2);
        }

        String hostname = url.substring(0, url.indexOf("/"));

        boolean result = Strings.contains(hostname, "_");
        return result;
    }
}
