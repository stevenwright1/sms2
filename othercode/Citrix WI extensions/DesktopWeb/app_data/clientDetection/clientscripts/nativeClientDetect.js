<%
// nativeClientDetect.js
// Copyright (c) 2008 - 2010 Citrix Systems, Inc. All rights reserved.
// Web Interface 5.4.0.0
%>

// Detect whether ICA Client is available.
<%
if ((sClientInfo.osWinCE() && sClientInfo.isIE())  || sClientInfo.osSymbian() ) {
%>
    // It is difficult to reliably detect processor type for WinCE and Symbian Devices
    // and therefore to choose the right flavour of the ICA client.
    // Also, it is not possible to simply download and install ICA client for some of these devices,
    // e.g. WBTs. Therefore, we assume that ICA client is always available.

    function detectNativeClient() {
        return true;
    }

<%
} else if ((sClientInfo.osWin32() || sClientInfo.osWin64()) && sClientInfo.isIE()) {
%>
    // For Win32 IE, we attempt to create an ActiveX Client object. If it
    // succeeds, client is available.
    function detectNativeClient() {
        return (createIcaObj() != null);
    }


    // Create an ActiveX ICA Object.
    function createIcaObj() {
        var obj = null;
        try {
            obj = new ActiveXObject("Citrix.ICAClient");
        } catch (e) {
        }
        return obj;
    }

    // For Win32 IE, after creating the ActiveX Client object we check
    // whether we can actually set a property on the ActiveX object.
    // If we can then we use ICO launch later on
    function isCorrectZone(){
        var obj = createIcaObj();
        var propertyChanged = null;
        try {
            if (obj != null) {
                obj.Launch = true;
                propertyChanged = obj.Launch;
                if (propertyChanged != null && propertyChanged) {
                    return true;
                }
             }
        } catch (e) {
        }
        return false;
    }
<%
} else if (sClientInfo.isNetscape() || sClientInfo.isFirefox() || (sClientInfo.osMacOSX() && sClientInfo.isSafari())) {
%>
    // For Win32 Netscape/Mozilla and Win16 IE and Netscape, we check the
    // presence of plug-in to determine whether the ICA client is available.
    function detectNativeClient() {
        return hasPlugin();
    }

    // Check whether ICA Plugin is available.
    function hasPlugin() {
        var idx;
        var plugIn;
        var mimeIdx;
        var mimeCount;
        var found = false;
        navigator.plugins.refresh(false);
        var count = navigator.plugins.length;

        for (idx = 0; (!found) && (idx < count); idx++) {
            plugIn = navigator.plugins[idx];
            mimeCount = plugIn.length;
            for (mimeIdx = 0; (!found) && (mimeIdx < mimeCount); mimeIdx++) {
                if (plugIn[mimeIdx].type == "application/x-ica") {
                    found = true;
                }
            }
        }
        return found;
    }

    // Non-IE browsers do not have zones so this always return true
    function isCorrectZone(){
        return true;
    }
<%
} else {
%>
    // For other platforms, we assume ICA Client is not available since we can not detect.
    function detectNativeClient() {
        return false;
    }

    // Zones are only applicable to IE and hence for other platforms this returns true
    function isCorrectZone(){
        return true;
    }

<%
}
%>

// Get the ICA Client version.
<%
if ((sClientInfo.osWin32() || sClientInfo.osWin64()) && sClientInfo.isIE()) {
%>
    // For Win32 IE, we get the version from the ActiveX ICO.
    function getIcaClientVersion() {
        var result = "<%=WizardConstants.VAL_ICO_NOT_PRESENT%>";
        var obj = createIcaObj();
        if (obj != null) {
            try {
                var temp = obj.ClientVersion;
                if (temp != null && temp != "") {
                    result = temp;
                }
            } catch (e) {}
        }
        return result;
    }

<%
} else if (sClientInfo.isNetscape() || sClientInfo.isFirefox() || (sClientInfo.osMacOSX() && sClientInfo.isSafari())) {
%>
    // For Win32 Netscape/Mozilla, we get the version from the Plug-in ICO.
    function getIcaClientVersion() {
        var result = "<%=WizardConstants.VAL_ICO_NOT_PRESENT%>";
        if (hasPlugin()) {
            try {
                var temp = document.IcaObj.GetPropValue("ClientVersion");
                if (temp != null && temp != "") {
                    result = temp;
                }
            } catch (e) {}
        }
        return result;
    }

    if (hasPlugin()) {
        document.writeln("<embed type='application/x-ica' hidden='true' name='IcaObj'></embed>");
    }

<%
} else {
%>
    // For other Win32 browsers and non-Win32 platforms, we cannot get the
    // client version.
    function getIcaClientVersion() {
        return "<%=WizardConstants.VAL_ICO_NOT_PRESENT%>";
    }

<%
}
%>


// Detects whether the browser is running in pass-through mode in order to
// determine whether to enable Workspace Control.
//
// If the OS is not Win32 we assume that it is not pass-through. If ICO is
// not present or IsPassThrough method is not available, to be safe, we
// assume it is pass-through as well.
<%
if ((sClientInfo.osWin32() || sClientInfo.osWin64())) {
    if (sClientInfo.isIE()) {
%>
// For Win32 IE, we detect pass-through via the ActiveX ICO.
function detectIcoStatus() {
    var result = null;
    var obj = createIcaObj();
    if (obj != null) {
        try {
            result = obj.IsPassThrough() ? "<%=WizardConstants.VAL_ICO_IS_PASS_THROUGH%>" : "<%=WizardConstants.VAL_ICO_NOT_PASS_THROUGH%>";
        } catch (e) {
            result = "<%=WizardConstants.VAL_ICO_OLD%>";
        }
    } else {
        result = "<%=WizardConstants.VAL_ICO_NOT_PRESENT%>";
    }
    return result;
}

<%
    } else if (sClientInfo.isNetscape() || sClientInfo.isFirefox()) {
%>
// For Win32 Netscape/Mozilla, we detect pass-through via the Plug-in ICO.
function detectIcoStatus() {
    var result = null;
    if (hasPlugin()) {
        try {
            result = document.IcaObj.IsPassThrough() ? "<%=WizardConstants.VAL_ICO_IS_PASS_THROUGH%>" : "<%=WizardConstants.VAL_ICO_NOT_PASS_THROUGH%>";
        } catch (e) {
            result = "<%=WizardConstants.VAL_ICO_OLD%>";
        }
    } else {
        result = "<%=WizardConstants.VAL_ICO_NOT_PRESENT%>";
    }
    return result;
}
<%
    } else {
%>
// For other Win32 browsers, we set the value as pass-through to stop Workspace Control.
function detectIcoStatus() {
    return "<%=WizardConstants.VAL_ICO_IS_PASS_THROUGH%>";
}

<%
    }
} else {
%>
// For non-Win32 platforms, we set the value as not pass-through since it
// does not matter for the time being.
function detectIcoStatus() {
    return "<%=WizardConstants.VAL_ICO_NOT_PASS_THROUGH%>";
}

<%
}
%>

// Compare the installed version of the ICA client with
// the latest version (ie the version that is available on the server).
function isIcaClientUpToDateClient() {
    var detectedVersion = getIcaClientVersion();
    var latestVersion = '<%= WizardUtil.getServerClientVersion(wizardContext) %>';

    if (detectedVersion == '<%=WizardConstants.VAL_ICO_NOT_PRESENT%>'){
        return true;
    }

    return isUpToDateVersion(detectedVersion, latestVersion);    // see commonDetection.js
}

function detectAndSaveIcoStatus() {
    var icoStatus = detectIcoStatus();
    setWizardCookieItem("<%=WizardConstants.ICO_STATUS%>", icoStatus);
}

function detectNative() {
    var detected = detectNativeClient();
    var isSilent = '<%=inputs.getMode() == Mode.SILENT%>' == '<%=WizardConstants.VAL_TRUE%>';
    var correctZone = false;
    var isJavaFallback = '<%=wizardContext.getInputs().isJavaFallback()%>' == '<%=WizardConstants.VAL_TRUE%>';

    // save the results
    if (detected){
        if (!isIcaClientUpToDateClient()){
            setWizardCookieItem('<%=WizardConstants.NATIVE%>','<%=WizardConstants.UPGRADEABLE%>');
        } else {
            setWizardCookieItem('<%=WizardConstants.NATIVE%>','<%=WizardConstants.VAL_TRUE%>');
        }
        correctZone = isCorrectZone();
        if (correctZone) {
            setWizardCookieItem('<%=WizardConstants.INCORRECT_ZONE_NATIVE%>','<%=WizardConstants.VAL_FALSE%>');
        } else {
            setWizardCookieItem('<%=WizardConstants.INCORRECT_ZONE_NATIVE%>','<%=WizardConstants.VAL_TRUE%>');
        }
    }
    if (isJavaFallback) {
        // run the java detection, but don't redirect to other pages
        checkJava(false);
    }

    // go to the next page
    if (isSilent){
        if (detected){
            location.href="<%=model.getNextStepWithCsrf(wizardContext, true) %>";
        } else {
            location.href="<%=model.getNextStepWithCsrf(wizardContext) %>";
        }
    } else {
        if (detected) {
            if (!isIcaClientUpToDateClient()
                 && getWizardCookieItem('<%=WizardConstants.COOKIE_UPGRADE_LATER%>') != '<%=WizardConstants.VAL_TRUE%>'
               ){
                location.href = '<%=WizardConstants.PAGE_UPGRADE_NATIVE %>';
                return;
            }
            if ('<%=inputs.getShowZonePage()%>' == '<%=WizardConstants.VAL_TRUE%>' && !isCorrectZone()
                && '<%=model.getAttribute(WizardConstants.SHOW_ZONE_PAGE_ONLY)%>' != '<%=WizardConstants.VAL_FALSE%>'){
                location.href = '<%=WizardConstants.PAGE_CHANGE_ZONE_HELP %>';
            } else {
                location.href="<%=model.getNextStepWithCsrf(wizardContext, true) %>";
            }
        } else {
            location.href='<%=WizardConstants.PAGE_DOWNLOAD_NATIVE %>';
        }
    }
}
