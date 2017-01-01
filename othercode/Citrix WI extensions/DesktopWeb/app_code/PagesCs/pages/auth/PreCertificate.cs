// Copyright (c) 2008 - 2010 Citrix Systems, Inc. All Rights Reserved.

using com.citrix.authentication.tokens;
using com.citrix.authentication.web;
using com.citrix.wi;
using com.citrix.wi.authservice;
using com.citrix.wi.config;
using com.citrix.wi.config.auth;
using com.citrix.wi.config.client;
using com.citrix.wi.controls;
using com.citrix.wi.pages;
using com.citrix.wi.pageutils;
using com.citrix.wi.types;
using com.citrix.wi.mvc;
using com.citrix.wi.mvc.asp;
using com.citrix.wi.ui;
using com.citrix.wi.util;
using com.citrix.wing;
using com.citrix.wing.types;
using com.citrix.wing.util;

using System.Configuration;

namespace com.citrix.wi.pages.auth
{
    // The sole purpose of this class is to work around CPR 200558
    // This page will be accessed between login.aspx and certificate.aspx page.
    // It will access certificate.aspx and will redirect to it on succes, or show
    // the error otherwise.

    // For more details see preCertificate.js
    public class PreCertificate : StandardLayout
    {
        // the page reuses the layout of the login page
        protected LoginPageControl viewControl = new LoginPageControl();

        public PreCertificate(WIContext wiContext)
            : base(wiContext)
        {
            wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
            layoutControl.layoutMode = Include.getLayoutMode(wiContext);
            // The loginPageControl requires it to be set. Serves no purpose on this page.
            layoutControl.formAction = Constants.FORM_POSTBACK;
        }

        override public bool performImp()
        {
            AuthenticationState authenticationState = Authentication.getAuthenticationState(wiContext.getWebAbstraction());

            // Auth filter allow only the page in front of its queue to be accessed, but there is
            // a situation that accessing any of two pages is possible.
            // Namely, the preCertificate page can be accessed only when certificate page can.
            if (authenticationState.getCurrentPage() != Constants.PAGE_CERTIFICATE)
            {
                // When performImp returns false IIS will trigger the certificate error page
                // which destroys the session, wipes out the auth filter queue,
                // disables auto login for the current browser session and recirects to the login page.
                // This enables user to login using different authentication method (if allowed by site configuration).
                wiContext.getWebAbstraction().clientRedirectToUrl(Constants.PAGE_LOGIN);
                return false;
            }

            layoutControl.isLoginPage = true;
            welcomeControl.setTitle(wiContext.getString("PINRequired"));
            // No message needed. Space triggers empty line to appear, it keeps layout nice.
            welcomeControl.setBody(" ");
            return true;
        }

        override public System.String getBrowserPageTitleKey()
        {
            // return null so only site name is used.
            // no UI associated with this class.
            return null;
        }
    }
}
