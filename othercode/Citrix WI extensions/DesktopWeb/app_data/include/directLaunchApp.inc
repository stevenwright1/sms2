<%
// directLaunchApp.inc
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<div class="commonInnerBox">
<div class="InformationCenterDivLeft">
<%
String retryInProgress = "false";

ResourceControl resource = viewControl.getResourceControl();

if (resource == null) {
%>
    <p class="noDirectLaunchApp"><%=wiContext.getString("NoLaunchApplication")%></p>
<%
}
%>

    <table id="directLaunchTable">
    <tr>
<%
if (resource != null) {
%>
    <td><%=resource.iconImg%></td>
    <td>
        <p id="directLaunchTitle"><%=viewControl.getAppNameText(wiContext)%></p>
        <%
        if (resource.isDelayedLaunch) {
        %>
            <div class="directDelayedLaunchInfo" style="padding:0;">
                <%
                if (resource.startInProgress) {
                    retryInProgress = "true";
                %>
                    <img id="progress_<%=resource.getEncodedAppId()%>"
                       class="progressIndicator"
                       src="../media/<%=DelayedLaunchUtilities.getDelayedLaunchProgressImg(wiContext)%>"
                       alt="<%=wiContext.getString("ProgressBarAltText")%>">
                <% } %>
                <ul class="delayedLaunchStatus">
                    <li id="launchStatus_<%=resource.getEncodedAppId()%>">
                        <p class="firstParagraph"><%=viewControl.getAppLaunchLink()%></p>
                    </li>
                </ul>
            </div>
            <div class="spacer"></div>
        <%
        } else {
        %>
            <p class="DirectLaunchAppLink"><%=viewControl.getAppLaunchLink()%></p>
        <% } %>
    </td>
<%
}
else
{
	// Display a blank table cell to ensure the table displays at its proper size.
%>
	<td>&nbsp;</td>
<% } %>
    </tr></table>

<% if (viewControl.getWizardLink() != null) { %>
<p class="TrustedZoneWizardLink"><%=wiContext.getString("TrustedZoneWizardLink", new Object[] {viewControl.getWizardLink(), resource.getName(wiContext)})%></p>
<% } %>

</div>


<%
if (resource != null) {
    %>

    <script type="text/javascript">
    <%
        // In low-graphics mode a separate page is used to prompt the user to confirm the restart operation.
        // The internal id of the desktop is passed to the confirmation page (via a query string parameter) so that, on positive confirmation,
        // the id of the desktop to power off can be set in the session.
        // If the RetryApplication query string is present for this page, it is passed on to avoid halting any delayed launch that may be in progress.
        String queryString = "&" + Constants.QSTR_APPLICATION + "=" + WebUtilities.escapeURL(resource.id);
        String retryApplication = wiContext.getWebAbstraction().getQueryStringParameter(Constants.QSTR_RETRY_APPLICATION);
        queryString += Strings.isEmpty(retryApplication) ? "" : "&" + Constants.QSTR_RETRY_APPLICATION + "=" + WebUtilities.escapeURL(retryApplication);
    %>
        // The following variable is used to track whether a retry operation is currently in progress. It is updated by the retry page once the desktop is ready to launch.
        var ctxRetryInProgress = <%=retryInProgress%>;

        function setLightboxText() {
            if (ctxRetryInProgress) {
                setLightboxTopMessage("<%=WebUtilities.escapeJavascript(wiContext.getString("SwitchOffWhileStarting", resource.getName(wiContext)))%>");
            } else {
                setLightboxTopMessage("<%=WebUtilities.escapeJavascript(wiContext.getString("SwitchOff", resource.getName(wiContext)))%>");
            }
        }

        function confirmRestart() {
            var url = "<%= Constants.PAGE_CONFIRM_RESTART_DESKTOP %>?<%= Constants.QSTR_RETRY_IN_PROGRESS %>=" + ctxRetryInProgress + "<%= queryString %>";

            window.location = url;
            return false;
        }

    </script>

    <%
    if (resource.isRestartable) {
        String restartLinkFragment = UIUtils.getDirectLaunchConfirmRestartLinkFragment(wiContext, resource.getName(wiContext), SessionToken.get(wiContext));
    %>
          <a <%=restartLinkFragment%> id="logoutAreaRestartLink" title="<%=wiContext.getString("TipRestartDesktop")%>"
          ><%= wiContext.getString("Restart") %></a>
    <% } %>
<% } %>
  <div class="spacer"></div>

</div>
