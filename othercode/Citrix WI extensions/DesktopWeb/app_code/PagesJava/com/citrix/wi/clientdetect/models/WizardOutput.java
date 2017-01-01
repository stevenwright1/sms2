/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */

package com.citrix.wi.clientdetect.models;

import java.util.LinkedList;

import com.citrix.wi.clientdetect.Client;
import com.citrix.wi.clientdetect.Mode;
import com.citrix.wi.clientdetect.type.ClientType;
import com.citrix.wi.clientdetect.type.IcoStatus;

/**
 * Output model of the client detection wizard.
 */
public class WizardOutput {
    private Client    remoteClientDetected    = null;
    private Client    streamingClientDetected = null;
    private String    alternateResult         = null;
    private String    rdpClientClassId        = null;
    private IcoStatus icoStatus               = null;

    /**
     * Constructor
     */
    public WizardOutput() {
    }

    /**
     * Initializes the WizardOutput with the default values.
     */
    public void initialize() {
        remoteClientDetected = null;
        streamingClientDetected = null;
        alternateResult = null;
        rdpClientClassId = null;
        icoStatus = IcoStatus.NOT_PRESENT;
    }

    /**
     * Sets the the detected streaming client.
     * @param streamingClientDetected the client
     */
    public void setStreamingClientDetected(Client streamingClientDetected) {
        this.streamingClientDetected = streamingClientDetected;
    }

    /**
     * Gets the detected streaming client.
     * @return the client or null if not detected
     */
    public Client getStreamingClientDetected() {
        return streamingClientDetected;
    }

    /**
     * Sets the detected remote client.
     * @param remoteClientDetected the client
     */
    public void setRemoteClientDetected(Client remoteClientDetected) {
        this.remoteClientDetected = remoteClientDetected;
    }

    /**
     * Gets the detected remote client
     * @param the client or null if not detected
     */
    public Client getRemoteClientDetected() {
        return remoteClientDetected;
    }

    /**
     * Sets the altername result of the wizard.
     *
     * @param altResult alternate result
     */
    public void setAlternateResult(String altResult) {
        this.alternateResult = altResult;
    }

    /**
     * Gets the alternate result of the client wizard.
     *
     * @return alternate result if set, null otherwise.
     */
    public String getAlternateResult() {
        return alternateResult;
    }

    /**
     * Sets the rdp client class id.
     *
     * @param classId rdp client class id
     */
    public void setRdpClientClassId(String classId) {
        this.rdpClientClassId = classId;
    }

    /**
     * Gets the rdp client class id.
     *
     * @return rdp client class id or null if none set
     */
    public String getRdpClientClassId() {
        return rdpClientClassId;
    }

    /**
     * Gets the ICO status
     * @return the icoStatus
     */
    public IcoStatus getIcoStatus() {
        return icoStatus;
    }

    /**
     * Sets the ICO status
     * @param icoStatus the icoStatus to set
     */
    public void setIcoStatus(IcoStatus icoStatus) {
        this.icoStatus = icoStatus;
    }

    /**
     * Updates the output model from the given WizardModel and WizardInput
     * model.
     *
     * @param model the wizard model
     * @param inputs the wizard inputs model
     */
    public void update(WizardModel model, WizardInput inputs) {
        String error = model.getError();
        // If there is an error then only AlternateResult is passed back
        if (error != null) {
            alternateResult = error;
        } else if (inputs.getMode() == Mode.ADVANCED) {
            updateAdvanced(model, inputs);
        } else { // AUTO or SILENT mode
            updateAutoAndSilent(model, inputs);
        }

        rdpClientClassId = model.getRdpClientClassId();
        icoStatus = model.getIcoStatus();
    }

    /**
     * Updates the output model from the given WizardModel and WizardInput
     * model when in ADVANCED mode.
     */
    private void updateAdvanced(WizardModel model, WizardInput inputs) {
        // If mode is Advanced then the user preference takes precedence over
        // auto detection
        Client userPrefClient = model.getUserPreferredClient();
        if (userPrefClient != null) {
            // update with all the information we have about this client
            model.updateClientFromModel(userPrefClient, userPrefClient.isAutoDetected());
            // set the result if we were allowed to choose this client
            if (inputs.detectRemoteClient(userPrefClient.getClientType())
                            || (ClientType.JAVA.equals(userPrefClient.getClientType()) && inputs.isJavaFallback())) {
                remoteClientDetected = userPrefClient;
            } else {
                remoteClientDetected = null;
            }
        }
    }

    /**
     * This is a utility method to update the output for the given model and
     * given inputs when in AUTO or SILENT mode.
     */
    private void updateAutoAndSilent(WizardModel model, WizardInput inputs) {
        // remote
        remoteClientDetected = null;
        if (model.getUserPreferredClient() != null) {
            // See if the user has forced a client
            // e.g. by pressing the "already installed" button
            remoteClientDetected = model.getUserPreferredClient();
            model.updateClientFromModel(remoteClientDetected, false);

        } else {
            // Process the clients in the order in which they were supplied by the caller, because
            // the ordering defines a preference.
            LinkedList remoteClientList = inputs.getRemoteClients();
            for (int i = 0; i < remoteClientList.size(); i++) {
                ClientType clientType = (ClientType)remoteClientList.get(i);
                if (inputs.detectRemoteClient(clientType) && model.isClientAvailable(clientType)) {
                    remoteClientDetected = createClient(clientType, model, true);
                    break;
                }
            }
            // if java fallback is enabled, we can also check for java
            if (inputs.isJavaFallback() && remoteClientDetected == null) {
                if (model.isClientAvailable(ClientType.JAVA)) {
                    remoteClientDetected = createClient(ClientType.JAVA, model, true);
                }
            }
        }

        // streaming
        streamingClientDetected = null;

        // go through to see if we found the streaming client
        // this can't be forced right now
        for (int i = 0; i < WizardModel.STREAMING_CLIENT_TYPES.length; i++) {
            ClientType clientType = WizardModel.STREAMING_CLIENT_TYPES[i];
            if (inputs.detectStreamingClient() && model.isClientAvailable(clientType)) {
                streamingClientDetected = createClient(clientType, model, true);
                break;
            }
        }
    }

    /**
     * Utility method to create a client and set upgradeable.
     */
    private Client createClient(ClientType clientType, WizardModel model, boolean isAutoDetected) {
        Client client = new Client(clientType);
        model.updateClientFromModel(client, isAutoDetected);
        return client;
    }


}
