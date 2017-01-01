<%
// challengeView.ascx
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<script runat="server">
  ChallengePageControl viewControl = null;
</script>
<%
viewControl = (ChallengePageControl)Context.Items["viewControl"];
%>
<!--#include file="../serverscripts/include.aspxf"-->
<!--#include file="challenge.inc"-->