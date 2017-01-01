package com.citrix.wi.pageutils;

import com.citrix.authentication.tokens.AccessToken;
import com.citrix.wing.MessageType;

/**
 * This is used to store results of the create access token method
 */
public class AccessTokenResult {
    /**
     * The access token that is generated. If this was a success, then all other
     * fields are null.
     */
    private AccessToken accessToken     = null;

    /** The type of the feedback message */
    private MessageType messageType     = null;
    /** The key into the resource file for the feedback message */
    private String      messageKey      = null;

    /** If true, a star is put next to the username field */
    private boolean     invalidUsername = false;
    /** If true, a star is put next to the domain field */
    private boolean     invalidDomain   = false;
    /** If true, a star is put next to the context field */
    private boolean     invalidContext  = false;

    /** The value of the username extracted from the given strings */
    private String      username        = null;
    /** The value of the domain extracted from the given strings */
    private String      domain          = null;

    /**
     * Constructor for the successful case
     *
     * @param accessToken
     */
    public AccessTokenResult(AccessToken accessToken) {
        this(accessToken, null, null, false, false, false, null, null);
    }

    /**
     *
     * @param accessToken
     * @param messageType the type of feedback to be displayed
     * @param messageKey the key into the resource bundle of the feedback
     * message
     * @param invalidUsername if the username is invalid
     * @param invalidDomain if the domain is invalid
     * @param invalidContext if the context is invalid
     * @param username the username that has been extracted
     * @param domain the domain that has been extracted
     */
    public AccessTokenResult(AccessToken accessToken, MessageType messageType, String messageKey,
                    boolean invalidUsername, boolean invalidDomain, boolean invalidContext, String username,
                    String domain) {
        this.accessToken = accessToken;
        this.messageType = messageType;
        this.messageKey = messageKey;
        this.invalidUsername = invalidUsername;
        this.invalidDomain = invalidDomain;
        this.invalidContext = invalidContext;
        this.username = username;
        this.domain = domain;

        if (isError()) {
            // when access token is null, we must have an error message
            if (messageType == null || messageKey == null) {
                throw new IllegalArgumentException("Must have a message when there is an error");
            }
        }
    }

    /**
     * Constructor for the error case
     *
     * @param messageType the type of feedback to be displayed
     * @param messageKey the key into the resource bundle of the feedback
     * message
     * @param invalidUsername if the username is invalid
     * @param invalidDomain if the domain is invalid
     * @param username the username that has been extracted
     * @param domain the domain that has been extracted
     */
    public AccessTokenResult(MessageType messageType, String messageKey, boolean invalidUsername,
                    boolean invalidDomain, String username, String domain) {
        this(null, messageType, messageKey, invalidUsername, invalidDomain, false, username, domain);
    }

    /**
     * Get the stored AccessToken
     *
     * @return the accessToken
     */
    public AccessToken getAccessToken() {
        return accessToken;
    }

    /**
     * Get the extracted domain
     *
     * @return the domain, null if it is not stored
     */
    public String getDomain() {
        return domain;
    }

    /**
     * Get the stored feedback message key
     *
     * @return the messageKey
     */
    public String getMessageKey() {
        return messageKey;
    }

    /**
     * Get the stored feedback message type
     *
     * @return the messageType
     */
    public MessageType getMessageType() {
        return messageType;
    }

    /**
     * Get the extracted username
     *
     * @return the username, null if it is not stored
     */
    public String getUsername() {
        return username;
    }

    /**
     * If there is no access token, then this is an error case
     *
     * @return true if this represents an Error case
     */
    public boolean isError() {
        // is an error if there is no access token
        return accessToken == null;
    }

    /**
     * See if the context is invalid.
     *
     * @return the invalidContext
     */
    public boolean isInvalidContext() {
        return invalidContext;
    }

    /**
     * See if the domain should be marked as invalid or not
     *
     * @return the invalidDomain
     */
    public boolean isInvalidDomain() {
        return invalidDomain;
    }

    /**
     * See if the username should be marked as invalid or not
     *
     * @return the invalidUsername
     */
    public boolean isInvalidUsername() {
        return invalidUsername;
    }
}