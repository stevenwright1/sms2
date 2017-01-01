<%
// appembedRDP.inc
// Copyright (c) 2003 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<%
if (viewControl.rdpParams != null) {
%>
<script type="text/javascript" src="clientscripts/appEmbedClient.js"></script>

<script type="text/javascript">
<!--

function initRdpClient() {

<%
    Iterator paramIter = viewControl.rdpParams.nameIterator();
    while( paramIter.hasNext() ) {
        String paramName = (String)paramIter.next();

        // DesktopWidth & DesktopHeight are special - we need to handle invalid values
        if ( viewControl.isParamDesktopWidth( paramName ) || viewControl.isParamDesktopHeight( paramName ) || viewControl.isParamRedirectDrive(paramName) ) {
%>

            try {

<%
        }

        if ( viewControl.rdpParams.isLiteralParam( paramName ) ) {
%>

        rdpclient.<%=paramName%> = "<%=viewControl.rdpParams.getParam(paramName)%>";

<%
                } else {
%>

        rdpclient.<%=paramName%> = "<%=WebUtilities.escapeJavascript(viewControl.rdpParams.getParam(paramName))%>";

<%
                }

        if( viewControl.isParamDesktopWidth( paramName ) || viewControl.isParamDesktopHeight( paramName ) ) {
%>

            } catch (e) {

                handleError(MESSAGE_CENTRE, "<%=WebUtilities.escapeJavascript("InvalidRDPWindowSize")%>");
                window.close();
                return false;
            }

<%      } else if (viewControl.isParamRedirectDrive(paramName)){ %>
            } catch (e){
                alert('<%=WebUtilities.escapeJavascript(wiContext.getString("NoRdpRedirectDrives"))%>');
            }

<%
        }
    } // Server-side while
%>
    // Hide the restore button on the connection bar to work around the issue that users
    // don't know how to get the desktop back to fullscreen mode.
    rdpclient.AdvancedSettings3.ConnectionBarShowRestoreButton = false;
    rdpclient.ConnectingText = "<%= WebUtilities.escapeJavascript(wiContext.getString("RDPConnectingText")) %>";
    if (rdpclient.ConnectedStatusText!=null) {
        rdpclient.ConnectedStatusText = "<%= WebUtilities.escapeJavascript(wiContext.getString("RDPConnectedText")) %>";
    }
    return true;
}

function RdpLaunchFailed() {
    return false;
}

// -->
</script>

<script type="text/javascript" for="rdpclient" event="OnDisconnected(disconnectCode)">
  <!--
  rdpclient_OnDisconnected(disconnectCode);
  // -->
</script>

<script type="text/javascript" for="rdpclient" event="OnFatalError(errorCode)">
  <!--
  rdpclient_OnFatalError(errorCode);
  // -->
</script>

<script type="text/javascript" for="rdpclient" event="OnWarning(warningCode)">
  <!--
rdpclient_OnWarning(warningCode);
  // -->
</script>

<script type="text/javascript" for="rdpclient" event="onreadystatechange()">
  <!--
  onInitIE();
  // -->
</script>

<script type="text/javascript">
    <!--
    CreateRDPControl("clsid:<%= viewControl.rdpClientClassID %>",
         "rdpclient",
         "<%=viewControl.desiredHRES%>", "<%=viewControl.desiredVRES%>");
    // -->
</script>

<%
} else {
%>

<script type="text/javascript">
  <!--
  function RdpLaunchFailed() {
      handleError(MESSAGE_CENTRE, "<%=WebUtilities.escapeJavascript(viewControl.rdpLaunchErrorKey)%>");
      window.close();
      return true;
  }
  // -->
</script>
<%
}
%>
