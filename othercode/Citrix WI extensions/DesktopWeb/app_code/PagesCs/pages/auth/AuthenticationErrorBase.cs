// Copyright (c) 2004 - 2010 Citrix Systems, Inc. All Rights Reserved.

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
using com.citrix.wi.ui;
using com.citrix.wi.util;
using com.citrix.wing;
using com.citrix.wing.types;
using com.citrix.wing.util;

using System.Web.Configuration;

namespace com.citrix.wi.pages.auth
{
    /// <summary>
    /// Base class for controllers that handle authentication errors
    /// (access denied, certificate errors etc).
    /// </summary>
    public abstract class AuthenticationErrorBase : StandardLayout
    {
        public AuthenticationErrorBase(WIContext wiContext)
            : base(wiContext)
        {
        }

        public override bool performImp()
        {

            // This page no longer returns the HTTP error code to the user.
            // In order to find this out, ASP.NET tracing can be turned on.
            AuthenticationState authState = Authentication.getAuthenticationState(wiContext.getWebAbstraction());

            if (authState != null && !authState.isAuthenticated())
            {
                // The error page has been hit and no one is logged
                // in to the current session.  This should only
                // occurs naturally during certificate login.
                // Redirect to the loggedout page.  Turn silent auth
                // off and destroy the session so that the user can
                // attempt to log in again with a different auth method.

                // Set to null otherwise login.aspx assumes session timeout and re-directs to loggedout page
                Authentication.setUntrustedLogonType(null, wiContext.getUserEnvironmentAdaptor());

                UserEnvironmentAdaptor envAdaptor = wiContext.getUserEnvironmentAdaptor();
                envAdaptor.getClientSessionState().put(Constants.COOKIE_ALLOW_AUTO_LOGIN, Constants.VAL_OFF);
                envAdaptor.commitState();
                envAdaptor.destroy();

                UIUtils.HandleLoginFailedMessage(wiContext, MessageType.ERROR, "GeneralAuthenticationError");
            }
            else
            {
                // The error page has been hit and a valid user is logged
                // in to the current session.  This should never naturally
                // occur under normal WI usage.
                // Redirect the user back to the last page they went to.
                PageHistory.redirectToLastPage(wiContext, "");
            }

            return false;
        }
    }
}