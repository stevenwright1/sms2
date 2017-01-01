package com.citrix.wi.controls;

import com.citrix.wing.ClientParams;

/**
 * Summary description for AppEmbedControl.
 */
public class AppEmbedControl
{
    public String homePage;
    public String client;

    public String appName;
    public String icaClientClassID;
    public String rdpClientClassID;
    public String sessionDisconnected;
    public String sessionQuestionDisconnect;

    public String desiredHRES;
    public String desiredVRES;

    public ClientParams rdpParams;
    public String appSizeString;
    public String initTag = "";
    public String jicaCode;
    public String jicaPackages;
    public String jicaLang = "";
	public boolean jicaUseZeroLatency = false;
	public String closeURL = null;
    public String rdpLaunchErrorKey = "";
    public String clientPath = null;
    public String JICACookie = "";

    public String radeClientClassID;
    public String QS;

    // Error conditions
    public String redirectUrl = null;

    public AppEmbedControl()
    {
    }

    public boolean isParamDesktopWidth(String paramName)
    {
        return "DesktopWidth".equals(paramName);
    }

    public boolean isParamDesktopHeight(String paramName)
    {
        return "DesktopHeight".equals(paramName);
    }

    public boolean isParamRedirectDrive(String paramName)
    {
        return "AdvancedSettings2.RedirectDrives".equals(paramName);
    }

}
