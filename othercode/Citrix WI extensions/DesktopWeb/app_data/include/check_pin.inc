<%
// check_pin.inc
// Mark Peel add new page to citrix.
// Web Interface 5.4.0.0
%>  

<body onLoad="onLoadLayout">
<form method="post" action="check_pin.aspx" name="<%=Constants.ID_CITRIX_FORM%>" autocomplete="off">
<input type="hidden" name="<%=SessionToken.ID_FORM%>" value="<%=SessionToken.get(wiContext)%>" >

<div class="changePasswordPane">
	<div class="authPane">
	  <dl class="pairPane">
		<dt>
		  <label for="password">
			PIN :
		  </label>
		</dt>
		<dd>
		  <input type="text" name="<%=Constants.ID_PASSWORD%>" id="password"
			  class="loginEntries" maxlength="254">
		</dd>    
	  </dl>
	  <div class="spacer"></div>
	</div> <% //Auth Pane %>
</div> <% //Change Password Pane %>

<div class="changePasswordButtons">
	  <div>
		<input type='hidden' name='<%=Constants.ID_SUBMIT_MODE%>' value='<%=Constants.VAL_OK%>'>
	        
		<input type="submit"
		  value="OK"
		  name="submit1"
		  class="Cancel_OK"
		  title="OK"
		 >
		 <input type="button"
		  value="New PIN"
		  onclick="form.reset();form.<%=Constants.ID_SUBMIT_MODE%>.value='<%=Constants.VAL_CANCEL%>';form.submit();"
		  title="Request Another pincode"
		  name="reset1" class="Cancel_OK">
	  </div>	  
	  
</div>

</form>
</body>
