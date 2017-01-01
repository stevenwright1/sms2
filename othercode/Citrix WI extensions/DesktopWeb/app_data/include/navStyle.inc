<%
// navStyle.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<%
// ===== Begin style for the navigation bar tabs
%>
#navBarWrapper {
  background:#F2F2F2 url("../media/GreyLine.gif") repeat-x bottom;
}

#navBar {
  background:#F2F2F2 url("../media/GreyLine.gif") repeat-x bottom;
  width: <%=wiContext.getString("PageWidth")%>;
  margin: 0 auto;
}

#navBar .leftDoor {
  background: url("../media/TabLeft.png") no-repeat left top;
  display: block;
  float: left;
  padding: 0 0 0 4px;
}

#navBar .rightDoor {
  background:url("../media/TabRight.png") no-repeat right top;
  display: block;
  float: left;
  padding: 0;
}

#navBar .leftDoor:hover {
  background:url("../media/TabHoverLeft.png") no-repeat left top;
}

#navBar .rightDoor:hover {
  background:url("../media/TabHoverRight.png") no-repeat right top;
}

#navBar .selectedTab .leftDoor {
  background:url("../media/TabSelectedLeft.png") no-repeat left top;
}

#navBar .selectedTab .rightDoor {
  background:url("../media/TabSelectedRight.png") no-repeat right top;
  padding-bottom: 1px;
}

#navBar ul {
  margin:0;
  padding:10px 0 0;
  list-style:none;
}

#navBar ul ul {
    padding: 0;
}

#navBar li {
  display:inline;
  margin:0;
  padding:0;
}

#navBar img {
  margin: 5px 5px 0 0;
}

#navBar span {
  white-space: nowrap;
}

#navBar .selectedTab span {
  cursor: default;
}

#navBar li.rightTab {
  float: left;
}

#navBar a {
  float: left;
  margin: 0;
  text-decoration: none;
}

#navBar a.tabText {
  float: left;
  padding: 8px 22px 8px 16px;
}

<%
// ===== End style for the navigation bar tabs
%>

<%
// ===== Begin style for the drop down menus
%>
.dropDownMenu
{
    padding:0; <%// padding in anchor so the hover background works %>
    margin:0;
    display: block;
    <%if(wiContext.getClientInfo().getPlatform() == ClientInfo.OS_WINCE){%>
    width: <%=wiContext.getString("SettingsSubMenuWidth")%>;
    <%} else {%>
    width: auto;
    <%}%>
    height: auto;
    color: black;
}

.dropDownMenu a
{
    display: block;
    margin: 0;
    border: none;
    font-size: <%=wiContext.getString("FontSizeBase")%>;
    padding: 3px 10px;
}

.dropDownMenu a,
.dropDownMenu a:link,
.dropDownMenu a:visited,
.dropDownMenu a:hover,
.dropDownMenu a:focus,
.dropDownMenu a:active
{
    text-decoration: none;
}

.dropDownMenu a:hover,
.dropDownMenu a:focus,
.dropDownMenu a:active
{
    background: #C5E6F3;
}
<%
// ===== End style for the drop down menus
%>

<% // Style the messages button to indicate the highest priority message %>
#navBar span.navButtonError
{
  background: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/Error16.png")%>) center right no-repeat;
  padding: 2px 20px 2px 0;
}

#navBar span.navButtonWarning
{
  background: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/WarningAlert.png")%>) center right no-repeat;
  padding: 2px 20px 2px 0;
}

#navBar span.navButtonInfo
{
  background: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/Info16.png")%>) center right no-repeat;
  padding: 2px 20px 2px 0;
}

#navBar span.navButtonSuccess
{
  background: url(../media/Tick.gif) center right no-repeat;
  padding: 2px 20px 2px 0;
}
