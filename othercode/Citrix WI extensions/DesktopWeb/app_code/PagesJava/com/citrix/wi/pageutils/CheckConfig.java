/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils;

import com.citrix.wi.config.AuthenticationConfiguration;
import com.citrix.wi.config.ConfigurationProvider;
import com.citrix.wi.config.WIConfiguration;
import com.citrix.wi.config.auth.AuthMethod;
import com.citrix.wi.config.auth.ExplicitNDSAuth;
import com.citrix.wi.config.auth.RADIUSAuth;
import com.citrix.wi.config.auth.TwoFactorAuthMethod;
import com.citrix.wi.config.auth.AGAuthPoint;
import com.citrix.wi.config.auth.WIAuthPoint;
import com.citrix.wi.mvc.StatusMessage;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.types.AGAuthenticationMethod;
import com.citrix.wi.types.WIAuthType;
import com.citrix.wi.util.AppAttributeKey;
import com.citrix.wi.util.Platform;
import com.citrix.wing.util.Strings;
import com.citrix.wi.types.MPSClientType;
import com.citrix.wing.types.AppAccessMethod;


/**
 * Utility methods to assist with checking the configuration.
 */
public class CheckConfig {

    private static final String CONFIG_VALID_REQUEST_CONTEXT_KEY = "com.citrix.wi.Configuration.isValid";
    private static final String CONFIG__REQUEST_CONTEXT_KEY      = "com.citrix.wi.Configuration";

    /**
     * Check whether the configuration is valid. If not write a suitable
     * response using the WebAbstraction.
     *
     * @param wiContext - WIContext object for the current request
     * @return true if a configuration is valid, false otherwise
     */
    public static boolean isValidConfiguration(WIContext wiContext) {
        WIConfiguration config = wiContext.getConfiguration();
        WebAbstraction web = wiContext.getWebAbstraction();

        boolean isValid = true;

        // only need to check this once per request
        // validate configuration can contain file IO.
        Object validObj = web.getRequestContextAttribute(CONFIG_VALID_REQUEST_CONTEXT_KEY);
        if (validObj != null) {

            // use the cached value
            isValid = ((Boolean)validObj).booleanValue();

        } else {

            // do the actual validation
            if (!Include.getLanguageManager(wiContext.getStaticEnvironmentAdaptor()).isLanguageAvailable(
                            config.getDefaultLocale())) {
                isValid = false;
            } else {
                StatusMessage validationError = validateConfiguration(config, web);
                if (validationError != null) {
                    isValid = false;
                    // Log the reason, if one is provided
                    if (!Strings.isEmpty(validationError.getDisplayMessageKey())) {
                        wiContext.log(validationError.getType(), validationError.getDisplayMessageKey());
                    }
                }
            }

            // cache the value for this request
            web.setRequestContextAttribute(CONFIG_VALID_REQUEST_CONTEXT_KEY, new Boolean(isValid));
        }

        return isValid;
    }

    /**
     * Check whether a valid configuration exists. If no configuration is found,
     * the response is set appropriately and false returned. The caller should
     * exit from the request handling in this case.
     *
     * @param web - WebAbstraction for the current request
     * @return true if a configuration exists, false otherwise
     */
    public static boolean checkConfiguration(WebAbstraction web) {
        boolean valid = (getConfiguration(web) != null);
        if (!valid) {
            doMissingConfigurationResponse(web);
        }
        return valid;
    }

    /**
     * Set the response appropriately for the case where no configuration is
     * found
     */
    public static void doMissingConfigurationResponse(WebAbstraction web) {
        web.clientRedirectToUrl("../html/noConfiguration.html");
    }

    /**
     * Get the configuration
     */
    public static WIConfiguration getConfiguration(WebAbstraction web) {
        // Cache this per-request to ensure the configuration
        // doesn't change during a request
        // Also, we cache the result of validating it
        WIConfiguration configuration = (WIConfiguration)web.getRequestContextAttribute(CONFIG__REQUEST_CONTEXT_KEY);
        if (configuration == null) {
            // get the configuration
            ConfigurationProvider configProvider = (ConfigurationProvider)web
                            .getApplicationAttribute(AppAttributeKey.CONFIGURATION_PROVIDER);
            if (configProvider != null) {
                configProvider.setSiteInfo(web.getBaseURL(), web.getApplicationPath());
                configuration = configProvider.getWIConfiguration();
            }
            // cache configuration in the request context
            web.setRequestContextAttribute(CONFIG__REQUEST_CONTEXT_KEY, configuration);
        }
        return configuration;
    }

    /**
     * Checks for parts of the configuration that are not supported by this
     * implementation of Web Interface. Specifically: ASP: -- two factor
     * authentication file exists JSP: -- NDS login is not supported --
     * Smartcard, Single Sign On, and Smartcard Single Sign On are not supported
     *
     * @param configuration the WI configuration
     */
    public static StatusMessage validateConfiguration(WIConfiguration wiConfig, WebAbstraction web) {

        AuthenticationConfiguration authConfig = wiConfig.getAuthenticationConfiguration();

        if (authConfig == null) {
            return new StatusMessage("NoAuthenticationMethods");
        }

        if (authConfig.getAuthPoint() instanceof WIAuthPoint) {
            WIAuthPoint wiAuthPoint = (WIAuthPoint)authConfig.getAuthPoint();

            if (wiAuthPoint.isAuthMethodEnabled(WIAuthType.ANONYMOUS) && wiConfig.isRoamingUserEnabled()) {
                return new StatusMessage("AnonymousAuthAndRoamingUserEnabled");
            }

            if (wiAuthPoint.isAuthMethodEnabled(WIAuthType.EXPLICIT)) {
                AuthMethod explicitMethod = wiAuthPoint.getAuthMethod(WIAuthType.EXPLICIT);

                // Check for NDS
                if (!Features.isNDSSupported() && (explicitMethod instanceof ExplicitNDSAuth)) {
                    return new StatusMessage("NDSUnsupported");
                }

                TwoFactorAuthMethod twoFactorMethod = explicitMethod.getTwoFactorAuth();
                if (twoFactorMethod != null) {
                    if (Platform.isDotNet() && !web.contextPathExists(aspTwoFactorPage(twoFactorMethod))) {
                        return new StatusMessage("2FactorConfigError");
                    } else if (Platform.isJava() && !(twoFactorMethod instanceof RADIUSAuth)) {
                        // Anything other than RADIUS is not allowed
                        return new StatusMessage("2FactorConfigError");
                    }
                }
            }

            // Check for Smartcard, Single Sign On, and Smartcard Single Sign On
            if (!Features.isSSOSupported()
                            && (wiAuthPoint.isAuthMethodEnabled(WIAuthType.SINGLE_SIGN_ON)
                                            || wiAuthPoint.isAuthMethodEnabled(WIAuthType.CERTIFICATE) || wiAuthPoint
                                            .isAuthMethodEnabled(WIAuthType.CERTIFICATE_SINGLE_SIGN_ON))) {
                return new StatusMessage("SSOUnsupported");
            }
        } else if (authConfig.getAuthPoint() instanceof AGAuthPoint) {
            AGAuthPoint agAuthPoint = (AGAuthPoint)authConfig.getAuthPoint();
            if (Platform.isJava() && (agAuthPoint.getAuthenticationMethod() != AGAuthenticationMethod.EXPLICIT)) {
                return new StatusMessage("SSOUnsupported");
            }
        }

		if (wiConfig.getIcaFileSigningEnabled())
		{
			if (Platform.isJava())
			{
				return new StatusMessage("NoIFSForJSP");
			}

			if (wiConfig.getClientDeploymentConfiguration().getLegacyClientSupportEnabled())
			{
				return new StatusMessage("NoIFSForLegacyClientSupport");
			}

			if (wiConfig.getAppAccessMethodConfiguration().isEnabledAppAccessMethod(AppAccessMethod.STREAMING))
			{
				return new StatusMessage("NoIFSForStreamingOnlySite");
			}

			if (!wiConfig.getClientDeploymentConfiguration().isEnabledClient(MPSClientType.LOCAL_ICA))
			{
				return new StatusMessage("NoIFSForNonNativeClient");
			}
		}

        return null; // ie no error status message
    }

    private static String aspTwoFactorPage(TwoFactorAuthMethod authMethod) {
        return "auth/" + authMethod.getName().toLowerCase() + ".aspx";
    }

}
