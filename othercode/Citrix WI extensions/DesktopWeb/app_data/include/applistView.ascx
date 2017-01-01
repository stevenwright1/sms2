<%
// applistView.ascx
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<%@ Register TagPrefix="wi" TagName="tabPane" Src="tabPane.ascx" %>
<%@ Register TagPrefix="wi" TagName="compactTabPane" Src="compactTabPane.ascx" %>

<!--#include file="../serverscripts/include.aspxf"-->

<%
if (Include.isCompactLayout(wiContext)) {
%>
  <wi:compactTabPane runat="server" />
<%
} else {
%>
  <wi:tabPane runat="server" />
<%
}
%>