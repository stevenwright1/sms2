/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils;

import java.net.UnknownHostException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import com.citrix.wi.clientdetect.ClientDetectionWizardState;
import com.citrix.wi.config.ClientDeploymentConfiguration;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.clientdetection.DetectionUtils;
import com.citrix.wi.types.DisplayInstallCaptionData;
import com.citrix.wi.types.InstallCaptionState;
import com.citrix.wi.types.JavaFallbackMode;
import com.citrix.wi.types.MPSClientType;
import com.citrix.wi.types.UserInterfaceBranding;
import com.citrix.wing.AccessTokenException;
import com.citrix.wing.MessageType;
import com.citrix.wing.ResourceUnavailableException;
import com.citrix.wing.types.AppAccessMethod;
import com.citrix.wing.util.WebUtilities;
import com.citrix.wing.webpn.AccessMethod;
import com.citrix.wing.webpn.ResourceInfo;
import com.citrix.wing.webpn.ResourceInfoSet;
import com.citrix.wing.webpn.UserContext;

/**
 * Class used to create any messages relating to client detection and install.
 */
public class Install {

    /**
     * Generate an array of messages.
     *
     * @param wiContext - the context
     * @return the messages.
     */
    public static MessageScreenMessage[] getMessages(WIContext wiContext) {

        ArrayList messages = new ArrayList(3);

        InstallCaptionControl installCaptionControl = installClient(wiContext);

        if (installCaptionControl.getMessageType() != InstallCaptionControl.MESSAGE_NONE) {

            String subject;
            if (installCaptionControl.getOverrideMessage() == null) {
                // The messageKey must contain no mark-up
                String messageKey = installCaptionControl.getMessageKey();
                if (installCaptionControl.getMessageArgs() == null) {
                    subject = wiContext.getString(messageKey);
                } else {
                    subject = wiContext.getString(messageKey, installCaptionControl.getMessageArgs());
                }
            } else {
                subject = installCaptionControl.getOverrideMessage();
            }

            String body = "<p>" + subject + "</p>";

            InstallCaption[] mainClients = installCaptionControl.getMainClients();

            for (int i = 0; i < mainClients.length; i++) {
                InstallCaption caption = mainClients[i];
                body += "<p><a id=\"mainClientLink_" + i + "\" href=\"" + caption.getLink() + "\">"
                                + caption.getCaption() + "</a></p>";
            }

            InstallCaption[] additionalClients = installCaptionControl.getAdditionalClients();

            for (int i = 0; i < additionalClients.length; i++) {
                InstallCaption caption = additionalClients[i];
                body += "<p><a id=\"extraClientLink_" + i + "\" href=\"" + caption.getLink() + "\">"
                                + caption.getCaption() + "</a></p>";
            }

            MessageType priority = installCaptionControl.getMessageType() == InstallCaptionControl.MESSAGE_WARN ? MessageType.WARNING
                            : MessageType.INFORMATION;

            messages.add(new MessageScreenMessage(priority, subject, body));
        }

        if (Include.getWizardState(wiContext).getIncorrectZone()) {
            String messageTitle = wiContext.getString("ChangeZoneMessageTitle");
            String messageBody = wiContext.getString("ChangeZoneMessageBody", "href=\"" + getChangeZoneLink() + "\"");

            messages.add(new MessageScreenMessage(MessageType.INFORMATION, messageTitle, messageBody));
        }

        return (MessageScreenMessage[])messages.toArray(new MessageScreenMessage[0]);
    }

    /**
     * Helper method to return a link to the wizard pre-inputs page with the
     * necessary query string parameters to show the change zone page. 
     *
     * @return link to trigger the wizard change zone page
     */
    public static String getChangeZoneLink() {
        String changeZoneLink = WebUtilities.addQueryStringToURL(Constants.PAGE_WIZARD_PRE_INPUT,
                                                                 Constants.QSTR_SHOW_ZONE,
                                                                 Constants.VAL_TRUE);

        changeZoneLink = WebUtilities.addQueryStringToURL(changeZoneLink,
                                                         Constants.QSTR_CLIENT_TYPE,
                                                         Constants.VAL_ACCESS_METHOD_REMOTE);

        changeZoneLink = WebUtilities.addQueryStringToURL(changeZoneLink,
                                                         Constants.QSTR_DETECT_CURRENT,
                                                         Constants.VAL_TRUE);
        return changeZoneLink;
    }

    // All the following code came more or less directly from install.aspxf and
    // install.jspf in Boxworth.

    /**
     * Produces client install captions suitable for display in the message
     * center.
     *
     * This method is public because it is also used by installembed.aspx/jsp.
     *
     * @param wiContext - WI context
     * @return InstallCaptionControl control to render
     */
    private static InstallCaptionControl installClient(WIContext wiContext) {
        InstallCaptionControl installCaption = _installClientFromWizardOutput(wiContext);
        return installCaption;
    }

    /**
     *
     * Utility method to produce client install captions for the message center
     * and plugin page.
     *
     * @param wiContext - WI context
     * @return InstallCaptionControl control to render
     */
    private static InstallCaptionControl _installClientFromWizardOutput(WIContext wiContext) {

        InstallCaptionControl caption = new InstallCaptionControl();
        ClientDeploymentConfiguration clientDConfig = wiContext.getConfiguration().getClientDeploymentConfiguration();
        InstallCaptionState showInstallCaptions = clientDConfig.getShowInstallCaption();

        // NOTHING AT ALL
        if (showInstallCaptions != InstallCaptionState.OFF) {
            if (Include.osSupported(wiContext) && !Include.clientWizardSupported(wiContext)) {
                caption.setMessageType(InstallCaptionControl.MESSAGE_NONE);
            }
            // UNSUPPORTED OS MESSAGE. SHOW DOWNLOAD LINK
            else if (!Include.osSupported(wiContext)) {
                // Unsupported client
                caption.setMessageType(InstallCaptionControl.MESSAGE_WARN);
                caption.setMessageKey("NoICAClientCaption");
            } else {
                caption = implementWizardCaptions(wiContext, showInstallCaptions, caption);
            }

        } else {
            caption.setMessageType(InstallCaptionControl.MESSAGE_NONE);
        }
        return caption;
    }

    /**
     * Utility method to produce client install captions for the message center
     * and plugin page, based on client wizard results.
     *
     * @param wiContext - WI context
     * @param showInstallCaptions - an InstallCaptionState object describing how
     * to show the install captions
     * @param caption - <code>InstallCaptionControl</code> to populate
     * @return InstallCaptionControl control to render
     */
    private static InstallCaptionControl implementWizardCaptions(WIContext wiContext,
                    InstallCaptionState showInstallCaptions, InstallCaptionControl caption) {

        ClientDetectionWizardState sWizardInfo = Include.getWizardState(wiContext);
        // If showInstallCaption = "OFF" we shouldn't be here
        // Do not show captions if sWizardInfo is not initialized e.g. on loggedout page
        if (sWizardInfo.hasDetectionResult()) {
            DisplayInstallCaptionData captionData = getInstallCaptionData(wiContext, sWizardInfo, showInstallCaptions == InstallCaptionState.QUIET);
            buildCaption(wiContext, caption, captionData);
        }
        return caption;
    }

    /**
     * Utility method to produce client install captions for the message center
     * and plugin page.
     *
     * @param wiContext - WI context
     * @param caption - <code>InstallCaptionControl</code> to populate
     * @param captionData - data to build the caption out of
     */
    private static void buildCaption(WIContext wiContext, InstallCaptionControl caption,
                    DisplayInstallCaptionData captionData) {

        if (captionData.getInstallCaptionKey() != null && captionData.getInstallCaptionKey() != "") {
            // set the message level
            ClientDetectionWizardState sWizardInfo = Include.getWizardState(wiContext);
            if (sWizardInfo.hasNoClientResult()) {
                caption.setMessageType(InstallCaptionControl.MESSAGE_WARN);
            } else {
                caption.setMessageType(InstallCaptionControl.MESSAGE_INFO);
            }

            // get the message argument
            // each message talks about either applications or desktops
            boolean isApplicationBrandedSite = UserInterfaceBranding.APPLICATIONS.equals(wiContext.getConfiguration()
                            .getUIConfiguration().getUIBranding());
            String argKey = isApplicationBrandedSite ? "PublishedResourcesApplications" : "PublishedResourcesDesktops";
            String argMessage = wiContext.getString(argKey);

            // set the message key and arguments
            caption.setMessageKey(captionData.getInstallCaptionKey(), new String[] { argMessage });

            // add the select remote / select streaming links
            if (captionData.getShowRemoteLink()) {
                ClientDeploymentConfiguration cdConfig = wiContext.getConfiguration().getClientDeploymentConfiguration();
                boolean nativeClientEnabled = cdConfig.isEnabledClient(MPSClientType.LOCAL_ICA);

                if ((Include.getEffectiveAppAccessMethod(wiContext) != AppAccessMethod.STREAMING)
                                || Include.isLoggedIn(wiContext.getWebAbstraction())) {
                    caption.addMainClient(getWizardCaption(wiContext, Constants.VAL_ACCESS_METHOD_REMOTE));
                    if (nativeClientEnabled) {
                        caption.addMainClient(getAlreadyInstalledCaption(wiContext));
                    }
                } else {
                    caption.addAdditionalClient(getWizardCaption(wiContext, Constants.VAL_ACCESS_METHOD_REMOTE));
                    if (nativeClientEnabled) {
                        caption.addAdditionalClient(getAlreadyInstalledCaption(wiContext));
                    }
                }
            }
            if (captionData.getShowStreamingLink()) {
                if (Include.getEffectiveAppAccessMethod(wiContext) == AppAccessMethod.STREAMING) {
                    caption.addMainClient(getWizardCaption(wiContext, Constants.VAL_ACCESS_METHOD_STREAMING));
                } else {
                    caption.addAdditionalClient(getWizardCaption(wiContext, Constants.VAL_ACCESS_METHOD_STREAMING));
                }
            }
        }
    }

    /**
     * Creates an "already installed" link to the PreInputs page to force the native client.
     *
     * @param wiContext - WI context
     * @return <code>InstallCaption</code> A link to the PreInputs page with appropriate
     *         string to force the native client.
     */
    private static InstallCaption getAlreadyInstalledCaption(WIContext wiContext) {
        String link = Constants.PAGE_WIZARD_PRE_INPUT + "?" + Constants.QSTR_FORCE_ICA_LOCAL + "=" + Constants.VAL_TRUE;
        return new InstallCaption(wiContext.getString("AlreadyInstalled"), link);
    }

    /**
     * Creates a link to the wizard which can run to detect either remote or
     * streaming clients.
     *
     * @param wiContext - WI context
     * @param <code>String</code> appAccessMethodType represents access
     * method.
     * @return <code>InstallCaption</code> A link to the wizard.
     */
    private static InstallCaption getWizardCaption(WIContext wiContext, String appAccessMethodType) {

        ClientDetectionWizardState sWizardInfo = Include.getWizardState(wiContext);
        String WizardCaptionLink = "WizardCaptionLink";
        if (!DetectionUtils.getDualAccessMethod(wiContext)) {
            WizardCaptionLink = SINGLE + WizardCaptionLink;
        } else {
            WizardCaptionLink = appAccessMethodType + WizardCaptionLink;
        }

        String link = Constants.PAGE_WIZARD_PRE_INPUT + "?" + Constants.QSTR_CLIENT_TYPE + "=" + appAccessMethodType;
        if (appAccessMethodType == Constants.VAL_ACCESS_METHOD_REMOTE && sWizardInfo.getRemoteCanUpgrade()) {
            link += "&" + Constants.QSTR_DETECT_CURRENT + "=" + Constants.VAL_TRUE + "&" + Constants.QSTR_SHOW_UPGRADE
                            + "=" + Constants.VAL_TRUE;
        }

        if (appAccessMethodType == Constants.VAL_ACCESS_METHOD_STREAMING && sWizardInfo.getStreamingCanUpgrade()) {
            link += "&" + Constants.QSTR_SHOW_UPGRADE + "=" + Constants.VAL_TRUE;
        }

        return new InstallCaption(wiContext.getString(WizardCaptionLink), link);
    }

    /**
     * Gets the information required to determine tha install captions
     *
     * @param <code> boolean </code> quiet mode only disaplays captions when
     * they are clients are absent Auto mode, displays more verbose information
     */
    private static DisplayInstallCaptionData getInstallCaptionData(WIContext wiContext,
                    ClientDetectionWizardState sWizardInfo, boolean quiet) {
        DisplayInstallCaptionData captionData = new DisplayInstallCaptionData();
        String installCaptionKey = "";

        // see what messages are required
        boolean noRemoteWhenRequired = DetectionUtils.getRemoteAccessPresent(wiContext)
                        && !sWizardInfo.hasRemoteClientResult();
        boolean noStreamingWhenRequired = DetectionUtils.getStreamingAccessPresent(wiContext)
                        && !sWizardInfo.hasStreamingClientResult();

        boolean upgradeRemote = DetectionUtils.getRemoteAccessPresent(wiContext) && sWizardInfo.getRemoteCanUpgrade()
                        && !quiet;
        boolean upgradeStreaming = DetectionUtils.getStreamingAccessPresent(wiContext)
                        && sWizardInfo.getStreamingCanUpgrade() && !quiet;

        boolean javaFallback = showJavaSelectedButNativeBetterMessage(wiContext) && !quiet;

        // Decide which of the possible messages to display
        // Only show one so we don't overload the user with messages
        if (DetectionUtils.getDualAccessMethod(wiContext)) {
            if (noRemoteWhenRequired && noStreamingWhenRequired) {
                installCaptionKey = DUAL + NONE;
                captionData.setShowRemoteLink(true);
                captionData.setShowStreamingLink(true);
            } else if (noRemoteWhenRequired || noStreamingWhenRequired) {
                installCaptionKey = DUAL + ONE + NONE;
                captionData.setShowRemoteLink(noRemoteWhenRequired);
                captionData.setShowStreamingLink(noStreamingWhenRequired);
            } else if (upgradeRemote || upgradeStreaming) {
                installCaptionKey = DUAL + UPGRADE;
                captionData.setShowRemoteLink(upgradeRemote);
                captionData.setShowStreamingLink(upgradeStreaming);
            } else if (javaFallback) {
                installCaptionKey = SINGLE + JAVA;
                captionData.setShowRemoteLink(true);
            }
        } else {
            if (noRemoteWhenRequired || noStreamingWhenRequired) {
                installCaptionKey = SINGLE + NONE;
                captionData.setShowRemoteLink(noRemoteWhenRequired);
                captionData.setShowStreamingLink(noStreamingWhenRequired);
            } else if (upgradeRemote || upgradeStreaming) {
                installCaptionKey = SINGLE + UPGRADE;
                captionData.setShowRemoteLink(upgradeRemote);
                captionData.setShowStreamingLink(upgradeStreaming);
            } else if (javaFallback) {
                installCaptionKey = SINGLE + JAVA;
                captionData.setShowRemoteLink(true);
            }
        }
        captionData.setInstallCaptionKey(installCaptionKey);

        return captionData;
    }

    /**
     * Find what applications are available for this use.
     *
     * @param wiContext
     * @return first element says if remote apps are available, second element
     * says if streaming apps are available
     */
    private static boolean[] findAvailableApps(WIContext wiContext) {
        // see what apps are available for this user
        boolean remoteAppsAvailable = false;
        boolean streamingAppsAvailable = false;

        try {
            UserContext context = SessionUtils.checkOutUserContext(wiContext);
            ResourceInfoSet resourcesSet = context.findAllVisibleResources();

            ResourceInfo[] resources = resourcesSet.getResources();
            for (int i = 0; i < resources.length; i++) {
                ResourceInfo resource = resources[i];
                if (resource.isAccessMethodAvailable(AccessMethod.DISPLAY)) {
                    remoteAppsAvailable = true;
                }
                if (resource.isAccessMethodAvailable(AccessMethod.STREAM)) {
                    streamingAppsAvailable = true;
                }
            }
        } catch (UnknownHostException e) {
            e.printStackTrace();
        } catch (ResourceUnavailableException e) {
            e.printStackTrace();
        } catch (AccessTokenException e) {
            e.printStackTrace();
        }

        return new boolean[] { remoteAppsAvailable, streamingAppsAvailable };
    }

    /**
     * If we are in manual Java Fallback Mode, and the Java client is the
     * selected client, then the user should be given a message to let them go
     * back into the wizard and get the Native client
     *
     * @param wiContext
     * @return true if the message should be shown
     */
    private static boolean showJavaSelectedButNativeBetterMessage(WIContext wiContext) {
        MPSClientType selectedClientType = Include.getSelectedRemoteClient(wiContext);
        boolean manualFallback = MPSClientType.JAVA.equals(selectedClientType)
                        && DetectionUtils.isJavaFallbackModeEnabled(wiContext, JavaFallbackMode.MANUAL);
        boolean dualAndAutoFallback = DetectionUtils.getDualAccessMethod(wiContext)
                        && MPSClientType.JAVA.equals(selectedClientType)
                        && DetectionUtils.isJavaFallbackModeEnabled(wiContext, JavaFallbackMode.AUTO);
        return manualFallback || dualAndAutoFallback;
    }

    // Install Caption Key.
    private static String NONE    = "None";
    private static String UPGRADE = "Upgrade";
    private static String JAVA    = "Java";
    private static String DUAL    = "Dual";
    private static String SINGLE  = "Single";
    private static String ONE     = "One";

    /**
     * Represents a caption and hyperlink that will point to the install program
     * of a particular client.
     */
    private static final class InstallCaption {

        /** The caption that is displayed on screen. */
        private String caption;

        /** The hyperlink that is associated with the caption. */
        private String link;

        /**
         * Creates a new instance.
         *
         * @param c the caption string
         * @param l the hyperlink string
         */
        public InstallCaption(String c, String l) {
            caption = c;
            link = l;
        }

        /**
         * Gets the caption that is displayed on screen.
         *
         * @return the caption string
         */
        public String getCaption() {
            return caption;
        }

        /**
         * Gets the hyperlink that is associated with the caption.
         *
         * @return the hyperlink string
         */
        public String getLink() {
            return link;
        }

        /**
         * Converts the InstallCaption to a string.
         *
         * @return the install caption as a string.
         */
        public String toString() {
            return caption + ":" + link;
        }
    }

    /**
     * Maintains presentation state for install captions.
     */
    private static class InstallCaptionControl {
        // Constants representing different icons

        /** A value representing a warning icon. */
        public static final int MESSAGE_WARN      = 0;

        /** A value representing a message icon. */
        public static final int MESSAGE_INFO      = 1;

        /** A value representing no icon. */
        public static final int MESSAGE_NONE      = 2;

        private int             messageType       = MESSAGE_NONE;
        private String          overrideMessage   = null;
        private String          messageKey        = null;
        private String[]        messageArguments  = null;
        private List          mainClients       = new ArrayList();
        private List          additionalClients = new ArrayList();

        /**
         * Constructor.
         */
        public InstallCaptionControl() {
        }

        /**
         * Adds an InstallCaption representing a client download to the
         * top-level links.
         *
         * @param clientInfo the client to be added
         */
        public void addMainClient(InstallCaption clientInfo) {
            mainClients.add(clientInfo);
        }

        /**
         * Adds an array of InstallCaptions to the control.
         *
         * @param clientInfo the clients to be added
         */
        public void addMainClient(InstallCaption[] clientInfo) {
            if (clientInfo != null) {
                mainClients.addAll(Arrays.asList(clientInfo));
            }
        }

        /**
         * Adds an InstallCaption representing a client download to the 'other
         * clients' links.
         *
         * @param clientInfo the client to be added
         */
        public void addAdditionalClient(InstallCaption clientInfo) {
            additionalClients.add(clientInfo);
        }

        /**
         * Adds an array of InstallCaptions to the control.
         *
         * @param clientInfo the clients to be added
         */
        public void addAdditionalClient(InstallCaption[] clientInfo) {
            if (clientInfo != null) {
                additionalClients.addAll(Arrays.asList(clientInfo));
            }
        }

        /**
         * Gets the message type used to identify the relevant icon. Defaults to
         * MESSAGE_NONE.
         *
         * @return MESSAGE_WARN, _INFO, or _NONE
         */
        public int getMessageType() {
            return messageType;
        }

        /**
         * Sets the message type used to identify the caption's icon.
         *
         * @param value the message type to use
         */
        public void setMessageType(int value) {
            messageType = value;
        }

        /**
         * Gets the key to be used for looking up the localised text for the
         * install caption message.
         *
         * @return string for looking up the message to display
         */
        public String getMessageKey() {
            return messageKey;
        }

        /**
         * Sets the key to be used for looking up the localised text for the
         * install caption message.
         *
         * @param key string for looking up the message to display
         */
        public void setMessageKey(String key) {
            messageKey = key;
        }

        /**
         * Sets the key to be used for looking up the localised text for the
         * install caption message, and arguments that will be used. in the
         * message.
         *
         * @param key string for looking up the message to display
         * @param args the message arguments to be used
         */
        public void setMessageKey(String key, String[] args) {
            messageKey = key;
            messageArguments = args;
        }

        /**
         * Gets the message arguments.
         *
         * @return String[] the keys for each token in the message
         */
        public String[] getMessageArgs() {
            return messageArguments;
        }

        /**
         * Gets the alternate message, intended to override the message key
         * supplied by getMessageKey().
         *
         * @return String the message for the caption
         */
        public String getOverrideMessage() {
            return overrideMessage;
        }

        /**
         * Sets an alternate message, intended to be used instead of the message
         * key.
         *
         * @param value the message for the caption
         */
        public void setOverrideMessage(String value) {
            overrideMessage = value;
        }

        /**
         * Gets the InstallCaptions for the main (top-level) clients.
         *
         * @return InstallCaption[] the clients
         */
        public InstallCaption[] getMainClients() {
            return (InstallCaption[])mainClients.toArray(new InstallCaption[0]);
        }

        /**
         * Gets the InstallCaptions for the additional (other) clients.
         *
         * @return InstallCaption[] the clients
         */
        public InstallCaption[] getAdditionalClients() {
            return (InstallCaption[])additionalClients.toArray(new InstallCaption[0]);
        }

        /**
         * Gets the number of main clients that will be presented.
         *
         * @return int
         */
        public int getMainClientsLength() {
            return mainClients.size();
        }

        /**
         * Gets the number of additional clients that will be presented.
         *
         * @return int
         */
        public int getAdditionalClientsLength() {
            return additionalClients.size();
        }
    }

}
