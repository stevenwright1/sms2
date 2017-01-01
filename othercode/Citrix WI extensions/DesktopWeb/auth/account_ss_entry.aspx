<%@ Register TagPrefix="wi" TagName="Layout" Src="~/app_data/include/layout.ascx" %>

<%
// account_ss_entry.aspx
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<script runat="server">
    void Page_Load(object sender, System.EventArgs e) {
        layout.PageClientScript = "~/auth/clientscripts/account_ss_entryClientScript.ascx";
        layout.PageView = "account_ss_entryView.ascx";
    }
</script>

<%
if (!(new com.citrix.wi.pages.auth.AccountSSEntry(wiContext).perform())) {
    return;
}
%>

<wi:Layout id="layout" runat="server" />
