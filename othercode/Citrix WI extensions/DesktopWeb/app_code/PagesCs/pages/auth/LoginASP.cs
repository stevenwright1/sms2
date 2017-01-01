// Copyright (c) 2004 - 2010 Citrix Systems, Inc. All Rights Reserved.

using com.citrix.authentication.tokens;
using com.citrix.authentication.web;
using com.citrix.wi;
using com.citrix.wi.config;
using com.citrix.wi.config.auth;
using com.citrix.wi.controls;
using com.citrix.wi.pages.auth;
using com.citrix.wi.pageutils;
using com.citrix.wi.types;
using com.citrix.wi.mvc;
using com.citrix.wi.mvc.asp;
using com.citrix.wi.ui;
using com.citrix.wing.util.serviceselector;
using com.citrix.wing;
using com.citrix.wing.util;
using com.citrix.wi.clientdetect;
using com.citrix.wi.util;
using java.util;

using System;
using System.DirectoryServices;

namespace com.citrix.wi.pages.auth
{

    public class LoginASP : Login
    {

        public LoginASP(WIContext wiContext) : base(wiContext) { }

        public override void addAuthPages(AuthenticationState authenticationState, AuthPoint authPoint)
        {
            base.addAuthPages(authenticationState, authPoint);

            // federated gets the identity, if there is one available
            // either from the context or in an ADFS claim
            if (authPoint.hasIdentity())
            {
                authenticationState.addPageToQueueHead("federated", new HashMap());
            }
        }

        public override void doViewControlSetup()
        {
            base.doViewControlSetup();

            AuthenticationConfiguration authConfig = wiContext.getConfiguration().getAuthenticationConfiguration();
            WIAuthPoint wiAuthPoint = authConfig.getAuthPoint() as WIAuthPoint;
            if (wiAuthPoint != null)
            {
                // Check whether to show login type options
                ExplicitAuth expAuth = (ExplicitAuth) wiAuthPoint.getAuthMethod(WIAuthType.EXPLICIT);
                bool onlyExplicitAllowed = ((getAllowedAuthMethods().size() == 1) && (expAuth != null));
                bool noOptionsAllowed = getAllowedAuthMethods().isEmpty();
                viewControl.setShowLoginTypeOptions(!(onlyExplicitAllowed || noOptionsAllowed));
                viewControl.setAllUIDisabled(noOptionsAllowed);

                // Do not overwrite a previous error if one has been stored
                if (noOptionsAllowed && !isFeedbackSet())
                {
                    setFeedback(MessageType.ERROR, "NoAuthenticationMethods");
                }

                // Check explicit login settings
                UserEnvironmentAdaptor envAdaptor = wiContext.getUserEnvironmentAdaptor();

                if (expAuth != null)
                {
                    // Check whether it is NDS login
                    if (expAuth is ExplicitNDSAuth)
                    {
                        ExplicitNDSAuth ndsAuth = (ExplicitNDSAuth) expAuth;
                        viewControl.setNDSEnabled(true);
                        viewControl.setNDSTree(ndsAuth.getTreeName());
                        viewControl.setShowFindContext(!ndsAuth.getServerList().isEmpty());

                        // If user has a context stored in a password, verify that it is allowed
                        // and then set it as the selected menu item
                        string lastContext = (string) envAdaptor.getUserState().get(Constants.COOKIE_NDS_CONTEXT);
                        if (lastContext != null && viewControl.getNDSContexts() != null &&
                            viewControl.getNDSContexts().Length < 1)
                        {
                            if (ndsAuth.getContextList().isEmpty() || ndsAuth.isValidContext(lastContext))
                            {
                                viewControl.setNDSContexts(new string[] {lastContext});
                            }
                        }
                    }
                    else
                    {
                        // Not NDS
                        setDomainDisplay((ExplicitUDPAuth) expAuth);

                        // don't show the password field if the 2-factor Password integration
                        // facility is configured.
                        viewControl.setShowPassword(
                            !(TwoFactorAuth.isPasswordIntegrationEnabled(wiContext.getConfiguration())));
                    }
                }
                else
                {
                    viewControl.setShowDomain(true);
                }


                bool isSMCLoggedOut =
                    Strings.equalsIgnoreCase(
                        (string) envAdaptor.getClientSessionState().get(Constants.COOKIE_SMC_LOGGED_OUT),
                        Constants.VAL_ON);
                if (isSMCLoggedOut)
                {
                    welcomeControl.setTitle(wiContext.getString("PleaseCloseBrowser"));
                    welcomeControl.setBody(wiContext.getString("CloseBrowserForSecurityReasons"));
                    setFeedback(MessageType.ERROR, "LoggedOutCloseBrowser");
                    viewControl.setAllUIDisabled(true);
                }
            }
        }

        /// <summary>
        /// Gets the set of authentication methods that the user is allowed to
        /// use.
        ///
        /// This implementation includes authentication methods that are only
        /// allowed on the IIS web server platform.
        /// </summary>
        /// <returns>set of authentication method names (Set containing String objects)</returns>
        public override Set getAllowedAuthMethods()
        {
            // Get a pre-populated set from the default implementation in the
            // base class.
            Set authMethods = base.getAllowedAuthMethods();

            AuthenticationConfiguration authConfig = wiContext.getConfiguration().getAuthenticationConfiguration();
            WIAuthPoint wiAuthPoint = authConfig.getAuthPoint() as WIAuthPoint;

            if (wiAuthPoint != null)
            {
                // Check whether the particular authentication methods are enabled by administrator
                // and supported on the user's browser and platform.

                // passthrough
                if (passthroughSupportedByClient() && wiAuthPoint.isAuthMethodEnabled(WIAuthType.SINGLE_SIGN_ON))
                {
                    authMethods.add(WIAuthType.SINGLE_SIGN_ON);
                }

                // smartcard
                if (certificateSupportedByClient() && wiAuthPoint.isAuthMethodEnabled(WIAuthType.CERTIFICATE))
                {
                    authMethods.add(WIAuthType.CERTIFICATE);
                }

                // passthrough smartcard
                if (certificateSingleSignOnSupportedByClient() &&
                    wiAuthPoint.isAuthMethodEnabled(WIAuthType.CERTIFICATE_SINGLE_SIGN_ON))
                {
                    authMethods.add(WIAuthType.CERTIFICATE_SINGLE_SIGN_ON);
                }
            }

            return authMethods;
        }

        private bool isOsWindows()
        {
            return wiContext.getClientInfo().osWin32() || wiContext.getClientInfo().osWin64();
        }

        private bool isOsLinux()
        {
            return wiContext.getClientInfo().osLinux();
        }
		
		// Determines whether passthrough, smartcard and passthrough smartcard
		// are supported on client's browser.
        private bool isBrowserAuthSupported()
        {
            return wiContext.getClientInfo().isIE() ||
                   wiContext.getClientInfo().isFirefox() ||
                   wiContext.getClientInfo().isIceweasel();
        }

        private bool certificateSupportedByClient()
        {
            bool isBrowserSupported = isBrowserAuthSupported();
            bool isOSsupported = isOsLinux() || isOsWindows();

            return isOSsupported && isBrowserSupported;
        }

        private bool certificateSingleSignOnSupportedByClient()
        {
            bool isBrowserSupported = isBrowserAuthSupported();
            bool isOSsupported = isOsWindows();

            return isOSsupported && isBrowserSupported;
        }

        /// <summary>
        /// Determines whether the combination of user's browser and OS support pass-through
        /// authentication.
        /// </summary>
        /// <returns>true if the browser on the given OS supports pass-through, otherwise false</returns>
        private bool passthroughSupportedByClient()
        {
            // CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP
            //
            // CUSTOMIZATION POINT
            //
            // Feature: allow pass-through authentication on any browser
            //
            // By default, Web Interface only supports pass-through authentication
            // when users are browsing with Internet Explorer and Firefox on Windows.
            //
            // However, it is possible to use pass-through with Opera and potentially
            // other browsers with a little browser configuration.
            //
            // This function decides whether the user's browser is suitable for
            // pass-through authentication.
            //
            // To let your users take advantage of pass-through, modify the
            // line below to return an appropriate result. For example, to
            // allow pass-through authentication on any browser on any OS,
            // change the line to:
            //
            // return true;
            //
            // CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP

            return isOsWindows() && isBrowserAuthSupported();
        }

        private PageAction authenticateIntegrated()
        {
            Authentication.getAuthenticationState(wiContext.getWebAbstraction()).addPageToQueueHead("integrated", null);
            Authentication.getAuthenticationState(wiContext.getWebAbstraction()).pageCompleted();

            // redirect to integrated page so that IIS can authenticate the user
            return new PageAction(Authentication.getAuthenticationState(wiContext.getWebAbstraction()).getCurrentPage(), true);
        }

        private PageAction authenticateCertificate()
        {
            if (!((AspWebAbstraction)wiContext.getWebAbstraction()).Context.Request.IsSecureConnection)
            {
                // Redirect to HTTPS so the user's cert can be sent.  When we do this, the
                // user's session is lost so we have to indicate to ourselves that the user
                // has already chosen their logon method
                string host = ((AspWebAbstraction)wiContext.getWebAbstraction()).Context.Request.ServerVariables[Constants.SRV_SERVER_NAME];
                string urlPath = wiContext.getWebAbstraction().getRequestPath();
                wiContext.getWebAbstraction().abandonSession();
                string url = "https://" + host + urlPath + "?" + Constants.QSTR_LOGINTYPE + "=" + WIAuthType.CERTIFICATE;
                return new PageAction(url, true);
            }
            else
            {
                Authentication.getAuthenticationState(wiContext.getWebAbstraction()).addPageToQueueHead("certificate", null);
                Authentication.getAuthenticationState(wiContext.getWebAbstraction()).pageCompleted();
                // redirect here so that IIS can authenticate the user

                // For IE, instead of going straight to the certificate.aspx page, go to the preCertificate.aspx.
                // This page will make AJAX call to the certificate and redirect to certificate page on success or
                // to the certificate error on failure. This is necessary to fix the problem when user cancels PIN
                // prompt on IE.
                // Page is set as unprotected by auth filter in web.config, however there is a logic on the page that
                // will allow it to be accessed only if the certificate page can be accessed.
                // The preCertificate page is optional in the auth process, it is only required for IE.
                PageAction pageAction = new PageAction(Authentication.getAuthenticationState(wiContext.getWebAbstraction()).getCurrentPage(), true);
                
                if (wiContext.getClientInfo().isIE())
                {
                    pageAction = new PageAction(Constants.PAGE_PRE_CERTIFICATE, true);
                }

                return pageAction;
            }
        }

        public override PageAction getPageAction(WIAuthType loginType)
        {
            PageAction pageAction = null;

            if ((WIAuthType.SINGLE_SIGN_ON.Equals(loginType)) || (WIAuthType.CERTIFICATE_SINGLE_SIGN_ON.Equals(loginType)))
            {
                pageAction = authenticateIntegrated();
            }
            else if (WIAuthType.CERTIFICATE.Equals(loginType))
            {
                pageAction = authenticateCertificate();
            }
            else
            {
                pageAction = base.getPageAction(loginType);
            }

            return pageAction;
        }

        /**
         * Attempt to perform silent authentication.
         *
         * If anonymous authentication is the only method allowed, then silent
         * authentication will always be performed.
         *
         * Other implicit login methods, in order of precedence, are as follows:
         *
         * - Smartcard Passthrough
         * - Passthrough
         * - Smartcard
         *
         * If any of the above methods is the only authentication method allowed, then
         * silent authentication will be performed.
         *
         * If they are among several allowed methods, then silent authentication is
         * performed using the priority order above.
         *
         * In all cases except anonymous, silent authentication may only work
         * on Internet Explorer.
         *
         * Returns a boolean to indicate if page processing should continue.
         */
        public override bool processDirectLogin()
        {

            AuthenticationConfiguration authConfig = wiContext.getConfiguration().getAuthenticationConfiguration();
            WIAuthType logonMode = null;
            WIAuthPoint wiAuthPoint = authConfig.getAuthPoint() as WIAuthPoint;
            if (wiAuthPoint != null)
            {

                bool oneLoginType = (getAllowedAuthMethods().size() == 1);
                if (oneLoginType && wiAuthPoint.isAuthMethodEnabled(WIAuthType.ANONYMOUS))
                {
                    logonMode = WIAuthType.ANONYMOUS;
                }
                else
                {
                    Set allowedAuthMethods = getAllowedAuthMethods();

                    // Go through the direct logon methods in order of precedence
                    // until we find one we can use, or we run out.
                    int i = 0;
                    while ((logonMode == null) && (i < DIRECT_LOGON_PRECEDENCE.Length))
                    {
                        if (allowedAuthMethods.contains(DIRECT_LOGON_PRECEDENCE[i]))
                        {
                            logonMode = DIRECT_LOGON_PRECEDENCE[i];
                        }

                        i++;
                    }
                }

                if (logonMode != null)
                {
                    // Attempt the silent authentication
                    return handleLoginRequest(logonMode);
                }
            }
            return true;
        }

        // Precedence order for direct logon.
        // The first of these to be allowed should be used.
        private static readonly WIAuthType[] DIRECT_LOGON_PRECEDENCE =
            new WIAuthType[]
                {
                    WIAuthType.CERTIFICATE_SINGLE_SIGN_ON, 
                    WIAuthType.SINGLE_SIGN_ON, 
                    WIAuthType.CERTIFICATE
                };

        /*
         * LoginNDS
         */

        public override PageAction authenticateNDS(ExplicitNDSAuth ndsAuth, string username, string password,
            string passcode, string context)
        {
            WebAbstraction web = wiContext.getWebAbstraction();
            UserEnvironmentAdaptor envAdaptor = wiContext.getUserEnvironmentAdaptor();
            AuthenticationState authenticationState = Authentication.getAuthenticationState(web);

            // store the result as we go
            AccessTokenResult accessTokenResult = null;

            if (string.IsNullOrEmpty(username))
            {
                // reject blank usernames
                accessTokenResult = new AccessTokenResult(MessageType.ERROR, "BlankUsername", true, false, null, null);
            }
            else
            {
                switch (username.IndexOf('.'))
                {
                    case -1:
                        // not fully qualified - either use given context or search
                        String actualContext = null;
                        if (string.IsNullOrEmpty(context) || (context == wiContext.getString("lookupContextString")))
                        {
                            // search for context
                            System.Collections.ArrayList contexts = findContext(username, ndsAuth);
                            if (contexts != null && contexts.Count > 0)
                            {
                                // context(s) found
                                if (contexts.Count == 1)
                                {
                                    // Use the single context found
                                    actualContext = (string)contexts[0];
                                }
                                else
                                {
                                    // multiple contexts were found for this user
                                    // set contexts in alphabetical order
                                    contexts.Sort();
                                    Authentication.setContextList((string[])(contexts.ToArray(typeof(string))), web);

                                    // Not invalid field as such, but we may as well draw attention to
                                    // the context selection dropdown
                                    accessTokenResult = new AccessTokenResult(null, MessageType.INFORMATION, "NDSMultiContext", false, false, true, username, null);
                                }
                            }
                            else
                            {
                                // error case
                                if (envAdaptor.getUserState().get(Constants.COOKIE_NDS_CONTEXT) != null)
                                {
                                    envAdaptor.getUserState().remove(Constants.COOKIE_NDS_CONTEXT);
                                }
                                accessTokenResult = new AccessTokenResult(MessageType.ERROR, "InvalidCredentials", false, false, null, null);
                            }
                        }
                        else
                        {
                            // Context appears to be valid, so we will use this one
                            actualContext = context;
                        }

                        if (accessTokenResult == null)
                        {
                            // Use the context as we have not had an error
                            envAdaptor.getUserState().put(Constants.COOKIE_NDS_CONTEXT, context);
                            // make context fully qualified so that username is also fully qualified
                            accessTokenResult = new AccessTokenResult(new NDSCredentials(username, "." + actualContext, ndsAuth.getTreeName(), password));
                        }
                        break;

                    case 0:
                        // fully qualified so try and login
                        accessTokenResult = new AccessTokenResult(new NDSCredentials(username, ndsAuth.getTreeName(), password));
                        break;

                    default:
                        // invalid username
                        accessTokenResult = new AccessTokenResult(MessageType.ERROR, "NDSBadUsername", true, false, username, null);
                        break;
                }
            }

            // Process the result
            if (accessTokenResult.isError())
            {
                Authentication.processAccessTokenResultError(web, accessTokenResult);
                return null; // do nothing more
            }
            else
            {
                Map parameters = createExplicitAuthenticationParameters(accessTokenResult.getAccessToken(), ndsAuth);
                authenticationState.addPageToQueueHead("explicit", parameters);

                // If two-factor authentication is in use, insert it in the sequence
                TwoFactorAuthMethod twoFactorMethod = TwoFactorAuth.getTwoFactorAuthMethod(wiContext.getConfiguration());
                if (twoFactorMethod != null)
                {
                    parameters.put(TwoFactorAuth.VAL_PASSCODE, passcode);
                    authenticationState.addPageToQueueHead(twoFactorMethod.getName().ToLower(), parameters);
                }

                authenticationState.pageCompleted();
                // Move on to the next page - no need to redirect
                return new PageAction(authenticationState.getCurrentPage(), false);
            }
        }

        /**
         * Returns a list of contexts for the given user and NDS configuration.
         */
        private System.Collections.ArrayList findContext(string username, ExplicitNDSAuth ndsAuth)
        {

            WebAbstraction web = wiContext.getWebAbstraction();
            ServicePool servicePool = (ServicePool)web.getApplicationAttribute(AppAttributeKey.NDS_SERVICE_POOL);
            if (servicePool == null)
            {
                if (ndsAuth.getServerList().isEmpty())
                {
                    // no server list disables context lookup feature
                    return null;
                }
                servicePool = new ServicePool(
                    (ndsAuth.getUseLoadBalancing() ? ServiceSelectorType.LOAD_BALANCER : ServiceSelectorType.FAILOVER),
                    ndsAuth.getServerList());
                web.setApplicationAttribute(AppAttributeKey.NDS_SERVICE_POOL, servicePool);
            }

            ServiceSelector serviceSelector = servicePool.getServiceSelector();
            System.Collections.ArrayList contexts = null;
            while (contexts == null)
            {
                string server = (string)serviceSelector.getService();
                if (server == null)
                {
                    wiContext.log(MessageType.ERROR, "NDSAllServersFailed");
                    return null;
                }

                contexts = findContext(username, server);

                if (contexts == null)
                {
                    serviceSelector.markFailed(server);
                }
            }

            // remove any contexts that aren't allowed
            if (contexts.Count > 0 && !ndsAuth.getContextList().isEmpty())
            {
                for (int i = contexts.Count - 1; i >= 0; i--)
                {
                    if (!ndsAuth.isValidContext((string)contexts[i]))
                    {
                        contexts.RemoveAt(i);
                    }
                }
            }
            return contexts;
        }

        /**
         * Returns a list of contexts for the given user and NDS server.
         */
        private System.Collections.ArrayList findContext(string username, string ndsServer)
        {

            // CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP
            //
            // CUSTOMIZATION POINT
            //
            // If ndsAdminUsername and ndsAdminPassword are left as empty string, an anonymous bind will be performed
            // For authenticated binds, most eDirectory servers insist that SSL must be used so ensure the following:
            //   - enter the username and password below in the format given in the comments
            //   - that an LDAPS URL is entered in the configuration
            //   - the NDS certificate must be in the trusted part of the local machine's certificate store (use mmc)
            //   - the NDS server name must match the certificate
            //
            // CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP
            string ndsAdminUsername = ""; // example: "cn=Administrator,ou=company,o=com";
            string ndsAdminPassword = ""; // example: "password";

            // Calculate server name and bind flags required for API
            AuthenticationTypes auth = AuthenticationTypes.ServerBind;
            string server = ndsServer.ToUpper();
            if (server.StartsWith("LDAPS://"))
            {
                server = "LDAP://" + server.Substring("LDAPS://".Length);
                auth |= AuthenticationTypes.SecureSocketsLayer;
            }
            else if (ndsAdminUsername == "" && ndsAdminPassword == "")
            {
                auth |= AuthenticationTypes.Anonymous;
            }

            // do search
            try
            {
                DirectoryEntry entry = new DirectoryEntry(server, ndsAdminUsername, ndsAdminPassword, auth);
                DirectorySearcher search = new DirectorySearcher(entry, "(&(|(objectclass=person)(objectclass=alias))(cn=" + DirectoryUtilities.escapeLDAPQueryParameter(username) + "))", new string[] { "cn" });
                System.Collections.ArrayList ndsContexts = new System.Collections.ArrayList();
                foreach (SearchResult result in search.FindAll())
                {
                    string url = result.Path;
                    // Convert LDAP URL into an NDS context
                    // eg. "LDAP://NDSMACHINE/cn=user,ou=company,o=com" into "company.com"
                    string[] pathElements = DirectoryUtilities.getPathElementsFromURL(url);
                    if (pathElements == null || pathElements.Length < 2)
                    {
                        throw new ArgumentException("path must contain at least 2 elements: " + url);
                    }

                    // Concatenate all but first element with . separator
                    string context = null;
                    for (int i = 1; i < pathElements.Length; i++)
                    {
                        context = (context == null) ? pathElements[i] : context + "." + pathElements[i];
                    }
                    ndsContexts.Add(context);
                }
                return ndsContexts;
            }
            catch (Exception e)
            {
                wiContext.log(MessageType.ERROR, "NDSServerFailedContextSearch", new object[] { ndsServer, e.ToString() });
                return null;
            }
        }
    }
}
