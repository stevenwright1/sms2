<%
// directLaunchDesktop.inc
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0

String retryInProgress = "false";

ResourceControl desktop = viewControl.getResourceControl();

if (desktop == null) {
%>
    <p class="noDirectLaunchApp"><%=wiContext.getString("NoLaunchApplication")%></p>
<% } else { %>

    <script type="text/javascript">
    <%
        // In low-graphics mode a separate page is used to prompt the user to confirm the restart operation.
        // The internal id of the desktop is passed to the confirmation page (via a query string parameter) so that, on positive confirmation,
        // the id of the desktop to power off can be set in the session.
        // If the RetryApplication query string is present for this page, it is passed on to avoid halting any delayed launch that may be in progress.
        String queryString = "&" + Constants.QSTR_APPLICATION + "=" + WebUtilities.escapeURL(desktop.id);
        String retryApplication = wiContext.getWebAbstraction().getQueryStringParameter(Constants.QSTR_RETRY_APPLICATION);
        queryString += Strings.isEmpty(retryApplication) ? "" : "&" + Constants.QSTR_RETRY_APPLICATION + "=" + WebUtilities.escapeURL(retryApplication);
    %>
        // The following variable is used to track whether a retry operation is currently in progress. It is updated by the retry page once the desktop is ready to launch.
        var ctxRetryInProgress = <%=retryInProgress%>;

        function setLightboxText() {
            if (ctxRetryInProgress) {
                setLightboxTopMessage("<%=WebUtilities.escapeJavascript(wiContext.getString("SwitchOffWhileStarting", desktop.getName(wiContext)))%>");
            } else {
                setLightboxTopMessage("<%=WebUtilities.escapeJavascript(wiContext.getString("SwitchOff", desktop.getName(wiContext)))%>");
            }
        }

        function confirmRestart() {
            var url = "<%= Constants.PAGE_CONFIRM_RESTART_DESKTOP %>?<%= Constants.QSTR_RETRY_IN_PROGRESS %>=" + ctxRetryInProgress + "<%= queryString %>";

            window.location = url;
            return false;
        }
    </script>
    <%
    DelayedLaunchControl delayedLaunchControl = DelayedLaunchUtilities.getDelayedLaunchControl(wiContext);
    DesktopLaunchHistory desktopLaunchHistory = DesktopLaunchHistory.getInstance(wiContext);
    // cannot use bool or boolean, as the code is shared between jsp and asp
    String desktopClass = "desktopScreen";
    if (desktopLaunchHistory.containsDesktop(desktop.id) ||
        delayedLaunchControl.isBlockedLaunch(desktop.id)) {
        desktopClass += " activeDesktop";
    }
    %>
    
    <div class="desktopResourceDirectLaunch" id="desktop_<%=desktop.getEncodedAppId()%>">
        <div class='desktopName'><a <%=desktop.launchHref%> id="<%=desktop.getEncodedAppId()%>" title="<%=desktop.getDescription(wiContext)%>"
            ><%=WebUtilities.escapeHTML(desktop.getTruncatedName(wiContext))%></a>
        </div>
        <div class="desktopScreenContainer">
            <a <%=desktop.launchHref%> id="<%=desktop.getEncodedAppId()%>" title="<%=desktop.getDescription(wiContext)%>"
                ><div class='<%=desktopClass%>' id='screen_<%=desktop.getEncodedAppId()%>'
                     onmouseover="updateDirectLaunchDisplay(this, true)" onmouseout="updateDirectLaunchDisplay(this, false);"
                     ><%=Include.getDelayedLaunchImg(wiContext, desktop, "desktopSpinner_" + desktop.getEncodedAppId())%></div
            ></a>
        </div>

        <%
        if (desktop.isRestartable) {
            String restartLinkFragment = UIUtils.getRestartDesktopUrl(wiContext, desktop.id, SessionToken.get(wiContext));
            %>
			<div id='directLaunchRestart' class='restartLinkNotRestartable'><!-- --></div>            
            <div id='directLaunchRestart' class='restartLinkAlwaysShow'>
                <a href="javascript:showRestart('<%=desktop.getEncodedAppId()%>', true);"><%=wiContext.getString("RestartDesktopText")%></a>
            </div>

            </div>
            <div id='restartConfirmation_<%=desktop.getEncodedAppId()%>' class="directLaunchRestartConfirmation">
                <div class="restartConfirmationContent">
                    <div class="restartConfirmationIcon"><!-- --></div>
                    <div class="restartConfirmationText">
                        <%=wiContext.getString("RestartDesktopConfirmationText")%>
                    </div>
                </div>
                <div class="spacer"></div>
                <div class="restartBoxButtonPane">
                    <div class="customButton">
                        <a href="<%=restartLinkFragment%>" class="leftDoor" style="text-decoration: none; margin-right:5px;"
                            ><span class="rightDoor">Restart</span
                        ></a>
                    </div>

                    <div class="customButton">
                        <a href="javascript:showRestart('<%=desktop.getEncodedAppId()%>', false);" class="leftDoor" style="text-decoration: none"
                            ><span class="rightDoor">Cancel</span
                        ></a>
                    </div>
                </div>
            </div>
        <%
        } else {
        %>
            <div id='directLaunchRestart' class='restartLinkNotRestartable'><!-- --></div>
        <% } %>
<% } %>
