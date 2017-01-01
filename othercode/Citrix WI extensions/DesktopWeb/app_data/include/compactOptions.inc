<%
// welcomeMessage.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<ul class="compactOptions">
<% if (navControl.getShowMessages() && navControl.getMessagesLinkActive() && (messagesControl.getNumMessages() > 0)) { %>
  <li><a class="navLink" href="<%=Constants.PAGE_MESSAGES%>"><%=wiContext.getString("ScreenTitleMessages")%></a></li>
<% }
   if (navControl.getShowLoginSettings() && navControl.getLoginSettingsLinkActive()) { %>
  <li><a class="navLink" href="<%=Constants.PAGE_LOGIN_SETTINGS%>"><%=wiContext.getString("ScreenTitleSettings")%></a></li>
<% }
   if (navControl.getShowSettings() && navControl.getSettingsLinkActive()) { %>
  <li><a class="navLink" href="<%=Constants.PAGE_PREFERENCES%>"><%=wiContext.getString("ScreenTitleSettings")%></a></li>
<% } %>
</ul>
<ul id="wscOptions" class="compactOptions">
<% if (navControl.getShowReconnect()) {
     String onClickAttrib = "";
       if (Include.isOSEUEMSupport(wiContext)) {
         onClickAttrib = "OnClick=\"addCurrentTimeToHref(this, '" + Constants.PAGE_RECONNECT + "?" + SessionToken.QSTR_TOKENNAME + "=" + SessionToken.get(wiContext) + "', '" + Constants.QSTR_METRIC_RECONNECT_ID + "');\" ";
       }
%>
  <li>
    <a id="logoutAreaReconnectLink"
      href="<%=Constants.PAGE_RECONNECT%>?<%=SessionToken.QSTR_TOKENNAME%>=<%=SessionToken.get(wiContext)%>" target="<%=Constants.ID_FRAME_RECONNECT + Include.getFrameSuffix(wiContext.getUserEnvironmentAdaptor()) %>"
      <%=onClickAttrib%>
      title="<%= wiContext.getString("TipReconnectButton")%>"
     ><%= wiContext.getString("Reconnect") %></a>
  </li>
<% }
   if (navControl.getShowDisconnect()) {
%>
  <li>
    <a id="logoutAreaDisconnectLink"
      href="javascript:location.replace('<%=Constants.PAGE_DISCONNECT%>?<%=SessionToken.QSTR_TOKENNAME%>=<%=SessionToken.get(wiContext)%>')"
      title="<%= wiContext.getString("TipDisConnectButton")%>"
     ><%= wiContext.getString("Disconnect") %></a>
  </li>
<% } %>
</ul>
