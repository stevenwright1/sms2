/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.clientdetect.models;

import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import com.citrix.wi.clientdetect.Client;
import com.citrix.wi.clientdetect.Mode;
import com.citrix.wi.clientdetect.WizardConstants;
import com.citrix.wi.clientdetect.WizardContext;
import com.citrix.wi.clientdetect.WizardUtil;
import com.citrix.wi.clientdetect.type.ClientType;
import com.citrix.wi.clientdetect.type.IcoStatus;
import com.citrix.wing.util.Cookies;

/**
 * This is the model of the actual working of the wizard and stores the information
 * required to run the wizard. The model provides the data needed in the scripts.
 */
public class WizardModel {
    private String currentStep = null;
    private WizardInput inputs = null;
    private LinkedList detectionPages = null;
    private boolean radeDetected = false;
    private boolean radeUpgradeable = false;
    private boolean nativeDetected = false;
    private boolean nativeUpgradeable = false;
    private boolean javaDetected = false;
    private boolean rdpDetected = false;
    private String rdpClientClassId = null;
    private boolean popupAllowed = false;
    private boolean correctZoneForNative = true;
    private boolean correctZoneForRade = false;
    private boolean correctZoneForRdp = false;
    private String error = null;
    private Client userPreferredClient = null;
    private IcoStatus icoStatus = null;

    /**
     * The remote detectable client types
     */
    public static final ClientType[] REMOTE_CLIENT_TYPES
    = new ClientType[]{ClientType.NATIVE, ClientType.JAVA, ClientType.RDP};

    /**
     * The streaming detectable client types
     */
    public static final ClientType[] STREAMING_CLIENT_TYPES
    = new ClientType[]{ClientType.RADE};

    /**
     * All detectable client types
     */
    public static final ClientType[] ALL_CLIENT_TYPES
    = new ClientType[]{ClientType.NATIVE, ClientType.JAVA, ClientType.RDP, ClientType.RADE};

    /**
     * Used to store items which need to be persisted for the
     * duration of this wizard invocation.
     *
     * Needs to be protected scope so that it can
     * be persisted and repopulated correctly.
     */
    protected HashMap attributeMap = new HashMap();

    /**
     * Creates a new WizardModel with default values and current step as the start page
     * of the wizard
     */
    public WizardModel() {
        currentStep = WizardConstants.PAGE_START;
    }

    /**
     * Initializes the model with default values.
     */
    public void initialize() {
        currentStep = WizardConstants.PAGE_START;
        inputs = null;
        radeDetected = false;
        radeUpgradeable = false;
        detectionPages = null;
        nativeDetected = false;
        nativeUpgradeable = false;
        javaDetected = false;
        rdpDetected = false;
        rdpClientClassId = null;
        popupAllowed = false;
        correctZoneForNative = true;
        correctZoneForRade = false;
        correctZoneForRdp = false;
        error = null;
        userPreferredClient = null;
        attributeMap.clear();
        icoStatus = IcoStatus.NOT_PRESENT;
    }

    /**
     * sets the current step of the wizard. This is needed to work out the next page
     * that needs to be detected.
     * Step is represented by a client's detection page.
     * @param value the current step
     */
    public void setCurrentStep(String value) {
        currentStep = value;
    }

    /**
     * gets the current step of the wizard.
     * @return the current step
     */
    public String getCurrentStep() {
        return currentStep;
    }

    /**
     * sets whether rade is detected or not
     * @param value true if detected, otherwise false
     */
    public void setRadeDetected(boolean value) {
        radeDetected = value;
    }

    /**
     * gets whether rade is detected or not
     * @return true if detected, otherwise false
     */
    public boolean radeDetected() {
        return radeDetected;
    }

    /**
     * gets whether rade is upgradeable or not
     * @return true if upgradeable, otherwise false
     */
    public boolean radeUpgradeable() {
        return radeUpgradeable;
    }

    /**
     * sets whether native is detected or not
     * @param value true if detected, otherwise false
     */
    public void setNativeDetected(boolean value) {
        nativeDetected = value;
    }

    /**
     * gets whether the native is detected or not
     * @return true if detected, otherwise false
     */
    public boolean nativeDetected() {
        return nativeDetected;
    }

    /**
     * gets whether native is uprgradeable or not
     * @return true if uprgradeable, otherwise false
     */
    public boolean nativeUpgradeable() {
        return nativeUpgradeable;
    }

    /**
     * sets whether java is detected or not
     * @param value true if detected, otherwise false
     */
    public void setJavaDetected(boolean value) {
        javaDetected = value;
    }

    /**
     * gets whether java is detected or not
     * @return true if detected, otherwise false
     */
    public boolean javaDetected() {
        return javaDetected;
    }

    /**
     * sets whether rdp is detected or not
     * @param value true if detected, otherwise false
     */
    public void setRdpDetected(boolean value) {
        rdpDetected = value;
    }

    /**
     * gets whether rdp is detected or not
     * @param true if detected, otherwise false
     */
    public boolean rdpDetected() {
        return rdpDetected;
    }

    /**
     * sets the rdp client class id that has been detected
     * @param value rdp client class Id
     */
    public void setRdpClientClassId(String value) {
        rdpClientClassId = value;
    }

    /**
     * gets the rdp client class id
     * @return rdp client class id
     */
    public String getRdpClientClassId() {
        return rdpClientClassId;
    }

    /**
     * sets whether pop up is allowed or not
     * @param value true if allowed, otherwise false
     */
    public void setPopupAllowed(boolean value) {
        popupAllowed = value;
    }

    /**
     * gets whether popup is allowed or not
     * @return true if detected, otherwise false
     */
    public boolean popupAllowed() {
        return popupAllowed;
    }

    /**
     * Gets whether the zone is correct for the native client
     * @return true if correct, otherwise false
     */
    public boolean isCorrectZoneForNative() {
        return correctZoneForNative;
    }

    /**
     * Sets whether the zone is correct for the native client
     * @param correctZoneForNative true if correct, otherwise false
     */
    public void setCorrectZoneForNative(boolean correctZoneForNative) {
        this.correctZoneForNative = correctZoneForNative;
    }

    /**
     * Gets whether the zone is correct for the streaming client
     * @return true if correct, otherwise false
     */
    public boolean isCorrectZoneForRade() {
        return correctZoneForRade;
    }

    /**
     * Sets whether the zone is correct for the streaming client
     * @param correctZoneForRade true if correct, otherwise false
     */
    public void setCorrectZoneForRade(boolean correctZoneForRade) {
        this.correctZoneForRade = correctZoneForRade;
    }

    /**
     * Gets whether the zone is correct for the RDP client
     * @return true if correct, otherwise false
     */
    public boolean isCorrectZoneForRdp() {
        return correctZoneForRdp;
    }

    /**
     * Sets whether the zone is correct for the RDP client
     * @param correctZoneForRade true if correct, otherwise false
     */
    public void setCorrectZoneForRdp(boolean correctZoneForRdp) {
        this.correctZoneForRdp = correctZoneForRdp;
    }

    /**
     * sets the error
     * @param value the error that occured in the wizard
     */
    public void setError(String value) {
        error = value;
    }

    /**
     * gets the errors that might have occured during the execution of the wizard
     * @return the error
     */
    public String getError() {
        return error;
    }

    /**
     * sets the user's preferred client.
     * @param value the preferred client
     */
    public void setUserPreferredClient(Client value) {
        userPreferredClient = value;
    }

    /**
     * gets the user preferred client or null if not set
     * @return preferred client or null if not set
     */
    public Client getUserPreferredClient() {
        return userPreferredClient;
    }

    /**
     * Sets the ICO status
     * @param icoStatus the status
     */
    public void setIcoStatus(IcoStatus icoStatus) {
        this.icoStatus = icoStatus;
    }

    /**
     * Gets the ICO status
     * @return the status
     */
    public IcoStatus getIcoStatus() {
        return icoStatus;
    }

    /**
     * Gets the next page that wizard needs to go to. This page is the start page of each client
     * detection process. If the wizard is running in Auto Mode or silent mode then it will return the detection page
     * of the next client that needs to be detected if finish parameter is false otherwise it returns the finish page.
     * If the wizard is running in advance mode, this will return the advanced page.
     * @param boolean whether to finish detection or not. if finish is true, wizard will return the finish
     * page otherwise will return the page of the next client
     * @return the next page
     */
    public String getNextStep(boolean finish) {
        String result = "";
        if (detectionPages != null && !detectionPages.isEmpty()) {
            int currentClientIndex = detectionPages.indexOf(currentStep);
            if (currentClientIndex == -1) {
                if (WizardConstants.PAGE_START.equals(currentStep)) {
                    result = (String)detectionPages.getFirst();
                }
            } else if (inputs.getMode() == Mode.ADVANCED) {
                // Re-do advanced detection in case something has changed since last time;
                // for example, popups blocking may have been switched off to enable the RDP
                // client; this could also impact the status of the Java client
                result = WizardConstants.PAGE_ADVANCED_DETECT;
            } else if (finish) {
                result = WizardConstants.PAGE_FINISH;
            } else if ((currentClientIndex < detectionPages.size() - 1)) {
                result = (String)detectionPages.get(currentClientIndex + 1);
            } else {
                result = WizardConstants.PAGE_FINISH;
            }
        } else {
            result = WizardConstants.PAGE_FINISH;
        }
        return result;
    }

    /**
     * Gets the next page. This is same as calling getNextPage(false).
     * @return the next Page
     */
    public String getNextStep() {
        return getNextStep(false);
    }

    /**
     * Gets the URL of the next page with CSRF token string.
     * @return the next page.
     */
    public String getNextStepWithCsrf(WizardContext context) {
        return getNextStepWithCsrf(context, false);
    }

    /**
     * Gets the URL of the next page with CSRF token
     * string, possibly forcing to step after dectection has finished.
     * @param finish Whether to finsih detection, see GetNextStep(boolean).
     * @return the next page.
     */
    public String getNextStepWithCsrf(WizardContext context,
        boolean finish) {
        return WizardUtil.getUrlWithQueryStrWithCsrf(context, getNextStep(finish));
    }

    /**
     * Populates the model's detection pages from the wizard input model
     *
     * @param input the wizard input model
     */
    private void populateClientDetectionPages(WizardInput input) {
        detectionPages = new LinkedList();

        // Add the streaming client if it has been requested
        if (input.detectStreamingClient()) {
            detectionPages.add(ClientType.RADE.getPage());
        }

        if (input.getRemoteClients().size() > 0) {
            // add all the clients into the detection pages
            // ignore the order in the list
            List remoteClients = input.getRemoteClients();
            if (remoteClients.contains(ClientType.NATIVE)) {
                detectionPages.add(ClientType.NATIVE.getPage());
            }
            if (remoteClients.contains(ClientType.JAVA) ||
                            (remoteClients.contains(ClientType.NATIVE) && input.isJavaFallback())) {
                detectionPages.add(ClientType.JAVA.getPage());
            }
            if (remoteClients.contains(ClientType.RDP)) {
                detectionPages.add(ClientType.RDP.getPage());
            }
        }
    }

    /**
     * Populates the model with the given WizardInput model.
     *
     * @param input the wizard input model
     */
    public void populateModel(WizardInput input) {
        this.inputs = input;
        populateClientDetectionPages(input);
    }

    /**
     * Updates the model with client detection data from the cookies.
     * @param cookieStr - all the cookies as a string - this will contain the
     * client detection data generated by the wizard's detection pages
     */
    public void updateClientsResult(String cookieStr) {
        if (cookieStr == null) {
            return;
        }
        String clientInfoCookie = WizardUtil.getCookieValue(cookieStr, WizardConstants.COOKIE_CLIENT_INFO);
        Map cookieMap = Cookies.parseCookieValue(clientInfoCookie);

        radeUpgradeable = isCookieValueEquals(cookieMap, WizardConstants.RADE, WizardConstants.UPGRADEABLE);
        radeDetected = isCookieValueTrue(cookieMap, WizardConstants.RADE) || radeUpgradeable;

        nativeUpgradeable = isCookieValueEquals(cookieMap, WizardConstants.NATIVE, WizardConstants.UPGRADEABLE);
        nativeDetected = isCookieValueTrue(cookieMap, WizardConstants.NATIVE) || nativeUpgradeable;

        javaDetected = isCookieValueTrue(cookieMap, WizardConstants.JAVA);
        rdpDetected = isCookieValueTrue(cookieMap, WizardConstants.RDP);
        popupAllowed = isCookieValueTrue(cookieMap, WizardConstants.POPUP_ALLOWED);

        // we can't tell what zone we are in
        // if we can't tell if the client is there
        correctZoneForNative = true;
        if (nativeDetected) {
            // if we can't detect the native client
            // just assume we are in the correct zone
            correctZoneForNative = !isCookieValueTrue(cookieMap, WizardConstants.INCORRECT_ZONE_NATIVE);
        }
        correctZoneForRade = false;
        if (radeDetected) {
            correctZoneForRade = isCookieValueTrue(cookieMap, WizardConstants.CORRECT_ZONE_RADE);
        }
        correctZoneForRdp = false;
        if (rdpDetected) {
            correctZoneForRdp = isCookieValueTrue(cookieMap, WizardConstants.CORRECT_ZONE_RDP);
        }

        // only look for the class id
        // if we found the RDP client was installed
        if (rdpDetected && rdpClientClassId == null) {
            rdpClientClassId = (String) cookieMap.get(WizardConstants.COOKIE_WIZARD + WizardConstants.COOKIE_RDPCLASSID);
        }

        // User is allowed to force the Native client even if the native client is not detected; this
        // is *not* the same as forcing a client in advanced mode
        String userPreferredCookie = (String)cookieMap.get(WizardConstants.COOKIE_WIZARD + WizardConstants.USER_PREFERRED);
        if (WizardConstants.NATIVE.equals(userPreferredCookie)) {
            userPreferredClient = new Client(ClientType.NATIVE);
        } else if (WizardConstants.JAVA.equals(userPreferredCookie)) {
            userPreferredClient = new Client(ClientType.JAVA);
        }

        icoStatus = IcoStatus.getIcoStatusFromString(
                        getCookieValue(cookieMap, WizardConstants.ICO_STATUS));
    }

    /**
     * Utility method to check whether the given name exist in the map and its value is True or False.
     * @param cookieMap all the subcookie values with name as the keys
     * @param name the subcookie name that we are checking
     * @return true if the name exist in the map and its value is True, false otherwise
     */
    private boolean isCookieValueTrue(Map cookieMap, String name) {
        String cookieValue = getCookieValue(cookieMap,name);
        return WizardConstants.VAL_TRUE.equalsIgnoreCase(cookieValue);
    }

    /**
     * Utility method to check whether the given name exist in the map and its value is equal to the given value.
     * This is method is case insensitive in respect of the value.
     * @param cookieMap all the subcookie values with name as the keys
     * @param name the subcookie name that we are checking the value for in the map
     * @param compareValueTo, the value that we are looking for the given name
     * @return true if the name exist in the map and its value is equal to the given value, false otherwise
     */
    private boolean isCookieValueEquals(Map cookieMap, String name, String compareValueTo) {
        String cookieValue = null;
        boolean result = false;
        cookieValue = getCookieValue(cookieMap,name);
        if (cookieValue != null) {
            result = cookieValue.equalsIgnoreCase(compareValueTo);
        }
        return result;
    }

    /**
     * Gets the value from the cookie map
     *
     * @param cookieMap where the values are stored
     * @param name the particular value to be extracted
     * @return the value found
     */
    private String getCookieValue(Map cookieMap, String name) {
        return (String)cookieMap.get(WizardConstants.COOKIE_WIZARD + name);
    }

    /**
     * Checks whether the given client is "available" for use or not. Available does not only mean
     * that the client is present: the client should satisfy all the conditions for use
     * before it is made available e.g. no pop up blocker or correct zone.
     * @param clientType client to check
     * @return true if the client name is valid and can be used, false otherwise.
     */
    public boolean isClientAvailable(ClientType clientType) {
        boolean result = false;
        if (clientType == ClientType.RADE && inputs.detectStreamingClient()) {
            result = radeDetected && correctZoneForRade;
        } else if (inputs.detectRemoteClient(clientType) ||
                        (ClientType.JAVA.equals(clientType)
                                        && inputs.isJavaFallback())) {
            if (clientType == ClientType.NATIVE) {
                // We can use the native client on IE even if it isn't in the correct IE zone (however
                // workspace control will not be available).
                result = nativeDetected;
            } else if (clientType == ClientType.JAVA) {
                result = javaDetected && popupAllowed;
            } else if (clientType == ClientType.RDP) {
                result = rdpDetected && (rdpClientClassId != null)
                                     && (!rdpClientClassId.equals(""))
                                     && correctZoneForRdp && popupAllowed;
            }
        }
        return result;
    }

    /**
     * Checks whether the given client has been detected. Used to generate the Client Status
     * table in Advanced mode.
     * @param clientType the client
     * @return true if the client is valid and has been detected, false otherwise.
     */
    public boolean isClientDetected(ClientType clientType) {
        boolean result = false;
        if (clientType == ClientType.RADE && inputs.detectStreamingClient()) {
            result = radeDetected;
        } else if (inputs.detectRemoteClient(clientType) ||
                        (ClientType.JAVA.equals(clientType)
                                        && inputs.isJavaFallback())) {
            if (clientType == ClientType.NATIVE) {
                result = nativeDetected;
            } else if (clientType == ClientType.JAVA) {
                result = javaDetected;
            } else if (clientType == ClientType.RDP) {
                result = rdpDetected;
            }
        }
        return result;
    }

    /**
     * Gets whether the given client is upgradeable.
     * @param client the cleint
     * @return true if the given client is valid and upgradeable, false otherwise
     */
    public boolean isClientUpgradeable(ClientType client) {
        boolean result = false;
        if (client == ClientType.NATIVE) {
            result = nativeUpgradeable;
        } else if (client == ClientType.RADE) {
            result = radeUpgradeable;
        }
        return result;
    }

    /**
     * Gets the auto preferred client. This is the client that the wizard thinks is best for the user.
     * This method checks for the clients in the order of preferrence, which is determined by the
     * order in which the clients were specified in the wizard inputs.
     * @return a Client object representing the auto preferred client.
     */
    public Client getAutoPreferredClient() {
        Client result = null;
        LinkedList remoteClientList = inputs.getRemoteClients();
        for (int i = 0; i < remoteClientList.size(); i++) {
            ClientType clientType = (ClientType)remoteClientList.get(i);
            if (isClientAvailable(clientType)) {
                result = new Client(clientType);
                updateClientFromModel(result, true);
                break;
            }
        }
        return result;
    }

    /**
     * Update the client object with data from
     * the wizard model
     */
    void updateClientFromModel(Client client, boolean autoDetected) {
        if (client == null) return; // nothing to update

        ClientType clientType = client.getClientType();
        client.setUpgradeable(isClientUpgradeable(clientType));
        client.setIncorrectZone(!isCorrectZone(clientType));
        client.setAutoDetected(autoDetected);
    }

    /**
     * Gets the attribute value from the model with the given string as the key.
     * @param key String key to look in the attributes map
     * @return value associated with the given key or null if does not exist.
     */
    public String getAttribute(String key) {
        return (String)attributeMap.get(key);
    }

    /**
     * Sets the given string value in the attribute map of the model.
     * with the given string key. This is a convenient place to store session values as strings
     * as this attribute map will be cleared once the wizard finishes.
     * @param key the key into the map
     * @param value the value to set
     */
    public void setAttribute(String key, String value) {
        attributeMap.put(key, value);
    }

    /**
     * This checks if the given client is in the correct zone or not
     * @param clientType the client to check
     * @return true if the client specified has been found to be in the correct zone, otherwise false
     */
    public boolean isCorrectZone(ClientType clientType) {
        boolean correctZone = true;
        if (clientType == ClientType.RADE) {
            correctZone = isCorrectZoneForRade();
        } else if (clientType == ClientType.NATIVE) {
            correctZone = isCorrectZoneForNative();
        } else if (clientType == ClientType.RDP) {
            correctZone = isCorrectZoneForRdp();
        }
        return correctZone;
    }

}
