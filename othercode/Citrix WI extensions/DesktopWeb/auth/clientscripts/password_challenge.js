<%
// password_challenge.js
// Copyright (c) 2003 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

function onLoadLayout() {
    setDefaultFocus();
}

function setDefaultFocus() {
    self.focus();
    var f = document.forms[0];
    f.<%=TwoFactorAuth.ID_PASSWORD_CHALLENGE%>.focus();
}
