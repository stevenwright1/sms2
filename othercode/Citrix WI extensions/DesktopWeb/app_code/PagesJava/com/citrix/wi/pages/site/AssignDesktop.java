// AssignDesktop.java
// Copyright (c) 2010 Citrix Systems, Inc. All Rights Reserved.
// [NFuseVersionAndBuildNumber]
package com.citrix.wi.pages.site;

import java.io.IOException;

import com.citrix.wi.controls.AssignDesktopControl;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pages.StandardPage;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.LaunchUtilities;
import com.citrix.wi.pageutils.ResourceEnumerationUtils;
import com.citrix.wi.pageutils.SessionUtils;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wing.MessageType;
import com.citrix.wing.DesktopUnavailableException;
import com.citrix.wing.NoSuchResourceException;
import com.citrix.wing.types.DesktopAssignmentType;
import com.citrix.wing.types.DesktopUnavailableErrorType;
import com.citrix.wing.util.Strings;
import com.citrix.wing.webpn.DesktopInfo;
import com.citrix.wing.webpn.ResourceInfo;
import com.citrix.wing.webpn.UserContext;
import com.citrix.wi.controls.ApplistPageControl;
import com.citrix.wing.util.WebUtilities;
import com.citrix.wi.controls.DesktopGroupsController;
import com.citrix.wi.util.Trace;

/**
 * This page calls into WING to assign a desktop to a user; it is expected to be requested
 * via an Ajax call and returns JSON data.
 *
 * If the desktop assignment succeeds, the JSON data supplies HTML markup to represent the
 * updated desktop group. If the assignment fails, a redirect URL or feedback message is
 * returned within the JSON data.
 */
public class AssignDesktop extends StandardPage {

    protected AssignDesktopControl viewControl = new AssignDesktopControl();
    UserContext userContext;

    public AssignDesktop(WIContext wiContext) {
        super(wiContext);
        wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
    }

    protected boolean performImp() throws IOException {
        WebAbstraction web = wiContext.getWebAbstraction();
        web.setResponseContentType("application/json; charset=UTF-8");

        userContext = SessionUtils.checkOutUserContext(wiContext);

        // The internal app id of the desktop is expected to be supplied via a query string parameter.
        String resId = web.getQueryStringParameter(Constants.QSTR_APPLICATION);

        ResourceInfo resInfo = ResourceEnumerationUtils.getResource(resId, userContext);
        if (resInfo == null || !resInfo.isEnabled()) {
            setRedirectUrl(MessageType.ERROR, "AppRemoved");
            return true;
        }

        // Try to perform the desktop assignment
        String desktopHostName = null;
        try {
			desktopHostName = userContext.assignDesktop(resId);
        } catch (NoSuchResourceException e) {
            setRedirectUrl(MessageType.ERROR, "AppRemoved");
            return true;
        } catch (DesktopUnavailableException due) {
            Trace.trace(this, "Exception thrown while launching. " + due.getMessage() + "Error type was: " + due.getErrorType());
            setRedirectUrl(MessageType.ERROR, "GeneralAppLaunchError");
            return true;
        } catch (Exception e) {
            Trace.trace(this, "Exception thrown while launching: " + e.getMessage());
            setRedirectUrl(MessageType.ERROR, "GeneralAppLaunchError");
            return true;
        }

        // A side-effect of assigning the desktop is that the desktop assignment type changes from assign-on-first-use
        // to assigned (and the resource's sequence number changes). The user's cache is cleared so that the next request
        // for resource info triggers an enumeration, which repopulates the cache with the latest resource information.
        userContext.getEnvironmentAdaptor().getSessionCache().clear();

        // When auto-launch is not possible, a feedback message is displayed to the user.
		// A redirect URL is not used because we want to display the returned markup, which includes a temporary
	    // 'play' icon indicating the desktop that is ready to be launched.
        if (LaunchUtilities.browserBlocksLaunch(wiContext)) {
            viewControl.autoLaunch = false;
            viewControl.feedbackMessage = WebUtilities.escapeHTML(wiContext.getString("AssignedDesktopReady"));
        }

        // Create the relevant markup for the entire desktop group, incorporating the new desktop name
        try {
            DesktopGroupsController groupsController = DesktopGroupsController.getInstance(wiContext);
            viewControl.markup = groupsController.assignDesktopAndGetMarkup(wiContext, desktopHostName, (DesktopInfo)resInfo);
            viewControl.markup = WebUtilities.escapeHTML(viewControl.markup);
        } catch (Exception e) {
            setRedirectUrl(MessageType.ERROR, "GeneralAppLaunchError");
        }
        return true;
    }

    private void setRedirectUrl(MessageType messageType, String messageKey) {
        viewControl.redirectUrl = UIUtils.getMessageRedirectUrl(wiContext, Include.getHomePage(wiContext), messageType, messageKey);
    }
}
