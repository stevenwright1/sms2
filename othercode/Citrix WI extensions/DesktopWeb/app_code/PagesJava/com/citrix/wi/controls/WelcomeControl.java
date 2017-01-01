/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */
package com.citrix.wi.controls;

/**
 * Maintains presentation state for the Welcome control.
 */
public class WelcomeControl {
    private String title = "";
    private String titleClass = "";
    private String secondTitle = "";
    private String body = "";

    /**
     * Gets the title.
     * @return the title string
     */
    public String getTitle() {
        return title;
    }

    /**
     * Sets the title.
     * @param value the title string
     */
    public void setTitle( String value ) {
        title = value;
    }

    /**
     * Gets the CSS class name for the title.
     * @return the CSS class name string.
     */
    public String getTitleClass() {
        return titleClass;
    }

    /**
     * Sets the CSS class name for the title.
     * @param value the CSS class name string.
     */
    public void setTitleClass(String value) {
        titleClass = value;
    }

    /**
     * Gets the second title.
     * @return the second title string
     */
    public String getSecondTitle() {
        return secondTitle;
    }

    /**
     * Gets the body text.
     * @return the body text
     */
    public String getBody() {
        return body;
    }

    /**
     * Sets the body text.
     * @param value the body text
     */
    public void setBody( String value ) {
        body = value;
    }

    /**
     * Shows the welcome body text if defined
     * @return the text
     */
    public boolean getShowBody() {
        return (body.length() > 0);
    }

    /**
     * Shows the welcome title if defined
     * @return true to show the title
     */
    public boolean getShowTitle() {
        return (title.length() > 0);
    }

    /**
     * Shows the welcome second title if defined
     * @return true to show the second title
     */
    public boolean getShowSecondTitle() {
        return (secondTitle.length() > 0);
    }

}