/*
 * ClientDetectionWizardState.java
 *
 * Copyright (c) 2003 - 2010 Citrix Systems, Inc. All Rights Reserved.
 *
 */
package com.citrix.wi.clientdetect;

import java.util.Iterator;
import java.util.List;

import com.citrix.wi.clientdetect.type.ClientType;
import com.citrix.wi.clientdetect.type.IcoStatus;
import com.citrix.wi.types.MPSClientType;

/**
 * Holds the state that represent the results of the client detection wizard.
 */
public class ClientDetectionWizardState
{
    // Record whether this object contains client detection data (as a result
    // of a run of the wizard).
    private boolean hasDetectionResult = false;
    // Check if the wizard has run, even if it didn't
    // give us a detection result.
    private boolean hasWizardRun = false;

    //
    // The following data was supplied to the wizard for the current invocation. We store
    // it here because it is also used in the processing of the wizard's output.
    //

    // The wizard mode
    private Mode mode = null;
    // The remote client list
    private List remoteClientsToDetect = null;
    // The streaming client list
    private List streamingClientsToDetect = null;
    // Wether the wizard was invoked from the toolbar
    private boolean invokedFromToolbar = false;
    // Whether when the wizard returns you should keep the same
    // client choice and only update the details of that client.
    private boolean keepSameClient = false;

    //
    // Detected results - this data is filled in after a wizard invocation using the wizard's
    // output.
    //
    private Client remoteClientResult;
    private Client streamingClientResult;
    private IcoStatus icoStatus;
    private boolean incorrectZoneResult = false;
    private String rdpClassID = "";

    // results from last time
    private Client oldRemoteClientResult;
    private Client oldStreamingClientResult;

    /**
     * Ctor.
     */
    public ClientDetectionWizardState() {
    }

    /**
     * Clear all the input associated with a previous wizard invokation.
     */
    public void clearInvocationState() {
        mode = null;
        remoteClientsToDetect = null;
        streamingClientsToDetect = null;
        invokedFromToolbar = false;
        keepSameClient = false;
    }

    /**
     * Initialises the ClientDetectionState with the outputs of the wizard.
     * @param remoteClientResult a <code>String</code> that represents remote client wizard result
     * @param streamingClientResult a <code>String</code> that represents streaming client wizard result
     * @param alternateResult a <code>String</code> that represents a wizard error or action result
     * @param rdpClassIDResult a <code>String</code> that represents the RDP client class id if a suitable RDP client was detected or null otherwise
     * @param icoStatusStr a <code>String</code> that represents the ico status result
     */
    public void setDetectionResult(String remoteClientResultStr, String streamingClientResultStr, String alternateResult, String rdpClassIDResult, String icoStatusStr) {
        // save the old result
        oldRemoteClientResult = remoteClientResult;
        oldStreamingClientResult = streamingClientResult;

        if (alternateResult != null && !alternateResult.equals("")) {
            // there was an error, so just say the wizard has run
            hasWizardRun = true;
            return;
        }

        if(remoteClientResultStr == null || streamingClientResultStr == null || icoStatusStr == null) {
            throw new IllegalArgumentException("Must have a remote client,  streaming client and ico status result, if there is no alternative result.");
        }

        remoteClientResult = null;
        Client client = Client.getClientFromString(remoteClientResultStr);
        if ((client != null) && client.getClientType().isRemote()) {
            remoteClientResult = client;
        }

        streamingClientResult = null;
        client = Client.getClientFromString(streamingClientResultStr);
        if ((client != null) && client.getClientType().isStreaming()) {
            streamingClientResult = client;
        }

        IcoStatus parsedIcoStatus = IcoStatus.getIcoStatusFromString(icoStatusStr);
        if (parsedIcoStatus != null) {
            icoStatus = parsedIcoStatus;
        } else {
            icoStatus = IcoStatus.NOT_PRESENT;
        }

        incorrectZoneResult = false;
        // Currently, only Ica-local will return
        // incorrect zone information
        if (remoteClientResult != null) {
            incorrectZoneResult = remoteClientResult.isIncorrectZone();
        }

        if (WizardUtil.isValidRdpClassId(rdpClassIDResult)) {
            rdpClassID = rdpClassIDResult.toUpperCase();
        } else {
            rdpClassID = "";
        }

        // store that we have a result
        hasDetectionResult = true;
        hasWizardRun = true;
    }

    /**
     * Sets the supplied client as the remote client result, as a forced (as opposed to auto-detected) client.
     * @param clientType the client to force
     */
    public void setForcedRemoteClientResult(MPSClientType clientType) {
        remoteClientResult = Client.getClientFromString(clientType.toString() + "=Forced");
    }

    /**
     * Gets if no client has been detected by wizard
     * @return true if client wizard has not detected a client, false otherwise.
     */
    public boolean hasNoClientResult() {
        return (getStreamingClientResult() == null && getRemoteClientResult() == null);
    }

    /**
     * Gets the ICO Status returned by client detection wizard.
     * @return the <code>IcoStatus</code> object that represents the wizard result
     */
    public IcoStatus getIcoStatusResult() {
        return icoStatus;
    }

    /**
     * Gets the streaming client result returned by client detection wizard.
     * @return client <code>Client</code> represents client type.
     */
    public Client getStreamingClientResult() {
        return streamingClientResult;
    }

    /**
     * Gets the remote client result returned by client detection wizard.
     * @return client <code>Client</code> represents client type.
     */
    public Client getRemoteClientResult() {
        return remoteClientResult;
    }

    /**
     * Gets if remote client can be upgraded.
     * This value is only used for native ica client.
     * @return true if remote client can be upgraded.
     */
    public boolean getRemoteCanUpgrade() {
        return remoteClientResult == null ? false : remoteClientResult.isUpgradeable();
    }

    /**
     * Gets if rade client can be upgraded.
     * @return <code>boolean</code> true if rade client can be upgraded.
     */
    public boolean getStreamingCanUpgrade() {
        return streamingClientResult == null ? false : streamingClientResult.isUpgradeable();
    }

    /**
     * If alternate result incorrect zone has been set true, otherwise false.
     * @return <code>boolean</code> true or false
     */
    public boolean getIncorrectZone() {
        return incorrectZoneResult;
    }

    /**
     * Gets the RDP client class ID if any suitable RDP client was detected.
     * @return the classID or an empty string
     */
    public String getRdpClassID() {
        return rdpClassID;
    }

    /**
     * If a streaming client was detected return true, otherwise false.
     * @return <code>boolean</code> true or false
     */
    public boolean hasStreamingClientResult() {
        return (getStreamingClientResult() != null);
    }

    /**
     * If a remote client was detected return true, otherwise false.
     * @return <code>boolean</code> true or false
     */
    public boolean hasRemoteClientResult() {
        return (getRemoteClientResult() != null);
    }

    /**
     * Gets value to represent if user has forced use of client
     * @return <code>boolean</code> represents if user has forced use of client
     */
    public boolean getResultForcedByUser() {
        return remoteClientResult == null ? false : !remoteClientResult.isAutoDetected();
    }

    /**
     * Sets the list of clients that needs to be detected.
     * The list consists of <code>ClientType</code> objects.
     */
    public void setRemoteClientsToDetect(List clients) {
        remoteClientsToDetect = clients;
    }

    /**
     * Gets the list of clients that needs to be detected.
     * The list consists of <code>ClientType</code> objects.
     */
    public List getRemoteClientsToDetect() {
        return remoteClientsToDetect;
    }

    /**
     * Returns a comma separated list of remote clients for detection.
     */
    public String getRemoteClientsToDetectAsString() {
        return convertClientListToString(remoteClientsToDetect);
    }

    /**
     * Sets the list of clients that needs to be detected.
     * The list consists of <code>ClientType</code> objects.
     */
    public void setStreamingClientsToDetect(List clients) {
        streamingClientsToDetect = clients;
    }

    /**
     * Gets the list of clients that needs to be detected.
     * The list consists of <code>ClientType</code> objects.
     */
    public List getStreamingClientsToDetect() {
        return streamingClientsToDetect;
    }

    /**
     * Returns a comma separated list of streaming clients for detection.
     */
    public String getStreamingClientsToDetectAsString() {
        return convertClientListToString(streamingClientsToDetect);
    }

    /**
     * Determines that rade client is/was the only client to detect.
     * @return true or false
     */
    public boolean detectRadeOnly() {
        boolean noRemoteClients    = remoteClientsToDetect == null || remoteClientsToDetect.isEmpty();
        boolean noStreamingClients = streamingClientsToDetect == null || streamingClientsToDetect.isEmpty();

        return noRemoteClients && !noStreamingClients;
    }

    /**
     * Utility method.
     */
    private String convertClientListToString(List clientList) {
        String clientsString = "";
        for (Iterator it = clientList.iterator(); it.hasNext(); ) {
            clientsString += it.next().toString() + ",";
        }
        if (clientsString.endsWith(",")) {
            clientsString = clientsString.substring(0, clientsString.length() - 1);
        }
        return clientsString;
    }

    /**
     * Determines whether this object contains a positive result of a wizard run.
     */
    public boolean hasDetectionResult() {
        return hasDetectionResult;
    }

    /**
     * Determines whether the wizard has run
     * (even returns true when the wizard has run, with an error returned)
     * @return
     */
    public boolean hasWizardRun() {
        return hasWizardRun;
    }

    /**
     * Set the mode the wizard is to be invoked in - needed when processing
     * the wizard results.
     */
    public void setMode(Mode mode) {
        this.mode = mode;
    }

    /**
     * Get the mode the wizard was last invoked in - needed when processing
     * the wizard results.
     */
    public Mode getMode() {
        return mode;
    }

    /**
     * Determine whether the wizard is being run in a hidden frame.
     */
    public boolean isInHiddenFrame() {
        // SILENT mode is run in a hidden frame
        return (mode == Mode.SILENT);
    }

    /**
     * Set whether the wizard was invoked from the toolbar - needed when
     * processing the wizard results.
     */
    public void setInvokedFromToolbar(boolean value) {
        invokedFromToolbar = value;
    }

    /**
     * Get whether the wizard was invoked from the toolbar - needed when
     * processing the wizard results.
     */
    public boolean getInvokedFromToolbar() {
        return invokedFromToolbar;
    }

    /**
     * Set whether the wizard should keep the users client choice
     * the same, despite what the wizard reports.
     * Only the extra details (zone/upgrade etc) should be
     * updated, and not the Auto/Forced or client type.
     *
     * This is used when you want to run the client detection wizard
     * only for the current client, in the hope of improving it,
     * rather than selecting a new client (e.g. zone change or upgrade).
     *
     * @param keepSameClient the keepSameClient to set
     */
    public void setKeepSameClient(boolean keepSameClient) {
        this.keepSameClient = keepSameClient;
    }

    /**
     * Get whether you should only update the
     * client information, rather than change the client.
     *
     * @return whether to keepSameClient
     */
    public boolean isKeepSameClient() {
        return keepSameClient;
    }

    /**
     * Decides if an ICO launch is possible.
     * That is if the native client is available,
     * and the site is in the correct zone, and
     * the ICO appears to be present
     *
     * @return true if you can do a scripted launch
     */
    public boolean isScriptedLocalIcaLaunchPossible() {
        if (!hasDetectionResult) {
            return false;
        }
        Client remoteClient = getRemoteClientResult();
        boolean icoLaunchPermitted = (remoteClient != null)
            && ClientType.NATIVE.equals(remoteClient.getClientType())
            && getIcoStatusResult() != null
            && getIcoStatusResult() != IcoStatus.NOT_PRESENT
            && !remoteClient.isIncorrectZone();
        return icoLaunchPermitted;
    }

    /**
     * Gets whether the streaming client is available
     * @return true, if the streaming client is detected
     */
    public boolean isRADEClientAvailable() {
        if (!hasDetectionResult) {
            return false;
        }
        Client streamingClient = getStreamingClientResult();
        boolean streamingDetected = (streamingClient != null)
            && ClientType.RADE.equals(streamingClient.getClientType());
        return streamingDetected;
    }

    /**
     * Gets the previous remote client result
     * returned by client detection wizard.
     *
     * @return the oldRemoteClientResult
     */
    public Client getOldRemoteClientResult() {
        return oldRemoteClientResult;
    }

    /**
     * Gets the previous streaming client result
     * returned by client detection wizard.
     *
     * @return the oldStreamingClientResult
     */
    public Client getOldStreamingClientResult() {
        return oldStreamingClientResult;
    }
}
