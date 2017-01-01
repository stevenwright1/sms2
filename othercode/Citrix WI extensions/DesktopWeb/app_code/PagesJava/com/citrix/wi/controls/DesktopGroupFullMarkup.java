/*
 * Copyright (c) 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.controls;

import java.util.Iterator;

import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wi.pageutils.SessionToken;
import com.citrix.wing.util.Strings;
import com.citrix.wing.util.WebUtilities;
import com.citrix.wi.pageutils.DelayedLaunchUtilities;
import com.citrix.wi.pageutils.DesktopLaunchHistory;
import com.citrix.wi.pageutils.Include;

/**
 * The class responsible for generating markup for the desktop tab in full graphics mode.
 */
public class DesktopGroupFullMarkup extends DesktopGroupMarkup {
    static private final int MAX_STRING_LENGTH = 55;

    static private final String RESTART_ICON = "../media/Restart.png";

    private String getLaunchLinkMarkup(ResourceControl desktop, WIContext wiContext) {
        String link = "<a " + desktop.launchHref +
                            " id='" + desktop.getEncodedAppId() + "'" +
                            " name='" + WebUtilities.escapeHTML(desktop.getName(wiContext)) + "'" +
                            " class='iconLink'" +
                            " title='" + WebUtilities.escapeHTML(desktop.getDescription(wiContext)) + "'>";
        return link;
    }

    private String getRestartLinkMarkup(ResourceControl desktop, WIContext wiContext) {
        String restartLinkFragment = UIUtils.getApplistConfirmRestartLinkFragment(wiContext, desktop, SessionToken.get(wiContext));
        String altCaption = wiContext.getString("Restart") + " " + WebUtilities.escapeHTML(desktop.getName(wiContext));

        String link = "<a " + restartLinkFragment +
                          " id='restart_" + desktop.getEncodedAppId() + "'" +
                          " class='restartLink'" +
                          " title='" + altCaption + "'>";
        return link;
    }

    public String getDesktopMarkup(ResourceControl desktop, WIContext wiContext) {
        StringBuffer markup = new StringBuffer();

        if (desktop.startInProgress) {
            markup.append("<script type='text/javascript'> " +
                                "delayedLaunchDesktops.push('" + desktop.getEncodedAppId() + "');" +
                          "</script>");
        }

        DelayedLaunchControl delayedLaunchControl = DelayedLaunchUtilities.getDelayedLaunchControl(wiContext);
        DesktopLaunchHistory desktopLaunchHistory = DesktopLaunchHistory.getInstance(wiContext);

        boolean isDesktopReadyToLaunch = delayedLaunchControl.isBlockedLaunch(desktop.id);
        boolean wasDesktopLaunched = desktopLaunchHistory.containsDesktop(desktop.id);

        String desktopClass = "desktopScreen";
        if (isDesktopReadyToLaunch || wasDesktopLaunched) {
            desktopClass += " activeDesktop";
        }
        String spinnerId = "desktopSpinner_" + desktop.getEncodedAppId();

        markup.append("<div class='desktopName'>");
        markup.append(getLaunchLinkMarkup(desktop, wiContext));
        markup.append(WebUtilities.escapeHTML(desktop.getTruncatedName(wiContext)));
        markup.append("  </a>");
        markup.append("</div>");
        markup.append("<div class='desktopScreenContainer'>");
        markup.append(getLaunchLinkMarkup(desktop, wiContext));
        markup.append("     <div class='" + desktopClass + "' id='screen_" + desktop.getEncodedAppId() + "'>");
        // When the user bookmarks the desktop launch link, the content of the span is displayed as the bookmark title.
        markup.append("       <span style='display:none'>" + desktop.getName(wiContext) + "</span>");
        markup.append(Include.getDelayedLaunchImg(wiContext, desktop, spinnerId));
        markup.append("     </div>");
        markup.append("  </a>");
        markup.append("</div>");

        if (canShowRestartButton(desktop)) {
            String desktopRestartClass = "restartLinkShowOnFocus";
            if (delayedLaunchControl.isResourcePending(desktop.id)) {
                desktopRestartClass = "restartLinkAlwaysShow";
            }

            markup.append("<div class='" + desktopRestartClass + "' id='restart_" + desktop.getEncodedAppId() + "'>");
            markup.append(getRestartLinkMarkup(desktop, wiContext));
            markup.append(wiContext.getString("RestartDesktopText"));
            markup.append("  </a>");
            markup.append("</div>");
        } else {
            markup.append("<div class='restartLinkNotRestartable' id='restart_" + desktop.getEncodedAppId() + "'><!-- --></div>");
        }

        return markup.toString();
    }

    /**
     * @see DesktopGroupMarkup
     */
    public String getGroupMarkup(DesktopGroup desktopGroup, WIContext wiContext) {
        StringBuffer markup = new StringBuffer();

        Iterator desktopIt = desktopGroup.getDesktops().iterator();
        while (desktopIt.hasNext()) {
            ResourceControl desktop = (ResourceControl)desktopIt.next();
            markup.append("<div id='desktop_" + desktop.getEncodedAppId() + "' class='desktopResource' ");
            markup.append(" onmouseout='updateDesktopDisplay(this, false);' onmouseover='updateDesktopDisplay(this, true)'>");
            markup.append(  getDesktopMarkup(desktop, wiContext));
            markup.append("</div>");
        }
        return markup.toString();
    }

}
