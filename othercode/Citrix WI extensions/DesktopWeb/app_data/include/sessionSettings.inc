<%
// sessionSettings.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<%
if (sessionSettings.getShowUserExperienceSection()) {
%>
<h6 class="settingsHeader userExperienceSettings"><%=wiContext.getString("HeadSectionPerformance")%></h6>
<%
if (sessionSettings.getShowBandwidthOptions() || sessionSettings.getShowColorSettings() || sessionSettings.getShowAudioSettings() || sessionSettings.getShowPrinterMapping()) {
%>
<dl>
  <dt>
    <p><%=wiContext.getString("DescSectionPerformance")%></p>
  </dt>
  <dd>
    <dl>
<%
if (sessionSettings.getShowBandwidthOptions()) {
%>
      <dt>
        <label id="lblBandwidth" for="<%=Constants.ID_OPTION_BANDWIDTH%>">
          <%=wiContext.getString("HeadConnectionSpeed")%>
        </label>
      </dt>
      <dd>
        <select name='<%=Constants.ID_OPTION_BANDWIDTH%>'
                id='<%=Constants.ID_OPTION_BANDWIDTH%>' size='1'
                onChange="onChangeBandwidth(this.form);">          
<%
	String customBandwidthStrKey = (sessionSettings.getShowBandwidthCustom()) ? "Custom" : "Default";
    String customBandwidth = wiContext.getString(customBandwidthStrKey);
%>
		  <option value='<%=Utils.toString(BandwidthProfilePreference.CUSTOM)%>'>
            <%=customBandwidth%>
          </option>
          <option value='<%=Utils.toString(BandwidthProfilePreference.LOW)%>'
            <%=sessionSettings.getBandwidthSelectedStr(BandwidthProfilePreference.LOW)%>>
            <%=wiContext.getString("LowBandwidth")%>
          </option>
          <option value='<%=Utils.toString(BandwidthProfilePreference.MEDIUM)%>'
            <%=sessionSettings.getBandwidthSelectedStr(BandwidthProfilePreference.MEDIUM)%>>
            <%=wiContext.getString("MediumBandwidth")%>
          </option>
          <option value='<%=Utils.toString(BandwidthProfilePreference.MEDIUM_HIGH)%>'
            <%=sessionSettings.getBandwidthSelectedStr(BandwidthProfilePreference.MEDIUM_HIGH)%>>
            <%=wiContext.getString("MediumHighBandwidth")%>
          </option>
          <option value='<%=Utils.toString(BandwidthProfilePreference.HIGH)%>'
            <%=sessionSettings.getBandwidthSelectedStr(BandwidthProfilePreference.HIGH)%>>
            <%=wiContext.getString("HighBandwidth")%>
          </option>
        </select>
        <%=UIUtils.generateHelpLinkMarkup(wiContext, "Help_Bandwidth", "inlineHelpLink")%>
        <div id='<%=UIUtils.getPopupId("Help_Bandwidth")%>' class="wiPopup" ><%=wiContext.getString(sessionSettings.getShowBandwidthCustom() ? "Help_BandwidthCustom" : "Help_Bandwidth")%></div><br>
      </dd>
<%
} //sessionSettings.getShowBandwidthOptions()

if (sessionSettings.getShowColorSettings()) {
%>
      <dt>
        <label id="lblWinColor" for="<%=Constants.ID_OPTION_WINDOW_COLOR%>">
         <%=wiContext.getString("WindowColor")%>
        </label>
      </dt>
      <dd>
        <select name="<%=Constants.ID_OPTION_WINDOW_COLOR%>"
                id="<%=Constants.ID_OPTION_WINDOW_COLOR%>" size="1">
          <option value="<%=ColorDepthPreference.NO_PREFERENCE%>">
            <%=wiContext.getString("Default")%>
          </option>
          <option value="<%=ColorDepthPreference.COLOR_HIGH%>"
            <%=sessionSettings.getColorSelectedStr(ColorDepthPreference.COLOR_HIGH)%>>
            <%=wiContext.getString("HighColor")%>
          </option>
          <option value="<%=ColorDepthPreference.COLOR_TRUE%>"
            <%=sessionSettings.getColorSelectedStr(ColorDepthPreference.COLOR_TRUE)%>>
            <%=wiContext.getString("TrueColor")%>
          </option>
        </select>
      </dd>
<%
} //sessionSettings.getShowColorSettings()

if (sessionSettings.getShowAudioSettings()) {
%>
      <dt>
        <label id="lblAudio" for="<%=Constants.ID_OPTION_AUDIO%>">
          <%=wiContext.getString("AudioQuality")%>
        </label>
      </dt>
      <dd>
        <select name="<%=Constants.ID_OPTION_AUDIO%>"
                id="<%=Constants.ID_OPTION_AUDIO%>" size="1">
          <option value="<%=AudioQualityPreference.NO_PREFERENCE%>">
            <%=wiContext.getString("Default")%>
          </option>
          <option value="<%=AudioQualityPreference.OFF%>"
            <%=sessionSettings.getAudioQualitySelectedStr(AudioQualityPreference.OFF)%>>
            <%=wiContext.getString("AudioOff")%>
          </option>
          <option value="<%=AudioQualityPreference.LOW%>"
            <%=sessionSettings.getAudioQualitySelectedStr(AudioQualityPreference.LOW)%>>
            <%=wiContext.getString("AudioLow")%>
          </option>
          <option value="<%=AudioQualityPreference.MEDIUM%>"
            <%=sessionSettings.getAudioQualitySelectedStr(AudioQualityPreference.MEDIUM)%>>
            <%=wiContext.getString("AudioMedium")%>
          </option>
          <option value="<%=AudioQualityPreference.HIGH%>"
            <%=sessionSettings.getAudioQualitySelectedStr(AudioQualityPreference.HIGH)%>>
            <%=wiContext.getString("AudioHigh")%>
          </option>
        </select>
      </dd>
<%
} //sessionSettings.getShowAudioSettings()

if (sessionSettings.getShowPrinterMapping()) {
%>
      <dt class="nonLabel">
        <ul>
          <li>
            <input type="checkbox" id = "<%=Constants.ID_CHECK_PRINTER%>"
                   class="marginAdjust"
                   name="<%=Constants.ID_CHECK_PRINTER%>" value="<%=Constants.VAL_ON%>"
                   <%=sessionSettings.getPrinterMappingCheckedStr()%>>
            <label id="lblPrinter" for="<%=Constants.ID_CHECK_PRINTER%>">
              <%=wiContext.getString("EnablePrinterMapping")%>
            </label>
          </li>
        </ul>
      </dt>
<%
} //sessionSettings.getShowPrinterMapping()
%>
    </dl>
  </dd>
</dl>
<%
} // sessionSettings.getShowBandwidthOptions() || sessionSettings.getShowColorSettings() || sessionSettings.getShowAudioSettings() || sessionSettings.getShowPrinterMapping()
%>
<dl>  
<%
if (sessionSettings.getShowWindowSizeSettings()) {
%>
  <dt>
    <p><%=wiContext.getString("DescSectionDisplay")%></p>
  </dt>
  <dd>
    <dl>
      <dt>
        <label for="<%=Constants.ID_OPTION_WINDOW_SIZE%>">
          <%=wiContext.getString("WindowSize")%>
        </label>
      </dt>
      <dd>
        <select name="<%=Constants.ID_OPTION_WINDOW_SIZE%>"
                id="<%=Constants.ID_OPTION_WINDOW_SIZE%>" size="1"
                onChange="click_standard_size(this.form);">
          <option value="<%=AppDisplaySizePreference.NO_PREFERENCE%>"
            <%=sessionSettings.getWindowSizeSelectedStr(AppDisplaySizePreference.NO_PREFERENCE)%>>
            <%=wiContext.getString("Default")%>
          </option>
          <option value="<%=AppDisplaySizePreference.SEAMLESS%>"
            <%=sessionSettings.getWindowSizeSelectedStr(AppDisplaySizePreference.SEAMLESS)%>>
            <%=wiContext.getString("Seamless")%>
          </option>
          <option value="<%=AppDisplaySizePreference.FULLSCREEN%>"
            <%=sessionSettings.getWindowSizeSelectedStr(AppDisplaySizePreference.FULLSCREEN)%>>
            <%=wiContext.getString("FullScreen")%>
          </option>
          <option value="<%=AppDisplaySizePreference.CUSTOM.getType()%>"
            <%=sessionSettings.getWindowSizeSelectedStr(AppDisplaySizePreference.CUSTOM)%>>
            <%=wiContext.getString("CustomDimension")%>
          </option>
          <option value="<%=AppDisplaySizePreference.PERCENT.getType()%>"
            <%=sessionSettings.getWindowSizeSelectedStr(AppDisplaySizePreference.PERCENT)%>>
            <%=wiContext.getString("PercentOfDisplay")%>
          </option>                
          <option value="<%=AppDisplaySizePreference.PIXELS_640X480%>"
            <%=sessionSettings.getWindowSizeSelectedStr(AppDisplaySizePreference.PIXELS_640X480)%>>
            640 X 480
          </option>
          <option value="<%=AppDisplaySizePreference.PIXELS_800X600%>"
            <%=sessionSettings.getWindowSizeSelectedStr(AppDisplaySizePreference.PIXELS_800X600)%>>
            800 X 600
          </option>
          <option value="<%=AppDisplaySizePreference.PIXELS_1024X768%>"
            <%=sessionSettings.getWindowSizeSelectedStr(AppDisplaySizePreference.PIXELS_1024X768)%>>
            1024 X 768
          </option>
          <option value="<%=AppDisplaySizePreference.PIXELS_1280X1024%>"
            <%=sessionSettings.getWindowSizeSelectedStr(AppDisplaySizePreference.PIXELS_1280X1024)%>>
            1280 X 1024
          </option>
          <option value="<%=AppDisplaySizePreference.PIXELS_1600X1200%>"
            <%=sessionSettings.getWindowSizeSelectedStr(AppDisplaySizePreference.PIXELS_1600X1200)%>>
            1600 X 1200
          </option>
        </select>
        <%=UIUtils.generateHelpLinkMarkup(wiContext, "Help_WindowSize", "inlineHelpLink")%>
        <div id='<%=UIUtils.getPopupId("Help_WindowSize")%>' class="wiPopup" ><%=wiContext.getString("Help_WindowSize")%></div>
      
        <ul>
          <li>
            <label for="<%=Constants.ID_TEXT_DESIRED_HRES%>" id="<%=Constants.ID_LABEL_WINSIZE_CUSTOM %>"
                   class='<%=sessionSettings.getLabelClass(Constants.ID_LABEL_WINSIZE_CUSTOM)%>'>
              <%=sessionSettings.getLabelPrefix(Constants.ID_LABEL_WINSIZE_CUSTOM)%><%=wiContext.getString("CustomSize")%>
            </label>
            <label for="<%=Constants.ID_TEXT_DESIRED_HRES%>" class="screenReaderText"><%=wiContext.getString("CustomWidth")%></label>
            <input name="<%=Constants.ID_TEXT_DESIRED_HRES%>" size="3" maxlength="4"
                   id="<%=Constants.ID_TEXT_DESIRED_HRES%>" onKeyPress="return numbersonly(this, event)"
                   value="<%=WebUtilities.escapeHTML(sessionSettings.getWindowHRes())%>">
            <span id="<%=Constants.ID_SPAN_WINSIZE_CUSTOM%>">x</span>
            <label for="<%=Constants.ID_TEXT_DESIRED_VRES%>" class="screenReaderText"><%=wiContext.getString("CustomHeight")%></label>
            <input name="<%=Constants.ID_TEXT_DESIRED_VRES%>" size="3" maxlength="4"
                   id="<%=Constants.ID_TEXT_DESIRED_VRES%>"
                   onKeyPress="return numbersonly(this, event)" value="<%=WebUtilities.escapeHTML(sessionSettings.getWindowVRes())%>">
          </li>
          <li>
            <label for="<%=Constants.ID_TEXT_SCREEN_PERCENT%>"
                   id="<%=Constants.ID_LABEL_WINSIZE_PERCENT%>"
                   class='<%=sessionSettings.getLabelClass(Constants.ID_LABEL_WINSIZE_PERCENT)%>'>
              <%=sessionSettings.getLabelPrefix(Constants.ID_LABEL_WINSIZE_PERCENT)%><%=wiContext.getString("PercentOfScreen")%>
            </label>
  
            <input name="<%=Constants.ID_TEXT_SCREEN_PERCENT%>" SIZE=3 MAXLENGTH=3
                   id="<%=Constants.ID_TEXT_SCREEN_PERCENT%>" onKeyPress="return numbersonly(this, event)"
                   value="<%=WebUtilities.escapeHTML(sessionSettings.getScreenPercent())%>">
            <span id="<%=Constants.ID_SPAN_WINSIZE_PERCENT%>">%</span>
            <span class="spacer"></span>
          </li>
        </ul>
      </dd>
    </dl>
  </dd>
</dl>
<%
}
  
if (sessionSettings.getShowKeyPassthroughSettings()) {
%>
<dl>
  <dt>
    <p><%=wiContext.getString("DescKeyboard")%></p>
  </dt>
  <dd>
    <dl>
      <dt>
        <label id="<%=Constants.ID_LABEL_KEY_PASSTHROUGH%>" for="<%=Constants.ID_OPTION_KEY_PASSTHROUGH%>">
          <%=wiContext.getString("KeyPassthroughLabel")%>
        </label>
      </dt>
      <dd>
        <select name="<%=Constants.ID_OPTION_KEY_PASSTHROUGH%>"
                id="<%=Constants.ID_OPTION_KEY_PASSTHROUGH%>" size="1">
          <option value="<%=KeyPassthroughOption.LOCAL%>"
            <%=sessionSettings.getKeyPassthroughSelectedStr(KeyPassthroughOption.LOCAL)%>>
            <%=wiContext.getString("KeyPassthroughLocal")%>
          </option>
          <option value="<%=KeyPassthroughOption.REMOTE%>"
            <%=sessionSettings.getKeyPassthroughSelectedStr(KeyPassthroughOption.REMOTE)%>>
            <%=wiContext.getString("KeyPassthroughRemote")%>
          </option>
          <option value="<%=KeyPassthroughOption.FULL_SCREEN_ONLY%>"
            <%=sessionSettings.getKeyPassthroughSelectedStr(KeyPassthroughOption.FULL_SCREEN_ONLY)%>>
            <%=wiContext.getString("KeyPassthroughFullscreenOnly")%>
          </option>
        </select>
        <%=UIUtils.generateHelpLinkMarkup(wiContext, "Help_WindowsKeys", "inlineHelpLink")%>
        <div id='<%=UIUtils.getPopupId("Help_WindowsKeys")%>' class="wiPopup" ><%=wiContext.getString("Help_WindowsKeys")%></div><br>
      </dd>
    </dl>
  </dd>
</dl>
<%
    } // sessionSettings.getShowKeyPassthroughSettings()
} // sessionSettings.getShowUserExperienceSection()

if (sessionSettings.getShowDevicesSection()) {
%>
<h6 class="settingsHeader devicesSettings"><%=wiContext.getString("HeadSectionLocalResources")%></h6>
<dl>
  <dt>
    <p><%=wiContext.getString("DescSectionLocalResources")%></p>
  </dt>
  <dd>
    <dl>
      <dt class="nonLabel">
        <ul>
<%
if (sessionSettings.getShowVirtualCOMPort()) {
%>
          <li>
            <input type="checkbox" id = "<%=Constants.ID_CHECK_VIRTUAL_COM_PORT%>"
                   class="marginAdjust"
                   name="<%=Constants.ID_CHECK_VIRTUAL_COM_PORT%>"
                   value="<%=Constants.VAL_ON%>"
                   <%=sessionSettings.getVirtualCOMPortCheckedStr()%>>
            <label id="lblVirtualCOMPort" for="<%=Constants.ID_CHECK_VIRTUAL_COM_PORT%>">
              <%=wiContext.getString("EnablePDADeviceSupport")%>
            </label>
          </li>
<%
} // sessionSettings.getShowVirtualCOMPortSetting()
%>
<%
    if (sessionSettings.getShowSpecialFolderRedirection()) {
%>
          <li>
            <input type="checkbox" id = "<%=Constants.ID_CHECK_SPECIALFOLDERREDIRECTION%>"
                   class="marginAdjust"
                   name="<%=Constants.ID_CHECK_SPECIALFOLDERREDIRECTION%>" value="<%=Constants.VAL_ON%>"
                   <%=sessionSettings.getSpecialFolderRedirectionCheckedStr()%>>
            <label id="lblSpecialFolderRedirection" for="<%=Constants.ID_CHECK_SPECIALFOLDERREDIRECTION%>">
              <%=wiContext.getString("EnableSpecialFolderRedirection")%>
            </label>
            <%=UIUtils.generateHelpLinkMarkup(wiContext, "Help_SFR", "inlineHelpLink checkboxHelp")%>
            <div id='<%=UIUtils.getPopupId("Help_SFR")%>' class="wiPopup" ><%=wiContext.getString("Help_SFR")%></div>
          </li>
<%
    }
%>
        </ul>
      </dt>
    </dl>
  </dd>
</dl>
<%
}
%>