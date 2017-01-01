// Copyright (c) 2002 - 2010 Citrix Systems, Inc. All Rights Reserved.

package com.citrix.wi.pages.site;

import java.io.IOException;

import com.citrix.wi.controlutils.FeedbackMessage;
import com.citrix.wi.mvc.StatusMessage;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pages.StandardPage;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.SessionToken;
import com.citrix.wi.pageutils.SessionUtils;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wi.pageutils.Utils;
import com.citrix.wing.AccessTokenException;
import com.citrix.wing.MessageType;
import com.citrix.wing.ResourceUseException;
import com.citrix.wing.webpn.SearchStatus;
import com.citrix.wing.webpn.UserContext;

public class Disconnect extends StandardPage {

    public Disconnect(WIContext wiContext) {
        super(wiContext);
    }

    protected boolean performImp() throws IOException {
        if (!SessionToken.checkQueryValid(wiContext.getWebAbstraction())) {
            return true;
        }

        // Get the user context
        UserContext userContext = SessionUtils.checkOutUserContext( wiContext );

        // Use this to keep track of the feedback to display on the next visible page
        FeedbackMessage feedback = null;

        if (Include.isWorkspaceControlEnabled( wiContext ) && !Authentication.isAnonUser( wiContext.getUserEnvironmentAdaptor() )) {
            try {
                SearchStatus result = userContext.disconnectClientSessions();

                if( result == SearchStatus.PARTIAL_TEMP ) {
                    feedback = new FeedbackMessage(MessageType.ERROR, "WorkspaceControlDisconnectPartialTemp");
                } else if( result == SearchStatus.PARTIAL_PERSISTENT ) {
                    feedback = new FeedbackMessage(MessageType.ERROR, "WorkspaceControlDisconnectPartialPersist");
                }
            } catch( AccessTokenException ate ) {
                feedback = new FeedbackMessage(MessageType.ERROR, Utils.getAuthErrorMessageKey(ate));
            } catch( ResourceUseException ignore) {
                feedback = new FeedbackMessage(MessageType.ERROR, "DisconnectError");
            }
        } else {
            // Workspace Control is not enabled, it is Anonymous or we fail to
            // obtains the credentials or client name. Puts up an error message.
            // Log a detailed message and display a general message to the user.
            feedback = new FeedbackMessage(MessageType.ERROR, "DisconnectError");
            feedback.setLogEventId(wiContext.log(MessageType.ERROR, "DisconnectInternalError"));
        }

        // Logs out Web Interface and displays a message as appropriate.
        // This may set cookies and so is called before the user context is returned
        MessageType msgType = (feedback == null) ? null : feedback.getType();
        String msgKey = (feedback == null) ? null : feedback.getKey();

        UIUtils.handleLogout(wiContext, msgType, msgKey, false);

        // Return the user context
        SessionUtils.returnUserContext(userContext);

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
