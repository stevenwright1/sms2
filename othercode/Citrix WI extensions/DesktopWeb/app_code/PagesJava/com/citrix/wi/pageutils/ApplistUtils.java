/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

import com.citrix.wi.IconCache;
import com.citrix.wi.UserPreferences;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.util.ClientInfo;
import com.citrix.wi.util.ClientInfoUtilities;
import com.citrix.wing.Icon;
import com.citrix.wing.types.FolderResourceTypeFlags;
import com.citrix.wing.util.CtxArrays;
import com.citrix.wing.util.WebUtilities;
import com.citrix.wing.webpn.AccessMethod;
import com.citrix.wing.webpn.ResourceInfo;
import com.citrix.wing.webpn.UserContext;

/**
 * Utility methods to support the application list page.
 */
public class ApplistUtils {

    private static Icon getPreferredIcon(WIContext wiContext, ResourceInfo resource, int appIconSize) {
        // Start off with the legacy 4bpp icon
        Icon preferredIcon = resource.getIcon();

        // For environments that are known support PNG, look for a better 32bpp icon
        // Environments support png natively, or require the directX hack
        if (ClientInfoUtilities.isPNGFullySupported(wiContext.getClientInfo()) ||
            ClientInfoUtilities.requiresDirectXFilterToShowPngAlpha(wiContext.getClientInfo()) ||
            isPngSupportedByCustomWhiteList(wiContext.getClientInfo())) {
            Icon[] availableIcons = resource.getMultiImageIcon();
            if (availableIcons != null) {
                for (int i = 0; i < availableIcons.length; i++) {
                    Icon icon = availableIcons[i];
                    // If displaying size 16 icons, look for a size 16 high-color icon (preferred over scaling a size 32 icon).
                    if ((appIconSize == Constants.ICON_SIZE_16) && (icon.getSize() == 16) && (icon.getColorDepth() == 32)) {
                        preferredIcon = icon;
                        break;
                    } else if ((icon.getSize() == 32) && (icon.getColorDepth() == 32)) {
                        preferredIcon = icon;
                        // If displaying size 32 icons, all done; otherwise (size 16), keep looking for a size 16 icon to use in
                        // preference to scaling the size 32 high-color icon.
                        if (appIconSize == Constants.ICON_SIZE_32) {
                            break;
                        }
                    }
                }
            }
        }
        return preferredIcon;
    }

    /**
     * Produces the icon markup, and optionally forces the height and width of the image.
     *
     * @param wiContext the Web Interface Context object
     * @param resource the <code>ResourceInfo</code> object to produce the icon for
     * @param appIconSize the size of the icon as an integer - used to select the appropriate image size
     * @param isTreeView whether tree view is the current view
     * @param forceAppSize whether to force the size of the image
     * @param showDescription whether to show the app description in the tooltip rather than the display name. Leave null for default behaviour
     * @return the HTML markup for the resource icon
     */
    public static String getIconMarkup(WIContext wiContext, ResourceInfo resource, int appIconSize,
                                       boolean isTreeView, boolean forceAppSize, Boolean showDescription) {

        // indicates this resource can only be launched via the streaming client
        boolean mustUseRade = !resource.isAccessMethodAvailable(AccessMethod.LOCATION)
            && !resource.isAccessMethodAvailable(AccessMethod.DISPLAY)
            && resource.isAccessMethodAvailable(AccessMethod.STREAM);

        // Apply disabled icon overlay if user machine is RADE-capable but a RADE launch is not possible
        boolean bRadeAppUnlaunchable = mustUseRade && !Include.getWizardState(wiContext).isRADEClientAvailable();

        Icon preferredIcon = getPreferredIcon(wiContext, resource, appIconSize);

        String appIconSizeStr = (appIconSize == Constants.ICON_SIZE_16) ? Constants.ICON_SIZE_SMALL : Constants.ICON_SIZE_NORMAL;
        String iconHash = resource.getFullIconHash();
        IconCache iconCache = Include.getIconCache(wiContext.getStaticEnvironmentAdaptor());
        String iconId = iconCache.putIcon(iconHash, preferredIcon);
        String appIcon = Constants.PAGE_ICON + "?size=" + appIconSizeStr + "&amp;id=" + WebUtilities.encodeForId(iconId);

        // Whether to show the description of the resource in the tooltip.
        boolean showToolTip = (showDescription == null) ? AGEUtilities.isAGEEmbeddedMode(wiContext) : showDescription.booleanValue();
        String toolTipText = showToolTip ? resource.getDescription() : resource.getDisplayName();
        toolTipText = WebUtilities.escapeHTML(toolTipText);

        String overlayIcon = null;
        if (bRadeAppUnlaunchable) {
            overlayIcon = "../media/Disabled" + appIconSize + ".gif";
            toolTipText += " " + wiContext.getString("TipUnavailableRadeApp");
        }

        String imgTag = " alt=\"" + toolTipText + "\"" +
                        " title=\"" + toolTipText + "\" >";

        // Put the name of the image as the url to the icon so that the browser doesn't fetch the icons while loading the tree.
        // The image icons only get fetched dynamically while expanding the folders.
        String imgSrc = (isTreeView) ? " src=\"../media/Transparent16.gif\" name =\"" : " src=\"";

        String forceHeightWidth = (forceAppSize) ? " width=\"" + appIconSize + "\" height=\"" + appIconSize + "\" " : "";

        String markup = "";
        if ("image/png" == preferredIcon.getImageMimeType() && ClientInfoUtilities.requiresDirectXFilterToShowPngAlpha(wiContext.getClientInfo())) {
            // The src argument to the alpha image loader is effectively applied as a background, and the
            // <img> tag src is rendered over it.
            String foregroundIcon = (overlayIcon != null) ? overlayIcon : "../media/Transparent" + appIconSize + ".gif";
            markup = "<img " + forceHeightWidth + imgSrc + foregroundIcon + "\" style=\"" +
                "filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(src='" + appIcon + "')\"" + imgTag;
        } else {
            if (overlayIcon == null) {
                markup += "<img " + forceHeightWidth + imgSrc + appIcon + "\"" + imgTag;
            } else {
                // Alternate method for incompatible platforms
                if (wiContext.getClientInfo().osPocketPC()) {
                    markup += "<div class=\"appIcon" + appIconSize + "\" style=\"background-image: url(" + appIcon + ");background-repeat:no-repeat;\">" +
                              "<img" + forceHeightWidth + imgSrc + overlayIcon + "\"" + imgTag + "</div>";
                } else {
                    markup += "<img " + forceHeightWidth + imgSrc + overlayIcon + "\" class=\"appIcon" + appIconSize + "\" style=\"background-image: url(" + appIcon + ");background-repeat:no-repeat;\"" + imgTag;
                }
            }
        }

        return markup;
    }

    /**
     * Returns whether a web browser on a particular platform supports the PNG image format.
     * When this is the case, Web Interface returns high-color (32bpp) icons for published applications
     * that provide such icons.
     *
     * Web Interface has a built-in white list of platforms/browsers that are known
     * to support PNG. This function exists to allow customers to easily extend the built-in white-list
     * with other browsers that are found to support PNG.
     *
     * @param clientInfo a ClientInfo object providing information about the user's browser and platform OS.
     * @return true if PNG is supported; otherwise false.
     */
    private static boolean isPngSupportedByCustomWhiteList(ClientInfo clientInfo) {
        // CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP
        //
        // CUSTOMIZATION POINT
        //
        // You can query the supplied clientInfo object and return true to indicate that a particular
        // platform supports PNG.
        //
        // CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP
        return false;
    }

    /**
     * Parse the given string to a positive integer.
     *
     * If the parsing fails or the parsed value is not positive, return the
     * default value.
     *
     * @param str the string to parse
     * @param defaultVal the default value to return in case of an error
     * @return the result of the parsing, or the default value
     */
    public static int parsePositiveInteger( String str, int defaultVal ) {
        int result = defaultVal;
        if( str != null ) {
            try {
                int parsed = Integer.parseInt( str );
                if( parsed > 0 ) {
                    result = parsed;
                }
            } catch( NumberFormatException nex ) {
            }
        }
        return result;
    }

    /**
     * Gets whether searching is allowed and enabled.
     *
     * @param wiContext the Web Interface Context object
     * @return <code>true</code> if searching is enabled, else <code>false</code>
     */
    public static boolean isSearchEnabled( WIContext wiContext ) {
        boolean showSearch = wiContext.getConfiguration().getUIConfiguration().getShowSearch();

        // For AGE embedded mode, searching is not allowed
        if( !AGEUtilities.isAGEEmbeddedMode( wiContext ) ) {
            if( wiContext.getConfiguration().getUIConfiguration().getAllowCustomizeShowSearch() ) {
                showSearch = ( !Boolean.FALSE.equals(wiContext.getUserPreferences().getShowSearch()) );
            }
        }

        return showSearch;
    }

    /**
     * Determine whether a search has been performed during this logon session.
     *
     * @param wiContext the Web Interface Context object
     * @return <code>true</code> if a search has been performed, else <code>false</code>
     */
    public static boolean getSearchPreviouslyPerformed( WIContext wiContext ) {
        return wiContext.getWebAbstraction().getSessionAttribute( Constants.SV_SEARCH_QUERY ) != null;
    }

    /**
     * Retrieve the search query string from the session.
     *
     * If there is no item in the session, default to an empty string.
     *
     * @param wiContext the Web Interface Context object
     * @return the search query as a string
     */
    public static String retrieveSearchQuery( WIContext wiContext ) {
        String searchQuery = (String)wiContext.getWebAbstraction().getSessionAttribute( Constants.SV_SEARCH_QUERY );
        if( searchQuery == null ) {
            searchQuery = "";
        }
        return searchQuery;
    }

    /**
     * Filter out any resources which should not be displayed.
     *
     * An example of such a resource would be a RADE-only app when the user's
     * platform does not support RADE.
     *
     * @param wiContext the Web Interface Context object
     * @param resources array of <code>ResourceInfo</code> objects
     * @return filtered copy of input array
     */
    public static ResourceInfo[] removeHiddenResources(WIContext wiContext, ResourceInfo[] resources) {
        ArrayList filtered = new ArrayList();
        boolean platformRadeCapable =
            Include.getOsRadeCapable(wiContext.getClientInfo(), wiContext.getUserEnvironmentAdaptor())
            && Include.getBrowserRadePluginCapable(wiContext.getClientInfo(), wiContext.getUserEnvironmentAdaptor());

        for (int ix = 0; ix < resources.length; ++ix) {
            ResourceInfo res = resources[ix];
            if (!platformRadeCapable) {
                if (!res.isAccessMethodAvailable(AccessMethod.LOCATION)
                    && !res.isAccessMethodAvailable(AccessMethod.DISPLAY)
                    && res.isAccessMethodAvailable(AccessMethod.STREAM)) {
                    // This resource can only be accessed by a rade capable platform
                    continue;
                }
            }

            // This resource should be shown
            filtered.add(res);

        }

        return (ResourceInfo[])filtered.toArray(new ResourceInfo[0]);
    }

    /**
     * Removes data in the session and user preferences which relate to
     * non-existent tabs (e.g. because the administrator changed the
     * configuration).
     *
     * @param keepList list of tab identifiers for tabs which should be kept
     * @param wiContext the Web Interface Context object
     * @param userContext the user context object
     */
    public static void scrubTabPreferences( String[] keepList, WIContext wiContext, UserContext userContext ) {
        scrubSessionFolders( wiContext.getWebAbstraction(), keepList );
        UserPreferences rawUserPrefs = Include.getRawUserPrefs( userContext.getEnvironmentAdaptor() );
        scrubUserPrefsFolders( rawUserPrefs, keepList );
        scrubUserPrefsViews( rawUserPrefs, keepList );
        Include.saveUserPrefs( rawUserPrefs, wiContext, userContext );
    }

    private static void scrubUserPrefsFolders( UserPreferences rawUserPrefs, String[] keepList ) {
        // for any non-existant tabs, delete the user prefs
        HashMap currentFolderMap = rawUserPrefs.getCurrentFolders();
        scrubMap( currentFolderMap, keepList );
        rawUserPrefs.setCurrentFolders( currentFolderMap );
    }

    private static void scrubUserPrefsViews( UserPreferences rawUserPrefs, String[] keepList ) {
        HashMap currentViewsMap = rawUserPrefs.getViewStyles();
        scrubMap( currentViewsMap, keepList );
        rawUserPrefs.setViewStyles( currentViewsMap );
    }

    private static void scrubSessionFolders( WebAbstraction web, String[] keepList ) {
        // for any non-existant tabs, delete the session prefs
        Map folderMap = (Map)web.getSessionAttribute( Constants.SV_CURRENT_FOLDER );
        scrubMap( folderMap, keepList );
        web.setSessionAttribute( Constants.SV_CURRENT_FOLDER, folderMap );
    }

    private static void scrubMap( Map map, String[] keepList ) {
        if( map == null ) {
            return;
        }

        String[] keys = (String[])map.keySet().toArray( new String[0] );

        for( int i = 0; i < keys.length; i++ ) {
            String key = keys[i];
            if( !CtxArrays.contains( keepList, key ) ) {
                map.remove( key );
            }
        }
    }

    /**
     * Gets the key for the message to display when no resources are avaiable to the user.
     * The key varies based on whether the user is anonymous.
     * @param wiContext the WI context
     * @return message key as a String
     */
    public static String getEmptyMessageKeyForUser( WIContext wiContext ) {
        String key = "";
        if( Authentication.isAnonUser( wiContext.getUserEnvironmentAdaptor() ) ) {
            key = "NoAppForGuest";
        } else {
            key = "NoApp";
        }
        return key;
    }

    /**
     * Resolves the actual set of allowed resource types given sets of
     * explicitly allowed and disallowed resource types.
     *
     * If no explicitly allowed resource types are specified, it is assumed
     * that all resources are allowed except for those explicitly disallowed.
     *
     * @param allowedResources set of explicitly allowed resource types as a
     * <code>FolderResourceTypeFlags</code> object
     * @param disallowedResources set of explicitly disallowed resource types as a
     * <code>FolderResourceTypeFlags</code> object
     * @return actual set of allowed resource types as a <code>FolderResourceTypeFlags</code> object
     */
    public static FolderResourceTypeFlags resolveAllowedResourceTypes(
        FolderResourceTypeFlags allowedResources, FolderResourceTypeFlags disallowedResources) {

        // Default to allowing all resource types
        int allowedResourcesValue = FolderResourceTypeFlags.ALL_KNOWN;
        if (allowedResources != null && allowedResources.getValue() > 0) {
            allowedResourcesValue = allowedResources.getValue();
        }

        if (disallowedResources != null && disallowedResources.getValue() > 0) {
            // We only want to show resources that are allowed and not disallowed
            allowedResourcesValue = (allowedResourcesValue & ~disallowedResources.getValue());
        }

        // Always include folders
        allowedResourcesValue |= FolderResourceTypeFlags.FOLDERS;

        return new FolderResourceTypeFlags(allowedResourcesValue);
    }

}
