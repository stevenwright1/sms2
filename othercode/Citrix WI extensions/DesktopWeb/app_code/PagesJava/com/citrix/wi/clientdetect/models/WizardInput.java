/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.clientdetect.models;

import java.util.Collection;
import java.util.LinkedList;
import java.util.Locale;

import com.citrix.wi.clientdetect.Client;
import com.citrix.wi.clientdetect.Mode;
import com.citrix.wi.clientdetect.type.ClientType;
import com.citrix.wi.types.ProductType;

/**
 * WizardInput is the model for holding input parameters to the client wizard
 * All the inputs can be referenced using this model.
 */
public class WizardInput {
    // url to redirect when the wizard is finished.
    private String redirectUrl;

    // User's Preferred Client. This is only used for advanced mode
    private Client preferredClient;

    // remote clients to be detected in the wizard
    private LinkedList remoteClients;

    // streaming clients to be detected in the wizard
    // At present there is only one streaming client, hence we use a flag.
    private boolean radeClientEnabled;

    // Mode to run the wizard
    private Mode mode;

    // Locale to use for displaying wizard strings
    private Locale locale;

    // whether to allow logout from the wizard
    private boolean allowLogout;

    // relative url of the master page for the wizard
    private String masterPage;

    // whether to show welcome screen of the wizard or not
    private boolean bShowWelcomeScreen;

    // whether to show zone help page for the native client or not
    private boolean bShowZonePage;

    // whether to show upgrade page for the native/streaming client or not
    private boolean bShowUpgradePage;

    // Is the site being proxied through NetScaler or Access Gateway.
    private boolean bUsingAccessGateway;

    // ProductType for which client is being detected
    private ProductType productType;

    // whether the native client should also try to fallback
    // to the java client
    private boolean javaFallback;

    public WizardInput() {
        // Explicitly set default values
        initialize();
    }

    /**
     * Initializes the wizard input model with default values
     */
    public void initialize() {
        redirectUrl = null;
        preferredClient = null;
        remoteClients = new LinkedList();
        radeClientEnabled = false;
        mode = Mode.SILENT;
        locale = null;
        allowLogout = false;
        masterPage = null;
        bShowWelcomeScreen = false;
        bShowZonePage = false;
        bShowUpgradePage = false;
        bUsingAccessGateway = false;
        productType = ProductType.XEN_APP;
        javaFallback = false;
    }

    /**
     * Initializes the wizard input model with default values and with the given
     * redirect Url and mode.
     * @param redirectUrl Url to use for redirecting when the wizard is finished
     * @param mode Mode to use for running the wizard
     */
    public void initialize(String redirectUrl, String mode) {
        initialize(redirectUrl, Mode.getModeFromString(mode));
    }

    /**
     * Initializes the wizard input model with default values and with the given
     * redirect Url and mode.
     * @param redirectUrl Url to use for redirecting when the wizard is finished
     * @param mode Mode to use for running the wizard
     */
    public void initialize(String redirectUrl, Mode mode) {
        initialize();
        this.redirectUrl = redirectUrl;
        this.mode = mode;
    }

    /**
     * Returns the redirect URL
     * @return String Redirect URL
     */
    public String getRedirectUrl() {
        return redirectUrl;
    }

    /**
     * Sets the redirect URL
     * @param String Valid Redirect URL
     */
    protected void setRedirectUrl(String url) {
        this.redirectUrl = url;
    }

    /**
     * Adds all the given remote clients
     * @param remoteClients Collection of remote Clients to be detected
     */
    public void setRemoteClients(Collection remoteClients) {
        if (remoteClients != null) {
            this.remoteClients.clear();
            this.remoteClients.addAll(remoteClients);
        }
    }

    /**
     * Returns the list of remote clients that needs to be detected
     * @return LinkedList list of remote clients
     */
    public LinkedList getRemoteClients() {
        return remoteClients;
    }

    /**
     * Sets whether the streaming client needs to be detected or not
     * @param true if the streaming client needs to be detected, otherwise false
     */
    public void setStreamingClient(boolean val) {
        radeClientEnabled = val;
    }

    /**
     * Returns whether the given remote client needs to be detected or not.
     * @param clientname the client type
     * @return true if it needs to be detected, otherwise false
     */
    public boolean detectRemoteClient(ClientType clientname) {
        return remoteClients.contains(clientname);
    }

    /**
     * Returns whether the streaming client needs to be detected
     * @return true if the streaming client needs to be detected, otherwise false
     */
    public boolean detectStreamingClient() {
        return radeClientEnabled;
    }

    /**
     * Gets the mode to run the wizard in.
     * @return the mode
     */
    public Mode getMode() {
        return mode;
    }

    /**
     * Sets the mode of the wizard.
     * @param mode the mode
     */
    public void setMode(Mode mode) {
        this.mode = mode;
    }

    /**
     * Gets the Locale of the wizard.
     * @return the local
     */
    public Locale getLocale() {
        return locale;
    }

    /**
     * Sets the Locale of the wizard to be used
     * @param locale the local
     */
    public void setlocale(Locale locale) {
        this.locale = locale;
    }

    /**
     * Sets the preferred client (needed for advanced mode).
     * @param client the client
     */
    public void setPreferredClient(Client client) {
        this.preferredClient = client;
    }

    /**
     * Gets the preferred client (needed for advanced mode).
     * @return the preferred client
     */
    public Client getPreferredClient() {
        Client result = preferredClient;
        if (!remoteClients.isEmpty() &&
            (preferredClient == null || !remoteClients.contains(preferredClient.getClientType()))) {
            //If the client is not passed or is invalid then use the first auto detected client.
            ClientType type = (ClientType)remoteClients.getFirst();
            result = new Client(type);
            result.setAutoDetected(true);
        }
        return result;
    }

    /**
     * Sets whether logout should be allowed from the wizard or not
     * @param val true if logout should be allowed, otherwise false
     */
    public void setAllowLogout(boolean val) {
        this.allowLogout = val;
    }

    /**
     * Gets whether logout should be allowed from the wizard or not
     * @return true if logout should be allowed, otherwise false
     */
    public boolean allowLogout() {
        return allowLogout;
    }

    /**
     * Sets the master page to use for the wizard
     * @param url this needs to be relative url
     */
    public void setMasterPage(String url) {
        this.masterPage = url;
    }

    /**
     * Gets the master page to use for the wizard
     * @return Relative url of the master page
     */
    public String getMasterPage() {
        return masterPage;
    }

    /**
     * Sets whether the welcome screen (start page) needs to be displayed or not
     * @param value true display welcome screen should be displayed, otherwise false
     */
    public void setShowWelcomeScreen(boolean value) {
        this.bShowWelcomeScreen = value;
    }

    /**
     * Gets whether to show welcome screen or not
     * @return true display welcome screen should be displayed, otherwise false
     */
    public boolean getShowWelcomeScreen() {
        return bShowWelcomeScreen;
    }

    /**
     * Sets whether to show the help page for changing zone for the native client
     * when the native client is in the wrong zone.
     * @param value true if the help page should be shown, otherwise false
     */
    public void setShowZonePage(boolean value) {
        this.bShowZonePage = value;
    }

    /**
     * Gets whether to show the help page for changing zone for the native client
     * when the native client in the wrong zone.
     * @return true if the help page should be shown, otherwise false
     */
    public boolean getShowZonePage() {
        return bShowZonePage;
    }

    /**
     * Sets whether to show the upgrade page for native/streaming client
     * if an upgrade is available.
     * @param value true if the upgrade page should be shown, otherwise false
     */
    public void setShowUpgradePage(boolean value) {
        this.bShowUpgradePage = value;
    }

    /**
     * Gets whether to show the upgrade page for native/streaming client
     * if an upgrade is available.
     * @return true if the upgrade page should be shown, otherwise false
     */
    public boolean getShowUpgradePage() {
        return bShowUpgradePage;
    }

    /**
     * Set that we are using NetScaler / Access Gateway and so must try to be compatible with it.
     */
    public void setUsingAccessGateway(boolean tf) {
        bUsingAccessGateway = tf;
    }

    /**
     * Get that we are using NetScaler / Access Gateway and so must try to be compatible with it.
     */
    public boolean getUsingAccessGateway() {
        return bUsingAccessGateway;
    }

    /**
     * Sets the product type which determines some of
     * the wizard behaviour.
     * @param value the type
     */
    public void setProductType(ProductType value) {
        productType = value;
    }

    /**
     * Gets the product type
     * @return the type
     */
    public ProductType getProductType() {
        return productType;
    }

    /**
     * If Java fallback is on, then the Java client
     * may be used if native is not available.
     *
     * @return the javaFallback
     */
    public boolean isJavaFallback() {
        return javaFallback;
    }

    /**
     * Set if we should fallback to java when
     * native is not available.
     *
     * @param javaFallback the javaFallback to set
     */
    public void setJavaFallback(boolean javaFallback) {
        this.javaFallback = javaFallback;
    }
}
