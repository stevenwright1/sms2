/* Copyright (c) Microsoft Corporation. All rights reserved. */
#pragma unmanaged
#include "windows.h"
#include "authif.h"
#include "lmcons.h"
#include "ntsecapi.h"
#include "radutil.h"
#include "communicate.h"
#include <stdio.h>

/* The registry value where this extension is registered. */
LPCWSTR pwszDllType = AUTHSRV_EXTENSIONS_VALUE_W;

/* Global handle to the LSA. This is initialized once at start-up and reused
 * until shutdown. */
LSA_HANDLE hPolicy = NULL;

DWORD
WINAPI
RadiusExtensionInit( VOID )
{
   NTSTATUS status;
   LSA_OBJECT_ATTRIBUTES objectAttributes;

   memset(&objectAttributes, 0, sizeof(objectAttributes));
   status = LsaOpenPolicy(
			   NULL,
			   &objectAttributes,
			   POLICY_LOOKUP_NAMES,
			   &hPolicy
			   );
   /*
   nChar = GetModuleFileNameA(
			hModuleInstance,
			szFileName,
			MAX_PATH
			);

   initManaged(szFileName);
   */
   initManaged();

   return LsaNtStatusToWinError(status);
}


VOID
WINAPI
RadiusExtensionTerm( VOID )
{
   LsaClose(hPolicy);
   hPolicy = NULL;
}

_Bool is_utf8(const char * string)
{
	if (!string)
		return 0;

	const unsigned char * bytes = (const unsigned char *)string;
	while (*bytes)
	{
		if ((// ASCII
			// use bytes[0] <= 0x7F to allow ASCII control characters
			bytes[0] == 0x09 ||
			bytes[0] == 0x0A ||
			bytes[0] == 0x0D ||
			(0x20 <= bytes[0] && bytes[0] <= 0x7E)
			)
			) {
			bytes += 1;
			continue;
		}

		if ((// non-overlong 2-byte
			(0xC2 <= bytes[0] && bytes[0] <= 0xDF) &&
			(0x80 <= bytes[1] && bytes[1] <= 0xBF)
			)
			) {
			bytes += 2;
			continue;
		}

		if ((// excluding overlongs
			bytes[0] == 0xE0 &&
			(0xA0 <= bytes[1] && bytes[1] <= 0xBF) &&
			(0x80 <= bytes[2] && bytes[2] <= 0xBF)
			) ||
			(// straight 3-byte
			((0xE1 <= bytes[0] && bytes[0] <= 0xEC) ||
			bytes[0] == 0xEE ||
			bytes[0] == 0xEF) &&
			(0x80 <= bytes[1] && bytes[1] <= 0xBF) &&
			(0x80 <= bytes[2] && bytes[2] <= 0xBF)
			) ||
			(// excluding surrogates
			bytes[0] == 0xED &&
			(0x80 <= bytes[1] && bytes[1] <= 0x9F) &&
			(0x80 <= bytes[2] && bytes[2] <= 0xBF)
			)
			) {
			bytes += 3;
			continue;
		}

		if ((// planes 1-3
			bytes[0] == 0xF0 &&
			(0x90 <= bytes[1] && bytes[1] <= 0xBF) &&
			(0x80 <= bytes[2] && bytes[2] <= 0xBF) &&
			(0x80 <= bytes[3] && bytes[3] <= 0xBF)
			) ||
			(// planes 4-15
			(0xF1 <= bytes[0] && bytes[0] <= 0xF3) &&
			(0x80 <= bytes[1] && bytes[1] <= 0xBF) &&
			(0x80 <= bytes[2] && bytes[2] <= 0xBF) &&
			(0x80 <= bytes[3] && bytes[3] <= 0xBF)
			) ||
			(// plane 16
			bytes[0] == 0xF4 &&
			(0x80 <= bytes[1] && bytes[1] <= 0x8F) &&
			(0x80 <= bytes[2] && bytes[2] <= 0xBF) &&
			(0x80 <= bytes[3] && bytes[3] <= 0xBF)
			)
			) {
			bytes += 4;
			continue;
		}

		return 0;
	}

	return 1;
}


DWORD
WINAPI
RadiusExtensionProcess2(
   PRADIUS_EXTENSION_CONTROL_BLOCK pECB
   )
{
   BYTE vePassByte[300] = {0};
   PRADIUS_ATTRIBUTE_ARRAY pInAttrs;
   PRADIUS_ATTRIBUTE_ARRAY pOutAttrs;
   const RADIUS_ATTRIBUTE* pAttr;
   const RADIUS_ATTRIBUTE* pAttrPass;
   const RADIUS_ATTRIBUTE* pAttrState;


   const char* blankPassword = "";
   const char* blankstate = "";
   const char* filterId = "sms2";
   const DWORD vendorID = 46122;
   struct C_M_R_STRUCT mr = {0};
   CALL_MANAGED_RET* managedResponse = &mr;
   RADIUS_ATTRIBUTE raRepMessage;
   RADIUS_ATTRIBUTE raRepState;
   RADIUS_ATTRIBUTE raFilterId;

   RADIUS_ATTRIBUTE raPass;
   RADIUS_ATTRIBUTE raGroups;
   RADIUS_ATTRIBUTE raPanic;
   RADIUS_VSA_FORMAT *vePass;
   RADIUS_VSA_FORMAT *veGroups;
   RADIUS_VSA_FORMAT *vePanic;

   FILE *f;

   // Authorization using ratFilterId == "sms2"
   if (pECB->repPoint == repAuthorization)
   {
		pInAttrs = pECB->GetResponse(pECB, rcAccessAccept);
		if ( RadiusHasString(pInAttrs, ratFilterId, filterId) != RADIUS_ATTR_NOT_FOUND) {
			pECB->SetResponseType(pECB, rcAccessAccept);
		}
		return NO_ERROR;
   }
   
   if (pECB->repPoint != repAuthentication)
   {
	  return NO_ERROR;
   }


   /* We only process Access-Requests. */
   if (pECB->rcRequestType != rcAccessRequest)
   {
	  return NO_ERROR;
   }

   /* Don't process if it's already been rejected. */
   /*if (pECB->rcResponseType == rcAccessReject)
   {
	  return NO_ERROR;
   }*/

   /* Get the attributes from the Access-Request. */
   pInAttrs = pECB->GetRequest(pECB);

   /* Only process Windows users */
   pAttr = RadiusFindFirstAttribute(pInAttrs, ratProvider);
   if ((pAttr == NULL) || (pAttr->dwValue != rapWindowsNT))
   {
	  return NO_ERROR;
   }
   /* Retrieve the username. If it doesn't exist, there's nothing to do. */
   pAttr = RadiusFindFirstAttribute(pInAttrs, ratUserName);
   if (pAttr == NULL)
   {
	  return NO_ERROR;
   }

   pAttrPass = RadiusFindFirstAttribute(pInAttrs, ratUserPassword);
   if (pAttrPass != NULL)
   {
	   blankPassword = (const char *)pAttrPass->lpValue;
   }

   pAttrState = RadiusFindFirstAttribute(pInAttrs, ratState);
   if (pAttrState != NULL)
   {
	   blankstate = (const char *)pAttrState->lpValue;
   }
   

   callManaged(managedResponse, (const char *)pAttr->lpValue, pAttr->cbDataLength, blankPassword, blankstate);
       
   if (managedResponse->status==0) { // Reject access
	   //return ERROR_BAD_USERNAME;

		raRepMessage.dwAttrType = ratReplyMessage;
		raRepMessage.fDataType = rdtString;
		raRepMessage.lpValue = (const BYTE *)managedResponse->message;
		raRepMessage.cbDataLength = strlen(managedResponse->message);
	
		pOutAttrs = pECB->GetResponse(pECB, rcAccessReject);
		RadiusReplaceFirstAttribute(pOutAttrs, &raRepMessage);

		pECB->SetResponseType(pECB, rcAccessReject);
		return NO_ERROR;
   } else 
   // Response Challenge
   if (managedResponse->status==1) { // ValidateUser succesfull, proceed to ValidatePin

		raRepMessage.dwAttrType = ratReplyMessage;
		raRepMessage.fDataType = rdtString;		

		int     nLength, nLength2;
		BSTR    bstrCode;
		char*   pszUTFCode = NULL;

		if (is_utf8(managedResponse->message)) {

			nLength = MultiByteToWideChar(CP_ACP, 0, managedResponse->message, lstrlen(managedResponse->message), NULL, 0);
			bstrCode = SysAllocStringLen(NULL, nLength);
			MultiByteToWideChar(CP_ACP, 0, managedResponse->message, lstrlen(managedResponse->message), bstrCode, nLength);

			nLength2 = WideCharToMultiByte(CP_UTF8, 0, bstrCode, -1, pszUTFCode, 0, NULL, NULL);
			pszUTFCode = (char*)malloc(nLength2);
			WideCharToMultiByte(CP_UTF8, 0, bstrCode, -1, pszUTFCode, nLength2, NULL, NULL);

			raRepMessage.lpValue = (const BYTE *)pszUTFCode;
			raRepMessage.cbDataLength = nLength2 - 1;
		}
		else {
			if (managedResponse->message == NULL) {
				raRepMessage.lpValue = NULL;
				raRepMessage.cbDataLength = 0;
			}
			else {
				raRepMessage.lpValue = (const BYTE *)managedResponse->message;
				raRepMessage.cbDataLength = strlen(managedResponse->message);
			}
		}
	
		pOutAttrs = pECB->GetResponse(pECB, rcAccessChallenge);
		RadiusReplaceFirstAttribute(pOutAttrs, &raRepMessage);

		raRepState.dwAttrType = ratState;
		raRepState.fDataType = rdtString;
		raRepState.lpValue = (const BYTE *)managedResponse->state;
		raRepState.cbDataLength = strlen(managedResponse->state);
	
		pOutAttrs = pECB->GetResponse(pECB, rcAccessChallenge);
		RadiusReplaceFirstAttribute(pOutAttrs, &raRepState);

		if (managedResponse->panic != NULL && managedResponse->panic == 1) {
			vePanic = (RADIUS_VSA_FORMAT *)malloc(sizeof(RADIUS_VSA_FORMAT) + sizeof(int));
			memset(vePanic, 0, sizeof(RADIUS_VSA_FORMAT) + sizeof(int) + 1);
			vePanic->VendorId[0] = HIBYTE(HIWORD(vendorID));
			vePanic->VendorId[1] = LOBYTE(HIWORD(vendorID));
			vePanic->VendorId[2] = HIBYTE(LOWORD(vendorID));
			vePanic->VendorId[3] = LOBYTE(LOWORD(vendorID));

			vePanic->VendorType = 5;
			vePanic->VendorLength = 2 + sizeof(int);
			vePanic->AttributeSpecific[0] = 1;

			raPanic.dwAttrType = ratVendorSpecific;
			raPanic.fDataType = rdtString;
			raPanic.lpValue = (const BYTE *)vePanic;
			raPanic.cbDataLength = 7 + vePanic->VendorLength;

			pOutAttrs = pECB->GetResponse(pECB, rcAccessChallenge);
			RadiusReplaceFirstAttribute(pOutAttrs, &raPanic);
		}

		pECB->SetResponseType(pECB, rcAccessChallenge);
		return NO_ERROR;
   } else 
   // Response Accept
   if (managedResponse->status==2) { // ValidatePin, auth succesfull
		raRepMessage.dwAttrType = ratReplyMessage;
		raRepMessage.fDataType = rdtString;
		raRepMessage.lpValue = (const BYTE *)managedResponse->message;
		raRepMessage.cbDataLength = strlen(managedResponse->message);
	
		pOutAttrs = pECB->GetResponse(pECB, rcAccessAccept);
		RadiusReplaceFirstAttribute(pOutAttrs, &raRepMessage);

		raRepState.dwAttrType = ratState;
		raRepState.fDataType = rdtString;
		raRepState.lpValue = (const BYTE *)managedResponse->state;
		raRepState.cbDataLength = strlen(managedResponse->state);
	
		pOutAttrs = pECB->GetResponse(pECB, rcAccessAccept);
		RadiusReplaceFirstAttribute(pOutAttrs, &raRepState);

		// Authorization using ratFilterId == "sms2"

		raFilterId.dwAttrType = ratFilterId;
		raFilterId.fDataType = rdtString;
		raFilterId.lpValue = (const BYTE *)filterId;
		raFilterId.cbDataLength = strlen(filterId);
	
		pOutAttrs = pECB->GetResponse(pECB, rcAccessAccept);
		RadiusReplaceFirstAttribute(pOutAttrs, &raFilterId);

        if ( managedResponse->panic != NULL && managedResponse->panic == 1 ) {			
			vePanic = (RADIUS_VSA_FORMAT *)malloc(sizeof(RADIUS_VSA_FORMAT) + sizeof(int));
			memset(vePanic, 0, sizeof(RADIUS_VSA_FORMAT) + sizeof(int) + 1);
			vePanic->VendorId[0] = HIBYTE(HIWORD(vendorID));
			vePanic->VendorId[1] = LOBYTE(HIWORD(vendorID));
			vePanic->VendorId[2] = HIBYTE(LOWORD(vendorID));
			vePanic->VendorId[3] = LOBYTE(LOWORD(vendorID));

			vePanic->VendorType = 5;
			vePanic->VendorLength = 2 + sizeof(int);
            vePanic->AttributeSpecific[0] = 1;

			raPanic.dwAttrType = ratVendorSpecific;
			raPanic.fDataType = rdtString;
			raPanic.lpValue = (const BYTE *)vePanic;
			raPanic.cbDataLength = 7 + vePanic->VendorLength;

			pOutAttrs = pECB->GetResponse(pECB, rcAccessAccept);
			RadiusReplaceFirstAttribute(pOutAttrs, &raPanic);
		}

		if ( managedResponse->password != NULL ) {
			/*
			f = fopen("C:\\temp\\file.txt", "a");
			fprintf(f, "!= NULL\n");
			fclose(f);
			
			f = fopen("C:\\temp\\file.txt", "a");
			fprintf(f, "managedResponse->password = %s\n", managedResponse->password);
			fprintf(f, "strlen(managedResponse->password) == %d\n", strlen(managedResponse->password));
			fclose(f);
			*/
			
			vePass = (RADIUS_VSA_FORMAT *)malloc(sizeof(RADIUS_VSA_FORMAT) + strlen(managedResponse->password) + 1);
			memset(vePass, 0, sizeof(RADIUS_VSA_FORMAT) + strlen(managedResponse->password) + 1);
			vePass->VendorId[0] = HIBYTE(HIWORD(vendorID));
			vePass->VendorId[1] = LOBYTE(HIWORD(vendorID));
			vePass->VendorId[2] = HIBYTE(LOWORD(vendorID));
			vePass->VendorId[3] = LOBYTE(LOWORD(vendorID));

			vePass->VendorType = 5;
			vePass->VendorLength = 2 + strlen(managedResponse->password);
			strncpy((char *)vePass->AttributeSpecific, managedResponse->password, strlen(managedResponse->password));
			/*
			f = fopen("C:\\temp\\file.txt", "a");
			fprintf(f, "== %s\n", vePass->AttributeSpecific);
			fclose(f);
			*/
			raPass.dwAttrType = ratVendorSpecific;
			raPass.fDataType = rdtString;
			raPass.lpValue = (const BYTE *)vePass;
			raPass.cbDataLength = 7 + vePass->VendorLength;

			pOutAttrs = pECB->GetResponse(pECB, rcAccessAccept);
			RadiusReplaceFirstAttribute(pOutAttrs, &raPass);
		} else {
			/*
			f = fopen("C:\\temp\\file.txt", "a");
			fprintf(f, "== NULL\n");
			fclose(f);
			*/
		}
		/*
		if ( managedResponse->groups != NULL ) {
			raGroups.dwAttrType = ratVendorSpecific;
			raGroups.fDataType = rdtString;
			raGroups.lpValue = (const BYTE *)managedResponse->groups;
			raGroups.cbDataLength = strlen(managedResponse->groups);

			pOutAttrs = pECB->GetResponse(pECB, rcAccessAccept);
			RadiusReplaceFirstAttribute(pOutAttrs, &raGroups);
		}
		*/

		pECB->SetResponseType(pECB, rcAccessAccept);
		return NO_ERROR;
   } else 
   // Response Accept NO STATE
   if (managedResponse->status==4) { // ValidateUser+ValidatePin
		raRepMessage.dwAttrType = ratReplyMessage;
		raRepMessage.fDataType = rdtString;
		raRepMessage.lpValue = (const BYTE *)managedResponse->message;
		raRepMessage.cbDataLength = strlen(managedResponse->message);
	
		pOutAttrs = pECB->GetResponse(pECB, rcAccessAccept);
		RadiusReplaceFirstAttribute(pOutAttrs, &raRepMessage);

		raFilterId.dwAttrType = ratFilterId;
		raFilterId.fDataType = rdtString;
		raFilterId.lpValue = (const BYTE *)filterId;
		raFilterId.cbDataLength = strlen(filterId);
	
		pOutAttrs = pECB->GetResponse(pECB, rcAccessAccept);
		RadiusReplaceFirstAttribute(pOutAttrs, &raFilterId);

		pECB->SetResponseType(pECB, rcAccessAccept);
		return NO_ERROR;
   } else 
   // Flood, drop the packet
   if (managedResponse->status==3) {
	   return ERROR_BAD_USERNAME;
   }

   //pECB->SetResponseType(pECB, rcAccessReject);
   //return ERROR_BAD_USERNAME;
   return NO_ERROR;
}
