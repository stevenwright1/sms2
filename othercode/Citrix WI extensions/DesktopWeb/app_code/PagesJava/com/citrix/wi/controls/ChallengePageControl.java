/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.controls;


/**
 * Maintains presentation state for the challenge page.
 */
public class ChallengePageControl extends DialogPageControl {

    /**
     * Holds value of property radiusChallenge.
     */
    private String radiusChallenge = null;

    /**
     * Default constructor.
     */
    public ChallengePageControl() {
    }

    /**
     * Gets the RADIUS challenge.
     * @return the challenge string
     */
    public String getRadiusChallenge() {
        return this.radiusChallenge;
    }

    /**
     * Sets the RADIUS challenge.
     * @param  radiusChallenge the challenge string
     */
    public void setRadiusChallenge(String radiusChallenge) {
        this.radiusChallenge = radiusChallenge;
    }

}
