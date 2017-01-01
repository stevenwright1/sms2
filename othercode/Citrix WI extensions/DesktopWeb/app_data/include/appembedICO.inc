<%
// appembedICO.inc
// Copyright (c) 2003 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<script type="text/javascript" event="OnICAFileFailed()" for="icaobj">
icaobj_OnICAFileFailed();
function icaobj_OnICAFileFailed(){
    var location = "<%= viewControl.homePage %>?<%= Constants.QSTR_EMBEDDED_RESOURCE_ERROR %>=<%=Constants.VAL_GENERAL%>";
    redirectToMainFrame(location);
}
</script>

<object id="icaobj" classid="clsid:<%=viewControl.icaClientClassID%>">
  <param name="Launch" value= "true">
  <param name="Start" value= "true">
  <param name="IPCLaunch" value="false">
  <param name="ICAfile"  value="<%=Constants.PAGE_LAUNCH%>?<%=viewControl.QS%>">
</object>

<script type="text/javascript">
var icoObj = document.getElementById("icaobj");

if (typeof icoObj == "undefined" || icoObj == null || typeof icoObj.ClientVersion == "undefined") {
	var location = "<%= viewControl.homePage %>?<%= Constants.QSTR_EMBEDDED_RESOURCE_ERROR %>=<%=Constants.VAL_ICO%>";
    redirectToMainFrame(location);
}
</script>