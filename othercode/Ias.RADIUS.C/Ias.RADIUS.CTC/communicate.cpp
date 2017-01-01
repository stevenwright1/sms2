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

	static void AddResolver(String^ lookFile)
	{
		RadiusResolver::LoadPath = Path::GetDirectoryName(lookFile);
		RadiusResolver::LoadPath = String::Concat(RadiusResolver::LoadPath,"\\Asm");

		array<String^>^ assemblies = System::IO::Directory::GetFiles(RadiusResolver::LoadPath, "*.dll");
		for (long ii = 0; ii < assemblies->Length; ii++) {
			Console::WriteLine(assemblies[ii]);
			try
			{
				Assembly::LoadFrom(assemblies[ii]);
			} catch(Exception^ e)  {
				
			}
		}

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

void initManaged(const char * pszModulePath) {
	RadiusResolver::AddResolver(gcnew String(pszModulePath));
}

//void callManaged(CALL_MANAGED_RET * cmr, const char * pszUsername,const char * pszPassword, const char * pszState)
void callManaged(CALL_MANAGED_RET * cmr, const char * pszUsername,const char * pszPassword)
{
	String^ path = "c:\\radius\\tmp\\managed.txt";
	String^ username = gcnew String(pszUsername);
	String^ password = gcnew String(pszPassword);
	//String^ state = gcnew String(pszState);
	/*
	StreamWriter^ sw = File::AppendText( path );
	try
	{
		sw->WriteLine( username );
		sw->WriteLine( password );
	}
	finally
	{
		if ( sw )
			delete (IDisposable^)sw;
	}
	*/
	ActOnRet^ actonret = RadiusLogic::Instance()->ActOn(username, password);
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