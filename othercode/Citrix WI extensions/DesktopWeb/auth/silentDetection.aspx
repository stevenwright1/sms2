<%
// silentDetection.aspx
// Copyright (c) 2002 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0

if (Request.QueryString.Get("compileonly") == null) {
%>
<!--#include file="~/app_data/serverscripts/include.aspxf"-->
<%
    if (new com.citrix.wi.pages.auth.SilentDetection(wiContext).perform()) {
        SilentDetectionPageControl viewControl = (SilentDetectionPageControl)Context.Items["viewControl"];
        if(viewControl.detectEnvironment) {
%>
            <!--#include file="~/app_data/include/environmentDetection.inc"-->
<%
        } else {
        // Show "Loading" browser title without branding to have smooth transition from
        // static html files that are unbranded.
        String browserTitle = wiContext.getString("BrowserTitleLoading");
%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
    <head>
        <title><%=browserTitle%></title>
        <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
        <meta name="ROBOTS" content="NOINDEX, NOFOLLOW, NOARCHIVE">
<% if (viewControl.detectClients) { %>
        <script type="text/javascript">
            <!--#include file="~/app_data/clientDetection/clientscripts/commonDetection.js"-->
            <!--#include file="~/app_data/clientscripts/silentDetection.js"-->
        </script>
<% } %>
        <style type="text/css" media="handheld,all">
            <!--#include file="~/app_data/include/silentDetectionStyle.inc"-->
        </style>
    </head>
<!--#include file="~/app_data/include/silentDetection.inc"-->
</html>
<%
        }
    }
}
%>
