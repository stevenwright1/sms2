/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.site;

import com.citrix.wi.UserPreferences;
import com.citrix.wi.controls.PageControl;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wing.util.Strings;
import com.citrix.wing.webpn.UserContext;

/**
 * Defines a base class for the different sections of the preferences page and
 * handles the logic common to all sections. Individual preferences pages can
 * plug in to the execution flow using a number of defined extension points, but
 * may not over-ride the execution flow itself.
 */
public abstract class PreferencesSection {

    protected WIContext      wiContext;
    protected WebAbstraction web;

    protected PageControl    viewControl;

    /**
     * Creates a new PreferencesSection given the provided WIContext and
     * sectionNameKey.
     * 
     * @param wiContext
     * @param sectionNameKey A resource file string identifier for the section's
     * title as displayed.
     * @throws IllegalArgumentException if any parameter is null.
     */
    protected PreferencesSection(WIContext wiContext, PageControl viewControl) {
        if (wiContext == null) {
            throw new IllegalArgumentException("wiContext is null.");
        }
        
        if (viewControl == null) {
            throw new IllegalArgumentException("viewControl is null.");
        }

        this.wiContext = wiContext;
        this.web = wiContext.getWebAbstraction();
        this.viewControl = viewControl;
    }

    /**
     * Indicates whether or not the PreferencesSection has visible settings.
     * Generally, settings are visible if the feature is supported and the
     * administrator has granted the user permission to customise the settings.
     * 
     * When the method returns false, the PreferencesSection will be hidden
     * entirely.
     * 
     * @return <code>true</code> if the PreferencesSection has visible settings,
     * else <code>false</code>.
     */
    public abstract boolean isVisible();

    /**
     * Processes a GET request for a PreferencesSection. There are two extension
     * points to this execution flow: getDataFromUserPreferences() and 
     * setupView(), both abstract methods that must be implemented.
     * 
     * The execution flow is:
     * <ol>
     * <li>Retrieve data from {@link UserPreferences} for display;</li>
     * <li>Set up the specific PreferencesSection view.</li>
     * </ol>
     * 
     * @return <code>true</code>, indicating that WI should display the form.
     * 
     * @see PreferencesSection#getDataFromUserPreferences()
     * @see PreferencesSection#setupView()
     */
    protected final void processGetRequest() {
        getDataFromUserPreferences(wiContext.getUserPreferences());
        setupView();
    }

    /**
     * Processes a POST request for a PreferencesSection. There are two
     * extension points to this execution flow: isDataValid() and
     * savePreferences(), both abstract methods that must be implemented.
     * 
     * The execution flow is:
     * <ol>
     * <li>If the user submitted valid data, save their preferences and cause
     * WI to return the user to the home page.</li>
     * <li>If the user submitted invalid data, don't save their preferences 
     * and cause WI to display an error.</li>
     * </ol>
     * 
     * @param userContext The user context for the request.
     * @param userPreferences The user preferences to update with the settings from 
     * this preferences section.
     * 
     * @return <code>false</code> to indicate that nothing went wrong in 
     * processing the POST request and that WI shouldn't re-display the form; 
     * else <code>true</code>.
     * 
     * @see PreferencesSection#isDataValid()
     * @see PreferencesSection#savePreferences()
     */
    protected final boolean processPostRequest(UserContext userContext, UserPreferences userPrefs) {
        if (isDataValid()) {
            savePreferences(userContext, userPrefs);
            return false;
        }

        return true;
    }

    /**
     * Retrieves data from the UserPreferences and stores it in the current
     * PreferencesSection's state. It is up to subclasses to determine what data
     * is to be retrieved from UserPreferences and how it is to be stored.
     * 
     * @param userPreferences The UserPreferences object from which the data are
     * to be retrieved.
     */
    protected abstract void getDataFromUserPreferences(UserPreferences userPreferences);

    /**
     * Indicates whether or not the data input into the form is valid for the
     * current PreferencesSection.
     * 
     * @return <code>true</code> if valid, else <code>false</code>.
     */
    protected abstract boolean isDataValid();

    /**
     * Persists the user's selections to UserPreferences. It is up to the
     * implementing subclass to determine what data is to be stored, and how.
     * 
     * @param userContext The UserContext for the current request.
     * @param userPreferences The user preferences to update with the settings from 
     * this preferences section. 
     */
    protected abstract void savePreferences(UserContext userContext, UserPreferences userPreferences);

    /**
     * Sets up the specific PreferencesSection's view. 
     */
    protected abstract void setupView();

    /**
     * Determines whether or not the value supplied by a checkbox is valid. The
     * value should only ever be "on" or "off". If no value is supplied (i.e.,
     * the parameter is null, empty, or whitespace only) then the method returns
     * true.
     * 
     * @param checkboxValue The value to validate.
     * @return <code>true</code> if the value is "on" or "off", or if the string
     * is null, empty or whitespace. Returns <code>false</code> in all other
     * situations.
     */
    protected boolean isCheckboxValueValid(String checkboxValue) {
        boolean valueSpecified = !Strings.isEmptyOrWhiteSpace(checkboxValue);
        boolean isValueOn = Constants.VAL_ON.equalsIgnoreCase(checkboxValue);
        boolean isValueOff = Constants.VAL_OFF.equalsIgnoreCase(checkboxValue);

		return (isValueOn || isValueOff || !valueSpecified);
    }
}
