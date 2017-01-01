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
import com.citrix.wi.types.AGAuthenticationMethod;
import com.citrix.wi.types.CredentialFormat;
import com.citrix.wi.types.UserInterfaceBranding;
import com.citrix.wi.types.WIAuthType;
import com.citrix.wi.ui.PageAction;
import com.citrix.wing.MessageType;
import com.citrix.wing.UserEnvironmentAdaptor;
import com.citrix.wing.util.Strings;

import custom.auth.*;

/**
 * Base class for the business logic of the login page.
 */
public abstract class Login extends PreLoginUIPage {

    protected LoginPageControl viewControl = new LoginPageControl();

    /**
     * Constructor.
     *
     * @param wiContext the Web Interface context object
     */
    public Login(WIContext wiContext) {
        super(wiContext);
        wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
        layoutControl.formAction = Constants.FORM_POSTBACK;
        layoutControl.layoutMode = Include.getLayoutMode(wiContext);
    }

    protected String getBrowserPageTitleKey() {
        return "BrowserTitleLogin";
    }

    protected boolean performGuard() throws IOException {
        // Login page not protected against CSRF.
        return true;
    }

    public final boolean performImp() throws IOException {
        boolean result;
        // If there is a post, process the login info to allow third party integrations to partially work.
        if (getWebAbstraction().isPostRequest()) {
            // Process the login information.
            result = performInternal();
        } else {
            // Default to the normal behavior on a GET request.
            result = super.performImp();
        }
        return result;
    }

    protected boolean performInternal() throws IOException {
        WebAbstraction web = wiContext.getWebAbstraction();

		
        setupDirectLaunch();

        // Only run the wizard in auto mode from the login page once, to avoid repeatedly 
        // redirecting back to the wizard when, for example, the user skips it.
        if (web.isGetRequest() && Include.isWizardModeSupported(wiContext, Mode.AUTO) && !isFeedbackSet() &&
            (web.getSessionAttribute("SV_WIZARD_AUTO_MODE_RUN") == null)) {

            web.setSessionAttribute("SV_WIZARD_AUTO_MODE_RUN", Boolean.TRUE);

            String inputUrl = Include.getWizardInputUrl(wiContext);
            if (inputUrl != null) {
                getWebAbstraction().clientRedirectToUrl(inputUrl);
                return false;
            }
        }

        UserEnvironmentAdaptor envAdaptor = wiContext.getUserEnvironmentAdaptor();
        AuthenticationState authenticationState = Authentication.getAuthenticationState(web);

        ClientUtils.transferClientInformationCookie(web, envAdaptor);

        if (!processNonWIAuthPoints(web, envAdaptor, authenticationState)) {
            return false;
        }

        if (web.isGetRequest()) {
            if (!processGet()) {
                return false;
            }
        }

        // If we've marked the browser session to indicate login has been started,
        // but we've ended up back at the login page the session has been terminated
        // for some reason. This comes after checking for URL login requests because
        // Certificate authentication may need to come through that route.
        // If, however, we have been provided with a message to display, then
        // we have to re-show the login page to display the message.
        WIAuthType logonMode = Authentication.getUntrustedLogonType(envAdaptor);
        if ((logonMode != null) && !isFeedbackSet()
                        && (web.getQueryStringParameter(Constants.QSTR_END_SELF_SERVICE) != null)) {
            UIUtils.handleLogout(wiContext, MessageType.INFORMATION, "SessionExpired");
            return false;
        }

        if (web.isPostRequest() && !bIsError()) {
            if (!processPost()) {
                return false;
            }
        }

        // Note business logic may have modified viewControl already.
        doViewControlSetup();

        envAdaptor.commitState();
        envAdaptor.destroy();

        return true;
    }

    /**
     * Processes the login when the authentication point is not Web Interface.
     *
     * @return <code>true</code> if the logon page should be displayed; <code>false</code> otherwise
     */
    protected boolean processNonWIAuthPoints(WebAbstraction web, UserEnvironmentAdaptor envAdaptor,
                    AuthenticationState authenticationState) {
        boolean continueProcessing = true;

        AuthPoint authPoint = wiContext.getConfiguration().getAuthenticationConfiguration().getAuthPoint();
        boolean nonWI = !(authPoint instanceof WIAuthPoint);
        // Only check for actual errors here, rather than the presence of any feedback message. Otherwise,
        // skipping the wizard (which generates a warning feedback message) would prevent the processing.
        FeedbackMessage feedback = feedbackControl.getFeedback();
        boolean isError = (feedback != null && feedback.getType() == MessageType.ERROR);

        // Only try to do the login if there is no error message.
        if (nonWI && !isError) {

            addAuthPages(authenticationState, authPoint);

            // Finished processing which pages to visit.
            authenticationState.pageCompleted();

            // Clear any logon type.
            Authentication.storeLogonType(null, envAdaptor);
            envAdaptor.commitState();
            envAdaptor.destroy();

            // When going to things like the federated page,
            // don't do a server side redirect as then browser sends NTLM
            // headers when it sees the 401.
            web.clientRedirectToUrl(authenticationState.getCurrentPage());
            continueProcessing = false;
        }

        return continueProcessing;
    }

    /**
     * Adds the authentication pages to the state as required by the given authentication point.
     *
     * @param authenticationState the state to update
     * @param authPoint the authentication point to use
     */
    protected void addAuthPages(AuthenticationState authenticationState, AuthPoint authPoint) {
        if (authPoint instanceof AGAuthPoint) {
            // Note, pages added in reverse order all AG auth types go to callback then authenticate.
            Map parameters = new HashMap();
            AGAuthenticationMethod authMethod = ((AGAuthPoint)authPoint).getAuthenticationMethod();

            if (authMethod.equals(AGAuthenticationMethod.SMART_CARD_KERBEROS) ||
                            authMethod.equals(AGAuthenticationMethod.SMART_CARD)) {
                // AG integration for Smart Card Authentication.
                authenticationState.addPageToQueueHead(AGEUtilities.AUTH_PAGE_CERTIFICATE, parameters);
            } else {
                // AG integration for Explicit Authentication.
                authenticationState.addPageToQueueHead(AGEUtilities.AUTH_PAGE_AUTHENTICATE, parameters);
            }

            authenticationState.addPageToQueueHead(AGEUtilities.AUTH_PAGE_CALLBACK, parameters);
            authenticationState.addPageToQueueHead(AGEUtilities.AUTH_PAGE_SSO, parameters);
        }
    }

    /**
     * Process a login request obtained from an HTTP GET.
     *
     * This method assumes that automatic login is permitted.
     *
     * @return <code>true</code> if the logon page should be displayed; <code>false</code> otherwise
     * @throws IOException
     */
    protected abstract boolean processDirectLogin() throws IOException;

    /**
     * Sets up direct launch if a request has been made.
     */
    protected void setupDirectLaunch() {
        WIConfiguration wiConfig = wiContext.getConfiguration();

        // Check for bookmarked URL to store.
        String appId = getAppId();
        if (appId != null) {
            LaunchUtilities.setRequestDirectLaunch(wiContext, true);
            if (wiConfig.getEnablePassthroughURLs()) {
                LaunchUtilities.setClientSessionLaunchApp(wiContext, appId);
            }
        }
    }

    /**
     * Determines whether automatic login is allowed.
     *
     * Automatic login is allowed unless:
	 * - The user has just arrived from the logged out page
	 * - An authentication error has just occurred (or auto login has temporarily been disallowed for some other reason)
	 * - The user has logged out with a smartcard but has not closed their browser
	 * - There is a direct launch but the configuration does not allow it
	 * - The user has chosen not to automatically log in, or the configuration disallows it
     *
     * @return <code>true</code> if automatic login is allowed, otherwise
     * <code>false</code>
     */
    protected boolean isAutoLoginAllowed() {
        WIConfiguration wiConfig = wiContext.getConfiguration();
        AuthenticationConfiguration authConfig = wiConfig.getAuthenticationConfiguration();
        UserEnvironmentAdaptor envAdaptor = wiContext.getUserEnvironmentAdaptor();
        WebAbstraction web = wiContext.getWebAbstraction();

        // No auto login if we have just come from the logged out page.
        boolean fromLoggedOutPage = (web.getQueryStringParameter(Constants.QSTR_FROM_LOGGEDOUT_PAGE) != null);

        // No auto login if it has been disallowed (e.g. after a certificate error (smartcard)).
        String allowAutoLoginCookie = (String)envAdaptor.getClientSessionState().get(Constants.COOKIE_ALLOW_AUTO_LOGIN);
        boolean autoLoginDisallowed = Strings.equalsIgnoreCase(Constants.VAL_OFF, allowAutoLoginCookie);

        // No auto login if the user has just logged off after using a smartcard, without closing the browser.
        String smcLoggedOutCookie = (String)envAdaptor.getClientSessionState().get(Constants.COOKIE_SMC_LOGGED_OUT);
        boolean isSMCLoggedOut = Strings.equalsIgnoreCase(Constants.VAL_ON, smcLoggedOutCookie);

        // No auto login if there is a direct launch but this is forbidden.
        // This ensures that the logon page is displayed and the user is
        // informed that the launch cannot go ahead.
        boolean directAppLaunchForbidden = (getAppId() != null) && !wiConfig.getEnablePassthroughURLs();

        // Determine what the user prefs or auth config dictate
        boolean autoLoginChoice = false;
        boolean anonOnly = false;
        if (authConfig.getAuthPoint() instanceof WIAuthPoint) {
            WIAuthPoint wiAuthPoint = (WIAuthPoint)authConfig.getAuthPoint();
            anonOnly = wiAuthPoint.isAuthMethodEnabled(WIAuthType.ANONYMOUS) && (getAllowedAuthMethods().size() == 1);
        }

        if (anonOnly) {
            // When anonymous authentication is the only supported auth method,
            // we always allow auto login regardless of config or user preference.
            autoLoginChoice = true;
        } else {
            // In all other cases, we have to check the user prefs/config
            autoLoginChoice = !Boolean.FALSE.equals(wiContext.getUserPreferences().getUseSilentAuth());
        }

        return (!fromLoggedOutPage && !autoLoginDisallowed && !isSMCLoggedOut && !directAppLaunchForbidden && autoLoginChoice);
    }

    /**
     * Processes an incoming GET request.
     *
     * Determines whether page processing should continue based on query string
     * values that were provided with the request.
     *
     * If auto login is possible, this is also attempted.
     *
     * @return <code>true</code> if page processing should continue,
     * <code>false</code> otherwise
     * @throws IOException
     */
    protected boolean processGet() throws IOException {
        boolean result = true;

        WebAbstraction web = wiContext.getWebAbstraction();

        // This may or may not return
        if (web.getQueryStringParameter(Constants.QSTR_LOGINTYPE) != null) {
            // Login data is given in the URL. Attempt to use it. We will fall
            // through if the data is not a valid login attempt. For example,
            // the login type is provided if we are using certificate
            // authentication and need to switch from HTTP to HTTPS.
            result = handleLoginRequest(WIAuthType.fromString(web.getQueryStringParameter(Constants.QSTR_LOGINTYPE)));
        } else if (web.getQueryStringParameter(Constants.QSTR_START_SELF_SERVICE) != null) {
            SetupAccountSelfService();
            result = false;
        } else if (isFeedbackSet()) {
            // This is a request to display an error message after a failed
            // login, do not attempt silent authentication

            // check for invalid fields
            Authentication.extractInvalidFieldData(viewControl, web);

            result = true;
        } else if (isAutoLoginAllowed() && !bIsError()) {
            result = processDirectLogin();
        }

        return result;
    }

    /**
     * Setup Account Self Service redirection on user request Account Self
     * Service.
     */
    protected void SetupAccountSelfService() {
        // This is a request to start the account self service process
        // Add the self service entry page to the page queue and redirect
        if (AccountSelfService.isAccountUnlockEnabled(wiContext.getConfiguration())
                        && AccountSelfService.isPasswordResetEnabled(wiContext)) {
            // Option of reset or unlock.
            Authentication.addPageToQueueHead(wiContext, "account_ss_entry", null);
        } else if (AccountSelfService.isAccountUnlockEnabled(wiContext.getConfiguration())) {
            // Skip the entry page as only Unlock Account is avaiable.
            AccountSelfService.BuildAuthenticationFilterQueue(wiContext, AccountTask.ACCOUNT_UNLOCK);
        } else if (AccountSelfService.isPasswordResetEnabled(wiContext)) {
            // Skip the entry page as only Reset Password is avaiable.
            AccountSelfService.BuildAuthenticationFilterQueue(wiContext, AccountTask.PASSWORD_RESET);
        } else {
            // Unexpected situation so revert to sending to the self service
            // entry page.
            Authentication.addPageToQueueHead(wiContext, "account_ss_entry", null);
        }

        Authentication.redirectToNextAuthPage(wiContext);
    }

    /**
     * Takes a URL of the form
     * http://server/WI/site/launcher.aspx?CTX_Application=aoeu and returns just
     * the application name, or null if it is not present.
     */
    protected String getAppIdFromUrl(String url) {
        final String prefix = "/site/" + Constants.PAGE_LAUNCHER + "?";

        if (url == null || !url.startsWith(prefix) || prefix.length() >= url.length()) {
            return null;
        }
        String queryString = url.substring(prefix.length());

        return LaunchUtilities.getAppIdFromInitialQueryString(wiContext, queryString);
    }

    protected static String getSafeFormParameter(WebAbstraction web, String name) {
        return Strings.ensureNonNull(web.getFormParameter(name));
    }

    protected Map createExplicitAuthenticationParameters(AccessToken credentials, ExplicitAuth expAuth) {
        Map parameters = new HashMap();
        parameters.put(Authentication.VAL_ACCESS_TOKEN, credentials);
        parameters.put(Authentication.VAL_EXPLICIT_AUTH, expAuth);

        return parameters;
    }

    protected String getAppId() {
        AuthenticationState authenticationState = Authentication.getAuthenticationState(wiContext.getWebAbstraction());

        // Check for bookmarked URL to store
        String initialURL = authenticationState.getInitialURL();
        String appId = null;

        if (initialURL != null) {
            appId = getAppIdFromUrl(initialURL);
        } else {
            appId = LaunchUtilities.getClientSessionLaunchApp(wiContext);
        }
        return appId;
    }

    /**
     * Clears any initial URL from the authentication state.
     */
    protected void clearInitialUrl() {
        Authentication.getAuthenticationState(wiContext.getWebAbstraction()).setInitialURL(null);
    }

    /**
     * This methods is used to work out which login method should be selected in
     * the drop down on the login form.
     *
     * @param lm the last login method
     * @return what should be selected
     */
    protected WIAuthType getDefaultLoginType(WIAuthType lm) {

        AuthenticationConfiguration authConfig = wiContext.getConfiguration().getAuthenticationConfiguration();

        // If the last logon mode is recorded and it is still permitted, then
        // use it. Otherwise decide on default logon type.
        // First clear associated variables
        WIAuthType logonMode = null;

        if (authConfig.getAuthPoint() instanceof WIAuthPoint) {
            WIAuthPoint wiAuthPoint = (WIAuthPoint)authConfig.getAuthPoint();
            if ((lm != null) && wiAuthPoint.isAuthMethodEnabled(lm)) {
                return lm;
            } else {
                // Use default - this is effectively the precedence order
                if (wiAuthPoint.isAuthMethodEnabled(WIAuthType.EXPLICIT)) {
                    logonMode = WIAuthType.EXPLICIT;
                } else if (wiAuthPoint.isAuthMethodEnabled(WIAuthType.CERTIFICATE)) {
                    logonMode = WIAuthType.CERTIFICATE;
                } else if (wiAuthPoint.isAuthMethodEnabled(WIAuthType.SINGLE_SIGN_ON)) {
                    logonMode = WIAuthType.SINGLE_SIGN_ON;
                } else if (wiAuthPoint.isAuthMethodEnabled(WIAuthType.CERTIFICATE_SINGLE_SIGN_ON)) {
                    logonMode = WIAuthType.CERTIFICATE_SINGLE_SIGN_ON;
                } else if (wiAuthPoint.isAuthMethodEnabled(WIAuthType.ANONYMOUS)) {
                    logonMode = WIAuthType.ANONYMOUS;
                }
            }
        }
        return logonMode;
    }

    /**
     * Process a login request obtained from an HTTP POST.
     *
     * @return <code>true</code> if the logon page should be displayed;
     * <code>false</code> otherwise
     * @throws IOException
     */
    protected boolean processPost() throws IOException {
        WebAbstraction web = wiContext.getWebAbstraction();

        WIAuthType loginType = WIAuthType.fromString(web.getFormParameter(Constants.ID_LOGIN_TYPE));
        AuthenticationConfiguration authConfig = wiContext.getConfiguration().getAuthenticationConfiguration();
        if (authConfig.getAuthPoint() instanceof WIAuthPoint) {
            WIAuthPoint wiAuthPoint = (WIAuthPoint)authConfig.getAuthPoint();
            // validate
            if (loginType == null || !wiAuthPoint.isAuthMethodEnabled(loginType)) {
                // Not valid; just re-render the login page
                return true;
            }
        }   

        // CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP
        //
        // CUSTOMIZATION POINT
        //
        // Feature: remember the user's previously selected domain from the
        // drop-down menu
        //
        // By default this feature is disabled
        // To enable this feature:
        // . Un-comment the line: '// setLoginDomainPreference();'
        // CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP CP
        // setLoginDomainPreference();

        // Handle the actual login request
        return handleLoginRequest(loginType);
    }

	protected boolean sendPin(WebAbstraction web)
	{
		//boolean result = false;
		TcpClients.LoadSettings();
		String username = web.getFormParameter(Constants.ID_USER);
		TcpClients.SendLoginDetails(username);
		return true;
	}
	/**
     * Handles a login request of the given type.
     *
     * @param loginType the authentication method to use to login
     * @return <code>true</code> if the logon page should be displayed;
     * <code>false</code> otherwise
     * @throws IOException
     */
    protected boolean handleLoginRequest(WIAuthType loginType) throws IOException {
        PageAction pageAction = null; // default to rendering the login page

        WebAbstraction web = wiContext.getWebAbstraction();
        UserEnvironmentAdaptor envAdaptor = wiContext.getUserEnvironmentAdaptor();
        AuthenticationConfiguration authConfig = wiContext.getConfiguration().getAuthenticationConfiguration();
					
        if (authConfig.getAuthPoint() instanceof WIAuthPoint) {
            WIAuthPoint wiAuthPoint = (WIAuthPoint)authConfig.getAuthPoint();
            if (wiAuthPoint.isAuthMethodEnabled(loginType)) {

                // Set the login option for the user based on how they are
                // logging in. So that when they logout & return to login they
                // are prompted with the last chosen authentication method.
                setLogonModePreference(loginType);

                pageAction = getPageAction(loginType);

                // Move the browser on to the next authentication page, if
                // appropriate.
                if (pageAction != null && pageAction != PageAction.SKIP_PAGE_RENDER) {
                    // Not just rendering the logon page, so record the user's
                    // choice of logon method.
                    Authentication.storeLogonType(loginType, envAdaptor);

                    // Script execution is always stopped by one of the
                    // following methods
                    if (pageAction.getUseRedirect()) {
                        envAdaptor.commitState();
                        envAdaptor.destroy();
                        web.clientRedirectToUrl(pageAction.getURL());
                    } else {
                        String forwardUrl = Authentication.getAuthenticationPageContextPath(wiContext, pageAction
                                        .getURL());                        
						web.serverForwardToContextUrl(forwardUrl);
                    }
                }
            }
        }

        // If pageAction is non-null then we have done some kind of redirect
        // Need to let perform() know to return false
    return (pageAction == null);
    }

    /**
     * Gets a page action that describes the next page in the authentication
     * process, if any.
     *
     * @param loginType the authentication method to use to login
     * @return a PageAction object, or null
     * @throws IOException
     */
    protected PageAction getPageAction(WIAuthType loginType) throws IOException {
        PageAction pageAction = null;

        AuthenticationConfiguration authConfig = wiContext.getConfiguration().getAuthenticationConfiguration();
        if (authConfig.getAuthPoint() instanceof WIAuthPoint) {
            WIAuthPoint wiAuthPoint = (WIAuthPoint)authConfig.getAuthPoint();

            if (WIAuthType.EXPLICIT.equals(loginType)) {
                pageAction = authenticateExplicit((ExplicitAuth)wiAuthPoint.getAuthMethod(WIAuthType.EXPLICIT));
            } else if (WIAuthType.ANONYMOUS.equals(loginType)) {
                pageAction = authenticateGuest();
            }
        }
        return pageAction;
    }

    /**
     * Log the user in with explicit authentication.
     *
     * @param expAuth the explicit authentication configuration
     * @return a PageAction object, or null if there was an error
     * @throws IOException
     */
    protected PageAction authenticateExplicit(ExplicitAuth expAuth) throws IOException {
        WebAbstraction web = wiContext.getWebAbstraction();
        PageAction result = null; // default to rendering the login page again

        if (!wiContext.getWebAbstraction().isPostRequest()) {
            // Explicit logins can only be performed via an HTTP POST
            return result;
        }

        // Pull out the fields we have interest in
        String user = getSafeFormParameter(web, Constants.ID_USER).trim();
        String password = getSafeFormParameter(web, Constants.ID_PASSWORD);
        String domain = getSafeFormParameter(web, Constants.ID_DOMAIN).trim();
        String context = getSafeFormParameter(web, Constants.ID_CONTEXT).trim();
        String passcode = getSafeFormParameter(web, Constants.ID_PASSCODE).trim();

        // Make sure none of the credential fields contain control characters
        if (Strings.hasControlChars(user) || Strings.hasControlChars(password) || Strings.hasControlChars(domain)
                        || Strings.hasControlChars(context) || Strings.hasControlChars(passcode)
                        || password.length() > Constants.PASSWORD_ENTRY_MAX_LENGTH) {
            UIUtils.HandleLoginFailedMessage(wiContext, MessageType.ERROR, "InvalidCredentials");
            return PageAction.SKIP_PAGE_RENDER;
        } else if (expAuth instanceof ExplicitNDSAuth) {
            // Defer to special NDS processing code that handles context lookup
            result = authenticateNDS((ExplicitNDSAuth)expAuth, user, password, passcode, context);
        } else {
            // Not NDS - parse the fields and go do any Two-factor
            ExplicitUDPAuth udpAuth = (ExplicitUDPAuth)expAuth;

            // Attempt to create an access token from the credentials
            AccessTokenResult accessTokenResult = Authentication.createAccessToken(user, domain, password, udpAuth);
            AccessToken credentials = accessTokenResult.getAccessToken();

            if (!accessTokenResult.isError()) {
                Map parameters = createExplicitAuthenticationParameters(credentials, expAuth);

                Authentication.getAuthenticationState(wiContext.getWebAbstraction()).addPageToQueueHead("explicit",
                                parameters);

                // If two-factor authentication is in use, insert it in the
                // sequence
                TwoFactorAuthMethod twoFactorMethod = TwoFactorAuth
                                .getTwoFactorAuthMethod(wiContext.getConfiguration());
                if (twoFactorMethod != null) {
                    parameters.put(TwoFactorAuth.VAL_PASSCODE, passcode);
                    Authentication.getAuthenticationState(wiContext.getWebAbstraction()).addPageToQueueHead(
                                    twoFactorMethod.getName().toLowerCase(), parameters);
                }

                Authentication.getAuthenticationState(wiContext.getWebAbstraction()).pageCompleted();

                // Move on to the next page - no need to redirect
                result = new PageAction(Authentication.getAuthenticationState(wiContext.getWebAbstraction())
                                .getCurrentPage(), false);
            } else {
                // we had an error building the AccessToken
                // show the appropriate messages to the users
                Authentication.processAccessTokenResultError(web, accessTokenResult);
            }
        }

        return result;
    }

    /**
     * Log the user in with explicit NDS authentication.
     *
     * @param ndsAuth the explicit NDS configuration
     * @param username the username
     * @param password the password
     * @param passcode the passcode for two-factor authentication
     * @param context the NDS context
     * @return a PageAction object, or null if there was an error
     */
    protected abstract PageAction authenticateNDS(ExplicitNDSAuth ndsAuth, String username, String password,
                    String passcode, String context);

    /**
     * Log the user in with anonymous authentication.
     *
     * @return a PageAction object, or null if there was an error
     */
    protected PageAction authenticateGuest() {
        Authentication.getAuthenticationState(wiContext.getWebAbstraction()).addPageToQueueHead("anonymous", null);
        Authentication.getAuthenticationState(wiContext.getWebAbstraction()).pageCompleted();
        return new PageAction(Authentication.getAuthenticationState(wiContext.getWebAbstraction()).getCurrentPage(),
                        false);
    }

    /**
     * Determines whether an error has been recorded.
     *
     * @return <code>true</code> if an error has been recorded, otherwise
     * <code>false</code>
     */
    protected boolean bIsError() {
        return isFeedbackSet();
    }

    /**
     * Gets the set of authentication methods that the user is allowed to use.
     *
     * This default implementation returns only the authentication methods that
     * are allowed on both the IIS and Java web server platforms.
     *
     * @return set of authentication method names (Set containing String
     * objects)
     */
    protected Set getAllowedAuthMethods() {
        Set authMethods = new HashSet(10);

        AuthenticationConfiguration authConfig = wiContext.getConfiguration().getAuthenticationConfiguration();
        if (authConfig.getAuthPoint() instanceof WIAuthPoint) {
            WIAuthPoint wiAuthPoint = (WIAuthPoint)authConfig.getAuthPoint();
            // Go through each available auth method and check whether the user
            // is
            // allowed to use it.
            for (int i = 0; i < ALL_AVAILABLE_METHODS.length; i++) {
                if (wiAuthPoint.isAuthMethodEnabled(ALL_AVAILABLE_METHODS[i])) {
                    authMethods.add(ALL_AVAILABLE_METHODS[i]);
                }
            }
        }
        return authMethods;
    }

    // The names of all the available authentication methods that apply to both
    // platforms.
    private static final WIAuthType[] ALL_AVAILABLE_METHODS = new WIAuthType[] { WIAuthType.EXPLICIT,
                    WIAuthType.ANONYMOUS                   };

    /**
     * Set the user preference for default domain when multiple domains are
     * available.
     */
    protected void setLoginDomainPreference() {

        WebAbstraction web = wiContext.getWebAbstraction();
        String loginDomainPreference = web.getFormParameter(Constants.ID_DOMAIN);

        if (loginDomainPreference != null) {
            UserPreferences newUserPrefs = Include.getRawUserPrefs(wiContext.getUserEnvironmentAdaptor());
            newUserPrefs.setLoginDomainPreference(loginDomainPreference);
            Include.saveUserPrefsPreLogin(newUserPrefs, wiContext);
        }
    }

    /**
     * Sets the user preference for Logon Mode (Smartcard, explicit, single
     * signon). This sets the default logon mode when more than one option is
     * available to the one the user last used (as long as the browser window is
     * not closed).
     */
    protected void setLogonModePreference(WIAuthType logonMode) {
        if (logonMode != null) {
            UserPreferences newUserPrefs = Include.getRawUserPrefs(wiContext.getUserEnvironmentAdaptor());
            newUserPrefs.setAuthMethod(logonMode);
            Include.saveUserPrefsPreLogin(newUserPrefs, wiContext);
        }
    }

    /**
     * Set up the view controls, etc used to control the page rendering.
     */
    protected void doViewControlSetup() {
        WIConfiguration wiConfig = wiContext.getConfiguration();

        recordCurrentPageURL();
        layoutControl.isLoginPage = true;

        super.setupNavControl();

        // System message control        
		String customText = LocalisedText.getLoginSysMessage(wiContext);
		
        if (customText != null) {
            sysMessageControl.setMessage(customText);
        }

        // Prepare welcome control
        welcomeControl.setTitle(wiContext.getString("LoginWelcomeTitle"));

        if (Include.isCompactLayout(wiContext)) {
            welcomeControl.setTitle(wiContext.getString("ScreenTitleLogin"));
        } else { // Check for custom welcome title and message, which would
            // override what we have already set
            String customTitle = LocalisedText.getLoginTitle(wiContext);
            if (customTitle != null) {
                welcomeControl.setTitle(customTitle);
            }
        }

        String customMsg = LocalisedText.getLoginWelcomeMessage(wiContext);
        if (customMsg != null) {
            welcomeControl.setBody(customMsg);
        }

        // Display the passcode box to the user if 2-factor authentication is
        // being used
        viewControl.setShowPasscode(TwoFactorAuth.getTwoFactorAuthMethod(wiConfig) != null);
		
        // Display the account self service link if configured
        // Client side cookie will determine if icon shown when connection is
        // https.
        viewControl.setShowAccountSelfService(AccountSelfService.isAccountSelfServiceConfigEnabled(wiContext));
        viewControl.setAccountSelfServiceLinkTextKey(AccountSelfService.getAccountSelfServiceLinkKey(wiContext));

        // Display Direct Launch Messages
        if (LaunchUtilities.getDirectLaunchModeInUse(wiContext) && !bIsError()) {
            if (!wiConfig.getEnablePassthroughURLs()) {
                setFeedback(MessageType.WARNING, "ShortcutDisabled");
                LaunchUtilities.setRequestDirectLaunch(wiContext, false);
            } else {
                setFeedback(MessageType.INFORMATION, "HaveAwaitingApplication");
            }
        }

        // Determine the allowed login modes
        viewControl.allowedLogonModes().addAll(getAllowedAuthMethods());

        WIAuthType logonMode = getDefaultLoginType(wiContext.getUserPreferences().getAuthMethod());
        viewControl.setSelectedLogonMode(logonMode);

        // Disable the UPD entries if explicit authentication is not being used.
        boolean isWIAuthPoint = wiContext.getConfiguration().getAuthenticationConfiguration().getAuthPoint() instanceof WIAuthPoint;
        boolean explicitInUse = isWIAuthPoint && (logonMode.equals(WIAuthType.EXPLICIT));

        if (!explicitInUse) {
            viewControl.setExplicitDisabled(true);
            viewControl.setDomainDisabled(true);
        }
    }

    /**
     * Set up the display of the domain field.
     *
     * @param updAuth the explicit authentication configuration
     */
    protected void setDomainDisplay(ExplicitUDPAuth udpAuth) {
        if (udpAuth != null) {
            int numDomains = udpAuth.getDomainSelection().size();
            int numRestrictedDomains = udpAuth.getDomains().size();

            // Hide the domain field only if this has been configured and:
            // there is at most one entry in the list of domains to display, or
            // login is restricted to the entries in the restricted domains list
            // (either NT style credentials or no domain allowed)
            // OR if the credential format has been set to allow only UPN style
            // credentials
            // OR if domains are restricted and the domain list is empty (i.e.
            // only UPNs allowed)
            boolean hideDomain = ((udpAuth.getDomainFieldHidden() && ((numDomains <= 1) || udpAuth
                            .getDomainsRestricted()))
                            || (udpAuth.getCredentialFormat() == CredentialFormat.UPN) || (udpAuth
                            .getDomainsRestricted() && (numRestrictedDomains == 0)));
            viewControl.setShowDomain(!hideDomain);

            if (numDomains != 0) {
                viewControl.setLoginDomainSelection(udpAuth.getDomainSelectionArray());
                viewControl.setLoginDomainPreference(wiContext.getUserPreferences().getLoginDomainPreference());
            }
            if (numRestrictedDomains > 0) {
                viewControl.setLoginDomains(udpAuth.getDomainsArray());
            }
            viewControl.setRestrictDomains(udpAuth.getDomainsRestricted());
        }
    }
}
