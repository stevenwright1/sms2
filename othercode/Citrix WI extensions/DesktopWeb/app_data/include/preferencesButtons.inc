<%
// preferencesButtons.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<%
  if (Include.isCompactLayout(wiContext)) {
%>
<input type="submit" id="submit" name="submit" title="<%=wiContext.getString("TipSaveSettings")%>"
	   onclick="document.<%=Constants.ID_CITRIX_FORM%>.submit();"
	   value="<%=wiContext.getString("Save")%>">
<input type="submit" id="reset" name="reset" title="<%=wiContext.getString("TipCancelSettings")%>"
       onclick="clearFormData();document.<%=Constants.ID_CITRIX_FORM%>.<%=Constants.ID_SUBMIT_MODE%>.value='<%=Constants.VAL_CANCEL%>';document.<%=Constants.ID_CITRIX_FORM%>.submit();"
       value="<%=wiContext.getString("Cancel")%>">
<%
  } else {
%>
<div class="customButton">
  <a id="reset" name="reset" title="<%=wiContext.getString("TipCancelSettings")%>" class="leftDoor" 
     href="#" onclick="clearFormData();document.<%=Constants.ID_CITRIX_FORM%>.<%=Constants.ID_SUBMIT_MODE%>.value='<%=Constants.VAL_CANCEL%>';document.<%=Constants.ID_CITRIX_FORM%>.submit();"><span class="rightDoor"><%=wiContext.getString("Cancel")%></span></a>
</div>
<div class="customButton">
  <a id="submit" name="submit" title="<%=wiContext.getString("TipSaveSettings")%>" class="leftDoor" 
     href="#" onclick="document.<%=Constants.ID_CITRIX_FORM%>.submit();"><span class="rightDoor"><%=wiContext.getString("Save")%></span></a>
</div>
<%
  }
%>