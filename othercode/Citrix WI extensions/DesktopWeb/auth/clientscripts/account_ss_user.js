<%
// account_ss_user.js
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

function onLoadLayout() {
    setDefaultFocus();
}

function setDefaultFocus() {
    if (document.getElementById("<%=Constants.ID_USER%>") != null) {
        document.getElementById("<%=Constants.ID_USER%>").focus();
    }
}

function onUsernameTextEntry(f) {
<%
if (! (viewControl.getRestrictDomains() && viewControl.getNumLoginDomains() == 0)) {
%>
    setDisabled(f.<%=Constants.ID_DOMAIN%>, usernameFieldContainsDomain(f));
<%
    if (! wiContext.getClientInfo().osWinCE()) {
%>
    setDisabled(document.getElementById("lblDomain"), usernameFieldContainsDomain(f));
<%
    }
%>
<%
}
%>
}

function setDisabled(item, disabled) {
    if (item) {
        item.disabled = disabled;
    }
}

function usernameFieldContainsDomain(f) {
<%
if (viewControl.getRestrictDomains()) {
%>
    return false;
<%
} else {
%>
    return (f.<%=Constants.ID_USER%>.value.indexOf("\\") != -1);
<%
}
%>
}