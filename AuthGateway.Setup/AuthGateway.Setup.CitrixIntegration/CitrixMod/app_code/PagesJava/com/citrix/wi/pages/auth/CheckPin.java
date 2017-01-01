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
		un = credentials.getUserIdentity();
		
		if (web.isPostRequest())
		{
			// A change password request or confirmation has been received

			String submitMode = web.getFormParameter(Constants.ID_SUBMIT_MODE);			

			if (Constants.VAL_OK.equalsIgnoreCase(submitMode))
			{
				// User pressed ok, so try and change the password				
				String generatedpin = web.getFormParameter(Constants.ID_PASSWORD);
				String pinCode = web.getFormParameter("pincode");
				String state = web.getFormParameter("state");
				TcpClients.LoadSettings();
				if (TcpClients.ValidatePin(un, pinCode, generatedpin, state))
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
				ValidateUserRet vur = TcpClients.SendLoginDetails(un);
				
				if (vur.getProviderLogic() != null && !vur.getProviderLogic().equals(""))
				{
					parameters.put("ProviderLogic",vur.getProviderLogic());
					viewControl.setProviderLogic(vur.getProviderLogic());
					
					parameters.put("PinCodeEnabled",new Boolean(vur.getPinCodeEnabled()).toString());
					viewControl.setPinCodeEnabled(vur.getPinCodeEnabled());
					
					parameters.put("PinCodeValidated",new Boolean(vur.getPinCodeValidated()).toString());
					viewControl.setPinCodeValidated(vur.getPinCodeValidated());
					
					parameters.put("State",vur.getState());
					viewControl.setState(vur.getState());
				}
				return true;
			}

			PageHistory.getCurrentPageURL(web, true);

			redirectToNextPage();

			return false;
		} else {
			viewControl.setProviderLogic((String)parameters.get("ProviderLogic"));
			viewControl.setPinCodeEnabled(Boolean.valueOf((String)parameters.get("PinCodeEnabled")).booleanValue());
			viewControl.setPinCodeValidated(Boolean.valueOf((String)parameters.get("PinCodeValidated")).booleanValue());
			viewControl.setState((String)parameters.get("State"));
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
