#pragma managed
#include "communicate.h"
#using "system.dll"
#using <mscorlib.dll>
#include <vcclr.h> // PtrToStringChars()
#include <stdio.h>
using namespace System;
using namespace System::Net;
using namespace System::Net::Sockets;
using namespace System::Threading;
using namespace System::Text;
using namespace System::IO;
using namespace System::Reflection;
using namespace System::Runtime::InteropServices;
using namespace Ias::RADIUS::CSharp;

public ref class RadiusResolver 
{
public:
	static String^ LoadPath;
	static Boolean^ Loaded;

	static void AddResolver(String^ path)
	{
		RadiusResolver::LoadPath = path;

		AppDomain::CurrentDomain->AssemblyResolve += gcnew ResolveEventHandler(&RadiusResolver::OnAssemblyResolve);
	}

	static Assembly^ OnAssemblyResolve(Object ^obj, ResolveEventArgs ^args)
	{
		array<String^>^ assemblies =
			System::IO::Directory::GetFiles(RadiusResolver::LoadPath, "*.dll");
		for (long ii = 0; ii < assemblies->Length; ii++) {
			try
			{
				AssemblyName ^name = AssemblyName::GetAssemblyName(assemblies[ii]);
				if (AssemblyName::ReferenceMatchesDefinition(gcnew AssemblyName(args->Name), name)) {
					return Assembly::Load(name);
				}
			} catch(Exception^ e)  {
				
			}
		}
		return nullptr;
	}
};

void initManaged() {
	try
	{
		DWORD nChar;
		CHAR szFileName[MAX_PATH];

		HMODULE hhModule = (HMODULE)Marshal::GetHINSTANCE(Assembly::GetExecutingAssembly()->GetModules()[0]).ToPointer();

		nChar = GetModuleFileNameA(
				hhModule,
				szFileName,
				MAX_PATH
				);

		String^ modpath = gcnew String(szFileName);
	
		modpath = Path::GetDirectoryName(modpath);
		RadiusResolver::AddResolver(modpath);

	} catch(Exception^ e) {
		/*
		String^ path = "c:\\radius\\tmp\\managed.txt";
		StreamWriter^ sw = File::AppendText( path );
		try
		{
			sw->WriteLine( e->Message );
			sw->WriteLine( e->StackTrace );
		}
		finally
		{
			if ( sw )
				delete (IDisposable^)sw;
		}
		*/
	}
}

void callManaged(CALL_MANAGED_RET * cmr, const char* pszUsername, int userLen, const char * pszPassword, const char * pszState )
{	
	System::IntPtr usernameptr(const_cast<char*>(pszUsername));
	array<Byte>^ usernameArray = gcnew array<Byte>(userLen);
	Marshal::Copy(usernameptr, usernameArray, 0, userLen);
	//pin_ptr<Byte> ptrBuffer = &usernameArray[usernameArray->GetLowerBound(0)];
	//memcpy(ptrBuffer, (void*)usernameptr, userLen);
	Encoding^ encoding = Encoding::UTF8;
	String^ username = encoding->GetString(usernameArray);

	String^ password = gcnew String(pszPassword);
	String^ state = gcnew String(pszState);

	RadiusLogic::Instance()->Init(RadiusResolver::LoadPath);
	ActOnRet^ actonret = RadiusLogic::Instance()->ActOn(
		XmlSanitizer::SanitizeXmlString(username)
		, XmlSanitizer::SanitizeXmlString(password)
		, state);
	Marshal::StructureToPtr(actonret, (IntPtr)cmr, false);
}

String^ ansi_to_managed(char *str)
{
	return Marshal::PtrToStringAnsi((IntPtr) (char *)str);
}

String^ unicode_to_managed(wchar_t *str)
{
	return Marshal::PtrToStringUni((IntPtr) (wchar_t *)str);
}
