<%
// workspaceControlSettings.inc
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<%
if (viewControl.getShowGeneralSection()) {
  if (workspaceControlSettings.getShowWorkspaceControlSettings() ||
      accountSettings.getShowAutoLoginSettings()) {
%>
<dl>
  <dt>
    <p><%=wiContext.getString("DescLogIn")%></p>
  </dt>
  <dd>
    <dl>
<%
    if (accountSettings.getShowAutoLoginSettings()) {
%>
      <dt class="nonLabel">
        <ul>
          <li>
            <input type="checkbox" id="<%=Constants.ID_CHECK_SILENT_AUTHENTICATION%>"
                   class="marginAdjust"
                   name="<%=Constants.ID_CHECK_SILENT_AUTHENTICATION%>" value="<%=Constants.VAL_ON%>"
                   <%=accountSettings.getAutoLoginCheckedStr()%>>
            <label for="<%=Constants.ID_CHECK_SILENT_AUTHENTICATION%>">
              <%=wiContext.getString("SilentAuthentication")%>
            </label>
            <%=UIUtils.generateHelpLinkMarkup(wiContext, "Help_SilentAuthentication", "inlineHelpLink checkboxHelp")%>
           <div id='<%=UIUtils.getPopupId("Help_SilentAuthentication")%>' class="wiPopup" ><%=wiContext.getString("Help_SilentAuthentication")%></div>        
         </li>
       </ul>
      </dt>
<%
    }

    if (workspaceControlSettings.getShowReconnectAtLoginSettings()) {
%>
      <dt>
        <input type="checkbox" id ="<%=Constants.ID_CHECK_RECONNECT_LOGIN%>"
               class="marginAdjust"
               name="<%=Constants.ID_CHECK_RECONNECT_LOGIN%>" value="<%=Constants.VAL_ON%>"
               <%=workspaceControlSettings.getReconnectCheckedStr()%> 
               onClick="reconnect_clicked(this.form.<%=Constants.ID_CHECK_RECONNECT_LOGIN%>, this.form.<%=Constants.ID_OPTION_RECONNECT_LOGIN%>);">
        <label for="<%=Constants.ID_CHECK_RECONNECT_LOGIN%>" class="noPush">
          <%=wiContext.getString("ReconnectAtLogon")%>
        </label>
        <label for="<%=Constants.ID_OPTION_RECONNECT_LOGIN%>" class="screenReaderText">
          <%=wiContext.getString("SelectSessionsToReconnect")%>
        </label>
      </dt>
      <dd>
        <select name='<%=Constants.ID_OPTION_RECONNECT_LOGIN%>'
                id='<%=Constants.ID_OPTION_RECONNECT_LOGIN%>' size='1'>
          <option value='<%=Constants.VAL_DISCONNECTED%>' <%=workspaceControlSettings.getReconnectAtLoginDisconnectedSelectedStr()%>>
            <%=wiContext.getString("DisconnectedApplicationsOnly")%>
          </option>
          <option value='<%=Constants.VAL_DISCONNECTED_ACTIVE%>'
            <%=workspaceControlSettings.getReconnectAtLoginAllSelectedStr()%>>
            <%=wiContext.getString("AllApplications")%>
          </option>
        </select>
        <%=UIUtils.generateHelpLinkMarkup(wiContext, "Help_LogOnExperience", "inlineHelpLink")%>
        <div id='<%=UIUtils.getPopupId("Help_LogOnExperience")%>' class="wiPopup" ><%=wiContext.getString("Help_LogOnExperience")%></div>
      </dd>
<%
    }
%>
	</dl>
  </dd>
</dl>
<%
  }
%>

<%
  if (workspaceControlSettings.getShowLogoffSettings()) {
%>
<dl>
  <dt>
    <p><%=wiContext.getString("DescLogOut")%></p>
  </dt>
  <dd>
    <dl>
	  <dt class="nonLabel">
	    <ul>
	      <li>
            <input type="checkbox" id ="<%=Constants.ID_CHECK_LOGOFF%>"
                   class="marginAdjust"
                   name="<%=Constants.ID_CHECK_LOGOFF%>" value="<%=Constants.VAL_ON%>"
                   <%=workspaceControlSettings.getLogoffAppsCheckedStr()%>>
            <label id="<%=Constants.ID_LABEL_LOGOFF%>" for="<%=Constants.ID_CHECK_LOGOFF%>" class="noPush">
              <%=wiContext.getString("LogoffWIApps")%>
            </label>
            <%=UIUtils.generateHelpLinkMarkup(wiContext, "Help_LogOffExperience", "inlineHelpLink checkboxHelp")%>
            <div id='<%=UIUtils.getPopupId("Help_LogOffExperience")%>' class="wiPopup" ><%=wiContext.getString("Help_LogOffExperience")%></div>
          </li>
        </ul>
      </dt>
    </dl>
  </dd>
</dl>
<%
  }
%>    

<%
  if (Include.getWizardState(wiContext).getIncorrectZone()) {
%>
<dl>
  <dt>
    <p><%=wiContext.getString("AccessVirtualDesktopsAutomatically")%></p>
  </dt>
  <dd>
<%
    if (Include.isCompactLayout(wiContext)) {
%>
    <input type="button" id="addToTrustedSites" name="addToTrustedSites" title="<%=wiContext.getString("TipAddToTrustedSites")%>"
           onclick="location.replace('<%=Install.getChangeZoneLink()%>')"
           value="<%=wiContext.getString("AddToTrustedSitesButtonCaption")%>">
<%
    } else {
%>
    <div class="customButton">
      <a id="addToTrustedSites" name="addToTrustedSites" title="<%=wiContext.getString("TipAddToTrustedSites")%>"
         class="leftDoor" href="#" onclick="location.replace('<%=Install.getChangeZoneLink()%>')"
         ><span class="rightDoor"><%=wiContext.getString("AddToTrustedSitesButtonCaption")%></span></a>
    </div>
<%
    }
%>
  </dd>
</dl>
<% 
  } // end getIncorrectZone
  if (workspaceControlSettings.getShowReconnectButtonSettings()) {
%>
<dl>
  <dt>
    <p><%=wiContext.getString("DescReconnectButton")%></p>
  </dt>
  <dd>
    <dl>
      <dt>
        <input type="checkbox" id="<%=Constants.ID_CHECK_RECONNECT_BUTTON%>"
               class="marginAdjust"
               name="<%=Constants.ID_CHECK_RECONNECT_BUTTON%>" value="<%=Constants.VAL_ON%>"
               <%=workspaceControlSettings.getReconnectButtonCheckedStr()%>
               onClick="reconnect_clicked(this.form.<%=Constants.ID_CHECK_RECONNECT_BUTTON%>, this.form.<%=Constants.ID_OPTION_RECONNECT_BUTTON%>);">
        <label for="<%=Constants.ID_CHECK_RECONNECT_BUTTON%>" class="noPush">
          <%=wiContext.getString("ReconnectLabel")%>
        </label>
        <label for="<%=Constants.ID_OPTION_RECONNECT_BUTTON%>" class="screenReaderText">
          <%=wiContext.getString("SelectSessionsToReconnect")%>
        </label>
      </dt>
      <dd>
        <select name='<%=Constants.ID_OPTION_RECONNECT_BUTTON%>'
                id='<%=Constants.ID_OPTION_RECONNECT_BUTTON%>' size='1'>
          <option value='<%=Utils.toString(ReconnectOption.DISCONNECTED)%>' <%=workspaceControlSettings.getReconnectButtonDisconnectedSelectedStr()%>>
            <%=wiContext.getString("DisconnectedApplicationsOnly")%>
          </option>
          <option value='<%=Utils.toString(ReconnectOption.DISCONNECTED_AND_ACTIVE)%>'
            <%=workspaceControlSettings.getReconnectButtonAllSelectedStr()%>>
            <%=wiContext.getString("AllApplications")%>
          </option>
        </select>
        <%=UIUtils.generateHelpLinkMarkup(wiContext, "Help_Reconnect", "inlineHelpLink")%>
        <div id='<%=UIUtils.getPopupId("Help_Reconnect")%>' class="wiPopup" ><%=wiContext.getString("Help_Reconnect")%></div>
      </dd>
    </dl>
  </dd>
</dl>
<%
    } // end getShowReconnectButtonSettings
%>    
<%
}  // end getShowGeneralSection
%>