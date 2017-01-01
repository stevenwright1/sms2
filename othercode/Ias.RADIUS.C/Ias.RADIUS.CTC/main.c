#pragma unmanaged
#include "communicate.h"
#include <stdio.h>
#include "windows.h"

int main(int argc, char** argv)
{
	if ( argc < 3 ) {
        printf( "usage: %s username password", argv[0] );
    }
    else 
    {
		struct C_M_R_STRUCT mr = {0};
		CALL_MANAGED_RET* managedResponse = &mr;
		char* t1 = argv[1];
		char* t2 = argv[2];
		char c;

		DWORD nChar, dwError;
		CHAR szFileName[MAX_PATH];
		LPSTR pszSelf;		

		nChar = GetModuleFileNameA(
					NULL,
					szFileName,
					MAX_PATH
					);
		
		printf("%s\n",szFileName);
		hModulePath = szFileName;

		initManaged(hModulePath);

		callManaged(managedResponse, t1,t2);

		printf("%s\n",managedResponse->message);

		callManaged(managedResponse, t1,t2);

		printf("%s\n",managedResponse->message);

		c = getchar();
	}

    return 0;
}