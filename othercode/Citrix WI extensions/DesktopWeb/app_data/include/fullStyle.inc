<%
// fullStyle.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<%
// All fonts are specified relative to the body font. To change the overall
// font size for the site, change the font-size percentage specified here.
%>
body
{
    color: #2C2D31;
    margin: 0;
    padding: 0;
    font-family: <%=wiContext.getString("FontFace")%>;
    font-size: 70%;
    text-align: center;
<%
// position: relative is needed to fix IE 7 zoom issues
// http://www.brunildo.org/test/ie7_relzoom2.html

if(wiContext.getClientInfo().isIE() && (wiContext.getClientInfo().getBrowserVersionMajor() == 7)) {
%>
    position: relative;
<%
}
%>
}

.gradientBackground
{
    background: url("../media/DesktopBackground.png") repeat-x scroll 0 45px transparent;
}

#overallWrapper
{
    text-align: left;
    margin: 0 auto;
}

img, form
{
    padding: 0;
    margin: 0;
    border: none;
}

table
{
    border: 0px;
}

a,
a:link,
a:visited
{
    color: #2C2D31;
    text-decoration:underline;
}

a:hover
{
    color: #2C2D31;
    text-decoration: underline;
}

<%
// Force custom fonts for form components e.g. select, input etc.
%>
a,
input,
label,
p,
select
{
    margin: 0;
    padding: 0;
    font-family: <%=wiContext.getString("FontFace")%>;
    font-size: <%=wiContext.getString("FontSizeBase")%>;
}

input.marginAdjust
{
<% // Correction for IE where Checkbox is shown about 4 pixels to the right of
// DropDown select options lists and labels.
if(wiContext.getClientInfo().isIE()) { %>
    margin-left : -4px;
<% } %>
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
  margin: 1em 0;
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

ul
{
    list-style: none;
    margin: 0;
    padding: 0;
}

ul li
{
    margin: 0;
    padding: 0;
}

.screenReaderText
{
    position: absolute;
    top: 0;
    left: -9999px;
}

.buttonsPane
{
    padding: 20px 0 20px 40px;
}

.buttonsPane div input
{
    display: inline;
    margin: 0 4px 0 0;
}
<%
// Use pairPane for single entry key value pairs
%>
.pairPane
{
  margin: 10px 0 0;
}

.pairPane dt
{
    clear: left;
    float: left;
    padding: 5px 0 0 45px;
    width: <%=wiContext.getString("AuthPairKeyWidth")%>;
}

.pairPane dd
{
    padding: 5px 0;
}

#advancedPageButtons {
    text-align: center;
}

.accountSelfServicePane,
.twoFactorAuthPane
{
   height: 125px;
   margin-top: 16px;
   margin-bottom: 10px;
}

.accountSelfServicePane input,
.accountSelfServicePane label,
.twoFactorAuthPane input,
.twoFactorAuthPane label
{
   padding: 0;
   margin: 2px 0 2px 0;
   font-weight: bold;
   vertical-align:middle;
}

.accountSelfServicePane input,
.twoFactorAuthPane input
{
   margin-left: 40px;
}

.accountSelfServicePane p.taskDesc,
.twoFactorAuthPane p.taskDesc
{
  padding: 0 0 10px 63px;
}

.accountSelfServicePane h5
{
  color: #0C4B8B;
  padding: 3px 0 3px 40px;
  margin-bottom: 4px;
  margin-top: 2px;
}

.accountSelfServicePane h6
{
   padding: 5px 0 3px 20px;
}

.accountSelfServicePane p,
.twoFactorAuthPane p
{
  padding: 0 0 5px 20px;
  margin: 0;
}

.accountSelfServicePane p.questionValue
{
  padding-left: 40px;
  padding-top: 5px;
  font-style: italic;
  font-weight: bold;
}

.accountSelfServicePane li,
.twoFactorAuthPane li
{
  margin-left: 40px;
}

#accountSSForm label,
#twoFactorAuthForm label,
#radiusAuthForm label
{
  float: left;
  margin: 2px 0;
  font-weight: normal;
  width: 20ex;
  vertical-align: top;
}

#radiusAuthForm label
{
  width: 40ex;
}

#accountSSForm input,
#twoFactorAuthForm input
{
  font-weight: normal;
  margin: 2px 0 5px 10px;
  vertical-align: top;
}

<%
// authPane for pages like account self service, change password
%>
.authPane input,
.authPane label
{
  padding: 0;
  margin: 2px 0 2px 0;
  vertical-align: middle;
}

<% // line-height added to ensure label and input are middle vertically-aligned %>
.authPane label
{
  line-height: 175%;
}

.outerContainerDiv
{
    padding-left: 10px;
    padding-top: 3px;
    margin: 16px 0;
    height:110px;
}

<%
// End authPane for pages like account self service, change password
%>

input.Apply_Cancel_OK
{
    width: <%=wiContext.getString("ButtonWidth_Apply_Cancel_OK")%>;
}

input.Cancel_LogIn
{
    width: <%=wiContext.getString("ButtonWidth_Cancel_LogIn")%>;
}

input.Cancel_OK
{
    width: <%=wiContext.getString("ButtonWidth_Cancel_OK")%>;
}

input.Proceed_To_Login
{
    width: <%=wiContext.getString("ButtonWidth_Proceed_To_Login")%>;
}

input.Back {
    width: <%=wiContext.getString("ButtonWidth_Back")%>;
}

input#changepwd
{
    width: <%=wiContext.getString("ButtonWidth_changepwd")%>;
    margin-bottom: 5px;
}

div.appIcon32, img.appIcon32
{
    background-repeat: no-repeat;
    width: <%=Constants.ICON_SIZE_32%>px;
    height: <%=Constants.ICON_SIZE_32%>px;
    margin: 0px;
    margin-left: auto;
    margin-right: auto;
}

div.appIcon16, img.appIcon16
{
    background-repeat: no-repeat;
    width: <%=Constants.ICON_SIZE_16%>px;
    height: <%=Constants.ICON_SIZE_16%>px;
    margin: 0px;
    margin-left: auto;
    margin-right: auto;
}

<%
// Header area
%>
#headerWrapper {
    background: <%=Branding.getBrandingColor(wiContext.getConfiguration())%> url(<%=Branding.getBrandingImageURL(wiContext.getConfiguration())%>) repeat-x bottom left;
    width: 100%; /* needed for the header wrapper to fill the entire width in IE6 on the login page. Alternative fix is to remove "position: relative" from the body (for IE6) */
}

#header {
    background: <%=Branding.getBrandingColor(wiContext.getConfiguration())%> url(<%=Branding.getBrandingImageURL(wiContext.getConfiguration())%>) repeat-x bottom left;
    margin: 0 auto;
    width: <%=wiContext.getString("PageWidth")%>;
    color: <%=Branding.getHeaderFontColor(wiContext.getConfiguration()) %>;
}

#headerLeft {
    width: 100%;
}

#header a {
  color: white;
}

#headerRight {
    margin-left: auto;
}

#headerLogo {
    padding: 11px 0 11px 18px;
}

a.navLink {
    display:block;
    zoom:1; /* needed for IE6 */
    text-align: center;
    text-decoration: none;
    padding: 18px 18px 3px;
    border-right: 1px solid #666;
}

a.navLink:link, a.navLink:visited, a.navLink:hover {
    text-decoration: none;
}

a.navLink span {
    text-decoration: none;
}

#wscLink span {
    padding-right: 12px;
    background: transparent url("../media/DropDownArrow.gif") no-repeat right 5px;
}

#messagesLink {
  background: transparent url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/Messages.png")%>) no-repeat top center;
}

.inactiveNavLink #messagesLink {
  background: transparent url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/MessagesHover.png")%>) no-repeat top center;
}

#messagesLink:hover {
  background: transparent url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/MessagesHover.png")%>) no-repeat top center;
}

#settingsLink {
  background: transparent url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/Settings.png")%>) no-repeat top center;
}

.inactiveNavLink #settingsLink {
  background: transparent url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/SettingsHover.png")%>) no-repeat top center;
}

.inactiveNavLink span {
  cursor: default;
}

#settingsLink:hover {
  background: transparent url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/SettingsHover.png")%>) no-repeat top center;
}

#logoffLink, #wscLink {
  background: transparent url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/Logoff.png")%>) no-repeat top center;
}

#logoffLink:hover, #wscLink:hover {
  background: transparent url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LogoffHover.png")%>) no-repeat top center;
}

.customButton .leftDoor {
  float: left;
  background: transparent url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/ButtonLeft.png")%>) no-repeat top left;
  height: 26px;
  padding: 0px 0px 0px 7px;
  text-decoration: none;
  white-space: nowrap;
  cursor: pointer;
}

.customButton {
  font-size: 11px; /* Fixed font-size to prevent the text overflowing the (fixed size) button when changing the text size in IE */
}

.customButton .leftDoor:hover {
  background: transparent url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/ButtonHoverLeft.png")%>) no-repeat top left;
}

.customButton .leftDoor:hover span {
  background: transparent url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/ButtonHoverRight.png")%>) no-repeat top right;
}

.customButton .rightDoor {
  float: left;
  background: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/ButtonRight.png")%>) no-repeat top right;
  color: #2C2D31;
  height: 20px;
  padding: 6px 15px 0 8px;
}

.customButton a,
.customButton a:link,
.customButton a:visited,
.customButton a:hover
{
    text-decoration: none;
}

.selectButton .rightDoor {
  padding-right: 26px;
  position: relative;
}

.selectButton img {
  border: none;
  position: absolute;
  top: 11px;
  right: 12px;
}

<%
// End Header area
%>

<%
// Welcome title
// Move the welcome DIV down so it is on top of the carbon fibre strip
%>
#welcome
{
    overflow: hidden;
    z-index: 100;
    text-align: left;
}

#welcome h4
{
    margin: 0;
    padding: 5px 0px 3px;
    font-weight: normal;
    font-size: 160%;
}

#welcomeMessage p
{
    padding: 10px 0;
}

#commonBox .welcomeText
{
    margin-top: 10px;
    padding: 2px 0 0 20px;
}

p.firstParagraph
{
    margin-top: 10px;
}

p.secondParagraph
{
    margin-top: 7px;
    padding-bottom: 12px;
}

#searchPane
{
    display: inline;
}

#searchPane form,
#searchPane form div
{
    display: inline;
}

#searchCell
{
    width: 216px;
}

#searchContainer
{
    float: left;
    background: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/Search.png")%>) no-repeat top left;
    width: 200px;
    padding: 5px 0 3px 0;
    font-size: 11px; /* Fixed font-size to prevent the text overflowing the (fixed size) search box when changing the text size in IE */
}

#searchContainer input
{
    float: left;
    background: transparent;
    color: #2C2D31;
    width: 168px;
    border: none;
    font-size: 11px;
    padding: 0 0 0 7px;
    outline: 0; /* prevents orange border in Chrome */
}

#searchContainer a#searchLink {
    float: right;
    background: transparent;
    width: 20px;
    height: 20px;
    outline: none;
}

#searchContainer .searchInactive
{
    color: #777777;
    font-style: italic;
}

<%
// Removed 1px padding from the top and added it to the #searchPane span
// so that the search button image doesnt get cropped.
%>
#searchPane a,
#searchPane a:link,
#searchPane a:visited
{
    margin: 0;
    padding: 3px 4px 4px 4px;
    text-decoration: none;
    background: #3E3E3E url(../media/SearchGradient.gif) repeat-x top left;
    border: solid 1px #646464;
    position: relative;
}

<%
// IE 6 fails to display the search gradient background correctly so use a solid background colour instead
if(wiContext.getClientInfo().isIE() && (wiContext.getClientInfo().getBrowserVersionMajor() < 7)) {
%>
#searchPane a,
#searchPane a:link,
#searchPane a:visited
{
    background: black;
    position: relative;
}

#searchPane a:hover
{
    background: #2D5EF6;
}
<% } %>

#searchPane span
{
    background: transparent url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/SearchMagnify.png")%>) no-repeat center left;
    padding-left: 20px;
    padding-top: 1px;
}

#highContrast_SearchButton
{
   position: relative;
   display: none;
}


#searchPane input,
#searchPane a
{
    vertical-align: middle;
}

<%
// End welcome area
%>

<%
// Pane containing the welcome, feedback and the view holder
// Should not use fixed width on applist page
%>
.mainPane
{
    margin: 0;
    padding: 0;
    left: 0;
    right: 0;
}

<%
// Use less padding at the top because by default the search box is shown and there is quite a gap above the tabs
// Use more padding at the bottom to balance the space at the top
%>
#appMainPane
{
    margin: 10px auto 0;
    width: <%=wiContext.getString("PageWidth")%>;
}

#commonMainPane
{
    width: <%=wiContext.getString("PageWidth")%>;
    margin: 18px auto 0;
}

.hideBox div#commonBoxTop,
.hideBox div#commonBoxFootLeft
{
display: none;
}

div#commonBoxTop
{
    background: black url("../media/TrailBackground.png") repeat-x bottom left;
    border-top: 1px solid #CCCCCC;
    border-left: 1px solid #CCCCCC;
    border-right: 1px solid #CCCCCC;
    height: 25px;
}

div#commonBoxFootLeft
{
    background: transparent url("../media/HintBackgroundLeft.png") no-repeat bottom left;
    padding-left: 6px;
}

div#commonBoxFootRight
{
    background: transparent url("../media/HintBackgroundRight.png") no-repeat bottom right;
    padding-right: 6px;
}

div#commonBoxFoot
{
    background: black url("../media/HintBackground.png") repeat-x bottom left;
    height: 22px;
}

#commonBox
{
    border-top: 1px solid #CCCCCC;
    border-bottom: 1px solid #CCCCCC;
    background-color: white;
    padding: 0;
}

.hideBox #commonBox
{
    border-width: 0px;
    background-color: transparent;
    padding: 0;
}

#commonBox a,
#commonBox a:link,
#commonBox a:visited
{
    color: #3F3F3F;
    text-decoration:underline;
}

#commonBox a:hover
{
    color: #008CBF;
    text-decoration: underline;
}

<%
// Use in direct launch, password expiry, logged out pages
// Padding top comes from welcomeText or other controls
%>
.commonInnerBox
{
    padding: 0 10px 10px 20px;
}

#backButton {
    float: right;
    margin-top: 10px;
}

<%
// Feedback area
%>

.horizonPage #feedbackArea {
    width: 21em;
}

#commonMainPane #feedbackArea
{
    margin: 0 0 18px 0;
}

#appMainPane #feedbackArea
{
    margin-bottom: 10px;
}

#appMainPane #feedbackArea.noFeedback
{
    display: block;
    height: 8px;
}

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
    padding-top: 6px;
    padding-right: 8px;
    padding-bottom: 6px;
    padding-left: 37px;
    font-size: <%=wiContext.getString("FontSizeBase")%>;
<%
// Use the Holly hack in IE6 to avoid intermittently disappearing text (the "peekaboo" bug).
if(wiContext.getClientInfo().isIE() && (wiContext.getClientInfo().getBrowserVersionMajor() < 7)) {
%>
    height: 1%;
<% } %>
}

p.feedbackStyleSuccess a,p.feedbackStyleInfo a,p.feedbackStyleWarning a,p.feedbackStyleError a,
p.feedbackStyleSuccess a:link,p.feedbackStyleInfo a:link,p.feedbackStyleWarning a:link,p.feedbackStyleError a:link,
p.feedbackStyleSuccess a:visited,p.feedbackStyleInfo a:visited,p.feedbackStyleWarning a:visited,p.feedbackStyleError a:visited,
p.feedbackStyleSuccess a:hover,p.feedbackStyleInfo a:hover,p.feedbackStyleWarning a:hover,p.feedbackStyleError a:hover
{
    color: black;
}

p.feedbackStyleSuccess
{
    background: #FAFAFA url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/Tick.png")%>) no-repeat 11px 4px;
    border: solid 1px #85E57F;
}

p.feedbackStyleInfo
{
    background: #FAFAFA url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/Info16.png")%>) no-repeat 11px 4px;
    border: solid 1px #63A7D2;
}

p.feedbackStyleWarning
{
    background: #FAFAFA url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/WarningAlert.png")%>) no-repeat 11px 4px;
    border: solid 1px #F9C887;
}

p.feedbackStyleError
{
    background:  #FAFAFA url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/Error16.png")%>) no-repeat 11px 4px;
    border: solid 1px #B05050;
}

<%
// End  feedback area
%>

<%
// Message center
%>
.messagesScreen li.messageRowSeparator {
    background: #FFF url(../media/FadeLine.gif) no-repeat 10px bottom;
    padding-bottom:10px;
}

<%
// Bottom Pane containing the useful and system message divs
%>

<%
// System message
%>
#sysMessage
{
    width: <%=wiContext.getString("PageWidth")%>;
    margin: 0 auto;
    padding-top: 8px;
}

<%
// The width is set shorter than the background width
%>
#sysMessage p
{
    text-align: left;
    padding: 10px 0;
}

<%
// Add margin for all main controls
%>
#pageContent,
#heightFiller,
#footer
{
    margin: 0;
    padding: 0;
}

<%
  // Footer area
 %>
#footer
{
    text-align: center;
    padding-bottom: 10px;
}

#footer p
{
    padding: 10px 0;
}

#footer img
{
    padding: 0 8px;
    vertical-align: middle;
}

#hdxLogo {
    display: none;
}

.horizonPage #hdxLogo {
    display: inline;
}

.horizonPage #footer p {
    color: #F2F2F2;
}

.loggedOut #footer p {
    color: #2C2D31;
}
<%
  // End footer area
%>

.spacer
{
    clear: both;
}

<%
// Styles for messages appearing in the application list area
%>

p.applistMessageStyleInfo
{
    margin: 5px 20px;
    padding: 5px 0px 65px 25px;
    background: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/Info16.png")%>) no-repeat 5px 5px;
}

<%
// Inline help - a mutli-line tooltip
%>
.wiPopup
{
    position: absolute;
    top: -999px; <%// not visible, but will be verbalized %>
    left: -999px;
    width: 25em;
    z-index: 1000;
    font-size: <%=wiContext.getString("FontSizeSmall")%>;
    padding: 4px 10px;
    border: solid 1px #CCCCCC;
    background: white;
    text-align: left;
}

.WizardBox .wiPopup
{
    background: white url(../media/HoverGradient.gif) repeat-x bottom left;
    border: 1px solid #A0A0A0;
    padding: 5px 10px;
    font-size: 100%;
}

.wiPopup h1
{
    font-size: <%=wiContext.getString("FontSizeLarge")%>;
    padding: 0;
    margin: 0px 0px 5px 0px;
}

.wiPopup ul
{
    list-style-type: disc;
}

.wiPopup li
{
    margin: 0;
}

.wiPopup p
{
    margin: 5px 0 0 0;
    padding: 0;
}

.wiPopup dl
{
    margin: 0;
    padding: 0;
    width: 100%;
}

.wiPopup dt
{
    margin: 5px 0 0 0;
    padding: 0;
    width: 100%;
}

.wiPopup dd
{
	float: none;
    margin: 0;
    padding: 0;
    width: 100%;
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

#Popup_viewButton a {
    padding: 3px 18px;
    white-space: nowrap;
}

<%
// Inline errors
%>

.invalidField
{
    color: #9D0000;
}

<%
// Direct launch
%>
#directLaunchTable td {
    vertical-align: top;
}

.directDelayedLaunchInfo
{
    padding-left: 35px;
}

.noDirectLaunchApp
{
	margin: 5px 20px 0 12px;
	padding: 5px;
}

img.progressIndicator
{
    margin-top: 2px;
    margin-left: 0px;
}

.directDelayedLaunchInfo .progressText
{
    padding-top: 0;
}

.loginForm {
    margin: 0 auto;
}

.loginForm li
{
    list-style-type:none;
    margin-bottom: 6px;
    padding: 0;
}

.loginForm label
{
    padding: 2px 0;
}

.loginTypeOptionEntries
{
    width: <%=wiContext.getString("LoginTypeOptionWidth")%>;
}

.loginEntries
{
    width: <%=wiContext.getString("LoginOptionWidth")%>;
}

<%
// Outer div for the Account Self service page
%>
.outerContainerDiv label
{
   margin-left: 40px;
}

<%
// Further info icons
%>
.loginForm img
{
    vertical-align: middle;
}
<%
// Positioning the help icon for the Account Self service page
%>
.outerContainerDiv img
{
   vertical-align: top;
   padding-top: 3px;
}
<%
// Style for initial pre-requrities pages i.e. no cookies, no javascripts
// Same style as the one for static HTML pages
%>
#mainPane
{
    margin: 0 5px;
}

#mainPane h3
{
    border: solid 1px #CDCDCD;
    padding: 10px 0 10px 40px;
}

#mainPane h3.info
{
    background: #EEEEEE url(../media/Info24.gif) no-repeat center left;
}

#mainPane p
{
    padding-bottom: 10px;
}

<%
// Style for the change password page buttons
%>
.changePasswordButtons
{
    padding: 16px 0 16px 45px;
}

#logoutAreaRestartLink
{
    background: transparent url(../media/Restart.png) no-repeat scroll 6px 2px;
    float: right;
    margin: 24px 100px 0 0;
    padding: 7px 10px 10px 36px;
}

#directLaunchTable
{
    margin-top: 25px;
}

.TrustedZoneWizardLink
{
    margin-top: 15px;
}

p.DirectLaunchAppLink
{
    margin-top: 3px;
}

div.InformationCenterDivLeft
{
    float: left;
    width: 70%;
    padding-bottom: 80px;
    overflow: hidden;
}

.horizonPage {
    background: #566169 url("../media/HorizonBgBottom.png") repeat-x left 325px;
}

#horizonTop {
    width: 100%;
    height: 325px;
    background: #FDFDFD url("../media/HorizonBgTop.png") no-repeat top left;
    text-align: center;
}

#horizonTop img {
    padding-top: 75px;
}

#horizonTagline {
  color: #F2F2F2;
  font-size: 180%;
  font-weight: normal;
  margin: 50px 0 0 0;
  padding-bottom: 10px;
  text-align: center;
}

.horizonPage .mainPane {
    position: relative;
    top: -120px;
    background: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/Devices.png")%>) no-repeat center 30px;
    color: white;
    padding: 0;
    overflow: auto;
}

.horizonPage .actionPane,
.horizonPage #welcomeMessage {
    width: <%=wiContext.getString("LoginGlowBoxWidth")%>;
}

.loggedOut {
	background: #383A3B url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/HorizonBgBottomLoggedOff.png")%>) repeat-x top left;
}

.loggedOut #horizonTop {
	background: #515151 url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/HorizonBgTopLoggedOff.png")%>) no-repeat top left;
}

.loggedOut .glowBoxMid {
	height: 17.5em;
	width: 28em;
}

.loggedOut .mainPane {
	background: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/DevicesLoggedOff.png")%>) no-repeat center 30px;
}

.loggedOut #feedbackArea {
	padding-bottom: 1em;
}

.loggedOut p {
	text-align: center;
}

.loggedOut .customButton {
	margin: 1em 0 0;
	zoom: 1;
}

.loggedOut .glowBoxMid {
    vertical-align: top;
}

.loggedOut .actionPane {
	width: 100%;
}

.loggedOut #sysMessage {
    margin: 4px 0 6px 0px;
    padding: 0;
    width: 28em;
}

.loggedOut #feedbackArea {
	width: 28em;
}

.loggedOut #horizonTagline {
    color: #515151;
}

.loggedOut td.glowBoxFooterLeft {
  background-image:url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LoginPaneFooterLeftLoggedOff.png")%>);
}

.loggedOut td.glowBoxFooterMid {
  background-image:url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LoginPaneFooterMidLoggedOff.png")%>);
}

.loggedOut td.glowBoxFooterRight {
  background-image:url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LoginPaneFooterRightLoggedOff.png")%>);
}

.loggedOut #feedbackArea
{
	margin: 3em 0 0;
}

.loggedOut #feedbackArea p
{
    padding: 4px 0 3px 22px;
    border: none;
    background-color: transparent;
    background-position: 0 4px;
    color: white;
    text-align:left;
}

.loggedOut p.feedbackStyleSuccess a,
.loggedOut p.feedbackStyleInfo a,
.loggedOut p.feedbackStyleWarning a,
.loggedOut p.feedbackStyleError a,
.loggedOut p.feedbackStyleInfo a:link,
.loggedOut p.feedbackStyleWarning a:link,
.loggedOut p.feedbackStyleError a:link,
.loggedOut p.feedbackStyleSuccess a:visited,
.loggedOut p.feedbackStyleInfo a:visited,
.loggedOut p.feedbackStyleWarning a:visited,
.loggedOut p.feedbackStyleError a:visited,
.loggedOut p.feedbackStyleSuccess a:hover,
.loggedOut p.feedbackStyleInfo a:hover,
.loggedOut p.feedbackStyleWarning a:hover,
.loggedOut p.feedbackStyleError a:hover
{
    color: white;
}

.loggedOut #feedbackArea.noFeedback {
	display: block;
}

table.carbonBox {
    width: 100%;
}

#returnToLogOn {
    margin: 0 auto;
}

<%
// Style the box to get a glow and/or blue line.
// This is used by both the login page and the lightbox.
%>
table.glowBox {
    cursor: default;
}

td.glowBoxLeft {
  width:20px;
}
td.glowBoxRight {
  width:20px;
}
td.glowBoxTop {
  height:20px;
}
td.glowBoxFooter {
  height:51px;
}
td.glowBoxMid {
  background: #73808C;
  padding: 7px 30px;
}

td.glowBoxTopLeft {
  background-position:bottom right;
  background-repeat:no-repeat;
  background-image:url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LoginPaneTopLeftBorderGlow.png")%>);
}
td.glowBoxTopMid .leftGradient {
  background-position:top left;
  background-repeat:no-repeat;
  background-image:url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LoginPaneTopLeftGradient.png")%>);
  height: 20px;
}
td.glowBoxTopMid .rightGradient {
  background-position:top right;
  background-repeat:no-repeat;
  background-image:url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LoginPaneTopRightGradient.png")%>);
  height: 20px;
  margin-left: 96px;
}
td.glowBoxTopMid .centerGradient {
  background-position:top left;
  background-repeat:repeat-x;
  background-image:url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LoginPaneTopMidBorderGlow.png")%>);
  height: 20px;
  margin-right: 96px;
}
td.glowBoxTopRight {
  background-position:bottom left;
  background-repeat:no-repeat;
  background-image:url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LoginPaneTopRightBorderGlow.png")%>);
}
td.glowBoxMidLeft {
  background-position:center left;
  background-repeat:repeat-y;
  background-image:url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LoginPaneCenterLeftBorderGlow.png")%>);
}
td.glowBoxMidRight {
  background-position:center right;
  background-repeat:repeat-y;
  background-image:url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LoginPaneCenterRightBorderGlow.png")%>);
}
td.glowBoxFooterLeft {
  background-position:top left;
  background-repeat:no-repeat;
  background-image:url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LoginPaneFooterLeftBorderGlow.png")%>);
}
td.glowBoxFooterMid {
  background-position:top center;
  background-repeat:repeat-x;
  background-image:url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LoginPaneFooterMidBorderGlow.png")%>);
}
td.glowBoxFooterRight {
  background-position:top right;
  background-repeat:no-repeat;
  background-image:url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LoginPaneFooterRightBorderGlow.png")%>);
}

.delayedImageNone {
    height: 90px;
    background: url("../media/Transparent16.gif") no-repeat scroll center center transparent;
}

.delayedImageSpinner {
    height: 90px;
    background: url("../media/Loader.gif") no-repeat scroll center center transparent;
}

.delayedImagePlay {
    height: 90px;
    background: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/DesktopReady.png")%>) no-repeat scroll center center transparent;
}

.restartConfirmationIcon {
    height: 70px;
    width: 50px;
    float: left;
    padding-left: 10px;
    background: url(../media/AlertIcon.png) no-repeat top center;
}

.restartConfirmationText {
    height: 70px;
    width: 260px;
    float: left;
}

#directLaunchRestart {
    padding-top: 13px;
}