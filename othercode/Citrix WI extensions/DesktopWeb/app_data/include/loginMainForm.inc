<%
// loginMainForm.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<%
ClientInfo clientInfo = wiContext.getClientInfo();
%>

<%
// need to do the hidden field first so it doesn't
// invalidate the table layout below
if (!viewControl.getShowLoginTypeOptions()) {
%>
    <input type="hidden" name="<%=Constants.ID_LOGIN_TYPE%>"
<% // LoginType set to Explicit, but only if it is allowed
   if (viewControl.getExplicitDisabled()) {
%>
      value="">
<% } else { %>
      value="<%=WIAuthType.EXPLICIT%>">
<% } %>
<%
}
%>

<table class="loginForm">
<%
if (viewControl.getShowLoginTypeOptions()) {
%>
<tr><td class='labelCell'>
      <label id='lblLoginType' for='<%=Constants.ID_LOGIN_TYPE%>'
          <%=viewControl.getAllUIDisabledStr()%> >
        <%=wiContext.getString("LoginType")%>
      </label>
</td>
<% if(Include.isCompactLayout(wiContext)){%>
</tr><tr>
<%}%>
<td>
      <select name='<%=Constants.ID_LOGIN_TYPE%>' id='<%=Constants.ID_LOGIN_TYPE%>' <%=viewControl.getAllUIDisabledStr()%>
        class='loginTypeOptionEntries'
        size='1' onchange='onChangeLoginType(this.form)'>
<%
    if( viewControl.allowedLogonModes().contains( WIAuthType.ANONYMOUS ) ) {
%>
        <option value='<%=WIAuthType.ANONYMOUS%>' <%=viewControl.getAnonymousSelectedStr()%>>
          <%=wiContext.getString("AnonymousUser")%>
        </option>
<%
    }
    if( viewControl.allowedLogonModes().contains( WIAuthType.CERTIFICATE_SINGLE_SIGN_ON ) ) {
%>
        <option value='<%=WIAuthType.CERTIFICATE_SINGLE_SIGN_ON%>'  <%=viewControl.getCertificateSSONSelectedStr()%>>
          <%=wiContext.getString("CertificateSSONUser")%>
        </option>
<%
    }
    if( viewControl.allowedLogonModes().contains( WIAuthType.CERTIFICATE ) ) {
%>
        <option value='<%=WIAuthType.CERTIFICATE%>' <%=viewControl.getCertificateSelectedStr()%>>
          <%=wiContext.getString("CertificateUser")%>
        </option>
<%
    }
    if( viewControl.allowedLogonModes().contains( WIAuthType.SINGLE_SIGN_ON ) ) {
%>
        <option value='<%=WIAuthType.SINGLE_SIGN_ON%>' <%=viewControl.getSSONSelectedStr()%>>
          <%=wiContext.getString("SingleSignOnUser")%>
        </option>
<%
    }
    if( viewControl.allowedLogonModes().contains( WIAuthType.EXPLICIT ) ) {
%>
        <option value='<%=WIAuthType.EXPLICIT%>' <%=viewControl.getExplicitSelectedStr()%>>
          <%=wiContext.getString("ExplicitUser")%>
        </option>
<%
    }
%>
    </select>
</td>
</tr>
<% // End LoginType
} //End viewControl.getShowLoginTypeOptions
%>

<tr><td class='labelCell'>
      <label id='lblUserName' for='<%=Constants.ID_USER%>'
        <%=viewControl.getExplicitDisabledStr()%>
        class='<%=viewControl.getLabelClass(Constants.ID_USER)%>'
        >
        <%=viewControl.getLabelPrefix(Constants.ID_USER)%><%=wiContext.getString("UserName")%>
      </label>
</td>
<% if(Include.isCompactLayout(wiContext)){%>
</tr><tr>
<% }%>
<td>
      <input type='text' name='<%=Constants.ID_USER%>' id='<%=Constants.ID_USER%>'
        class='loginEntries<%=viewControl.getExplicitDisabled()?" loginEntriesDisabled":""%>'
        maxlength='<%=Constants.LOGIN_ENTRY_MAX_LENGTH%>' <%=viewControl.getExplicitDisabledStr()%>
<%
if (viewControl.getUserName() != null) {
%>
        value='<%=WebUtilities.escapeHTML(viewControl.getUserName())%>'
<%
}
%>
        onkeyup='onUsernameTextEntry(this.form)'
        onChange='onUsernameTextEntry(this.form)'>
</td></tr>
<% // End Username
if (viewControl.getShowPassword()) {
%>
<tr><td class='labelCell'>
      <label id='lblPasswd' for='<%=Constants.ID_PASSWORD%>'
        <%=viewControl.getExplicitDisabledStr()%> >
        <%=wiContext.getString("Password")%>
      </label>
</td>
<% if(Include.isCompactLayout(wiContext)){%>
</tr><tr>
<% }%>
<td>
      <input type='password' name='<%=Constants.ID_PASSWORD%>' id='<%=Constants.ID_PASSWORD%>'
        class='loginEntries<%=viewControl.getExplicitDisabled()?" loginEntriesDisabled":""%>'
        maxlength='<%=Constants.LOGIN_ENTRY_MAX_LENGTH%>'
        <%=viewControl.getExplicitDisabledStr()%>>
</td></tr>
<% // End Password
} else if (viewControl.getShowPasscode()) { //if passcode is visible, but password is not, then replace the password field with the passcode field
%>
<tr><td class='labelCell'>
      <label id='lblPasscode' for='<%=Constants.ID_PASSCODE%>'
          <%=viewControl.getExplicitDisabledStr()%>>
          <%=wiContext.getString("Passcode")%>
      </label>
</td>
<% if(Include.isCompactLayout(wiContext)){%>
</tr><tr>
<% }%>
<td>
      <input type='password' name='<%=Constants.ID_PASSCODE%>' id='<%=Constants.ID_PASSCODE%>'
        class='loginEntries<%=viewControl.getExplicitDisabled()?" loginEntriesDisabled":""%>'
        maxlength='<%=Constants.LOGIN_ENTRY_MAX_LENGTH%>' <%=viewControl.getExplicitDisabledStr()%>>
</td>
</tr>
<% // End Passcode
}
if (viewControl.getShowDomain()) {
%>
<tr><td class='labelCell'>
      <label id='lblDomain' for='<%=Constants.ID_DOMAIN%>'
        <%=viewControl.getDomainDisabledStr()%>
        class='<%=viewControl.getLabelClass(Constants.ID_DOMAIN)%>'
      >
        <%=viewControl.getLabelPrefix(Constants.ID_DOMAIN)%><%=wiContext.getString("Domain")%>
      </label>
</td>
<% if(Include.isCompactLayout(wiContext)){%>
</tr><tr>
<% }%>
<td>
  <% if (viewControl.getNumLoginDomainSelection() > 0) { %>
    <select size='1'
        class='loginTypeOptionEntries<%=viewControl.getDomainDisabled()?" loginEntriesDisabled":""%>'
        name='<%=Constants.ID_DOMAIN%>' id='<%=Constants.ID_DOMAIN%>'
    <%=viewControl.getDomainDisabledStr()%>>
    <%
        for (int i=0; i < viewControl.getNumLoginDomainSelection(); i++) {
        String domainName = viewControl.getLoginDomainSelection()[i];
    %>
        <option value='<%=WebUtilities.escapeHTML(domainName)%>' <%=viewControl.getDomainSelectedStr(domainName)%>>
            <%=WebUtilities.escapeHTML(domainName)%>
        </option>
    <% } %>
    </select>
  <% } else { %>
    <input type='text'
        class='loginEntries<%=viewControl.getDomainDisabled()?" loginEntriesDisabled":""%>'
        name='<%=Constants.ID_DOMAIN%>' id='<%=Constants.ID_DOMAIN%>'
        MAXLENGTH='<%=Constants.LOGIN_ENTRY_MAX_LENGTH%>'
        <%=viewControl.getDomainDisabledStr()%>
    <% if (viewControl.getDomain() != null) { %>
        value='<%=WebUtilities.escapeHTML(viewControl.getDomain())%>'
    <% } %>
    >
  <% } %>
</td>
</tr>
<% } // End Domain
if (viewControl.getNDSEnabled()) {
    if (viewControl.getNumNDSContexts() > 0 || viewControl.getShowFindContext()) {
%>
<tr><td class='labelCell'>
      <label id='lblContext' for='<%=Constants.ID_CONTEXT%>'
        class='<%=viewControl.getLabelClass(Constants.ID_CONTEXT)%>'
        >
        <%=viewControl.getLabelPrefix(Constants.ID_CONTEXT)%><%=wiContext.getString("Context")%>
      </label>
</td>
<% if(Include.isCompactLayout(wiContext)){%>
</tr><tr>
<% }%>
<td>
      <select name='<%=Constants.ID_CONTEXT%>' id='<%=Constants.ID_CONTEXT%>'
        class='loginTypeOptionEntries' size='1'>
<%
        // Find context is always at the top of the list if it is present
        // Only selected if there are no found contexts being shown
        if (viewControl.getShowFindContext()) {
%>
        <option <%=(viewControl.getNumNDSContexts() > 0) ? "" : "selected"%>>
          <%=wiContext.getString("lookupContextString")%>
        </option>
<%
        }
        if (viewControl.getNumNDSContexts() > 0) {
            for (int j=0; j < viewControl.getNumNDSContexts(); j++) {
%>
        <option <%=(j == 0) ? "selected" : ""%>
            value='<%=WebUtilities.escapeHTML(viewControl.getNDSContexts()[j])%>'>
            <%=WebUtilities.escapeHTML(viewControl.getNDSContexts()[j])%>
        </option>
<%
            }
        }
%>
    </select>
</td>
</tr>
<%
}
%>
<tr><td class='labelCell'>
     <label id='lblTree' for='<%=Constants.ID_TREE%>'>
      <%=wiContext.getString("NDSTree")%>
      </label>
</td>
<% if(Include.isCompactLayout(wiContext)){%>
</tr><tr>
<% }%>
<td>
      <input type='text' readonly name='<%=Constants.ID_TREE%>' id='<%=Constants.ID_TREE%>'
          class='loginTreeEntry' value='<%=WebUtilities.escapeHTML(viewControl.getNDSTree())%>'>
</td>
</tr>
<% // End NDS Tree
}

if (viewControl.getShowPasscode() && viewControl.getShowPassword() ) { //only display the passcode field here if the password field is displayed above
%>
<tr><td class='labelCell'>
      <label id='lblPasscode' for='<%=Constants.ID_PASSCODE%>'
          <%=viewControl.getExplicitDisabledStr()%> >
          <%=wiContext.getString("Passcode")%>
      </label>
</td>
<% if (Include.isCompactLayout(wiContext)) { %>
</tr><tr>
<% } %>
<td>
      <input type='password' name='<%=Constants.ID_PASSCODE%>' id='<%=Constants.ID_PASSCODE%>' 
        class='loginEntries<%=viewControl.getExplicitDisabled()?" loginEntriesDisabled":""%>'
        maxlength='<%=Constants.LOGIN_ENTRY_MAX_LENGTH%>' <%=viewControl.getExplicitDisabledStr()%>>
</td>
</tr>
<% // End Passcode
}
%>
