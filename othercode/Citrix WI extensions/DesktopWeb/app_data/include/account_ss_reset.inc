<%
// account_ss_reset.inc
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<%
if ( viewControl.getShowSuccessMessage() ) {
%>
<div class="accountSelfServicePane">
  <h6><%=wiContext.getString("PasswordWasReset")%></h6>
  <p><%=wiContext.getString("ResetPasswordInfo")%></p>
</div>
<div class="buttonsPane" id="authButtons">
  <div>
    <input type='submit' name='<%=AccountSelfService.ID_BUTTON_FINISH%>' id='<%=AccountSelfService.ID_BUTTON_FINISH%>' value='<%=wiContext.getString("ProceedToLogin")%>'
      title='<%=wiContext.getString("ProceedToLogin")%>'
      onclick="location.href='<%=Constants.PAGE_ACCOUNT_SS_UNLOCK%>?<%=Constants.QSTR_END_SELF_SERVICE%>=<%=Constants.VAL_ON%>'"
      class='Proceed_To_Login'>
  </div>
</div>
<%
} else {
%>
<div class="accountSelfServicePane">
    <ul id="accountSSForm">
      <li>
        <label for="<%=AccountSelfService.ID_NEW_PASSWORD%>">
            <%=wiContext.getString("ChangePasswordNew")%>
        </label>
        <input type="password" name="<%=AccountSelfService.ID_NEW_PASSWORD%>"
          id="<%=AccountSelfService.ID_NEW_PASSWORD%>" class="loginEntries" maxlength="254">
      </li>

      <li>
        <label for='<%=AccountSelfService.ID_CONFIRM_PASSWORD%>'>
          <%=wiContext.getString("ChangePasswordConfirm")%>
        </label>
        <input type="password" name="<%=AccountSelfService.ID_CONFIRM_PASSWORD%>"
          id="<%=AccountSelfService.ID_CONFIRM_PASSWORD%>" class="loginEntries" maxlength="254">
      </li>
    </ul>
  </div>
  <div class="buttonsPane" id="authButtons">
    <div>
      <input type='hidden' name='<%=Constants.ID_SUBMIT_MODE%>' value='<%=Constants.VAL_OK%>'>
      <input type='submit' name='<%=AccountSelfService.ID_BUTTON_FINISH%>' id='<%=AccountSelfService.ID_BUTTON_FINISH%>' value='<%=wiContext.getString("Finish")%>'
        title='<%=wiContext.getString("OKTooltip")%>' onclick='this.disabled=true;form.submit();' class='Cancel_OK'>
      <input type='button' name='btnCancel' value='<%=wiContext.getString("Cancel")%>'
        title='<%=wiContext.getString("CancelTooltip")%>'
        onclick="form.reset();form.<%=Constants.ID_SUBMIT_MODE%>.value='<%=Constants.VAL_CANCEL%>';form.submit();" class='Cancel_OK'>
    </div>
  </div>
<%
}
%>