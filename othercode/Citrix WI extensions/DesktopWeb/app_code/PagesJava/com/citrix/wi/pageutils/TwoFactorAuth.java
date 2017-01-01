/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils;

import com.citrix.authentication.tokens.AccessToken;
import com.citrix.authentication.tokens.NDSCredentials;
import com.citrix.authentication.tokens.UPNCredentials;
import com.citrix.authentication.tokens.UserDomainPasswordCredentials;
import com.citrix.wi.config.AuthenticationConfiguration;
import com.citrix.wi.config.WIConfiguration;
import com.citrix.wi.config.auth.ExplicitAuth;
import com.citrix.wi.config.auth.SecurIDAuth;
import com.citrix.wi.config.auth.TwoFactorAuthMethod;
import com.citrix.wi.config.auth.WIAuthPoint;
import com.citrix.wi.types.WIAuthType;

/**
 * This contains constants used to identify HTML elements related to 2-factor
 * authentication and some utility methods for two factor authentication
 */
public class TwoFactorAuth {

    // Constants related to two-factor authentication pages
    public static final String ID_CHALLENGE_RESPONSE            = "ID_CHALLENGE_RESPONSE";
    public static final String ID_CHALLENGE_RESPONSE_MAX_LENGTH = "256";
    public static final String ID_PASSWORD_CHALLENGE            = "PASSWORD_CHALLENGE";
    public static final String ID_PIN_TYPE                      = "type";
    public static final String ID_PIN1                          = "PIN1";
    public static final String ID_PIN2                          = "PIN2";
    public static final String ID_TOKENCODE                     = "tokencode";
    public static final String PASSCODE_ENTRY_MAX_LENGTH        = "256";
    public static final String TOKENCODE_ENTRY_MAX_LENGTH       = "256";
    public static final String VAL_SYSTEM                       = "system";
    public static final String VAL_USER                         = "user";
    public static final String VAL_PASSWORD_FROM_CHALLENGE      = "passwordFromChallenge";

    // RADIUS codes
    public static final String RADIUS_SUCCESS                   = "SUCCESS";                                             // RADIUSAuthenticator.RET_SUCCESS??
    public static final String RADIUS_INVALID                   = "INVALID";
    public static final String RADIUS_FAILED                    = "FAILED";

    // Safeword codes
    public static final String SAFEWORD_SUCCESS                 = "SUCCESS";
    public static final String SAFEWORD_INVALID                 = "INVALID";

    // RAIDUS Challenges (for SecurID servers)
    public static final String RADIUS_CHANGE_PIN_USER           = "CHANGE_PIN_USER";
    public static final String RADIUS_NEXT_TOKENCODE            = "NEXT_TOKENCODE";
    public static final String RADIUS_SYSTEM_PIN_READY          = "SYSTEM_PIN_READY";
    public static final String RADIUS_CHANGE_PIN_EITHER         = "CHANGE_PIN_EITHER";
    public static final String RADIUS_NO                        = "n";
    public static final String RADIUS_YES                       = "y";

    // SecurID challenges
    public static final String SECURID_NEXT_TOKENCODE           = "NEXT_TOKENCODE";
    public static final String SECURID_CHANGE_PIN_USER          = "CHANGE_PIN_USER";
    public static final String SECURID_CHANGE_PIN_SYSTEM        = "CHANGE_PIN_SYSTEM";
    public static final String SECURID_CHANGE_PIN_EITHER        = "CHANGE_PIN_EITHER";
    public static final String SECURID_SUCCESS                  = "SUCCESS";
    public static final String SECURID_INVALID                  = "INVALID";

    // Values for the authentication filter params
    public static final String VAL_CHALLENGE                    = "challenge";
    public static final String VAL_AUTHENTICATOR                = "authenticator";
    public static final String VAL_SYSTEM_PIN                   = "systemPin";

    // Values for form parameters
    public static final String VAL_PASSCODE                     = "Passcode";

    // Application attribute
    public static final String VAL_RADIUS_AUTHENTICATOR_FACTORY = "com.citrix.authenticators.RADIUSAuthenticatorFactory";

    // Constants for the names of the two-factor pages
    public static final String PAGE_CHANGE_PIN_WARNING          = "change_pin_warning";
    public static final String PAGE_CHANGE_PIN_USER             = "change_pin_user";
    public static final String PAGE_CHANGE_PIN_SYSTEM           = "change_pin_system";
    public static final String PAGE_GET_PASSWORD                = "get_password";
    public static final String PAGE_NEXT_TOKENCODE              = "next_tokencode";
    public static final String PAGE_CHANGE_PIN_EITHER           = "change_pin_either";
    public static final String PAGE_CHALLENGE                   = "challenge";
    public static final String PAGE_PASSWORD_CHALLENGE          = "password_challenge";

    // Message Keys
    public final static String KEY_PIN_NOT_CHANGED              = "PINNotChanged";
    public final static String KEY_PIN_CHANGED                  = "PINChanged";
    public final static String KEY_PIN_REJECTED                 = "PINRejected";
    public final static String KEY_PIN_NO_MATCH                 = "PINNoMatch";
    public final static String KEY_MUST_ENTER_PIN               = "MustEnterPIN";
    public final static String KEY_MUST_ENTER_TOKENCODE         = "MustEnterTokencode";

    /**
     * Returns the user represented in the AccessToken in a format that will be
     * sent to the RSA Authentication server. If the fullyQualified flag
     * instanceof set to true, then (for example) Windows - based tokens will
     * return a username consinstanceofting of both the domain and username.
     */
    public static String getUserName(AccessToken token, boolean fullyQualified) {
        String result = "";
        if (token instanceof NDSCredentials) {
            result = getUserName((NDSCredentials)token, fullyQualified);
        } else if (token instanceof UPNCredentials) {
            result = getUserName((UPNCredentials)token, fullyQualified);
        } else if (token instanceof UserDomainPasswordCredentials) {
            result = getUserName((UserDomainPasswordCredentials)token, fullyQualified);
        } else {
            result = token.getShortUserName();
        }

        return result;
    }

    /**
     * Returns an NDS user '.user.ctx1.ctxn' in the NDS tree 'TREE' as
     * 'TREE\.user.ctx1.ctxn' if the fullyQualified instanceof true, else
     * 'user'.
     */
    public static String getUserName(NDSCredentials token, boolean fullyQualified) {
        if (fullyQualified) {
            return token.getTree() + "\\" + token.getName();
        } else {
            return token.getShortUserName();
        }
    }

    /**
     * Returns a UPN 'user@domain.com' as 'domain \ user' if the fullyQualified
     * instanceof true, else 'user'.
     */
    public static String getUserName(UPNCredentials token, boolean fullyQualified) {
        if (fullyQualified) {
            return token.getShortDomain() + "\\" + token.getShortUserName();
        } else {
            return token.getShortUserName();
        }
    }

    /**
     * Returns a user 'user' with the domain 'domain' as 'domain \ user' if the
     * fullyQualified instanceof true, else 'user'.
     */
    public static String getUserName(UserDomainPasswordCredentials token, boolean fullyQualified) {
        if (fullyQualified) {
            return token.getUserIdentity();
        } else {
            return token.getUser();
        }
    }

    /**
     * Tests whether the Two Factor Password Integration feature is enabled.
     *
     * @return <code>true</code> if enabled, else <code>false</code>.
     */
    public static boolean isPasswordIntegrationEnabled(WIConfiguration wiConfig) {
        boolean result = false;

        if (Features.isSecurIDSupported()) {
            Object obj = TwoFactorAuth.getTwoFactorAuthMethod(wiConfig);
            if (obj != null && obj instanceof SecurIDAuth) {
                result = ((SecurIDAuth)obj).getUsePasswordIntegration();
            }
        }

        return result;
    }

    /**
     * Tests whether fully-qualified usernames (that is distinguished by domain
     * or NDS context) are to be used when authenticating users against 2-factor
     * authentication systems such as RSA SecurID where users are stored in a
     * proprietary database as opposed to (say) Active Directory.
     *
     * @return <code>true</code> if enabled, else <code>false</code>.
     */
    public static boolean useTwoFactorFullyQualifiedUserNames(WIConfiguration wiConfig) {
        boolean result = false;

        if (Features.isSecurIDSupported()) {
            Object obj = getTwoFactorAuthMethod(wiConfig);
            if (obj != null && obj instanceof SecurIDAuth) {
                result = ((SecurIDAuth)obj).getUseFullyQualifiedUsernames();
            }
        }

        return result;
    }

    /**
     * Returns the Two Factor Authentication method that is configured.
     *
     * @return a TwoFactorAuthMethod
     */
    public static TwoFactorAuthMethod getTwoFactorAuthMethod(WIConfiguration wiConfig) {
        AuthenticationConfiguration authConfig = wiConfig.getAuthenticationConfiguration();
        if (authConfig.getAuthPoint() instanceof WIAuthPoint) {
            WIAuthPoint wiAuthPoint = (WIAuthPoint)authConfig.getAuthPoint();
            ExplicitAuth expAuth = (ExplicitAuth)wiAuthPoint.getAuthMethod(WIAuthType.EXPLICIT);
            return getTwoFactorAuthMethod(expAuth);
        }

        return null;
    }

    /**
     * Returns the Two Factor Authentication method for the specified explicit
     * auth method.
     *
     * @return a TwoFactorAuthMethod
     */
    public static TwoFactorAuthMethod getTwoFactorAuthMethod(ExplicitAuth expAuth) {
        return (expAuth != null ? expAuth.getTwoFactorAuth() : null);
    }
}