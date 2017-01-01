<%
// account_ss_entry.js
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

function onLoadLayout() {
    setDefaultFocus();
}

function setDefaultFocus() {
    if (document.getElementById("<%=AccountSelfService.ID_RADIO_ACCOUNT_UNLOCK%>") != null) {
        document.getElementById("<%=AccountSelfService.ID_RADIO_ACCOUNT_UNLOCK%>").focus();
    } else {
        document.forms[0].btnContinue.focus();
    }
}
