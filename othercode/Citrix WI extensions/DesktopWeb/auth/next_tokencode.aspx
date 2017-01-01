<%@ Register TagPrefix="wi" TagName="Layout" Src="~/app_data/include/layout.ascx" %>

<%
// next_tokencode.aspx
// Copyright (c) 2002 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<script runat="server">
    void Page_Load(object sender, System.EventArgs e) {
        layout.PageClientScript = "~/auth/clientscripts/next_tokencodeClientScript.ascx";
        layout.PageView = "next_tokencodeView.ascx";
    }
</script>

<%
if (!(new com.citrix.wi.pages.auth.twofactor.NextTokencode(wiContext).perform())) {
    return;
}
%>

<wi:Layout id="layout" runat="server" />
