/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils;

import java.util.HashSet;
import java.util.Set;

import com.citrix.wi.config.Configuration;

import com.citrix.wing.AccessTokenException;
import com.citrix.wing.AccessTokenInvalidException;
import com.citrix.wing.DesktopUnavailableException;
import com.citrix.wing.NoSuchResourceException;
import com.citrix.wing.PasswordExpiredException;
import com.citrix.wing.types.AccessTokenValidationResult;
import com.citrix.wing.types.DesktopUnavailableErrorType;
import com.citrix.wing.webpn.DocumentInfo;
import com.citrix.wing.webpn.FarmBinding;
import com.citrix.wing.util.Strings;


public class Utils {

    /**
     * Returns a URL for a document in a format suitable for all browsers
     */
    public static String documentURLForBrowser(DocumentInfo docInfo) {
        String docURL = docInfo.getDocumentURL();
        if (docURL.startsWith("\\\\")) {
            // This helps firefox and netscape to generates proper UNC path
            // so that user can create shortcuts using these browsers
            docURL = docURL.substring(2, docURL.length());
            docURL = docURL.replace('\\', '/');
            docURL = "file://///" + docURL;
        }
        return docURL;
    }

    /**
     * Get a message key that describes a specific AccessTokenException failure.
     * This message key can then be used to look up a suitable message in the
     * resource bundle to describe the specific failure that has occurred. In
     * the event that no extra information can be obtained from the
     * AccessTokenException (e.g. if the AccessTokenException is an instance of
     * an unexpected type) then the key returned is "GeneralCredentialsFailure".
     * This describes a general authentication failure scenario and is the best
     * that can be done under the circumstances.
     *
     * @param ate an AccessTokenException
     * @return a message key string referring to a message that describes the
     * AccessTokenException
     */
    public static String getAuthErrorMessageKey(AccessTokenException ate) {
        String invalidTokenMessageKey = null;

        if (ate instanceof PasswordExpiredException) {
            // The AccessTokenException is an instance of a
            // PasswordExpiredException.
            // Assign an appropriate message key for the specific exception.
            // This
            // occurs if the token validation result is
            // AccessTokenValidationResult.FAILED_SECRET_EXPIRED.
            // Failure: password has expired
            // Likely cause:
            // TransactionStatusException.ErrorId_CharlotteErrorCredentialsMustChange
            invalidTokenMessageKey = "CredentialsMustChange";
        } else if (ate instanceof AccessTokenInvalidException) {
            // The AccessTokenException is an instance of a
            // AccessTokenInvalidException.
            // Get a message key based on the reason the atie is invalid.
            invalidTokenMessageKey = getAuthErrorMessageKey(((AccessTokenInvalidException)ate).getReason());
        } else {
            // The specific AccessTokenException has not been recognised
            // and the message key is null. Set the key to refer to
            // a general authentication error.
            // This will occur if the AccessTokenException is an instance of
            // an unrecognised type.
            invalidTokenMessageKey = "GeneralCredentialsFailure";
        }
        return invalidTokenMessageKey;
    }

    /**
     * Get a message key that describes a specific AccessTokenValidationResult.
     * This message key can then be used to look up a suitable message in the
     * resource bundle to describe the specific failure that has occurred. In
     * the event that no useful information can be obtained from the
     * AccessTokenValidationResult (e.g. if it is null or an unexpected value)
     * then the key returned is "GeneralCredentialsFailure". This describes a
     * general authentication failure scenario and is the best that can be done
     * under the circumstances.
     *
     * @param atvr an AccessTokenValidationResult
     * @return a message key string referring to a message that describes the
     * AccessTokenValidationResult
     */
    public static String getAuthErrorMessageKey(AccessTokenValidationResult atvr) {
        String invalidTokenMessageKey = null;

        if (atvr == AccessTokenValidationResult.FAILED) {
            // Failure: access token is invalid.
            // Likely cause:
            // TransactionStatusException.ErrorId_CharlotteErrorBadCredentials
            invalidTokenMessageKey = "InvalidCredentials";
        } else if (atvr == AccessTokenValidationResult.FAILED_ACCOUNT_EXPIRED) {
            // Failure: account expired.
            // Likely cause:
            // TransactionStatusException.ErrorId_CharlotteErrorCredentialsExpired
            invalidTokenMessageKey = "CredentialsExpired";
        } else if (atvr == AccessTokenValidationResult.FAILED_ACCOUNT_LOCKED_OUT) {
            // Failure: account locked out.
            // Likely cause:
            // TransactionStatusException.ErrorId_CharlotteErrorAccountLockedOut
            invalidTokenMessageKey = "AccountLockedOut";
        } else if (atvr == AccessTokenValidationResult.FAILED_ACCOUNT_DISABLED) {
            // Failure: account disabled.
            // Likely cause:
            // TransactionStatusException.ErrorId_CharlotteErrorAccountDisabled
            invalidTokenMessageKey = "AccountDisabled";
        } else if (atvr == AccessTokenValidationResult.FAILED_OUTSIDE_LOGIN_HOURS) {
            // Failure: outside of login period.
            // Likely cause:
            // TransactionStatusException.ErrorId_CharlotteErrorInvalidLogonHours
            invalidTokenMessageKey = "InvalidLogonHours";
        } else if (atvr == AccessTokenValidationResult.FAILED_SECRET_EXPIRED) {
            // Failure: password expired.
            // Likely cause:
            // TransactionStatusException.ErrorId_CharlotteErrorCredentialsMustChange
            // In reality this should be a PasswordExpiredException but this
            // case
            // is here to protect against it being incorrectly raised as an
            // AccessTokenInvalidException.
            invalidTokenMessageKey = "CredentialsMustChange";
        } else if (atvr == AccessTokenValidationResult.FAILED_NOT_LICENSED) {
            // Failure: user specific license.
            // Likely cause: TransactionStatusException.ErrorId_NotLicensed
            invalidTokenMessageKey = "NotLicensed";
        } else {
            // Failure: - unknown/unrecognised/null reason
            // Likely cause: unknown
            // All that can be said is that there is an "authentication
            // problem".
            invalidTokenMessageKey = "GeneralCredentialsFailure";
        }
        return invalidTokenMessageKey;
    }

    /**
     * Get a message key that describes a specific DesktopUnvailableException
     * failure during an application launch.
     *
     * This message key can then be used to look up a suitable message in the
     * resource bundle to describe the specific failure that has occurred. In
     * the event that no extra information can be obtained from the
     * DesktopUnavailableException then the key returned is "ResourceError".
     *
     * @param due a DesktopUnavailableException
     * @return a message key string referring to a message that describes the
     * exception
     */
    public static String getDesktopErrorMessageKey(DesktopUnavailableException due) {
        String errorKey = Constants.CONST_RESOURCE_ERROR; // Default to generic response

        DesktopUnavailableErrorType errorType = due.getErrorType();

        if (errorType == DesktopUnavailableErrorType.NO_AVAILABLE_WORKSTATION) {
            // No workstations available for the user
            errorKey = "UnavailableDesktop";
        } else if (errorType == DesktopUnavailableErrorType.CONNECTION_REFUSED) {
            // Workstation refused a connection - usually a temporary issue
            errorKey = "CouldNotConnectToWorkstation";
        } else if (errorType == DesktopUnavailableErrorType.WORKSTATION_IN_MAINTENANCE) {
            // Workstation in maintenance mode
            errorKey = "WorkstationInMaintenance";
        }

        return errorKey;
    }

    /**
     * Gets a message key that describes a specific failure during a desktop
     * power off operation.
     *
     * If no specific message could be found, a default message key is returned.
     *
     * @param e the exception that occurred during the power off operation
     * @return message key string
     */
    public static String getPowerOffErrorMessageKey(Exception e) {
        String errorKey = "DesktopPowerOffError";

        if (e instanceof AccessTokenException) {
            errorKey = Utils.getAuthErrorMessageKey((AccessTokenException)e);
        } else if (e instanceof NoSuchResourceException) {
            errorKey = "AppRemoved";
        } else if (e instanceof DesktopUnavailableException) {
            DesktopUnavailableErrorType errorType = ((DesktopUnavailableException)e).getErrorType();
            if (errorType == DesktopUnavailableErrorType.WORKSTATION_IN_MAINTENANCE) {
                errorKey = "WorkstationInMaintenancePowerOff";
            }
        }

        return errorKey;
    }

    /**
     * Compares two objects for equality, handling nulls
     *  (if both null, the values are treated as being equal).
     * @param v1 first object
     * @param v2 second object
     * @return true if v1 and v2 are both null or if v1.equals(v2)
     */
    public static boolean safeEquals(Object v1, Object v2) {
        return (v1 == null ? v2 == null : v1.equals(v2));
    }

    /**
     * toString method that can be used by ASP or JSP pages.
     */
    public static String toString(Object obj) {
        String retVal = "";
        if (obj != null) {
            retVal = obj.toString();
        }
        return retVal;
    }

    /**
     * Parse a POST-data On/Off item. Result is a tri-state.
     */
    public static Boolean parseOnOffPostValue(String str) {
        Boolean result;
        if (Strings.equalsIgnoreCase(Constants.VAL_ON, str)) {
            result = Boolean.TRUE;
        } else if (Strings.equalsIgnoreCase(Constants.VAL_OFF, str)) {
            result = Boolean.FALSE;
        } else {
            result = null;
        }
        return result;
    }

}