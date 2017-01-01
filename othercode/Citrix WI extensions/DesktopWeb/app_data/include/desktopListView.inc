<%
// desktopListView.inc
// Copyright (c) 2008 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<script type="text/javascript">
   // clear all the elements in the array before we render the page.
   delayedLaunchDesktops = [];
</script>
<div class="desktopView">
    <%=viewControl.desktopTabViewMarkup%>
</div>

<%-- A spacer div here avoids a problem with Safari 3.1 not clearing the mouse-over highlight --%>
<div class="spacer"></div>

