/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils;

import java.util.HashMap;
import com.citrix.wi.accountselfservice.AccountTask;
import com.citrix.wi.config.AuthenticationConfiguration;
import com.citrix.wi.config.WIConfiguration;
import com.citrix.wi.config.auth.ExplicitAuth;
import com.citrix.wi.config.auth.ExplicitUDPAuth;
import com.citrix.wi.config.auth.WIAuthPoint;
import com.citrix.wi.controls.LoginPageControl;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.types.AllowChangePassword;
import com.citrix.wi.types.CredentialFormat;
import com.citrix.wi.types.WIAuthType;

/**
 * This class holds some constants and utility functions related to
 * AccountSelfService functionality.
 */
public class AccountSelfService {

    public static final String  COOKIE_ACCOUNT_SS_AUTH  = "WIAccountSSAuthId";
    public static final String  ID_ANSWER               = "answer";
    public static final String  ID_CONFIRM_ANSWER       = "answerConfirm";
    public static final String  ID_CONFIRM_PASSWORD     = "passwordConfirm";
    public static final String  ID_LABEL_ACCOUNT_UNLOCK = "labelAccountUnlock";
    public static final String  ID_LABEL_PASSWORD_RESET = "labelPasswordReset";
    public static final String  ID_NEW_PASSWORD         = "passwordNew";
    public static final String  ID_RADIO_ACCOUNT_UNLOCK = "radioAccountUnlock";
    public static final String  ID_RADIO_PASSWORD_RESET = "radioPasswordReset";
    public static final String  ID_TASK                 = "taskselection";
    public static final String  KEY_ACCOUNT_LOCKED_OUT  = "AccountLockedOut";
    public static final String  KEY_INVALID_CREDENTIALS = "InvalidCredentials";
    public static final String  ID_BUTTON_FINISH        = "btnFinish";

    // Authentication state constants
    public static final String  VAL_TASK                = "task";
    public static final String  VAL_ACCOUNT_SS_CONTEXT  = "AccountSSContext";

    // Session state constants
    private static final String SV_PASSWORD_WAS_RESET   = "PasswordWasReset";
    private static final String SV_ACCOUNT_SS_AUTH_ID   = "CTX_AccountSSAuthId";

    /**
     * Gets additional text, if applicable, to append to a message in the
     * Message Center. The additional text will provide a context-sensitive
     * suggestion to use Account Self Service.
     *
     * @param wiContext the current WIContext
     * @param messageKey the key to evaluate
     * @return additional text as a string, or null if none is applicable.
     */
    public static String getAccountSelfServiceSuffix(WIContext wiContext, String messageKey) {
        String suffix = null;

        // Construct a link to start Account Self Service
        String selfServiceLink = "<a id=\"loginAcctSSLink\" href=\"" + Constants.PAGE_LOGIN + "?"
                        + Constants.QSTR_START_SELF_SERVICE + "=" + Constants.VAL_ON + "\">"
                        + wiContext.getString("AccountSelfService") + "</a>";

        if (isAccountUnlockEnabled(wiContext.getConfiguration()) && messageKey.equals(KEY_ACCOUNT_LOCKED_OUT)) {

            // User should be told that they can use Account Self Service to
            // unlock their account.
            suffix = wiContext.getString("ToUnlockAccountMessage", selfServiceLink);
        }

        return suffix;
    }

    /**
     * Gets whether account self service is enabled, both in configuration and
     * connection type.
     *
     * @return true if enabled, otherwise false
     */
    public static boolean isAccountSelfServiceEnabled(WIContext wiContext) {
        return (Include.isClientConnectionSecure(wiContext.getUserEnvironmentAdaptor()) && isAccountSelfServiceConfigEnabled(wiContext));
    }

    /**
     * Gets whether account self service is enabled in the configuration.
     *
     * @return true if enabled, otherwise false
     */
    public static boolean isAccountSelfServiceConfigEnabled(WIContext wiContext) {
        return (isAccountUnlockEnabled(wiContext.getConfiguration()) || isPasswordResetEnabled(wiContext));
    }

    /**
     * Gets the key for the string used for the account self service link
     *
     * @param the key for the string
     */
    public static String getAccountSelfServiceLinkKey(WIContext wiContext) {
        if (isAccountUnlockEnabled(wiContext.getConfiguration()) && isPasswordResetEnabled(wiContext)) {
            return "AccountSSLink";
        } else if (isAccountUnlockEnabled(wiContext.getConfiguration())) {
            return "AccountSSLinkUnlock";
        } else if (isPasswordResetEnabled(wiContext)) {
            return "AccountSSLinkReset";
        } else {
            return "AccountSSLink";
        }
    }

    /**
     * Gets whether self service account unlock is enabled
     *
     * @param wiConfig the current WI Configuration
     * @return true if enabled, otherwise false
     */
    public static boolean isAccountUnlockEnabled(WIConfiguration wiConfig) {
        AuthenticationConfiguration authConfig = wiConfig.getAuthenticationConfiguration();
        if (authConfig.getAuthPoint() instanceof WIAuthPoint) {
            WIAuthPoint wiAuthPoint = (WIAuthPoint)authConfig.getAuthPoint();
            ExplicitAuth expAuth = (ExplicitAuth)wiAuthPoint.getAuthMethod(WIAuthType.EXPLICIT);
            return ((expAuth != null) && (expAuth instanceof ExplicitUDPAuth)
                            && (((ExplicitUDPAuth)expAuth).getCredentialFormat() != CredentialFormat.UPN) && wiConfig
                            .getAccountSelfServiceConfiguration().getAllowAccountUnlock());
        }

        return false;
    }

    /**
     * Gets whether self service password reset is enabled.
     *
     * @param wiContext the WI context
     * @return true if enabled, otherwise false
     */
    public static boolean isPasswordResetEnabled(WIContext wiContext) {
        WIConfiguration wiConfig = wiContext.getConfiguration();
        AuthenticationConfiguration authConfig = wiConfig.getAuthenticationConfiguration();

        if (authConfig.getAuthPoint() instanceof WIAuthPoint) {
            WIAuthPoint wiAuthPoint = (WIAuthPoint)authConfig.getAuthPoint();
            ExplicitAuth expAuth = (ExplicitAuth)wiAuthPoint.getAuthMethod(WIAuthType.EXPLICIT);
            return (expAuth != null) && (expAuth instanceof ExplicitUDPAuth)
                            && !CredentialFormat.UPN.equals(((ExplicitUDPAuth)expAuth).getCredentialFormat())
                            && AllowChangePassword.ALWAYS.equals(Authentication.getChangePasswordPolicy(wiContext))
                            && !TwoFactorAuth.isPasswordIntegrationEnabled(wiConfig)
                            && wiConfig.getAccountSelfServiceConfiguration().getAllowPasswordReset();
        }

        return false;
    }

    /**
     * Disables the login button for account self service pages
     *
     * @param viewControl The <code>FooterControl</code>.
     */
    public static void populate(LoginPageControl viewControl) {
        viewControl.setShowLoginButton(false);
    }

    /**
     * Record that password was reset
     */
    public static void recordPasswordWasReset(WIContext wiContext) {
        wiContext.getWebAbstraction().setSessionAttribute(SV_PASSWORD_WAS_RESET, Boolean.TRUE);
    }

    /**
     * Return true if password reset has been recorded
     */
    public static boolean wasPasswordReset(WIContext wiContext) {
        return Boolean.TRUE.equals(wiContext.getWebAbstraction().getSessionAttribute(SV_PASSWORD_WAS_RESET));
    }

    /**
     *
     */
    public static String getRecordedAuthId(WIContext wiContext) {
        return (String)wiContext.getWebAbstraction().getSessionAttribute(SV_ACCOUNT_SS_AUTH_ID);
    }

    public static void recordAuthId(WIContext wiContext, String authId) {
        wiContext.getWebAbstraction().setSessionAttribute(SV_ACCOUNT_SS_AUTH_ID, authId);
    }

    /**
     * Build the Authentication filter page queue for the pages to visit during
     * the account self service, defined by the task that the user will be
     *
     * @param wiContext The WI context.
     * @param task The Account Self Service Task to be performed (Password
     * reset/Unlock account)
     */
    public static void BuildAuthenticationFilterQueue(WIContext wiContext, AccountTask task) {
        // Store the task for later
        HashMap parameters = new HashMap();
        parameters.put(AccountSelfService.VAL_TASK, task);

        // Build the list of pages required for account self service
        if (task == AccountTask.ACCOUNT_UNLOCK) {
            Authentication.addPageToQueueHead(wiContext, "account_ss_unlock", parameters);
        } else if (task == AccountTask.PASSWORD_RESET) {
            Authentication.addPageToQueueHead(wiContext, "account_ss_reset", parameters);
        }

        Authentication.addPageToQueueHead(wiContext, "account_ss_qba", parameters);
        Authentication.addPageToQueueHead(wiContext, "account_ss_verify", parameters);
        Authentication.addPageToQueueHead(wiContext, "account_ss_user", parameters);
    }

}
