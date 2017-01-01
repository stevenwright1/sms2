<%
// check_pin.inc
// Mark Peel add new page to citrix.
// Web Interface 5.4.0.0
%>  

<body onLoad="onLoadLayout">
<form method="post" action="check_pin.aspx" name="<%=Constants.ID_CITRIX_FORM%>" autocomplete="off">
<input type="hidden" name="<%=SessionToken.ID_FORM%>" value="<%=SessionToken.get(wiContext)%>" >

<div class="changePasswordPane">
	<div class="<%=viewControl.getTitleClass()%>">
		<h4 id="viewTitle" style="padding:5px 0px;">
			<%=viewControl.getTitle()%>
		</h4>
	</div>
	<div class="authPane">
	<%
		if (viewControl.getPinCodeEnabled() && (!viewControl.getPinCodeValidated())) {
	%>
		
		<dl class="pairPane">
			<dt>
				<label for="pincode">
				PIN Code :
				</label>
			</dt>
			<dd>
				<input type="text" name="pincode" id="pincode" class="loginEntries" maxlength="254"/>
			</dd>
		</dl>
		<div class="spacer"></div>
	<%
		} else {
	%>
		<input type="hidden" name="pincode" id="pincode" class="loginEntries" maxlength="254" value=""/>
	<%
		}
	%>
		<dl class="pairPane">
			<dt>
				<label for="password">
					PIN Generated :
				</label>
			</dt>
			<dd>
				<input type="hidden" name="state" id="state" class="loginEntries" maxlength="254" value="<%=viewControl.getState()%>"/>
				<input type="text" name="<%=Constants.ID_PASSWORD%>" id="password" class="loginEntries" maxlength="254"/>
			</dd>
		</dl>
		<div class="spacer"></div>
	</div> <% //Auth Pane %>
</div> <% //Change Password Pane %>

<div class="changePasswordButtons">
	  <div>
		<input type='hidden' name='<%=Constants.ID_SUBMIT_MODE%>' value='<%=Constants.VAL_OK%>'/>
		<input type="submit"
		  value="OK"
		  name="submit1"
		  class="Cancel_OK"
		  title="OK"
		 />
		<%
		if ((viewControl.getProviderLogic() == "CloudSMSProviderLogic") || (viewControl.getProviderLogic() == "EmailProviderLogic")) {
		%>
			<input type="button"
				value="New PIN"
				onclick="form.reset();form.<%=Constants.ID_SUBMIT_MODE%>.value='<%=Constants.VAL_CANCEL%>';form.submit();"
				title="Request Another pincode"
				name="reset1" class="Cancel_OK"/>
		<%
		} else {
		%>
		  
		<%
		}
		%>
	  </div>	  
	  
</div>

</form>
</body>
