<%
// javascript.aspx
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="~/app_data/serverscripts/include.aspxf"-->

<%
(new com.citrix.wi.pages.site_auth.JavaScript(wiContext)).perform();
StyleControl viewControl = (StyleControl)Context.Items["viewControl"];
// Feedback control is required from the timeoutclientscript to know when to timeout
// this can be cached as it is based on the configuration, and the caching key contains a hash of the config.
FeedbackControl feedbackControl = (FeedbackControl)wiContext.getWebAbstraction().getRequestContextAttribute("feedbackControl");
%>
    <!--#include file="~/app_data/clientscripts/commonUtils.js" -->
<% if(!Include.isCompactLayout(wiContext)) { %>
    <!--#include file="~/app_data/clientscripts/fullUtils.js" -->
    <!--#include file="~/app_data/clientscripts/popups.js" -->
    <!--#include file="~/app_data/clientscripts/layout.js" -->
    <!--#include file="~/app_data/clientscripts/lightbox.js" -->
    <!--#include file="~/app_data/clientscripts/cookies.js" -->
<% } %>
<% if(Include.isLoggedIn(wiContext.getWebAbstraction())) { %>
    <!--#include file="~/app_data/clientscripts/feedback.js" -->
    <!--#include file="~/site/clientscripts/euem.js" -->
    <!--#include file="~/app_data/clientscripts/removeDelayedUI.js" -->
<% } %>
