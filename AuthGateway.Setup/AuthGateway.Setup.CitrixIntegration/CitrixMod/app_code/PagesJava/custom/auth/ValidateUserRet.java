package custom.auth;

import java.io.*;
import java.net.*;
import java.io.IOException;

import System.*;
import System.Text.*;

public class ValidateUserRet {
	private String providerLogic = "";
	private String state = "";
	private String error = "";
	private boolean pinCodeEnabled;
	private boolean pinCodeValidated = false;

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

	public String getError()
	{
		return this.error;
	}

	public void setError(String newerror)
	{
		this.error = newerror;
	}
	
	public boolean getPinCodeValidated()
	{
		return this.pinCodeValidated;
	}

	public void setPinCodeValidated(boolean pcValidated)
	{
		this.pinCodeValidated = pcValidated;
	}
}