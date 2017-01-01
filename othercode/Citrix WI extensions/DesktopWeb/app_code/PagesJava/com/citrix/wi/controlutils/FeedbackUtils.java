/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.controlutils;

import com.citrix.wi.controls.FeedbackControl;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.DisasterRecoveryUtils;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.LaunchUtilities;
import com.citrix.wing.MessageType;
import com.citrix.wing.types.AppAccessMethod;
import com.citrix.wing.util.Strings;
import com.citrix.wing.util.WebUtilities;
import com.citrix.wi.types.ClientPlatform;

/**
 * Utility class for the Feedback functionality.
 */
public class FeedbackUtils {

    /**
     * This code to set up the feedback control is common to all pages.
     * It does the following:
     *      - sets up feedback from the query string parameters of the request
     *      - sets up any persistent feedback
     */
    public static void setupFeedback(WIContext wiContext, FeedbackControl feedbackControl) {
        setFeedbackFromQueryString(wiContext, feedbackControl);
        setPersistentFeedback(wiContext, feedbackControl);
    }


    private static FeedbackMessage getEmbeddedResourceErrorMessage(String embeddedError, WIContext wiContext) {
        // This query string indicates problems with embedded launch that may cause infinite loop in WI.
        // To prevent that, the launch data needs to be cleared.
        LaunchUtilities.clearSessionLaunchData(wiContext);

        FeedbackMessage message = new FeedbackMessage(MessageType.ERROR, "GeneralAppLaunchError");
        
        String classId = null;
        String messageKey = null;

        if (Constants.VAL_GENERAL.equalsIgnoreCase(embeddedError)) {
            FeedbackMessage sessionMessage = (FeedbackMessage)wiContext.getWebAbstraction().getSessionAttribute(Constants.SV_LAUNCH_FILE_FEEDBACK_MSG);
            return (sessionMessage != null) ? sessionMessage : message;
        } else if (Constants.VAL_ICO.equalsIgnoreCase(embeddedError)) {
            classId = wiContext.getConfiguration().getClientDeploymentConfiguration().getClientPackageConfig(wiContext.getClientInfo().getClientPlatform()).getClassId();
            messageKey = "ICOClientLaunchFailed";
        } else if (Constants.VAL_RCO.equalsIgnoreCase(embeddedError)) {
            classId = wiContext.getConfiguration().getClientDeploymentConfiguration().getClientPackageConfig(ClientPlatform.STREAMING_WIN32).getClassId();
            messageKey = "RCOClientLaunchFailed";
        } else {
        	return null;
        }

        String logId = wiContext.log(MessageType.ERROR, messageKey, new Object[] { classId });

        message.setLogEventId(logId);
        return message;
    }

    private static String getEmbeddedErrorValue(WIContext wiContext) {
        String embeddedError = wiContext.getWebAbstraction().getQueryStringParameter(Constants.QSTR_EMBEDDED_RESOURCE_ERROR);
        embeddedError = (Strings.isEmptyOrWhiteSpace(embeddedError)) ? null : embeddedError.trim();
        return embeddedError;
    }

    /**
     * Extracts a feedback message from the query string and attempts to set it
     * in the feedback control.
     *
     * If the feedback control already contains a message then the highest
     * priority message wins.
     *
     * If the query string contains the ID of a stored message then this takes
     * priority over any other message-related query string parameters.
     *
     * @param wiContext the Web Interface context
     * @param feedbackControl the <code>FeedbackControl</code> for the page
     */
    private static void setFeedbackFromQueryString(WIContext wiContext, FeedbackControl feedbackControl) {
        // Do nothing if there is no feedback control.
        if (feedbackControl == null) {
            return;
        }
        FeedbackMessage message = null;

        String embeddedError = getEmbeddedErrorValue(wiContext);
        if (embeddedError != null) {
            message = getEmbeddedResourceErrorMessage(embeddedError, wiContext);
        } else {
            String messageId = wiContext.getWebAbstraction().getQueryStringParameter(QSTR_MESSAGE_ID);
            if (!Strings.isEmpty(messageId)) {
                // A stored message needs to be retrieved
                message = getFeedbackStore(wiContext).get(messageId);
                if (message != null && message.isTransient()) {
                    // Transient messages are removed from the store, so they are not found when the page is reloaded
                    getFeedbackStore(wiContext).remove(messageId);
                }
            } else {
                // Check for any message that has been passed in the query string
                message = buildFeedbackFromQueryString(wiContext);
            }
        }

        // Set the message in the feedback control
        if (message != null) {
            feedbackControl.setFeedback(message);
        }
    }

    /**
     * Builds a new feedback message from query string parameters.
     *
     * The message type, key, argument and log event ID are all obtained from
     * the query string if present.
     *
     * The message suffix is looked up using the message key.
     *
     * @param wiContext the Web Interface context
     * @return a FeedbackMessage object, or null if none could be created.
     */
    private static FeedbackMessage buildFeedbackFromQueryString(WIContext wiContext) {
        FeedbackMessage result = null;

        WebAbstraction web = wiContext.getWebAbstraction();
        String messageTypeStr = web.getRequestParameter(Constants.QSTR_MSG_TYPE);
        MessageType messageType = MessageType.fromString(messageTypeStr);
        String messageKey = web.getRequestParameter(Constants.QSTR_MSG_KEY);

        if ((messageType != null) && !Strings.isEmpty(messageKey)) {
            String argStr = WebUtilities.escapeHTML(web.getQueryStringParameter(Constants.QSTR_MSG_ARGS));
            String[] args = Strings.isEmpty(argStr) ? null : new String[] { argStr };

            String logEventId = WebUtilities.escapeHTML(web.getQueryStringParameter(Constants.QSTR_LOG_EVENT_ID));

            result = new FeedbackMessage(
                messageType,
                messageKey,
                args,
                logEventId,
                Include.getSuffixFromMessageKey(wiContext, messageKey));
        }

        return result;
    }

    /**
     * Sets up any persistent feedback, that is, feedback which
     * should always be displayed (unless there is something more
     * important to display).
     *
     * @param wiContext The WI context.
     * @param feedbackControl The <code>FeedbackControl</code>.
     */
    private static void setPersistentFeedback(WIContext wiContext, FeedbackControl feedbackControl) {

        if (!wiContext.getConfiguration().getAppAccessMethodConfiguration().isEnabledAppAccessMethod(AppAccessMethod.REMOTE)
            && !feedbackControl.isFeedbackSet()) {
            // streaming-only site
            if (!Include.getOsRadeCapable(wiContext.getClientInfo(), wiContext.getUserEnvironmentAdaptor())) {
                feedbackControl.setFeedback(new FeedbackMessage(MessageType.WARNING, "StreamingOptionDisabledNoOsSupport"));
            } else if (!Include.getBrowserRadePluginCapable(wiContext.getClientInfo(), wiContext.getUserEnvironmentAdaptor())) {
                feedbackControl.setFeedback(new FeedbackMessage(MessageType.WARNING, "StreamingOptionDisabledNoBrowserSupport"));
            }
        }

        if (DisasterRecoveryUtils.isDisasterRecoveryInUse(wiContext)) {
            feedbackControl.setFeedback(new FeedbackMessage(MessageType.WARNING, "DisasterRecoveryInUse"));
        }
    }

    /**
     * Modifies a given URL to append query string information relating to a
     * feedback message.
     *
     * The feedback message is stored in the session state for later use.
     *
     * This method can be used to generate a URL which, when requested, will
     * cause the stored message to display in the feedback area.
     *
     * For the display of ad hoc feedback messages (not previously stored), see
     * {@link UIUtils.getMessageRedirectUrl}.
     *
     * @param wiContext the Web Interface context
     * @param message the feedback message to store
     * @param url the URL to append with query string information
     * @return modified version of the URL
     * @throws IllegalArgumentException if message or url are null, or if url is empty
     */
    public static String getFeedbackUrl(WIContext wiContext, FeedbackMessage message, String url) {
        if ((message == null) || Strings.isEmpty(url)) {
            throw new IllegalArgumentException("Cannot have null arguments");
        }

        String messageId = getFeedbackStore(wiContext).put(message);
        String qStrParam = QSTR_MESSAGE_ID + "=" + messageId;

        boolean hasQueryString = (url.indexOf("?") >= 0);
        String prefix = hasQueryString ? "&" : "?";

        return url + prefix + qStrParam;
    }

    private static final String QSTR_MESSAGE_ID = "CTX_MessageId";

    /**
     * Gets the feedback store from the session state.
     *
     * @param wiContext the Web Interface context
     * @return the FeedbackStore
     */
    private static synchronized FeedbackStore getFeedbackStore(WIContext wiContext) {
        WebAbstraction web = wiContext.getWebAbstraction();

        FeedbackStore store = (FeedbackStore)web.getSessionAttribute(SESSION_FEEDBACK_STORE);
        if (store == null) {
            store = new FeedbackStore();
            web.setSessionAttribute(SESSION_FEEDBACK_STORE, store);
        }

        return store;
    }

    private static final String SESSION_FEEDBACK_STORE = "sessionFeedbackStore";
}
