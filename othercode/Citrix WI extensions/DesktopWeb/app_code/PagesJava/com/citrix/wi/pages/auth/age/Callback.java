/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth.age;

import java.io.IOException;
import java.util.Map;

import com.citrix.wi.authservice.ASClient;
import com.citrix.wi.authservice.ASCommunicationException;
import com.citrix.wi.authservice.ASOperationFailedException;
import com.citrix.wi.authservice.AccessInfo;
import com.citrix.wi.config.auth.AGAuthPoint;
import com.citrix.wi.config.auth.AuthPoint;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pages.StandardPage;
import com.citrix.wi.pageutils.AGEUtilities;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.types.AGEAccessMode;
import com.citrix.wing.MessageType;
import com.citrix.wing.types.AccessConditions;

public class Callback extends StandardPage {

    public Callback(WIContext wiContext) {
        super(wiContext);
    }

    protected boolean performImp() throws IOException {

        WebAbstraction web = getWebAbstraction();
        Map parameters = (Map)Authentication.getAuthenticationState(web).getParameters();

        AuthPoint authPoint = wiContext.getConfiguration().getAuthenticationConfiguration().getAuthPoint();
        if (authPoint != null && authPoint instanceof AGAuthPoint) {
            // Create a client for the Authentication Service
            try {
                ASClient asClient = wiContext.getUtils().getASClient(
                                ((AGAuthPoint)authPoint).getAGAuthenticationServiceUrl(),
                                wiContext.getStaticEnvironmentAdaptor());

                String sessionId = (String)parameters.get(Constants.AGE_SESSION_ID);

                // Call the service
                AccessInfo aInfo = asClient.getAccessInfo(sessionId, (String)parameters.get(Constants.AGE_USERNAME),
                                (String)parameters.get(Constants.AGE_DOMAIN));

                AGEUtilities.recordAGEState(
                            wiContext,
                            AGEAccessMode.fromString(aInfo.getAccessMode()),
                            aInfo.getClientIPAddress(),
                            aInfo.getClientName(),
                            sessionId,
                            aInfo.getSSLProxyHost(),
                            new AccessConditions(sessionId, aInfo.getAccessConditions(), aInfo.getFarmName(), aInfo.getFarmID()));

                // Check whether to redirect to the password prompt page
                // The password prompt page must appear after this one because
                // it contains UI and
                // thus requires Constants.SV_AGE_ACCESS_MODE to know how to
                // render itself
                if (((AGAuthPoint)authPoint).isPromptPasswordEnabled()) {
                    Authentication.addPageToQueueHead(wiContext, "agepassword", parameters);
                    Authentication.redirectToNextAuthPage(wiContext);
                    return false;
                }

                // Forward to the next page
                // Return immediately to prevent further execution of this page.
                Authentication.forwardToNextAuthPage(wiContext);
                return false;

            } catch (ASCommunicationException asce) {
                // There was a problem communicating with the Authentication
                // Service
                wiContext.log(MessageType.ERROR, asce.getMessageKey(), asce.getMessageArguments());
            } catch (ASOperationFailedException aofe) {
                // The Authentication Service denied the request
                wiContext.log(MessageType.ERROR, aofe.getMessageKey(), aofe.getMessageArguments());
            }
        }

        web.abandonSession();
        web.setResponseStatus(WebAbstraction.SC_UNAUTHORIZED);
        web.writeToResponse(wiContext.getString(AGEUtilities.KEY_AG_AUTH_ERROR));

        return false;
    }
}
