package com.citrix.wi.controls;

import com.citrix.wi.localization.LanguagePack;
import com.citrix.wi.pageutils.Markup;
import com.citrix.wi.types.LayoutType;

/**
 * Encapsulates Display Settings that are common to both the pre-login settings
 * page and the post-login settings page. 
 */
public class CommonDisplaySettingsControl extends PageControl {

    protected boolean showLayoutSelection = false;
    private LayoutType layoutType = LayoutType.AUTO;
    
    protected boolean showLanguageOptions = false;
    private LanguagePack selectedLanguage = null;
    private LanguagePack[] languages = null;

    /**
     * Tests whether layout selection should be shown.
     * @return <code>true</code> if shown, else <code>false</code>
     */
    public boolean getShowLayoutSelection() {
        return showLayoutSelection;
    }

    /**
     * Sets whether layout selection should be shown.
     * @param value <code>true</code> if shown, else <code>false</code>
     */
    public void setShowLayoutSelection(boolean value) {
        showLayoutSelection = value;
    }

    /**
     * Gets the layout type.
     * @return the <code>LayoutType</code>
     */
    public LayoutType getLayoutType() {
        return layoutType;
    }

    /**
     * Sets the layout type.
     * @param value the <code>LayoutType</code>
     */
    public void setLayoutType(LayoutType value) {
        layoutType = value;
    }

    /**
     * Tests whether automatic layout is selected.
     * @return <code>selected/code> if selected else an empty string
     */
    public String getLayoutAutoSelectedStr() {
        return Markup.selectedStr( (layoutType == LayoutType.AUTO) || (layoutType == null) );
    }

    /**
     * Tests whether normal layout is selected.
     * @return <code>selected/code> if selected else an empty string
     */
    public String getLayoutNormalSelectedStr() {
        return Markup.selectedStr( layoutType == LayoutType.NORMAL );
    }

    /**
     * Tests whether compact layout is selected.
     * @return <code>selected/code> if selected else an empty string
     */
    public String getLayoutCompactSelectedStr() {
        return Markup.selectedStr( layoutType == LayoutType.COMPACT );
    }

    /**
     * Gets whether to display the language selection options.
     *
     * @return <code>true</code> if the language options should be displayed, otherwise <code>false</code>
     */
    public boolean getShowLanguageOptions() {
        return showLanguageOptions;
    }

    /**
     * Sets whether to display the language selection options.
     *
     * @param value <code>true</code> if the language options should be displayed, otherwise <code>false</code>
     */
    public void setShowLanguageOptions(boolean value) {
        showLanguageOptions = value;
    }

    /**
     * Gets the array of languages available for selection.
     *
     * @return an array of <code>LanguagePack</code> objects
     */
    public LanguagePack[] getLanguages() {
        return languages;
    }

    /**
     * Sets the array of languages available for selection.
     *
     * @param langs an array of <code>LanguagePack</code> objects
     */
    public void setLanguages(LanguagePack[] langs) {
        languages = langs;
    }

    /**
     * Gets the number of languages available for selection.
     *
     * @return the number of available languages
     */
    public int getNumLanguages() {
        if ( languages == null ) {
            return 0;
        } else {
            return languages.length;
        }
    }

    /**
     * Gets the language that should appear selected.
     *
     * @return a <code>LanguagePack</code> object
     */
    public LanguagePack getSelectedLanguage() {
        return selectedLanguage;
    }

    /**
     * Sets the language that should appear selected.
     *
     * @param lang a <code>LanguagePack</code> object
     */
    public void setSelectedLanguage(LanguagePack lang) {
        selectedLanguage = lang;
    }

    /**
     * Gets the string that controls whether the specified language pack
     * appears selected.
     *
     * @param p the language pack for which to return the string
     * @return either "selected" or the empty string, depending on whether the
     * given pack should appear selected
     */
    public String getLanguageSelectedStr(LanguagePack p) {
        boolean b = false;
        if ( ( selectedLanguage != null ) && ( selectedLanguage.equals( p ) ) ) {
            b = true;
        }
    
        return Markup.selectedStr( b );
    }

}