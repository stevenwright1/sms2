/* Copyright (c) Microsoft Corporation. All rights reserved. */
#pragma unmanaged
#include "windows.h"
#include "radutil.h"
#include "communicate.h"

extern LPCWSTR pwszDllType;

static HMODULE hModule;

BOOL
WINAPI
DllMain(
   HINSTANCE hInstance,
   DWORD dwReason,
   LPVOID lpReserved
   )
{
	DWORD nChar;
	CHAR szFileName[MAX_PATH];

   if (dwReason == DLL_PROCESS_ATTACH)
   {
      hModule = hInstance;
	  
		if ((hModule == NULL))
		{
			return ERROR_INVALID_PARAMETER;
		}
		
		DisableThreadLibraryCalls(hInstance);

		hModuleInstance = hInstance;

		nChar = GetModuleFileNameA(
			hModule,
			szFileName,
			MAX_PATH
			);

		hModulePath = szFileName;
   }

   return TRUE;
}

STDAPI
DllRegisterServer( VOID )
{
   return RadiusExtensionInstall(hModule, pwszDllType, TRUE);
}

STDAPI
DllUnregisterServer( VOID )
{
   return RadiusExtensionInstall(hModule, pwszDllType, FALSE);
}
