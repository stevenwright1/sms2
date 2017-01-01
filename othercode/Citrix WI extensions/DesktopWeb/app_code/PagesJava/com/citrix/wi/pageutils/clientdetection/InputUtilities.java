/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.
 */
package com.citrix.wi.pageutils.clientdetection;

import com.citrix.wi.clientdetect.Client;
import com.citrix.wi.clientdetect.type.ClientType;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.pageutils.Include;
import com.citrix.wi.types.MPSClientType;

/**
 * Helper routines to generate the inputs needed by the Client
 * Detection Wizard.
 */
public class InputUtilities {
    private InputUtilities() {
    }

    /**
     * Get the user's current preferred (remote) client - the returned <code>Client</code>
     * object will indicate if this was the automatically chosen client.
     */
    public static Client getPreferredClient(WIContext wiContext) {
        Client client;
        if(Boolean.TRUE.equals(wiContext.getUserPreferences().getForcedClient())) {
            client = getClientForMPSClientType(Include.getSelectedRemoteClient(wiContext));
        } else {
            client = getClientForMPSClientType(Include.getSelectedRemoteClient(wiContext));
            client.setAutoDetected(true);
        }
        return client;
    }

    /**
     * Convert an <code>MPSClientType</code> into a <code>Client</code>.
     */
    private static Client getClientForMPSClientType(MPSClientType mpsClientType) {
        return new Client(ClientType.getClientTypeFromMPSClientType(mpsClientType));
    }

}
