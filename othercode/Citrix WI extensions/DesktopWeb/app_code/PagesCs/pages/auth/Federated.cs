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
using com.citrix.wi.mvc.asp;
using com.citrix.wi.ui;
using com.citrix.wi.util;
using com.citrix.wing;
using com.citrix.wing.types;
using com.citrix.wing.util;
using com.citrix.wing.webpn;

using Citrix.Platform.Authentication.InboundSingleSignOn;
using java.lang;
using java.util;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Web;
using System.Reflection;


namespace com.citrix.wi.pages.auth
{
    public class Federated : StandardLayout
    {
        public Federated(WIContext wiContext)
            : base(wiContext)
        {
        }

        public override bool performImp()
        {
            AuthenticationConfiguration authConfig = wiContext.getConfiguration().getAuthenticationConfiguration();
            if (!authConfig.getAuthPoint().hasIdentity())
            {
                UIUtils.HandleLoginFailedMessage(wiContext, MessageType.ERROR, "NoFederatedLogin");
                return false;
            }

            AspWebAbstraction abstraction = (AspWebAbstraction)wiContext.getWebAbstraction();
            HttpContext context = abstraction.Context;

            ISingleSignOnIdentity singleSignOnIdentity =
                SingleSignOnIdentity.GetIdentity(context);

            WindowsIdentity windowsIdentity = singleSignOnIdentity.WindowsIdentity;

            if (windowsIdentity == null ||
                windowsIdentity.IsGuest || !windowsIdentity.IsAuthenticated)
            {
                // Most likely cause for this is if the IIS MetaBase is not set correctly.
                // To prevent the possibility of unauthenticated access, explicitly
                // reject access here.
                UIUtils.HandleLoginFailedMessage(wiContext, MessageType.ERROR, "GeneralAuthenticationError", "", "ADFSUnauthenticatedUser", null);
                return false;
            }

            // Creates an object for pre authenticated user.
            WindowsToken windowsToken = new WindowsToken(windowsIdentity, true);

            // If the roaming user feature is enabled, the access token needs to be populated with the SIDs of the AD groups
            // the user belongs to.
            StatusMessage statusMessage = DisasterRecoveryUtils.fillInUserIdentityInfoIfNeeded(wiContext, windowsToken);
            if (statusMessage != null)
            {
                UIUtils.HandleLoginFailedMessage(wiContext, statusMessage);
                return false;
            }

            // mark this session as authenticated
            Authentication.getAuthenticationState(wiContext.getWebAbstraction()).setAuthenticated(windowsToken);

            // Create a UserContext to hold user-specific app state; this
            // is null if the creation failed, which indicates some horrible
            // problem has arisen somewhere.
            UserContext userContext = SessionUtils.createNewUserContext(wiContext);
            if (userContext == null)
            {
                UIUtils.HandleLoginFailedMessage(wiContext, MessageType.ERROR, "GeneralAuthenticationError");
                return false;
            }

            LaunchUtilities.transferLaunchDataToSession(wiContext);
            Authentication.redirectToCurrentAuthPage(wiContext);

            return false;
        }

        override public System.String getBrowserPageTitleKey() {
            // return null so only site name is used.
            // no UI associated with this class.
            return null;
        }
    }
}
