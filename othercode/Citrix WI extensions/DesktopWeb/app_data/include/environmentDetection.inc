<%
// environmentDetection.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0

// Use noscript tag so that browsers with Javascript disabled get an error message.
// Javascript enabled browser call client side environment detection code which redirects.

// Show "Loading" browser title without branding to have smooth transition from
// static html files that are unbranded.
String browserTitle = wiContext.getString("BrowserTitleLoading");
%>

<%@ Register TagPrefix="wi" TagName="Header" Src="~/app_data/include/headerView.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
  <meta HTTP-EQUIV="Content-Type" CONTENT="text/html; charset=UTF-8">
  <meta NAME="ROBOTS" CONTENT="NOINDEX, NOFOLLOW, NOARCHIVE">
  <title><%=browserTitle%></title>
  <link rel="stylesheet" type="text/css" href="../html/styles/basicStyle.css" >
  <script type="text/javascript">
    <!--
    <!--#include file="~/app_data/clientscripts/cookies.js" -->
    <!--#include file="~/app_data/clientscripts/environmentDetection.js" -->
    // -->
  </script>
</head>

<body onLoad="setClientAndEnvironmentCookies('');">
<noscript>
<%
    wiContext.getWebAbstraction().setRequestContextAttribute(Constants.CTRL_HEADER, new HeaderControl());
    wiContext.getWebAbstraction().setRequestContextAttribute(Constants.CTRL_SEARCH_BOX, new SearchBoxControl());
    wiContext.getWebAbstraction().setRequestContextAttribute(Constants.CTRL_NAV, new NavControl());
%>
  <div id="overallWrapper">
    <div id="pageContent">
      <wi:Header runat="server" />
      <div class="mainPane">
        <div id="welcome">
          <h4 class="info"><%=wiContext.getString("JavascriptRequiredTitle")%></h4>
        </div>
        <div id="commonBox">
          <p><%=wiContext.getString("JavascriptRequiredDesc")%></p>
          <p><%=wiContext.getString("JavascriptRequiredAction")%></p>
        </div>
      </div>
    </div>
  </div>
</noscript>
</body>
</html>
