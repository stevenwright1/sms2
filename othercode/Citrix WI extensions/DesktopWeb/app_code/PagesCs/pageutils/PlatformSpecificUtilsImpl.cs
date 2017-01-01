// Copyright (c) 2006 - 2010 Citrix Systems, Inc. All Rights Reserved.

using com.citrix.authentication.tokens;
using com.citrix.authentication.web;
using com.citrix.authenticators;
using com.citrix.wi;
using com.citrix.wi.accountselfservice;
using com.citrix.wi.accountselfservice.aspnet;
using com.citrix.wi.config;
using com.citrix.wi.config.auth;
using com.citrix.wi.config.client;
using com.citrix.wi.types;
using com.citrix.wi.mvc;
using com.citrix.wi.util;
using com.citrix.wing;
using com.citrix.wing.types;
using com.citrix.wing.util;
using com.citrix.wi.authservice;
using com.citrix.wi.authservice.aspnetproxy;
using com.citrix.wi.mvc.asp;

using java.util;

using System;
using System.Web;
using System.Web.SessionState;
using System.Reflection;

using Citrix.Platform.Authentication.InboundSingleSignOn;

namespace com.citrix.wi.pageutils
{
    public class PlatformSpecificUtilsImpl : PlatformSpecificUtils
    {
        public PlatformSpecificUtilsImpl() { }

        public IPasswordCachingSecurIDAuthenticator getNewPasswordCachingSecurIDAuthenticator()
        {
            return new PasswordCachingSecurIDAuthenticator();
        }

        public ISafewordAuthenticator getNewSafewordAuthenticator()
        {
            return new SafewordAuthenticator();
        }

        public ISecurIDAuthenticator getNewSecurIDAuthenticator()
        {
            return new SecurIDAuthenticator();
        }

        public bool checkForSessionFixation(WIContext wiContext)
        {
            HttpContext context = ((AspWebAbstraction)wiContext.getWebAbstraction()).Context;

            bool isFixationFound = false;

            string sessionAuthId = AccountSelfService.getRecordedAuthId(wiContext);
            string cookieAuthId = AuthUtilities.readAuthCookie(context.Request, AccountSelfService.COOKIE_ACCOUNT_SS_AUTH);

            if (Strings.isEmpty(sessionAuthId) || sessionAuthId != cookieAuthId)
            {
                string appPath = AuthUtilities.getNormalizedAppPath(context.Request);

                context.Response.Clear();
                context.Response.Redirect(appPath + context.Application["AUTH:SESSION_ERROR_URL"]);
                isFixationFound = true;
            }
            return isFixationFound;
        }

        public void expireAuthCookie(WIContext wiContext, string authCookieName)
        {
            HttpContext context = (wiContext.getWebAbstraction() as AspWebAbstraction).Context;
            AuthUtilities.expireAuthCookie(context.Request, context.Response, authCookieName);
        }

        public ContextFactory getAccountSSContextFactory()
        {
            return new ASPNetContextFactory();
        }

        public string addAuthCookie(WIContext wiContext, string authCookieName)
        {
            HttpContext context = (wiContext.getWebAbstraction() as AspWebAbstraction).Context;
            return AuthUtilities.addAuthCookie(context.Request, context.Response, authCookieName);
        }

        public void storeAGLogoutTicket(WIContext wiContext, string logoutTicket)
        {
            Map logoutMap = AGEUtilities.getAGSessionMap(wiContext);

            AspWebAbstraction web = wiContext.getWebAbstraction() as AspWebAbstraction;
            logoutMap.put(logoutTicket, new WeakReference(web.Context.Session));
        }

        public void abandonSession(WIContext wiContext, String logoutTicket)
        {
            Map logoutMap = AGEUtilities.getAGSessionMap(wiContext);

            if (logoutTicket != null && logoutTicket.Length > 0 && logoutMap != null)
            {
                // Find the session associated with the logout ticket we were given.
                // This is the session that we want to invalidate.
                // Note that this is not the same as the current request's session, because
                // AG requests the logout page in a different context.
                WeakReference wr = logoutMap.remove(logoutTicket) as WeakReference;

                if (wr != null)
                {
                    HttpSessionState mappedSession = wr.Target as HttpSessionState;
                    if (mappedSession != null)
                    {
                        // We cannot abandon the session here because ASP.NET does not allow
                        // you to call the Abandon() method on arbitrary sessions.
                        // However, we can achieve a similar effect by clearing the session's
                        // contents. This will cause the WI authentication filter to reject
                        // the session as being unauthenticated.
                        mappedSession.Clear();
                    }
                }
            }
        }

        public StatusMessage doFederatedLogout(WIContext wiContext)
        {
            StatusMessage errorMessage = null;
            AuthenticationConfiguration authConfig = wiContext.getConfiguration().getAuthenticationConfiguration();
            AuthPoint authPoint = authConfig.getAuthPoint();
            if (authPoint != null && (authPoint is ADFSAuthPoint) && 
                        ((ADFSAuthPoint)authPoint).isGlobalLogoutEnabled())
            {
                try
                {
                    AspWebAbstraction abstraction = (AspWebAbstraction)wiContext.getWebAbstraction();
                    HttpContext context = abstraction.Context;
                    ISingleSignOnIdentity identity = SingleSignOnIdentity.GetIdentity(context);
                    if (identity.SupportsSignOut)
                    {
                        identity.SignOut();
                    }
                    else
                    {
                        errorMessage = new StatusMessage(MessageType.ERROR, "LogoffFederatedServicesError");
                    }
                }
                catch
                {
                    errorMessage = new StatusMessage(MessageType.ERROR, "LogoffFederatedServicesError");
                }
            }
            return errorMessage;
        }

        public ASClient getASClient(string url, StaticEnvironmentAdaptor envAdaptor)
        {
            // ASP.NET implementation of Authentication Service client does not
            // currently use StaticEnvironmentAdaptor
            return new ASPNetASClient(url);
        }
    }
}