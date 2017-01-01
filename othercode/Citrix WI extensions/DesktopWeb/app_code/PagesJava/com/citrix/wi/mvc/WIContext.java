package com.citrix.wi.mvc;

import java.io.IOException;
import java.util.Locale;
import java.util.MissingResourceException;

import com.citrix.wi.UserPreferences;
import com.citrix.wi.config.WIConfiguration;
import com.citrix.wi.util.ClientInfo;
import com.citrix.wing.MessageType;
import com.citrix.wing.StaticEnvironmentAdaptor;
import com.citrix.wing.UserEnvironmentAdaptor;
import com.citrix.wing.util.LocalizableString;
import com.citrix.wing.util.ResourceBundleFactory;
import com.citrix.wi.util.AppAttributeKey;

/**
 * Summary description for WIContext.
 *
 * The methods are all final for performance reasons.
 */
public final class WIContext {
    private final WIConfiguration configuration;
    private final WebAbstraction webAbstraction;
    private final UserEnvironmentAdaptor userEnvAdaptor;
    private final StaticEnvironmentAdaptor staticEnvAdaptor;
    private final ClientInfo clientInfo;
    private final UserPreferences userPrefs;
    private Locale currentLocale; // this can be changed
    private final PlatformSpecificUtils utils;

    public WIContext(WIConfiguration configuration,
                     WebAbstraction webAbstraction,
                     UserEnvironmentAdaptor userEnvAdaptor,
                     StaticEnvironmentAdaptor staticEnvAdaptor,
                     ClientInfo clientInfo,
                     UserPreferences userPrefs,
                     Locale currentLocale,
                     PlatformSpecificUtils utils
                     ) {
        if ((configuration == null)
            || (webAbstraction == null)
            || (userEnvAdaptor == null)
            || (staticEnvAdaptor == null)
            || (clientInfo == null)
            || (userPrefs == null)
            || (currentLocale == null)
            || (utils == null) ){
            throw new IllegalArgumentException("WIContext cannot be constructed with null constituents");
        }
        this.configuration = configuration;
        this.webAbstraction = webAbstraction;
        this.userEnvAdaptor = userEnvAdaptor;
        this.staticEnvAdaptor = staticEnvAdaptor;
        this.clientInfo = clientInfo;
        this.userPrefs = userPrefs;
        this.currentLocale = currentLocale;
        this.utils = utils;
    }

    public final WIConfiguration getConfiguration() {
        return configuration;
    }

    public final WebAbstraction getWebAbstraction() {
        return webAbstraction;
    }

    public final UserEnvironmentAdaptor getUserEnvironmentAdaptor() {
        return userEnvAdaptor;
    }

    public final StaticEnvironmentAdaptor getStaticEnvironmentAdaptor() {
        return staticEnvAdaptor;
    }

    public final ClientInfo getClientInfo() {
        return clientInfo;
    }

    public final UserPreferences getUserPreferences() {
        return userPrefs;
    }

    public final PlatformSpecificUtils getUtils() {
        return utils;
    }

    public final Locale getCurrentLocale() {
        return currentLocale;
    }

    public final Locale getDefaultLocale() {
        return configuration.getDefaultLocale();
    }

    // This is the only mutable member because it does not have an extra level of redirection like UserPreferences
    public final void setCurrentLocale(Locale currentLocale) {
        if (currentLocale == null) {
            throw new IllegalArgumentException("WIContext cannot be constructed with null constituents");
        }
        this.currentLocale = currentLocale;
    }

    /**
     * This gets a hash of the current local's strings.
     * This is used to ensure the client is informed
     * when the style sheet changes
     */
    public final String getResourceBundleCryptoHash() throws IOException{
        return getBundleFactory().getResourceBundleCryptoHash(getCurrentLocale());
    }

    /**
     * Gets a localized string from the site's resource bundle.
     *
     * @param key The key of the desired string.
     * @return The string corresponding to the given key in the resource bundle.
     */
    public final String getString(String key, Locale locale) {
        return getString(new LocalizableString(getBundleFactory(), key), locale);
    }

    /**
     * Gets a localized string from the site's resource bundle,
     * using the current locale.
     * @param key The key of the desired string.
     * @return The string corresponding to the given key in the resource bundle.
     */
    public final String getString(String key) {
        return getString(key, getCurrentLocale()) ;
    }

    /**
     * Gets a localized string from the site's resource bundle, populated with
     * the given arguments.
     *
     * @param key The key of the desired string.
     * @param args An array of arguments to insert into the localized string.
     * @return The string corresponding to the given key, with the given arguments
     * inserted where appropriate.
     */
    public final String getString(String key, Object[] args, Locale locale) {
        return getString(new LocalizableString(getBundleFactory(), key, args), locale);
    }

    /**
     * Gets a localized string from the site's resource bundle, populated with
     * the given arguments, and using the default locale
     *
     * @param key The key of the desired string.
     * @param args An array of arguments to insert into the localized string.
     * @return The string corresponding to the given key, with the given arguments
     * inserted where appropriate.
     */
    public final String getString(String key, Object[] args) {
        return getString(key, args, getCurrentLocale());
    }

    /**
     * Gets a localized string from the site's resource bundle, populated with
     * the given argument.
     *
     * This method exists for better code sharing between platforms.
     *
     * @param key The key of the desired string.
     * @param arg The argument to insert into the localized string.
     * @return The string corresponding to the given key, with the given argument
     * inserted where appropriate.
     */
    public final String getString(String key, String arg) {
        return getString(key, new Object[] { arg });
    }

    /**
     * Gets a localized string from the site's resource bundle, populated with
     * the given arguments.
     *
     * This method exists for better code sharing between platforms.
     *
     * @param key The key of the desired string.
     * @param arg1 The first argument to insert into the localized string.
     * @param arg2 The second argument to insert into the localized string.
     * @return The string corresponding to the given key, with the given arguments
     * inserted where appropriate.
     */
    public final String getString(String key, String arg1, String arg2) {
        return getString(key, new Object[] { arg1, arg2 });
    }

    /**
     * Log an error message through the static
     * environment adaptor's diagnostic logger.
     *
     * @return The event ID the message was logged under.
     */
    public final String log(MessageType msgType, String logMessageKey) {
        return log(msgType, logMessageKey, null);
    }

    /**
     * Log an error message through the static
     * environment adaptor's diagnostic logger.
     *
     * @return The event ID the message was logged under.
     */
    public final String log(MessageType msgType, String logMessageKey, Object[] logMessageArgs) {
        StaticEnvironmentAdaptor adaptor = (StaticEnvironmentAdaptor)getWebAbstraction().getApplicationAttribute(AppAttributeKey.STATIC_ADAPTOR);
        return adaptor.getDiagnosticLogger().log(msgType, new LocalizableString(getBundleFactory(), logMessageKey, logMessageArgs));
    }

    /**
     * Gets a localized string from the site's resource bundle.
     *
     * Base method that contains all error handling for the UI
     * string localization methods.
     *
     * If the key contained in the LocalizableString cannot be
     * found in the resource bundle try to get a localized error
     * message.  If this fails display an english error message.
     *
     * @param str the LocalizableString to localize
     *
     * @return the string corresponding to the given localizable string in the resource bundle
     *
     */
    private String getString(LocalizableString str, Locale locale) {
        try {
            return str.toString(locale);
        } catch (MissingResourceException mre) {
            // Cannot find the requested resource, try to find the "UnknownString" message.
            try {
                return (new LocalizableString(getBundleFactory(), "UnknownString")).toString(locale);
            } catch (MissingResourceException innerMre) {
                // Cannot find the localized "UnknownString" message.
                // Display a hard-coded English message.
                return "The \"UnknownString\" message could not be found in the resource bundle.";
            }
        }
    }

    private ResourceBundleFactory getBundleFactory() {
        return (ResourceBundleFactory)getStaticEnvironmentAdaptor().getApplicationAttribute(AppAttributeKey.RESOURCE_BUNDLE_FACTORY);
    }

}
