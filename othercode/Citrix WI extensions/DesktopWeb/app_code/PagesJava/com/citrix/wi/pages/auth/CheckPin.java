/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth;

import java.io.IOException;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Map;
import java.util.Set;

import com.citrix.authentication.tokens.AccessToken;
import com.citrix.authentication.web.AuthenticationState;
import com.citrix.wi.UserPreferences;
import com.citrix.wi.accountselfservice.AccountTask;
import com.citrix.wi.clientdetect.Mode;
import com.citrix.wi.config.AuthenticationConfiguration;
import com.citrix.wi.config.WIConfiguration;
import com.citrix.wi.config.LocalisedTextConfiguration;
import com.citrix.wi.config.auth.AGAuthPoint;
import com.citrix.wi.config.auth.AuthPoint;
import com.citrix.wi.config.auth.ExplicitAuth;
import com.citrix.wi.config.auth.ExplicitNDSAuth;
import com.citrix.wi.config.auth.ExplicitUDPAuth;
import com.citrix.wi.config.auth.TwoFactorAuthMethod;
import com.citrix.wi.config.auth.WIAuthPoint;
import com.citrix.wi.controls.LoginPageControl;
import com.citrix.wi.controlutils.FeedbackMessage;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.mvc.StatusMessage;
import com.citrix.wi.pageutils.AGEUtilities;
import com.citrix.wi.pageutils.AccessTokenResult;
import com.citrix.wi.pageutils.AccountSelfService;
import com.citrix.wi.pageutils.Authentication;
import com.citrix.wi.pageutils.ClientUtils;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.LaunchUtilities;
import com.citrix.wi.pageutils.LocalisedText;
import com.citrix.wi.pageutils.TwoFactorAuth;
import com.citrix.wi.pageutils.TabUtils;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wi.pageutils.DisasterRecoveryUtils;
import com.citrix.wi.pageutils.PageHistory;
import com.citrix.wi.pageutils.PasswordExpiryWarningUtils;
import com.citrix.wi.pageutils.SessionUtils;
import com.citrix.wi.pageutils.Utils;



import com.citrix.wing.AccessTokenValidity;
import com.citrix.wing.MessageType;
import com.citrix.wing.SourceIOException;
import com.citrix.wing.SourceUnavailableException;
import com.citrix.wing.types.AccessTokenValidationResult;
import com.citrix.wing.webpn.UserContext;
import com.citrix.wing.webpn.WebPN;

import com.citrix.wi.types.AllowChangePassword;
import com.citrix.wi.types.AGAuthenticationMethod;
import com.citrix.wi.types.CredentialFormat;
import com.citrix.wi.types.UserInterfaceBranding;
import com.citrix.wi.types.WIAuthType;

import com.citrix.wi.ui.PageAction;
import com.citrix.wing.MessageType;
import com.citrix.wing.UserEnvironmentAdaptor;
import com.citrix.wing.util.Strings;
import com.citrix.wi.pages.*;
import com.citrix.wi.controls.*;

import com.citrix.wing.webpn.UserContext;
import com.citrix.wing.webpn.WebPN;
import com.citrix.wi.controlutils.*;


import com.citrix.wi.pageutils.PageHistory;
import custom.auth.*;


/**
 * Base class for the business logic of the login page.
 */
public class CheckPin extends StandardLayout
{

	protected CheckPinPageControl viewControl = new CheckPinPageControl();

	private String un = "";

	/**
     * Constructor.
     *
     * @param wiContext the Web Interface context object
     */
    public CheckPin(WIContext wiContext) {
        super(wiContext);
        wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
    }


	protected String getBrowserPageTitleKey()
	{
		return "BrowserTitleCheckPin";
	}

    protected boolean performGuard() throws IOException {
        // Login page not protected against CSRF.
        return true;
    }

    public final boolean performImp() throws IOException {
		WebAbstraction web = wiContext.getWebAbstraction();
		Map parameters = (Map)Authentication.getAuthenticationState(wiContext.getWebAbstraction()).getParameters();
		AccessToken credentials = (AccessToken)parameters.get(Authentication.VAL_ACCESS_TOKEN);
		un = credentials.getShortUserName();
		
		if (web.isPostRequest())
		{
			// A change password request or confirmation has been received

			String submitMode = web.getFormParameter(Constants.ID_SUBMIT_MODE);			

			if (Constants.VAL_OK.equalsIgnoreCase(submitMode))
			{
				// User pressed ok, so try and change the password				
				String pin = web.getFormParameter(Constants.ID_PASSWORD);
				TcpClients.LoadSettings();
				if (TcpClients.ValidatePin(un, pin))
				{
					LaunchUtilities.transferLaunchDataToSession(wiContext);
					// Authentication successful
					Authentication.getAuthenticationState(wiContext.getWebAbstraction()).setAuthenticated(credentials);
					
					// Create a UserContext to hold user-specific app state; this
					// is null if the creation failed, which indicates some horrible
					// problem has arisen somewhere.
					UserContext userContext = SessionUtils.createNewUserContext(wiContext);
					if (userContext == null)
					{
						return false;
					}
					wiContext.getWebAbstraction().clientRedirectToUrl(
								   Authentication.getAuthenticationState(wiContext.getWebAbstraction()).getCurrentPage());

					return true;
				}
				else
				{
					/*
					FeedbackStore store = (FeedbackStore)web.getSessionAttribute("sessionFeedbackStore");
					if (store == null)
					{
						store = new FeedbackStore();
						web.setSessionAttribute("sessionFeedbackStore", store);
					}
					FeedbackMessage pinErrMsg = new FeedbackMessage(MessageType.ERROR, "pinError");
					String msgId = store.put(pinErrMsg);
					wiContext.getConfiguration().getLocalisedTextConfiguration().setLocalizedLoginSysMessage(wiContext.getCurrentLocale(), "abcabcabc");
					
					 */
					UIUtils.HandleLoginFailedMessage(wiContext, MessageType.ERROR, "InvalidCredentials");
					return false;
					/*
					wiContext.getConfiguration().getLocalisedTextConfiguration().setLocalizedWelcomeMessage(wiContext.getCurrentLocale(), "Wrong PIN");
					UserEnvironmentAdaptor envAdaptor = wiContext.getUserEnvironmentAdaptor();
					if (!(envAdaptor.isCommitted()))
					{
						envAdaptor.commitState();
						envAdaptor.destroy();
					}
					wiContext.getWebAbstraction().clientRedirectToUrl(PageHistory.getCurrentPageURL(web, true));
					
					return false;*/
				}
			}else if(Constants.VAL_CANCEL.equalsIgnoreCase(submitMode))
			{
				TcpClients.SendLoginDetails(un);
				return true;
			}

			PageHistory.getCurrentPageURL(web, true);

			redirectToNextPage();

			return false;
		}
		
		return true;
		
    }
	protected static final String FROM_ACCOUNT_SETTINGS = "fromAccountSettings";

	/**
     * When you cancel, always return to the login page telling the
     * user to change their password.
     */
	protected void redirectToNextPage() throws IOException
	{
		UIUtils.HandleLoginFailedMessage(wiContext, MessageType.ERROR, "MustChangePasswordToLogin");
	}

	protected void handleError(MessageType msgType, String displayMsgKey)
	{
		WebAbstraction web = wiContext.getWebAbstraction();
		String redirectUrl = UIUtils.getMessageRedirectUrl(wiContext, Constants.PAGE_CHANGE_PASSWD, msgType,
						displayMsgKey, null);

		if (Constants.VAL_YES.equals(web.getSessionAttribute(FROM_ACCOUNT_SETTINGS)))
		{
			redirectUrl += "&" + Constants.QSTR_FROM_ACCOUNT_SETTINGS + "=" + Constants.VAL_YES;
		}

		web.clientRedirectToUrl(redirectUrl);
	}
}
