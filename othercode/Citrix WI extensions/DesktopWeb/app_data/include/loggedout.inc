<%
// loggedout.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<p><%=wiContext.getString("YouHaveLoggedOutText")%></p>
<%
if (viewControl.getShowCloseWindow()) {
%>
  <p class="secondParagraph">
    <%= wiContext.getString("CloseBrowserForSecurityReasons")%>
  </p>
<%
} else {
%>
<table id="returnToLogOn"><tr><td>
<div class="customButton">
  <a id="loginPageLink" class="leftDoor" href="<%=Constants.PAGE_LOGIN%>?<%=Constants.QSTR_FROM_LOGGEDOUT_PAGE%>=1" title="<%=wiContext.getString("TipLogBackIntoWI")%>"
     ><span class="rightDoor"><%=wiContext.getString("ReturnToLogon")%></span></a>
</div>
</td></tr></table>
<%
}
%>