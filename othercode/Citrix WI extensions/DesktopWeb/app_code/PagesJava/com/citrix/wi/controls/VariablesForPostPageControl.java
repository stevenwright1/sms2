package com.citrix.wi.controls;

/**
 * Page control to set up the POST data to pass to the Client
 * Detection Wizard.
 */
public class VariablesForPostPageControl extends PageControl {
    private String remoteClients;
    private String streamingClient;
    private String WizardMode;
    private String showZonePage;
    private String showUpgradePage;
    private String preferredClient;
    private String redirectURL;
    private String logout;
    private boolean showWelcomeScreen;
    private String productName;
    private String msgKey;
    private String msgType;
    private String productType;
    private boolean javaFallback = false;
    private boolean usingAccessGateway = false;

    public String getRemoteClients() {
        return remoteClients;
    }
    public void setRemoteClients(String remoteClients) {
        this.remoteClients = remoteClients;
    }
    public String getStreamingClient() {
        return streamingClient;
    }
    public void setStreamingClient(String streamingClient) {
        this.streamingClient = streamingClient;
    }
    public String getWizardMode() {
        return WizardMode;
    }
    public void setWizardMode(String wizardMode) {
        WizardMode = wizardMode;
    }
    public String getShowZonePage() {
        return showZonePage;
    }
    public void setShowZonePage(String showZonePage) {
        this.showZonePage = showZonePage;
    }
    public String getShowUpgradePage() {
        return showUpgradePage;
    }
    public void setShowUpgradePage(String showUpgradePage) {
        this.showUpgradePage = showUpgradePage;
    }
    public String getPreferredClient() {
        return preferredClient;
    }
    public void setPreferredClient(String preferredClient) {
        this.preferredClient = preferredClient;
    }
    public String getRedirectURL() {
        return redirectURL;
    }
    public void setRedirectURL(String redirectURL) {
        this.redirectURL = redirectURL;
    }
    public String getLogout() {
        return logout;
    }
    public void setLogout(String logout) {
        this.logout = logout;
    }
    public boolean isShowWelcomeScreen() {
        return showWelcomeScreen;
    }
    public void setShowWelcomeScreen(boolean showWelcomeScreen) {
        this.showWelcomeScreen = showWelcomeScreen;
    }

    /**
     * Sets the key for the message to show in the feedback area
     */
    public void setMessageKey(String key)
    {
        msgKey = key;
    }

    /**
     * Gets the key for the message to show in the feedback area
     */
    public String getMessageKey()
    {
        return msgKey;
    }

    /**
     * Sets the type of the message to show in the feedback area
     */
    public void setMessageType(String type)
    {
        msgType = type;
    }

    /**
     * Gets the type of the message to show in the feedback area
     */
    public String getMessageType()
    {
        return msgType;
    }

    /**
     * Gets whether a message type and key have been set
     */
    public boolean hasMessage()
    {
        return (msgType != null && msgType.length() > 0)
            && (msgKey != null && msgKey.length() > 0);
    }

    /**
     * Set whether to use compatibility for NetScaler / Access Gateway.
     */
    public void setAccessGatewayCompatibility(boolean tf) {
        usingAccessGateway = tf;
    }

    /**
     * Get whether to use compatibility for NetScaler / Access Gateway.
     */
    public boolean getAccessGatewayCompatibility() {
        return usingAccessGateway;
    }

    /**
     * Sets the ProductType value
     */
    public String getProductType() {
        return productType;
    }

    /**
     * Gets the ProductType value
     */
    public void setProductType(String productType) {
        this.productType = productType;
    }

    /**
     * Gets if Java Fallback is enabled
     * @return true if fallback is enabled
     */
    public boolean isJavaFallback() {
        return javaFallback;
    }

    /**
     * Sets if Java Fallback is enabled
     * @param javaFallback if true then Java Fallback is enabled
     */
    public void setJavaFallback(boolean javaFallback) {
        this.javaFallback = javaFallback;
    }
}
