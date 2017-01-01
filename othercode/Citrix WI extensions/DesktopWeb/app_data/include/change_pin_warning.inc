<%
// change_pin_warning.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<div class="twoFactorAuthPane">
  <p class="firstParagraph"><%=wiContext.getString("PINExpired")%></p>
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