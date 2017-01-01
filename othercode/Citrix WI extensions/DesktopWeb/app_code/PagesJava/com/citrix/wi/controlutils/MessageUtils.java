/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.controlutils;

import com.citrix.wi.controls.MessagesControl;
import com.citrix.wi.pageutils.MessageScreenMessage;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.Install;
import com.citrix.wi.pageutils.PasswordExpiryWarningUtils;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wing.MessageType;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.types.MPSClientType;

/**
 * Utility class for the Messages fuctionality.
 */
public class MessageUtils {

    /**
     * Set up a <code>MessagesControl</code>.
     *
     * @param wiContext The <code>WIContext</code>
     * @param viewControl The <code>MessagesControl</code> to populate
     * @param loggedIn Indicate whether this control is being used pre- or post-login
     */
    static public void populate(WIContext wiContext, MessagesControl viewControl, boolean loggedIn) {

        addPasswordExpiryWarningMessage(wiContext, viewControl);
        addInstallMessage(wiContext, viewControl);
        addSaveLaunchFileToDiskMessage(wiContext, viewControl);
    }

    /**
     * Show a warning message if we detect the ICA client and we cant do scripting for ICA launching
     * when using HTTPS on IE on Windows.
     *
     * @param wiContext The <code>WIContext</code>
     * @param viewControl The <code>MessagesControl</code> to populate
     */
    static private void addSaveLaunchFileToDiskMessage(WIContext wiContext, MessagesControl viewControl) {
        MPSClientType selectedClient = Include.getSelectedRemoteClient(wiContext);
        boolean icaLaunchMayCauseSaveDialog =
            selectedClient == MPSClientType.LOCAL_ICA
            && !Include.getWizardState(wiContext).isScriptedLocalIcaLaunchPossible();

        if (Include.isClientConnectionSecure(wiContext.getUserEnvironmentAdaptor())
            && (wiContext.getClientInfo().osWin32() || wiContext.getClientInfo().osWin64())
            && wiContext.getClientInfo().isIE()
            && icaLaunchMayCauseSaveDialog) {

            viewControl.addMessage(new MessageScreenMessage(MessageType.WARNING,
                wiContext.getString("SaveLaunchFileToDisk"), wiContext.getString("SaveLaunchFileToDisk")));
        }
    }

    /**
     * Show any messages relating to client detect or client install.
     *
     * @param wiContext The <code>WIContext</code>
     * @param viewControl The <code>MessagesControl</code> to populate
     */
    static private void addInstallMessage(WIContext wiContext, MessagesControl viewControl) {
        MessageScreenMessage[] installMessages = Install.getMessages(wiContext);
        for (int ix = 0; ix < installMessages.length; ix++) {
            viewControl.addMessage(installMessages[ix]);
        }
    }

    /**
     * Show a message if appropriate to warn of imminent password expiry.
     *
     * @param wiContext The <code>WIContext</code>
     * @param viewControl The <code>MessagesControl</code> to populate
     */
    static private void addPasswordExpiryWarningMessage(WIContext wiContext, MessagesControl viewControl) {
        if (PasswordExpiryWarningUtils.warnUser(wiContext)) {
            String subject = PasswordExpiryWarningUtils.getExpiryDaysAsSentence(wiContext);
            String body = subject + " <a id=\"chgPwdLink\" href='" + Constants.PAGE_CHANGE_PASSWD + "'>" + wiContext.getString("PwdExpWarnLink") + "</a>";
            viewControl.addMessage(new MessageScreenMessage(MessageType.WARNING, subject, body));
        }
    }

    /**
     * Gets a message summary for use on the messages navbar button mouseover tooltip.
     * The summary is the message's subject, truncated if necessary.
     */
    static public String getSummary(WIContext wiContext, MessageScreenMessage message) {
        String result;
        String subject = message.getSubject();
        if (subject.length() <= getMaxMessageSummaryChars(wiContext)) {
            result = subject;
        } else {
            result = subject.substring(0, getMaxMessageSummaryChars(wiContext)).trim() + "...";
        }
        return "- " + result;
    }

    // Stores the maximum number of characters for a message summary.
    static private int maxMessageSummaryChars = -1; // Not yet set up

    // Gets the maximum number of characters for a message summary. The maximum length is
    // read from the language pack.
    static private int getMaxMessageSummaryChars(WIContext wiContext) {
        if (maxMessageSummaryChars == -1) {
            String maxLenStr = wiContext.getString("MaxMessageSummaryChars");
            if (maxLenStr != null) {
                try {
                    maxMessageSummaryChars = Integer.parseInt(maxLenStr);
                } catch (NumberFormatException nfe) { }
            }
            if (maxMessageSummaryChars <= 0) {
                maxMessageSummaryChars = 40; // hardcoded default
            }
        }
        return maxMessageSummaryChars;
    }



}
