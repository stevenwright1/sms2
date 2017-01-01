/*
 * Copyright (c) 2005 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */
package com.citrix.wi.clientdetect.models;

public class FinishViewModel extends WizardViewModel {
    public String redirectURL = null;

    public boolean showAlternateResult = false;
    public String alternateResult = null;

    public boolean showRDPClassID = false;
    public String RDPClassID = null;

    public String remoteClientResult = null;
    public String streamingClientResult = null;
    public String icoStatusResult = null;

    public boolean reloadingPage = false;

}
