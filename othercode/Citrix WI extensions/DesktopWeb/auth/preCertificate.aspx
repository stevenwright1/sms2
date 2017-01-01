<%@ Register TagPrefix="wi" TagName="Layout" Src="~/app_data/include/layout.ascx" %>

<%
// preCertificate.aspx
// Copyright (c) 2008 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<script runat="server">
    void Page_Load(object sender, System.EventArgs e) {
        layout.PageClientScript = "~/auth/clientscripts/preCertificateClientScript.ascx";
        layout.PageView = "preCertificateView.ascx";
    }
</script>


<%
if (new com.citrix.wi.pages.auth.PreCertificate(wiContext).perform()) {
%>
<wi:Layout id="layout" runat="server" />
<%
}
%>

