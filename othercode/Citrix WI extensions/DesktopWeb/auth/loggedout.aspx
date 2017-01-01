<%@ Register TagPrefix="wi" TagName="Layout" Src="~/app_data/include/layout.ascx" %>
<%
// loggedout.aspx
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>


<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<script runat="server">

    void Page_Load(object sender, System.EventArgs e) {
        layout.PageClientScript = "~/auth/clientscripts/loggedoutClientScript.ascx";
        layout.PageView = "loggedoutView.ascx";
    }

</script>

<%
if ((new com.citrix.wi.pages.auth.LoggedOut(wiContext)).perform()) {
%>
<wi:Layout id="layout" runat="server" />

<%
}

// A new Session will have been created for this page request as it has already been
// abandoned while logging out.
// Abandon this new session otherwise the session will remain active until timeout.
// Avoid session fixation by checking that it's a new session which is abandoned
if (wiContext.getWebAbstraction().isNewSession()) {
    wiContext.getWebAbstraction().abandonSession();
}
%>
