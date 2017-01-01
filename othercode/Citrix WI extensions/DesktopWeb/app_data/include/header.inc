<%
// header.inc
// Copyright (c) 2002 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<div id="headerWrapper">
  <div id="header">
    <table id="headerLeft" cellspacing="0">
      <tr>
<%
if (!Include.isCompactLayout(wiContext) && searchBoxControl.show) {
    String searchValue = wiContext.getString("Search");
    String searchClass = "searchInactive";
    if (layoutControl.isSearchPage) {
        searchValue = WebUtilities.escapeHTML(searchBoxControl.query);
        searchClass = "";
    }
%>
        <td id="searchCell">
         <form action="javascript:redirectToSearchResults()" >
           <div id="searchContainer"> <% // Apparently, you cannot have an input element directly inside a form element, hence this div %>
             <input type="text" maxlength="60" title="<%=wiContext.getString("SearchTooltip")%>" id="searchEntry" class="<%=searchClass%>" value="<%=searchValue%>"
                   onfocus="searchTextFocus(this);" onblur="updateSearchStyle(false);"><a id="searchLink" href="javascript:redirectToSearchResults();" tabIndex="-1"></a>
               <% // This button is hidden by default and is only used when high contrast mode is enabled. %>
             <input id="highContrast_SearchButton" type="button"
                     onClick="javascript:redirectToSearchResults()"
                     title="<%=wiContext.getString("SearchTooltip")%>"
                     value='<%=wiContext.getString("Search")%>'>
           </div>
         </form>
        </td>
<%
}
%>

<%
if (navControl.getShowUserName()) {
%>
        <td>
          <span id="username" ><strong>
            <%= wiContext.getString("LoggedOnAs", WebUtilities.escapeHTML(navControl.getUserName())) %>
          </strong></span>
        </td>
<%
}
%>

        <td>
          <table id="headerRight" cellspacing="0">
            <tr>
<%
if (!Include.isCompactLayout(wiContext)) {
    ClientInfo clientInfo = wiContext.getClientInfo();
%>

<%
    if (navControl.getShowMessages() && (messagesControl.getNumMessages() > 0)) {
        String href = navControl.getMessagesLinkActive() ? "href=\"" + Constants.PAGE_MESSAGES + "\"" : "";
        String className = navControl.getMessagesLinkActive() ? "" : "class=\"inactiveNavLink\"";
%>
              <td <%=className%>>
                <a id="messagesLink" class="navLink" <%=href%>><span><%=wiContext.getString("ScreenTitleMessages")%></span></a>
              </td>
<% } %>

<% if (navControl.getShowLoginSettings()) {
        String href = navControl.getLoginSettingsLinkActive() ? "href=\"" + Constants.PAGE_LOGIN_SETTINGS + "\"" : "";
        String className = navControl.getLoginSettingsLinkActive() ? "" : "class=\"inactiveNavLink\"";
%>
              <td <%=className%>>
                <a id="settingsLink" class="navLink" <%=href%>><span><%=wiContext.getString("ScreenTitleSettings")%></span></a>
              </td>
<% } %>

<%
    if (navControl.getShowSettings()) {
        String href = navControl.getSettingsLinkActive() ? "href=\"" + Constants.PAGE_PREFERENCES + "\"" : "";
        String className = navControl.getSettingsLinkActive() ? "" : "class=\"inactiveNavLink\"";
%>
              <td <%=className%>>
                <a id="settingsLink" class="navLink" <%=href%>><span><%=wiContext.getString("ScreenTitleSettings")%></span></a>
              </td>
<% } %>

              <td>
<% if (navControl.getShowReconnect() || navControl.getShowDisconnect()) { %>
              <a id="wscLink" class="navLink DropDownMenu" href="javascript:location.replace('../site/<%=Constants.PAGE_LOGOUT%>?<%=SessionToken.QSTR_TOKENNAME%>=<%=SessionToken.get(wiContext)%>')"
                ><span><%=wiContext.getString("LogOff")%></span></a>
<% } else if (navControl.getShowLogout()) { %>
              <a id="logoffLink" class="navLink" href="javascript:location.replace('../site/<%=Constants.PAGE_LOGOUT%>?<%=SessionToken.QSTR_TOKENNAME%>=<%=SessionToken.get(wiContext)%>')"
                ><span><%=wiContext.getString("LogOff")%></span></a>
<% } %>
              </td>
<%
}
%>

<%
  String imageSrc = Include.isCompactLayout(wiContext)? headerControl.getCompactHeaderImage() : headerControl.getHeadingImage();
  imageSrc = ClientInfoUtilities.getImageName(wiContext.getClientInfo(), imageSrc);
  String headingUrl = headerControl.getHeadingHomePage();
  String headingImageTag = "<img id=\"headerLogo\" src=\"" + imageSrc + "\" alt=\"" + wiContext.getString("HeadingImage") + "\" title=\"\">";

  if (headingUrl != null) {
      String homePageTarget = wiContext.getClientInfo().osPocketPC() ? "_top" : "_blank";
%>
              <td>
                <a id="homePageLink" target="<%=homePageTarget%>" href="<%=headingUrl%>"
                   ><%=headingImageTag%></a>
              </td>
<%
  } else {
%>
              <td>
                <%=headingImageTag%>
              </td>
<%
  }
%>
            </tr>
          </table>
        </td>
      </tr>
    </table>
  </div>
</div>

<%-- sub-menu for the workspace control button --%>

<% if (!Include.isCompactLayout(wiContext)) { %>

<div id="Popup_wscLink" class="wiPopup dropDownMenu">

<% if (navControl.getShowReconnect()) {
    String onClickAttrib = "";
    if (Include.isOSEUEMSupport(wiContext)) {
        onClickAttrib = "OnClick=\"addCurrentTimeToHref(this, '" + Constants.PAGE_RECONNECT + "?" + SessionToken.QSTR_TOKENNAME + "=" + SessionToken.get(wiContext) + "', '" + Constants.QSTR_METRIC_RECONNECT_ID + "');\" ";
    }
%>
    <a id="logoutAreaReconnectLink"
        href="<%=Constants.PAGE_RECONNECT%>?<%=SessionToken.QSTR_TOKENNAME%>=<%=SessionToken.get(wiContext)%>" target="<%=Constants.ID_FRAME_RECONNECT + Include.getFrameSuffix(wiContext.getUserEnvironmentAdaptor()) %>"
        <%=onClickAttrib%>
        title="<%= wiContext.getString("TipReconnectButton")%>"
    ><%= wiContext.getString("Reconnect") %></a>
<% } %>

<% if (navControl.getShowDisconnect()) { %>
    <a id="logoutAreaDisconnectLink"
        href="javascript:location.replace('<%=Constants.PAGE_DISCONNECT%>?<%=SessionToken.QSTR_TOKENNAME%>=<%=SessionToken.get(wiContext)%>')"
        title="<%= wiContext.getString("TipDisConnectButton")%>"
    ><%= wiContext.getString("Disconnect") %></a>
<% } %>

</div>

<%
  }
%>
