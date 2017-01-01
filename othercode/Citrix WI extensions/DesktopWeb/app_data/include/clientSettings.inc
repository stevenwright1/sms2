<%
// clientSettings.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<%
// CAUTION: make sure client settings HTML in this file and in loginSettings.inc
// are kept in synch.
if( clientSettings.getShowClientSettings() && viewControl.getShowGeneralSection() ) {
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
%>