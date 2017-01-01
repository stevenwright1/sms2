/*
 * Copyright (c) 2001 - 2010 Citrix Systems, Inc.  All Rights Reserved.
 */

package com.citrix.wi.controls;

import custom.auth.*;

/**
 * Maintains presentation state for the change system PIN page.
 */
public class CheckPinPageControl extends DialogPageControl
{

	private String systemPin = "";
	private String providerLogic = "";
	private String state = "";
	private boolean pinCodeEnabled;
	private boolean pinCodeValidated = false;

	/**
     * Gets the system PIN for this page.
	 * @return the system PIN
	 */
		public String getSystemPin()
	{
		return systemPin;
	}

	/**
     * Sets the system PIN for this page.
     * @param systemPin the system PIN
	 */
		public void setSystemPin(String systemPin)
	{
				this.systemPin = systemPin;
	}

	public String getProviderLogic()
	{
		return this.providerLogic;
	}

	public void setProviderLogic(String pLogic)
	{
		this.providerLogic = pLogic;
	}
	
	public boolean getPinCodeEnabled()
	{
		return this.pinCodeEnabled;
	}

	public void setPinCodeEnabled(boolean pcEnabled)
	{
		this.pinCodeEnabled = pcEnabled;
	}

	public String getState()
	{
		return this.state;
	}

	public void setState(String newstate)
	{
		this.state = newstate;
	}
	
	public boolean getPinCodeValidated()
	{
		return this.pinCodeValidated;
	}

	public void setPinCodeValidated(boolean pcValidated)
	{
		this.pinCodeValidated = pcValidated;
	}
	
	public String getTitleClass() {
		return "";
	}

	public String getTitle() {
		return TcpClients.CitrixWITitle;
	}
}
