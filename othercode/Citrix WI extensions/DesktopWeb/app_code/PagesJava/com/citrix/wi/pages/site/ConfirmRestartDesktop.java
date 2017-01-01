package com.citrix.wi.pages.site;

import java.io.IOException;

import com.citrix.wi.controls.ConfirmRestartDesktopPageControl;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.pages.StandardLayout;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.pageutils.ResourceEnumerationUtils;
import com.citrix.wi.pageutils.SessionToken;
import com.citrix.wi.pageutils.SessionUtils;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wing.MessageType;
import com.citrix.wing.util.Strings;
import com.citrix.wing.util.WebUtilities;
import com.citrix.wing.webpn.ResourceInfo;
import com.citrix.wing.webpn.UserContext;

public class ConfirmRestartDesktop extends StandardLayout
{
    protected ConfirmRestartDesktopPageControl viewControl = new ConfirmRestartDesktopPageControl();

    public ConfirmRestartDesktop(WIContext wiContext)
    {
        super(wiContext);
        wiContext.getWebAbstraction().setRequestContextAttribute("viewControl", viewControl);
    }

    protected boolean performImp() throws IOException {
        WebAbstraction web = getWebAbstraction();

        if (web.isPostRequest()) {
            String submitMode = web.getFormParameter(Constants.ID_SUBMIT_MODE);
            String url;

            boolean retryInProgress = Constants.VAL_TRUE.equalsIgnoreCase(web.getFormParameter(Constants.ID_RETRY_IN_PROGRESS));
            String appId = web.getFormParameter(Constants.ID_APPLICATION);

            if (Constants.VAL_OK.equals(submitMode) && !Strings.isEmpty(appId)) {
                String sessionToken = web.getFormParameter(SessionToken.ID_FORM);
                url = UIUtils.getRestartDesktopUrl(wiContext, appId, sessionToken);
            } else {
                url = Constants.PAGE_DIRECT_LAUNCH;
                if (retryInProgress && !Strings.isEmpty(appId)) {
                    url += "?" + Constants.QSTR_RETRY_APPLICATION + "=" + WebUtilities.escapeURL(appId);
                }
            }

            // On a POST, the user is always redirected and no UI is shown.
            web.clientRedirectToUrl(url);
            return false;
        }

        setFeedback(MessageType.WARNING, "RestartDesktopWarning");

        // The internal app id of the desktop is expected to be supplied via a query string parameter.
        String appId = web.getQueryStringParameter(Constants.QSTR_APPLICATION);
        if (Strings.isEmpty(appId)) {
            String url = UIUtils.getMessageRedirectUrl(wiContext, Include.getHomePage(wiContext), MessageType.ERROR, "AppRemoved");
            web.clientRedirectToUrl(url);
            return false;
        }

        viewControl.appId = appId;

        // Fetch the desktop's ResourceInfo object so that the display name (used in the page UI) can be retrieved.
        ResourceInfo resInfo = null;
        UserContext userContext = SessionUtils.checkOutUserContext(wiContext);
        resInfo = ResourceEnumerationUtils.getResource(appId, userContext);

        if ((resInfo == null) || !resInfo.isEnabled()) {
            String url = UIUtils.getMessageRedirectUrl(wiContext, Include.getHomePage(wiContext), MessageType.ERROR, "AppRemoved");
            web.clientRedirectToUrl(url);
            return false;
        } else {
            viewControl.desktopDisplayName = resInfo.getDisplayName();
        }

        layoutControl.formAction = Constants.FORM_POSTBACK;

        // The page UI displays a different message if the desktop is currently being started.
        String retryInProgressQS = web.getQueryStringParameter(Constants.QSTR_RETRY_IN_PROGRESS);
        viewControl.retryInProgress = "true".equals(retryInProgressQS);

        return true;
    }

    protected String getBrowserPageTitleKey() {
        return "BrowserTitleRestartPage";
    }
}
