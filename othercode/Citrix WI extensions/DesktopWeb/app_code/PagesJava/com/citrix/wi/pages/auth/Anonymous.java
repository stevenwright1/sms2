/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth;

import java.io.IOException;

import com.citrix.authentication.tokens.GuestToken;
import com.citrix.wi.config.AuthenticationConfiguration;
import com.citrix.wi.config.auth.WIAuthPoint;
import com.citrix.wi.mvc.ActionState;
import com.citrix.wi.mvc.StatusMessage;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.LaunchUtilities;
import com.citrix.wi.pageutils.SessionUtils;
import com.citrix.wi.types.WIAuthType;
import com.citrix.wing.webpn.UserContext;

public class Anonymous extends AbstractAuthLayout {

    public Anonymous(WIContext wiContext) {
        super(wiContext);
    }

    public ActionState performInternal() throws IOException {
        AuthenticationConfiguration authConfig = wiContext.getConfiguration().getAuthenticationConfiguration();
        if (authConfig.getAuthPoint() instanceof WIAuthPoint) {
            WIAuthPoint wiAuthPoint = (WIAuthPoint)authConfig.getAuthPoint();
            if (wiAuthPoint.isAuthMethodEnabled(WIAuthType.ANONYMOUS)) {
                Authentication.getAuthenticationState(wiContext.getWebAbstraction()).setAuthenticated(new GuestToken());

                // Create a UserContext to hold user-specific app state; this
                // is null if the creation failed, which indicates some horrible
                // problem has arisen somewhere.
                UserContext userContext = SessionUtils.createNewUserContext(wiContext);
                if (userContext == null) {
                    return new ActionState(new StatusMessage("GeneralCredentialsFailure"));
                }

                LaunchUtilities.transferLaunchDataToSession(wiContext);
                Authentication.redirectToCurrentAuthPage(wiContext);
            } else {
                return new ActionState(new StatusMessage("NoAnonymousLogin"));
            }
        }
        return new ActionState(false);
    }

    protected String getBrowserPageTitleKey() {
        return "BrowserTitleAnonymous";
    }
}