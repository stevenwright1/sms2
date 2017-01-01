<%
// certificateError.aspx
// Copyright (c) 2002 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->
<%
new com.citrix.wi.pages.auth.CertificateError(wiContext).perform();
LayoutControl layoutControl = (LayoutControl)wiContext.getWebAbstraction().getRequestContextAttribute("layoutControl");
%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
    <title><%=layoutControl.browserTitle%></title>
</head>
<body>
</body>
</html>
