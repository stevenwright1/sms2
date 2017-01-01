<%@ Register TagPrefix="wi" TagName="Layout" Src="~/app_data/include/layout.ascx" %>

<%
// change_pin_user.aspx
// Copyright (c) 2002 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<%@ Import namespace="com.citrix.authenticators" %>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<script runat="server">
    void Page_Load(object sender, System.EventArgs e) {
        layout.PageClientScript = "~/auth/clientscripts/change_pin_userClientScript.ascx";
        layout.PageView = "change_pin_userView.ascx";
    }

</script>

<%
if (!(new com.citrix.wi.pages.auth.twofactor.ChangePinUser(wiContext).perform())) {
    return;
}
%>

<wi:Layout id="layout" runat="server" />
