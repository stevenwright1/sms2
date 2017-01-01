<%
// appembed.aspx
// Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<%@ Import Namespace="com.citrix.wi.pages.site" %>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->
<%
if (!(new AppEmbed(wiContext)).perform()) {
    return;
}

AppEmbedControl viewControl = (AppEmbedControl)wiContext.getWebAbstraction().getRequestContextAttribute("viewControl");

%>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<!--#include file="~/app_data/include/appembedheaders.inc" -->
<!--#include file="~/app_data/include/cachedJavaScript.inc"-->

<script type="text/javascript">
    <!--
    <!--#include file="~/app_data/clientscripts/environmentDetection.js" -->
    // -->
</script>
<!--#include file="clientscripts/appembed.js" -->
<!--#include file="clientscripts/handleError.js" -->
    <style type="text/css">
    <!--
    body
    {
        margin: 0px;
        padding: 0px;
    }
    -->
    </style>
</head>

<body <%= viewControl.initTag %> dir="<%=wiContext.getString( "TextDirection" )%>">
<div>
<%
if ( viewControl.redirectUrl != null ) {
%>
<script type="text/javascript">
<!--
redirectToMainFrame('<%=viewControl.redirectUrl%>');
// -->
</script>
<%
} else if (Embed.ICA_JAVACLIENT == viewControl.client) {
    // Install the Java client
%>
<!--#include file="~/app_data/include/appembedJICA.inc"-->
<%

} else if (Embed.RDP_ACTIVEX == viewControl.client) {
%>
<!--#include file="~/app_data/include/appembedRDP.inc"-->
<%

} else if (Embed.ICA_ICOCLIENT == viewControl.client) {
%>
<!--#include file="~/app_data/include/appembedICO.inc"-->
<%

} else if (Embed.RADE_RCOCLIENT == viewControl.client) {
%>
<!--#include file="~/app_data/include/appembedRCO.inc"-->
<%

} else { // ICAClient = unknown
    Response.Write( UIUtils.getJavascript(
                                        "handleError(MESSAGE_CENTRE, "
                                        + WebUtilities.escapeJavascript(viewControl.rdpLaunchErrorKey) + ","
                                        + WebUtilities.escapeJavascript(viewControl.appName) +")"
                                        )
                   );
}
%>

</div>

</body>
</html>
