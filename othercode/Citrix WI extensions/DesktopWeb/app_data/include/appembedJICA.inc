<%
// appembedJICA.inc
// Copyright (c) 2003 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<script type="text/javascript" src="clientscripts/appEmbedClient.js"></script>

<script type="text/javascript">
    <!--
    CreateJICAApplet("<%=viewControl.jicaCode%>", "<%=viewControl.desiredHRES%>", "<%=viewControl.desiredVRES%>", "<%=viewControl.clientPath%>", "<%=viewControl.jicaPackages%>", "<%=Constants.PAGE_LAUNCH%>?<%=viewControl.QS%>", "<%=viewControl.closeURL%>", "<%=viewControl.jicaLang%>", "<%=viewControl.JICACookie%>", "<%=wiContext.getString("NoJavaApplet")%>", <%=viewControl.jicaUseZeroLatency ? "true" : "false"%>);
    // -->
</script>
