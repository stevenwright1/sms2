/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.controls;

import com.citrix.wi.pageutils.Markup;

/**
 * Maintains presentation state for the Display Settings page.
 */
public class DisplaySettingsPageControl extends CommonDisplaySettingsControl {

    private boolean showRememberFolderOption = true;
    private boolean rememberFolder = false;

    private boolean showSearchDisplayOption = true;
    private boolean showSearch = false;

    private boolean showHintsDisplayOption = true;
    private boolean showHints = false;

    /**
     * Tests whether the settings section should be shown.
     * @return <code>true</code> if they should be shown, else <code>false</code>
     */
    public boolean getShowDisplaySettings() {
        return showLanguageOptions ||
               showLayoutSelection ||
               showSearchDisplayOption ||
               showHintsDisplayOption ||
               showRememberFolderOption;
    }

    /**
     * Tests whether the option to remember the current folder should be shown.
     * @return <code>true</code> if the option should be shown, else <code>false</code>
     */
    public boolean getShowRememberFolderOption() {
        return showRememberFolderOption;
    }

    /**
     * Sets whether the option to remember the current folder should be shown.
     * @param value <code>true</code> if the option should be shown, else <code>false</code>
     */
    public void setShowRememberFolderOption( boolean value ) {
        showRememberFolderOption = value;
    }

    /**
     * Tests whether the current folder should be remembered.
     * @return <code>true</code> if remembered, else <code>false</code>
     */
    public boolean getRememberFolder() {
        return rememberFolder;
    }

    /**
     * Sets whether the current folder should be remembered.
     * @param value <code>true</code> if remembered, else <code>false</code>
     */
    public void setRememberFolder( boolean value ) {
        rememberFolder = value;
    }

    /**
    * Tests whether the option to show search should be shown.
    * @return <code>true</code> if the option should be shown, else <code>false</code>
    */
    public boolean getShowSearchDisplayOption() {
        return showSearchDisplayOption;
    }

    /**
     * Sets whether the option to show search should be shown.
     * @param value <code>true</code> if the option should be shown, else <code>false</code>
     */
    public void setShowSearchDisplayOption(boolean value) {
        showSearchDisplayOption = value;
    }

    /**
     * Tests whether search controls should be shown.
     * @return <code>true</code> if shown, else <code>false</code>
     */
    public boolean getShowSearch() {
        return showSearch;
    }

    /**
     * Sets whether the search controls should be shown.
     * @param value <code>true</code> if shown, else <code>false</code>
     */
    public void setShowSearch(boolean value) {
        showSearch = value;
    }

    /**
    * Tests whether the option to show hint should be shown.
    * @return <code>true</code> if the option should be shown, else <code>false</code>
    */
    public boolean getShowHintsDisplayOption() {
        return showHintsDisplayOption;
    }

    /**
     * Sets whether the option to show hint should be shown.
     * @param value <code>true</code> if the option should be shown, else <code>false</code>
     */
    public void setShowHintsDisplayOption(boolean value) {
        showHintsDisplayOption = value;
    }

    /**
     * Tests whether hint text should be shown.
     * @return <code>true</code> if shown, else <code>false</code>
     */
    public boolean getShowHints() {
        return showHints;
    }

    /**
     * Sets whether the hint text should be shown.
     * @param value <code>true</code> if shown, else <code>false</code>
     */
    public void setShowHints(boolean value) {
        showHints = value;
    }

    /**
     * Tests whether the remember folder option is selected.
     * @return <code>checked</code> if selected else an empty string
     */
    public String getRememberFolderCheckedStr() {
        return Markup.checkedStr( rememberFolder );
    }

    /**
     * Tests whether the show search option is selected.
     * @return <code>checked</code> if selected else an empty string
     */
    public String getShowSearchCheckedStr() {
        return Markup.checkedStr(showSearch);
    }

    /**
     * Tests whether the show hint option is selected.
     * @return <code>checked</code> if selected else an empty string
     */
    public String getShowHintsCheckedStr() {
        return Markup.checkedStr(showHints);
    }
}