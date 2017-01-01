<%
// changepassword.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<% if (viewControl.isConfirmationOnly()) { %>
<div class="twoFactorAuthPane">
  <p class="firstParagraph"><%=wiContext.getString("ChangePasswordChanged")%></p>
</div>
<div class="buttonsPane" id="authButtons">
  <div>
    <input type='hidden' name='<%=Constants.ID_SUBMIT_MODE%>' value='<%=Constants.VAL_CONFIRM%>'>
    <input type="submit"
      value='<%= wiContext.getString("OK") %>'
      name="submit1"
      class="Cancel_OK"
      title="<%=wiContext.getString("OKTooltip")%>"
     >
  </div>
</div>
<% } else { %>
<div class="changePasswordPane">
<div class="authPane">
  <dl class="pairPane">
    <dt>
      <label for="password">
        <%=wiContext.getString("ChangePasswordOld")%>
      </label>
    </dt>
    <dd>
      <input type="password" name="password" id="password"
          class="loginEntries" maxlength="254">
    </dd>
    <dt>
      <label for="passwordNew">
        <%=wiContext.getString("ChangePasswordNew")%>
        </label>
    </dt>
    <dd>
      <input type="password" name="passwordNew" id="passwordNew"
          class="loginEntries" maxlength="254">
    </dd>
    <dt>
      <label for='passwordConfirm'>
        <%=wiContext.getString("ChangePasswordConfirm")%>
      </label>
      </dt>
      <dd>
        <input type="password" name="passwordConfirm"
            id="passwordConfirm" class="loginEntries" maxlength="254">
      </dd>
    </dl>
<div class="spacer"></div>
</div> <% //Auth Pane %>
</div> <% //Change Password Pane %>
<div class="changePasswordButtons">
  <div>
    <input type='hidden' name='<%=Constants.ID_SUBMIT_MODE%>' value='<%=Constants.VAL_OK%>'>
    <input type="submit"
      value='<%= wiContext.getString("OK") %>'
      name="submit1"
      class="Cancel_OK"
      title="<%=wiContext.getString("TipOKChangePassword")%>"
     >
   <input type="button"
      value='<%= wiContext.getString("Cancel") %>'
      onclick="form.reset();form.<%=Constants.ID_SUBMIT_MODE%>.value='<%=Constants.VAL_CANCEL%>';form.submit();"
      title="<%=wiContext.getString("TipCancelChangePassword")%>"
      name="reset1" class="Cancel_OK">
  </div>
</div>
<% } %>