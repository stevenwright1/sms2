<%@ Register TagPrefix="wi" TagName="Layout" Src="~/app_data/include/layout.ascx" %>

<%
// password_challenge.aspx
// Copyright (c) 2002 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<%@ Import namespace="com.citrix.authenticators" %>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<script runat="server">
    void Page_Load(object sender, System.EventArgs e) {
        layout.PageClientScript = "~/auth/clientscripts/password_challengeClientScript.ascx";
        layout.PageView = "password_challengeView.ascx";
    }
</script>

<%
if (!(new com.citrix.wi.pages.auth.twofactor.PasswordChallenge(wiContext).perform())) {
    return;
}
%>

<wi:Layout id="layout" runat="server" />
