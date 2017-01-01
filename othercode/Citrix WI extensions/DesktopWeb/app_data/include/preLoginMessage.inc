<%
// preLoginMessage.inc
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>

<div class="preLoginPane">
    <p><%=viewControl.MessageString%></p>
</div>

<div class="buttonsPane" id="preLoginButtonPane">
  <div>
    <input type="submit" name="<%=Constants.VAL_YES%>" value="<%=viewControl.ButtonString%>"
      onClick="this.disabled=true;form.submit()">
  </div>
</div>

<div class="spacer"><!-- --></div>