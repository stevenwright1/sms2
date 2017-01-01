<%
// displaySettings.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<%
if (viewControl.getShowGeneralSection()) {
%>
<h6 class="settingsHeader generalSettings"><%=wiContext.getString("HeadGeneral")%></h6>
<dl>
  <dt><p><%=wiContext.getString("DescGeneral")%></p></dt>
  <dd>
    <dl>
<%
if (displaySettings.getShowLanguageOptions()) {
%>
      <dt>
        <label for="<%=Constants.ID_OPTION_LANGUAGE%>"><%=wiContext.getString("Language")%></label>
      </dt>
	  <dd>
        <select name="<%=Constants.ID_OPTION_LANGUAGE%>"
                id="<%=Constants.ID_OPTION_LANGUAGE%>" size='1'>
<%
for ( int i=0; i < displaySettings.getNumLanguages(); i++ ) {
LanguagePack p = displaySettings.getLanguages()[i];
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
} // end getShowLanguageOptions

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
%>
      <dt class="nonLabel">
        <ul>
<%
if (displaySettings.getShowSearchDisplayOption()) {
%>
          <li>
            <input type="checkbox" id = "<%=Constants.ID_CHECK_SHOW_SEARCH%>"
                   class="marginAdjust"
                   name="<%=Constants.ID_CHECK_SHOW_SEARCH%>" value="<%=Constants.VAL_ON%>" 
                   <%=displaySettings.getShowSearchCheckedStr()%>>
            <label for="<%=Constants.ID_CHECK_SHOW_SEARCH%>" title="<%=wiContext.getString("TipShowSearch")%>">
              <%=wiContext.getString("ShowSearch")%>
            </label>
          </li>
<%
}

if (displaySettings.getShowHintsDisplayOption()) {
%>
          <li>
            <input type="checkbox" id = "<%=Constants.ID_CHECK_SHOW_HINTS%>"
                   class="marginAdjust"
                   name="<%=Constants.ID_CHECK_SHOW_HINTS%>" value="<%=Constants.VAL_ON%>" 
                   <%=displaySettings.getShowHintsCheckedStr()%>>
            <label for="<%=Constants.ID_CHECK_SHOW_HINTS%>" title="<%=wiContext.getString("TipShowHints")%>">
              <%=wiContext.getString("ShowHints")%>
            </label>
          </li>
<%
}

if (displaySettings.getShowRememberFolderOption()) {
%>
          <li>
            <input type="checkbox" id = "<%=Constants.ID_CHECK_REMEMBER_FOLDER%>"
                   class="marginAdjust"
                   name="<%=Constants.ID_CHECK_REMEMBER_FOLDER%>" value="<%=Constants.VAL_ON%>" 
                   <%=displaySettings.getRememberFolderCheckedStr()%>>
            <label for="<%=Constants.ID_CHECK_REMEMBER_FOLDER%>">
              <%=wiContext.getString("RememberFolder")%>
            </label>
            <%=UIUtils.generateHelpLinkMarkup(wiContext, "Help_Return", "inlineHelpLink checkboxHelp")%>
            <div id='<%=UIUtils.getPopupId("Help_Return")%>' class="wiPopup" ><%=wiContext.getString("Help_Return")%></div>
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