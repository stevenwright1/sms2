<%
// account_ss_unlock_account.js
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

function onLoadLayout() {
    setDefaultFocus();
}

function setDefaultFocus() {
    if (document.getElementById("<%=AccountSelfService.ID_BUTTON_FINISH%>") != null) {
        document.getElementById("<%=AccountSelfService.ID_BUTTON_FINISH%>").focus();
    }
}
