/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pages.site;

import java.io.IOException;

import com.citrix.wi.UserPreferences;
import com.citrix.wi.controls.ViewStyleSettingsPageControl;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pages.StandardLayout;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.SessionUtils;
import com.citrix.wi.types.CompactApplicationView;
import com.citrix.wing.MessageType;
import com.citrix.wing.webpn.UserContext;

public class ViewStyleSettings extends StandardLayout {

    private ViewStyleSettingsPageControl viewControl = new ViewStyleSettingsPageControl();
    private boolean showPage = true;

    public ViewStyleSettings(WIContext wiContext) {
        super(wiContext);
        wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
    }

    protected boolean performImp() throws IOException {
        UserContext userContext = SessionUtils.checkOutUserContext(wiContext);
        processRequest(userContext);

        if (showPage) {
            initHtml();
        }

        SessionUtils.returnUserContext(userContext);

        return showPage;
    }

    private void initHtml() {
        recordCurrentPageURL();

        setupNavControl();

        navControl.setShowGraphicsMode(false);
        welcomeControl.setTitle(wiContext.getString("ScreenTitleChangeView"));
        welcomeControl.setBody("");
        layoutControl.formAction = Constants.FORM_POSTBACK;
        viewControl.viewStyle = wiContext.getUserPreferences().getCompactViewStyle();
    }

    protected String getBrowserPageTitleKey() {
        return "BrowserTitleChangeView";
    }

    private void processRequest(UserContext userContext) {
        WebAbstraction web = wiContext.getWebAbstraction();
        viewControl.allowedViewStyles = wiContext.getConfiguration().getUIConfiguration().getCompactViewStyles();

        if (viewControl.allowedViewStyles.size() > 1) {
            if (Include.isCompactLayout(wiContext)) {

                if (web.isPostRequest()) {

                    String submitMode = web.getFormParameter(Constants.ID_SUBMIT_MODE);

                    if (Constants.VAL_OK.equals(submitMode)) {

                        // SAVE button
                        CompactApplicationView viewStyle = CompactApplicationView.fromString(
                            web.getFormParameter(Constants.ID_RADIO_COMPACT_VIEW_STYLE));

                        if (!viewControl.allowedViewStyles.contains(viewStyle)) {
                            // An invalid view style was specified in the POST data
                            setFeedback(MessageType.ERROR, "SettingsError");
                        } else {
                            // Valid data was supplied
                            showPage = false;
                            UserPreferences newUserPrefs = Include.getRawUserPrefs(wiContext.getUserEnvironmentAdaptor());
                            newUserPrefs.setCompactViewStyle(viewStyle);
                            Include.saveUserPrefs(newUserPrefs, wiContext, userContext);
                        }

                    } else {
                        // CANCEL button
                        showPage = false;
                    }
                }
            } else {
                // we're not in low graphics mode, show this page shouldn't be shown
                showPage = false;
            }
        } else {
            // there are no views to choose from
            showPage = false;
        }

        if (!showPage) {
            // redirect to home page
            web.clientRedirectToUrl(Constants.PAGE_APPLIST);
        }
    }

}
