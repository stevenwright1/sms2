// LaunchDocument.java
// Copyright (c) 2009 - 2010 Citrix Systems, Inc. All Rights Reserved.
// [NFuseVersionAndBuildNumber]
package com.citrix.wi.pages.site;

import com.citrix.wi.pageutils.clientdetection.DetectionUtils;
import com.citrix.wing.webpn.DocumentInfo;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wing.webpn.ResourceInfo;
import com.citrix.wing.webpn.UserContext;
import com.citrix.wi.pageutils.LaunchUtilities;
import com.citrix.wing.MessageType;
import com.citrix.wi.clientdetect.Mode;
import com.citrix.wi.types.MPSClientType;
import com.citrix.wi.pageutils.Constants;
import com.citrix.wi.pageutils.Utils;
import com.citrix.wing.webpn.AccessMethod;
import com.citrix.wi.pageutils.UIUtils;
import com.citrix.wi.pageutils.Include;

/**
 * Class responsible for handling document launching.
 */
public class LaunchDocument extends LaunchResource
{
    final DocumentInfo docInfo;

    LaunchDocument(WIContext wiContext, DocumentInfo docInfo, String appID)
    {
        super(wiContext, (ResourceInfo)docInfo, appID);
        this.docInfo = docInfo;
    }

    public LaunchResult launch(boolean isDirectLaunch, UserContext userContext, String sitePath, long EUEMlaunchTime)
    {
        if (isClientAvailableForDocument())
        {
            LaunchResult result = super.launch(isDirectLaunch, userContext, sitePath, EUEMlaunchTime);
            return result;
        }
        else
        {
            // download if you can't use either the streaming or remote client

            // Clear launch data because we have made it to downloading the content to client
            LaunchUtilities.clearSessionLaunchData(wiContext);

            LaunchResult result = new LaunchResult();
            result.resultCode = LaunchResult.SUCCESS;
            result.launchTag =
                    "try { window.open('" + Utils.documentURLForBrowser(docInfo) + "');" +
                    "} catch (e) {" +
                    "parent.document.location.replace('" + UIUtils.getMessageRedirectUrl(wiContext,
                                            Constants.PAGE_APPLIST, MessageType.ERROR, "AppRemoved") + "');" +
                    " window.close(); }";
            return result;
        }
    }

    // if there are no clients available, documents still can be downloaded
    private boolean isClientAvailableForDocument()
    {
        boolean canUseStreaming = false;
        if (docInfo.isAccessMethodAvailable(AccessMethod.STREAM)
                        && Include.getWizardState(wiContext).isRADEClientAvailable())
        {
            // RADE client is installed - launch is possible
            canUseStreaming = true;
        }

        boolean canUseRemote = false;
        // The RDP client cannot open documents
        if (docInfo.isAccessMethodAvailable(AccessMethod.DISPLAY)
                        && (DetectionUtils.createRemoteClientList(wiContext, Mode.AUTO).size() > 0)
                        && !MPSClientType.EMBEDDED_RDP.equals(Include.getSelectedRemoteClient(wiContext)))
        {
            canUseRemote = true;
        }

        return canUseStreaming || canUseRemote;
    }
}
