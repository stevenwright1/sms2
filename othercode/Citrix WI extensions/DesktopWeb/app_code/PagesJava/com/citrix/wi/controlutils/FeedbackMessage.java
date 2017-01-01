/*
 * Copyright (c) 2008 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.controlutils;

import com.citrix.wi.mvc.WIContext;
import com.citrix.wing.MessageType;
import com.citrix.wing.util.CtxArrays;
import com.citrix.wing.util.Objects;
import com.citrix.wing.util.Strings;
import com.citrix.wing.util.WebUtilities;

/**
 * Represents a message that may appear in the feedback area of a page.
 *
 * A feedback message is a localizable string (comprising a resource key and
 * optionally some arguments). There may also be a log event ID (to connect
 * the message to a particular event in the system log) and a message suffix,
 * this being some arbitrary HTML.
 *
 * A message can be marked as transient to request that it is displayed once
 * and not displayed again when the page is refreshed. NOTE, transient messages
 * are only supported when using the feedback store.
 *
 * A message also has a type which indicates its priority level.
 */
public class FeedbackMessage {
    private MessageType type = null;
    private String key = null;
    private String[] args = null;
    private String logEventId = null;
    private String suffix = null;
    private boolean isTransient = false;

    /**
     * Creates a new instance of the specified type, with the specified key.
     *
     * @param type the type of the feedback message
     * @param key the key to the resource string for the message
     * @throws IllegalArgumentException if either type or key is null
     */
    public FeedbackMessage(MessageType type, String key) {
        this(type, key, null, null, null);
    }

    /**
     * Creates a new instance of the specified type, with the specified key and
     * arguments.
     *
     * @param type the type of the feedback message
     * @param key the key to the resource string for the message
     * @param args the arguments to the message
     * @throws IllegalArgumentException if either type or key is null
     */
    public FeedbackMessage(MessageType type, String key, String[] args) {
        this(type, key, args, null, null);
    }

    /**
     * Creates a new instance with the specified properties.
     *
     * @param type the type of the feedback message
     * @param key the key to the resource string for the message
     * @param args the arguments to the message
     * @param logEventId the event ID of the associated log message
     * @param suffix arbitrary string to display after the message
     * @throws IllegalArgumentException if either type or key is null
     */
    public FeedbackMessage(MessageType type, String key, String[] args,
        String logEventId, String suffix) {
        setType(type);
        setKey(key);
        setArgs(args);
        setLogEventId(logEventId);
        setSuffix(suffix);
    }

    /**
     * Gets the type of the feedback message.
     *
     * @return the type of the feedback message
     */
    public MessageType getType() {
        return type;
    }

    /**
     * Sets the type of the feedback message.
     *
     * @param type the type of the feedback message - cannot be null
     * @throws IllegalArgumentException if type is null
     */
    public void setType(MessageType type) {
        if (type == null) {
            throw new IllegalArgumentException("Cannot have null type");
        }

        this.type = type;
    }

    /**
     * Gets the key to the resource string for the message.
     *
     * @return the resource key
     */
    public String getKey() {
        return key;
    }

    /**
     * Sets the key to the resource string for the message.
     *
     * @param key the resource key - cannot be null
     * @throws IllegalArgumentException if key is null
     */
    public void setKey(String key) {
        if (key == null) {
            throw new IllegalArgumentException("Cannot have null resource key");
        }

        this.key = key;
    }

    /**
     * Gets the arguments to the message.
     *
     * @return the message arguments
     */
    public String[] getArgs() {
        return args;
    }

    /**
     * Sets the arguments to the message.
     *
     * @param args the message arguments
     */
    public void setArgs(String[] args) {
        this.args = args;
    }

    /**
     * Gets the event ID of the associated log message.
     *
     * @return the log event ID
     */
    public String getLogEventId() {
        return logEventId;
    }

    /**
     * Sets the event ID of the associated log message.  Validates the provided
     * logEventId to see if it is a hexadecimal number and throws an
     * IllegalArgumentException if it is not.  Null values are permitted.
     *
     * @param logEventId the log event ID
     * @throws IllegalArgumentException if the logEventId is not a valid
     * hexadecimal number
     */
    public void setLogEventId(String logEventId) {
        if (Strings.isLong(logEventId, 16) || logEventId == null) {
            this.logEventId = logEventId;
        } else {
            throw new IllegalArgumentException("logEventId must be a valid hexadecimal number.");
        }
    }

    /**
     * Gets the suffix to be added to the message.
     *
     * @return the message suffix
     */
    public String getSuffix() {
        return suffix;
    }

    /**
     * Sets the suffix to be added to the message.
     *
     * @param suffix the message suffix
     */
    public void setSuffix(String suffix) {
        this.suffix = suffix;
    }

    /**
     * Gets whether the message is marked as transient.
     *
     * @return <code>true</code> if the message is transient.
     */
    public boolean isTransient() {
        return this.isTransient;
    }

    /**
     * Marks the message as transient.
     *
     * @param value <code>true</code> to mark the feedback message as transient.
     */
    public void setTransient(boolean value) {
        this.isTransient = value;
    }

    /**
     * Return the localised text for the message.  The message, any suffix and
     * any event log id are combined.
     *
     * @param wiContext
     * @return The message.
     */
    public String getMessageString(WIContext wiContext) {
        StringBuffer result = new StringBuffer();

        // We assume that the feedback message was constructed in trusted code,
        // so we don't HTML encode it before displaying.
        // If any elements of the message (e.g. its arguments) may have come from
        // an untrusted source such as the query string, we assume that they were
        // safely HTML encoded at the time that the message was created.
        result.append(wiContext.getString(key, args));

        if (suffix != null) {
            result.append(" ");
            result.append(suffix);
        }

        if (logEventId != null) {
            result.append(" ");
            result.append(WebUtilities.partialEscapeHTML(wiContext.getString("ErrorIdText")));
            result.append(" ");
            result.append(logEventId);
        }

        return result.toString();
    }

    /**
     * Determines whether this feedback message has a higher priority than the
     * given message.
     *
     * @param m the feedback message to compare with
     * @return <code>true</code> if this message has a higher priority, <code>false</code> if
     * it has a lower or equal priority
     * @throws IllegalArgumentException if m was null
     */
    public boolean isHigherPriorityThan(FeedbackMessage m) {
        if (m == null) {
            throw new IllegalArgumentException("Cannot have null parameter");
        }

        return type.isHigherPriorityThan(m.type);
    }

    /* See Object */
    public boolean equals(Object o) {
        boolean result = false;

        if ((o != null) && o instanceof FeedbackMessage) {
            FeedbackMessage other = (FeedbackMessage)o;
            result = Objects.equals(this.type, other.type) &&
                     Objects.equals(this.key, other.key) &&
                     CtxArrays.equals(this.args, other.args) &&
                     Objects.equals(this.logEventId, other.logEventId) &&
                     Objects.equals(this.suffix, other.suffix) &&
                     this.isTransient == other.isTransient;
        }

        return result;
    }

    /* See Object */
    public int hashCode() {
        int hashCode = type.hashCode() ^ key.hashCode();

        if (args != null) {
            for (int i = 0; i < args.length; i++) {
                if (args[i] != null) {
                    hashCode ^= args[i].hashCode();
                }
            }
        }

        if (logEventId != null) {
            hashCode ^= logEventId.hashCode();
        }

        if (suffix != null) {
            hashCode ^= suffix.hashCode();
        }

        hashCode ^= new Boolean(isTransient).hashCode();

        return hashCode;
    }

    /* See Object */
    public String toString() {
        StringBuffer buf = new StringBuffer(50);
        buf.append("FeedbackMessage");
        buf.append("[");

        buf.append("Type: ");
        buf.append(type);
        buf.append(",");

        buf.append("Key: ");
        buf.append(key);
        buf.append(",");

        buf.append("Arguments: ");
        buf.append("{");
        if (args != null) {
            for (int i = 0; i < args.length; i++) {
                if (i > 0) {
                    buf.append(",");
                }

                buf.append(args[i]);
            }
        }
        buf.append("}");
        buf.append(",");

        buf.append("Log Event ID: ");
        buf.append(logEventId);
        buf.append(",");

        buf.append("Suffix: ");
        buf.append(suffix);

        buf.append(",");

        buf.append("Transient: ");
        buf.append(isTransient);

        buf.append("]");

        return buf.toString();
    }
}
