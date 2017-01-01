/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth.twofactor;

import java.io.IOException;
import java.util.Map;

import com.citrix.authentication.tokens.AccessToken;
import com.citrix.authentication.tokens.UPNCredentials;
import com.citrix.authentication.tokens.UserDomainPasswordCredentials;
import com.citrix.authenticators.AuthenticatorInitializationException;
import com.citrix.authenticators.ISafewordAuthenticator;
import com.citrix.wi.mvc.StatusMessage;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pages.StandardPage;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.TwoFactorAuth;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wing.MessageType;

/**
 * This class processes a Safeword authentication.
 */
public class Safeword extends StandardPage {

    private final static StatusMessage SAFEWORD_ERROR_STATUS = new StatusMessage(MessageType.ERROR, AbstractTwoFactorAuthLayout.VAL_GENERAL_AUTHENTICATION_ERROR, null, "SafewordAuthenticatorError", null);

    public Safeword(WIContext wiContext) {
        super(wiContext);
    }

    protected boolean performImp() throws IOException {
        Map parameters = (Map)Authentication.getAuthenticationState(wiContext.getWebAbstraction()).getParameters();

        AccessToken credentials = (AccessToken)parameters.get(Authentication.VAL_ACCESS_TOKEN);

        String username = null;
        String domain = null;
        if( (credentials != null) && (credentials instanceof UPNCredentials) ) {
            UPNCredentials upn = (UPNCredentials)credentials;
            username = upn.getUPN();
            domain = "";
        } else if ( (credentials != null) && (credentials instanceof UserDomainPasswordCredentials) ) {
            UserDomainPasswordCredentials udp = (UserDomainPasswordCredentials)credentials;
            username = udp.getUser();
            domain = udp.getDomain();
        } else {
            throw new IllegalArgumentException("safeword.aspx can only be passed a UPNCredentials or UserDomainPasswordCredentials object.");
        }

        parameters.put(TwoFactorAuth.VAL_USER, username);

        String passcode = (String)parameters.get(TwoFactorAuth.VAL_PASSCODE);
        String clientId = Include.getClientAddress(wiContext);

        try {

            ISafewordAuthenticator authenticator = wiContext.getUtils().getNewSafewordAuthenticator();

            String result = authenticator.Authenticate(username, domain, passcode, clientId);
            if (TwoFactorAuth.SAFEWORD_SUCCESS.equals(result)) {
                Authentication.forwardToNextAuthPage(wiContext);
            } else if (TwoFactorAuth.SAFEWORD_INVALID.equals(result)) {
                UIUtils.HandleLoginFailedMessage(wiContext, AbstractTwoFactorAuthLayout.INVALID_CREDENTIALS_STATUS);
            } else {
                UIUtils.HandleLoginFailedMessage(wiContext, SAFEWORD_ERROR_STATUS);
            }

        } catch(AuthenticatorInitializationException aie) {
            UIUtils.HandleLoginFailedMessage(wiContext, MessageType.ERROR, AbstractTwoFactorAuthLayout.VAL_GENERAL_AUTHENTICATION_ERROR, "", aie.getMessage(), null);
        }

        return false;
    }
}
