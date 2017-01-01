<%
// appembed.js
// Copyright (c) 2003 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<script type="text/javascript">
<!--

var alerted = false;
var connectionMethod = false;

function onUnload()
{
// Note: this function will never run on Safari
    if (!alerted && connectionMethod)
    {
        var connected = document.javaclient.isConnected();
        if (connected) {
            window.parent.alert('<%= viewControl.sessionDisconnected %>');
        }
    }
}

// Note: this function only used for IE

function onBeforeUnload()
{
    if (connectionMethod)
    {
        var connected = document.javaclient.isConnected();
        if (connected) {
            alerted = true;
            return "<%= viewControl.sessionQuestionDisconnect %>";
        }
    }
}

function onInitIE()
{
    window.onbeforeunload = onBeforeUnload;
    onInit();
}

function launchRadeApp(url) {
    try {
        radeobj.LaunchRadeApp("<%=Constants.PAGE_STREAMING_LAUNCH%>?<%=viewControl.QS%>");
    } catch (e) {
        handleError(MESSAGE_CENTRE, "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("StreamingAppLaunchFailureNoClientInstalled")) %>");
    }
}

function onInit()
{
    window.onunload = onUnload;
<%
if (!wiContext.getClientInfo().isSafari()) {
%>
    if (typeof(document.javaclient) != "undefined")
    {
        connectionMethod = true;
    }
<%
}
%>
    <% // RdpLaunchFailed  and checkRdpClient are included in appembed.aspx and
       // defined in appembedRDP.inc.  %>
    if ("<%= viewControl.client %>" == "<%= Embed.RDP_ACTIVEX %>") {
        if (!RdpLaunchFailed() && checkRdpClient() && initRdpClient()) {
            connectRdpClient();
        }
    }
}

function icaobj_OnDisconnect()
{
    window.close();
}

function rdpclient_OnDisconnected(disconnectCode) {
    var errorType;
    var errorMessageData = "";
    var extendedDisconnectCode;
    var args;

    extendedDisconnectCode = rdpclient.ExtendedDisconnectReason;

    switch(extendedDisconnectCode) {
        case 0:
            // No additional information is available.
            break;
        case 1:
            // An application initiated the disconnection.
            //
            // Logoff/disconnect initiated from the start menu on the
            // remote desktop always trigger this event code when the
            // remote desktop is a workstation (e.g. Windows XP). Hence we
            // are going to surpress this alert.
            break;
        case 2:
            // An application logged off the client.
            errorMessageData = "<%= WebUtilities.escapeJavascript(wiContext.getString("IMsRdpClient-ExtendedDisconnectReason-2")) %>";
            errorType = MESSAGE_ALERT;
            break;
        case 3:
            // The server has disconnected the client because the client has been idle for a period of time longer than the designated timeout period.
            errorMessageData = "<%= WebUtilities.escapeJavascript(wiContext.getString("IMsRdpClient-ExtendedDisconnectReason-3")) %>";
            errorType = MESSAGE_ALERT;
            break;
        case 4:
            // The server has disconnected the client because the client has exceeded the period designated for connection.
            errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsRdpClient-ExtendedDisconnectReason-4")) %>";
            errorType = MESSAGE_CENTRE;
            break;
        case 5:
            // The client's connection was replaced by another connection.
            errorMessageData = "<%= WebUtilities.escapeJavascript(wiContext.getString("IMsRdpClient-ExtendedDisconnectReason-5")) %>";
            errorType = MESSAGE_ALERT;
            break;
        case 6:
            // No memory is available.
            errorMessageData = "<%= WebUtilities.escapeJavascript(wiContext.getString("IMsRdpClient-ExtendedDisconnectReason-6")) %>";
            errorType = MESSAGE_ALERT;
            break;
        case 7:
            // The server refused the connection.
            errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsRdpClient-ExtendedDisconnectReason-7")) %>";
            errorType= MESSAGE_CENTRE;
            break;
        case 11:
            // This is an undocumented code occurs for Windows 7 when disconnected from start menu. It appears to be beneign and hence
            // we ignore it.
            break;
        case 256:
            // Internal licensing error.
            errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsRdpClient-ExtendedDisconnectReason-256")) %>";
            errorType = MESSAGE_CENTRE;
            break;
        case 257:
            // No licensing server was available.
            errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsRdpClient-ExtendedDisconnectReason-257")) %>";
            errorType = MESSAGE_CENTRE;
            break;
        case 258:
            // No valid software license was available.
            errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsRdpClient-ExtendedDisconnectReason-258")) %>";
            errorType = MESSAGE_CENTRE;
            break;
        case 259:
            // The remote computer received an invalid licensing message.
            errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsRdpClient-ExtendedDisconnectReason-259")) %>";
            errorType = MESSAGE_CENTRE;
            break;
        case 260:
            // The hardware ID does not match the ID on the software license.
            errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsRdpClient-ExtendedDisconnectReason-260")) %>";
            errorType = MESSAGE_CENTRE;
            break;
        case 261:
            // Client license error.
            errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsRdpClient-ExtendedDisconnectReason-261")) %>";
            errorType = MESSAGE_CENTRE;
            break;
        case 262:
            // Network problems occurred during the licensing protocol.
            errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsRdpClient-ExtendedDisconnectReason-262")) %>";
            errorType = MESSAGE_CENTRE;
            break;
        case 263:
            // The client ended the licensing protocol prematurely.
            errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsRdpClient-ExtendedDisconnectReason-263")) %>";
            errorType = MESSAGE_CENTRE;
            break;
        case 264:
            // A licensing message was encrypted incorrectly.
            errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsRdpClient-ExtendedDisconnectReason-264")) %>";
            errorType = MESSAGE_CENTRE;
            break;
        case 265:
            // The local computer client access license could not be upgraded or renewed.
            errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsRdpClient-ExtendedDisconnectReason-265")) %>";
            errorType = MESSAGE_CENTRE;
            break;
        case 266:
            // The remote computer is not licensed to accept remote connections.
            errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsRdpClient-ExtendedDisconnectReason-266")) %>";
            errorType = MESSAGE_CENTRE;
            break;
        default:
            if ((extendedDisconnectCode >= 0x1000) && (extendedDisconnectCode <= 0x7FFF)) {
                // An internal protocol error occured.
                errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsRdpClient-ExtendedDisconnectReason-4096")) %>";
                args = extendedDisconnectCode;
                errorType = MESSAGE_CENTRE;
            } else {
                errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsRdpClient-ExtendedDisconnectReason-Unknown")) %>";
                args = extendedDisconnectCode;
                errorType = MESSAGE_CENTRE;
            }
    }

    if (errorMessageData=="") {
        switch(disconnectCode) {
            case 0:
                // No information is available.
                errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnDisconnected-0")) %>";
                errorType = MESSAGE_CENTRE;
                break;
            case 1:
                // Local disconnection. This is not an error code.
                errorMessageData = "";
            case 2:
                // Remote disconnection by user. This is not an error code.
                errorMessageData = "";
            case 3:
                // Remote disconnection by server. This is not an error code.
                errorMessageData = "";
                break;
            case 260:
                //DNS name lookup failure.
                errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnDisconnected-260")) %>";
                errorType = MESSAGE_CENTRE;
                break;
            case 262:
                // Out of memory.
                errorMessageData = "<%= WebUtilities.escapeJavascript(wiContext.getString("IMsTscAxEvents-OnDisconnected-262")) %>";
                errorType = MESSAGE_ALERT;
                break;
            case 264:
                // Connection timed out.
                errorMessageData = "<%=WebUtilities.escapeJavascript( WebUtilities.escapeURL("IMsTscAxEvents-OnDisconnected-264")) %>";
                errorType = MESSAGE_CENTRE;
                break;
            case 516:
                // WinSock socket connect failure.
                errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnDisconnected-516")) %>";
                errorType = MESSAGE_CENTRE;
                break;
            case 518:
                // Out of memory.
                errorMessageData = "<%= WebUtilities.escapeJavascript(wiContext.getString("IMsTscAxEvents-OnDisconnected-518")) %>";
                errorType = MESSAGE_ALERT;
                break;
            case 520:
                // Host not found error.
                errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnDisconnected-520")) %>";
                errorType = MESSAGE_CENTRE;
                break;
            case 772:
                // WinSock send call failure.
                errorMessageData = "<%= WebUtilities.escapeJavascript(wiContext.getString("IMsTscAxEvents-OnDisconnected-772")) %>";
                errorType = MESSAGE_ALERT;
                break;
            case 774:
                // Out of memory.
                errorMessageData = "<%= WebUtilities.escapeJavascript(wiContext.getString("IMsTscAxEvents-OnDisconnected-774")) %>";
                errorType = MESSAGE_ALERT;
                break;
            case 776:
                // Invalid IP address specified.
                errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnDisconnected-776")) %>";
                errorType = MESSAGE_CENTRE;
                break;
            case 1028:
                // WinSock recv call failure.
                errorType = MESSAGE_ALERT;
                errorMessageData = "<%= WebUtilities.escapeJavascript(wiContext.getString("IMsTscAxEvents-OnDisconnected-1028")) %>";
                break;
            case 1030:
                // Invalid security data.
                errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnDisconnected-1030")) %>";
                errorType = MESSAGE_CENTRE;
                break;
            case 1032:
                // Internal error.
                errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnDisconnected-1032")) %>";
                errorType = MESSAGE_CENTRE;
                break;
            case 1286:
                // Invalid encryption method specified.
                errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnDisconnected-1286")) %>";
                errorType = MESSAGE_CENTRE;
                break;
            case 1288:
                // DNS lookup failed.
                errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnDisconnected-1288")) %>";
                errorType = MESSAGE_CENTRE;
                break;
            case 1540:
                // GetHostByName call failed.
                errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnDisconnected-1540")) %>";
                errorType = MESSAGE_CENTRE;
                break;
            case 1542:
                // Invalid server security data.
                errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnDisconnected-1542")) %>";
                errorType = MESSAGE_CENTRE;
                break;
            case 1544:
                // Internal timer error.
                errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnDisconnected-1544")) %>";
                errorType = MESSAGE_CENTRE;
                break;
            case 1796:
                // Timeout occurred.
                errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnDisconnected-1796")) %>";
                errorType = MESSAGE_CENTRE;
                break;
            case 1798:
                // Failed to unpack server certificate.
                errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnDisconnected-1798")) %>";
                errorType = MESSAGE_CENTRE;
                break;
            case 2052:
                // Bad IP address specified.
                errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnDisconnected-2052")) %>";
                errorType = MESSAGE_CENTRE;
                break;
            case 2056:
                // Internal security error.
                errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnDisconnected-2056")) %>";
                errorType = MESSAGE_CENTRE;
                break;
            case 2308:
                // Socket closed.
                errorMessageData = "<%= WebUtilities.escapeJavascript(wiContext.getString("IMsTscAxEvents-OnDisconnected-2308")) %>";
                errorType = MESSAGE_ALERT;
                break;
            case 2310:
                // Internal security error.
                errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnDisconnected-2310")) %>";
                errorType = MESSAGE_CENTRE;
                break;
            case 2312:
                // Licensing timeout.
                errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnDisconnected-2312")) %>";
                errorType = MESSAGE_CENTRE;
                break;
            case 2566:
                // Internal security error.
                errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnDisconnected-2566")) %>";
                errorType = MESSAGE_CENTRE;
                break;
            case 2822:
                // Encryption error.
                errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnDisconnected-2822")) %>";
                errorType = MESSAGE_CENTRE;
                break;
            case 3078:
                // Decryption error.
                errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnDisconnected-3078")) %>";
                errorType = MESSAGE_CENTRE;
                break;
            default:
                errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnDisconnected-Unknown")) %>";
                args = disconnectCode
                errorType = MESSAGE_CENTRE;
                break;
        }
    }
    if (errorMessageData!="") {
        handleError(errorType, errorMessageData, args);
    }
    window.close();
}

function rdpclient_OnFatalError(errorCode) {
    var errorMessageKey;
    var args;

    switch (errorCode) {
        case 0:
            // An unknown error has occurred.
            errorMessageKey = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnFatalError-0")) %>";
            errorType = MESSAGE_CENTRE
            break;
        case 1:
            // Internal error code 1.
            errorMessageKey = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnFatalError-1")) %>";
            errorType = MESSAGE_CENTRE
            break;
        case 2:
            // An out-of-memory error has occurred.
            errorMessageKey = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnFatalError-2")) %>";
            errorType = MESSAGE_CENTRE
            break;
        case 3:
            // A window creation error has occurred.
            errorMessageKey = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnFatalError-3")) %>";
            errorType = MESSAGE_CENTRE
            break;
        case 4:
            // Internal error code 2.
            errorMessageKey = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnFatalError-4")) %>";
            errorType = MESSAGE_CENTRE
            break;
        case 5:
            // Internal error code 3. This is an invalid state.
            errorMessageKey = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnFatalError-5")) %>";
            errorType = MESSAGE_CENTRE
            break;
        case 6:
            // Internal error code 4.
            errorMessageKey = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnFatalError-6")) %>";
            errorType = MESSAGE_CENTRE
            break;
        case 7:
            // An unrecoverable error has occurred during client connection.
            errorMessageKey = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnFatalError-7")) %>";
            errorType = MESSAGE_CENTRE
            break;
        case 100:
            // Winsock initialization error.
            errorMessageKey = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnFatalError-100")) %>";
            errorType = MESSAGE_CENTRE
            break;
        default:
            // Unknown error code.
            errorMessageKey = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnFatalError-Unknown")) %>";
            errorType = MESSAGE_CENTRE
            args = errorCode;
            break;
    }

    handleError(errorType, errorMessageKey, args);
    window.close();
}

function rdpclient_OnWarning(warningCode) {
    var errorMessageData;
    var errorType;
    var args;

    if (warningCode==1) {
        errorMessageData = "<%= WebUtilities.escapeJavascript(wiContext.getString("IMsTscAxEvents-OnWarning-1")) %>";
        errorType = MESSAGE_ALERT;
    } else {
        errorMessageData = "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IMsTscAxEvents-OnWarning-Unknown")) %>";
        errorType = MESSAGE_CENTRE;
        args = warningCode;
    }

    handleError(errorType, errorMessageData, args);
}

function checkRdpClient() {
    var rdpClientClassID;
    if (rdpclient.object==null) {
        rdpClientClassID = "<%=viewControl.rdpClientClassID%>";
        if (rdpClientClassID=="") {
            handleError(MESSAGE_CENTRE, "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("FailedToFindRDPClient")) %>");
            window.close();
        } else {
            window.location = window.location.href + "&CTX_RdpClientProgId=" + rdpClientClassID;
        }
    } else {
        if (!rdpclient.SecuredSettingsEnabled) {
            handleError(MESSAGE_CENTRE, "<%= WebUtilities.escapeJavascript(WebUtilities.escapeURL("IncorrectZone")) %>");
            window.close();
        } else {
            return true;
        }
    }
}

function connectRdpClient()
{
    rdpclient.Connect();
}

// -->
</script>
