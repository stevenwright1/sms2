/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.controls;

import java.util.HashMap;

import com.citrix.wi.controlutils.FeedbackMessage;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wing.MessageType;

/**
 * Maintains presentation state for the Feedback control.
 */
public class FeedbackControl {
    private FeedbackMessage message = null;

    // Timeout settings (all in minutes).
    // The timeout warning displays in the feedback area so the settings are here.

    // Time interval before session timeout to display warning message.
    private int advanceWarningMins = 5;

    // Time interval to wait before displaying warning message.
    private int warningTimeoutMins = 0;

    // Interval at which to refresh the warning message.
    private int alertIntervalMins = 1;

    private boolean showTimeoutWarning = true;

    /**
     * Gets the CSS class to apply to the feedback area.
     * @return the CSS class as a string
     */
    public String getFeedbackCssClass() {
        String style = null;

        if (message != null) {
            style = (String)FEEDBACK_STYLES.get(message.getType());
        }

        return style;
    }

    private static final HashMap FEEDBACK_STYLES = new HashMap(5);
    static {
        FEEDBACK_STYLES.put(MessageType.ERROR, "feedbackStyleError");
        FEEDBACK_STYLES.put(MessageType.WARNING, "feedbackStyleWarning");
        FEEDBACK_STYLES.put(MessageType.INFORMATION, "feedbackStyleInfo");
        FEEDBACK_STYLES.put(MessageType.SUCCESS, "feedbackStyleSuccess");
    }

    /**
     * Sets feedback unless there is already a higher priority message in the
     * control.
     *
     * @param newMessage the feedback message to set
     * @throws IllegalArgumentException if newMessage is null
     */
    public void setFeedback(FeedbackMessage newMessage) {
        setFeedback(newMessage, false);
    }

    /**
     * Sets feedback (optionally overriding any previous feedback).
     *
     * @param newMessage the new feedback message to set
     * @param override whether to override any previous feedback, regardless of priority
     * @throws IllegalArgumentException if newMessage is null
     */
    public void setFeedback(FeedbackMessage newMessage, boolean override) {
        if (newMessage == null) {
            throw new IllegalArgumentException("new message cannot be null");
        }

        boolean addFeedback = override || // wipe any existing feedback
            !isFeedbackSet() || // no feedback already present
            newMessage.isHigherPriorityThan(this.message); // new feedback takes precedence over old

        if (addFeedback) {
            this.message = newMessage;
        }
    }

    /**
     * Gets the feedback from the control.
     *
     * @return the feedback message
     */
    public FeedbackMessage getFeedback() {
        return message;
    }

    /**
     * Determines whether any feedback has been set.
     *
     * @return <code>true</code> if feedback is set in the control, otherwise <code>false</code>
     */
    public boolean isFeedbackSet() {
        return (message != null);
    }

    /**
     * Gets the time interval before session timeout to display warning message.
     * @return the time interval in minutes
     */
    public int getAdvanceWarningMins() {
        return advanceWarningMins;
    }

    /**
     * Sets the time interval before session timeout to display warning message.
     * @param value the time interval in minutes
     */
    public void setAdvanceWarningMins(int value) {
        advanceWarningMins = value;
    }

    /**
     * Gets the time interval at which to refresh the warning message.
     * @return the time interval in minutes
     */
    public int getAlertIntervalMins() {
        return alertIntervalMins;
    }

    /**
     * Sets the time interval at which to refresh the warning message.
     * @param value the time interval in minutes
     */
    public void setAlertIntervalMins(int value) {
        alertIntervalMins = value;
    }

    /**
     * Gets the time interval to wait before displaying warning message.
     * @return the time interval in minutes
     */
    public int getWarningTimeoutMins() {
        return warningTimeoutMins;
    }

    /**
     * Sets the time interval to wait before displaying warning message.
     * @param value the time interval in minutes
     */
    public void setWarningTimeoutMins(int value) {
        warningTimeoutMins = value;
    }

    /**
     * Gets whether to show the timeout warning.
     * @return <code>true</code> if the warning should be shown, else <code>false</code>
     */
    public boolean getShowTimeoutWarning() {
        return showTimeoutWarning;
    }

    /**
     * Sets whether to show the timeout warning.
     * @param value the value
     */
    public void setShowTimeoutWarning(boolean value) {
        showTimeoutWarning = value;
    }

    /**
     * Sets the timeout alert settings according to the site configuration settings.
     * @param web the <code>WebAbstraction</code>
     */
    public void setTimeoutAlert(WebAbstraction web) {
        // The client-side timeout is set as 1 minute shorter than the
        // server-side timeout. This is close the window that the session is
        // already timed out on the server when the user click logoff or
        // disconnect.
        int timeout = web.getSessionTimeoutMinutes() - 1;
        if (timeout < 5) {
            setAdvanceWarningMins(timeout);
        }
        int warningMins = timeout - getAdvanceWarningMins();
        setWarningTimeoutMins(warningMins);
    }
}
