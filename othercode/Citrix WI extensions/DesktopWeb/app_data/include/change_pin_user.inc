<%
// change_pin_user.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<div class="twoFactorAuthPane">
  <ul id="twoFactorAuthForm">
    <li>
      <label id='lblPIN1' for='<%=TwoFactorAuth.ID_PIN1%>'>
        <%=wiContext.getString("NewPIN1")%>
      </label>
    <input type='password' name='<%=TwoFactorAuth.ID_PIN1%>' id='<%=TwoFactorAuth.ID_PIN1%>'
        class='loginEntries' maxlength='<%=Constants.PASSCODE_ENTRY_MAX_LENGTH%>'>
    </li>
    <li>
      <label id='lblPIN2' for='<%=TwoFactorAuth.ID_PIN2%>'>
        <%=wiContext.getString("NewPIN2")%>
      </label>
      <input type='password' name='<%=TwoFactorAuth.ID_PIN2%>' id='<%=TwoFactorAuth.ID_PIN2%>'
        class='loginEntries' maxlength='<%=Constants.PASSCODE_ENTRY_MAX_LENGTH%>'>
    </li>
  </ul>
</div> <% //twoFactorAuthPane %>

<div  class="buttonsPane" id="authButtons">
 <div>
  <input type='hidden' name='<%=Constants.ID_SUBMIT_MODE%>' value='<%=Constants.VAL_OK%>'>
    <input type='submit' name='btnOK' value='<%=wiContext.getString("OK")%>'
      title='<%=wiContext.getString("OKTooltip")%>' class='Cancel_OK'>
    <input type='button' name='btnCancel' value='<%=wiContext.getString("Cancel")%>'
      title='<%=wiContext.getString("CancelTooltip")%>'
      onclick="form.reset();form.<%=Constants.ID_SUBMIT_MODE%>.value='<%=Constants.VAL_CANCEL%>';form.submit();" class='Cancel_OK'>
 </div>
</div> <% // End buttonsPane %>