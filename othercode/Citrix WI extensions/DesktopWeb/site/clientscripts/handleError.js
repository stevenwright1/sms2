<%
// handleError.js
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0

// Functions for handling errors in JavaScript (used by appembed page)
// In general, JavaScript error handling should be performed by constructing
// the redirect URL in server-side code and then redirecting to this URL in
// JavaScript (see redirectToMainFrame() in fullUtils.js).

// The functions on this page require fullUtils.js to be included.
%>
<script type="text/javascript">
<!--
var MESSAGE_ALERT = "alert";
var MESSAGE_CENTRE = "centre";

function handleError(errorType, errorMessageData) {
    handleError(errorType, errorMessageData, null);
}

function handleError(errorType, errorMessageData, args) {
    // If message type is MESSAGE_CENTER then errorMessageData is a message key.
    // Otherwise it is the actual message.
    if (errorType==MESSAGE_CENTRE) {
        var location = "<%= viewControl.homePage %>?<%= Constants.QSTR_MSG_TYPE %>=<%= WebUtilities.escapeURL("" + MessageType.ERROR) %>&<%= Constants.QSTR_MSG_KEY %>=" + errorMessageData;
        // Args is currently only numeric, so do not URL encode it.
        if ((args!=null) && (args !="")) {
            location = location + "&<%= Constants.QSTR_MSG_ARGS %>=" + args;
        }

        // Perform the redirection
        redirectToMainFrame(location);
    } else {
        // Arguments are not currently supported in alerts.
        alert(errorMessageData);
    }
}
// -->
</script>