/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.auth;

import java.io.IOException;
import java.util.List;

import com.citrix.wi.clientdetect.ClientDetectionWizardState;
import com.citrix.wi.clientdetect.Mode;
import com.citrix.wi.clientdetect.WizardConstants;
import com.citrix.wi.clientdetect.WizardUtil;
import com.citrix.wi.controls.SilentDetectionPageControl;
import com.citrix.wi.localization.ClientManager;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pages.StandardPage;
import com.citrix.wi.pageutils.ClientUtils;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.PageHistory;
import com.citrix.wi.pageutils.clientdetection.DetectionUtils;
import com.citrix.wi.types.ClientPlatform;
import com.citrix.wi.types.JavaFallbackMode;
import com.citrix.wing.UserEnvironmentAdaptor;

/**
 * This Page deals with the running of the silent detection which must take
 * place before any of the site's pages are accessed.
 */
public class SilentDetection extends StandardPage {

    protected SilentDetectionPageControl viewControl = new SilentDetectionPageControl();

    public SilentDetection(WIContext wiContext) {
        super(wiContext);
        wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
    }

    public boolean performImp() throws IOException {
        UserEnvironmentAdaptor envAdaptor = wiContext.getUserEnvironmentAdaptor();
        // Transfer any detection data to the client session state
        ClientUtils.transferClientInformationCookie(wiContext.getWebAbstraction(), wiContext.getUserEnvironmentAdaptor());

        if (isEnvironmentDetectionRequired(wiContext)) {
            viewControl.detectEnvironment = true;
        } else if (isClientDetectionRequired(wiContext)) {
            viewControl.detectClients = true;
            setNeedsReloadToKillInfoBar(wiContext, true);
            // We want to return to the silent detection page once client detection is done
            recordCurrentPageURL();
            prepareDetection();
        } else if (getNeedsReloadToKillInfoBar(wiContext)) {
            viewControl.needsReloadToKillInfoBar = true;
            setNeedsReloadToKillInfoBar(wiContext, false);
        } else {
            // Ideally, we would simply do a 302 to the first WI page once silent detection is complete,
            // but this causes a problem with Password Manager (PM does not notice that the
            // login page has been loaded), so we do a javascript redirect instead

            // The WI page to redirect to after silent detection is complete will be on the top of the history stack,
            // so pop it off and set it as the next page.
            viewControl.nextPage = PageHistory.getCurrentPageURL(wiContext.getWebAbstraction(), true);

            if (PreLoginMessage.isPreLoginMessageConfiguredToAppear(wiContext)) {
                if (getWebAbstraction().getSessionAttribute("SV_PRE_LOGIN_MESSAGE_VISITED") == null) {
                    viewControl.nextPage = Constants.PAGE_PRE_LOGIN_MESSAGE;
                }
            }

            viewControl.silentDetectionComplete = true;
        }

        // Cookies may have been written to the client session state
        envAdaptor.commitState();
        envAdaptor.destroy();

        return true;
    }

    /**
     * Sets up the wizard state and view control for the silent detection process.
     *
     * Silent detection is performed by JavaScript in this page to avoid the overhead of running
     * the wizard with its multiple page redirects. However, the detection results are posted to the
     * outputs page to effectively simulate a wizard run. The wizard state therefore needs to be set up
     * appropriately.
     */
    private void prepareDetection() {
        ClientDetectionWizardState sWizardInfo = Include.getWizardState(wiContext);

        // Clear out any existing wizard invocation data
        sWizardInfo.clearInvocationState();
        sWizardInfo.setMode(Mode.SILENT);

        // If the user has forced their client, then we don't want to change the client the user has stored
        if (Boolean.TRUE.equals(wiContext.getUserPreferences().getForcedClient())) {
            sWizardInfo.setKeepSameClient(true);
        }

        List remoteClients = DetectionUtils.createRemoteClientList(wiContext, sWizardInfo.getMode());
        List streamingClients = DetectionUtils.createStreamingClientList(wiContext, sWizardInfo.getMode());

        // Store the list of remote and streaming clients as this is used in the Outputs page
        sWizardInfo.setRemoteClientsToDetect(remoteClients);
        sWizardInfo.setStreamingClientsToDetect(streamingClients);

        viewControl.remoteClientToDetect = getClientToDetect(remoteClients);
        viewControl.streamingClientToDetect = getClientToDetect(streamingClients);

        // Get the version of the remote and streaming client on the server, so that it can later be determined if
        // the version installed on the client can be upgraded.
        ClientManager clientManager = Include.getClientManager(wiContext.getStaticEnvironmentAdaptor());

        viewControl.serverRemoteClientVersion = WizardUtil.getClientVersionNumber(clientManager,
            wiContext.getClientInfo().getClientPlatform(), wiContext.getCurrentLocale());

        viewControl.serverStreamingClientVersion = WizardUtil.getClientVersionNumber(clientManager,
            ClientPlatform.STREAMING_WIN32, wiContext.getCurrentLocale());

        // Allow Java fallback when the fallback mode is Auto
        if (DetectionUtils.isJavaFallbackModeEnabled(wiContext, JavaFallbackMode.AUTO)) {
            viewControl.useJavaFallback = true;
        }

        // Only report the RDP class id when detecting the RDP client
        viewControl.showRdpClassId = WizardConstants.RDP.equals(viewControl.remoteClientToDetect);
    }

    /**
     * Gets the name of the first client in the supplied list of ClientType objects.
     * For silent detection, there should only ever be a single client in this list.
     */
    private String getClientToDetect(List clientList) {
        String client = "";

        if (clientList.size() > 0) {
            client = clientList.get(0).toString();
        }

        return client;
    }

    /**
     * Determines whether any kind of silent detection is required.
     */
    public static boolean isDetectionRequired(WIContext wiContext) {
        return isEnvironmentDetectionRequired(wiContext)
            || isClientDetectionRequired(wiContext)
            || getNeedsReloadToKillInfoBar(wiContext);
    }

    /**
     * Environment detection determines things like the screen res
     */
    private static boolean isEnvironmentDetectionRequired(WIContext wiContext) {
        return ClientUtils.clientDetectionRequired(wiContext.getUserEnvironmentAdaptor());
    }

    /**
     * Determines if client detection is required.
     */
    private static boolean isClientDetectionRequired(WIContext wiContext) {
        boolean runSilentWizard = false;
        if (Include.isWizardModeSupported(wiContext, Mode.SILENT)
                        && DetectionUtils.clientsAvailableForDetection(wiContext, Mode.SILENT)) {
            // the wizard is supported, see if we need to run it
            if (!Include.getWizardState(wiContext).hasWizardRun()) {
                runSilentWizard = true;
            }
        }
        return runSilentWizard;
    }

    /**
     * Indicates that an operation has taken place which may have caused the ActiveX info-bar
     * to appear.
     */
    private static void setNeedsReloadToKillInfoBar(WIContext wiContext, boolean value) {
        wiContext.getWebAbstraction().setSessionAttribute(SV_NEED_RELOAD_TO_KILL_INFOBAR,
            value ? Constants.VAL_TRUE : Constants.VAL_FALSE);
    }

    /**
     * Determines whether an operation has taken place which may have caused the ActiveX info-bar
     * to appear.
     */
    private static boolean getNeedsReloadToKillInfoBar(WIContext wiContext) {
        return Constants.VAL_TRUE.equals(wiContext.getWebAbstraction().getSessionAttribute(SV_NEED_RELOAD_TO_KILL_INFOBAR));
    }

    private static final String SV_NEED_RELOAD_TO_KILL_INFOBAR = "NeedReloadToKillInfoBar";
}
