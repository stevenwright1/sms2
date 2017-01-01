<%
// footer.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<div id="footer">
  <a href="<%=footerControl.getFooterImgLink(wiContext)%>"
    ><img src="<%=footerControl.getFooterImg(wiContext, layoutControl)%>" alt="<%=footerControl.getFooterImgAltText(wiContext)%>" title=""></a>
  <a href="<%=footerControl.getHdxImgLink(wiContext)%>"
    ><img id="hdxLogo" src="<%=footerControl.getHdxImg(wiContext, layoutControl)%>" alt="<%=footerControl.getHdxImgAltText(wiContext)%>" title=""></a>
<%
  String footerText = footerControl.getFooterText();
  if (!Strings.isEmpty(footerText)) {
%>
  <p><%=footerText%></p>
<% } %>
</div>
