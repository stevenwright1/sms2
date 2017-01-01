<%
// javaClientDetect.js
// Copyright (c) 2008 - 2010 Citrix Systems, Inc. All rights reserved.
// Web Interface 5.4.0.0
%>

function detectJava() {
<%if (sClientInfo.isIE()) {%>
     try {
        bMSvmAvailable = oClientCaps.isComponentInstalled("{08B0E5C0-4FCB-11CF-AAA5-00401C608500}", "ComponentID");
        return bMSvmAvailable && oClientCaps.javaEnabled;
     } catch (e) {}
<%} else if (sClientInfo.isFirefox() || sClientInfo.isNetscape()) {%>
    if (navigator.plugins) {
       // Refresh 'navigator.plugins' to get newly installed plugins.
       // false means do not reload a page with embed tags in
       navigator.plugins.refresh(false);

       // check to find a java plugin
       for (var i=0; i < navigator.plugins.length; i++) {
            var plugin = navigator.plugins[i];
            if (plugin && plugin.name.toLowerCase().indexOf('java') != -1) {
                // Check if this java plugin has a mime type for applets
                for (var j=0; j < plugin.length; j++) {
                    var mimetype = plugin[j];
                    if (mimetype && mimetype.type && mimetype.type.toLowerCase().indexOf('java-applet') != -1) {
                        return true; // we have java applet support
                    }
                }
            }
        }
    }
<%} else {%>
     return navigator.javaEnabled();
<%}%>
}

function checkJavaAvailable() {
    checkJava(true);
}

function checkJava(performRedirects) {
    var detected = detectJava();
    var popupsAllowed = false;
    if (detected) {
        setWizardCookieItem('<%=WizardConstants.JAVA %>','<%=WizardConstants.VAL_TRUE%>');
        popupsAllowed = !popupBlocked();
        if (popupsAllowed) {
            setWizardCookieItem('<%=WizardConstants.POPUP_ALLOWED %>','<%=WizardConstants.VAL_TRUE%>');
        }
    }

    if (performRedirects) {
        if (detected) {
            if (popupsAllowed) {
                location.href="<%=model.getNextStepWithCsrf(wizardContext, true) %>";
            } else if('<%=inputs.getMode() != Mode.SILENT%>' == '<%=WizardConstants.VAL_TRUE%>'){
                location.href="<%=WizardConstants.PAGE_POPUP_HELP %>";
            } else {
                location.href="<%=model.getNextStepWithCsrf(wizardContext) %>";
            }
        } else {
            if ('<%=inputs.getMode() != Mode.SILENT%>' == '<%=WizardConstants.VAL_TRUE%>'){
                location.href="<%=WizardConstants.PAGE_JAVA_NOT_AVAILABLE%>";
            } else {
                location.href="<%=model.getNextStepWithCsrf(wizardContext)%>";
            }
        }
    }
}
