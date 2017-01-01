<%
// account_ss_qba.js
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

function onLoadLayout() {
    setDefaultFocus();
}

function setDefaultFocus() {
    if (document.getElementById("<%=AccountSelfService.ID_ANSWER%>") != null) {
        document.getElementById("<%=AccountSelfService.ID_ANSWER%>").focus();
    }
}
