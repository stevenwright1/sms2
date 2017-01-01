<%
// account_ss_reset.js
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

function onLoadLayout() {
    setDefaultFocus();
}

function setDefaultFocus() {
    // set focus to new password text field
    if (document.getElementById("<%=AccountSelfService.ID_NEW_PASSWORD%>") != null) {
        document.getElementById("<%=AccountSelfService.ID_NEW_PASSWORD%>").focus();
    } else if (document.getElementById("<%=AccountSelfService.ID_BUTTON_FINISH%>") != null) {
        // set focus to finish button if this is the finishing page
        document.getElementById("<%=AccountSelfService.ID_BUTTON_FINISH%>").focus();
    }
}
