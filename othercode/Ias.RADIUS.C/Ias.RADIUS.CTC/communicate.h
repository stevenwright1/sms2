typedef struct C_M_R_STRUCT
{
    int status;
    char *state;
    char *message;
} CALL_MANAGED_RET;
#ifdef __cplusplus
extern "C" {  
#endif
void initManaged(const char * pszModulePath);
//void callManaged(CALL_MANAGED_RET * cmr, const char * pszUsername,const char * pszPassword, const char * pszState);
void callManaged(CALL_MANAGED_RET * cmr, const char * pszUsername,const char * pszPassword);
#ifdef __cplusplus
} 
#endif

const char * hModulePath;