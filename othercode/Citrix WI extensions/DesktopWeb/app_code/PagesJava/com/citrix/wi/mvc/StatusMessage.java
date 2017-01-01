package com.citrix.wi.mvc;

import com.citrix.wing.MessageType;

/**
 * Represents a status message that needs to be displayed to the end user.
 *
 * There may also be an associated log message that needs to be displayed to
 * the administrator.
 */
public class StatusMessage
{
    private MessageType type;
    private String displayMessageKey;
    private String displayMessageArg;
    private String logMessageKey;
    private Object[] logMessageArgs;
    private boolean recoverableError;

    /**
     * Create a new StatusMessage which is not recoverable.
     *
     * @param type The type of the message (e.g., error, warning, etc.)
     * @param displayMessageKey The key of the localised message string
     * @param displayMessageArg Arguments for the message
     * @param logMessageKey The key of the localised log message string
     * @param logMessageArgs Arguments for the log message
     */
    public StatusMessage(MessageType type,
                   String displayMessageKey,
                   String displayMessageArg,
                   String logMessageKey,
                   Object[] logMessageArgs)
    {
        setType(type);
        setDisplayMessageKey(displayMessageKey);
        setDisplayMessageArg(displayMessageArg);
        setLogMessageKey(logMessageKey);
        setLogMessageArgs(logMessageArgs);
        recoverableError = false;
    }

    /**
     * Create a new StatusMessage of type ERROR, with no arguments, which has no
     * log message and is not recoverable.
     *
     * @param displayMessageKey The key of the localised message string
     */
    public StatusMessage(String displayMessageKey)
    {
        this(MessageType.ERROR, displayMessageKey, null, null, null);
    }

    /**
     * Create a new StatusMessage with no arguments, which has no log message
     * and is not recoverable.
     *
     * @param type The type of the message (e.g., error, warning, etc.)
     * @param displayMessageKey The key of the localised message string
     */
    public StatusMessage(MessageType type, String displayMessageKey)
    {
        this(type, displayMessageKey, null, null, null);
    }

    /**
     * Create a new StatusMessage with a single argument, which has no log
     * message and is not recoverable.
     *
     * @param type The type of the message (e.g., error, warning, etc.)
     * @param displayMessageKey The key of the localised message string
     * @param displayMessageArg Single argument for the message
     */
    public StatusMessage(MessageType type, String displayMessageKey, String displayMessageArg)
    {
        this(type, displayMessageKey, displayMessageArg, null, null);
    }

    /**
     * @return the arguments to the displayed message
     */
    public String getDisplayMessageArg() {
        return displayMessageArg;
    }

    /**
     * @param displayMessageArg the arguments for the displayed message
     */
    public void setDisplayMessageArg(String displayMessageArg) {
        this.displayMessageArg = displayMessageArg;
    }

    /**
     * @return the key to the localised message to be displayed
     */
    public String getDisplayMessageKey() {
        return displayMessageKey;
    }

    /**
     * @param displayMessageKey the key to the localised message
     */
    public void setDisplayMessageKey(String displayMessageKey) {
        this.displayMessageKey = displayMessageKey;
    }

    /**
     * @return the arguments to the localised log message
     */
    public Object[] getLogMessageArgs() {
        return logMessageArgs;
    }

    /**
     * @param logMessageArgs the arguments for the localised log message
     */
    public void setLogMessageArgs(Object[] logMessageArgs) {
        this.logMessageArgs = logMessageArgs;
    }

    /**
     * @return the key for the localised log message
     */
    public String getLogMessageKey() {
        return logMessageKey;
    }

    /**
     * @param logMessageKey the key for the localised log message
     */
    public void setLogMessageKey(String logMessageKey) {
        this.logMessageKey = logMessageKey;
    }

    /**
     * @return the type of the message (e.g., warning, error, etc.)
     */
    public MessageType getType() {
        return type;
    }

    /**
     * @param type the type of the message (e.g., warning, error, etc.)
     */
    public void setType(MessageType type) {
        this.type = type;
    }

    /**
     * @return whether the error is recoverable by a restart of the remote
     * desktop
     */
    public boolean isRecoverableError() {
        return recoverableError;
    }

    /**
     * @param errorIsRecoverable whether the error is recoverable by a restart
     * of the remote desktop.
     */
    public void setRecoverableError(boolean errorIsRecoverable) {
        this.recoverableError = errorIsRecoverable;
    }
}
