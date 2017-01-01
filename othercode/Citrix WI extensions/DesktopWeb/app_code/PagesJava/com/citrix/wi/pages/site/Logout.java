/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.site;

import java.io.IOException;

import com.citrix.wi.controlutils.FeedbackMessage;
import com.citrix.wi.mvc.StatusMessage;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pages.StandardPage;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.LaunchUtilities;
import com.citrix.wi.pageutils.SessionToken;
import com.citrix.wi.pageutils.SessionUtils;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wi.pageutils.Utils;
import com.citrix.wing.AccessTokenException;
import com.citrix.wing.MessageType;
import com.citrix.wing.ResourceUseException;
import com.citrix.wing.webpn.SearchStatus;
import com.citrix.wing.webpn.UserContext;

public class Logout extends StandardPage {

    public Logout(WIContext wiContext) {
        super(wiContext);
    }

    protected boolean performImp() throws IOException {
        if (!SessionToken.checkQueryValid(wiContext.getWebAbstraction()))
        {
            return true;
        }

        // Get the user context
        UserContext userContext = SessionUtils.checkOutUserContext(wiContext);

        // Use this to keep track of the feedback to display on the next visible page
        FeedbackMessage feedback = null;

        // Clear the session state variables before logging out from a direct launch
        if (LaunchUtilities.getDirectLaunchModeInUse(wiContext)) {
            LaunchUtilities.clearSessionLaunchData(wiContext);
        }

        if (Constants.VAL_ON.equalsIgnoreCase(wiContext.getWebAbstraction().getQueryStringParameter(Constants.QSTR_TIMEOUT))) {
            // Invoked by session timeout.
            feedback = new FeedbackMessage(MessageType.INFORMATION, "SessionExpired");
        } else {
            if (Include.isWorkspaceControlEnabled(wiContext) && (feedback == null)) {

                // Checks whether to log off active sessions. The default behaviour is
                // to log off active sessions.
                if ( !Boolean.FALSE.equals( wiContext.getUserPreferences().getLogoffApps() ) ) {
                    try {
                        SearchStatus result = userContext.logOffClientSessions();

                        if( result == SearchStatus.PARTIAL_TEMP ) {
                            feedback = new FeedbackMessage(MessageType.ERROR, "WorkspaceControlLogoffPartialTemp");
                        } else if( result == SearchStatus.PARTIAL_PERSISTENT ) {
                            feedback = new FeedbackMessage(MessageType.INFORMATION, "WorkspaceControlLogoffPartialPersist");
                        }
                    } catch( AccessTokenException ate ) {
                        feedback = new FeedbackMessage(MessageType.ERROR, Utils.getAuthErrorMessageKey(ate));
                    } catch( ResourceUseException rue ) {
                        feedback = new FeedbackMessage(MessageType.ERROR, "LogoutError");
                    }
                }
            }
        }

        // Logs out Web Interface and displays a message as appropriate.
        // This may set cookies and so is called before the user context is returned
        MessageType msgType = (feedback == null) ? null : feedback.getType();
        String msgKey = (feedback == null) ? null : feedback.getKey();

        UIUtils.handleLogout(wiContext, msgType, msgKey, false);

        // Return the user context
        SessionUtils.returnUserContext( userContext );

        // Destroy the session
        // We do this after returning the user context, as one of the last actions
        // on the page, to avoid any additional actions creating a new session
        // inadvertently.
        wiContext.getWebAbstraction().abandonSession();

        // Finally, carry out global logout for federated services if necessary.
        // If this succeeds, page execution will stop and the user is redirected to
        // a page controlled by ADFS.
        // Therefore, this has to be the last action on the page, and we must make
        // sure that the session has been abandoned before calling the federated logout.
        StatusMessage errorMessage = wiContext.getUtils().doFederatedLogout(wiContext);
        if (errorMessage != null) {
            String loggedOutUrl = UIUtils.getLoggedOutRedirectURL(wiContext, errorMessage.getType(),
                errorMessage.getDisplayMessageKey(), errorMessage.getDisplayMessageArg(), null);
            wiContext.getWebAbstraction().clientRedirectToUrl(loggedOutUrl);
        }

        return false;
    }

}
