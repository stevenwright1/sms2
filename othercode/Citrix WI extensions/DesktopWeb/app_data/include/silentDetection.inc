<%
// silentDetection.inc
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<% if(viewControl.needsReloadToKillInfoBar) { %>
    <body onload="location.reload();">
<% } else if(viewControl.silentDetectionComplete) { %>
    <body onload="window.location='<%=viewControl.nextPage%>';">
<% } else { %>
    <body onload="postWizardResults();">

    <script type="text/javascript">
    function postWizardResults() {
        // Perform the required silent detection and update wizardOutputForm with the results
        var clientToDetect = "<%=viewControl.remoteClientToDetect%>";
        var useJavaFallback = <%=viewControl.useJavaFallback ? "true" : "false"%>;
        var detectStreamingClient = <%=viewControl.streamingClientToDetect == WizardConstants.RADE ? "true" : "false"%>;
        var serverRemoteClientVersion = "<%=viewControl.serverRemoteClientVersion%>";
        var serverStreamingClientVersion = "<%=viewControl.serverStreamingClientVersion%>";
        var result = SilentDetection.run(clientToDetect, useJavaFallback, detectStreamingClient, serverRemoteClientVersion, serverStreamingClientVersion);

        document.wizardOutputForm.<%=WizardConstants.PARAMETER_REMOTE_CLIENT%>.value = result.remoteClient;
        document.wizardOutputForm.<%=WizardConstants.PARAMETER_STREAMING_CLIENT%>.value = result.streamingClient;
        document.wizardOutputForm.<%=WizardConstants.PARAMETER_ICO_STATUS%>.value = result.icoStatus;
    <% if (viewControl.showRdpClassId) { %>
        document.wizardOutputForm.<%=WizardConstants.PARAMETER_RDP_CLIENT_CLASS_ID%>.value = result.rdpClassId;
    <% } %>
        document.wizardOutputForm.submit();
    }
    </script>
<% } %>


  <div id="horizonTop"></div>
  <div id="mainPane">

    <table class="glowBox dynamicGlowBoxMargin" cellpadding="0" cellspacing="0" align="center">
      <tr>
        <td class="glowBoxTop glowBoxLeft glowBoxTopLeft"></td>
        <td class="glowBoxTop glowBoxTopMid"><div class="leftGradient"><div class="rightGradient"><div class="centerGradient"></div></div></div></td>
        <td class="glowBoxTop glowBoxRight glowBoxTopRight"></td>
      </tr>
      <tr>
        <td class="glowBoxLeft glowBoxMidLeft"></td>
        <td class="glowBoxMid loginTableMidWidth">
          <div id="actionPane">
            <h3><%=wiContext.getString("LoadingTitle", wiContext.getDefaultLocale())%></h3>
<% if (Include.isCompactLayout(wiContext)) { %>
            <p><%=wiContext.getString("LoadingAction", wiContext.getDefaultLocale())%></p>
<% } else { %>
            <img src="../media/Loader.gif" alt="">
<% } %>
          </div>
        </td>
        <td class="glowBoxRight glowBoxMidRight"></td>
      </tr>
      <tr>
        <td class="glowBoxFooter glowBoxLeft glowBoxFooterLeft"></td>
        <td class="glowBoxFooter glowBoxFooterMid"></td>
        <td class="glowBoxFooter glowBoxRight glowBoxFooterRight"></td>
      </tr>
    </table>
  </div>

<% if(viewControl.detectClients) { %>
  <form name="wizardOutputForm" action="<%=Constants.PAGE_WIZARD_OUTPUT%>" method="post">
    <input type="hidden" name="<%=WizardConstants.FORM_CSRF_TOKEN%>" value="<%=SessionToken.get(wiContext)%>">
    <input type="hidden" name="<%=WizardConstants.PARAMETER_REMOTE_CLIENT%>" value="placeholder">
    <input type="hidden" name="<%=WizardConstants.PARAMETER_STREAMING_CLIENT%>" value="placeholder">
    <input type="hidden" name="<%=WizardConstants.PARAMETER_ICO_STATUS%>" value="placeholder">
<% if (viewControl.showRdpClassId) { %>
    <input type="hidden" name="<%=WizardConstants.PARAMETER_RDP_CLIENT_CLASS_ID%>" value="placeholder">
<% } %>
  </form>
<% // The following hidden div is used by the JavaScript client detection code for creating browser plug-in objects as required. %>
  <div id='hiddenDiv'></div>
<% } %>
</body>
