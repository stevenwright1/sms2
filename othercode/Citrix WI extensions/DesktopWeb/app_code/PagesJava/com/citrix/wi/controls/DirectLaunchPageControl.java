/*
 * DirectLaunchPageControl.java
 *
 * Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
 *
 */
package com.citrix.wi.controls;

import com.citrix.wi.mvc.WIContext;

/**
 * Maintains presentation state for the Direct Launch page.
 */
public class DirectLaunchPageControl {
    private String appNameDisplayTextKey = null;
    private boolean isAppNameBold = false;
    private String logOffHelpString = null;
    private String wizardLink = null;
    private ResourceControl resourceControl = null;
    private boolean showDesktopUI;
    private String appLaunchLink;

    /**
     * Sets the launch link used for launching in application view.
     * @param value the launch link used for launching in application view
     */
    public void setAppLaunchLink(String value) {
        appLaunchLink = value;
    }

    /**
     * Gets the launch link used for launching in application view.
     * @return the launch link used for launching in application view.
     */
    public String getAppLaunchLink() {
        return appLaunchLink;
    }

    /**
     * Sets if the application name is displayed in a bold font.
     * @param value boolean telling if the application name should be displayed in a bold font.
     */
    public void setIsAppNameBold(boolean value) {
        isAppNameBold = value;
    }

    /**
     * Whether to display the desktop-specific UI.
     * 
     * @return boolean indicating whether to display the desktop-specific UI
     */
    public boolean getShowDesktopUI() {
        return showDesktopUI;
    }

    /**
     * Sets to display the desktop-specific UI.
     * 
     * @param value boolean indicating whether to display the desktop-specific UI
     */
    public void setShowDesktopUI(boolean value) {
        showDesktopUI = value;
    }

    /**
     * Sets the ResourceControl to be displayed on the direct launch page
     * @param value the resource control
     */
    public void setResourceControl(ResourceControl resourceControl) {
        this.resourceControl = resourceControl;
    }

    /**
     * Gets the ResourceControl if need to be displayed
     * @return ResourceControl if any, null if there is no delayed launch
     */
    public ResourceControl getResourceControl() {
        return resourceControl;
    }

    /**
     * Gets the application name string to be displayed.
     * @return the string for the application name
     */
    public String getAppNameText(WIContext wiContext) {
        if (resourceControl == null) {
            throw new IllegalStateException("need a non-null resource control");
        }
        String displayName = resourceControl.getName(wiContext);

        if (isAppNameBold) {
            displayName = "<strong>" + displayName + "</strong>";
        }

        if (appNameDisplayTextKey != null) {
            displayName = wiContext.getString(appNameDisplayTextKey, displayName);
        }

        return displayName;
    }

    /**
     * Sets the application name display text key to be displayed
     * @param value the application name display text key
     */
    public void setAppNameDisplayTextKey(String value) {
        appNameDisplayTextKey = value;
    }

    /**
     * Sets the text for the log off help ("?" icon)
     */
    public void setLogOffHelpString(String value) {
        logOffHelpString = value;
    }

    /**
     * Gets the text for the log off help ("?" icon)
     */
    public String getLogOffHelpString() {
        return logOffHelpString;
    }

    /**
     * Get the link that will run the wizard.
     * If it is null, don't show the link.
     * @return the wizardLink
     */
    public String getWizardLink() {
        return wizardLink;
    }

    /**
     * Set the link that takes you into the wizard.
     * Set it to null to hide the link.
     * @param wizardLink the wizardLink to set
     */
    public void setWizardLink(String wizardLink) {
        this.wizardLink = wizardLink;
    }
}
