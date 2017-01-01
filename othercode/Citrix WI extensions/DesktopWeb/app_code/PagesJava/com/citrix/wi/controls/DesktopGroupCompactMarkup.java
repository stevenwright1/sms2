/*
 * Copyright (c) 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.controls;

import java.util.Iterator;
import java.util.List;

import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wi.pageutils.SessionToken;
import com.citrix.wing.util.Strings;
import com.citrix.wing.util.WebUtilities;

/**
 * The class responsible for generating markup for the desktop tab in low graphics mode.
 */
public class DesktopGroupCompactMarkup extends DesktopGroupMarkup {
    private static final String DESKTOP_ICON = "../media/DesktopIcon16.gif";

    private String generateLaunchCell(ResourceControl resource, WIContext wiContext) {
        String displayName = WebUtilities.escapeHTML(resource.getTruncatedName(wiContext));

        String markup = "<td class='desktopColumn'>" +
                           "<a " + resource.launchHref +
                               " id='" + resource.getEncodedAppId() + "'" +
                               " name='" + WebUtilities.encodeForId(resource.getName(wiContext)) + "'" +
                               " class='iconLink desktopsTabLaunchLink'" +
                               " title='" + WebUtilities.escapeHTML(resource.getDescription(wiContext)) + "'>" +
                            "<img" +
                              " id='spinner_" + resource.getEncodedAppId() + "'" +
                              " class='spinner'" +
                              " width='11' height='11'" +
                              " src='" + resource.getDelayedLaunchImgSrc(false) + "'" +
                              " alt=''>" +
                              "<img " +
                              " src='" + DESKTOP_ICON + "'" +
                              " alt=''" +
                              " class='desktopIconGroup'>" +
                            "<span>" + displayName + "</span></a>" +
                         "</td>";
        return markup;
    }

    private String generateRestartMarkup(ResourceControl resource, WIContext wiContext) {
        String markup = "<td class='restartColumn'>";

        if (canShowRestartButton(resource)) {
            String restartLinkFragment = UIUtils.getApplistConfirmRestartLinkFragment(wiContext, resource, SessionToken.get(wiContext));
            String altCaption = wiContext.getString("Restart") + " " + WebUtilities.escapeHTML(resource.getName(wiContext));

            markup += "<span id='restart_" + resource.getEncodedAppId() + "' class='iconLink'>" +
                      "[ <a " + restartLinkFragment + " title='" + altCaption + "' class='desktopsTabRestartLink'>" +
                         "<span class='desktopsTabRestartDescription'>" + wiContext.getString("Restart") + "</span></a> ]" +
                      "</span>";
        }
        markup += "</td>";
        return markup;
    }


    public String getDesktopMarkup(ResourceControl desktop, WIContext wiContext) {
        StringBuffer markup = new StringBuffer();

        markup.append(generateLaunchCell(desktop, wiContext));
        markup.append(generateRestartMarkup(desktop, wiContext));

        return markup.toString();
    }
    
    /**
     * @see DesktopGroupMarkup
     */
    public String getGroupMarkup(DesktopGroup desktopGroup, WIContext wiContext) {
		StringBuffer markup = new StringBuffer("<div id='" + WebUtilities.encodeForId(desktopGroup.getFirstResource().getName(wiContext)) + "'>" +
                                               "<table width=\"100%\" cellspacing=\"0\">");

        Iterator desktopIt = desktopGroup.getDesktops().iterator();
        while (desktopIt.hasNext()) {
            ResourceControl resource = (ResourceControl)desktopIt.next();
            markup.append("<tr>");
            markup.append(getDesktopMarkup(resource, wiContext));
            markup.append("</tr>");
        }

        markup.append("</table></div>");
        return markup.toString();
    }

}
