<%
// ageEmbedStyle.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<% // This file contains overrides to lowstyle.inc %>

body
{
    font-size: <%=wiContext.getString("FontSizeAGEmbedded")%>;
}

a,
a:link,
a:visited,
a:hover
{
    color: blue;
    text-decoration:underline;
}

.screenReaderText
{
    position: absolute;
    top: 0;
    left: -9999px;
}

<%
// -------------------------------------------------------------------
// Main pane
// -------------------------------------------------------------------
%>

.mainPane
{
  padding: 0 <%=wiContext.getString("LGHorizMargin")%> 0 <%=wiContext.getString("LGHorizMargin")%>;
  margin: <%=wiContext.getString("LGVertBigGap")%> 0 0 0;
  border-top: none;
}

<%
// -------------------------------------------------------------------
// Application list
// -------------------------------------------------------------------
%>

a.iconLink span
{
    text-decoration:none;
}

a.iconLink:hover span
{
    text-decoration:underline;
}