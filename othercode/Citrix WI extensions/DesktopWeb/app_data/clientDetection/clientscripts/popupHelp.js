<%
// popupHelp.js
// Copyright (c) 2008 - 2010 Citrix Systems, Inc. All rights reserved.
// Web Interface 5.4.0.0
%>

function onLoadLayout() {
    function checkPopup() {
        var bpopupBlocked = popupBlocked();
        if (!bpopupBlocked){
            setWizardCookieItem('<%=WizardConstants.POPUP_ALLOWED%>','<%=WizardConstants.VAL_TRUE%>');
            location.href='<%=model.getNextStepWithCsrf(wizardContext, true)%>';
        }
    }
    <% if (wizardContext.getClientInfo().isFirefox()) {
    // In Firefox 3.0 we need to wait 100ms
    // otherwise the popup blocker info bar is not shown
    %>
    setTimeout(checkPopup, 100);
    <% } else { %>
    checkPopup();
    <% } %>
}
