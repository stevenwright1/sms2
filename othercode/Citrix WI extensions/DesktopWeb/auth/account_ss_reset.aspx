<%@ Register TagPrefix="wi" TagName="Layout" Src="~/app_data/include/layout.ascx" %>

<%
// account_ss_reset.aspx
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<script runat="server">
    void Page_Load(object sender, System.EventArgs e) {
        layout.PageClientScript = "~/auth/clientscripts/account_ss_resetClientScript.ascx";
        layout.PageView = "account_ss_resetView.ascx";
    }
</script>

<%
if(!(new com.citrix.wi.pages.auth.AccountSSReset(wiContext).perform())) {
    return;
}
%>

<wi:Layout id="layout" runat="server" />

<%
if (wiContext.getWebAbstraction().isGetRequest() && AccountSelfService.wasPasswordReset(wiContext)) {
    // Destroy the session so that the login page is displayed if the user refreshes this page in any way.
    // Otherwise an "inconsistent state" page would be displayed due to the auth cookie having been deleted.
    wiContext.getWebAbstraction().abandonSession();
}
%>

