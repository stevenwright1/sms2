/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils;

import com.citrix.wing.MessageType;

/**
 * Holds a single message for displaying on the Messages screen.
 */
public class MessageScreenMessage {
    private MessageType priority;
    private String subject;
    private String body;

    /**
     * Ctor.
     * @param priority The priority of the message.
     * @param subject The messages's subject - the subject must *not* contain any markup. The
     *                subject is displayed on mouseover of the Messages button and may be
     *                truncated.
     * @param body The message's body - this can contain markup. The body is displayed on the
     *                Messages page.
     */
    public MessageScreenMessage(MessageType priority, String subject, String body) {
        this.priority = priority;
        this.subject = subject;
        this.body = body;
    }

    /**
     * Get the message's priority.
     * @return The priority
     */
    public MessageType getPriority() {
        return priority;
    }

    /**
     * Get the message's subject.
     * @return The subject
     */
    public String getSubject() {
        return subject;
    }

    /**
     * Get the message's body.
     * @return The further information
     */
    public String getBody() {
        return body;
    }
}
