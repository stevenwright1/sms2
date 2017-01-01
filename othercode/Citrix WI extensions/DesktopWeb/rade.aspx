<%
// rade.aspx
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<%@ EnableSessionState = False %>
<%@ Import Namespace="com.citrix.wi.config" %>
<%@ Import Namespace="com.citrix.wi.mvc" %>
<%@ Import Namespace="com.citrix.wi.mvc.asp" %>
<%@ Import Namespace="com.citrix.wi.pageutils" %>
<%@ Import Namespace="com.citrix.wing.rade" %>
<%@ Import Namespace="com.citrix.wing.types" %>
<%@ Import Namespace="com.citrix.wi.util" %>

<%

WebAbstraction _web = AspWebAbstraction.getInstance(Context);

// check that configuration is available
WIConfiguration configuration = CheckConfig.getConfiguration(_web);
if ((configuration == null)) {
    CheckConfig.doMissingConfigurationResponse(_web);
    return;
}

// Handle rade requests from the client if streaming is enabled
if ( configuration.getAppAccessMethodConfiguration().isEnabledAppAccessMethod(AppAccessMethod.STREAMING) )
{
    byte[] requestByteArr = Request.BinaryRead(Request.TotalBytes);
    string requestContent = System.Text.Encoding.UTF8.GetString(requestByteArr, 0, requestByteArr.Length);

    try {
            RadeService radeService = (RadeService)Application[AppAttributeKey.RADE_SERVICE];
            String response = radeService.parseRadeRequests(requestContent);
            Response.Write(response);
    } catch (Exception) {
           Response.Clear();
           Response.StatusCode = 500;
           Response.AddHeader("DPErrorId", "CharlotteErrorOther");
           Response.AddHeader("DPError", "");
           Response.End();
    }
}
%>
