<%
// change_pin_either.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<div class="twoFactorAuthPane">
  <p><%=wiContext.getString("ChoosePIN")%></p>
  <input type='radio' name='<%=TwoFactorAuth.ID_PIN_TYPE%>' id='rdPINTypeUser'
  value='<%=Constants.VAL_USER%>'>
  <label id='lblPINTypeUser' for='rdPINTypeUser'
    title="<%=wiContext.getString("TipPinTypeUser")%>" class="taskLabel">
    <%=wiContext.getString("PINUser")%>
  </label>
  <p class="taskDesc">
    <%=wiContext.getString("PINUserDesc")%>
  </p>
  <input type='radio' name='<%=TwoFactorAuth.ID_PIN_TYPE%>' id='rdPINTypeSystem'
    value='<%=Constants.VAL_SYSTEM%>' checked>
  <label id='lblPINTypeSystem' for='rdPINTypeSystem' class="taskLabel">
    <%=wiContext.getString("PINSystem")%>
  </label>
  <p class="taskDesc">
    <%=wiContext.getString("PINSystemDesc")%>
  </p>
</div>
<div class="buttonsPane" id="authButtons">
  <div>
    <input type='hidden' name='<%=Constants.ID_SUBMIT_MODE%>' value='<%=Constants.VAL_OK%>'>
    <input type='submit' name='btnOK' value='<%=wiContext.getString("OK")%>'
      title='<%=wiContext.getString("OKTooltip")%>' class='Cancel_OK'>
    <input type='button' name='btnCancel' value='<%=wiContext.getString("Cancel")%>'
      title='<%=wiContext.getString("CancelTooltip")%>'
      onclick="form.reset();form.<%=Constants.ID_SUBMIT_MODE%>.value='<%=Constants.VAL_CANCEL%>';form.submit();" class='Cancel_OK'>
  </div>
</div>