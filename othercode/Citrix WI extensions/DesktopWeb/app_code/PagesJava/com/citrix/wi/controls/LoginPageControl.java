/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.controls;

import java.util.HashSet;
import java.util.Set;
import java.util.Vector;

import com.citrix.wi.pageutils.Markup;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.types.WIAuthType;


/**
 * Maintains presentation state for the Login Page.
 */
public class LoginPageControl extends PageControl {
    private boolean showLoginTypeOptions = false;
    private boolean showAccountSelfService = false;
    private WIAuthType selectedLogonMode = WIAuthType.EXPLICIT;
    private Set allowedLogonModes = new HashSet();

    private boolean showPasscode = false;
    private boolean showPassword = true;
    private boolean showLoginButton = true;

    private boolean explicitDisabled = false;

    private boolean domainDisabled = false;
    private boolean restrictDomains = false;
    private boolean showDomain = false;
    private String[] loginDomainSelection = new String[0];
    private String[] loginDomains = new String[0];
    // Store the value submitted in the domain field
    private String domain = null;
    // Store the value submitted in the domain dropdown
    private String loginDomainPreference = null;

    // Store the value submitted in the username field
    private String userName = null;
    private boolean NDSEnabled = false;
    private String NDSTree = "";
    private String[] NDSContexts = new String[0];
    private boolean showFindContext = false;

    private boolean allUIDisabled = false;

    private String assLinkTextKey = "";

    /**
     * Tests whether to show the available Login types.
     * @return <code>true</code> if they should be shown, else <code>false</code>
     */
    public boolean getShowLoginTypeOptions()
    {
        return showLoginTypeOptions;
    }
    /**
     * Sets whether to show the available Login types.
     * @param value <code>true</code> if they should be shown, else <code>false</code>
     */
    public void setShowLoginTypeOptions( boolean value ) {
        showLoginTypeOptions = value;
    }

    /**
     * Tests whether the account self service link should be shown.
     * @return <code>true</code> if shown, else <code>false</code>
     */
    public boolean getShowAccountSelfService () {
        return showAccountSelfService;
    }

    /**
     * Sets whether the account self service link should be shown.
     * @param value <code>true</code> if shown, else <code>false</code>
     */
    public void setShowAccountSelfService (boolean value) {
        showAccountSelfService = value;
    }

    /**
     * Gets the key for the text in the account self service link
     * @return the key for the text
     */
    public String getAccountSelfServiceLinkTextKey() {
        return assLinkTextKey;
    }

    /**
     * Sets what text to display in the account self service link
     * @param the key for the text
     */
    public void setAccountSelfServiceLinkTextKey(String text) {
        this.assLinkTextKey = text;
    }

    /**
     * Tests whether to show the Passcode field.
     * @return <code>true</code> if it should be shown, else <code>false</code>
     */
    public boolean getShowPasscode() {
        return showPasscode;
    }

    /**
     * Sets whether to show the Passcode field.
     * @param value <code>true</code> if it should be shown, else <code>false</code>
     */
    public void setShowPasscode( boolean value) {
        showPasscode = value;
    }

    /**
     * Gets the selected Logon mode.
     * @return the selected mode
     */
    public WIAuthType getSelectedLogonMode() {
        return selectedLogonMode;
    }

    /**
     * Sets the selected Logon mode.
     * @param value the selected mode
     */
    public void setSelectedLogonMode( WIAuthType value ) {
        selectedLogonMode = value;
    }

    /**
     * Gets the allowed Logon modes.
     * @return a <code>Set</code> of modes
     */
    public Set allowedLogonModes() {
        return allowedLogonModes;
    }

    /**
     * Tests whether to disable the explicit credentials fields.
     * @return <code>true</code> if disabled, else <code>false</code>
     */
    public boolean getExplicitDisabled() {
        return explicitDisabled;
    }

    /**
     * Sets whether to disable the explicit credentials fields.
     * @param value <code>true</code> if disabled, else <code>false</code>
     */
    public void setExplicitDisabled( boolean value ) {
        explicitDisabled = value;
    }


    /**
     * Tests whether to disable the domain field.
     * @return <code>true</code> if disabled, else <code>false</code>
     */
    public boolean getDomainDisabled() {
        return domainDisabled;
    }

    /**
     * Sets whether to disable the domain field.
     * @param value <code>true</code> if disabled, else <code>false</code>
     */
    public void setDomainDisabled( boolean value ) {
        domainDisabled = value;
    }

    /**
     * Tests whether to restrict the domains.
     * @return <code>true</code> if restricted, else <code>false</code>
     */
    public boolean getRestrictDomains() {
        return restrictDomains;
    }

    /**
     * Sets whether to restrict the domains.
     * @param value <code>true</code> if restricted, else <code>false</code>
     */
    public void setRestrictDomains( boolean value ) {
        restrictDomains = value;
    }

    /**
     * Tests whether to show the domain.
     * @return <code>true</code> if shown, else <code>false</code>
     */
    public boolean getShowDomain() {
        return showDomain;
    }

    /**
     * Tests whether to show inline help for any element in the login form.
     * @param wiContext WI context object.
     * @return <code>true</code> if inline help is shown for any login form element, else <code>false</code>
     */
    public boolean getShowAnyInlineHelp(WIContext wiContext) {
        boolean showPasswordInlineHelp = getShowPasscode() && getShowPassword();
        boolean showDomainInlineHelp = getShowDomain();

        // help is never shown in compact layout
        boolean showAnyInlineHelp = (showPasswordInlineHelp || showDomainInlineHelp) && !Include.isCompactLayout(wiContext);
        return showAnyInlineHelp;
    }

    /**
     * Sets whether to show the domain.
     * @param value <code>true</code> if shown, else <code>false</code>
     */
    public void setShowDomain( boolean value ) {
        showDomain = value;
    }

    /**
     * Gets the available domains.
     * @return an array of domain names
     */
    public String[] getLoginDomainSelection() {
        return loginDomainSelection;
    }

    /**
     * Gets the number available domains.
     * @return the number available domains
     */
    public int getNumLoginDomainSelection() {
        return loginDomainSelection.length;
    }

    /**
     * Gets the domain that should appear selected.
     *
     * @return a <code>String</code> object
     */
    public String getLoginDomainPreference() {
        return loginDomainPreference;
    }

    /**
     * Sets the domain that should appear selected.
     *
     * @param domainPref a <code>String</code> object
     */
    public void setLoginDomainPreference( String domainPref ) {
        loginDomainPreference = domainPref;
    }

    /**
    * Gets the string that controls whether the domain
    * appears selected.
    *
    * @param domainPreference the domain for which to return the string
    * @return either "selected" or the empty string, depending on whether the
    * given domain should appear selected
    */
    public String getDomainSelectedStr(String domainPreference){
        boolean b = false;
        if ((loginDomainPreference != null) && (loginDomainPreference.equals(domainPreference)))
        {
            b = true;
        }
        return Markup.selectedStr(b);
    }
    /**
     * Sets the available domains.
     * @param value a <code>Vector</code> of domain names
     */
    public void setLoginDomainSelection( Vector value ) {
        loginDomainSelection = new String[value.size()];
        for( int i = 0; i < value.size(); ++i ) {
            loginDomainSelection[i] = (String)value.elementAt(i);
        }
    }

    /**
     * Sets the available domains.
     * @param value an array of domain names
     */
    public void setLoginDomainSelection( String[] value ) {
        loginDomainSelection = value;
    }

    /**
     * Gets the available domains.
     * @return an array of domain names
     */
    public String[] getLoginDomains() {
        return loginDomains;
    }

    /**
     * Gets the number available domains.
     * @return the number available domains
     */
    public int getNumLoginDomains() {
        return loginDomains.length;
    }

    /**
     * Sets the available domains.
     * @param value a <code>Vector</code> of domain names
     */
    public void setLoginDomains( Vector value ) {
        loginDomains = new String[value.size()];
        for( int i = 0; i < value.size(); ++i ) {
            loginDomains[i] = (String)value.elementAt(i);
        }
    }

    /**
     * Sets the available domains.
     * @param value an array of domain names
     */
    public void setLoginDomains( String[] value ) {
        loginDomains = value;
    }

    /**
     * Gets the value for the domain field.
     * @return the domain or <code>null</code> if no default is to be used
     */
    public String getDomain()
    {
        return domain;
    }

    /**
     * Sets the value for the domain field.
     * @param domain the domain
     */
    public void setDomain(String domain)
    {
        this.domain = domain;
    }

    /**
     * Tests whether domains are disabled.
     * @return <code>disabled</code> or an empty string
     */
    public String getDomainDisabledStr()
    {
        return Markup.disabledStr(allUIDisabled || domainDisabled);
    }

    /**
     * Sets the value for the user and domain form fields.
     * @param user the user
     * @param domain the domain
     */
    public void setUserAndDomain(String user, String domain)
    {
        setUserName(user);
        setDomain(domain);
    }



    /**
     * Gets the user name to default to if any.
     * @return the user name or <code>null</code> if no default is to be used
     */
    public String getUserName() {
        return userName;
    }

    /**
     * Sets the username to default to.
     * @param userName the username
     */
    public void setUserName(String userName) {
        this.userName = userName;
    }

    /**
     * Tests whether NDS login is enabled.
     * @return <code>true</code> if enabled, else <code>false</code>
     */
    public boolean getNDSEnabled() {
        return NDSEnabled;
    }

    /**
     * Sets whether NDS login is enabled.
     * @param value <code>true</code> if enabled, else <code>false</code>
     */
    public void setNDSEnabled( boolean value ) {
        NDSEnabled = value;
    }

    /**
     * Gets the NDS Tree name.
     * @return the Tree name
     */
    public String getNDSTree() {
        return NDSTree;
    }

    /**
     * Sets the NDS Tree name.
     * @param value the Tree name
     */
    public void setNDSTree( String value ) {
        NDSTree = value;
    }

    /**
     * Gets the NDS Contexts.
     * @return an array of NDS Context names
     */
    public String[] getNDSContexts() {
        return NDSContexts;
    }

    /**
     * Sets the NDS Contexts.
     * @param value an array of NDS Context names
     */
    public void setNDSContexts( String[] value ) {
        NDSContexts = value;
    }

    /**
     * Gets the number of NDS Contexts.
     * @return the number of NDS Contexts
     */
    public int getNumNDSContexts() {
        return NDSContexts.length;
    }

    /**
     * Test whether the NDS 'Find Context' option should be displayed.
     * @return <code>true</code> if the 'Find Context' should be displayed, otherwise <code>false</code>
     */
    public boolean getShowFindContext() {
        return showFindContext;
    }

    /**
     * Sets whether the NDS 'Find Context' option should be displayed.
     * @param showFindContext <code>true</code> if enabled, else <code>false</code>
     */
    public void setShowFindContext(boolean showFindContext) {
        this.showFindContext = showFindContext;
    }

    /**
     * Tests whether the entire login page is disabled.
     * @return value <code>true</code> if disabled, else <code>false</code>
     */
    public boolean getAllUIDisabled() {
        return allUIDisabled;
    }

    /**
     * Sets whether the entire login page is disabled.
     * @param value <code>true</code> if disabled, else <code>false</code>
     */
    public void setAllUIDisabled( boolean value ) {
        allUIDisabled = value;
    }


    /**
     * Gets whether to display the password field.
     *
     * @return <code>true</code> if the password field should be
     * shown, else <code>false</code>
     */
    public boolean getShowPassword() {
        return showPassword;
    }

    /**
     * Sets whether to display the password field.
     *
     * @param  showPassword <code>true</code> if the password field should be
     * shown, else <code>false</code>
     */
    public void setShowPassword(boolean showPassword) {
        this.showPassword = showPassword;
    }

    /**
     * Gets whether to display the Login button field.
     * @return <code>true</code> if the Login button should be
     * shown, else <code>false</code>
     */
    public boolean getShowLoginButton()
    {
        return showLoginButton;
    }

    /**
     * Sets whether to display the Login button field.
     * @param  showLoginButton <code>true</code> if the Login button field should be
     * shown, else <code>false</code>
     */
    public void setShowLoginButton(boolean showLoginButton)
    {
        this.showLoginButton = showLoginButton;
    }

    /**
     * Gets the selected Logon mode.
     * @return the selected Logon mode
     */
    public String getSelectedLogonModeStr() {
        return selectedLogonMode.toString();
    }

    /**
     * Tests whether Anonymous Logon is selected.
     * @return <code>selected</code> or an empty string
     */
    public String getAnonymousSelectedStr() {
        return Markup.selectedStr(WIAuthType.ANONYMOUS.equals(selectedLogonMode));
    }

    /**
     * Tests whether Explicit Logon is selected.
     * @return <code>selected</code> or an empty string
     */
    public String getExplicitSelectedStr() {
        return Markup.selectedStr(WIAuthType.EXPLICIT.equals(selectedLogonMode));
    }

    /**
     * Tests whether Certificate Logon is selected.
     * @return <code>selected</code> or an empty string
     */
    public String getCertificateSelectedStr() {
        return Markup.selectedStr(WIAuthType.CERTIFICATE.equals(selectedLogonMode));
    }

    /**
     * Tests whether Certificate SSO Logon is selected.
     * @return <code>selected</code> or an empty string
     */
    public String getCertificateSSONSelectedStr() {
        return Markup.selectedStr(WIAuthType.CERTIFICATE_SINGLE_SIGN_ON.equals(selectedLogonMode));
    }

    /**
     * Tests whether SSO Logon is selected.
     * @return <code>selected</code> or an empty string
     */
    public String getSSONSelectedStr() {
        return Markup.selectedStr(WIAuthType.SINGLE_SIGN_ON.equals(selectedLogonMode));
    }

    /**
     * Tests whether Explicit Logon is disabled.
     * @return <code>disabled</code> or an empty string
     */
    public String getExplicitDisabledStr() {
        return Markup.disabledStr( allUIDisabled || explicitDisabled
                            || (! WIAuthType.EXPLICIT.equals(selectedLogonMode)) );
    }

    /**
     * Tests whether the entire UI disabled.
     * @return <code>disabled</code> or an empty string
     */
    public String getAllUIDisabledStr() {
        return Markup.disabledStr( allUIDisabled );
    }

    /**
     * Gets the localized inline help text for the login methods.
     */
    public String getLoginMethodInlineHelp(WIContext wiContext) {
        String inlineHelp = "";
        if (allowedLogonModes().contains(WIAuthType.ANONYMOUS)) {
            inlineHelp += wiContext.getString("Help_LoginMethod_Anonymous");
        }
        if (allowedLogonModes().contains(WIAuthType.CERTIFICATE_SINGLE_SIGN_ON)) {
            inlineHelp += wiContext.getString("Help_LoginMethod_PassCard");
        }
        if (allowedLogonModes().contains(WIAuthType.CERTIFICATE)) {
            inlineHelp += wiContext.getString("Help_LoginMethod_Card");
        }
        if (allowedLogonModes().contains(WIAuthType.SINGLE_SIGN_ON)) {
            inlineHelp += wiContext.getString("Help_LoginMethod_Pass");
        }
        if (allowedLogonModes().contains(WIAuthType.EXPLICIT)) {
            inlineHelp += wiContext.getString("Help_LoginMethod_Explicit");
        }
        inlineHelp = wiContext.getString("Help_LoginMethod", inlineHelp);

        return inlineHelp;
    }
}
