/*
 * Copyright (c) 2007-2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils;

import java.util.ResourceBundle;
import java.util.Enumeration;
import java.util.ArrayList;
import java.util.Random;

import com.citrix.wing.util.ResourceBundleFactory;
import com.citrix.wing.util.WebUtilities;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.UserPreferences;
import com.citrix.wi.types.AllowChangePassword;
import com.citrix.wi.types.UserInterfaceBranding;
import com.citrix.wi.config.WIConfiguration;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.util.AppAttributeKey;

/**
 * Utility class to generate a hint for the AccessPoint screen.
 */
public class Hints {
    // All the keys to hint messages
    private static String[] hintKeys = null;

    // Session variable used to store the index in the hintKeys array of the
    // last hint displayed.
    public static final String SV_LAST_HINT_INDEX = "CTX_Last_Hint_Index";

    // Page tags used in the hints strings to indicate that a page reference should be set.
    public static final String CHANGE_PASSWORD_PAGE_TAG = "{TAG_CHANGE_PASSWORD_PAGE}";
    public static final String SETTINGS_PAGE_TAG = "{TAG_SETTINGS_PAGE}";

    // Built-in Hint key strings
    private static final String CHANGE_PASSWORD_DIRECT_HINT = "Hint_ChangePasswordDirect";
    private static final String CHANGE_VIEW_HINT = "Hint_ChangeView";
    private static final String DISCONNECT_HINT = "Hint_Disconnect";
    private static final String DRAG_DROP_HINT = "Hint_DragDrop";
    private static final String RECONNECT_HINT = "Hint_Reconnect";
    private static final String REMEMBER_LAST_FOLDER_HINT = "Hint_RememberLastFolder";
    private static final String SEARCH_HINT = "Hint_Search";
    private static final String SPECIAL_FOLDER_REDIRECTION_HINT = "Hint_SpecialFolderRedirection";

    /**
     * Get a localized hint. Only hints which make sense given the site configuration and the
     * user's preferences are returned.
     *
     * @param wiContext the WI context
     * @param currentTab a string representing the currently selected tab of the AccessPoint screen
     * @return a hint or null if there is no suitable hint.
     */
    public static String getHint(WIContext wiContext, String currentTab) {
        String hint = null;
        String hintKey = pickHintKey(wiContext, currentTab);

        // Some hints need parameters...

        if (hintKey != null) {
            hint = wiContext.getString(hintKey);
        }

        if (hint != null) {
            // Replace page Url place holders tags in the string for the real page reference.
            hint = ReplacePageTags(hint);

            // Put in prefix "Hint: "
            hint = wiContext.getString("HintPrefix", hint);
        }

        return hint;
    }

    /**
     * Replace tag placeholders for page's with the correct page Url as it may be .jsp or .aspx
     */
    private static String ReplacePageTags(String hint)
    {
        hint = com.citrix.wing.util.Strings.replace(hint, SETTINGS_PAGE_TAG, Constants.PAGE_PREFERENCES);
        hint = com.citrix.wing.util.Strings.replace(hint, CHANGE_PASSWORD_PAGE_TAG, Constants.PAGE_CHANGE_PASSWD);
        
        return hint;
    }

    /**
     * Select a hint key from the pool of possible hints.
     */
    private static String pickHintKey(WIContext wiContext, String currentTab) {
        initializeHints(wiContext);
        String hintKey = null;

        if (hintKeys.length == 0) {
            hintKey = null;
        } else {

            int index = getLastHintIndex(wiContext) + 1; // go round robin

            for (int offset = 0; offset < hintKeys.length; offset++) {
                int newIndex = (index + offset) % hintKeys.length;
                if (canShow(wiContext, currentTab, hintKeys[newIndex])) {
                    // this hint is appropriate; use it
                    hintKey = hintKeys[newIndex];
                    setLastHintIndex(wiContext, newIndex);
                    break;
                }
            }
        }

        return hintKey;
    }

    /**
     * Load up all the hint keys. The hint keys are defined as those strings
     * starting with the prefix "Hint_" which are defined in the site's default locale.
     */
    private static void initializeHints(WIContext wiContext) {

        if (hintKeys == null) {
            ResourceBundleFactory resourceBundleFactory = (ResourceBundleFactory)wiContext.getStaticEnvironmentAdaptor().getApplicationAttribute(AppAttributeKey.RESOURCE_BUNDLE_FACTORY);
            Enumeration keys = null;
            try {
                if (resourceBundleFactory != null) { // keeps the unit tests happy
                    ResourceBundle resourceBundle = resourceBundleFactory.getResourceBundle(wiContext.getConfiguration().getDefaultLocale());
                    keys = resourceBundle.getKeys();
                }
            } catch (java.io.IOException ignored) {
            }

            ArrayList hintKeysList = new ArrayList(10);

            if (keys != null) {
                while (keys.hasMoreElements()) {
                    String key = (String)keys.nextElement();
                    if ((key != null) && key.startsWith("Hint_")) {
                        hintKeysList.add(key);
                    }
                }
            }

            Hints.hintKeys = (String[])hintKeysList.toArray(new String[0]);
        }
    }

    /**
     * Retrieve the index of the last hint which was displayed. If we are at
     * the start of a session, a random index is generated.
     */
    private static int getLastHintIndex(WIContext wiContext) {
        int result;
        Integer indexObj = (Integer)wiContext.getWebAbstraction().getSessionAttribute(SV_LAST_HINT_INDEX);
        if(indexObj == null) {
            Random random = new Random();
            result = random.nextInt(hintKeys.length);
        } else {
            result = indexObj.intValue();
        }

        return result;
    }

    /**
     * Set the index of the last hint which was displayed.
     */
    private static void setLastHintIndex(WIContext wiContext, int index) {
        wiContext.getWebAbstraction().setSessionAttribute(SV_LAST_HINT_INDEX, new Integer(index));
    }

    /**
     * Decide whether a hint should be shown based on the site configuration and
     * user preferences.
     *
     * By default, any localization string with a key starting "Hint_" will be shown, unless
     * code in this method disallows it.
     *
     * @return <code>true</code> if the hint should be shown, otherwise <code>false</code>
     */
    private static boolean canShow(WIContext wiContext, String currentTab, String hintKey) {

        boolean showHint = true; // default to showing the hint

        UserPreferences userPrefs = wiContext.getUserPreferences();
        WIConfiguration config = wiContext.getConfiguration();

        if (Hints.DISCONNECT_HINT.equals(hintKey)) {
            showHint = Settings.getShowDisconnectLink(wiContext);
        } else if (Hints.RECONNECT_HINT.equals(hintKey)) {
            showHint = Settings.getShowReconnectLink(wiContext);
        } else if (Hints.DRAG_DROP_HINT.equals(hintKey)) {
            showHint = config.getEnablePassthroughURLs();
        } else if (Hints.CHANGE_PASSWORD_DIRECT_HINT.equals(hintKey)) {
            showHint = canShowChangePasswordHint(wiContext, config);
        } else if (Hints.SEARCH_HINT.equals(hintKey)) {
            // Only show this hint if searching is enabled
            showHint = !Boolean.FALSE.equals(userPrefs.getShowSearch());
        } else if (Hints.REMEMBER_LAST_FOLDER_HINT.equals(hintKey)) {
            // Only show if the user can change this setting
            showHint =
                config.getAllowCustomizeSettings()
                && config.getUIConfiguration().getAllowCustomizePersistFolderLocation();
        } else if (Hints.CHANGE_VIEW_HINT.equals(hintKey)) {
            showHint =
                Settings.getShowChangeViewDropdown(wiContext)
                && canShowChangeViewOptionsHint(wiContext, currentTab);
        } else if (Hints.SPECIAL_FOLDER_REDIRECTION_HINT.equals(hintKey)) {
            // Show only if customise special folder, redirect special folder
            // and allow customize settings are enables.
            showHint  =
                config.getClientConnectionConfiguration().getAllowCustomizeSpecialFolderRedirection()
                && Boolean.FALSE.equals(userPrefs.getSpecialFolderRedirectionEnabled())
                && config.getAllowCustomizeSettings();
        }

        return showHint;
    }

    /**
     * Determine if the Change Password hint should be shown.
     *
     * This hint is used when the user can change their password.
     *
     * @return <code>true</code> if the hint should be shown, otherwise <code>false</code>
     */
    private static boolean canShowChangePasswordHint(WIContext wiContext, WIConfiguration config)
    {
        boolean showHint = Authentication.getChangePasswordPolicy(wiContext) == AllowChangePassword.ALWAYS;
        return showHint;
    }

    /**
     * Determine if the Change View options should be shown or not.
     *
     * We do not show change view options for
     * 1.Desktops tabs(for both XA and XD branded sites)
     * 2.Search tabs
     * 3.AllResources tabs for XD branded sites.
     *
     * @return <code>true</code> if the change view options should be shown, otherwise <code>false</code>
     */
    private static boolean canShowChangeViewOptionsHint(WIContext wiContext, String currentTab) {
        boolean showChangeViewHint = true;
        if (Constants.TAB_NAME_DESKTOPS.equals(currentTab) ||
           (Include.getSiteBranding(wiContext) == UserInterfaceBranding.DESKTOPS && Constants.TAB_NAME_ALL_RESOURCES.equals(currentTab))) {
               showChangeViewHint = false;
        }
        return showChangeViewHint;
    }
}
