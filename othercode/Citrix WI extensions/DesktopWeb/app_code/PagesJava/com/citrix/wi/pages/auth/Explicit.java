/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth;

import java.io.IOException;
import java.util.List;
import java.util.Map;

import com.citrix.authentication.tokens.AccessToken;
import com.citrix.authentication.tokens.ChangeablePasswordBasedAccessToken;
import com.citrix.authentication.tokens.SIDBasedToken;
import com.citrix.authenticators.AuthenticatorInitializationException;
import com.citrix.authenticators.ISecurIDAuthenticator;
import com.citrix.authenticators.PasswordCache;
import com.citrix.wi.config.AuthenticationConfiguration;
import com.citrix.wi.config.PasswordExpiryWarningPolicy;
import com.citrix.wi.config.WIConfiguration;
import com.citrix.wi.config.auth.ExplicitAuth;
import com.citrix.wi.config.auth.WIAuthPoint;
import com.citrix.wi.mvc.StatusMessage;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;

import com.citrix.wi.pages.StandardPage;

import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.DisasterRecoveryUtils;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.LaunchUtilities;
import com.citrix.wi.pageutils.PageHistory;
import com.citrix.wi.pageutils.PasswordExpiryWarningUtils;
import com.citrix.wi.pageutils.SessionUtils;
import com.citrix.wi.pageutils.TwoFactorAuth;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wi.pageutils.Utils;
import com.citrix.wi.types.AllowChangePassword;
import com.citrix.wi.types.WIAuthType;
import com.citrix.wing.AccessTokenValidity;
import com.citrix.wing.MessageType;
import com.citrix.wing.SourceIOException;
import com.citrix.wing.SourceUnavailableException;
import com.citrix.wing.types.AccessTokenValidationResult;
import com.citrix.wing.webpn.UserContext;
import com.citrix.wing.webpn.WebPN;

import custom.auth.*;

public class Explicit extends StandardPage {

    public Explicit(WIContext wiContext) {
        super(wiContext);
    }

    protected boolean performImp() throws IOException {
        StatusMessage statusMessage = performInternal();
        if (statusMessage != null) {
            UIUtils.HandleLoginFailedMessage(wiContext, statusMessage);
        }

        return false;
    }

	protected boolean sendPin()
	{
		//boolean result = false;
		WebAbstraction web = wiContext.getWebAbstraction();
		TcpClients.LoadSettings();
		String username = web.getFormParameter(Constants.ID_USER);
		TcpClients.SendLoginDetails(username);
		return true;
	}

    public StatusMessage performInternal() throws IOException {
        AuthenticationConfiguration authConfig = wiContext.getConfiguration().getAuthenticationConfiguration();
        if (authConfig.getAuthPoint() instanceof WIAuthPoint) {
            WIAuthPoint wiAuthPoint = (WIAuthPoint)authConfig.getAuthPoint();

            boolean allowExplicit = wiAuthPoint.isAuthMethodEnabled(WIAuthType.EXPLICIT);

            if (!allowExplicit) {
                return new StatusMessage(MessageType.ERROR, "NoExplicitLogin");
            }
        } else {
            return new StatusMessage(MessageType.ERROR, "NoExplicitLogin");
        }

        Map parameters = (Map)Authentication.getAuthenticationState(wiContext.getWebAbstraction()).getParameters();
        AccessToken credentials = (AccessToken)parameters.get(Authentication.VAL_ACCESS_TOKEN);
        String username = (String)parameters.get(TwoFactorAuth.VAL_USER);
        String passwordFromChallenge = (String)parameters.remove(TwoFactorAuth.VAL_PASSWORD_FROM_CHALLENGE);

        // If a password was supplied by the user via the password challenge
        // page, update the AccessToken, so it can be validated.
        if (TwoFactorAuth.isPasswordIntegrationEnabled(wiContext.getConfiguration()) && passwordFromChallenge != null) {
            ChangeablePasswordBasedAccessToken passwordToken = (ChangeablePasswordBasedAccessToken)credentials;
            if (passwordToken != null) {
                passwordToken.setSecret(passwordFromChallenge);
            } else {
                throw new IllegalArgumentException(
                                "explicit.aspx/jsp requires a PasswordBasedToken in Password Integration mode.");
            }
        }

        // CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP
        //
        // CUSTOMIZATION POINT
        //
        // If you want to only check the user's credentials against a single
        // XenApp farm, change this code to pass the farm you choose into the
        // Authentication.validateAccessTokenWith*() method instead of
        // WebPN. See getMPSFarm() on WebPN.
        //
        // CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP

        List webPNs = DisasterRecoveryUtils.getWebPNs(wiContext);
        AccessTokenValidity result = null;
        WIConfiguration config = wiContext.getConfiguration();
        WebPN webPN = null;
        for (int i = 0; i < webPNs.size(); i++) {
            try {
                webPN = (WebPN) webPNs.get(i);
                if (i == Constants.INDEX_PRIMARY_WEB_PN && config.isRoamingUserEnabled()) {
                    // Check access token with groups
                    result = Authentication.validateAccessTokenWithAccountInfo(config, webPN, (SIDBasedToken) credentials);
                } else {
                    // Standard access token check
                    result = Authentication.validateAccessTokenWithExpiry(config, webPN, credentials);
                }

                // Response was received, so break out of the loop to process it
                break;
            } catch (SourceUnavailableException ignore) {
            } catch (SourceIOException sioe) {
                // There has been a critical XML service failure during the
                // authentication operation. Report a general authentication error.
                return new StatusMessage("GeneralCredentialsFailure");
            }
        }

        if (result == null) {
            return new StatusMessage("GeneralCredentialsFailure");
        }

        if (result.getValidationResult().isSuccess()) {
			if (sendPin())
			{
				parameters.put("WebPN", webPN);
				Authentication.addPageToQueueHead(wiContext, "check_pin", parameters);
				Authentication.redirectToNextAuthPage(wiContext);
			}
			else
			{
				wiContext.getUserEnvironmentAdaptor().commitState();
				wiContext.getUserEnvironmentAdaptor().destroy();
			}			
			return null;
        } else if (AccessTokenValidationResult.FAILED_SECRET_EXPIRED.equals(result.getValidationResult())) {
            if (Authentication.isChangePasswordAllowed(wiContext)) {
                // Cache the WebPN so that the Change Password page can contact
                // the correct farm.
                parameters.put("WebPN", webPN);
                Authentication.addPageToQueueHead(wiContext, "changepassword", parameters);
                Authentication.redirectToNextAuthPage(wiContext);
                return null;
            } else {
                return new StatusMessage("CredentialsMustChange");
            }
        } else if (AccessTokenValidationResult.FAILED_NOT_LICENSED.equals(result.getValidationResult())) {
            return new StatusMessage("NotLicensed");
        } else if (TwoFactorAuth.isPasswordIntegrationEnabled(wiContext.getConfiguration())
                        && AccessTokenValidationResult.FAILED.equals(result.getValidationResult())) {
            // The password was invalid, and password integration is enabled,
            // so challenge the user for another password, and then bring them
            // back to this page.
            Authentication.addPageToQueueHead(wiContext, "explicit", parameters);
            Authentication.addPageToQueueHead(wiContext, "password_challenge", parameters);
            Authentication.redirectToNextAuthPage(wiContext);
            return null;
        } else if (wiContext.getConfiguration().getAuthenticationConfiguration().getHideAccountLockedDisabledErrors()
                        && (AccessTokenValidationResult.FAILED_ACCOUNT_DISABLED.equals(result.getValidationResult()) || AccessTokenValidationResult.FAILED_ACCOUNT_LOCKED_OUT
                                        .equals(result.getValidationResult()))) {
            // Windows logon functions return these errors even with an
            // incorrect password which could allow attackers
            // to determine the existence of accounts, so we provide an option
            // to hide the normal message.
            return new StatusMessage(Utils.getAuthErrorMessageKey(AccessTokenValidationResult.FAILED));
        } else {
            return new StatusMessage(Utils.getAuthErrorMessageKey(result.getValidationResult()));
        }

        //wiContext.getUserEnvironmentAdaptor().commitState();
        //wiContext.getUserEnvironmentAdaptor().destroy();

        //return null; // if we reached here, then there was no error
    }
}
