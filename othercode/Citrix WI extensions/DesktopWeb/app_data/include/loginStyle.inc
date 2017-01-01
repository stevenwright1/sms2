<%
// loginStyle.inc
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<%
ClientInfo clientInfo = wiContext.getClientInfo();
LoginPageControl viewControl = (LoginPageControl)wiContext.getWebAbstraction().getRequestContextAttribute("viewControl");
%>
<style type="text/css" media="handheld,all">
<!--
body {
<%
// This serves to provide a positioned ancestor to the login button, and prevents a problem where the
// button jumps around unexpectedly when zooming the page in IE7.
// http://www.brunildo.org/test/ie7_relzoom2.html
%>
    position: relative;
    background: #566169 url("../media/HorizonBgBottom.png") repeat-x left 325px;
}

.loginForm td#feedbackCell {
  text-align: left;
}

.horizonPage #feedbackArea {
  margin: 0;
  padding: 0;    
<% // If certificate single sign-on is available, the login fields are widened to accommodate the
   // longer option string ("Pass-through with smart card") in all languages.
  if (viewControl.allowedLogonModes().contains(WIAuthType.CERTIFICATE_SINGLE_SIGN_ON)) { %>
    width: <%=wiContext.getString("LoginFeedbackWideWidth")%>;
<% } else { %>
    width: <%=wiContext.getString("LoginFeedbackWidth")%>;
<% } %>
}

#sysMessage {
  margin: 4px 0 6px 0px;
  padding: 0;
  width: 28em;
}

#sysMessage p {
  padding: 0;
}

<%
// Style inside the login box
%>
#welcome
{
    background: none;
    margin: 0;
    padding: 0;
    height:auto;
    width:auto;
    border: none;
}

#welcome h4
{
    font-size: 170%;
    padding: 6px 0 12px;
    margin: 0;
    margin-top: 5px;
    color: white;
}

.welcomeText
{
    clear: left;
    width: auto;
    text-align: left;
    padding: 5px 0 10px 0;
    margin: 0;
}

#welcomeMessage p {
    padding: 6px 0;
}

<%
// Set left padding to show the login icon
%>
.actionPane
{
    margin:0;
    padding:0;
}

.loginForm
{
    text-align: left;
}

.loginForm td.labelCell {
    text-align: right;
}

.loginForm input,
.loginForm select
{
    margin: 0.2em 0 0.2em 0.2em;
    padding: 2px;
    text-align: left;
}
.loginEntriesDisabled
{
    background-color:#aaaaaa;
}

.loginTreeEntry
{
    color: #000000;
    background-color: #A7A7A7;
<% if (viewControl.allowedLogonModes().contains(WIAuthType.CERTIFICATE_SINGLE_SIGN_ON)) { %>
    width: <%=wiContext.getString("LoginWideOptionWidth")%>;
<% } else { %>
    width: <%=wiContext.getString("LoginOptionWidth")%>;
<% } %>
}

<% if (viewControl.allowedLogonModes().contains(WIAuthType.CERTIFICATE_SINGLE_SIGN_ON)) { %>
.horizonPage .actionPane,
.horizonPage #welcomeMessage {
    width: <%=wiContext.getString("LoginGlowBoxWideWidth")%>;
}

.loginTypeOptionEntries
{
    width: <%=wiContext.getString("LoginTypeWideOptionWidth")%>;
}

.loginEntries
{
    width: <%=wiContext.getString("LoginWideOptionWidth")%>;
}
<% } %>

#accountSS a:link,
#accountSS a:visited,
#accountSS a:hover
{
    text-decoration: underline;
    color: white;
}

#accountSS {
    height:auto;
    padding: 2px 0;
    text-align: right;
}

.loginButtonPane a:link,
.loginButtonPane a:visited,
.loginButtonPane a:hover
{
    text-decoration: none;
}

.loginButton {
    float: right;
}

#loginButtonWrapper
{
    position: relative;
    white-space: nowrap;
    margin: 0px;
    padding: 0px;
    border: 0px;
    line-height: 5ex;
    text-align: center;
    z-index: 1;
    cursor: pointer;
}

#<%=Constants.ID_LOGIN_BTN%>
{
    margin: 10px 0 5px;
}

.carbonBox td.glowBoxMid {
  padding: 7px 30px;
}

<%
// Hide the high contrast button on load by default and show it only if the high contrast
// mode is enabled by the user
%>
#highContrast_LoginButton {
   display: none;
   height: 3ex;
}

.carbonBoxBottom {
    text-align:center;
}

p.feedbackStyleSuccess, p.feedbackStyleInfo, p.feedbackStyleWarning, p.feedbackStyleError
{
    margin:0;
    padding: 4px 0 3px 22px;
    border: none;
    background-color: transparent;
    background-position: 0 4px;
    color: white;
    text-align:left;
}

p.feedbackStyleSuccess a,p.feedbackStyleInfo a,p.feedbackStyleWarning a,p.feedbackStyleError a,
p.feedbackStyleSuccess a:link,p.feedbackStyleInfo a:link,p.feedbackStyleWarning a:link,p.feedbackStyleError a:link,
p.feedbackStyleSuccess a:visited,p.feedbackStyleInfo a:visited,p.feedbackStyleWarning a:visited,p.feedbackStyleError a:visited,
p.feedbackStyleSuccess a:hover,p.feedbackStyleInfo a:hover,p.feedbackStyleWarning a:hover,p.feedbackStyleError a:hover
{
    color: white;
}

-->
</style>
