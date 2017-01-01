<%
// radeClientDetection.js
// Copyright (c) 2008 - 2010 Citrix Systems, Inc. All rights reserved.
// Web Interface 5.4.0.0
%>

// Library of RADE client detection functions
<%
if ((sClientInfo.osWin32() || sClientInfo.osWin64()) && sClientInfo.isIE()) {
%>

    function detectRadeClient() {
        return (createRadeObj() != null);
    }

    // Create an ActiveX RADE Object.
    function createRadeObj() {
        var obj = null;
        try {
            obj = new ActiveXObject("Rco.RadeClient");
        } catch (e) {
        }
        return obj;
    }

    function getRadeClientVersion() {
        var result = "<%=WizardConstants.VAL_RCO_NOT_PRESENT%>";
        var obj = createRadeObj();
        if (obj != null) {
            try {
                var temp = obj.ClientVersion;
                if (temp != null && temp != "") {
                    // Streaming client version 1.0 returns 4.5.3276.1
                    if (temp.indexOf("4.5.") == 0){
                        result = "1.0.0.0";
                    } else {
                        result = temp;
                    }
                }
            } catch (e) {}
        }
        return result;
    }

    function isCorrectRadeZone(){
        var obj = createRadeObj();
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
} else if ((sClientInfo.osWin32() || sClientInfo.osWin64()) && (sClientInfo.isNetscape() || sClientInfo.isFirefox()) ) {
%>

    // For Win32 Netscape/Mozilla, we check the
    // presence of plug-in to determine whether the RADE client is available.
    function detectRadeClient() {
        return hasRadePlugin();
    }

    // Check whether RADE Plugin is available.
    function hasRadePlugin() {
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
                if (plugIn[mimeIdx].type == "application/x-ctxrade") {
                    found = true;
                }
            }
        }
        return found;
    }

    function getRadeClientVersion() {
        var result = "<%=WizardConstants.VAL_RCO_NOT_PRESENT%>";
        if (hasRadePlugin()) {
            try {
                var temp = document.RadeObj.GetPropValue("ClientVersion");
                if (temp != null && temp != "") {
                    result = temp;
                }
            } catch (e) {}
        }
        return result;
    }

    if (hasRadePlugin()){
        document.writeln("<embed type='application/x-ctxrade' hidden='true' name='RadeObj'></embed>");
    }

    //zone are only applicable for IE
    function isCorrectRadeZone(){
        return true;
    }

<%
} else if ((sClientInfo.osWinCE() && sClientInfo.isIE())  || sClientInfo.osSymbian() ) {
%>

    // On WinCE RADE launch is always permitted (but not supported)
    function detectRadeClient() {
        return true;
    }

    function isCorrectRadeZone(){
        return true;
    }

<%
} else {
%>

    // For other platforms, we assume RADE Client is not available since we can not detect.
    function detectRadeClient() {
        return false;
    }

    function getRadeClientVersion() {
        return "<%=WizardConstants.VAL_RCO_NOT_PRESENT%>";
    }

    function isCorrectRadeZone(){
        return true;
    }
<%
}
%>

// Compare the installed version of the ICA client with
// the latest version (ie the version that is available on the server).
function isRadeClientUpToDateClient() {
    var detectedVersion = getRadeClientVersion();
    var latestVersion = '<%=WizardUtil.getServerStreamingClientVersion(wizardContext)%>';

    if (detectedVersion == '<%=WizardConstants.VAL_RCO_NOT_PRESENT%>') {
        return true;
    }

    return isUpToDateVersion(detectedVersion, latestVersion);    // see commonDetection.js
}