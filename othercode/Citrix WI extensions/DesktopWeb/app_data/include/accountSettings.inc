<%
// accountSettings.inc
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<%
if (accountSettings.getShowPasswordSection()) {
%>
<h6 class="settingsHeader passwordSettings"><%=wiContext.getString("HeadPassword")%></h6>
<dl>  
  <dt><p><%=wiContext.getString("DescPassword")%></p></dt>
  <dd>
    <dl>
      <dt class="nonLabel">
<%
if (Include.isCompactLayout(wiContext)) {
%>
		<input type="button" id="changepwd" name="changepwd" title="<%=wiContext.getString("TipOKChangePassword")%>" 
		       onclick="location.replace('<%=Constants.PAGE_CHANGE_PASSWD%>?<%=Constants.QSTR_FROM_ACCOUNT_SETTINGS%>=<%=Constants.VAL_YES%>')"
		       value="<%=wiContext.getString("ChangePassword")%>">
<% 
} else { 
%>
        <div class="customButton">
		  <a id="changepwd" name="changepwd" title="<%=wiContext.getString("TipOKChangePassword")%>" 
		     class="leftDoor" href="#" 
		     onclick="location.replace('<%=Constants.PAGE_CHANGE_PASSWD%>?<%=Constants.QSTR_FROM_ACCOUNT_SETTINGS%>=<%=Constants.VAL_YES%>')"
		     ><span class="rightDoor"><%=wiContext.getString("ChangePassword")%></span></a>
		</div>
<%
}
%>
      </dt>
    </dl>
  </dd>
</dl>
<%
} //accountSettings.getShowPasswordSection
%>