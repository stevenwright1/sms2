<%
// viewStyle.inc
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// [NFuseVersionAndBuildNumber]
%>
<%
// Start tabPaneHead.inc style
%>

<%
// The tab layout is based on the "sliding doors" approach, as described at http://www.alistapart.com/articles/slidingdoors2/
%>
#tabHeader {
  float:left;
  width:100%;
  background:url("../media/BlackLine.gif") repeat-x bottom;
  line-height:normal;
}

#tabHeader ul {
  margin:0;
  padding:10px 10px 0 0;
  list-style:none;
}

#tabHeader li {
  float: left;
  margin:0;
  padding:0;
}

#tabHeader .leftDoor {
  float: left;
  display:block;
  background:url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/TabLeft.png")%>) no-repeat left top;
  padding: 0 4px 0 4px;
}

#tabHeader .rightDoor {
  float: left;
  display: block;
  background:url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/TabRight.png")%>) no-repeat right top;
  padding: 0;
}

#tabHeader .leftDoor:hover {
  background:url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/TabHoverLeft.png")%>) no-repeat left top;
}

#tabHeader .rightDoor:hover {
  background:url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/TabHoverRight.png")%>) no-repeat right top;
}

#tabHeader .selectedTab .leftDoor {
  background:url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/TabSelectedLeft.png")%>) no-repeat left top;
}

<%
// Bottom padding must be 1px more than for other non-selected tabs.
%>
#tabHeader .selectedTab .rightDoor {
  background:url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/TabSelectedRight.png")%>) no-repeat right top;
  padding-bottom: 1px;
}

#tabHeader .tabText {
  float: left;
<%
// Use slightly less vertical padding for Mozilla-based browsers on Linux to allow the text to fit within the tab without a "step" effect.
%>
<% if (wiContext.getClientInfo().osLinux() && wiContext.getClientInfo().isMozillaBased()) { %>
  padding: 4px 55px 4px 8px;
<% } else { %>
  padding: 7px 55px 7px 8px;
<% } %>
  margin-right: 25px;
}

<%
// Reduce the right padding for image tabs, since the image itself will contribute additional width to the tab.
%>
#tabHeader .imageTab .tabText {
  padding-right: 35px;
  margin-right:0;
}

#tabHeader a {
  text-decoration: none;
}

#tabHeader img {
  float: left;
  margin: 5px 28px 0 0;
  padding: 0;
  border: none;
}

#tabHeader span {
  color: white;
  font-weight: bold;
  white-space: nowrap;
}

#tabFooter {
  clear: both;
  height: 4px;
  background:#3772E3 url(../media/TabBaseLeft.gif) no-repeat top left;
  border-right: 1px solid black;
}

<%
// End tabPaneHead.inc style
%>
.selectedTabContent
{
    clear: left;
    height: 100%;
    overflow: none;
    border: 1px solid #CCC;
    background: white;
}

.hideBox .selectedTabContent {
    border-width: 0px;
    background-color: transparent;
}

.displayTools
{
  background: #E4E4E4 url(../media/TrailBackground.png) repeat-x top left;
  clear: both;
  width: 100%;
  border-bottom: solid 1px #CCC;
  overflow: auto;
  letter-spacing: 0;   <%// cures IE7 zoom bug %>
}

.hideBox .displayTools {
    border-width: 0px;
}

.displayTools a:link,
.displayTools a:visited,
.displayTools a:hover,
#hintArea a:link,
#hintArea a:visited,
#hintArea a:hover
{
    color: #393939;
}

.displayTools table tr td
{
  padding-right: 6px;
}

<%
// Breadcrumb trail area
%>
#displayToolsTable {
    width: 100%;
}

.hideBox #displayToolsTable {
    display: none;
}

#displayToolsViewCell .selectButton {
    float: right;
}

#displayToolsMainCell
{
    vertical-align: middle;
    padding-left: 18px;
    padding-right: 8px;
    color: #393939;
}

#displayToolsRefreshCell {
    width: 34px;
}

<%
// Other breadcrumb trail style
%>

.breadcrumb
{
    white-space: nowrap;
}

.breadcrumbLong
{
    white-space: normal;
}

.lastBreadcrumb
{
    font-weight: bold;
}

.hiddenElement {
    visibility: hidden;
}

#viewButton {
    outline: 0;
}

#refreshLink
{
    background: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/Refresh.png")%>) no-repeat top left;
    text-decoration: none;
    width: 34px;
    height: 26px;
    display: block;
}

a#refreshLink:hover
{
    background: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/RefreshHover.png")%>) no-repeat top left;
}

<%
// Style for the div containing the hint text
%>
#hintArea
{
    clear: both;
    overflow: auto;
    background: #F9F9F9 url(../media/HintBackground.png) repeat-x bottom left;
}

#hintAreaLeft {
    padding-left: 6px;
    background: #F9F9F9 url(../media/HintBackgroundLeft.png) no-repeat bottom left;
}

.hideBox #hintAreaLeft {
    display: none;
}

#hintAreaRight {
    padding-right: 6px;
    background: #F9F9F9 url(../media/HintBackgroundRight.png) no-repeat bottom right;
}

#hintArea p
{
    float: left;
    display: inline;
    padding: 6px 2px 6px 10px;
    vertical-align: middle;
}

#hintArea img
{
  display: inline;
  padding: 0px 10px 0px 6px;
  vertical-align: middle;
}

<%
// Style for the resource name\icon links for all views.
%>
a.iconLink:link,
a.iconLink:visited,
a.folderIconLink:link,
a.folderIconLink:visited,
a.folderIconLink:hover
{
    color: <%=wiContext.getString("AppListLinkColor")%>;
    text-decoration:none;
}

<%
// This is needed as a "trigger" for IE6 to apply the hover style to the <span> element inside the <a> tag.
// Details at http://locusoptimus.com/css-trickery/ie-image-border-styles-on-hover-solution.php
%>
a.iconLink:hover
{
    color: #123456;
}

a.iconLink:hover span,
a.folderIconLink:hover span
{
    color: <%=wiContext.getString("AppListLinkColor")%>;
    text-decoration:underline;
}

.folderIconLink img
{
    float:left;
}

<%
// Style for the Icon View
%>
.iconView,
.groupView
{
    width: 10em;
    height: 8.4em;
    padding: 3px;
    float: left;
    overflow: hidden;
    margin: 5px 0 3px 10px;
<%if(!wiContext.getClientInfo().isFirefox() && !wiContext.getClientInfo().isMozillaBased()) {%>
    line-height: 2.5ex;
<%}%>
}

.iconView .spinner, .groupView .spinner {
    padding-top: 4px;
}

.iconView .iconLink,
.groupView .iconLink
{
    text-align: center;
}

.iconView .folderLink,
.groupView .folderLink
{
    padding-bottom: 19px;
}

.iconView a,
.groupView a,
#listView a,
#tabularView tr a
{
    display: block;
    padding: 4px;
}

.iconView a:hover,
.groupView a:hover,
#listView a:hover,
#tabularView tr a:hover
{
    background: #ECF5FA url("../media/HoverGradient.png") no-repeat top left;
    cursor: pointer;
    display: block;
    border: 1px solid #B3C9E9;
    padding: 3px;
}

#listView a:hover,
#tabularView tr a:hover
{
    background: #DCEDF5 url("../media/HoverGradientList.png") repeat-y top left;
}

.desktopView {
    margin-left: -20px;
}

.iconView a.iconLinkLaunching, .iconView a.iconLinkLaunching:hover,
.groupView a.iconLinkLaunching, .groupView a.iconLinkLaunching:hover,
.desktopView a.iconLinkLaunching, .desktopView a.iconLinkLaunching:hover,
#listView a.iconLinkLaunching, #listView a.iconLinkLaunching:hover,
#treeView a.iconLinkLaunching, #treeView a.iconLinkLaunching:hover,
#tabularView a.iconLinkLaunching, #tabularView a.iconLinkLaunching:hover
{
    cursor: default;
}

.iconView a.folderLink:hover,
.groupView a.folderLink:hover
{
    padding-bottom: 19px;
}

#listView a.folderLink:hover {
    padding-left: 17px;
}

#tabularView a.folderLink:hover {
    padding-left: 18px;
}

.iconView p,
.groupView p
{
    text-align: center;
}

<%
 // Limit the size of the span that displays the name of the application
 // so that it doesn't overflow the box causing focus truncation.
 // The width of the span is set to 9.0em, 1em shorter than the iconView width,
 // to keep the text off the border.
%>
.iconView span,
.groupView span
{
    display: block;
    overflow: hidden;
    width: 9.0em;
    margin: 0 auto;
}

<%
// Style for the list view of applications
%>
#listView
{
    padding: 0 0 10px 10px;
    margin: 8px 0 5px 0;
}

#listView .spinner {
    padding-right: 2px;
}

#listView .folderLink {
    padding-left: 18px;
}

#listView li
{
    float: left;
    width: 26em;
}

#listView li img,
#listView span
{
    vertical-align:middle;
}

#listView span
{
  padding: 0 0 2px 4px;
}

<%
// Style for the details view of applications
%>
#tabularView
{
    clear:both;
    width:100%;
    margin: 0;
    padding: 0 0 12px 0;
}

#tabularView table
{
   width:100%;
   border-collapse: collapse;
}

#tabularView .spinner {
    padding-top: 1px;
    padding-right: 2px;
}

#tabularView .folderLink
{
    padding-left: 19px;
}

#tabularView .leftColumn {
  width: 40%;
  padding: 2px;
  vertical-align: top;
}

#tabularView .descriptionColumn {
  width: auto;
  padding: 2px 20px 2px 0;
}

#tabularView .noLeftBorder {
    padding-left:0px;
}

#tabularView th
{
    padding: 5px 5px 5px 24px;
    text-align: left;
    background-color: #F7F7F7;
}

#tabularView table td
{
   padding: 2px 20px;
   vertical-align: top;
}

#tabularView img
{
    float: left;
}

#tabularView span
{
    padding-left: 3px;
}

#tabularView tr
{
    padding-left: 20px;
}

<%
// Style for the tree view of applications
%>
#treeView
{
    list-style-type: none;
    list-style-image: none;
    list-style-position: outside;
    padding: 14px 14px 18px 50px;
    margin: 0;
}

#treeView li
{
    position: relative;
    margin: 5px 15px 5px 5px;
    padding: 1px 0 0 0;
}

#treeView .spinner {
    left: -35px;
    top: 4px;
}

#treeView li img
{
    position: absolute;
    left: -21px;
    vertical-align: middle;
}

#treeView li ul
{
    display:none;
    margin: 0 0 0 10px;
}

li.rootApps
{
    padding-left:16px;
}

<%
// Style for the Group View
%>

.groupHeader
{
    clear: both;
    padding:20px 8px 0pt 10px;
}

.groupHeaderRule
{
    border-top: 1px solid #b5b5b5;
    margin-right: 5px;
}

.groupHeaderText {
    display: inline;
    background-color: white;
    padding-right: 0.25em;
    position: relative;
    top: -0.75em;
    font-weight: bold;
}

a.folderClose, a.folderOpen,
a.folderClose:visited, a.folderOpen:visited,
a.folderClose:hover, a.folderOpen:hover
{
    color: <%=wiContext.getString("AppListLinkColor")%>;
    text-decoration: none;
}

#treeView li a.folderClose img,
#treeView li a.folderOpen img
{
    left:-36px; <%// 31px icon + 5px padding%>
}
<%
// Style for the search results paging
%>

#tabularView div
{
    clear:both;
    float:left;
    width:90%;
}


#nextLink:hover,#previousLink:hover
{
    text-decoration: underline;
}


<%
// Specifying the css properties for Next link inside the search page
%>
#nextLink {
   padding: 5px;
   float:right;
   text-decoration: none;
   color:#000;
   margin-right:10px;
}

<%
// Specifying the css properties for Previous link inside the search page
%>
#previousLink {
   padding: 5px;
   float:right;
   text-decoration: none;
   color:#000;
   margin-right:10px;
}
<%
// Specifying the css properties for the currently displaying search result count.
%>
#currentResultsNo {
    float:right;
    padding:5px;
}


<% // Delayed launch feedback style %>

.delayedLaunch {
     padding: 15px 5px 15px 45px;
}

.delayedLaunchStatus a:link,
.delayedLaunchStatus a:visited,
.delayedLaunchStatus a:hover {
    color: <%=wiContext.getString("AppListLinkColor")%>;
}

.delayedLaunch img, .delayedLaunch span {
     vertical-align:middle;
}

.delayedLaunchStatus {
    clear: left;
}

.delayedLaunchStatus p {
    padding: 0px;
    margin-top: 3px;
}

.delayedLaunchStatus li {
    display: inline;
}

.delayedLaunchLink {
    border-left: 1px #696969 solid;
    padding-left: 5px;
}

.progressText {
    float: left;
    font-weight: bold;
    padding: 5px 0px;
}

.delayedLaunchInfo {
   margin-top: -33px;
   padding-left: 50px;
}

.desktopScreenContainer {
    margin-left: 30px;
    width: 130px;
}

.restartBoxButtonPane {
    margin-left: 195px;
}

.desktopResourceDirectLaunch {
	padding-top: 160px;
    width: 186px;
    margin: auto;
	font-size: 120%;
	text-decoration: none;
	display: block;
}

.desktopResource a,
.desktopResource a:link,
.desktopResource a:visited,
.desktopResource a:hover,
#commonBox .desktopResourceDirectLaunch a,
#commonBox .desktopResourceDirectLaunch a:link,
#commonBox .desktopResourceDirectLaunch a:visited,
#commonBox .desktopResourceDirectLaunch a:hover
{
    cursor: hand;
    color: <%=wiContext.getString("AppListLinkColor")%>;
}

.directLaunchRestart {
	width: 130px;
	height: 10px;
	vertical-align: bottom;
	text-align: center;
    font-size: 95%;
    margin: 11px 0 6px 40px;
    visibility: hidden;
}

.visible {
    visibility: visible;
}

.restartConfirmationContent {
    padding: 28px 15px 10px 10px;
    font-size: 110%;
}

.directLaunchRestartConfirmation {
    margin:auto;
    width: 200px;
    visibility: hidden;
    background: url("../media/HoverBubble.png") no-repeat scroll 0pt 0pt;
    width: 350px;
    height: 146px;
}


<%
// Style for the desktops view
%>

.desktopResource {
	float: left;
	padding: 20px 0px;
    width: 186px;
	font-size: 120%;
	display: block;
    margin: 0px 15px;
}

.desktopFocus {
	background: url('../media/DesktopRollover.png') no-repeat bottom;
}

.restartLinkHidden,
.restartLinkShowOnFocus,
.restartLinkNotRestartable,
.restartLinkAlwaysShow {
	width: 130px;
	height: 22px;
    margin: 0px 30px;
	text-align: center;
    padding-top: 16px;
}

.restartLinkHidden a,
.restartLinkShowOnFocus a,
.restartLinkNotRestartable a {
    display: none;
}


.restartLinkShowOnFocus a,
.restartLinkAlwaysShow a {
    font-size: 85%;
    text-decoration: underline;
}

.restartLinkNotRestartable,
.restartLinkHidden,
.restartLinkShowOnFocus {
    <%
    // Display shadow only if the browser supports png.
    if ( ClientInfoUtilities.isPNGFullySupported( wiContext.getClientInfo() ) ) { %>
	    background: url('../media/DesktopShadow.png') no-repeat scroll center 18px transparent;
	<% } %>
}

.desktopFocus .restartLinkShowOnFocus {
    background-image: none;
}

.desktopFocus .restartLinkShowOnFocus a {
    display: block;
}

.desktopScreen {
	display: block;
	width: 130px;
    margin: 0;
	height: 88px;
	background: url('../media/DesktopInactive.png') no-repeat bottom;
	text-align: center;
}

.activeDesktop {
    background: url('../media/DesktopActive.png') no-repeat bottom;
}

.restartPane {
    display: none;
	padding-top: 10px;
}

.restartLink {
    font-size: 95%;
}


.desktopFocus .restartPane {
    display: block;
}

.restartPaneHidden {
    display: none;
	padding-top: 10px;
}

.launchPane {
    margin: 0px 40px;
	display: none;
}

.desktopName {
    padding: 0px;
    margin: 3px 0 0 0;
    text-align: center;
   	text-decoration: none;
    overflow: hidden;
}


#commonBox .desktopName a,
#commonBox .desktopName a:link,
#commonBox .desktopName a:hover,
#commonBox .desktopName a:visited {
   	text-decoration: none;
}