package com.citrix.wi.pages.site_auth.clientdetection;

import java.io.IOException;
import java.net.UnknownHostException;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;

import com.citrix.wi.UserPreferences;
import com.citrix.wi.clientdetect.Client;
import com.citrix.wi.clientdetect.ClientDetectionWizardState;
import com.citrix.wi.clientdetect.Mode;
import com.citrix.wi.clientdetect.WizardConstants;
import com.citrix.wi.clientdetect.type.ClientType;
import com.citrix.wi.controls.VariablesForPostPageControl;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pages.StandardPage;
import com.citrix.wi.pages.site.DirectLaunch;
import com.citrix.wi.pageutils.AGEUtilities;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.PageHistory;
import com.citrix.wi.pageutils.SessionUtils;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wi.pageutils.WIBrowserTitleBuilder;
import com.citrix.wi.pageutils.clientdetection.DetectionUtils;
import com.citrix.wi.pageutils.clientdetection.InputUtilities;
import com.citrix.wi.types.JavaFallbackMode;
import com.citrix.wi.types.MPSClientType;
import com.citrix.wi.types.ProductType;
import com.citrix.wi.types.UserInterfaceBranding;
import com.citrix.wing.MessageType;
import com.citrix.wing.UserEnvironmentAdaptor;
import com.citrix.wing.webpn.UserContext;

/**
 * The pre-inputs page is used as a convenient stepping-stone for calling
 * the wizard.
 *
 * It takes a small number of query string parameters which can easily
 * be specified eg. in a hyperlink, and causes appropriate input parameters
 * to be POSTed to the wizard input page.
 */
public class PreInputs extends StandardPage {

    protected VariablesForPostPageControl viewControl = new VariablesForPostPageControl();
    private WebAbstraction web;

    public PreInputs(WIContext wiContext) {
        super(wiContext);
        wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
        web = getWebAbstraction();
    }

     protected boolean performImp() throws IOException {

        // This page can either call the wizard, or force a client
        // If there is the following parameter, the Java client is forced.
        // This should only happen when the manual Java fallback is enabled.
        // Forcing the Java client will stop the user being prompted to install
        // the native client every time they log into the site
        boolean forceJavaClient = Constants.VAL_TRUE.equals(web.getRequestParameter(Constants.QSTR_FORCE_ICA_JAVA));
        if (forceJavaClient && DetectionUtils.isJavaFallbackModeEnabled(wiContext, JavaFallbackMode.MANUAL)) {
            forceUseOfClient(MPSClientType.JAVA);
            return false;
        }

        boolean forceNativeClient = Constants.VAL_TRUE.equals(web.getRequestParameter(Constants.QSTR_FORCE_ICA_LOCAL));
        if (forceNativeClient) {
            forceUseOfClient(MPSClientType.LOCAL_ICA);
            return false;
        }

        ClientDetectionWizardState sWizardInfo = Include.getWizardState(wiContext);
        UserEnvironmentAdaptor userEnv = wiContext.getUserEnvironmentAdaptor();

        // Clear out any existing wizard invocation data
        sWizardInfo.clearInvocationState();

        // Read in and validate the query string parameters

        // A download caption for the streaming client would specify this query string - it
        // constrains the wizard to streaming clients only.
        boolean overrideStreaming = Constants.VAL_ACCESS_METHOD_STREAMING.equals(web.getRequestParameter(Constants.QSTR_CLIENT_TYPE));

        // A download caption for the remote clients would specify this query string - it
        // constrains the wizard to remote clients only.
        boolean overrideRemote = Constants.VAL_ACCESS_METHOD_REMOTE.equals(web.getRequestParameter(Constants.QSTR_CLIENT_TYPE));

        // A download caption to upgrade the current client would specify this query string.
        boolean detectCurrentClient = Constants.VAL_TRUE.equals(web.getRequestParameter(Constants.QSTR_DETECT_CURRENT));

        // Whether to show the IE zone help page
        boolean showZone = Constants.VAL_TRUE.equals(web.getRequestParameter(Constants.QSTR_SHOW_ZONE));

        // Whether to show the upgrade page (applies to native and streaming clients)
        boolean showUpgrade = Constants.VAL_TRUE.equals(web.getRequestParameter(Constants.QSTR_SHOW_UPGRADE));

        // Whether the wizard is being called from the toolbar (Boxworth) or navbar (Caxton)
        boolean toolbar = Constants.VAL_TRUE.equals(web.getRequestParameter(Constants.QSTR_WIZARD_TOOLBAR));

        // Whether the wizard is being called from the client settings page
        boolean calledFromSettingsPage = Constants.VAL_TRUE.equals(web.getRequestParameter(Constants.QSTR_SETTINGS));

        // The wizard mode requested
        Mode requestedMode = Mode.getModeFromString(web.getQueryStringParameter(WizardConstants.PARAMETER_MODE));

        boolean allowCustomizeClients = Include.getAllowUserToCustomizeClient(wiContext);

        // Determine whether to call the wizard in advanced mode.
        // Advanced mode is for remote clients only, and should be allowed only when user customization of the remote
        // client is allowed.
        boolean advanced = allowCustomizeClients
            && (calledFromSettingsPage || toolbar);

        // Is this a SILENT mode wizard run?
        boolean runSilently = requestedMode == Mode.SILENT;

        // Whether to show the logoff link
        boolean showLogoff = Include.isLoggedIn(web);

        // If run manually from the settings screen or the toolbar, allow the upgrade page and change zone page to be shown
        // since this may solve some problem the user is experiencing
        if (calledFromSettingsPage || toolbar) {
            showUpgrade = true;
            showZone = true;
        }

        // Set the mode and other inputs
        if (runSilently) {
            sWizardInfo.setMode(Mode.SILENT);
        } else if (advanced) {
            sWizardInfo.setMode(Mode.ADVANCED);
            // Always allow advanced mode to show upgrade and zone change screens
            showZone = true;
            showUpgrade = true;
            // Advanced mode needs to know the current client, including whether it was forced or auto
            Client preferredClient = InputUtilities.getPreferredClient(wiContext);
            if (preferredClient != null) {
                viewControl.setPreferredClient(preferredClient.getClientAsString());
            }
        } else {
            sWizardInfo.setMode(Mode.AUTO);
            viewControl.setShowWelcomeScreen(false);
            if (toolbar || calledFromSettingsPage) {
                sWizardInfo.setInvokedFromToolbar(true);
            } else if (!showZone) { // If the zone change page should be shown, that takes precedence over the upgrade page.
                // Show the upgrade page if we are meant to prompt for an upgrade at login.
                showUpgrade = showUpgrade
                        || DetectionUtils.promptForStreamingUpgrade(wiContext)
                        || DetectionUtils.promptForLocalIcaUpgrade(wiContext);
            }
        }

        if (showZone) {
            viewControl.setShowZonePage(WizardConstants.ON);
        }

        if (showUpgrade) {
            viewControl.setShowUpgradePage(WizardConstants.ON);
        }

        List remoteClients = new ArrayList(); // a list of remote <code>ClientType<code> for the wizard to detect
        List streamingClients = new ArrayList(); // a list of streaming <code>ClientType<code> for the wizard to detect

        if (detectCurrentClient) {
            // We only want the wizard to consider the current client (for upgrading)
            remoteClients.add(ClientType.getClientTypeFromMPSClientType(Include.getSelectedRemoteClient(wiContext)));
        } else if (overrideStreaming){
            // Ask the wizard to consider all streaming clients only
            streamingClients = DetectionUtils.createStreamingClientList(wiContext, null);
        } else if (overrideRemote) {
            // Ask the wizard to consider remote client(s) only
            remoteClients = DetectionUtils.createRemoteClientList(wiContext, null);
        } else {
            // Detect remote and streaming clients
            streamingClients = DetectionUtils.createStreamingClientList(wiContext, sWizardInfo.getMode());
            remoteClients = DetectionUtils.createRemoteClientList(wiContext, sWizardInfo.getMode());
        }

        // If we are only detecting the current client
        // or we are in silent mode and the user has forced their client,
        // then we don't want to change the client the user has stored
        // nor flip between advanced / auto mode
        if( detectCurrentClient
                || (Boolean.TRUE.equals(wiContext.getUserPreferences().getForcedClient())
                    && (sWizardInfo.getMode() == Mode.SILENT)) ) {
            sWizardInfo.setKeepSameClient(true);
        }

        sWizardInfo.setRemoteClientsToDetect(remoteClients); // Wizard input stored for future reference
        sWizardInfo.setStreamingClientsToDetect(streamingClients); // Wizard input stored for future reference

        DetectionUtils.setRadeDetectionStartCookie(wiContext, sWizardInfo);

        viewControl.setRemoteClients(sWizardInfo.getRemoteClientsToDetectAsString());
        viewControl.setStreamingClient(sWizardInfo.getStreamingClientsToDetectAsString());
        viewControl.setWizardMode(sWizardInfo.getMode().toString());

        if (web.getRequestPathRelativeToContext().startsWith("/auth")) {
            viewControl.setRedirectURL("../auth/" + Constants.PAGE_WIZARD_OUTPUT);
        } else {
            String redirectUrl = "../site/" + Constants.PAGE_WIZARD_OUTPUT;
            // Direct launch query string (if any) needs to be passed through.
            redirectUrl = DirectLaunch.propagateDirectLaunchInfo(web, redirectUrl);
            viewControl.setRedirectURL(redirectUrl);
        }

        viewControl.setLogout(showLogoff ? WizardConstants.ON : WizardConstants.OFF);

        // Set the branding
        ProductType productType
            = Include.getSiteBranding(wiContext) == UserInterfaceBranding.DESKTOPS
                ? ProductType.XEN_DESKTOP
                : ProductType.XEN_APP;
        viewControl.setProductType(productType.toString());

        // The wizard should do fallback when Java fallback is turned on
        // except for not silently selecting Java when in manual fallback
        if (DetectionUtils.isJavaFallbackModeEnabled(wiContext, JavaFallbackMode.AUTO)) {
            viewControl.setJavaFallback(true);
        } else if (DetectionUtils.isJavaFallbackModeEnabled(wiContext, JavaFallbackMode.MANUAL)) {
            if (Mode.AUTO.equals(sWizardInfo.getMode()) || Mode.ADVANCED.equals(sWizardInfo.getMode())) {
                viewControl.setJavaFallback(true);
            }
        }

        // Store whether to use the client detection wizard in Netscalar / Access Gateway compatibility mode.
        viewControl.setAccessGatewayCompatibility(AGEUtilities.isAGEIntegrationEnabled(wiContext));

        if (!sWizardInfo.isInHiddenFrame()) {
            // Push this page onto the page history, so that when the wizard is done we can get
            // back to where we started with a call to PageHistory.redirectToLastPage()
            PageHistory.recordCurrentPageURL(web);
        }

        userEnv.commitState();
        userEnv.destroy();

        return true; // render the VariablesForPostPageControl which will invoke the wizard
    }

    /**
     * This updates the user preferences, session state and detection cookie to force use of the
     * specified remote client.
     *
     * @param clientType the type of client to force.
     * @throws UnknownHostException
     */
    private void forceUseOfClient(MPSClientType clientType) throws UnknownHostException {
        UserContext userContext = null;
        if (Include.isLoggedIn(wiContext.getWebAbstraction())) {
            userContext = SessionUtils.checkOutUserContext(wiContext);
        }

        // add the Java client as forced
        UserPreferences writableUserPrefs = Include.getRawUserPrefs(wiContext.getUserEnvironmentAdaptor());
        writableUserPrefs.setForcedClient(Boolean.TRUE);
        writableUserPrefs.setClientType(clientType);

        // save the change
        if (userContext == null) {
            Include.saveUserPrefsPreLogin(writableUserPrefs, wiContext);
        } else {
            Include.saveUserPrefs(writableUserPrefs, wiContext, userContext);
        }

        Map clientSessionState = wiContext.getUserEnvironmentAdaptor().getClientSessionState();
        clientSessionState.put(Constants.COOKIE_REMOTE_CLIENT_DETECTED, clientType.toString() + "=Forced");

        // Commit the change
        if (userContext == null) {
            // Not logged in
            wiContext.getUserEnvironmentAdaptor().commitState();
            wiContext.getUserEnvironmentAdaptor().destroy();
        } else {
            // Logged in
            SessionUtils.returnUserContext(userContext);
        }

        ClientDetectionWizardState sWizardInfo = Include.getWizardState(wiContext);
        sWizardInfo.setForcedRemoteClientResult(clientType);
        sWizardInfo.setKeepSameClient(true);

        String redirectQueryString = UIUtils.getMessageQueryStr(MessageType.SUCCESS, "SettingsSaved");
        PageHistory.redirectToHomePage(wiContext, redirectQueryString);
    }

}
