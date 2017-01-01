/*
 * Copyright (c) 2007 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.controls;

/**
 * Maintains presentation state for the Silent Detection page.
 */
public class SilentDetectionPageControl {
    public boolean detectEnvironment; // Need to detect the browser environment
    public boolean detectClients; // Need to detect the installed clients
    public boolean needsReloadToKillInfoBar; // Need a reload to get rid of the ActiveX control info bar
    public boolean silentDetectionComplete; // No more silent detection is needed; redirect to the first WI page
    public boolean showRdpClassId; // The RDP class ID should be included in the (simulated) wizard output parameters
    public boolean useJavaFallback; // Attempt Java detection when remote native detection fails
    public String remoteClientToDetect; // The single remote client to detect
    public String streamingClientToDetect; // The single streaming client to detect
    public String serverRemoteClientVersion; // The version of the native remote client on the server (if available)
    public String serverStreamingClientVersion; // The version of the streaming client on the server (if available)
    public String nextPage; // The page to redirect to once silent detection has completed
}
