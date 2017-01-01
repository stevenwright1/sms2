<%@ Register TagPrefix="wi" TagName="Layout" Src="~/app_data/include/layout.ascx" %>

<%
// change_pin_system.aspx
// Copyright (c) 2002 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<script runat="server">
    void Page_Load(object sender, System.EventArgs e) {
        layout.PageClientScript = "~/auth/clientscripts/change_pin_systemClientScript.ascx";
        layout.PageView = "change_pin_systemView.ascx";
    }
</script>

<%
if (!(new com.citrix.wi.pages.auth.twofactor.ChangePinSystem(wiContext).perform())) {
    return;
}
%>

<wi:Layout id="layout" runat="server" />
