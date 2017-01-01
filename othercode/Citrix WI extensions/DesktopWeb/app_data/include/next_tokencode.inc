<%
// next_tokencode.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<div class="twoFactorAuthPane">
  <ul id="twoFactorAuthForm">
    <li>
      <label id='lblPasscode' for='<%=TwoFactorAuth.ID_TOKENCODE%>'>
        <%=wiContext.getString("NextTokenText")%>
      </label>
    <input type='password' name='<%=TwoFactorAuth.ID_TOKENCODE%>' id='<%=TwoFactorAuth.ID_TOKENCODE%>'
        class='loginEntries' maxlength='<%=TwoFactorAuth.TOKENCODE_ENTRY_MAX_LENGTH%>'>
    </li>
  </ul>
</div> <% // twoFactorAuthPane %>
<div class="buttonsPane" id="authButtons">
  <div>
    <input type='hidden' name='<%=Constants.ID_SUBMIT_MODE%>' value='<%=Constants.VAL_OK%>'>
    <input type=submit value='<%= wiContext.getString("LogIn") %>'
      name=submit1 title="<%=wiContext.getString("TipLoginButton")%>"
      onClick="this.disabled=true;form.submit()"
      class='Cancel_LogIn'>
    <input type=button value='<%= wiContext.getString("Cancel") %>'
      onclick="form.reset();form.<%=Constants.ID_SUBMIT_MODE%>.value='<%=Constants.VAL_CANCEL%>';form.submit();"
      title="<%=wiContext.getString("CancelTooltip")%>"
      name=reset1 class="Cancel_LogIn">
  </div>
</div>
