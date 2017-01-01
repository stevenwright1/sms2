<%
// styleWizard.inc
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<style type="text/css" media="handheld,all">
<!--

.horizonPage .actionPane {
    text-align: center;
}

p#footer
{
    clear: both;
    border-top: <%=brandingColor%> solid 1px;
    margin-bottom: 5px;
    font-size: <%=fontSizeXSmall%>;
}

div.spacer
{
    clear: both;
    line-height: 0;
    height: 0;
    margin: 0;
    padding: 0;
}

<% if (!sClientInfo.osSymbian()) { %>

a:link,
a:visited
{
    color: #000088;
}

a:hover
{
    color: #AAAACC;
}

<% } %>

label
{
    font-size: <%=fontSizeBase%>;
}

table
{
    border: 0px;
}

input
{
    font-family: <%=fontFace%>;
    font-size: <%=fontSizeBase%>;
}

<%
// Force custom fonts for form components e.g. select, input etc.
%>
select
{
    font-family: <%=fontFace%>;
    font-size: <%=fontSizeBase%>;
    margin-top: 5px; <% // space away from label %>
    margin-bottom: 5px;
}

body
{
    color: black;
    background-color: white;
    margin: 0px;
    padding: 0px;
    font-family: <%=fontFace%>;
    font-size: 70%;
    margin-left: 1em;
    margin-right: 1em;
    min-width: 600px;
}

img
{
    border: none;
    padding:0px 8px 5px 0px;
}

img.HelpImage
{
    text-align: center;
    padding: 5px 0px 5px 0px;
    margin-left: 25px;
}

img#clientIcon
{
    padding: 5px;
}

legend, input, a, div td
{
    font-size: <%=fontSizeBase%>;
}

div.MainDiv table
{
    color: #3F3F3F;
    background-color: #FFFFFF;
}

table#clientStatus a
{
    color: #3F3F3F;
}

<%
// <td> and <a> tags are already sized to 80%, so when <a> is nested within <td>
// it should be 100% to avoid further shrinkage.
%>
td a
{
    font-size: 100%;
}

tr.headingRow th
{
    background-color: #DCDCDC;
    border-bottom: solid 1px #A0A0A0;
    font-weight: bold;
}

table.clientStatus
{
    padding: 0;
    margin: 0;
    float: left;
    width: <%=wizardContext.getString("AdvanceModeWidth")%>;
}

.clientStatus td
{
    padding-top: 3px;
    padding-bottom: 3px;
    padding-left: 3px;
}

#ClientDropDownLabel
{
    padding-top: 6px;
}

#advancedPageButtons {
    padding: 20px 0 10px;
    clear: both;
}

.linkColumn
{
    text-align: right;
}

.inlineHelpLink
{
    padding-left: 5px;
}

.horizonPage .inlineHelpLink
{
    padding-left: 0;
}

hr {
    height: 1px;
}

<% // Main client detection heading %>
h1
{
    background-color: #DFDFDF;
    color: #000000;
    padding: 5px;
    font-size: <%=fontSizeXLarge%>;
    display: block;
}

<% // Page heading %>
h2
{
    font-size: <%=fontSizeLarge%>;
    font-weight: bold;
    margin: 0px 0px 5px 0px;
}

<% // Help heading %>
h3
{
    font-size: <%=fontSizeLarge%>;
    font-weight: normal;
    margin: 0;
    padding: 0;
}

h4
{
    font-size: <%=fontSizeBase%>;
    font-weight: bold;
}

<%
// We can't use "display: none" because some browsers eg. FF won't
// get the contents of the frame.
%>
.HiddenControl
{
    width: 0px;
    height: 0px;
    position: fixed;
    left:-100px;
    top: -100px;
    visibility: hidden;
}

<%
// IE6 doubles the left and right margins due to float so need to reduce by half
if (sClientInfo.isIE() && sClientInfo.getBrowserVersionMajor() == 6) { %>
div.MainDiv
{
    margin: 20px 10px 0 10px;
}
<% } else { %>
div.MainDiv
{
    margin: 20px 20px 0 20px;
}
<% } %>

.horizonPage div.MainDiv {
    margin: 5px 0px 10px;
    float: none;
}

div.WizardBox div.MainDiv
{
    width: 85%
}

div.MainDiv h2
{
    font-size: 160%;
    font-weight: normal;
}

.horizonPage div.MainDiv h2 {
    margin-bottom: 15px;
}

<%
// Wizard does not show the second heading on the start page, but include it so it provides an option for WI
%>
div.MainDiv h2#StartHeading
{
    display: none;
}

div.MainDiv p {
    margin: 10px 0 0 0;
}

.horizonPage div.MainDiv p {
    margin: 0;
}

.horizonPage .actionPane a,
.horizonPage .actionPane a:link,
.horizonPage .actionPane a:visited,
.horizonPage .actionPane a:hover
{
    color: white;
}

.horizonPage div.MainDiv #JavaClientLink
{
    margin-top: 12px;
}

p.CaptionInfo
{
    padding: 2px 0;
}

<%
// Advanced page
%>
#clientSelection
{
    width: 25%;
}

div.WizardBox div.AdvancedMode
{
    width: auto;
}

div.SecurityImplications
{
    margin: 20px 20px 0 20px;
}

.horizonPage div.SecurityImplications {
    display: inline;
    float: none;
    margin: 0;
    padding-right: 7px;
}

#ShortSecurityContent
{
    background: url(<%=ClientInfoUtilities.getImageName(sClientInfo, "media/" + WizardConstants.ICON_SECURITY_SHIELD)%>) left center no-repeat;
    padding: 5px 0 7px 23px;
}

.horizonPage #ShortSecurityContent {
    display: inline;
}

#wizardUsefulLinks {
    margin: 30px 0 7px;
}

#wizardUsefulLinks li {
    display: inline;
}

#wizardUsefulLinks li.linkSeparator {
    border-left: 1px solid white;
    padding-left: 8px;
}

#ClientInstallDescription {
    padding: 12px 0;
}

.ShowMoreSecurity
{
    display: block;
}

.HideMoreSecurity
{
    display: none;
}

div.PrintDiv
{
    width: 100%;
    font-size: <%=fontSizeSmall%>;
}

div.PrintDiv a
{
    float: right;
    background: url(<%=ClientInfoUtilities.getImageName(sClientInfo, "media/" + WizardConstants.ICON_PRINT)%>) center left no-repeat;
    padding: 2px 1px 2px 20px;
}

<%
// Down arrow should align with the middle of the security shield
%>
#HelpDiv
{
    width: 90%;
    color: #000;
    margin: 6px 0 0 6px;
    padding: 10px 0 0 18px;
}

div.SectionOpen
{
    background: transparent url(media/RoundedFadeBg.gif) top left no-repeat;
}

div.SectionClose #HelpLink
{
    background: url(<%=ClientInfoUtilities.getImageName(sClientInfo, "media/" + WizardConstants.ICON_ARROW_DOWN)%>) center left no-repeat;
}

div.SectionClose a#HelpLink:hover
{
    background: url(<%=ClientInfoUtilities.getImageName(sClientInfo, "media/" + WizardConstants.ICON_ARROW_DOWN_ROLLOVER)%>) center left no-repeat;
}

div.SectionOpen #HelpLink
{
    background: url(<%=ClientInfoUtilities.getImageName(sClientInfo, "media/" + WizardConstants.ICON_ARROW_UP)%>) center left no-repeat;
}

div.SectionOpen a#HelpLink:hover
{
    background: url(<%=ClientInfoUtilities.getImageName(sClientInfo, "media/" + WizardConstants.ICON_ARROW_UP_ROLLOVER)%>) center left no-repeat;
}

div.SectionClose #HelpContent
{
    display: none;
}

div.SectionOpen #HelpContent
{
    display: block;
}

#HelpLink
{
    padding: 2px 0 2px 20px;
    line-height: 16px;
}

a#HelpLink,
a#HelpLink:hover,
a#HelpLink:link,
a#HelpLink:visited
{
    text-decoration: none;
}

#HelpContent
{
    margin: 7px 0 0 20px;
}

h4.HelpSectionTitle
{
    padding: 0px 0px 5px 0px;
}

div.HelpStepNo
{
    float: left;
}

div.HelpStepText
{
    margin-left: 25px;
    margin-bottom: 5px;
<%
// Use the Holly hack in IE6 to avoid extra padding on the first paragraph line (the "3px text jog" bug).
if (sClientInfo.isIE() && sClientInfo.getBrowserVersionMajor() == 6) { %>
    height: 1%;
<% } %>
}

div.HelpStepText p {
  margin: 0;
}

div.SecurityImplications a
{
    font-size:<%=fontSizeBase%>;
}

input.Buttons
{
    margin: 10px 5px 5px 0;
    padding: 0.1em 0.5em;
    width: auto;
    overflow: visible;
}

div.description
{
    font-size: 85%;
    margin-top: 0px;
    padding-top: 0px;
}

div.SiteAddr
{
    color: #000088;
    font-style: italic;
    padding: 10px 20px 10px 20px;
}

#defaultClient
{
    margin-top: 2px;
    margin-bottom: 0px;
    float:left;
    width: <%=wizardContext.getString("AdvanceModeWidth")%>;
}

div.MainDiv .ClientText
{
    margin-bottom: 8px;
}

div.MainDiv .ClientDropDown
{
    margin-top: 4px;
    margin-bottom: 12px;
}

.AsteriskText
{
    margin: 8px 0px;
    font-size: <%=fontSizeSmall%>;
    font-style: italic;
}

.Error
{
    color: red;
}

input.Apply_Cancel_OK
{
    width: <%=buttonWidth_Apply_Cancel_Ok%>;
    margin: 10px 5px 5px 0;
}

p.feedbackStyleInfo
{
    background: #F2F2F2 url(<%=ClientInfoUtilities.getImageName(sClientInfo, "../media/Info16.png")%>) no-repeat 11px 7px;
    border: solid 1px #BCBCBC;
}

div.wiPopup p
{
    margin: 5px 0;
}

#Popup_MoreSecurity
{
    width: 30em;
}

<%
// Hide the high contrast button on load by default and show it only if the high contrast
// mode is enabled by the user
%>
#highContrast_DownloadButton
{
     display: none;
}

<%
// Style used to make sure screenreader doesn't read the popup
// unless it is truely visible.
%>
.invisiblePopup
{
    visibility: hidden;
}

#chkLegalStatement {
    vertical-align: middle;
}

<%
// Warning text is actually a link, so it can receive focus (and
// it will be read by the screenreader when this happens).
%>
#downloadWarningText #downloadWarningTextLink
{
    text-decoration: none;
    display: block;
    color: #2C2D31;
}

<%
// Adjust the padding etc for the warning text so that the
// focus box and the div edge co-incide.
%>
#downloadWarningText
{
    padding: 0;
    margin: 0;
    width: 17em;
}
#downloadWarningTextLink
{
    padding: 5px 10px 10px 10px;
    margin: 0;
}

-->
</style>
