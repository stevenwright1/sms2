<%
// advancedDetection.js
// Copyright (c) 2008 - 2010 Citrix Systems, Inc. All rights reserved.
// Web Interface 5.4.0.0
%>

function onLoadLayout(){
    detectAll();
    redirectToAdvancedPage();
}

function detectAll() {
    var nativeDetected = null;
    var javaDetected = null;
    var rdpDetected = null;
    var bPopupBlocked = null;
    var bCorrectZoneNative = null;
    var bCorrectZoneRdp = null;

    <% if (inputs.detectRemoteClient(ClientType.NATIVE)) { %>
            nativeDetected = detectNativeClient();
            if (nativeDetected) {
                if (!isIcaClientUpToDateClient('<%= WizardUtil.getServerClientVersion(wizardContext) %>')) {
                    setWizardCookieItem('<%=WizardConstants.NATIVE %>','<%=WizardConstants.UPGRADEABLE%>');
                } else {
                    setWizardCookieItem('<%=WizardConstants.NATIVE %>','<%=WizardConstants.VAL_TRUE%>');
                }
                bCorrectZoneNative = isCorrectZone();
                if (bCorrectZoneNative) {
                    setWizardCookieItem('<%=WizardConstants.INCORRECT_ZONE_NATIVE%>','<%=WizardConstants.VAL_FALSE%>');
                } else {
                    setWizardCookieItem('<%=WizardConstants.INCORRECT_ZONE_NATIVE%>','<%=WizardConstants.VAL_TRUE%>');
                }
            }
    <% } %>
     <% if (inputs.detectRemoteClient(ClientType.JAVA) || inputs.isJavaFallback()) { %>
            javaDetected = detectJava();
            if (javaDetected) {
                setWizardCookieItem('<%=WizardConstants.JAVA %>','<%=WizardConstants.VAL_TRUE%>');
                if (bPopupBlocked == null) {
                    bPopupBlocked = popupBlocked();
                }
                if (!bPopupBlocked) {
                    setWizardCookieItem('<%=WizardConstants.POPUP_ALLOWED %>','<%=WizardConstants.VAL_TRUE%>');
                }
            }
     <% } %>
     <% if (inputs.detectRemoteClient(ClientType.RDP)) { %>
            rdpDetected = detectRdp();
            if (rdpDetected) {
                setWizardCookieItem('<%=WizardConstants.RDP %>','<%=WizardConstants.VAL_TRUE%>');
                setWizardCookieItem('<%=WizardConstants.COOKIE_RDPCLASSID%>',rdpDetected);
                bCorrectZoneRdp = checkRdpZone(rdpDetected);
                if (bCorrectZoneRdp) {
                    setWizardCookieItem('<%=WizardConstants.CORRECT_ZONE_RDP %>','<%=WizardConstants.VAL_TRUE%>');
                    if (bPopupBlocked == null) {
                        bPopupBlocked = popupBlocked();
                    }
                    if(!bPopupBlocked) {
                        setWizardCookieItem('<%=WizardConstants.POPUP_ALLOWED %>','<%=WizardConstants.VAL_TRUE%>');
                    }
                }
            }
     <% } %>
}
function redirectToAdvancedPage() {
     location.href="<%=WizardConstants.PAGE_ADVANCED%>";
}
function redirectToFinishPage() {
     location.href="<%=WizardUtil.getUrlWithQueryStrWithCsrf(wizardContext, WizardConstants.PAGE_FINISH)%>";
}
