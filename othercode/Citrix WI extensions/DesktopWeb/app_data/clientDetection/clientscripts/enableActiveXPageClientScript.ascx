<!--#include file="~/app_data/clientDetection/clientscripts/include.aspxf" -->

<% if (model.getCurrentStep() == WizardConstants.PAGE_RADE) { %>
<!--#include file="~/app_data/clientDetection/clientscripts/radeClientDetection.js" -->
<% } else { %>
<!--#include file="~/app_data/clientDetection/clientscripts/nativeClientDetect.js" -->
<!--#include file="~/app_data/clientDetection/clientscripts/javaClientDetect.js" -->
<% } %>

<!--#include file="~/app_data/clientDetection/clientscripts/enableActiveX.js" -->
<!--#include file="~/app_data/clientDetection/clientscripts/cookies.js" -->
<!--#include file="~/app_data/clientDetection/clientscripts/util.js" -->
<!--#include file="~/app_data/clientDetection/clientscripts/commonDetection.js" -->
