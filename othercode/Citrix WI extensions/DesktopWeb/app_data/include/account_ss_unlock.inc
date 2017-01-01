<%
// account_ss_unlock.inc
// Copyright (c) 2005 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<div class="accountSelfServicePane">
  <h6><%=wiContext.getString("AccountUnlocked")%></h6>
  <p><%=wiContext.getString("AccountUnlockedInfo")%></p>
</div>
<div class="buttonsPane" id="authButtons">
  <div>
    <input type='submit' name='<%=AccountSelfService.ID_BUTTON_FINISH%>' id='<%=AccountSelfService.ID_BUTTON_FINISH%>' value='<%=wiContext.getString("ProceedToLogin")%>'
      title='<%=wiContext.getString("ProceedToLogin")%>'
      class='Proceed_To_Login'>
  </div>
</div>