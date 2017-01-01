using com.citrix.authentication.web;
using com.citrix.wi.aspnetenvironment;
using com.citrix.wi.clientdetect;
using com.citrix.wi.clientdetect.util;
using com.citrix.wi.config;
using com.citrix.wi.localization;
using com.citrix.wi.mvc.asp;
using com.citrix.wi.types;
using com.citrix.wi;
using com.citrix.wi.ui;
using com.citrix.wi.util;
using com.citrix.wing;
using com.citrix.wing.config;
using com.citrix.wing.rade;
using com.citrix.wing.util;
using com.citrix.wing.webpn;
using com.citrix.wi.config.auth;
using com.citrix.authenticators;
using com.citrix.wi.pageutils;

using Citrix.DeliveryServices.Security.Tokens.Configuration.Factories;
using Citrix.DeliveryServices.Security.Tokens.Interfaces;

using java.io;
using java.util;

using System;
using System.Web.Configuration;

namespace com.citrix
{
    public class WebInterface : System.Web.HttpApplication
    {
        private const string CONFIG_TOKEN_MGR = "citrix.deliveryservices/tokenManager";

        // CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP
        //
        // CUSTOMIZATION POINT
        //
        // Modify this file to pull configuration from other sources, such as
        // a database. It is also possible to have multiple ConfigurationProvider and
        // WebPN instances stored in the Application state.  It would then be
        // possible to switch between them based on user identity, remote IP
        // address, etc.
        //
        // CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP

        public static string APPLICATION_FILE_SYSTEM_PATH;

        protected void Application_Start(object sender, EventArgs e)
        {
            // Store application path.
            APPLICATION_FILE_SYSTEM_PATH = Server.MapPath("~");

            // Create a StaticEnvironmentAdaptor instance.
            //
            // StaticEnvironmentAdaptors tie the WING API into the underlying
            // application server environment.  Only one instance is needed per
            // web application.
            //
            WIASPNetStaticAdaptor staticEnvAdaptor = new WIASPNetStaticAdaptor(this);
            staticEnvAdaptor.setApplicationAttribute(AppAttributeKey.STATIC_ADAPTOR, staticEnvAdaptor);

            // Record the application startup.
            // Pages check this application-level variable to decide whether to display an
            // internal error page.
            staticEnvAdaptor.setApplicationAttribute(AppAttributeKey.SITE_STARTUP_OK, java.lang.Boolean.FALSE);

            // Create the resource bundle factory for the site
            // This needs to be one of the first things to happen at site startup,
            // so that any errors from this point on are localised correctly.
            ResourceBundleFactory bundleFactory = createResourceBundleFactory(staticEnvAdaptor);
            staticEnvAdaptor.setApplicationAttribute(AppAttributeKey.RESOURCE_BUNDLE_FACTORY, bundleFactory);

            // Create a WebPNBuilder instance.
            //
            // WebPNBuilders are used to create instances of WebPN and
            // UserContexts - which are the key classes in the WING API.  As
            // with the static adaptor, only one is needed per web application.
            //
            WebPNBuilder builder = WebPNBuilder.getInstance();
            staticEnvAdaptor.setApplicationAttribute(AppAttributeKey.WEBPN_BUILDER, builder);

            // Create a RoamingUserManager instance.
            staticEnvAdaptor.setApplicationAttribute(AppAttributeKey.ROAMING_USER_MANAGER, new RoamingUserManager());

            // Create a RadeBuilder instance.
            //
            // RadeBuilders are used to create instances of RadeService
            //
            RadeBuilder radeBuilder = RadeBuilder.getInstance();
            staticEnvAdaptor.setApplicationAttribute(AppAttributeKey.RADE_BUILDER, radeBuilder);

            // Create a RADIUSAuthenticatorFactory instance.
            //
            RADIUSAuthenticatorFactory staticRADIUSAuthenticatorFactory = new RADIUSAuthenticatorFactory();
            staticEnvAdaptor.setApplicationAttribute(TwoFactorAuth.VAL_RADIUS_AUTHENTICATOR_FACTORY, staticRADIUSAuthenticatorFactory);

            // Create a LanguageManager instance.
            //
            // The LanguageManager handles multi-language support for a site,
            // including access to localized strings and other resources.
            LanguageManager langManager = new WILanguageManager(staticEnvAdaptor, null);
            staticEnvAdaptor.setApplicationAttribute(AppAttributeKey.LANGUAGE_MANAGER, langManager);

            // Create a WizardLanguageManager instance for the client wizard
            WizardLanguageManager wizLangManager = new WizardLanguageManager(staticEnvAdaptor, null);

            // Create the MultiLanguage instance for the client wizard
            MultiLanguage mui = new MultiLanguage(wizLangManager);
            staticEnvAdaptor.setApplicationAttribute(WizardConstants.APP_ATTRIBUTE_MULTI_LANGUAGE, mui);

            WIReloadHandler reloadHandler = new WIReloadHandler(staticEnvAdaptor);

            try
            {
                // Initialize the configuration provider
                // The bootstrapper reads the bootstrap file and then creates the appropriate ConfigurationProvider
                // The ConfigurationProviders can be instantiated by hand for greater control

                // Register the site with a version number of the form X.Y.0.0, where X and Y are the WI major and minor version numbers.
                VersionNumber wiVersion = com.citrix.wi.Version.getVersionNumber();
                string registrationVersionNumber = wiVersion.getComponent(0) + "." + wiVersion.getComponent(1) + ".0.0";
                ConfigurationProvider configProvider = ConfigurationBootstrapper.bootstrap(staticEnvAdaptor, reloadHandler);
                staticEnvAdaptor.setApplicationAttribute(AppAttributeKey.CONFIGURATION_PROVIDER, configProvider);

                // Since we now know the installation locale we should use this now
                staticEnvAdaptor.overrideSystemLocale(configProvider.getDefaultLocale());

                // Start a FileSystemWatcher to reload the local configuration if it changes
                string configPath = getConfigPath(configProvider);
                object changeWatcher = FileSystemWatcherWrapper.Create(
                    new SimpleDelegate(configProvider.trustedReloadConfiguration), new System.IO.ErrorEventHandler(filewatcherErrorHandler), configPath, "", false, staticEnvAdaptor, bundleFactory);
                staticEnvAdaptor.setApplicationAttribute(AppAttributeKey.CHANGE_WATCHER, changeWatcher);

                // Start FileSystemWatchers to reload any new languages and clients
                string clientsPath = langManager.getPath("COMMON_CLIENTS_PATH");
                object commonClientsWatcher = FileSystemWatcherWrapper.Create(
                    new SimpleDelegate(reloadHandler.reloadClients), new System.IO.ErrorEventHandler(filewatcherErrorHandler), clientsPath, "", true, staticEnvAdaptor, bundleFactory);

                string languagesPath = langManager.getPath("COMMON_LANGUAGES_PATH");
                object commonLanguagesWatcher = FileSystemWatcherWrapper.Create(
                    new SimpleDelegate(reloadHandler.reloadLanguages), new System.IO.ErrorEventHandler(filewatcherErrorHandler), languagesPath, "", true, staticEnvAdaptor, bundleFactory);

                object privateClientsWatcher = FileSystemWatcherWrapper.Create(
                    new SimpleDelegate(reloadHandler.reloadClients), new System.IO.ErrorEventHandler(filewatcherErrorHandler), Server.MapPath("~/Clients"), "", true, staticEnvAdaptor, bundleFactory);

                object privateLanguagesWatcher = FileSystemWatcherWrapper.Create(
                    new SimpleDelegate(reloadHandler.reloadLanguages), new System.IO.ErrorEventHandler(filewatcherErrorHandler), Server.MapPath("~/languages"), "", true, staticEnvAdaptor, bundleFactory);

                string wizardLanguagesPath = langManager.getPath("COMMON_WIZARD_LANG_PATH");
                object commonWizardLanguagesWatcher = FileSystemWatcherWrapper.Create(
                    new SimpleDelegate(reloadHandler.reloadWizardLanguages), new System.IO.ErrorEventHandler(filewatcherErrorHandler), wizardLanguagesPath, "", true, staticEnvAdaptor, bundleFactory);

                object privateWizardLanguagesWatcher = FileSystemWatcherWrapper.Create(
                    new SimpleDelegate(reloadHandler.reloadWizardLanguages), new System.IO.ErrorEventHandler(filewatcherErrorHandler), Server.MapPath("~/clientDetection/localizedContent"), "", true, staticEnvAdaptor, bundleFactory);

                string helpPath = langManager.getPath("COMMON_WIZARD_HELP_PATH");
                object commonWizardHelpWatcher = FileSystemWatcherWrapper.Create(
                    new SimpleDelegate(reloadHandler.reloadWizardLanguages), new System.IO.ErrorEventHandler(filewatcherErrorHandler), helpPath, "", true, staticEnvAdaptor, bundleFactory);

                object privateWizardHelpWatcher = FileSystemWatcherWrapper.Create(
                    new SimpleDelegate(reloadHandler.reloadWizardLanguages), new System.IO.ErrorEventHandler(filewatcherErrorHandler), Server.MapPath("~/clientDetection/help"), "", true, staticEnvAdaptor, bundleFactory);

                string mediaPath = langManager.getPath("COMMON_WIZARD_MEDIA_PATH");
                object commonWizardMediaWatcher = FileSystemWatcherWrapper.Create(
                    new SimpleDelegate(reloadHandler.reloadWizardLanguages), new System.IO.ErrorEventHandler(filewatcherErrorHandler), mediaPath, "", true, staticEnvAdaptor, bundleFactory);

                object privateWizardMediaWatcher = FileSystemWatcherWrapper.Create(
                    new SimpleDelegate(reloadHandler.reloadWizardLanguages), new System.IO.ErrorEventHandler(filewatcherErrorHandler), Server.MapPath("~/clientDetection/media"), "", true, staticEnvAdaptor, bundleFactory);

            }
            catch (com.citrix.wi.config.ConfigurationException ce)
            {
                // Log the configuration load/parse error and a stack trace.
                // Note that this will appear in the installation locale if the bootstrap config was loaded, otherwise it will be the system locale.
                staticEnvAdaptor.getDiagnosticLogger().log(
                    MessageType.ERROR, new LocalizableString(bundleFactory, ce.getMessageKey(), ce.getMessageArguments()), ce);
            }

            // Application startup has completed without any serious errors such as
            // unhandled exceptions or framework errors.
            // Record this so individual pages know that processing can continue.
            staticEnvAdaptor.setApplicationAttribute(AppAttributeKey.SITE_STARTUP_OK, java.lang.Boolean.TRUE);
        }

        private string getConfigPath(ConfigurationProvider configProvider)
        {
            // Find the absolute path to the folder containing the WebInterface config file.
            // The path in the configuration provider (from bootstrap.conf) may be an absolute path
            //  e.g. "c:\wiconfig\WebInterface.conf", or "\\host\wiconfig\WebInterface.conf"
            // or a partial path which is taken as relative to the "conf" directory in the web application root.
            //  e.g. "WebInterface.conf" becomes "~/conf/WebInterface.conf", "child/WebInterface.conf" becomes "~/conf/child/WebInterface.conf")
            //
            // Note, the config provider will have translated any backward slashes into forward slashes.
            string providedFilePath = configProvider.getConfigurationLocation();
            string providedFolderPath = providedFilePath.Substring(0, providedFilePath.LastIndexOf('/') + 1);
            if (new File(providedFilePath).isAbsolute())
            {
                return providedFolderPath;
            }
            else
            {
                return Server.MapPath("~/conf/" + providedFolderPath);
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // Get the top level exception for the case where ASP.Net has thrown an HTTPException
            System.Exception topLevelEx = Server.GetLastError();
            // Get the original exception, the root cause of the problem.
            System.Exception rootEx = topLevelEx.InnerException;

            StaticEnvironmentAdaptor staticEnvAdaptor = (StaticEnvironmentAdaptor)Application[AppAttributeKey.STATIC_ADAPTOR];
            if (staticEnvAdaptor != null)
            {
                ResourceBundleFactory bundleFactory =
                        (ResourceBundleFactory)staticEnvAdaptor.getApplicationAttribute(AppAttributeKey.RESOURCE_BUNDLE_FACTORY);
                if (rootEx is java.lang.Throwable)
                {
                    // A Java Exception has occurred.  Log a general fatal error message
                    // and pass in the Java Throwable for stack tracing.
                    staticEnvAdaptor.getDiagnosticLogger().log(
                            MessageType.ERROR,
                            new LocalizableString(bundleFactory, "JavaFatalError"),
                            (java.lang.Throwable)rootEx);
                }
                else
                {
                    // A .Net Exception has occurred.  Log a general error message with
                    // the .Net Exception data passed in as a message parameter, unless the error
                    // was that the requested page didn't exist (a 404 error).
                    if (topLevelEx is System.Web.HttpException && ((System.Web.HttpException)topLevelEx).GetHttpCode() == 404)
                    {
                        // Do not log the 404 error, send the error:
                        Response.Status = "404 not found";
                        Response.End();
                    }
                    else
                    {
                        //Need to check that inner exception is not null before logging it along with
                        if (rootEx != null)
                        {
                            staticEnvAdaptor.getDiagnosticLogger().log(
                                    MessageType.ERROR,
                                    new LocalizableString(bundleFactory, "DotNetFatalError", topLevelEx.ToString() + '\n' + rootEx.ToString()));
                        }
                        else
                        {
                            //inner exception is null, so only logging the outer exception
                            staticEnvAdaptor.getDiagnosticLogger().log(
                            MessageType.ERROR,
                            new LocalizableString(bundleFactory, "DotNetFatalError", topLevelEx.ToString()));
                        }
                    }
                }
            }
        }

        protected void Application_EndRequest(Object sender, EventArgs e)
        {
            // Find out whether the connection is secure from the client's point of view
            UserEnvironmentAdaptor userAdaptor = (UserEnvironmentAdaptor)Context.Items["com.citrix.wi.UserEnvAdaptor"];
            bool isClientConnSecure = Include.isClientConnectionSecure(userAdaptor);

            // Add the "Secure" cookie attribute for all secure connections
            if (Request.IsSecureConnection || isClientConnSecure)
            {
                foreach (string cookie in Response.Cookies)
                {
                    Response.Cookies[cookie].Secure = true;
                }
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            ClientInfo info = (ClientInfo)base.Session[Include.SV_CLIENT_INFO];
            if (info == null)
            {
                info = new ClientInfo();
            }

            try
            {
                info.initialize(Request.ServerVariables["HTTP_USER_AGENT"]);
            }
            catch (java.lang.IllegalArgumentException)
            {
                bool acceptsNullUserAgent = Include.checkRequestAcceptsNullUserAgent(Request.FilePath);
                if (!acceptsNullUserAgent)
                {
                    Include.logNullUserAgentMessage((StaticEnvironmentAdaptor)Application[AppAttributeKey.STATIC_ADAPTOR], Request.UserHostAddress);
                    Session[Include.SV_NULL_USER_AGENT] = java.lang.Boolean.TRUE;
                }
            }

            base.Session[Include.SV_CLIENT_INFO] = info;
        }

        private void filewatcherErrorHandler(Object sender, System.IO.ErrorEventArgs e)
        {
            StaticEnvironmentAdaptor staticEnvAdaptor = (StaticEnvironmentAdaptor)Application[AppAttributeKey.STATIC_ADAPTOR];
            if (staticEnvAdaptor == null) return;
            ResourceBundleFactory bundleFactory = (ResourceBundleFactory)
                    staticEnvAdaptor.getApplicationAttribute(AppAttributeKey.RESOURCE_BUNDLE_FACTORY);
            if (bundleFactory == null) return;
            staticEnvAdaptor.getDiagnosticLogger().log(
                MessageType.ERROR,
                new LocalizableString(bundleFactory, "ConfLoadError"));
        }

        /**
         * Creates a ResourceBundleFactory for use with this site.
         *
         * The resource bundles produced by the factory contain strings from all of
         * the resource files that apply to the end user-facing UI of the site.
         *
         * @param sea StaticEnvironmentAdaptor
         * @return a ResourceBundleFactory instance
         */
        static ResourceBundleFactory createResourceBundleFactory(StaticEnvironmentAdaptor sea)
        {
            ResourceBundleFactory[] factories = new WIResourceBundleFactory[4];
            factories[0] = sea.getResourceBundleFactory("common_strings");
            factories[1] = sea.getResourceBundleFactory("accessplatform_strings");
            factories[2] = sea.getResourceBundleFactory("help_strings");
            factories[3] = sea.getResourceBundleFactory("eventlog_strings");
            return new CachedChainedResourceBundleFactory(factories);
        }

        const string MSG_NO_DEFAULT_LANG_PACK = "The default language pack could not be found. " +
                                               "Please ensure that the language pack for the default locale is " +
                                               "correctly installed and restart the web server.";

        private class WIReloadHandler : ReloadHandler
        {

            private WIASPNetStaticAdaptor staticEnvAdaptor;

            public WIReloadHandler(WIASPNetStaticAdaptor staticEnvAdaptor)
            {
                this.staticEnvAdaptor = staticEnvAdaptor;
            }

            // Reloads languages
            public void reloadLanguages()
            {
                // Re-discover available language packs for the site and the client detection wizard
                LanguageManager langManager = (LanguageManager)staticEnvAdaptor.getApplicationAttribute(AppAttributeKey.LANGUAGE_MANAGER);
                langManager.discoverLanguages();

                // Clear the cache of resource bundle factories in the StaticEnvironmentAdaptor
                // This will force the factories to be rebuilt from scratch
                staticEnvAdaptor.clearBundleFactoryCache();

                // Recreate the resource bundle factory used for end user UI strings
                ResourceBundleFactory bundleFactory = createResourceBundleFactory(staticEnvAdaptor);
                staticEnvAdaptor.setApplicationAttribute(AppAttributeKey.RESOURCE_BUNDLE_FACTORY, bundleFactory);
            }

            // Reloads languages and strings for the client detection wizard
            public void reloadWizardLanguages()
            {
                MultiLanguage mui = (MultiLanguage)staticEnvAdaptor.getApplicationAttribute(WizardConstants.APP_ATTRIBUTE_MULTI_LANGUAGE);
                mui.reloadLanguages();
            }

            // Reloads clients
            public void reloadClients()
            {
                ConfigurationProvider configProvider = (ConfigurationProvider)staticEnvAdaptor.getApplicationAttribute(AppAttributeKey.CONFIGURATION_PROVIDER);
                WIConfiguration wiConfig = configProvider.getWIConfiguration();
                if (wiConfig != null)
                {
                    ClientManager clientManager = new ClientManager(staticEnvAdaptor, wiConfig.getClientDeploymentConfiguration());
                    staticEnvAdaptor.setApplicationAttribute(AppAttributeKey.CLIENT_MANAGER, clientManager);
                }
            }

            public void reloadConfiguration(ConfigurationProvider configProvider)
            {

                // The event id file might have been modified. We need to log events with new ids.
                staticEnvAdaptor.reloadDiagnosticLogger();

                // Ask the config provider for the initial configuration.
                WIConfiguration config = configProvider.getWIConfiguration();

                if (config == null)
                {
                    // Couldn't load config
                    return;
                }
                // load all the clients
                reloadClients();

                // store the ClientDeploymentConfiguration for the client detection wizard
                ClientDeploymentConfiguration clientDeploymentConfiguration = config.getClientDeploymentConfiguration();
                staticEnvAdaptor.setApplicationAttribute(WizardConstants.APP_ATTRIBUTE_CLIENT_DEPLOYMENT_CONFIGURATION,
                                                         clientDeploymentConfiguration);

                // The WI configuration has loaded successfully.
                // Set the configuration in the WIASPNetStaticAdaptor
                // so that the configured log settings can take effect.
                staticEnvAdaptor.setDiagnosticLoggerConfiguration(config.getDiagnosticLoggerConfiguration());

                LanguageManager langManager = (LanguageManager)
                                              staticEnvAdaptor.getApplicationAttribute(AppAttributeKey.LANGUAGE_MANAGER);
                Locale defaultLocale = config.getDefaultLocale();

                ResourceBundleFactory bundleFactory = (ResourceBundleFactory)
                                                      staticEnvAdaptor.getApplicationAttribute(
                                                          AppAttributeKey.RESOURCE_BUNDLE_FACTORY);

                // Check that the default locale is available
                if (!langManager.isLanguageAvailable(defaultLocale))
                {
                    // The precise default locale was not available
                    // Attempt to fall back to a more general alternative
                    defaultLocale = langManager.getBestMatch(defaultLocale);
                    if (defaultLocale != null)
                    {
                        // A match has been found
                        // Log a message warning that an alternative locale was used
                        staticEnvAdaptor.getDiagnosticLogger().log(
                            MessageType.WARNING,
                            new LocalizableString(bundleFactory, "DefaultLocaleFallback", config.getDefaultLocale(),
                                                  defaultLocale));
                    }
                    else
                    {
                        // No match was found for the specified locale
                        // Attempt to fall back to English and log an error message
                        defaultLocale = Locale.ENGLISH;
                        staticEnvAdaptor.getDiagnosticLogger().log(
                            MessageType.ERROR,
                            MSG_NO_DEFAULT_LANG_PACK);
                    }
                }

                // Re-set the default locale in case it has changed
                config.setDefaultLocale(defaultLocale);
                staticEnvAdaptor.overrideSystemLocale(defaultLocale);
                langManager.setDefaultLocale(defaultLocale);

                // Throw away NDS ServicePool so that a new one will be created
                staticEnvAdaptor.setApplicationAttribute(AppAttributeKey.NDS_SERVICE_POOL, null);

                // If RADIUS 2-factor authentication is configured, then update the
                // RADIUSAuthenticatorFactory (which is used to create RADIUSAuthenticator
                // objects) with the WIConfiguration settings.
                //
                AuthPoint authPoint = config.getAuthenticationConfiguration().getAuthPoint();
                if (authPoint is WIAuthPoint)
                {
                    RadiusConfig.updateAuthFactory((WIAuthPoint)authPoint, bundleFactory,
                        staticEnvAdaptor, APPLICATION_FILE_SYSTEM_PATH);
                }

                if (UseProtocolTransition(config))
                {
                    SetupProtocolTransitionServiceProxy(bundleFactory);
                }

                List webPNs = new ArrayList();

                try
                {
                    // CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP
                    //
                    // CUSTOMIZATION POINT
                    //
                    // Modify the configuration here to suit your need.  The configuration
                    // objects are more flexible than the configuration file.
                    //
                    // The first WebPN created is for normal usage, and aggregates all
                    // published resources on its farms for the user.  The loop further
                    // down is for Disaster Recovery.  Each of these farms is contacted
                    // separately, with no aggregation of published resources.
                    //
                    // CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP

                    // Get the builder objects
                    WebPNBuilder builder = (WebPNBuilder)staticEnvAdaptor.getApplicationAttribute(AppAttributeKey.WEBPN_BUILDER);
                    RadeBuilder radeBuilder = (RadeBuilder)staticEnvAdaptor.getApplicationAttribute(AppAttributeKey.RADE_BUILDER);

                    // Create a WebPN instance from the configuration.
                    //
                    // WebPN instances allow you to perform activities outside of a user
                    // context, such as checking access tokens (credentials) are valid.
                    WebPN primaryWebPN = builder.createWebPN(CreateWINGConfigFromWIConfig(config), staticEnvAdaptor);
                    webPNs.add(primaryWebPN);

                    // Create RADE service config so that it has details for
                    // all farms (normal and disaster recovery)
                    com.citrix.wing.config.Configuration radeServiceConfig = CreateWINGConfigFromWIConfig(config);

                    // Create Disaster Recovery farms from the configuration.
                    Iterator recoveryFarmsIterator = config.getRecoveryMPSFarmConfigs().iterator();
                    while (recoveryFarmsIterator.hasNext())
                    {
                        MPSFarmConfig recoveryFarmConfig = recoveryFarmsIterator.next() as MPSFarmConfig;

                        webPNs.add(builder.createWebPN(
                            CreateWINGConfigFromWIConfigWithFarmConfig(config, recoveryFarmConfig),
                            staticEnvAdaptor));

                        radeServiceConfig.addMPSFarmConfig(recoveryFarmConfig);
                    }

                    // Notify the authentication layer of any authentication configuration changes
                    // and check for any farm configuration changes.
                    // This must be done after any customization has been done to the configuration
                    // ie. the configuration is now immutable
                    IntermediateSessionConfiguration sessionConfig = GlobalDataUpdateUtil.updateIntermediateSessionConfig(config);
                    staticEnvAdaptor.setApplicationAttribute(AuthenticationFilter.CONFIG_OBJECT_KEY, sessionConfig);
                    staticEnvAdaptor.setApplicationAttribute(AuthenticationFilter.CONFIG_TIMESTAMP_KEY, DateTime.Now.Ticks);

                    // Create a RadeService instance from the configuration.
                    RadeService radeService = radeBuilder.createService(radeServiceConfig, staticEnvAdaptor);
                    staticEnvAdaptor.setApplicationAttribute(AppAttributeKey.RADE_SERVICE, radeService);

                    if (config.isRoamingUserEnabled())
                    {
                        // Update the farm group mappings that are used to support Roaming User
                        // This can trigger a WIException so is done after the intermediate session config has been created
                        // (if it were done before and an exception occurred the intermediate session config would not be updated
                        // and the corresponding change to the farm groups would go undetected)
                        //
                        // The Roaming User scenario only uses the primary WebPN, as this is where the user's
                        // home farm is located.
                        RoamingUserManager rum = (RoamingUserManager)staticEnvAdaptor.getApplicationAttribute(AppAttributeKey.ROAMING_USER_MANAGER);
                        try
                        {
                            rum.update(primaryWebPN, config.getFarmGroups());
                        }
                        catch (com.citrix.wi.WIException wie)
                        {
                            staticEnvAdaptor.getDiagnosticLogger().log(MessageType.ERROR, new LocalizableString(bundleFactory, wie.getMessageKey()), wie);
                        }
                    }
                }
                catch (com.citrix.wing.config.ConfigurationException ce)
                {
                    // Log the configuration error and a stack trace.
                    staticEnvAdaptor.getDiagnosticLogger().log(MessageType.ERROR, ce.getLocalizableMessage(), ce);
                }

                staticEnvAdaptor.setApplicationAttribute(AppAttributeKey.WEBPN_LIST, webPNs);
            }

            private void SetupProtocolTransitionServiceProxy(ResourceBundleFactory bundleFactory)
            {
                string ptsProducerId = WebConfigurationManager.AppSettings["AG_PTS_PRODUCER_ID"];

                // The PTS requires some extra configuration parameters and other objects,
                // notably an ITokenService for converting the username into a token the
                // PTS can use to complete its work.

                TokenManagerConfigurationParser tokenManagerParser = new TokenManagerConfigurationParser();
                ITokenManager tokenMgr = tokenManagerParser.CreateTokenManager();

                bool producerServiceIsConfigured = TokenManagerHasServicesDefined(tokenMgr) &&
                    !string.IsNullOrEmpty(ptsProducerId) &&
                    tokenMgr.Services.ContainsKey(ptsProducerId);

                if (producerServiceIsConfigured)
                {
                    ITokenService tokenService = tokenMgr.Services[ptsProducerId];
                    staticEnvAdaptor.setApplicationAttribute(AppAttributeKey.PTS_TOKEN_SERVICE_KEY, tokenService);
                }
                else
                {
                    staticEnvAdaptor.getDiagnosticLogger().log(MessageType.ERROR,
                            new LocalizableString(bundleFactory, "PTSConfigurationError"));
                }
            }

            private bool UseProtocolTransition(WIConfiguration config)
            {
                AuthenticationConfiguration authConfig = config.getAuthenticationConfiguration();
                AGAuthPoint authPoint = authConfig.getAuthPoint() as AGAuthPoint;
                if (authPoint != null)
                {
                    return authPoint.getAuthenticationMethod() == AGAuthenticationMethod.SMART_CARD_KERBEROS;
                }

                return false;
            }

            private bool TokenManagerHasServicesDefined(ITokenManager tokenMgr)
            {
                return (tokenMgr != null) && (tokenMgr.Services != null);
            }

            private com.citrix.wing.config.Configuration CreateWINGConfigFromWIConfigWithFarmConfig(WIConfiguration config, MPSFarmConfig farmConfig)
            {
                com.citrix.wing.config.Configuration wingConfig = new com.citrix.wing.config.Configuration();
                wingConfig.setGlobalConfig(config.getGlobalConfig());
                wingConfig.addMPSFarmConfig(farmConfig);
                wingConfig.setDMZRoutingPolicy(config.getDMZRoutingPolicy());
                wingConfig.setClientProxyPolicy(config.getClientProxyPolicy());

                return wingConfig;
            }

            private com.citrix.wing.config.Configuration CreateWINGConfigFromWIConfig(WIConfiguration config)
            {
                com.citrix.wing.config.Configuration wingConfig = new com.citrix.wing.config.Configuration();
                wingConfig.setGlobalConfig(config.getGlobalConfig());
                wingConfig.setMPSFarmConfigs(config.getMPSFarmConfigs());
                wingConfig.setDMZRoutingPolicy(config.getDMZRoutingPolicy());
                wingConfig.setClientProxyPolicy(config.getClientProxyPolicy());

                return wingConfig;
            }
        }
    }
}