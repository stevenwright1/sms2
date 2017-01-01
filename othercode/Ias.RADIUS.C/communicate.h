#include "windows.h"

typedef struct C_M_R_STRUCT
{
    int status;
    char *state;
    char *message;
	char *password;
	char *groups;
    int panic;
} CALL_MANAGED_RET;
#ifdef __cplusplus
extern "C" {  
#endif
void initManaged();
void callManaged(CALL_MANAGED_RET * cmr, const char * pszUsername, int userLen, const char * pszPassword, const char * pszState);
#ifdef __cplusplus
} 
#endif
#ifdef __cplusplus
extern "C" {  
#endif
const char * hModulePath;
static HMODULE hModuleInstance;
#ifdef __cplusplus
} 
#endif