<%
// password_challenge.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<div class="twoFactorAuthPane">
  <ul id="twoFactorAuthForm">
    <li>
        <label id='lblPassword' for='<%=TwoFactorAuth.ID_PASSWORD_CHALLENGE%>'>
            <%=wiContext.getString( "PasswordChallengeText" )%>
        </label>
       <input type='password' name='<%=TwoFactorAuth.ID_PASSWORD_CHALLENGE%>' id='<%=TwoFactorAuth.ID_PASSWORD_CHALLENGE%>'
        class='loginEntries' maxlength='<%=Constants.PASSWORD_ENTRY_MAX_LENGTH%>'>
    </li>
  </ul>
</div>
<div class="buttonsPane" id="authButtons">
  <div>
    <%
        // The buttons are named "submit1" and "reset1"; previously they were named "submit"
        // and "reset" which seemed to break them when used in AG embedded mode.
    %>
    <input type='hidden' name='<%=Constants.ID_SUBMIT_MODE%>' value='<%=Constants.VAL_OK%>'>
    <input type=submit value='<%= wiContext.getString( "LogIn" ) %>'
      name=submit1 title="<%=wiContext.getString( "TipLoginButton" )%>"
      onClick="this.disabled=true;form.submit()" class='Cancel_Login'>
    <input type=button value='<%= wiContext.getString( "Cancel" ) %>'
      onclick="form.reset();form.<%=Constants.ID_SUBMIT_MODE%>.value='<%=Constants.VAL_CANCEL%>';form.submit();"
      title="<%=wiContext.getString( "CancelTooltip" )%>"
      name=reset1 class="Cancel_Login">
  </div>
 </div>
