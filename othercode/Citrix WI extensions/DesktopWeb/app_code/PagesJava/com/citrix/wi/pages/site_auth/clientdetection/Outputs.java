package com.citrix.wi.pages.site_auth.clientdetection;

import java.io.IOException;
import java.util.List;
import java.util.Map;

import com.citrix.wi.UserPreferences;
import com.citrix.wi.clientdetect.Client;
import com.citrix.wi.clientdetect.ClientDetectionWizardState;
import com.citrix.wi.clientdetect.Mode;
import com.citrix.wi.clientdetect.WizardConstants;
import com.citrix.wi.clientdetect.WizardUtil;
import com.citrix.wi.clientdetect.type.ClientType;
import com.citrix.wi.clientdetect.type.IcoStatus;
import com.citrix.wi.config.ClientDeploymentConfiguration;
import com.citrix.wi.controlutils.FeedbackMessage;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pages.StandardPage;
import com.citrix.wi.pages.site.DirectLaunch;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.PageHistory;
import com.citrix.wi.pageutils.SessionToken;
import com.citrix.wi.pageutils.SessionUtils;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wi.pageutils.clientdetection.DetectionUtils;
import com.citrix.wi.types.JavaFallbackMode;
import com.citrix.wi.types.MPSClientType;
import com.citrix.wing.MessageType;
import com.citrix.wing.util.Strings;
import com.citrix.wing.webpn.UserContext;

/**
 * This wizard POSTs its output to this page.
 */
public class Outputs extends StandardPage {

    private String remoteClient = "";
    private String streamingClient = "";
    private String alternateResult = "";
    private String rdpClassID = "";
    private String icoStatus = "";

    ClientDetectionWizardState sWizardInfo;
    WebAbstraction web;

    public Outputs(WIContext wiContext) {
        super(wiContext);
        web = getWebAbstraction();
        sWizardInfo = Include.getWizardState(wiContext);
    }

    /**
     * Returning true causes a javascript redirect attempt with the assumption
     * this page is running inside an iframe.
     */
    protected boolean performImp() throws IOException {
        // Read in the POST data
        if (web.isPostRequest()) {
            remoteClient = web.getRequestParameter(WizardConstants.PARAMETER_REMOTE_CLIENT);
            streamingClient = web.getRequestParameter(WizardConstants.PARAMETER_STREAMING_CLIENT);
            rdpClassID = web.getRequestParameter(WizardConstants.PARAMETER_RDP_CLIENT_CLASS_ID);
            icoStatus = web.getRequestParameter(WizardConstants.PARAMETER_ICO_STATUS);

            // Validate the inputs
            remoteClient = Client.getClientFromString(remoteClient) == null ? "" : remoteClient;
            streamingClient = Client.getClientFromString(streamingClient) == null ? "" : streamingClient;
            rdpClassID = WizardUtil.isValidRdpClassId(rdpClassID) ? rdpClassID : "";
            icoStatus = IcoStatus.getIcoStatusFromString(icoStatus) == null ? "" : icoStatus;
        }

        // Alternate result could be query string or POST data e.g. it's a query
        // string when the session times out
        alternateResult = web.getRequestParameter(WizardConstants.ALTERNATE_RESULT);
        alternateResult = isValidAlternateResult(alternateResult) ? alternateResult : "";

        // Session has expired, so show the message on the logged out page
        if (WizardConstants.SESSION_EXPIRED.equals(alternateResult)) {
            web.clientRedirectToUrl(UIUtils.getLoggedOutRedirectURL(wiContext, MessageType.INFORMATION,
                            "SessionExpired", null));
            return false;
        }

        // The user pressed the logout link, so execute it
        if (WizardConstants.LOGOUT.equals(alternateResult)) {
            web.clientRedirectToUrl(Constants.PAGE_LOGOUT + "?" + SessionToken.QSTR_TOKENNAME + "="
                            + SessionToken.get(wiContext));
            return false;
        }

        // There are different calls to update the user preferences if
        // logged in as we have to make sure that the user context and access
        // prefs are updated appropriately.
        UserContext userContext = null;
        if (Include.isLoggedIn(wiContext.getWebAbstraction())) {
            userContext = SessionUtils.checkOutUserContext(wiContext);
        }

        final boolean skipped = WizardConstants.SKIPPED.equals(alternateResult);
        // only precess the results if it was not skipped
        if (!skipped) {
            // process the wizard results
            processWizardResults(wiContext, userContext, sWizardInfo, remoteClient, streamingClient,
                            alternateResult, rdpClassID, icoStatus);
        } else if (!DetectionUtils.isValidClientDetectionResult(wiContext)) {
            Outputs.processWizardResults(wiContext, userContext, sWizardInfo, "", "", "", "", "");
            sWizardInfo.clearInvocationState();
        }

        // Work out where to go
        FeedbackMessage feedback = null;
        if (alternateResult != null && alternateResult.startsWith(WizardConstants.ERROR)) {
            // There has been an error, so tell the user about it
            feedback = getFeedbackForError(wiContext, alternateResult);
        } else {
            // add the appropriate message for the user
            feedback = generateFeedback(wiContext, skipped);
        }
        String redirectQueryString = "";
        if (feedback != null) {
            redirectQueryString = UIUtils.getMessageQueryStr(
                feedback.getType(), feedback.getKey(), feedback.getLogEventId());
        }

        // Direct launch query string (if any) needs to be passed through.
        redirectQueryString = DirectLaunch.propagateDirectLaunchInfo(web, redirectQueryString);

        // Clear the rade cookie if needed
        DetectionUtils.clearRadeDetectionStartCookie(wiContext, sWizardInfo);

        // Commit the state
        if (userContext == null) {
            // Not logged in
            wiContext.getUserEnvironmentAdaptor().commitState();
            wiContext.getUserEnvironmentAdaptor().destroy();
        } else {
            // Logged in
            SessionUtils.returnUserContext(userContext);
        }

        if (sWizardInfo.getMode() == Mode.SILENT) {
            // The caller will have pushed the page to return to onto the page history stack;
            // pop it off and redirect to that page
            String nextPage = PageHistory.getCurrentPageURL(wiContext.getWebAbstraction(), true);
            web.clientRedirectToUrl(nextPage);
            return false;
        } else {
            // Redirect to the appropriate place
            if (skipped) {
                PageHistory.redirectToLastPage(wiContext, redirectQueryString);
            } else {
                PageHistory.redirectToHomePage(wiContext, redirectQueryString);
            }
            return false;
        }
    }

    /**
     * For CSRF protection
     */
    protected boolean performGuard() throws IOException {
        return SessionToken.guard(wiContext) && super.performGuard();
    }

    /**
     * Process the wizard results. Uses a <code>ClientDetectionWizardState</code>
     * to hold results. This method also validates the results; if any item is
     * invalid it is replaced with an empty string.
     */
    public static void processWizardResults(WIContext wiContext, UserContext userContext,
                    ClientDetectionWizardState sWizardInfo, String remoteClient, String streamingClient,
                    String alternateResult, String rdpClassID, String icoStatusResult) {

        // don't process the client result if there
        // was an error
        if (alternateResult == null || alternateResult.equals("")) {
            List streamingClients = sWizardInfo.getStreamingClientsToDetect();
            List remoteClients = sWizardInfo.getRemoteClientsToDetect();
            boolean streamingClientDetection = streamingClients == null ? false : !streamingClients.isEmpty();
            boolean remoteClientDetection = remoteClients == null ? false : !remoteClients.isEmpty();

            // If we weren't trying to detect a streaming client, don't overwriting
            // the streaming client result form a previous invocation the wizard.
            if (!streamingClientDetection) {
                streamingClient = sWizardInfo.getStreamingClientResult() == null ? "" : sWizardInfo
                                .getStreamingClientResult().toString();
            }
            // If we weren't trying to detect a remote client, don't overwriting the
            // remote client result form a previous invocation the wizard.
            if (!remoteClientDetection) {
                remoteClient = sWizardInfo.getRemoteClientResult() == null ? "" : sWizardInfo.getRemoteClientResult()
                                .toString();
            }

            // Respect the user's remote client choice, if required
            if (sWizardInfo.isKeepSameClient()) {

                // get the one client that got passed into the wizard
                ClientType userPrefClientType = getSingleClientPassedToWizard(sWizardInfo);
                // see if the client is forced
                boolean clientIsForced = Boolean.TRUE.equals(
                                wiContext.getUserPreferences().getForcedClient());

                // only continue if we get the client
                if (userPrefClientType != null) {
                    // parse the current output
                    Client wizardResult = Client.getClientFromString(remoteClient);
                    if (wizardResult != null && wizardResult.getClientType().equals(userPrefClientType)) {
                        // wizard returned a result for the client selected,
                        // so respect the users forcing,
                        // but use the latest information about it
                        wizardResult.setAutoDetected(!clientIsForced);
                        remoteClient = wizardResult.toString();
                    } else {
                        // client not found,
                        // but we should keep using it anyway
                        Client userPref = new Client(userPrefClientType);
                        userPref.setAutoDetected(!clientIsForced);
                        remoteClient = userPref.toString();
                    }
                }
            }
        }

        // Save the wizard result in the session
        sWizardInfo.setDetectionResult(remoteClient, streamingClient, alternateResult, rdpClassID, icoStatusResult);

        // Save the result in client session state so that we don't need to run
        // the wizard if user logs off and log back in
        // Or if the session times out, or they get their password wrong
        saveClientDetectionResult(wiContext, remoteClient, streamingClient, alternateResult, rdpClassID, icoStatusResult);

        if (alternateResult == null || alternateResult.equals("")) {
            // if it is not just an error, save the user preferences
            updateUserPrefs(wiContext, sWizardInfo, userContext);
        }
    }

    /**
     * Try and get the one client that was passed into
     * the wizard for detection.
     *
     * @param sWizardInfo the information about the wizard
     * @return the ClientType of the remote client passed into the wizard
     */
    private static ClientType getSingleClientPassedToWizard(ClientDetectionWizardState sWizardInfo) {
        // Find the specific client the was passed into the wizard
        ClientType clientType = null;
        List listOfClients = sWizardInfo.getRemoteClientsToDetect();
        // the list should contain one ClientType object
        if (listOfClients.size() == 1) {
            Object clientTypeObj = listOfClients.get(0);
            if (clientTypeObj != null && clientTypeObj instanceof ClientType) {
                clientType = (ClientType)clientTypeObj;
            }
        }
        return clientType;
    }

    /**
     * Saves the wizard's output data as session cookies, so the silent wizard
     * does *not* have to re-run if the user logs off and on. By saving the
     * result in cookies, we also cater for the following situations:
     * - the user enters an invalid password and so his session is destroyed
     * - the user goes to the login page, but the session times out before he logs in
     */
    private static void saveClientDetectionResult(WIContext wiContext, String remoteClient, String streamingClient,
                    String alternateResult, String rdpClassID, String icoStatus) {
        Map clientSessionState = wiContext.getUserEnvironmentAdaptor().getClientSessionState();
        clientSessionState.put(Constants.COOKIE_REMOTE_CLIENT_DETECTED, remoteClient);
        clientSessionState.put(Constants.COOKIE_STREAMING_CLIENT_DETECTED, streamingClient);
        clientSessionState.put(Constants.COOKIE_ALTERNATE_RESULT, alternateResult);
        clientSessionState.put(Constants.COOKIE_ICO_STATUS, icoStatus);
        if (WizardUtil.isValidRdpClassId(rdpClassID)) {
            clientSessionState.put(Constants.COOKIE_RDP_CLASS_ID, rdpClassID);
        } else {
            clientSessionState.remove(Constants.COOKIE_RDP_CLASS_ID);
        }
    }

    /**
     * Update the user's client preferences according to the wizard results.
     */
    private static void updateUserPrefs(WIContext wiContext, ClientDetectionWizardState sWizardInfo,
                    UserContext userContext) {

        UserPreferences writeableUserPrefs = Include.getRawUserPrefs(wiContext.getUserEnvironmentAdaptor());

        if (sWizardInfo.hasRemoteClientResult()) {
            // we have remote client result, so save their new preference
            setClientType(wiContext, writeableUserPrefs, sWizardInfo.getRemoteClientResult());
        } else {
            // No client detection result - this means no working client was detected
            // and the user hasn't forced a client, so just use the default client.
            ClientType preferredType = DetectionUtils.getPreferredRemoteClientType(wiContext);
            if (preferredType != null) {
                Client defaultClient = new Client(preferredType);
                defaultClient.setAutoDetected(true);
                setClientType(wiContext, writeableUserPrefs, defaultClient);
            }
        }

        // Save the above changes, as required
        if (userContext != null) {
            Include.saveUserPrefs(writeableUserPrefs, wiContext, userContext);
        } else {
            Include.saveUserPrefsPreLogin(writeableUserPrefs, wiContext);
        }
    }

    /**
     * Utility method to set the user's client preferences. It checks that the
     * client is allowed by the admin configuration.
     */
    private static void setClientType(WIContext wiContext, UserPreferences writeableUserPrefs, Client client) {
        if (client != null) {
            ClientDeploymentConfiguration cdConfig = wiContext.getConfiguration().getClientDeploymentConfiguration();
            MPSClientType mpsClientType = client.getClientType().getMPSClientType();
            if ((mpsClientType != null)
                            && (cdConfig.isEnabledClient(mpsClientType)
                                            || MPSClientType.JAVA.equals(mpsClientType)
                                            && (DetectionUtils.isJavaFallbackModeEnabled(wiContext, JavaFallbackMode.MANUAL) || DetectionUtils.isJavaFallbackModeEnabled(wiContext, JavaFallbackMode.AUTO)))) {
                writeableUserPrefs.setClientType(mpsClientType);
                writeableUserPrefs.setForcedClient(new Boolean(!client.isAutoDetected()));
            }
        }
    }

    /**
     * Extracts the error key represented by wizard alternateResult or null if
     * there is no error.
     */
    private static String getErrorFromString(String alternateResult) {
        String errMsgKey = null;
        if (alternateResult != null) {
            if (alternateResult.indexOf(WizardConstants.ERROR) != -1
                            && alternateResult.length() > WizardConstants.ERROR.length()) {
                // log error id which is after "Error:"
                errMsgKey = alternateResult.substring(WizardConstants.ERROR.length() + 1);
            }
        }
        return errMsgKey;
    }

    /**
     * Gets a feedback message appropriate to a wizard error.
     *
     * @param wiContext the Web Interface context
     * @param alternateResult possible alternate result from client detection wizard
     * @return a <code>FeedbackMessage</code> object, or null
     */
    private static FeedbackMessage getFeedbackForError(WIContext wiContext, String alternateResult) {
        FeedbackMessage feedback = null;
        String errMsgKey = getErrorFromString(alternateResult);
        if (!Strings.isEmpty(errMsgKey)) {
            // log error id which is after "Error:"

            String logID = wiContext.log(MessageType.ERROR, errMsgKey);

            // If the error was that no clients were enabled
            // need to be specific about this to the user

            if (WizardConstants.NO_CLIENT_TO_DETECT.equals(errMsgKey)) {
                feedback = new FeedbackMessage(MessageType.ERROR, "WizardNoClientError", null, logID, null);
            } else {
                feedback = new FeedbackMessage(MessageType.ERROR, "WizardError", null, logID, null);
            }
        }
        return feedback;
    }

    /**
     * Gets if rade client is the most preferred client. @return <code>true</code>
     * if the rade client is the most preferred client
     */
    private static boolean isRadeClientPreferred(Client streamingClient) {
        boolean radePreferred = false;
        if (streamingClient != null) {
            if (streamingClient.equals(new Client(ClientType.RADE))) {
                radePreferred = true;
            }
        }
        return radePreferred;
    }

    /**
     * Gets if the detected remote client is preferred client.
     * @param preferredClientType the preferred client
     * @param remoteClient holds newly determined remote client
     * @return true if remote client is preferred client
     */
    private static boolean isRemoteClientPreferred(ClientType preferredClientType, Client remoteClient) {
        if (preferredClientType == null || remoteClient == null) {
            return false;
        }
        return (preferredClientType == remoteClient.getClientType()) && remoteClient.isAutoDetected()
                        && !remoteClient.isUpgradeable();

    }

    /**
     * Creates a feedback message to indicate the wizard result.
     *
     * This will be displayed in the feedback area. It is needed for the case
     * where:
     *
     * (1) the user invokes the auto-mode wizard from the toolbar, but the wizard
     * has no work to do because the preferred client is already in use
     *
     * (2) the wizard is invoked in auto-mode and:
     * - a usable client was detected
     * - no usable client was detected
     * - a client was forced
     *
     * (3) the wizard is invoked in advanced mode and the user clicked OK to save
     * the client selection
     *
     * @param wiContext The WI context
     * @param skipped True if the wizard was "skipped"
     * @return Query string to appended or ""
     */
    private static FeedbackMessage generateFeedback(WIContext wiContext, boolean skipped) {
        FeedbackMessage feedback = null;
        ClientDetectionWizardState sWizardInfo = Include.getWizardState(wiContext);

        Client remoteClient = sWizardInfo.getRemoteClientResult();
        Client streamingClient = sWizardInfo.getStreamingClientResult();
        Client oldRemoteClient = sWizardInfo.getOldRemoteClientResult();
        Client oldStreamingClient = sWizardInfo.getOldStreamingClientResult();

        if (sWizardInfo.getInvokedFromToolbar() && !skipped) {
            // Check for the case where the user invokes the auto-mode wizard
            // from the toolbar, but the wizard has no work to do
            // because the preferred client is already in use.
            // We check be looking if the old and new result is the same, and
            // the preferred client is available
            boolean showPreferredClientMessage = false;

            if (DetectionUtils.getStreamingAccessPresent(wiContext)
                            && !DetectionUtils.getRemoteAccessPresent(wiContext)) {
                // only want to show rade if it's a rade only site
                showPreferredClientMessage = streamingClient.equals(oldStreamingClient)
                        && isRadeClientPreferred(streamingClient);
            } else {
                ClientType preferredRemoteClient = DetectionUtils.getPreferredRemoteClientType(wiContext);
                showPreferredClientMessage = remoteClient.equals(oldRemoteClient)
                        && isRemoteClientPreferred(preferredRemoteClient, remoteClient);
            }

            if (showPreferredClientMessage) {
                feedback = new FeedbackMessage(MessageType.INFORMATION, "PreferredClientDetected");
            }
        }

        if (feedback == null) {
            if ((sWizardInfo.getMode() == Mode.ADVANCED) && !skipped) {
                // Inform the user their client selection has been saved
                feedback = new FeedbackMessage(MessageType.SUCCESS, "SettingsSaved");
            } else if (sWizardInfo.getMode() == Mode.AUTO) {
                if (remoteClient == null && streamingClient == null) {
                    // Warn the user that no usable client has been detected
                    feedback = new FeedbackMessage(MessageType.WARNING, "NoUsableClientDetected");
                } else {
                    boolean forced = Boolean.TRUE.equals(wiContext.getUserPreferences().getForcedClient());
                    if (!skipped) {
                        // Inform the user they have been successful in enabling a client
                        if (forced) {
                            feedback = new FeedbackMessage(MessageType.SUCCESS, "UsableClientForced");
                        } else if (!sWizardInfo.getRemoteCanUpgrade()) { // Don't display a feedback message if the user already has a usable client
                            feedback = new FeedbackMessage(MessageType.SUCCESS, "UsableClientDetected");
                        }
                    }
                }
            }
        }

        // check we need a message for the user
        // to confirm if they always want to use the Java client
        if (!skipped && sWizardInfo.getMode() == Mode.AUTO && DetectionUtils.isJavaFallbackModeEnabled(wiContext, JavaFallbackMode.MANUAL)
                        && sWizardInfo.getRemoteClientResult() != null
                        && ClientType.JAVA.equals(sWizardInfo.getRemoteClientResult().getClientType())) {
            // the user has chosen the Java Client, but this is only temporary
            // give them the option of remembering this
            feedback = new FeedbackMessage(MessageType.INFORMATION, "AlwaysUseJavaClient");
        }

        return feedback;
    }

    /**
     * Validates the alternate result
     *
     * @param String alternateResult to validate
     * @returns boolean true if valid or null otherwise false
     */
    private static boolean isValidAlternateResult(String alternateResult) {
        if (alternateResult == null) {
            return true;
        } else {
            return (alternateResult.equalsIgnoreCase(WizardConstants.LOGOUT)
                            || alternateResult.equalsIgnoreCase(WizardConstants.SKIPPED)
                            || alternateResult.equalsIgnoreCase(WizardConstants.SESSION_EXPIRED)
                            || alternateResult.equalsIgnoreCase("Error:" + WizardConstants.INVALID_REDIRECT_URL)
                            || alternateResult.equalsIgnoreCase("Error:" + WizardConstants.UNSUPPORTED_OS) || alternateResult
                            .equalsIgnoreCase("Error:" + WizardConstants.NO_CLIENT_TO_DETECT));
        }
    }
}
