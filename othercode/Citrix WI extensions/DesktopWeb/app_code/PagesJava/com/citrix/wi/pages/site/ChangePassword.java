/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.site;

import java.io.IOException;
import java.net.UnknownHostException;

import com.citrix.authentication.tokens.AccessToken;
import com.citrix.wi.config.AuthenticationConfiguration;
import com.citrix.wi.config.auth.ExplicitAuth;
import com.citrix.wi.config.auth.WIAuthPoint;
import com.citrix.wi.controls.ChangePasswordPageControl;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pages.StandardLayout;
import com.citrix.wi.pageutils.AGEUtilities;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.DisasterRecoveryUtils;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.LaunchUtilities;
import com.citrix.wi.pageutils.PageHistory;
import com.citrix.wi.pageutils.PasswordExpiryWarningUtils;
import com.citrix.wi.pageutils.TwoFactorAuth;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wi.types.AGEAccessMode;
import com.citrix.wi.types.AllowChangePassword;
import com.citrix.wi.types.WIAuthType;
import com.citrix.wing.MessageType;
import com.citrix.wing.SourceIOException;
import com.citrix.wing.SourceUnavailableException;
import com.citrix.wing.UserEnvironmentAdaptor;
import com.citrix.wing.types.ChangePasswordResult;
import com.citrix.wing.util.Strings;
import com.citrix.wing.webpn.WebPN;

public class ChangePassword extends StandardLayout {

    protected ChangePasswordPageControl viewControl             = new ChangePasswordPageControl();

    // Key for server variable indicating whether user came from account
    // settings
    protected static final String         FROM_ACCOUNT_SETTINGS   = "fromAccountSettings";

    // Key for session variable storing the change password confirm required
    // flag
    protected static final String       CHANGE_PASSWORD_CONFIRM = "changePasswordConfirm";

    public ChangePassword(WIContext wiContext) {
        super(wiContext);
        wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
    }

    protected boolean performImp() throws IOException {

        WebAbstraction web = wiContext.getWebAbstraction();

        if (!checkUserAllowedToChangePassword()) {
            // if the user is not allowed to change their password
            // then the above sets up the appropriate redirect
            // so we return false
            return false;
        }

        if (AGEUtilities.getAGEAccessMode(wiContext) == AGEAccessMode.DIRECT) {
            // When Web Interface is integrated with Access Gateway and using
            // direct mode, password change requests are handled by an Access
            // Gateway page.
            redirectToAGChangePasswordPage(wiContext);
            return false;
        }

        if (web.isPostRequest()) {
            // A change password request or confirmation has been received

            String submitMode = web.getFormParameter(Constants.ID_SUBMIT_MODE);

            if (Constants.VAL_OK.equalsIgnoreCase(submitMode)) {
                // User pressed ok, so try and change the password
                attemptChangePassword(web);
            } else {
                // User has already changed the password successfully
                // and is Ok'ing the change confirmation page, or has pressed
                // the Cancel button on the change page

                if (Constants.VAL_CONFIRM.equalsIgnoreCase(submitMode)) {
                    web.setSessionAttribute(CHANGE_PASSWORD_CONFIRM, null);
                }

                PageHistory.getCurrentPageURL(web, true);

                // redirect the appropriate page
                redirectToNextPage();
            }

            // we always redirect elsewhere
            return false;

        } else {
            // store if we need to return to the account settings if it was a
            // sucess
            web.setSessionAttribute(FROM_ACCOUNT_SETTINGS, web
                            .getQueryStringParameter(Constants.QSTR_FROM_ACCOUNT_SETTINGS));

            layoutControl.formAction = Constants.FORM_POSTBACK;

            recordCurrentPageURL();

            // Determine and store the UI mode
            if (Constants.VAL_TRUE.equals(web.getSessionAttribute(CHANGE_PASSWORD_CONFIRM))) {
                viewControl.setConfirmOnly(true);
            } else {
                // Override the navcontrol settings if in direct mode
                if (!LaunchUtilities.getDirectLaunchModeInUse(wiContext)
                                && !PasswordExpiryWarningUtils.warnUser(wiContext)) {
                    navControl.setAllowChangePassword(false);
                }

                welcomeControl.setBody(wiContext.getString("ChangePassword3"));
            }

            welcomeControl.setTitle(wiContext.getString("ScreenTitleChangePassword"));

            // render the form
            return true;
        }
    }

    /**
     * Terminates the user's Web Interface session and redirects to the Access
     * Gateway change password page.
     * 
     * @param wiContext the WI context
     */
    protected void redirectToAGChangePasswordPage(WIContext wiContext) {
        // Redirect to the AG change password page
        String changePasswordUrl = AGEUtilities.getAGEChangePasswordUrl(wiContext);
        wiContext.getWebAbstraction().clientRedirectToUrl(changePasswordUrl);

        // Save any state changes
        UserEnvironmentAdaptor envAdaptor = wiContext.getUserEnvironmentAdaptor();
        if (!envAdaptor.isCommitted()) {
            envAdaptor.commitState();
            envAdaptor.destroy();
        }

        // Terminate the WI session
        wiContext.getWebAbstraction().abandonSession();
    }

    /**
     * Attempts to perform the change password
     *
     * @param web
     * @return true if the page should be rendered
     * @throws IOException
     */
    protected void attemptChangePassword(WebAbstraction web) throws IOException {
        String passwordOld = web.getFormParameter("password");
        String passwordNew = web.getFormParameter("passwordNew");
        String passwordConfirm = web.getFormParameter("passwordConfirm");

        // Make sure none of the posted fields contain control characters
        if (Strings.hasControlChars(passwordOld) || Strings.hasControlChars(passwordNew)
                        || Strings.hasControlChars(passwordConfirm)) {
            handleError(MessageType.ERROR, "ChangePasswordFailed");
        } else if (TwoFactorAuth.isPasswordIntegrationEnabled(wiContext.getConfiguration())
                        && (passwordNew.length() < 1 || passwordNew.length() > Constants.PASSWORD_ENTRY_MAX_LENGTH)) {
            handleError(MessageType.ERROR, "InvalidPasswordChallengeResponse");
        } else if (passwordNew.equals(passwordConfirm)) {

            AccessToken credentials = getCredentials();
            if (credentials == null) {
                throw new java.lang.NullPointerException("Credentials must not be null");
            }

            changePassword(web, credentials, passwordOld, passwordNew);
        } else {
            // Password is different, show error message
            handleError(MessageType.ERROR, "ChangePasswordConfirmDifferent");
        }
    }

    protected void changePassword(WebAbstraction web, AccessToken credentials, String passwordOld, String passwordNew) throws IOException {
        WebPN webPN = DisasterRecoveryUtils.getWebPN(wiContext);

        // Do change password performs the change password,
        // and redirects with the appropriate error if it fails
        try {
            if (doChangePassword(webPN, credentials, passwordOld, passwordNew)) {
                // Clear the fact that we came the account settings page
                web.setSessionAttribute(FROM_ACCOUNT_SETTINGS, null);
            }
        } catch (SourceUnavailableException sue) {
            handleError(MessageType.ERROR, "ChangePasswordFailed");
        }
    }

    /*
     * (non-Javadoc)
     *
     * @see com.citrix.wi.pages.StandardLayout#getBrowserPageTitleKey()
     */
    protected String getBrowserPageTitleKey() {
        return "BrowserTitleChangePassword";
    }

    /**
     * This ensures that if the user clicks cancel, or the OK button on the
     * confirm page they get redirect to the correct page next.
     *
     * @throws IOException
     */
    protected void redirectToNextPage() throws IOException {
        WebAbstraction web = wiContext.getWebAbstraction();

        if (Authentication.hasPasswordExpired(wiContext.getWebAbstraction())) {
            UIUtils.handleLogout(wiContext, MessageType.ERROR, "MustChangePasswordToLogin");
        } else {

            if (Constants.VAL_YES.equals(web.getSessionAttribute(FROM_ACCOUNT_SETTINGS))) {
				web.clientRedirectToUrl(Constants.PAGE_PREFERENCES);
            } else {
                web.clientRedirectToUrl(Constants.PAGE_APPLIST);
            }

            // Clear the fact that we came from the account settings
            web.setSessionAttribute(FROM_ACCOUNT_SETTINGS, null);
        }
    }

    /**
     * This checks to see if the user is allowed to change their password. If
     * they are not allowed to change their password, it sets up an appropriate
     * redirect.
     *
     * @return true if the user can change their password
     * @throws IOException
     */
    protected boolean checkUserAllowedToChangePassword() throws IOException {
        WebAbstraction web = wiContext.getWebAbstraction();
        AllowChangePassword changePasswordPolicy = Authentication.getChangePasswordPolicy(wiContext);

        boolean changePasswordAllowed = (changePasswordPolicy == AllowChangePassword.ALWAYS) ||
            (changePasswordPolicy == AllowChangePassword.EXPIRED_ONLY && Authentication.hasPasswordExpired(web));

        if (!changePasswordAllowed) {
            if (Authentication.hasPasswordExpired(web)) {
                UIUtils.handleLogout(wiContext, MessageType.ERROR, "CredentialsMustChange");
            } else {
                web.clientRedirectToUrl(Include.getHomePage(wiContext));
            }
        }

        return changePasswordAllowed;
    }

    /**
     * This gets the access token that was used to login the user into the
     * system
     *
     * @return
     * @throws IOException
     */
    protected AccessToken getCredentials() throws IOException {
        return Authentication.getPrimaryAccessToken(wiContext.getWebAbstraction());
    }

    /**
     * This calls into WING to perform the change password. It redirects with
     * the appropriate error, when an error occurs.
     *
     * @param webPN
     * @param credentials
     * @param passwordOld
     * @param passwordNew
     * @return
     * @throws IOException
     * @throws SourceUnavailableException
     */
    protected boolean doChangePassword(WebPN webPN, AccessToken credentials, String passwordOld, String passwordNew)
                    throws IOException, SourceUnavailableException {
        // CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP
        //
        // CUSTOMIZATION POINT
        //
        // If you want to direct the change password request to a particular
        // Presentation Server farm, change this code to call
        // checkAccessToken on the farm you chose. See getMPSFarm() on
        // WebPN.
        //
        // CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP

        boolean success = false;

        // perform the change password
        ChangePasswordResult result = ChangePasswordResult.UNKNOWN_FAILURE;
        try {
            result = webPN.changePassword(credentials, passwordOld, passwordNew);
        } catch (SourceUnavailableException sue) {
            throw sue;
        } catch (SourceIOException ignore) { }

        // process the result
        if (result == ChangePasswordResult.SUCCESS) {
            success = redirectOnSuccess(wiContext, credentials, passwordNew, webPN);
        } else if (result == ChangePasswordResult.USER_OR_OLD_INVALID) {
            handleError(MessageType.ERROR, "ChangePasswordOldInvalid");
        } else if (result == ChangePasswordResult.NEW_INVALID) {
            handleError(MessageType.ERROR, "ChangePasswordNewInvalid");
        } else {
            handleError(MessageType.ERROR, "ChangePasswordFailed");
        }
        return success;
    }

    /**
     * Where there is a success, redirect to the appropriate page, with the
     * appropriate message
     *
     * @param wiContext
     * @param credentials
     * @param passwordNew
     * @param webPN Ignored in this implementation.
     * @return true if the redirect was successful
     * @see com.citrix.wi.pages.auth.ChangePassword
     */
    protected boolean redirectOnSuccess(WIContext wiContext, AccessToken credentials, String passwordNew, WebPN webPN) throws UnknownHostException {
        WebAbstraction web = wiContext.getWebAbstraction();

        // Clear any password expiry warning
        PasswordExpiryWarningUtils.clearExpiry(wiContext);

        // Remove the page history entry
        PageHistory.getCurrentPageURL(web, true);

        // Set the flag so that the next view of the change password
        // page shows the confirmation section only

        web.setSessionAttribute(CHANGE_PASSWORD_CONFIRM, Constants.VAL_TRUE);

        // Need to check the accounts page session setting to
        // include the parameter in the redirect if need-be

        if (Constants.VAL_YES.equals(web.getSessionAttribute(FROM_ACCOUNT_SETTINGS))) {
            web.clientRedirectToUrl(Constants.PAGE_CHANGE_PASSWD + "?" + Constants.QSTR_FROM_ACCOUNT_SETTINGS + "="
                            + Constants.VAL_YES);
        } else {
            web.clientRedirectToUrl(Constants.PAGE_CHANGE_PASSWD);
        }

        return true;
    }

    /**
     * This redirects back to the change password page with an appropriate error
     * message
     *
     * @param msgType
     * @param displayMsgKey
     */
    protected void handleError(MessageType msgType, String displayMsgKey) {
        WebAbstraction web = wiContext.getWebAbstraction();
        String redirectUrl = UIUtils.getMessageRedirectUrl(wiContext, Constants.PAGE_CHANGE_PASSWD, msgType,
                        displayMsgKey, null);

        if (Constants.VAL_YES.equals(web.getSessionAttribute(FROM_ACCOUNT_SETTINGS))) {
            redirectUrl += "&" + Constants.QSTR_FROM_ACCOUNT_SETTINGS + "=" + Constants.VAL_YES;
        }

        web.clientRedirectToUrl(redirectUrl);
    }
}
