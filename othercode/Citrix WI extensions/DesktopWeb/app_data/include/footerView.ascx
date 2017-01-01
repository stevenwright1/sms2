<%
// footerView.ascx
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="../serverscripts/include.aspxf"-->

<%
FooterControl footerControl = (FooterControl)wiContext.getWebAbstraction().getRequestContextAttribute(Constants.CTRL_FOOTER);
Footer.populate(wiContext, footerControl);

LayoutControl layoutControl = (LayoutControl)wiContext.getWebAbstraction().getRequestContextAttribute(Constants.CTRL_LAYOUT);
%>

<!--#include file="footer.inc"-->
