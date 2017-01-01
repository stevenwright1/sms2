<%@ Register TagPrefix="wi" TagName="Layout" Src="~/app_data/include/layout.ascx" %>

<%
// change_pin_either.aspx
// Copyright (c) 2002 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<%@ Import namespace="com.citrix.authenticators" %>


<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<script runat="server">
    void Page_Load(object sender, System.EventArgs e) {
        layout.PageClientScript = "~/auth/clientscripts/change_pin_eitherClientScript.ascx";
        layout.PageView = "change_pin_eitherView.ascx";
    }
</script>

<%
new com.citrix.wi.pages.auth.twofactor.ChangePinEither(wiContext).perform();
%>

<wi:Layout id="layout" runat="server" />
