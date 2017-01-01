package custom.auth;

/*
import com.citrix.wing.util.Base64Codec;
import java.math.BigInteger;
import java.security.*;
import java.security.SecureRandom;
import java.security.KeyFactory;
import java.security.spec.RSAPublicKeySpec;
import javax.crypto.Cipher;
*/

import java.io.*;
import java.net.*;
import java.io.IOException;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Map;
import java.util.Set;

import System.*;
import System.Net.*;
import System.Net.Sockets.*;
import System.Text.*;
import System.Text.RegularExpressions.*;
import System.Threading.*;
import System.IO.*;
import System.Xml.*;

/**
 * Base class for the business logic of the login page.
 */
public class TcpClients {
	//public static String confPath = "C:/inetpub/wwwroot/Citrix/DesktopWeb/conf/"; // Always add trailing slash
	public static String confPath = "C:/Program Files/WrightCCS2/SettingsPublic/";
	
	public static int Port = 9060;
	public static boolean RadiusShowPin = false;
	public static String CitrixWITitle = "Please enter your token code";
	public static IPAddress IP_Server = null;
	public static String AEPubKeyModulus = "0704aTJ5fG5jYRPui9ml7OIx6s2cE6QkQTaXZsDd9/BplBwVMxEUjw2HIl1D7rYdbXpSlWcSKsSWcTOsOtD3QdmD3cHAFW36pKU7q7HV8QDf2y7Sys4ATp9O4v/mTyaJ1O36xdwW/+VHAal82QDBXdDdRMKDqgKfTwcciQCZpU4mnjX4Ejme8tXys5jcWuyl4eDjebSlCWtIZktJKLwfmEv0nsntJggXObthmHnr/rW8Se2p7D7qEUyAnL+eM6DlNtx0gMeczILRx3qb4HrVzGgjYcj3RKhisG9aSIijCN8UoATsxsKhXIz0nUPjYV6nI4jUVzmctgun1RqsBnpoOw==";
	public static String AEPubKeyExponent = "AQAB";

	/**
	 * Constructor.
	 *     
	 */
	public TcpClients()
	{
		
	}
	
	public static boolean ValidatePin(String vuserName, String pinCode, String genPin, String state){
		if(IP_Server == null)
			IP_Server = IPAddress.Parse("192.168.1.234");
		if(Port==0)Port = 9060;
		TcpClient tcpClient = new TcpClient();
		String returndata = "";
		try
		{
			tcpClient.Connect(IP_Server, Port);
			NetworkStream networkStream = tcpClient.GetStream();
			if (networkStream.get_CanWrite() && networkStream.get_CanRead())
			{
				/*
				// Do a simple write.
				byte[] modBytes = Base64.decode(AEPubKeyModulus);
				byte[] expBytes = Base64.decode(AEPubKeyExponent);

				BigInteger modules = new BigInteger(1, modBytes);
				BigInteger exponent = new BigInteger(1, expBytes);
				
				KeyFactory factory = KeyFactory.getInstance("RSA");
				Cipher cipher = Cipher.getInstance("RSA");
				
				RSAPublicKeySpec pubSpec = new RSAPublicKeySpec(modules, exponent);
				PublicKey pubKey = factory.generatePublic(pubSpec);
				cipher.init(Cipher.ENCRYPT_MODE, pubKey);
				SecureRandom random = SecureRandom.getInstance("SHA1PRNG");
				byte ckey[] = new byte[31];
				random.nextBytes(ckey);
				byte civ[] = new byte[15];
				random.nextBytes(civ);
				String exchangeKeyRequest = "<AuthEngineRequest><CKey>"+Base64.encode(ckey)+"</CKey><CIV>"+Base64.encode(civ)+"</CIV></AuthEngineRequest>"
				byte[] encrypted = cipher.doFinal(exchangeKeyRequest.getBytes("UTF-8"));
				*/
				
				ubyte[] sendBytes = Encoding.get_UTF8().GetBytes("<AuthEngineRequest><Commands><ValidatePin><User>"
				+vuserName
				+"</User><Pin>"
				+genPin
				+"</Pin><PinCode>"
				+pinCode
				+"</PinCode>"
				+"<State>"
				+state
				+"</State>"
				+"</ValidatePin></Commands></AuthEngineRequest>");
				networkStream.Write(sendBytes, 0, sendBytes.length);
				// Read the NetworkStream into a byte buffer.
				ubyte[] bytes = new ubyte[tcpClient.get_ReceiveBufferSize()];
				networkStream.Read(bytes, 0, tcpClient.get_ReceiveBufferSize());
				// Output the data received from the host to the console.
				returndata = Encoding.get_UTF8().GetString(bytes);
				//Console.WriteLine("Server Returned: " + TrimA(returndata,'\0'));
			}

			if (!networkStream.get_CanRead()) 
			{
				tcpClient.Close();
			}
			else
			{
				if (!networkStream.get_CanWrite())
				{
					tcpClient.Close();
				}
			}

			if (returndata.indexOf("<Validated>true</Validated>") == -1)
			{
				return false; 
			}else{
				return true;
			}
		}
		catch(Exception ex)
		{
			Console.WriteLine(ex.ToString());
			return false;
		}
	}

	public static ValidateUserRet SendLoginDetails(String Username)
	{
		System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
		TcpClient tcpClient = new TcpClient();
		String returndata = "";
		ValidateUserRet returnObj = new ValidateUserRet();
		returnObj.setProviderLogic("");
		returnObj.setPinCodeEnabled(false);
		returnObj.setPinCodeValidated(false);
		returnObj.setState("");
		try
		{
			if(IP_Server == null)
				IP_Server = IPAddress.Parse("192.168.1.234");
			tcpClient.Connect(IP_Server, Port);
			NetworkStream networkStream = tcpClient.GetStream();
			ubyte[] sendBytes = Encoding.get_UTF8().GetBytes("<AuthEngineRequest><Commands><ValidateUser><User>"+Username+"</User></ValidateUser></Commands></AuthEngineRequest>");
			networkStream.Write(sendBytes, 0, sendBytes.length);
			// Read the NetworkStream into a byte buffer.
			ubyte[] bytes = new ubyte[tcpClient.get_ReceiveBufferSize()];
			networkStream.Read(bytes, 0, tcpClient.get_ReceiveBufferSize());
			// Output the data received from the host to the console.
			returndata = Encoding.get_UTF8().GetString(bytes);
			//Dim w As New BinaryWriter(networkStream, Encoding.UTF8)
			//w.Write(Username)
			tcpClient.Close();
			if (returndata != null && returndata != "") {
				int pnameStart = returndata.indexOf("<PName>");
				int pnameStop = returndata.indexOf("</PName>");
				if (pnameStart >= 0 && pnameStop >= 0 && pnameStart < pnameStop) {
					pnameStart+=7;
					returnObj.setProviderLogic(returndata.substring(pnameStart,pnameStop));
				}
				
				pnameStart = returndata.indexOf("<PinCodeEnabled>");
				pnameStop = returndata.indexOf("</PinCodeEnabled>");
				if (pnameStart >= 0 && pnameStop >= 0 && pnameStart < pnameStop) {
					pnameStart+=16;
					returnObj.setPinCodeEnabled((returndata.substring(pnameStart,pnameStop).equals("true")));
				}

				pnameStart = returndata.indexOf("<State>");
				pnameStop = returndata.indexOf("</State>");
				if (pnameStart >= 0 && pnameStop >= 0 && pnameStart < pnameStop) {
					pnameStart+=7;
					returnObj.setState(returndata.substring(pnameStart,pnameStop));
				}
				
				pnameStart = returndata.indexOf("<Validated>");
				pnameStop = returndata.indexOf("</Validated>");
				if (pnameStart >= 0 && pnameStop >= 0 && pnameStart < pnameStop) {
					pnameStart+=11;
					returnObj.setPinCodeValidated((returndata.substring(pnameStart,pnameStop).equals("true")));
				}

				pnameStart = returndata.indexOf("<Error>");
				pnameStop = returndata.indexOf("</Error>");
				if (pnameStart >= 0 && pnameStop >= 0 && pnameStart < pnameStop) {
					pnameStart+=7;
					returnObj.setError(returndata.substring(pnameStart,pnameStop));
				}
			}
		}
		catch(Exception Err)
		{
			Console.WriteLine(Err.ToString());
		}
		return returnObj;
	}

	private static IPAddress IPAddress()
	{
		//To get local address
		String LocalHostName;
		LocalHostName = Dns.GetHostName();
		IPHostEntry ipEnter = Dns.GetHostEntry(LocalHostName);
		IPAddress[] IpAdd = ipEnter.get_AddressList();
		return IpAdd[0];
	}

	public static void LoadSettings()
	{
		XmlDocument doc = new XmlDocument();
		String tmp = "";
		try
		{
			doc.Load(confPath+"Configuration.xml");
			XmlNodeList list = doc.GetElementsByTagName("AuthEngineServerIP");
			for (int i=0; i< list.get_Count(); i++){
				XmlElement item = (XmlElement) list.get_ItemOf(i);
				IP_Server = IPAddress.Parse(item.get_InnerText());
			}

			list = doc.GetElementsByTagName("AuthEngineServerPort");
			for (int i = 0; i < list.get_Count(); i++)
			{
				XmlElement item = (XmlElement)list.get_ItemOf(i);
				Port = Integer.parseInt(item.get_InnerText());
			}
			
			list = doc.GetElementsByTagName("RadiusShowPin");
			for (int i = 0; i < list.get_Count(); i++)
			{
				XmlElement item = (XmlElement)list.get_ItemOf(i);
				tmp = item.get_InnerText();
				RadiusShowPin = System.Boolean.Parse(tmp);
			}

			list = doc.GetElementsByTagName("CitrixWITitle");
			for (int i = 0; i < list.get_Count(); i++)
			{
				XmlElement item = (XmlElement)list.get_ItemOf(i);
				CitrixWITitle = item.get_InnerText();
			}

			list = doc.GetElementsByTagName("AuthEnginePublicKey");
			for (int i = 0; i < list.get_Count(); i++)
			{
				//XmlElement item = (XmlElement)list.get_ItemOf(i);
				//AEPubKey = item.get_InnerText();
			}
		}
		catch(Exception ex){}
	}
	
	public static String trimA(String str, char ch) {
		if (str==null) return null;
		int count = str.length();
		int len = str.length();
		int st = 0;
		int off = 0;
		
		char[] val = str.toCharArray();
		while ((st<len)&&(val[off+st] == ch)) {
			st++;
		}
		while((st < len) && (val[off+len-1] == ch)) {
			len--;
		}
		return ((st>0)||(len<count)) ? str.substring(st, len) : str;
	}
}
