<%
// viewStyleSettings.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<div>
  <div id="changeView">
    <ul>
      <li>
        <input type="radio" name="<%=Constants.ID_RADIO_COMPACT_VIEW_STYLE%>" id="radioViewStyleList"
          <%=viewControl.getListViewCheckedStr()%> value="<%=Constants.VAL_VIEW_STYLE_LIST%>">
        <label for="radioViewStyleList">
          <%=wiContext.getString("LowGraphicsListView")%>
        </label>
      </li>
      <li>
        <input type="radio" name="<%=Constants.ID_RADIO_COMPACT_VIEW_STYLE%>" id="radioViewStyleIcon"
          <%=viewControl.getIconsViewCheckedStr()%> value="<%=Constants.VAL_VIEW_STYLE_ICONS%>">
        <label for="radioViewStyleIcon">
          <%=wiContext.getString("LowGraphicsIconView")%>
        </label>
      </li>
    </ul>
  </div>
</div>

<div class="buttonsPane" id="settingsButtons">
  <input type="submit" value="<%=wiContext.getString("Save")%>"
    title="<%=wiContext.getString("TipSaveSettings")%>"
    id="submit1" name="submit1" class="Apply_Cancel_OK">
  <input type="button" value="<%=wiContext.getString("Cancel")%>"
    title="<%=wiContext.getString("TipCancelSettings")%>"
    onclick="clearFormData();form.<%=Constants.ID_SUBMIT_MODE%>.value='<%=Constants.VAL_CANCEL%>';form.submit();"
    id="reset1" name="reset1" class="Apply_Cancel_OK">
  <input type="hidden" name="<%=Constants.ID_SUBMIT_MODE%>" value="<%=Constants.VAL_OK%>">
</div>