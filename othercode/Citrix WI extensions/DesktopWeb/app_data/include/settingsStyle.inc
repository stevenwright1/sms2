<%
// settingsStyle.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
//
// Styles for the Preferences screens
%>

div.AdvancedMode {
    margin-left: 0;
}

.WizardBox .settingsHeader {
    border-top: none;
    padding-left: 0;
}

#preferences h3 {
	font-family: Arial, Helvetica, Sans-Serif;
	color: #888;
	font-size: 170%;
	font-weight: normal;
	margin: 0;
	padding: 5px 0 0;
}

h6.settingsHeader
{
	color: #2C639A;
	font-family: Arial, Helvetica, Sans-Serif;
    margin: 8px 0 0 0;
    padding: 10px 0 2px 25px;
    font-size: 150%;
    font-weight: normal;
    clear: left;
    border-top: 1px solid silver;
    background: white no-repeat 1px 12px;
}

h6.generalSettings 
{
	background-image: url('<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/User.png")%>');
}

h6.passwordSettings 
{
	background-image: url('<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/Password.png")%>');
}

h6.userExperienceSettings 
{
	background-image: url('<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/Desktop.png")%>');
}

h6.devicesSettings 
{
	background-image: url('<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/Plug.png")%>');
}

dl 
{
	width: 100%;
	margin: 0;
}

dd, dt 
{
    float: left;
    margin: 0;
    padding: 0;
}

dt 
{
    clear: left;
    width: <%=wiContext.getString("SettingsLeftColumnWidth")%>;
    padding: 3px 5px 0 0;
}

dt p
{
	padding-left: 25px;
}

dd 
{
    width: <%=wiContext.getString("SettingsRightColumnWidth")%>;
    margin: 0;
    padding: 1px 0 0 20px;
    margin-bottom: <%=wiContext.getString("SettingsGap")%>;
}

dd dt 
{
    width: <%=wiContext.getString("SettingsMiddleColumnWidth")%>;
    padding-top: 0;
}

dd dt.nonLabel 
{
    width: <%=wiContext.getString("SettingsRightColumnWidth")%>;
}

dd dt p
{
	padding-bottom: 0.5em;
	padding-left: 0;
}

dd dd
{
	width: auto;
	margin-bottom: <%=wiContext.getString("SettingsGap")%>;
}

dt ul, dd ul 
{
    list-style-type: none;
    margin: 0;
    padding: 0;
}

dt li, dd li 
{
    margin: 0;
    padding: 0;
}

.settingsButtons
{
    margin: -38px auto 0;
    padding: 0;
    text-align: right;
    width: 50%;
    float: right;
}

.settingsButtons.topBorder
{
	border-top: 1px solid silver;
	margin-top: 0;
	padding-top: 10px;
	width: 100%;
}

.settingsButtons .customButton 
{
	float: right;
	margin: 0 2px;
}

dt .customButton {
	padding-bottom: 35px;
}

.preLoginPane
{
    background: #FFF;
    color: #000;
    width: 104ex;
    margin-top: <%=wiContext.getString("SettingsGap")%>;
    margin-left: 15px;
    padding: 4px 6px;
}

<%
// Bottom margin sets the gap between the options
%>
#<%=Constants.ID_OPTION_WINDOW_SIZE%>,
#Help_WindowSize img
{
    margin-bottom: <%=wiContext.getString("SettingsGap")%>;
}

<%
// Make inline help icons vertically centered with the select
// they are next too
%>
dd dd select,
dd dd input,
dd dd a img,
dd dt input,
dd dt a img,
dt a img,
label {
	vertical-align: middle;
}

<%
// Remove any margin when the inline help icon is next to a label
%>
a.checkboxHelp img
{
    margin: 0;
}

dd dt ul li,
dd dd ul li
{
  margin: 0 0 <%=wiContext.getString("SettingsGap")%> 0;
}

<%
// Ensure all select have the same width
%>
dd select
{
    width: <%=wiContext.getString("SettingsOptionWidth")%>;
}


label#lblWindowSizeCustom,
label#lblWindowSizePercent
{
    float: left;
    width: <%=wiContext.getString("SettingsLabelWidth")%>;
}

#preLoginButtonPane
{
    width: 104ex;
    padding: 6px;
    margin: 15px;
}

#preLoginButtonPane div input
{
    overflow:visible;
    padding-left: 1.25em;
    padding-right: 1.25em;
}

#bandwidthWarning
{
    margin: 8px 0;
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

<%
// IE-specific fixes
if (wiContext.getClientInfo().isIE()) {
%>

dd {
    /* IE ignores margin-bottom. */
    padding-bottom: <%=wiContext.getString("SettingsGap")%>;
}

.topBorder
{
	/* IE ignores margin-top. */
    position: relative;
    top: 10px;
}


.settingsButtons
{
    margin-top: -30px;
}

dl
{
	margin-bottom: <%=wiContext.getString("SettingsGap")%>;
}

dl dl 
{
	margin-bottom: 0;
}

dd dt label,
dd dt p
{
	display: inline-block;
    padding: 2px 3px 3px 0;
}

dd dt label.noPush
{
	display: inline;
	padding-top: 0;
}

dd dt input
{
	padding-left: 5px;
}

<%
}
%>

<%
if ((wiContext.getClientInfo().isFirefox() || wiContext.getClientInfo().isIceweasel())
      && (wiContext.getClientInfo().getBrowserVersionMajor() == 2)) {
%>
#welcome
{
	overflow: visible;
    width: 400px;
    white-space: nowrap;
<%
}
%>
}