<%
// clientDetectionVariablesForPost.inc
// Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
  <link rel="stylesheet" type="text/css"  media="handheld,all" href="<%=UIUtils.getStyleSheetURL(wiContext)%>" >
  <link rel="SHORTCUT ICON" href="../media/<%=Images.getFavoritesIcon(wiContext)%>" type="image/vnd.microsoft.icon">
<title></title>
</head>
<body>
    <form name="clientWizardVariablesForPost" method="post" action="../clientDetection/<%=Constants.PAGE_WIZARD_START%>">
        <input type="hidden" name="<%=WizardConstants.PARAMETER_REMOTE_CLIENTS%>" value="<%=viewControl.getRemoteClients() %>">
        <input type="hidden" name="<%=WizardConstants.PARAMETER_STREAMING_CLIENTS%>" value="<%=viewControl.getStreamingClient() %>">
        <input type="hidden" name="<%=WizardConstants.PARAMETER_MODE%>" value="<%=viewControl.getWizardMode() %>">
        <input type="hidden" name="<%=WizardConstants.PARAMETER_SHOW_ZONE_PAGE%>" value="<%=viewControl.getShowZonePage() %>">
        <input type="hidden" name="<%=WizardConstants.PARAMETER_SHOW_UPGRADE_PAGE%>" value="<%=viewControl.getShowUpgradePage() %>">
        <input type="hidden" name="<%=WizardConstants.PARAMETER_PREFERRED_CLIENT%>" value="<%=viewControl.getPreferredClient() %>">
        <input type="hidden" name="<%=WizardConstants.PARAMETER_REDIRECT_URL%>" value="<%=viewControl.getRedirectURL() %>">
        <input type="hidden" name="<%=WizardConstants.PARAMETER_CSRF_TOKEN%>" value="<%=SessionToken.get(wiContext)%>">
        <input type="hidden" name="<%=WizardConstants.PARAMETER_LOCALE%>" value="<%=wiContext.getCurrentLocale() %>">
        <input type="hidden" name="<%=WizardConstants.PARAMETER_ALLOW_LOGOUT%>" value="<%=viewControl.getLogout() %>">
        <input type="hidden" name="<%=WizardConstants.PARAMETER_MASTER_PAGE%>" value="<%=Constants.PRIVATE_FOLDER_PATH + "include/" + Constants.PAGE_WIZARD_MASTER%>">
        <input type="hidden" name="<%=WizardConstants.PARAMETER_PRODUCT_TYPE%>" value="<%= viewControl.getProductType() %>">

    <% if (viewControl.isShowWelcomeScreen()){ %>
        <input type="hidden" name="<%=WizardConstants.PARAMETER_SHOW_WELCOME_SCREEN%>" value=<%=Constants.VAL_ON%>>
    <% } %>
    <% if (viewControl.getAccessGatewayCompatibility()){ %>
        <input type="hidden" name="<%=WizardConstants.PARAMETER_ACCESS_GATEWAY_PROXY%>" value=<%=Constants.VAL_ON%>>
    <% } %>
    <% if (viewControl.isJavaFallback()){ %>
        <input type="hidden" name="<%=WizardConstants.PARAMETER_JAVA_FALLBACK%>" value=<%=WizardConstants.ON%>>
    <% } %>
    </form>
    <script type="text/javascript">
        <!--
        document.clientWizardVariablesForPost.submit();
        // -->
    </script>
</body>
</html>

