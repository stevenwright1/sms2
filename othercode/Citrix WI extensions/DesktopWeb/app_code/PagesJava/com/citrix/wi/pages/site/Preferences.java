package com.citrix.wi.pages.site;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;

import com.citrix.wi.UserPreferences;
import com.citrix.wi.controls.PreferencesPageControl;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pages.StandardLayout;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.PageHistory;
import com.citrix.wi.pageutils.SessionUtils;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wing.MessageType;
import com.citrix.wing.util.Strings;
import com.citrix.wing.webpn.UserContext;

/**
 * This class composes the {@link AccountSettings}, {@link ClientSettings},
 * {@link DisplaySettings}, {@link SessionSettings} and {@link WorkspaceControlSettings} 
 * classes into a single object for display in a single page.
 */
public final class Preferences extends StandardLayout {

    private PreferencesPageControl viewControl = new PreferencesPageControl();

    private WebAbstraction web;
    private UserContext userContext;

    private List preferencesSections = new ArrayList();

    /**
     * Creates a new UserSettings object, which allows the user to customise a 
     * number of different settings, such as workspace control and the display
     * language. 
     * 
     * @param wiContext
     */
    public Preferences(WIContext wiContext) {
        super(wiContext);
        wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);

        preferencesSections.add(new AccountSettings(wiContext, viewControl.getAccountSettings()));
        preferencesSections.add(new ClientSettings(wiContext, viewControl.getClientSettings()));
        preferencesSections.add(new DisplaySettings(wiContext, viewControl.getDisplaySettings()));
        preferencesSections.add(new SessionSettings(wiContext, viewControl.getSessionSettings()));
        preferencesSections.add(new WorkspaceControlSettings(wiContext, viewControl.getWorkspaceControlSettings()));
    }

    protected boolean performImp() throws IOException {
        if (!isAnySectionVisible()) {
            UIUtils.handleMessage(wiContext, Constants.PAGE_APPLIST, MessageType.ERROR, "CustomSettingsNotAllowed");
            return false;
        }

        recordCurrentPageURL();
        web = wiContext.getWebAbstraction();
        layoutControl.isPreferencesPage = true;

        try {
            userContext = SessionUtils.checkOutUserContext(wiContext);
            if (web.isPostRequest()) {
                // Update a private copy of user prefs to avoid the risk of polluting
                // the global one with un-verified data.
                UserPreferences newUserPrefs = Include.getRawUserPrefs(wiContext.getUserEnvironmentAdaptor());

                boolean showForm = processPostRequest(newUserPrefs);

                if (!showForm) {
                    // If no error occurred in processing the POST, save the user preferences.
                    Include.saveUserPrefs(newUserPrefs, wiContext, userContext);
                } else {
					// If an error did occur, re-display the settings.
                    displaySettings();
                }

                return showForm;
            } else {
                displaySettings();
                return true;
            }
        } finally {
            // Dispose of the userContext.  This block will run between the
            // selected process*Request() method and the return statement.  
            SessionUtils.returnUserContext(userContext);
        }
    }

    protected String getBrowserPageTitleKey() {
        return "BrowserTitleUserSettings";
    }

    private void displaySettings() {
        web.setRequestContextAttribute("viewControl", this.viewControl);
        super.setupNavControl();
        navControl.setSettingsLinkActive(false);

        Iterator preferencesSections = this.preferencesSections.iterator();
        while (preferencesSections.hasNext()) {
            PreferencesSection preferencesSection = (PreferencesSection)preferencesSections.next();
            preferencesSection.processGetRequest();
        }

        welcomeControl.setTitle(wiContext.getString("ScreenTitleSettings"));
        layoutControl.formAction = Constants.FORM_POSTBACK;
    }

    private boolean processPostRequest(UserPreferences userPreferences) {
        if (isCancelRequest()) {
            redirectToHomePage();
            return false;
        }

        boolean hasError = false;

        Iterator preferencesSections = this.preferencesSections.iterator();
        while (preferencesSections.hasNext() && !hasError) {
            PreferencesSection preferencesSection = (PreferencesSection)preferencesSections.next();

            // We want to save any values of "true" that are returned by the different 
            // sections as these indicate that the form needs to be redisplayed to 
            // correct errors.  At the same time, however, we don't want to invert or 
            // lose results indicating no errors; nor do we want to exit early and end
            // up with only part of a preferences page.
            if (preferencesSection.processPostRequest(userContext, userPreferences) && !hasError) {
                hasError = true;
            }
        }

        if (hasError) {
            setFeedback(MessageType.ERROR, "SettingsError");
        } else {
            redirectToHomePage("SettingsSaved");
        }

        return hasError;
    }

    private boolean isAnySectionVisible() {
        if (!wiContext.getConfiguration().getAllowCustomizeSettings()) {
            return false;
        }
        
        Iterator preferencesSections = this.preferencesSections.iterator();
        boolean anySectionVisible = false;
        while (preferencesSections.hasNext() && !anySectionVisible) {
            anySectionVisible = ((PreferencesSection)preferencesSections.next()).isVisible();
        }
        return anySectionVisible;
    }

    /*
     * Indicates whether or not the POST request was a Cancel operation.
     */
    private boolean isCancelRequest() {
        String submitMode = web.getFormParameter(Constants.ID_SUBMIT_MODE);
        return !Constants.VAL_OK.equalsIgnoreCase(submitMode);
    }

    /*
     * Redirect the user to the home page.
     */
    private void redirectToHomePage() {
        redirectToHomePage(Strings.EMPTY);
    }

    /*
     * Redirect the user to the home page and display a success message using 
     * the provided key. 
     */
    private void redirectToHomePage(String messageKey) {
        if (!Strings.isEmptyOrWhiteSpace(messageKey)) {
            PageHistory.redirectToHomePage(wiContext, UIUtils.getMessageQueryStr(MessageType.SUCCESS, messageKey));
        } else {
            PageHistory.redirectToHomePage(wiContext, Strings.EMPTY);
        }
    }
}
