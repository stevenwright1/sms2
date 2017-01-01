<%
// loginMainFormFoot.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<%
if (viewControl.getShowPassword() && viewControl.getShowAccountSelfService()) {
%>
<tr><td colspan="2" class="loginButtonPane" id='<%=Constants.ID_ACCOUNTSS%>'>
    <a id="loginAcctSSLink" href="<%=Constants.PAGE_LOGIN%>?<%=Constants.QSTR_START_SELF_SERVICE%>=<%=Constants.VAL_ON%>"
       ><%=wiContext.getString(viewControl.getAccountSelfServiceLinkTextKey())%></a>
</td></tr>
<%
} //end getShowAccountSelfService
if (viewControl.getShowLoginButton()) {
%>
<tr><td colspan="2" class="loginButtonPane">
<%
  if (!Include.isCompactLayout(wiContext)) {
%>
      <div class="customButton loginButton"><a
           class="leftDoor" href="javascript:submitForm()" title="<%=wiContext.getString("TipLoginButton")%>"
           id="<%=Constants.ID_LOGIN_BTN%>"
           name="<%=Constants.ID_LOGIN_BTN%>"
        ><span class="rightDoor"><%=wiContext.getString("LogIn")%></span></a>
      </div>
<%
  } else {
%>
    <input id="<%=Constants.ID_LOGIN_BTN%>" type="submit" name="login"
          value='<%=wiContext.getString("LogIn")%>'
          title="<%=wiContext.getString("TipLoginButton")%>"
          onClick="this.disabled=true;form.submit()"
         <%=viewControl.getAllUIDisabledStr()%>>
<%
  }
}
%>
</td></tr>
</table>
