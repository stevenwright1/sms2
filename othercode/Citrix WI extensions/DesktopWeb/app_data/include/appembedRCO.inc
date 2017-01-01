<%
// appembedRCO.inc
// Copyright (c) 2003 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<script type="text/javascript" event="OnRADFileFailed()" for="radeobj">
radeobj_OnRADFileFailed();
function radeobj_OnRADFileFailed(){
    var location = "<%= viewControl.homePage %>?<%= Constants.QSTR_EMBEDDED_RESOURCE_ERROR %>=<%=Constants.VAL_GENERAL%>";
    redirectToMainFrame(location);
}
</script>

<object id="radeobj" classid="clsid:<%=viewControl.radeClientClassID%>">
</object>
<script type="text/javascript">
    <!--
    var rcoObj = document.getElementById("radeobj");

    if (typeof rcoObj == "undefined" || rcoObj == null || typeof rcoObj.ClientVersion == "undefined") {
	    var location = "<%= viewControl.homePage %>?<%= Constants.QSTR_EMBEDDED_RESOURCE_ERROR %>=<%=Constants.VAL_RCO%>";
        redirectToMainFrame(location);
    }

    launchRadeApp("<%=Constants.PAGE_STREAMING_LAUNCH%>?<%=viewControl.QS%>");
    // -->
</script>