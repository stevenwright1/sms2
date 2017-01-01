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

using System.Configuration;
using System.Security.Principal;
using java.lang;
using java.util;

namespace com.citrix.wi.pages.auth {

    public class Integrated : StandardLayout {

        public Integrated(WIContext wiContext) : base(wiContext) {
        }

        public override bool performImp() {
            // Note: CERTIFICATE_SINGLE_SIGN_ON is here because that method is Integrated to WI, then SmartCard to MPS.

            AspWebAbstraction web = (AspWebAbstraction)wiContext.getWebAbstraction();
                
            AuthenticationConfiguration authConfig = wiContext.getConfiguration().getAuthenticationConfiguration();
            WIAuthPoint wiAuthPoint = authConfig.getAuthPoint() as WIAuthPoint;
            if (wiAuthPoint != null)
            {
                if (!wiAuthPoint.isAuthMethodEnabled(WIAuthType.SINGLE_SIGN_ON) &&
                    !wiAuthPoint.isAuthMethodEnabled(WIAuthType.CERTIFICATE_SINGLE_SIGN_ON))
                {
                    UIUtils.HandleLoginFailedMessage(wiContext, MessageType.ERROR, "NoIntegratedLogin");
                    return false;
                }
            }

            WindowsIdentity identity = (WindowsIdentity)web.Context.User.Identity;
            if (identity.IsGuest || !identity.IsAuthenticated) {
                // Most likely cause for this is if the IIS MetaBase is not set correctly.
                // To prevent the possibility of unauthenticated access, explicitly
                // reject access here.
                UIUtils.HandleLoginFailedMessage(wiContext, MessageType.ERROR, "GeneralAuthenticationError", "", "UnauthenticatedUser", null);
                return false;
            }

            // Creates an object for pre authenticated user.
            WindowsToken windowsToken = new WindowsToken(identity);

            AccessTokenValidity validAccessToken = Authentication.validateAccessTokenWithExpiry(wiContext.getConfiguration(), null, windowsToken);
            AccessTokenValidationResult result = validAccessToken.getValidationResult();
            // HandleLoginFailedMessage will destroy built-up session state for failures:
            if (result == AccessTokenValidationResult.FAILED_NOT_LICENSED) {
                UIUtils.HandleLoginFailedMessage(wiContext, MessageType.ERROR, "NotLicensed");
                return false;
            } 
            
            if (result == AccessTokenValidationResult.FAILED) {
                UIUtils.HandleLoginFailedMessage(wiContext, MessageType.ERROR, "InvalidCredentials");
                return false;
            }

            // If the roaming user feature is enabled, the access token needs to be populated with the SIDs of the AD groups
            // the user belongs to.
            StatusMessage statusMessage = DisasterRecoveryUtils.fillInUserIdentityInfoIfNeeded(wiContext, windowsToken);
            if (statusMessage != null)
            {
                UIUtils.HandleLoginFailedMessage(wiContext, statusMessage);
                return false;
            }

            Authentication.getAuthenticationState(web).setAuthenticated(windowsToken);

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
