/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */

package com.citrix.wi.controls;

import java.util.Date;
import com.citrix.wing.types.ClientType;
import com.citrix.wing.RetryRequiredException;

/**
 * Tracks the number of seconds after which delayed resource should be retried
 */
public class RetryControl {
    private long time;
    private String appId;
    private ClientType client;
    private RetryRequiredException retryRequiredException;
    private boolean powerOff;

    public RetryControl(String appId, ClientType client, RetryRequiredException value, boolean powerOff) {
        this.appId = appId;
        this.client = client;
        retryRequiredException = value;
        this.powerOff = powerOff;
        time = new Date().getTime();
    }

    /**
     * Updates the RetryRequiredException with the remaining retryDelayHint.
     * This is calculated by taking the difference between the current time and
     * the time this was created or last calling this method.
     */
    private void updateTimer() {
        long currentTime = new Date().getTime();
        Long timeElapsed = new Long(currentTime - time);
        int timeElapsedInSec = timeElapsed.intValue() / 1000;
        int remainingDelayHint = retryRequiredException.getRetryDelayHint() - timeElapsedInSec;

        // update the retry delay Hint with the remaining delay time
        remainingDelayHint = (remainingDelayHint > 0) ? remainingDelayHint : 0;
        retryRequiredException.setRetryDelayHint(remainingDelayHint);
        time = currentTime;
    }

    /**
     * gets the retry key for this delay
     * @return the retry key
     */
    public String getRetryKey() {
        String result = null;
        if (retryRequiredException != null) {
            result = retryRequiredException.getRetryKey();
        }
        return result;
    }

    /**
     * gets the retry reason
     * @return retry reason
     */
    public String getRetryReason() {
        String result = null;
        if (retryRequiredException != null) {
            result = retryRequiredException.getRetryReason();
        }
        return result;
    }

    /**
     * Gets the retry delay hint in seconds. If the resource needs to be
     * retried immediately i.e. when the elapsed time is more than the
     * retry delay hint then 0 is returned.
     * @return retry delay hint
     */
    public int getRetryDelayHint() {
        int result = 0;
        if (retryRequiredException != null) {
            updateTimer();
            result = retryRequiredException.getRetryDelayHint();
        }
        return result;
    }

    /**
     * Gets the App Id for this control
     * This can be used to get the ResourceInfo
     * @return appId
     */
    public String getAppId() {
        return appId;
    }

    /**
     * Gets the client type for this control.
     * @return client
     */
    public ClientType getClientType()
    {
        return client;
    }

    /**
     * Gets whether the desktop should be powered-off before the initial launch attempt.
     * @return true if a power-off is required
     */
    public boolean getPowerOff() {
        return powerOff;
    }

    /**
     * Sets whether the desktop should be powered-off before the initial launch attempt.
     * @param val true if a power-off is required
     */
    public void setPowerOff(boolean val) {
        powerOff = val;
    }
}
