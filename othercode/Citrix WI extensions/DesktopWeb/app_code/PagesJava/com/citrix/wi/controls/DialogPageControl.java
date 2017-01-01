/*
 * Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.controls;

/**
 * Maintains presentation state common to all dialog pages.
 *
 * All dialog pages have in common the URL that the dialog should
 * post data back to.
 */
public class DialogPageControl extends PageControl {
    private String postURL = "";

    /**
     * Gets the URL to which the dialog should POST.
     * @return the POST URL string.
     */
    public String getPostURL() {
        return postURL;
    }

    /**
     * Sets the URL to which the dialog should POST.
     * @param postURL the POST URL string.
     */
    public void setPostURL( String postURL ) {
        if( postURL == null ) {
            throw new IllegalArgumentException( "POST URL must be non-null" );
        }

        this.postURL = postURL;
    }
}
