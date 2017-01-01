/*
 * Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils;

import com.citrix.wi.config.ConfigurationKeys;
import com.citrix.wi.mvc.WIContext;

/**
 * This class contains utility functions related to localised text
 */
public class LocalisedText {

    /**
     * Returns the customized welcome message for the Messages screen from
     * the LocalisedTextConfiguration.
     *
     * First it tries to get a message appropriate to the given locale
     * If this is not possible, it will attempt to retrive the default customized message
     *
     * @param wiContext the WI context
     * @return the string to display in the footer.
     */
    public static String getFooterText(WIContext wiContext) {
        return wiContext.getConfiguration().getLocalisedTextConfiguration().getLocalizedFooterMessage(wiContext.getCurrentLocale());
    }

    /**
     * Returns the customized title for the Login screen from
     * the LocalisedTextConfiguration.
     *
     * First it tries to get a title appropriate to the given locale
     * If this is not possible, it will attempt to retrive the default customized title
     *
     * @param wiContext the WI context
     * @return the title as a string, or null if no customized message has
     * been defined.
     */
    public static String getLoginTitle(WIContext wiContext) {
        return wiContext.getConfiguration().getLocalisedTextConfiguration().getLocalizedLoginTitle(wiContext.getCurrentLocale());
    }

    /**
     * Returns the customized welcome message for the Login screen from
     * the LocalisedTextConfiguration.
     *
     * First it tries to get a message appropriate to the given locale
     * If this is not possible, it will attempt to retrive the default customized message
     *
     * @param wiContext the WI context
     * @return the message as a string, or null if no customized message has
     * been defined.
     */
    public static String getLoginWelcomeMessage(WIContext wiContext) {
        return wiContext.getConfiguration().getLocalisedTextConfiguration().getLocalizedWelcomeMessage(wiContext.getCurrentLocale());
    }

    /**
     * Returns the customized system message for the Login screen from
     * the LocalisedTextConfiguration.
     *
     * First it tries to get a message appropriate to the given locale
     * If this is not possible, it will attempt to retrive the default customized message
     *
     * @param wiContext the WI context
     * @return the message as a string, or null if no customized message has
     * been defined.
     */
    public static String getLoginSysMessage(WIContext wiContext) {
        return wiContext.getConfiguration().getLocalisedTextConfiguration().getLocalizedLoginSysMessage(wiContext.getCurrentLocale());
    }

    /**
     * Returns the customized welcome message for the AccessPoint screen from
     * the LocalisedTextConfiguration.
     *
     * First it tries to get a message appropriate to the given locale
     * If this is not possible, it will attempt to retrive the default customized message
     *
     * @param wiContext the WI context
     * @return the message as a string, or null if no customized message has
     * been defined.
     */
    public static String getAppWelcomeMessage(WIContext wiContext) {
        return wiContext.getConfiguration().getLocalisedTextConfiguration().getLocalizedAppWelcomeMessage(wiContext.getCurrentLocale());
    }

    /**
     * Returns the customized system message for the AccessPoint screen from
     * the LocalisedTextConfiguration.
     *
     * First it tries to get a message appropriate to the given locale
     * If this is not possible, it will attempt to retrive the default customized message
     *
     * @param wiContext the WI context
     * @return the message as a string, or null if no customized message has
     * been defined.
     */
    public static String getAppSysMessage(WIContext wiContext) {
        return wiContext.getConfiguration().getLocalisedTextConfiguration().getLocalizedAppSysMessage(wiContext.getCurrentLocale());
    }

    /**
    * Returns the customized title text for the Pre-Login Message Button
    *
    * @param wiContext the WI context
    * @return the message as a string, or null if no customized message has
    * been defined.
    */
    public static String getPreLoginMessageButton(WIContext wiContext) {
        return wiContext.getConfiguration().getLocalisedTextConfiguration().getLocalizedPreLoginMessageButton(wiContext.getCurrentLocale());
    }

    /**
     * Returns the customized title text for the Pre-Login Message Text
     *
     * @param wiContext the WI context
     * @return the message as a string, or null if no customized message has
     * been defined.
     */
    public static String getPreLoginMessageText(WIContext wiContext) {
        return wiContext.getConfiguration().getLocalisedTextConfiguration().getLocalizedPreLoginMessageText(wiContext.getCurrentLocale());
    }

    /**
     * Returns the customized title text for the Pre-Login Message Title
     *
     * @param wiContext the WI context
     * @return the message as a string, or null if no customized message has
     * been defined.
     */
    public static String getPreLoginMessageTitle(WIContext wiContext) {
        return wiContext.getConfiguration().getLocalisedTextConfiguration().getLocalizedPreLoginMessageTitle(wiContext.getCurrentLocale());
    }

    /**
     * Gets the localised title for a given tab
     * @param wiContext the WI context
     * @param tabName the internal ID for the tab
     * @return the localised tab title as a string, or null if no title has been defined for the given tab
     */
    public static String getAccessPointTabTitle( WIContext wiContext, String tabName ) {
        return wiContext.getConfiguration().getLocalisedTextConfiguration().getLocalizedTabTitle( ConfigurationKeys.ConfKey_AppTabPrefix + tabName, wiContext.getCurrentLocale() );
    }
}
