<%
// loginSettings.inc
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<%
ClientSettingsControl clientSettings = viewControl.clientSettingsControl;

if (viewControl.getShowGeneralSection()) {
%>
<h6 class="settingsHeader generalSettings"><%=wiContext.getString("HeadGeneral")%></h6>
<dl>
<%
  CommonDisplaySettingsControl displaySettings = viewControl.getDisplaySettings();
  if (viewControl.showDisplaySection) {
%>
  <dt><p><%=wiContext.getString("DescGeneral")%></p></dt>
  <dd>
    <dl>
      <dt>
        <label for="<%=Constants.ID_OPTION_LANGUAGE%>"><%=wiContext.getString("Language")%></label>
      </dt>
	  <dd>
        <select name="<%=Constants.ID_OPTION_LANGUAGE%>"
                id="<%=Constants.ID_OPTION_LANGUAGE%>" size='1'>
<%
	LanguagePack[] languages = displaySettings.getLanguages();
    for ( int i=0; i < displaySettings.getNumLanguages(); i++ ) {
      LanguagePack p = languages[i];
%>
          <option value="<%=Locales.toString(p.getLocale())%>" <%=displaySettings.getLanguageSelectedStr(p)%>>
            <%=p.getFriendlyName()%>
          </option>
<%
    }
%>
        </select>
      </dd>
<%
	
	
    if (displaySettings.getShowLayoutSelection()) {
%>
      <dt>
        <label for="<%=Constants.ID_OPTION_LAYOUT_TYPE%>"><%=wiContext.getString("Layout")%></label>
      </dt>
      <dd>
        <select name="<%=Constants.ID_OPTION_LAYOUT_TYPE%>" id="<%=Constants.ID_OPTION_LAYOUT_TYPE%>"
                size='1'>
          <option value="<%=Constants.VAL_LAYOUT_TYPE_AUTO%>"
            <%=displaySettings.getLayoutAutoSelectedStr()%>>
            <%=wiContext.getString("Auto")%>
          </option>
          <option value="<%=Constants.VAL_LAYOUT_TYPE_NORMAL%>"
            <%=displaySettings.getLayoutNormalSelectedStr()%>>
            <%=wiContext.getString("FullGraphics")%>
          </option>
          <option value="<%=Constants.VAL_LAYOUT_TYPE_COMPACT%>"
            <%=displaySettings.getLayoutCompactSelectedStr()%>>
            <%=wiContext.getString("LowGraphics")%>
          </option>
        </select>
        <%=UIUtils.generateHelpLinkMarkup(wiContext, "Help_Layout", "inlineHelpLink")%>
        <div id='<%=UIUtils.getPopupId("Help_Layout")%>' class="wiPopup" ><%=wiContext.getString("Help_Layout")%></div>
      </dd>
<%
    } // end getShowLayoutSelection
  } // end showDisplaySection
%>
    </dl>
  </dd>
</dl>

<%  
  if (viewControl.showReconnectOptions) {
%>
<dl>
  <dt>
    <p><%=wiContext.getString("DescLogIn")%></p>
  </dt>
  <dd>
    <dl>
      <dt>
        <input type="checkbox" id ="<%=Constants.ID_CHECK_RECONNECT_LOGIN%>"
               class="marginAdjust"
               name="<%=Constants.ID_CHECK_RECONNECT_LOGIN%>" value="<%=Constants.VAL_ON%>"
               <%=viewControl.getReconnectCheckedStr()%> 
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
          <option value='<%=Constants.VAL_DISCONNECTED%>' <%=viewControl.getDisconnectedSelectedStr()%>>
            <%=wiContext.getString("DisconnectedApplicationsOnly")%>
          </option>
          <option value='<%=Constants.VAL_DISCONNECTED_ACTIVE%>'
            <%=viewControl.getAllSelectedStr()%>>
            <%=wiContext.getString("AllApplications")%>
          </option>
        </select>
        <%=UIUtils.generateHelpLinkMarkup(wiContext, "Help_LogOnExperience", "inlineHelpLink")%>
        <div id='<%=UIUtils.getPopupId("Help_LogOnExperience")%>' class="wiPopup" ><%=wiContext.getString("Help_LogOnExperience")%></div>
      </dd>
    </dl>
  </dd>
</dl>
<%
  } // end ShowReconnectOptions

  // CAUTION: make sure client settings HTML in this file and in clientSettings.inc
  // are kept in synch.
  if( clientSettings.getShowClientSettings() ) {
%>
<dl>
  <dt><p><%=wiContext.getString("DescClientSettings")%></p></dt>
  <dd>
    <dl>      
<%
    if(clientSettings.showAdvancedModeWizardLink) {
%>
      <dt id="<%=Constants.ID_DIV_REMOTE_CLIENTS%>" class="nonLabel">
        <p><%=wiContext.getString("CurrentlySelectedClient",
              wiContext.getString(Utils.toString(clientSettings.selectedClient)))%></p>
<%
      if (Include.isCompactLayout(wiContext)) {
%>
		<input type="button" id="runAdvancedWizard" name="runAdvancedWizard" title="<%=wiContext.getString("TipAdvancedWizardStart")%>"
		       onclick="location.replace('<%=Constants.PAGE_WIZARD_PRE_INPUT%>?<%=Constants.QSTR_SETTINGS%>=<%=Constants.VAL_TRUE%>')"
		       value="<%=wiContext.getString("AdvancedWizardButtonCaption")%>">
<% 
      } else { 
%>
        <div class="customButton">
          <a id="runAdvancedWizard" name="runAdvancedWizard" title="<%=wiContext.getString("TipAdvancedWizardStart")%>" 
	         class="leftDoor" href="#" onclick="location.replace('<%=Constants.PAGE_WIZARD_PRE_INPUT%>?<%=Constants.QSTR_SETTINGS%>=<%=Constants.VAL_TRUE%>')"
  	         ><span class="rightDoor"><%= wiContext.getString("AdvancedWizardButtonCaption") %></span></a>
  	    </div>
<% 
      } 
%>
      </dt>
<%
    } // clientSettings.showAdvancedModeWizardLink
%>
<%
    if(clientSettings.showAutoModeWizardLink) {
%>
      <dt id="<%=Constants.ID_DIV_REMOTE_CLIENTS%>" class="nonLabel">
        <p><%=wiContext.getString("CurrentlySelectedClient",
           wiContext.getString(Utils.toString(clientSettings.selectedClient)))%></p>
<%
      if (Include.isCompactLayout(wiContext)) {
%>
		<input type="button" id="runAutoWizard" name="runAutoWizard" title="<%=wiContext.getString("TipAutoWizardStart")%>" 
		       onclick="location.replace('<%=Constants.PAGE_WIZARD_PRE_INPUT%>?<%=Constants.QSTR_SETTINGS%>=<%=Constants.VAL_TRUE%>')"
		       value="<%=wiContext.getString("AutoWizardButtonCaption")%>">
<% 
      } else { 
%>
        <div class="customButton">
          <a id="runAutoWizard" name="runAutoWizard" title="<%=wiContext.getString("TipAutoWizardStart")%>" 
	         class="leftDoor" href="#" onclick="location.replace('<%=Constants.PAGE_WIZARD_PRE_INPUT%>?<%=Constants.QSTR_SETTINGS%>=<%=Constants.VAL_TRUE%>')"
	         ><span class="rightDoor"><%=wiContext.getString("AutoWizardButtonCaption")%></span></a>
    	</div>
<% 
      } 
%>
      </dt>
<%
    } // clientSettings.showAutoModeWizardLink
%>
<%
    if( clientSettings.showClientType ) {
%>
      <dt id="<%=Constants.ID_DIV_REMOTE_CLIENTS%>">
        <label for="<%=Constants.ID_OPTION_CLIENT_TYPE%>">
          <%=wiContext.getString("RemoteClientTypeSection")%>
        </label>
      </dt>
      <dd>
        <select name="<%=Constants.ID_OPTION_CLIENT_TYPE%>"
          id="<%=Constants.ID_OPTION_CLIENT_TYPE%>" size="1">
<%
      if (clientSettings.showICALocal) {
%>
          <option value="<%=Utils.toString(MPSClientType.LOCAL_ICA)%>"
            <%=clientSettings.getClientICALocalSelectedStr()%>>
            <%=wiContext.getString("Ica-Local")%>
          </option>
<%
      } // clientSettings.showICALocal
      if (clientSettings.showICAJava) {
%>
          <option value="<%=Utils.toString(MPSClientType.JAVA)%>"
            <%=clientSettings.getClientICAJavaSelectedStr()%>>
            <%=wiContext.getString("Ica-Java")%>
          </option>
<%
      } // clientSettings.showICAJava
      if (clientSettings.showRDPEmbedded) {
%>
          <option value="<%=Utils.toString(MPSClientType.EMBEDDED_RDP)%>"
            <%=clientSettings.getClientRDPEmbeddedSelectedStr()%>>
            <%=wiContext.getString("Rdp-Embedded")%>
          </option>
<%
      } // clientSettings.getShowRDPEmbedded
%>
        </select>
        <%=UIUtils.generateHelpLinkMarkup(wiContext, "Help_ClientType", "inlineHelpLink")%>
        <div id='<%=UIUtils.getPopupId("Help_ClientType")%>' class="wiPopup" ><%=clientSettings.clientTypeInlineHelp%></div>
      </dd> <% // end divRemoteClients %>
<%
    } // clientSettings.showClientType
%>
    </dl>
  </dd>
</dl>
<%
    if (clientSettings.showJicaPackages) {
%>
<dl>
  <dt id="<%=Constants.ID_DIV_JICA_PACKAGE%>">
    <p id="<%=Constants.ID_LABEL_JICA_PACKAGE%>">
      <%=wiContext.getString("OverrideJICAPackages")%>
      <%=UIUtils.generateHelpLinkMarkup(wiContext, "Help_JavaPackages", "inlineHelpLink")%>
	  <div id='<%=UIUtils.getPopupId("Help_JavaPackages")%>' class="wiPopup wiPopupWide" ><%=wiContext.getString("Help_JavaPackages")%></div>
    </p>
  </dt>
  <dd>
    <dl>
      <dt class="nonLabel">
        <ul>
          <li>
            <input type="checkbox" name="<%=Constants.ID_CHECK_JICA_AUDIO%>"
              class="marginAdjust"
              id="<%=Constants.ID_CHECK_JICA_AUDIO%>" value="<%=Constants.VAL_ON%>"
              <%=clientSettings.getJICAAudioCheckedStr()%>>
            
            <label for="<%=Constants.ID_CHECK_JICA_AUDIO%>"
              id="<%=Constants.ID_LABEL_JICA_AUDIO%>">
              <%=wiContext.getString("JICAAudio")%>
            </label>            
            
		    <%=UIUtils.generateHelpLinkMarkup(wiContext, "Help_JavaPackagesAudio", "inlineHelpLink")%>
			<div id='<%=UIUtils.getPopupId("Help_JavaPackagesAudio")%>' class="wiPopup wiPopupWide" ><%=wiContext.getString("Help_JavaPackagesAudio")%></div>
          </li>
          <li>
            <input type="checkbox" name="<%=Constants.ID_CHECK_JICA_CDM%>"
              class="marginAdjust"
              id="<%=Constants.ID_CHECK_JICA_CDM%>" value="<%=Constants.VAL_ON%>"
              <%=clientSettings.getJICACDMCheckedStr()%>>
 
            <label for="<%=Constants.ID_CHECK_JICA_CDM%>"
              id="<%=Constants.ID_LABEL_JICA_CDM%>">
              <%=wiContext.getString("JICACDM")%>
            </label>
            
            <%=UIUtils.generateHelpLinkMarkup(wiContext, "Help_JavaPackagesClientDriveMapping", "inlineHelpLink")%>
			<div id='<%=UIUtils.getPopupId("Help_JavaPackagesClientDriveMapping")%>' class="wiPopup wiPopupWide" ><%=wiContext.getString("Help_JavaPackagesClientDriveMapping")%></div>
          </li>
          <li>
            <input type="checkbox" name="<%=Constants.ID_CHECK_JICA_CLIPBOARD%>"
              class="marginAdjust"
              id="<%=Constants.ID_CHECK_JICA_CLIPBOARD%>" value="<%=Constants.VAL_ON%>"
              <%=clientSettings.getJICAClipboardCheckedStr()%>>
 
            <label for="<%=Constants.ID_CHECK_JICA_CLIPBOARD%>"
              id="<%=Constants.ID_LABEL_JICA_CLIPBOARD%>">
              <%=wiContext.getString("JICAClipboard")%>
            </label>
            
            <%=UIUtils.generateHelpLinkMarkup(wiContext, "Help_JavaPackagesClipboard", "inlineHelpLink")%>
			<div id='<%=UIUtils.getPopupId("Help_JavaPackagesClipboard")%>' class="wiPopup wiPopupWide" ><%=wiContext.getString("Help_JavaPackagesClipboard")%></div>
          </li>
          <li>
            <input type="checkbox" name="<%=Constants.ID_CHECK_JICA_CONFIGUI%>"
              class="marginAdjust"
              id="<%=Constants.ID_CHECK_JICA_CONFIGUI%>" value="<%=Constants.VAL_ON%>"
              <%=clientSettings.getJICAConfigUICheckedStr()%>>
 
            <label for="<%=Constants.ID_CHECK_JICA_CONFIGUI%>"
              id="<%=Constants.ID_LABEL_JICA_CONFIGUI%>">
              <%=wiContext.getString("JICAConfigUI")%>
            </label>
            
            <%=UIUtils.generateHelpLinkMarkup(wiContext, "Help_JavaPackagesConfigurationUI", "inlineHelpLink")%>
			<div id='<%=UIUtils.getPopupId("Help_JavaPackagesConfigurationUI")%>' class="wiPopup wiPopupWide" ><%=wiContext.getString("Help_JavaPackagesConfigurationUI")%></div>
          </li>
          <li>
            <input type="checkbox" name="<%=Constants.ID_CHECK_JICA_PRINTER%>"
              class="marginAdjust"
              id="<%=Constants.ID_CHECK_JICA_PRINTER%>" value="<%=Constants.VAL_ON%>"
              <%=clientSettings.getJICAPrinterCheckedStr()%>>
 
            <label for="<%=Constants.ID_CHECK_JICA_PRINTER%>"
              id="<%=Constants.ID_LABEL_JICA_PRINTER%>">
              <%=wiContext.getString("JICAPrinter")%>
            </label>
            
            <%=UIUtils.generateHelpLinkMarkup(wiContext, "Help_JavaPackagesPrinterMapping", "inlineHelpLink")%>
			<div id='<%=UIUtils.getPopupId("Help_JavaPackagesPrinterMapping")%>' class="wiPopup wiPopupWide" ><%=wiContext.getString("Help_JavaPackagesPrinterMapping")%></div>
          </li>
          <li>
            <input type="checkbox" name="<%=Constants.ID_CHECK_JICA_ZERO%>"
              class="marginAdjust"
              id="<%=Constants.ID_CHECK_JICA_ZERO%>" value="<%=Constants.VAL_ON%>"
              <%=clientSettings.getJICAZeroCheckedStr()%>>
 
            <label for="<%=Constants.ID_CHECK_JICA_ZERO%>"
              id="<%=Constants.ID_LABEL_JICA_ZERO%>">
              <%=wiContext.getString("JICAZero")%>
            </label>
            
            <%=UIUtils.generateHelpLinkMarkup(wiContext, "Help_JavaPackagesLocalTextEcho", "inlineHelpLink")%>
			<div id='<%=UIUtils.getPopupId("Help_JavaPackagesLocalTextEcho")%>' class="wiPopup wiPopupWide" ><%=wiContext.getString("Help_JavaPackagesLocalTextEcho")%></div>
          </li>
        </ul>
      </dd>
    </dl>
  </dd>
</dl>
<%
    } // clientSettings.showJICAPackages
  } // clientSettings.getShowClientSettings()
} // viewControl.getShowGeneralSection()

if (viewControl.showBandwidthOptions) {
%>
<h6 class="settingsHeader userExperienceSettings"><%=wiContext.getString("HeadSectionPerformance")%></h6>
<dl>
  <dt>
    <p><%=wiContext.getString("DescSectionPerformance")%></p>
  </dt>
  <dd>
    <dl>
      <dt>
        <label id="lblBandwidth" for="<%=Constants.ID_BANDWIDTH%>">
          <%=wiContext.getString("HeadConnectionSpeed")%>
        </label>
      </dt>
      <dd>
        <select name='<%=Constants.ID_BANDWIDTH%>'
                id='<%=Constants.ID_BANDWIDTH%>' size='1'>
<%
    String customBandwidthStrKey = viewControl.showBandwidthCustom ? "Custom" : "Default";
    String customBandwidth = wiContext.getString(customBandwidthStrKey);
%>
          <option value='<%=Utils.toString(BandwidthProfilePreference.CUSTOM)%>' 
            <%=viewControl.getCustomBandwidthSelectedStr()%>>
            <%=customBandwidth%>
          </option>
          <option value='<%=Utils.toString(BandwidthProfilePreference.LOW)%>'
            <%=viewControl.getLowBandwidthSelectedStr()%>>
            <%=wiContext.getString("LowBandwidth")%>
          </option>
          <option value='<%=Utils.toString(BandwidthProfilePreference.MEDIUM)%>'
            <%=viewControl.getMediumBandwidthSelectedStr()%>>
            <%=wiContext.getString("MediumBandwidth")%>
          </option>
          <option value='<%=Utils.toString(BandwidthProfilePreference.MEDIUM_HIGH)%>'
            <%=viewControl.getMediumHighBandwidthSelectedStr()%>>
            <%=wiContext.getString("MediumHighBandwidth")%>
          </option>
          <option value='<%=Utils.toString(BandwidthProfilePreference.HIGH)%>'
            <%=viewControl.getHighBandwidthSelectedStr()%>>
            <%=wiContext.getString("HighBandwidth")%>
          </option>
        </select>
        <%=UIUtils.generateHelpLinkMarkup(wiContext, "Help_Bandwidth", "inlineHelpLink")%>
        <div id='<%=UIUtils.getPopupId("Help_Bandwidth")%>' class="wiPopup" ><%=wiContext.getString(viewControl.showBandwidthCustom ? "Help_BandwidthCustomPreLogin" : "Help_Bandwidth")%></div><br>
      </dd>
    </dl>
  </dd>
  <span class="spacer"></span>
</dl>
<%
  } // end ShowBandwidthOptions
%>