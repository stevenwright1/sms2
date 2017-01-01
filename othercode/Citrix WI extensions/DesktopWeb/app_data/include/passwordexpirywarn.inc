<%
// passwordexpirywarn.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<div class="commonInnerBox">
  <p class="firstParagraph"><%=viewControl.expiryMessage%></p>
  <p class="secondParagraph"><%=wiContext.getString("PwdExpWarnQuestion")%></p>
</div>
<div class="buttonsPane" id="authButtons">
  <div>
    <input type="hidden" name="<%=Constants.ID_SUBMIT_MODE%>" value="<%=Constants.VAL_OK%>">
    <input type="submit" value="<%=wiContext.getString("Yes")%>"
      title="<%=wiContext.getString("TipOKChangePassword")%>"
      id="submit1" name="submit1" class="Cancel_OK">
    <input type="button" value="<%=wiContext.getString("No")%>"
      title="<%=wiContext.getString("TipNoPasswordChange")%>"
      onclick="form.<%=Constants.ID_SUBMIT_MODE%>.value='<%=Constants.VAL_CANCEL%>';form.submit();"
      id="reset1" name="reset1" class="Cancel_OK">
  </div>
</div>
<div class="spacer"></div>