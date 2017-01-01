<%
// lowStyle.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<%
// -------------------------------------------------------------------
// General style
// -------------------------------------------------------------------
%>

body
{
    color: black;
    margin: 0;
    padding: 0;
    font-family: <%=wiContext.getString("FontFace")%>;
    font-size: <%=wiContext.getString("FontSizeXSmall")%>;
}

h4 <% // Heading for normal text e.g. in Welcome area %>
{
    font-size: <%=wiContext.getString("FontSizeBase")%>;
    font-weight: bold;
    padding: 0;
    margin: 0;
}

<%
// Heading for account self service screens
%>
h5
{
  margin: 0;
  padding: 0;
  font-size: <%=wiContext.getString("FontSizeLarge")%>;
}

<%
// Heading for Preferences screens
%>
h6
{
  margin: 0;
  padding: 0;
  font-size: <%=wiContext.getString("FontSizeBase")%>;
}

<% // stop browser doing its own thing for these elements %>
a,
input,
label,
p,
select,
ul,
dd,
dt,
dl
{
    margin: 0;
    padding: 0;
    font-family: <%=wiContext.getString("FontFace")%>;
    font-size: <%=wiContext.getString("FontSizeBase")%>;
}

form
{
    margin: 0;
    padding: 0;
}

div
{
    margin: 0;
    padding: 0;
    border: 0;
}

ul
{
    list-style-type: none;
}

a,
a:link,
a:visited
{
    color: #404040;
    text-decoration:underline;
}

a:hover
{
    color: #008CBF;
    text-decoration: underline;
}

.screenReaderText
{
    position: absolute;
    top: 0;
    left: -9999px;
}

<%
// In general don't show any inline help
//
%>
.wiPopup,
.inlineHelpLink
{
    display: none; <% // don't want screen-readers to see them either %>
}

<%
// Show inline help for the wizard advanced page, even in low-graphics
//
%>
.WizardBox .wiPopup,
.WizardBox .inlineHelpLink
{
    display: inline;
}

.WizardBox .wiPopup
{
    position: absolute;
    top: -999px; <%// not visible, but will be verbalized %>
    left: -999px;
    width: 25em;
    z-index: 1000;
    padding: 5px 10px;
    border: 1px solid #A0A0A0;
    background: white url(../media/HoverGradient.gif) repeat-x bottom left;
    color: black;
}

.WizardBox .wiPopup p
{
    margin: 5px 0;
    padding: 0;
}

<%
// Some browsers get the stacking order wrong. We place
// an iframe exactly behind the tooltip to fix this.
%>
.hiddenFrameLayer
{
    position: absolute;
    z-index: 990;
    border: none;
}

<%
// Spacer divs are used to clear sections of HTML containing floats
%>
.spacer
{
    clear: both;
}

<%
// Use in direct launch, password expiry, logged out pages
%>
.commonInnerBox
{
    padding: <%=wiContext.getString("LGVertBigGap")%>;
    padding-top: 0;
}

p.secondParagraph
{
    margin-top: <%=wiContext.getString("LGVertSmallGap")%>;
}

<%
// -------------------------------------------------------------------
// Header
// -------------------------------------------------------------------
%>

#header {
    background: #7B8994;
}

#header img {
    padding: 5px 12px 5px 0;
    margin: 0;
}

#headerLeft {
    width: 100%;
}

#headerRight {
    margin-left: auto;
    float: right; <% // required for correct display in Pocket IE %>
}

<%
// -------------------------------------------------------------------
// Main panes - contains everything below the header
// -------------------------------------------------------------------
%>

.mainPane
{
  padding: <%=wiContext.getString("LGVertBigGap")%> <%=wiContext.getString("LGHorizMargin")%> <%=wiContext.getString("LGVertBigGap")%> <%=wiContext.getString("LGHorizMargin")%>;
  margin: 0;
  border-top: 1px solid #CDCDCD;
}

<%
// -------------------------------------------------------------------
// Screen title
// -------------------------------------------------------------------
%>
#welcome h4
{
  padding: 3px 0;
  margin: 0;
  font-size: 180%;
  font-weight: normal;
}

#welcome h5
{
  font-size: <%=wiContext.getString("FontSizeBase")%>;
  padding: 0;
  margin: <%=wiContext.getString("LGVertSmallGap")%> 0 <%=wiContext.getString("LGVertSmallGap")%> 0;
}

<%
// -------------------------------------------------------------------
// Action pane
//
// Contains the page content (excluding the header, title, navbar,
// quick links, logout links but including the feedback area)
// -------------------------------------------------------------------
%>

.actionPane
{
  padding: 0;
  margin: <%=wiContext.getString("LGVertBigGap")%> 0 0 0;
}

<%
// -------------------------------------------------------------------
// Feedback area
// -------------------------------------------------------------------
%>

#feedbackArea.noFeedback
{
    display: none;
}

p.feedbackStyleSuccess,
p.feedbackStyleInfo,
p.feedbackStyleWarning,
p.feedbackStyleError
{
    color: black;
    margin: 0 0 <%=wiContext.getString("LGVertBigGap")%> 0;
    padding: 4px 4px 4px 24px;
    min-height: 16px;
    background-color: white;
}

p.feedbackStyleSuccess
{
    background: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/Tick.png")%>) no-repeat 5px 4px;
    border: solid 1px #85E57F;
}

p.feedbackStyleInfo
{
    background: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/Info16.png")%>) no-repeat 5px 4px;
    border: solid 1px #63A7D2;
}

p.feedbackStyleWarning
{
    background: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/WarningAlert.png")%>) no-repeat 5px 4px;
    border: solid 1px #F9C887;
}

p.feedbackStyleError
{
    background: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/Error16.png")%>) no-repeat 5px 4px;
    border: solid 1px #B05050;
}

<%
// -------------------------------------------------------------------
// Login
// -------------------------------------------------------------------
%>

<%
// Stack the controls and labels vertically
%>
#accountSSForm li,
#twoFactorAuthForm li,
#radiusAuthForm li
{
    float: left;
    clear: left;
}

#accountSSForm label,
#accountSSForm input,
#twoFactorAuthForm label,
#twoFactorAuthForm input,
#radiusAuthForm label,
#radiusAuthForm input
{
    float: left;
    clear: left;
}

.buttonsPane {
    clear: both;
}

.buttonsPane div {
    clear:both;
    float:left;
    margin-top: 4px;
}

<%
// Each label/input box pair is contained in a li
%>
#twoFactorAuthForm li
{
    margin: <%=wiContext.getString("LGVertSmallGap")%> 0 <%=wiContext.getString("LGVertSmallGap")%> 0;
}

.loginButtonPane input
{
    width: <%=wiContext.getString("LGButtonWidth_Login")%>;
    margin: <%=wiContext.getString("LGVertBigGap")%> 0 0 0; <% // insert extra vertical space %>
}

.loginEntriesDisabled
{
    background-color: silver;
}
<%
// -------------------------------------------------------------------
// Authentication pages - set PIN, etc.
// -------------------------------------------------------------------
%>
.authPane,
.accountSelfServicePane,
.twoFactorAuthPane
{
  padding: 5px 0 0 0;
}

.authPane p,
.accountSelfServicePane p,
.twoFactorAuthPane p
{
  padding: 1px 0 5px 0;
  margin: 0;
}



.authPane p.taskDesc,
.accountSelfServicePane p.taskDesc,
.twoFactorAuthPane p.taskDesc
{
  padding: 0 0 10px 23px;
}

.accountSelfServicePane input,
.accountSelfServicePane label,
.twoFactorAuthPane input,
.twoFactorAuthPane label
{
   padding: 0;
   font-weight: bold;
   vertical-align:middle;
}

#accountSSForm label,
#accountSSForm input,
#twoFactorAuthForm label,
#twoFactorAuthForm input,
#radiusAuthForm label,
#radiusAuthForm input
{
  font-weight: normal;
}

.accountSelfServicePane h5,
.accountSelfServicePane h6
{
  padding: 5px 0 10px 0;
}

<%
// -------------------------------------------------------------------
// Application list
// -------------------------------------------------------------------
%>

<%
// Tab navigation (the tabs are actually a dropdown list in low graphics mode)
%>
#tabHeader
{
    margin: <%=wiContext.getString("LGVertBigGap")%> 0 8px 0;
    padding: <%=wiContext.getString("LGVertBigGap")%> 0 <%=wiContext.getString("LGVertBigGap")%> 0;
    border-bottom: 1px solid #CDCDCD;
}

#tabHeader select
{
    margin-bottom: <%=wiContext.getString("LGVertBigGap")%>;
}

.selectedTab {
    display: none;
}

.otherTab {
    padding: 3px 0;
}

<%
// Folder navigation
%>

#lgApplistHeading
{
    padding: 4px 0px;
    margin: 0;
}

<%
// In our app links, make sure the image is never underlined, and
// the text is always underlined.
%>
a.iconLink,
a.iconLink:link,
a.iconLink:visited,
a.iconLink:hover
{
    text-decoration:none;
}

a.iconLinkLaunching
{
    cursor: default;
}

a.iconLink span
{
    text-decoration:underline;
    padding-left: 2px;
}

.iconLink img
{
    border: none;
    vertical-align: middle;
}

<%
// In icon view, make the app link expand so the whole block is clickable.
%>
a.expandedLink
{
    display: block;
}

#breadcrumbTrail
{
    padding-bottom: <%=wiContext.getString("LGVertBigGap")%>;
}

.breadcrumb
{
    white-space: nowrap;
}

.lastBreadcrumb
{
    font-weight: bold;
}

#iconsView div
{
    width: 7.5em;
    height: 6.3em;
    padding: <%=wiContext.getString("LGVertSmallGap")%>;
    word-wrap: break-word;
    overflow: hidden;
}

<%
// Icons are arranged based the viewport width
%>
#iconsView div.col1
{
    float: left;
}

#iconsView p
{
    text-align: center;
}

#iconsView img
{
    border: none;
}

#iconsView .spinner {
    padding: 2px 0;
}

#iconsView .folderLink {
    padding-bottom: 15px;
}

#noSearchResults
{
    padding: 4px 0px;
}

#listView li
{
    padding: 4px 0;
}

#listView .spinner {
    padding: 0 2px 0 3px;
}

#listView .folderLink {
    padding-left: 16px;
}

#searchResultsView dt
{
    padding: 4px 0 0 0;
}

#searchResultsView dd
{
    padding: 0 0 4px 32px;
}

#searchPane
{
    margin: <%=wiContext.getString("LGVertBigGap")%> 0 0;
    padding: <%=wiContext.getString("LGVertBigGap")%> 0;
    border-top: 1px solid #CDCDCD;
}

#searchPane li,
#searchPane form
{
    padding: <%=wiContext.getString("LGVertSmallGap")%> 0 <%=wiContext.getString("LGVertSmallGap")%> 0;
    margin: 0;
}

.lowGraphicsSearchArea input
{
    vertical-align:middle;
}

.searchNextPrevious p
{
    padding: 2px 0;
}

.desktopsTabHeaderDescription {
    font-weight: bold;
    margin-left: 3px;
}

a.desktopsTabRestartLink,
a.desktopsTabRestartLink:link,
a.desktopsTabRestartLink:visited {
    text-decoration: none;
}

.desktopsTabRestartDescription {
    text-decoration: underline;
    font-weight: bold;
}

#changeView li {
    margin: 3px 0;
    padding: 0;
}

<%
// -------------------------------------------------------------------
// Settings pages
// -------------------------------------------------------------------
%>

<%
// This is the sub-title, underneath the page title
%>
h4
{
  margin: 0 0 0 <%=wiContext.getString("LGHorizSettingsMargin")%>;
  padding: 0;
  font-size: <%=wiContext.getString("FontSizeBase")%>;
}

dd dt,
dd dt li
{
  margin: 7px 0 0;
  width: 100%;
}

h6.settingsHeader
{
  padding: 7px 0 3px 0;
  margin: 15px 0 0;
  font-size: <%=wiContext.getString("FontSizeBase")%>;
  border-top: 1px solid silver;
}

<%
// Don't show the descriptive text in the settings key
%>
dt p,
#preferences h3
{
  display: none;
}

<%
// But do show the control labels
%>
dl dl dt p
{
  display: block;
}

dd dd
{
  width: 100%;
}

dd dd select
{
  margin: <%=wiContext.getString("LGVertSmallGap")%> 0 <%=wiContext.getString("LGVertSmallGap")%> 0;
}

dd dd li
{
  margin: <%=wiContext.getString("LGVertSmallGap")%> 0 <%=wiContext.getString("LGVertSmallGap")%> 0;
  padding: 0;
}

.settingsButtons,
.buttonsPane
{
    margin: <%=wiContext.getString("LGVertBigGap")%> 0 0 0;
}

.settingsButtons input,
.buttonsPane input,
.changePasswordButtons input
{
    margin: <%=wiContext.getString("LGVertSmallGap")%> 0 <%=wiContext.getString("LGVertSmallGap")%> 0;
    width: <%=wiContext.getString("LGButtonWidth_Apply_Cancel_OK")%>;
}

.settingsButtons.topBorder
{
	border-top: 1px solid silver;
	margin-top: 15px;
	padding: 8px 0 5px;
}

<%
// Don't specify a fixed width for the pre-login button as the text for this button's label is customizable.
%>
#preLoginButtonPane input {
    padding: 0px 10px;
    width: auto;
}

.pairPane dd
{
    margin: <%=wiContext.getString("LGVertSmallGap")%> 0 0 0;
}

.pairPane dd
{
    margin: 0 0 <%=wiContext.getString("LGVertSmallGap")%> 0;
}

<%
// -------------------------------------------------------------------
// Messages screen
// -------------------------------------------------------------------
%>

#messagesContent ul
{
    margin-bottom: 20px;
    padding-bottom: 10px;
}

.messagesScreen h2
{
    margin: 10px 0 0 0;
    padding: 5px 15px;
    border-top: solid 1px #CDCDCD;
    border-bottom: solid 1px #CDCDCD;
    background-color: #ECECEC;
    font-size: <%=wiContext.getString("FontSizeBase")%>;
    font-weight: normal;
}

.messagesScreen h3
{
    padding: 2px 0 2px 10px;
    margin: 2px 0;
    font-size: <%=wiContext.getString("FontSizeLarge")%>;
    background-repeat: no-repeat;
    background-position: left center;
    color: black;
}

.messagesScreen #errorMessages
{
    background-image: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/Error16.png")%>);
    background-position: center left;
    padding-left: 25px;
}

.messagesScreen #warningMessages
{
    background-image: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/WarningAlert.png")%>);
    background-position: center left;
    padding-left: 25px;
}

.messagesScreen #infoMessages
{
    background-image: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/Info16.png")%>);
    background-position: center left;
    padding-left: 25px;
}

.messagesScreen li
{
    padding: 2px 20px 2px 20px;
    margin: 0px;
}

.messagesScreen li.messageRowSeparator {
    border-bottom: solid 1px #CDCDCD;
    padding-bottom:4px;
    padding-left:0px;
    margin: 0px 20px 0px 20px;
}

.headerMessage
{
   background-color: #efefef;
   padding: 0;
   margin: 0;
}
<%
// -------------------------------------------------------------------
// Nav bar
// -------------------------------------------------------------------
%>

#navBar
{
  margin: <%=wiContext.getString("LGVertBigGap")%> 0 0 0;
  padding: <%=wiContext.getString("LGVertBigGap")%> 0 <%=wiContext.getString("LGVertBigGap")%> 0;
  border-top: 1px solid #CDCDCD;
}

#navBar li,
.compactOptions li
{
  padding: <%=wiContext.getString("LGVertSmallGap")%> 0 <%=wiContext.getString("LGVertSmallGap")%> 0;
}

<%
// -------------------------------------------------------------------
// Inline errors
// -------------------------------------------------------------------
%>

.invalidField
{
    color: red;
}

<%
// Account self service - Label class for the wrong answer to the security questions.
%>
.lblAnswerError
{
color: red;
}

input#changepwd
{
    width: <%=wiContext.getString("ButtonWidth_changepwd")%>;
}

input#lowGraphicsSearchButton
{
    width: <%=wiContext.getString("LGButtonWidth_Search")%>;
}

input#runAutoWizard,
input#runAdvancedWizard,
input#addToTrustedSites
{
    margin-top: 5px;
}

input#runAutoWizard
{
    width: <%=wiContext.getString("ButtonWidth_runAutoWizard")%>;
}

input#runAdvancedWizard
{
    width: <%=wiContext.getString("ButtonWidth_runAdvancedWizard")%>;
}

input#addToTrustedSites
{
    width: <%=wiContext.getString("ButtonWidth_addToTrustedSites")%>;
}

#logoutLink input, input#logoutButton {
    width: <%=wiContext.getString("LGButtonWidth_Logoff")%>;
}

input#backButton {
    width: <%=wiContext.getString("LGButtonWidth_Back")%>;
}

#wscOptions {
	border-top: 1px solid #CCC;
	margin-top: 3px;
}

#wscLinks li,
#logoutArea form {
    margin: 8px 0;
}

#returnToLogOn {
    padding: 3px 0;
}

<%
// Button class for the Procede to Login button on account self service page.
%>
input.Proceed_To_Login
{
    width: <%=wiContext.getString("ButtonWidth_Proceed_To_Login")%>;
}

<%
// We can't use "display: none" because some browsers eg. FF won't
// get the contents of the frame.
%>
iframe.HiddenIframe, div.HiddenDiv
{
    width: 0px;
    height: 0px;
    position: fixed;
    left:-100px;
    top: -100px;
    visibility: hidden;
}

<% // Delayed launch feedback style %>

.delayedLaunch {
    padding:5px 10px 5px 10px;
}

.delayedLaunch img {
    float: left;
    padding-right: 7px;
}

.delayedLaunchLink {
    border-left: 1px #696969 solid;
    padding-left: 5px;
}

.delayedLaunchInfo {
   margin-top: -35px;
   padding: 2px 5px 5px 50px;
}

img.progressIndicator {
    margin-top: 2px;
}

.desktopView {
    margin:5px 0 20px 0;
}

.desktopView .spinner {
    padding: 0 4px 0 2px;
}

.desktopColumn {
    padding-top: 2px;
}

.folderColumn {
    padding-left: 16px;
}

.progressText {
    font-weight:bold;
    padding: 5px 0px;
}

.groupHeader {
    clear: both;
    padding:8px 8px 5px 0pt;
}

.groupHeaderText {
    display: inline;
    font-weight: bold;
}

<%
// -------------------------------------------------------------------
// Client Detection Wizard
// -------------------------------------------------------------------
%>

h1#ClientDetectOverallHeading
{
    display: none;
}

<% // Do not put any restrictions on the width and keep everything to the left%>

body
{
    min-width: 0px;
}

table#clientStatus,
td.clientColumn
{
    width: auto;
}

div.MainDiv
{
    width: auto;
    padding: 5px 0;
    margin: 0;
}

div.SecurityImplications
{
    margin-left:0;
    border-top: 1px solid #CCC;
}

#HelpDiv
{
    width: 90%;
    color: #000;
    margin: 0;
    padding: 5px 0 0 5px;
}

div.SectionOpen
{
    background: none;
}

#HelpLink
{
    padding: 2px 0 2px 25px;
}

.TrustedZoneWizardLink
{
    padding: 4px 0 5px 0;
}

.restartColumn
{
    text-align: right;
    margin-left: 3px;
}
