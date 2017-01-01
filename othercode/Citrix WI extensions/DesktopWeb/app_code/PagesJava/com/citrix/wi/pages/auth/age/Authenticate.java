/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth.age;

import java.io.IOException;
import java.util.List;
import java.util.Map;

import com.citrix.authentication.tokens.AccessToken;
import com.citrix.authentication.tokens.SIDBasedToken;
import com.citrix.wi.config.PasswordExpiryWarningPolicy;
import com.citrix.wi.config.WIConfiguration;
import com.citrix.wi.config.auth.AGAuthPoint;
import com.citrix.wi.config.auth.AuthPoint;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pages.StandardPage;
import com.citrix.wi.pageutils.AGEUtilities;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.DisasterRecoveryUtils;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.PageHistory;
import com.citrix.wi.pageutils.PasswordExpiryWarningUtils;
import com.citrix.wi.pageutils.SessionUtils;
import com.citrix.wi.types.AllowChangePassword;
import com.citrix.wing.AccessTokenValidity;
import com.citrix.wing.SourceIOException;
import com.citrix.wing.SourceUnavailableException;
import com.citrix.wing.types.AccessTokenValidationResult;
import com.citrix.wing.webpn.UserContext;
import com.citrix.wing.webpn.WebPN;

public class Authenticate extends StandardPage {

    public Authenticate(WIContext wiContext) {
        super(wiContext);
    }

    protected boolean performImp() throws IOException {
        WebAbstraction web = wiContext.getWebAbstraction();

        Map parameters = (Map)Authentication.getAuthenticationState(web).getParameters();
        AccessToken credentials = (AccessToken)parameters.get(Constants.AGE_ACCESS_TOKEN);

        if (credentials != null) {
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
                    // authentication operation.  The result is null; break to
                    // allow it to be processed.
                    cleanUpAfterFailedAuth();
                    return false;
                }
            }

            if (result == null) {
                cleanUpAfterFailedAuth();
                return false;
            }

            if (result.getValidationResult().isSuccess()) {
                // Authentication successful
                Authentication.getAuthenticationState(wiContext.getWebAbstraction()).setAuthenticated(credentials);

                // Save WebPN we're using for future calls
                DisasterRecoveryUtils.setWebPN(wiContext, webPN);

                // Create a UserContext to hold user-specific app state; this
                // is null if the creation failed, which indicates some horrible
                // problem has arisen somewhere.
                UserContext userContext = SessionUtils.createNewUserContext(wiContext);
                if (userContext == null) {
                    cleanUpAfterFailedAuth();
                    return false;
                }

                // Might need to warn the user about impending password expiry
                AGAuthPoint agAuthPoint = (AGAuthPoint) wiContext.getConfiguration().getAuthenticationConfiguration().getAuthPoint();
                PasswordExpiryWarningPolicy expiryWarningPolicy = agAuthPoint.getPasswordExpiryWarnPolicy();

                AllowChangePassword acp = Authentication.getChangePasswordPolicy(wiContext);
                if (acp == AllowChangePassword.ALWAYS && expiryWarningPolicy.isUserNotificationRequired(result)) {
                    // Forward to the change password warning page
                    PageHistory.addToPostLoginPages(wiContext.getWebAbstraction(), Constants.PAGE_PASSWORD_EXPIRY_WARN);
                    PasswordExpiryWarningUtils.setDaysUntilExpiry(wiContext, result.getDaysUntilPasswordExpiry());
                }

                Authentication.redirectToCurrentAuthPage(wiContext);
                return false;
            } else if (AccessTokenValidationResult.FAILED_SECRET_EXPIRED.equals(result.getValidationResult())) {
                if (Authentication.isChangePasswordAllowed(wiContext)) {
                    Authentication.addPageToQueueHead(wiContext, "changepassword", parameters);
                    Authentication.redirectToNextAuthPage(wiContext);
                    return false;
                }
            }
        }

        cleanUpAfterFailedAuth();

        return false;
    }

    private void cleanUpAfterFailedAuth() throws IOException {
        wiContext.getWebAbstraction().abandonSession();
        wiContext.getWebAbstraction().setResponseStatus( WebAbstraction.SC_UNAUTHORIZED );
        wiContext.getWebAbstraction().writeToResponse(wiContext.getString( AGEUtilities.KEY_AG_AUTH_ERROR ));
    }
}
