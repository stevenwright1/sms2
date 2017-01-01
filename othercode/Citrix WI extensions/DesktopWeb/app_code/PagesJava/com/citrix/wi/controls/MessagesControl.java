/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.controls;

import java.util.ArrayList;
import java.util.Iterator;

import com.citrix.wi.pageutils.MessageScreenMessage;
import com.citrix.wing.MessageType;

/**
 * Class used both by:
 *   - the Messages screen, to display the user's messages
 *   - the navbar messages link, to display the number of waiting messages and a tooltip
 */
public class MessagesControl {

    private ArrayList messages;

    /**
     * Ctor.
     */
    public MessagesControl() {
        messages = new ArrayList(5);
    }

    /**
     * Add a message to be displayed to the user.
     * @param The <code>MessageScreenMessage</code> to add
     */
    public void addMessage(MessageScreenMessage message) {
        messages.add(message);
    }

    /**
     * Get the messages with the given priority.
     * @param priority The priority
     * @return A non-null array of <code>MessageScreenMessage<code>
     */
    public MessageScreenMessage[] getMessages(MessageType priority) {
        return (MessageScreenMessage[])getMessagesList(priority).toArray(new MessageScreenMessage[0]);
    }

    /**
     * Get all the messages, ordered by priority, with most severe first.
     * @return A non-null array of <code>MessageScreenMessage<code>
     */
    public MessageScreenMessage[] getMessages() {
        ArrayList allMessages = getMessagesList(MessageType.ERROR);
        allMessages.addAll(getMessagesList(MessageType.WARNING));
        allMessages.addAll(getMessagesList(MessageType.INFORMATION));
        allMessages.addAll(getMessagesList(MessageType.SUCCESS));

        return (MessageScreenMessage[])allMessages.toArray(new MessageScreenMessage[0]);
    }

    /**
     * Get the number of messages with the given priority.
     * @param priority The priority
     * @return The number of messages.
     */
    public int getNumMessages(MessageType priority) {
        return getMessages(priority).length;
    }

    /**
     * Get the total number of messages (any priority).
     * @return The number of messages.
     */
    public int getNumMessages() {
        return messages.size();
    }

    /**
     * Get the number of messages with the given priority as a <code>String</code>.
     * @return The number of messages.
     */
    public String getNumMessagesAsString(MessageType priority) {
        return Integer.toString(getNumMessages(priority));
    }

    /**
     * Get the total number of messages (any priority) as a <code>String</code>.
     * @return The number of messages.
     */
    public String getNumMessagesAsString() {
        return Integer.toString(getNumMessages());
    }

    // Gets a list of all the messages with the given priority.
    private ArrayList getMessagesList(MessageType priority) {
        ArrayList filtered = new ArrayList(5);
        Iterator it = messages.iterator();
        while (it.hasNext()) {
            MessageScreenMessage message = (MessageScreenMessage)it.next();
            if (message.getPriority() == priority) {
                filtered.add(message);
            }
        }
        return filtered;
    }

    /**
     * Gets the style to apply to the messages nav button.
     * @return the style
     */
    public String getNavButtonStyle() {
        String style = "";
        if (getNumMessages(MessageType.ERROR) > 0) {
            style = "navButtonError";
        } else if (getNumMessages(MessageType.WARNING) > 0) {
            style = "navButtonWarning";
        } else if (getNumMessages(MessageType.INFORMATION) > 0) {
            style = "navButtonInfo";
        } else if (getNumMessages(MessageType.SUCCESS) > 0) {
            style = "navButtonSuccess";
        }
        return style;
    }
}
