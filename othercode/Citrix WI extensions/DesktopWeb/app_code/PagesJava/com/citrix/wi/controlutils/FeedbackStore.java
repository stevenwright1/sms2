/*
 * Copyright (c) 2008 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.controlutils;

import java.util.Collections;
import java.util.Map;

import com.citrix.authentication.web.AuthUtilities;
import com.citrix.wing.util.SizeLimitedMap;

/**
 * Storage for feedback messages.
 *
 * Messages are associated with a unique ID and then stored. The ID can be used
 * to retrieve the message from the store at a later date.
 *
 * The store has a maximum capacity which is never exceeded. Once full capacity
 * has been reached, the storage of additional messages causes existing
 * messages to be deleted. Messages are deleted in chronological order, oldest
 * first.
 */
public class FeedbackStore {
    // Map of message ID (string) to message (FeedbackMessage)
    private Map messages;

    /** The default maximum capacity of the feedback store. */
    public static final int DEFAULT_MAX_CAPACITY = 25;

    /**
     * Creates a new instance with the default maximum capacity.
     */
    public FeedbackStore() {
        this(DEFAULT_MAX_CAPACITY);
    }

    /**
     * Creates a new instance with the specified maximum capacity.
     *
     * @param maxCapacity the maximum capacity of the store
     * @throws IllegalArgumentException if capacity is less than 1
     */
    public FeedbackStore(int maxCapacity) {
        // This is a constrained map - it will never exceed its maximum capacity
        // Synchronized for access by multiple requests
        this.messages = Collections.synchronizedMap(new SizeLimitedMap(maxCapacity));
    }

    /**
     * Puts a feedback message into the store.
     *
     * The message may be deleted from the store if the store reaches
     * maximum capacity and needs space to store a new message.
     *
     * @param message the feedback message to store
     * @return unique ID (String) for the message that can be used to retrieve it
     * @throws IllegalArgumentException if message was null
     */
    public String put(FeedbackMessage message) {
        if (message == null) {
            throw new IllegalArgumentException("Cannot have a null message");
        }

        String newId = AuthUtilities.createRandomString();
        messages.put(newId, message);

        return newId;
    }

    /**
     * Gets a feedback message from the store.
     *
     * A previously stored message may have been deleted from the store if the
     * store reached maximum capacity.
     *
     * @param id the unique ID of the message
     * @return the stored feedback message or null if the message could not be found
     * @throws IllegalArgumentException if id was null
     */
    public FeedbackMessage get(String id) {
        if (id == null) {
            throw new IllegalArgumentException("Cannot have a null ID");
        }

        return (FeedbackMessage) messages.get(id);
    }

    /**
     * Removes a feedback message from the store.
     *
     * @param id the unique ID of the message
     * @throws IllegalArgumentException if id was null
     */
    public void remove(String id) {
        if (id == null) {
            throw new IllegalArgumentException("Cannot have a null ID");
        }

        messages.remove(id);
    }

    /**
     * Gets the current size of the store.
     *
     * @return the size of the store
     */
    public int size() {
        return messages.size();
    }
}
