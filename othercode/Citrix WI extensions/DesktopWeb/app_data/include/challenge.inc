<%
// challenge.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<div class="twoFactorAuthPane">
  <ul id="radiusAuthForm">
    <li>
      <label id='lblChallenge' for='<%=TwoFactorAuth.ID_CHALLENGE_RESPONSE%>' class="multiliner">
        <%=viewControl.getRadiusChallenge()%>
      </label>
     <input type='password' name='<%=TwoFactorAuth.ID_CHALLENGE_RESPONSE%>' id='<%=TwoFactorAuth.ID_CHALLENGE_RESPONSE%>'
        class='loginEntries' maxlength='<%=TwoFactorAuth.ID_CHALLENGE_RESPONSE_MAX_LENGTH%>'>
    </li>
  </ul>
</div> <% // twoFactorAuthPane %>

<div class="buttonsPane" id="authButtons">
  <div>
    <input type='hidden' name='<%=Constants.ID_SUBMIT_MODE%>' value='<%=Constants.VAL_OK%>'>
    <input type=submit value='<%= wiContext.getString("OK") %>'
      name=submit1 title="<%=wiContext.getString("OKTooltip")%>"
      class='Cancel_OK'>
    <input type=button value='<%= wiContext.getString("Cancel") %>'
      onclick="form.reset();form.<%=Constants.ID_SUBMIT_MODE%>.value='<%=Constants.VAL_CANCEL%>';form.submit();"
      title="<%=wiContext.getString("CancelTooltip")%>" name=reset1 class="Cancel_OK">
  </div>
</div>