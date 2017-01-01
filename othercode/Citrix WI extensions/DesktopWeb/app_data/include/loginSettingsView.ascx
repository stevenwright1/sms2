<%@ Register TagPrefix="wi" TagName="PreferencesButtons" Src="~/app_data/include/preferencesButtonsView.ascx" %>
<%
// loginSettingsView.ascx
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="../serverscripts/include.aspxf"-->

<%
LoginSettingsPageControl viewControl = (LoginSettingsPageControl)wiContext.getWebAbstraction().getRequestContextAttribute("viewControl");
%>

<div class="settingsButtons">
  <wi:PreferencesButtons runat="server" />
</div>
      
<input type="hidden" name="<%=Constants.ID_SUBMIT_MODE%>" value="<%=Constants.VAL_OK%>">
<!--#include file="loginSettings.inc"-->

<div class="settingsButtons topBorder">
  <wi:PreferencesButtons runat="server" />
</div>
