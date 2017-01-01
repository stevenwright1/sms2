<%
// account_ss_entry.inc
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<div class="accountSelfServicePane">
<%
if ( viewControl.getAllowedTasksCount() > 1 ) {
%>
  <p><%=wiContext.getString("ChooseTask")%></p>
  <input type="radio" name="<%=AccountSelfService.ID_TASK%>"
  value="<%=VAL_ACCOUNT_UNLOCK%>"
  id="<%=AccountSelfService.ID_RADIO_ACCOUNT_UNLOCK%>"
  checked
  >
  <label for="<%=AccountSelfService.ID_RADIO_ACCOUNT_UNLOCK%>"
    id="<%=AccountSelfService.ID_LABEL_ACCOUNT_UNLOCK%>"
    title="<%=wiContext.getString("UnlockAccountDesc")%>" class="taskLabel">
    <%=wiContext.getString("UnlockAccount")%>
  </label>
  <p class="taskDesc">
    <%=wiContext.getString("UnlockAccountDesc")%>
  </p>
  <input type="radio" name="<%=AccountSelfService.ID_TASK%>"
  value="<%=VAL_PASSWORD_RESET%>"
  id="<%=AccountSelfService.ID_RADIO_PASSWORD_RESET%>"
  >
  <label for="<%=AccountSelfService.ID_RADIO_PASSWORD_RESET%>"
  id="<%=AccountSelfService.ID_LABEL_PASSWORD_RESET%>"
  title="<%=wiContext.getString("ResetAccountPasswordDesc")%>" class="taskLabel">
      <%=wiContext.getString("ResetAccountPassword")%>
  </label>
  <p class="taskDesc">
      <%=wiContext.getString("ResetAccountPasswordDesc")%>
  </p>
<%
} else if ( viewControl.isTaskAllowed( com.citrix.wi.accountselfservice.AccountTask.ACCOUNT_UNLOCK ) ) {
%>
  <p><%=wiContext.getString("StartUnlockAccount")%></p>
<%
} else if ( viewControl.isTaskAllowed( com.citrix.wi.accountselfservice.AccountTask.PASSWORD_RESET ) ) {
%>
  <p><%=wiContext.getString("StartResetAccountPassword")%></p>
<%
}
%>
</div> <!-- End accountSelfServicePane -->
<div class="buttonsPane" id="authButtons">
  <div>
    <input type='hidden' name='<%=Constants.ID_SUBMIT_MODE%>' value='<%=Constants.VAL_OK%>'>
    <input type='submit' name='btnContinue' value='<%=wiContext.getString("Next")%>'
      title='<%=wiContext.getString("OKTooltip")%>' onclick='this.disabled=true;form.submit();' class='Cancel_OK'>
    <input type='button' name='btnCancel'  value='<%=wiContext.getString("Cancel")%>'
      title='<%=wiContext.getString("CancelTooltip")%>'
      onclick="form.reset();form.<%=Constants.ID_SUBMIT_MODE%>.value='<%=Constants.VAL_CANCEL%>';form.submit();" class='Cancel_OK'>
  </div>
</div>