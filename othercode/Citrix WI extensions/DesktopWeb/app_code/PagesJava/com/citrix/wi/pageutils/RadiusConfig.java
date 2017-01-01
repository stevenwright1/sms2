/*
 * Copyright (c) 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils;

import java.io.BufferedReader;
import java.io.InputStream;
import java.io.IOException;

import com.citrix.authenticators.AuthenticatorInitializationException;
import com.citrix.authenticators.RADIUSAuthenticatorFactory;
import com.citrix.radius.RadiusConfiguration;
import com.citrix.wi.config.ConfigurationException;
import com.citrix.wi.config.auth.ExplicitAuth;
import com.citrix.wi.config.auth.RADIUSAuth;
import com.citrix.wi.config.auth.WIAuthPoint;
import com.citrix.wi.config.auth.TwoFactorAuthMethod;
import com.citrix.wi.types.WIAuthType;
import com.citrix.wi.util.Platform;
import com.citrix.wi.util.UTF8InputStreamReader;
import com.citrix.wing.MessageType;
import com.citrix.wing.StaticEnvironmentAdaptor;
import com.citrix.wing.util.LocalizableString;
import com.citrix.wing.util.ResourceBundleFactory;
import com.citrix.wing.util.Strings;


/**
 * Class responsible for configuring RADIUS authentication factory.
 */
public class RadiusConfig {

    // Tells if RADIUS configuration is valid (lack of configuration is also valid!).
    private static boolean configurationValid = true;

    /**
     * Updates RADIUS authentication factory with data from configuration files.
     * 
     * @param wiAuthPoint wi authentication point
     * @param bundleFactory resource bundle factory needed for localization
     * @param staticEnvAdaptor the environment adaptor
     * @param appFileSystemPath physical application path
     */
    public static void updateAuthFactory(WIAuthPoint wiAuthPoint, ResourceBundleFactory bundleFactory,
                                         StaticEnvironmentAdaptor staticEnvAdaptor,
                                         String appFileSystemPath) {
        // Mark configuration as valid, which means either:
        // - there is no radius configuration - mark as success and do nothing, as radius settings won't be used anyway
        // - there is radius configuration - set parsingSuccess to true if configuration is valid, false otherwise
        configurationValid = true;

        // If RADIUS 2-factor authentication is configured, then update the
        // RADIUSAuthenticatorFactory (which is used to create RADIUSAuthenticator
        // objects) with the WIConfiguration settings.
        if (wiAuthPoint != null) {
            ExplicitAuth expAuth = (ExplicitAuth)wiAuthPoint.getAuthMethod(WIAuthType.EXPLICIT);
            if (expAuth != null) {
                TwoFactorAuthMethod twoFactorSettings = expAuth.getTwoFactorAuth();
                if (twoFactorSettings instanceof RADIUSAuth) {
                    configurationValid = configureRadius((RADIUSAuth)twoFactorSettings, bundleFactory, staticEnvAdaptor, appFileSystemPath);
                }
            }
        }
    }

    /**
     * Tells whether RADIUS configuration is valid.
     * @returns true if configuration is valid or non-existent, false if config is invalid
     */
    public static boolean isConfigurationValid() {
        return configurationValid;
    }

    private static String getLogFilePath(StaticEnvironmentAdaptor staticEnvAdaptor, String appFileSystemPath) {
        String appConfigPath = appFileSystemPath
                                + staticEnvAdaptor.getConfigurationValue("RESOURCE-ROOT:Config")
                                + staticEnvAdaptor.getConfigurationValue("RADIUS_SECRET_PATH");

        appConfigPath = appConfigPath.replace('\\', '/');
        // get rid of duplicate slashes, not to show paths like //conf//WI
        appConfigPath = Strings.replace(appConfigPath, "//", "/");

        // for ASP, convert to backslashes
        if (!Platform.isJava()) {
            appConfigPath = appConfigPath.replace('/', '\\');
        }

        return appConfigPath;
    }

    private static boolean configureRadius(RADIUSAuth radiusAuthSettings, ResourceBundleFactory bundleFactory,
                                                              StaticEnvironmentAdaptor staticEnvAdaptor,
                                                              String appFileSystemPath) {
        final String LOG_PATH = getLogFilePath(staticEnvAdaptor, appFileSystemPath);

        try {
            RadiusConfiguration radiusConfig = radiusAuthSettings.toRadiusConfiguration();

            if (radiusConfig != null) {
                // Get the shared secret
                String radiusSecretPath = staticEnvAdaptor.getConfigurationValue("RADIUS_SECRET_PATH");
                InputStream inst = staticEnvAdaptor.getResourceStream("Config", radiusSecretPath);

                if (inst != null) {
                    BufferedReader reader = new BufferedReader(new UTF8InputStreamReader(inst));
                    String secret = reader.readLine();
                    inst.close();
                    if (secret == null) {
                        throw new AuthenticatorInitializationException("SecretEmpty");
                    }
                    radiusConfig.setSecret(secret);
                } else {
                    throw new AuthenticatorInitializationException("SecretReadError");
                }

                // Get the NAS-Identifier
                String nasIdentifier = staticEnvAdaptor.getConfigurationValue("RADIUS_NAS_IDENTIFIER");

                // RFC 2865 requires NAS-Identifier to have minimum lenght of 3
                boolean validNasId = (nasIdentifier != null && nasIdentifier.length() >= 3);

                // Get the NAS-IP-Address
                String ipAddressStr = staticEnvAdaptor.getConfigurationValue("RADIUS_NAS_IP_ADDRESS");
                byte[] ipAddress = Strings.parseIpAddress(ipAddressStr);
                boolean validIpAddress = (ipAddress != null);

                if (!validNasId && !validIpAddress) {
                    throw new AuthenticatorInitializationException("NASIdentifierOrIPAddressRequired");
                }

                if (validNasId) {
                    radiusConfig.setNasIdentifier(nasIdentifier);
                }

                if (validIpAddress) {
                    radiusConfig.setNasIpAddress(ipAddress);
                }

                RADIUSAuthenticatorFactory staticRADIUSAuthenticatorFactory =
                    (RADIUSAuthenticatorFactory)staticEnvAdaptor.getApplicationAttribute(TwoFactorAuth.VAL_RADIUS_AUTHENTICATOR_FACTORY);
                if (staticRADIUSAuthenticatorFactory == null) {
                    staticRADIUSAuthenticatorFactory = new RADIUSAuthenticatorFactory();
                    staticEnvAdaptor.setApplicationAttribute(TwoFactorAuth.VAL_RADIUS_AUTHENTICATOR_FACTORY, staticRADIUSAuthenticatorFactory);
                }
                staticRADIUSAuthenticatorFactory.configure(radiusConfig);

                // configuration is ok
                return true;
            }
        } catch (ConfigurationException ce) {
            // Log the configuration load/parse error and a stack trace.
            staticEnvAdaptor.getDiagnosticLogger().log(
                    MessageType.ERROR,
                    new LocalizableString(bundleFactory, ce.getMessageKey(), ce.getMessageArguments()),
                    ce);
        } catch (AuthenticatorInitializationException aie) {
            // Log the exception with a stack trace.
            staticEnvAdaptor.getDiagnosticLogger().log(
                    MessageType.ERROR,
                    new LocalizableString(bundleFactory, aie.getMessageKey(), LOG_PATH),
                    aie);
        } catch (IOException ioe) {
            // Log the exception and a stack trace.
            staticEnvAdaptor.getDiagnosticLogger().log(
                    MessageType.ERROR,
                    new LocalizableString(bundleFactory, "SecretReadError", LOG_PATH),
                    ioe);
        }

        // if we got here, it means the exception was thrown and config is invalid
        return false;
    }

}
