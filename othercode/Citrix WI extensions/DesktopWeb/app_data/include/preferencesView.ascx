<%@ Register TagPrefix="wi" TagName="PreferencesButtons" Src="~/app_data/include/preferencesButtonsView.ascx" %>
<%
// preferencesView.ascx
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<!--#include file="../serverscripts/include.aspxf"-->

<%
PreferencesPageControl  viewControl = (PreferencesPageControl)Context.Items["viewControl"];
  
AccountSettingsPageControl accountSettings = viewControl.getAccountSettings();
ClientSettingsControl clientSettings = viewControl.getClientSettings();
DisplaySettingsPageControl displaySettings = viewControl.getDisplaySettings();
SessionSettingsPageControl sessionSettings = viewControl.getSessionSettings();
WorkspaceControlSettingsViewControl workspaceControlSettings = viewControl.getWorkspaceControlSettings();
%>

<div class="settingsButtons">
  <wi:PreferencesButtons runat="server" />
</div>
      
<input type="hidden" name="<%=Constants.ID_SUBMIT_MODE%>" value="<%=Constants.VAL_OK%>">
<!--#include file="displaySettings.inc"-->
<!--#include file="workspaceControlSettings.inc"-->
<!--#include file="clientSettings.inc"-->
<!--#include file="accountSettings.inc"-->
<!--#include file="sessionSettings.inc"-->

<div class="settingsButtons topBorder">
  <wi:PreferencesButtons runat="server" />
</div>