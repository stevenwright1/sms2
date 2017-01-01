/*
 * Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils;

import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.types.AllowChangePassword;

/**
 * This class contains some utility methods related to password expiry warning messages.
 */
public class PasswordExpiryWarningUtils {

    // The expiry information is only valid for this login session, so store as a session variable
    private static final String SV_DAYS_UNTIL_PWD_EXP = "CTX_DaysUntilPwdExp";

    /**
     * Sets the number of days until the password will expire.
     *
     * @param wiContext the WI context
     * @param days the number of days (can be zero)
     */
    public static void setDaysUntilExpiry(WIContext wiContext, int days) {
        wiContext.getWebAbstraction().setSessionAttribute(SV_DAYS_UNTIL_PWD_EXP, new Integer(days));
    }

    /**
     * Gets the number of days until the password will expire.
     *
     * @param wiContext the WI context
     * @return the number of days (can be zero, indicating that the password will expire in a few hours)
     */
    public static int getDaysUntilExpiry(WIContext wiContext) {
        Integer days = (Integer)wiContext.getWebAbstraction().getSessionAttribute(SV_DAYS_UNTIL_PWD_EXP);
        return days.intValue();
    }

    /**
     * Clears the password expiry so that no warning will be generated.
     *
     * @param wiContext the WI context
     */
    public static void clearExpiry(WIContext wiContext) {
        wiContext.getWebAbstraction().setSessionAttribute(SV_DAYS_UNTIL_PWD_EXP, null);
    }

    /**
     * Determine whether to warn the user of an imminent password expiry.
     *
     * @param wiContext the WI context
     * @return <code>true</code> if a warning should be given, else <code>false</code>
     */
    public static boolean warnUser(WIContext wiContext) {
        WebAbstraction web = wiContext.getWebAbstraction();
        boolean warn = false;

        if (Include.isLoggedIn(web) && (web.getSessionAttribute(SV_DAYS_UNTIL_PWD_EXP) != null)) {
            if (AllowChangePassword.ALWAYS.equals(Authentication.getChangePasswordPolicy(wiContext))) {
                warn = true;
            }
        }

        return warn;
    }

    /**
     * Returns a sentence describing when the password will expire eg.
     * "Your password will expire within the next 2 days."
     *
     * @param wiContext the WI context
     * @return the sentence
     */
    public static String getExpiryDaysAsSentence(WIContext wiContext) {
        int daysUntilPwdExpiry = getDaysUntilExpiry(wiContext);
        String expiryMessage = wiContext.getString("PwdExpWarnBodyPre");
        if (0 == daysUntilPwdExpiry) {
            expiryMessage += " " + wiContext.getString("PwdExpWarnBodyPostHours");
        } else {
            if (1 == daysUntilPwdExpiry) {
                expiryMessage += " " + wiContext.getString("PwdExpWarnBodyPostDay");
            } else {
                expiryMessage += " " + daysUntilPwdExpiry + " " + wiContext.getString("PwdExpWarnBodyPostDays");
            }
        }

        return expiryMessage;
    }
}
