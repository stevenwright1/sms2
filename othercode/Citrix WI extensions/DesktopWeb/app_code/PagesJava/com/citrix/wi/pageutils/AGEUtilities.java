package com.citrix.wi.pageutils;

import java.util.Collections;
import java.util.HashMap;
import java.util.Map;

import com.citrix.authentication.tokens.AccessToken;
import com.citrix.authentication.tokens.UPNCredentials;
import com.citrix.authentication.tokens.UserDomainPasswordCredentials;
import com.citrix.authentication.web.AuthenticationState;
import com.citrix.wi.authservice.ASClient;
import com.citrix.wi.authservice.ASCommunicationException;
import com.citrix.wi.authservice.ASOperationFailedException;
import com.citrix.wi.config.WIConfiguration;
import com.citrix.wi.config.auth.AGAuthPoint;
import com.citrix.wi.config.auth.AuthPoint;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.mvc.WebAbstraction;
import com.citrix.wi.types.AGEAccessMode;
import com.citrix.wing.MessageType;
import com.citrix.wing.types.AccessConditions;
import com.citrix.wing.util.Strings;

public class AGEUtilities {

    // Auth pages for Access Gateway integration
    public static final String AUTH_PAGE_SSO = "agesso";
    public static final String AUTH_PAGE_CALLBACK = "agecallback";
    public static final String AUTH_PAGE_PASSWORD = "agepassword";
    public static final String AUTH_PAGE_AUTHENTICATE = "ageauthenticate";
    public static final String AUTH_PAGE_CERTIFICATE = "agecertificate";

    /**
     * Query string parameter used for passing the logout ticket.
     */
    public static final String QSTR_LOGOUT_TICKET = "ticket";

    public static final String KEY_AG_AUTH_ERROR = "AGEAuthenticationError";

    private static final String SV_AGE_ACCESS_MODE       = "CTX_AgeAccessMode";
    private static final String SV_AGE_ACCESS_CONDITIONS = "CTX_AgeAccessConditions";
    private static final String SV_AGE_CLIENT_IP         = "CTX_AgeClientIP";
    private static final String SV_AGE_CLIENT_NAME       = "CTX_AgeClientName";
    private static final String SV_AGE_CHANGE_PASSWORD_URL = "CTX_AgeChangePasswordUrl";
    private static final String SV_AGE_SSL_PROXY_HOST = "CTX_AgeSSLProxyHost";

    // Currently the SDK uses the value Constants.AGE_SESSION_ID to access this information
    private static final String SV_AGE_SESSION_ID        = Constants.AGE_SESSION_ID; //"CTX_AgeSessionId";

    public static void recordAGEState(WIContext wiContext,
                                      AGEAccessMode mode,
                                      String clientIPAddress,
                                      String clientName,
                                      String ageSessionId,
                                      String sslProxyHost,
                                      AccessConditions accessConditions) {
        WebAbstraction web = wiContext.getWebAbstraction();
        web.setSessionAttribute(SV_AGE_ACCESS_MODE, mode);
        web.setSessionAttribute(SV_AGE_CLIENT_IP, clientIPAddress);
        web.setSessionAttribute(SV_AGE_CLIENT_NAME, clientName);
        web.setSessionAttribute(SV_AGE_SESSION_ID, ageSessionId);
        web.setSessionAttribute(SV_AGE_SSL_PROXY_HOST, sslProxyHost);
        web.setSessionAttribute(SV_AGE_ACCESS_CONDITIONS, accessConditions);
    }

    /**
     * Gets the URL of the Access Gateway change password page (if defined).
     * 
     * @param wiContext the WI context
     * @return the URL of the page, or null if it has not been defined
     */
    public static String getAGEChangePasswordUrl(WIContext wiContext) {
        return (String) wiContext.getWebAbstraction().getSessionAttribute(SV_AGE_CHANGE_PASSWORD_URL);
    }

    /**
     * Sets the URL of the Access Gateway change password page.
     * 
     * @param wiContext the WI context
     * @param url the URL of the page
     */
    public static void setAGEChangePasswordUrl(WIContext wiContext, String url) {
        wiContext.getWebAbstraction().setSessionAttribute(SV_AGE_CHANGE_PASSWORD_URL, url);
    }

    public static String getAGEClientIPAddress(WIContext wiContext) {
        return (String)wiContext.getWebAbstraction().getSessionAttribute(SV_AGE_CLIENT_IP);
    }

    public static String getAGEClientName(WIContext wiContext) {
        return (String)wiContext.getWebAbstraction().getSessionAttribute(SV_AGE_CLIENT_NAME);
    }

    public static String getAGESessionId(WIContext wiContext) {
        return (String)wiContext.getWebAbstraction().getSessionAttribute(SV_AGE_SESSION_ID);
    }

    public static String getSSLProxyHost(WIContext wiContext) {
        return (String) wiContext.getWebAbstraction().getSessionAttribute(SV_AGE_SSL_PROXY_HOST);
    }

    public static AccessConditions getAGEAccessConditions(WIContext wiContext) {
        return (AccessConditions)wiContext.getWebAbstraction().getSessionAttribute(SV_AGE_ACCESS_CONDITIONS);
    }

    /**
     * Gets the access mode of the site when the Web Interface is being
     * accessed via Access Gateway Enterprise. The access method determines the
     * behaviour of the site. This method will return null if called before
     * authentication has completed.
     *
     * @param wiContext the WI context
     * @return the current access mode, or null if the access mode was not
     * recognised or the site is not being accessed via AGE.
     */
    public static AGEAccessMode getAGEAccessMode(WIContext wiContext) {
        return getAGEAccessMode(wiContext.getWebAbstraction(), wiContext.getConfiguration());
    }

    /**
     * Determines if the site is running in AGE embedded (NavUI) mode.
     */
    public static boolean isAGEEmbeddedMode(WIContext wiContext) {
        return getAGEAccessMode(wiContext) == AGEAccessMode.EMBEDDED;
    }

    /**
     * Determines if the site is running in AGE embedded or indirect mode.
     */
    public static boolean isAGEEmbeddedOrIndirectMode(WIContext wiContext) {
        AGEAccessMode mode = getAGEAccessMode(wiContext);
        return (mode == AGEAccessMode.EMBEDDED) || (mode == AGEAccessMode.INDIRECT);
    }

    /**
     * Gets whether the site has been configured for integration with
     * Access Gateway.
     *
     * @param wiConfig the WebInterface configuration
     * @return true if the platform supports AGE integration and the site is
     * configured for Access Gateway integration, else false
     */
    public static boolean isAGEIntegrationEnabled(WIContext wiContext) {
        return isAGEIntegrationEnabled(wiContext.getConfiguration());
    }

    /**
     * Gets whether the site has been configured for integration with
     * Access Gateway.
     *
     * @param wiConfig the WebInterface configuration
     * @return true if the platform supports AGE integration and the site is
     * configured for Access Gateway integration, else false
     */
    public static boolean isAGEIntegrationEnabled(WIConfiguration wiConfig) {
        return wiConfig.getAuthenticationConfiguration().getAuthPoint() instanceof AGAuthPoint;
    }

    /**
     * Form of getAGEAccessMode used when initialising the WIContext (hence package/default visibility)
     */
    static AGEAccessMode getAGEAccessMode(WebAbstraction web, WIConfiguration wiConfig) {
        AGEAccessMode accessMode = (AGEAccessMode)web.getSessionAttribute(SV_AGE_ACCESS_MODE);
        return isAGEIntegrationEnabled(wiConfig) ? accessMode : null;
    }

    /**
     * Contacts the AGE authentication service and requests that it logs out
     * the AGE Session associated with Web Interface's.
     *
     * @param wiContext the Web Interface context
     */
    public static void doAGELogout(WIContext wiContext) {
        WIConfiguration wiConfig = wiContext.getConfiguration();
        WebAbstraction web = wiContext.getWebAbstraction();

        AuthPoint authPoint = wiConfig.getAuthenticationConfiguration().getAuthPoint();
        if (authPoint != null && authPoint instanceof AGAuthPoint) {
            try {
                AuthenticationState authState = (AuthenticationState)web.getRequestContextAttribute(Constants.AUTHSTATE_VARIABLE_NAME);

                // Assign the username and domain properly, depending on whether the credentials are
                // UPN or UDP
                AccessToken token = null;
                if (authState != null) {
                    token = authState.getAccessToken();
                }

                if (token != null) {
                    AGEWebServiceCredentials credentials = getAGEWebServiceCredentials(token, wiContext);
                    if (credentials != null) {
                        // Create a client for the Authentication Service
                        ASClient asClient = wiContext.getUtils().getASClient(
                                        ((AGAuthPoint)authPoint).getAGAuthenticationServiceUrl(),
                                        wiContext.getStaticEnvironmentAdaptor());

                        // Attempt the remote AGE logout
                        asClient.logoutSession(credentials.getAGESessionId(),
                            credentials.getUsername(),
                            credentials.getDomain());
                    }
                }

            } catch (ASCommunicationException asce) {
                // There was a problem communicating with the Authentication Service
                wiContext.log(MessageType.WARNING, asce.getMessageKey(), asce.getMessageArguments());
            } catch (ASOperationFailedException aofe) {
                // The Authentication Service denied the request
                wiContext.log(MessageType.WARNING, aofe.getMessageKey(), aofe.getMessageArguments());
            }
        }
    }

    /**
     * Gets the map used to store references to sessions that were started
     * through Access Gateway.
     *
     * The map is synchronized.
     *
     * @param wiContext the Web Interface context
     * @return <code>Map</code> of logout ticket to (platform-dependent) session object
     */
    public static synchronized Map getAGSessionMap(WIContext wiContext) {
        WebAbstraction web = wiContext.getWebAbstraction();

        Map logoutMap = (Map)web.getApplicationAttribute(AGE_LOGOUT_TICKET_TO_SESSION_MAP);
        if (logoutMap == null) {
            logoutMap = Collections.synchronizedMap(new HashMap());
            web.setApplicationAttribute(AGE_LOGOUT_TICKET_TO_SESSION_MAP, logoutMap);
        }

        return logoutMap;
    }

    private static final String AGE_LOGOUT_TICKET_TO_SESSION_MAP = "AGE_LOGOUT_TICKET_TO_SESSION_MAP";

    /**
     * Provides credentials required for calls to the AGE web service; specifically the AGE Session ID, and
     * the username and domain associated with that session.
     *
     * This method returns null if it failed to ascertain the properly formed credentials.
     */
    public static AGEWebServiceCredentials getAGEWebServiceCredentials(AccessToken token, WIContext wiContext) {

        if (token == null) {
            return null;
        }

        // Find the AGE Session ID associated with this WI Session.
        String ageSessionId = AGEUtilities.getAGESessionId(wiContext);
        if (Strings.isEmptyOrWhiteSpace(ageSessionId)) {
            return null;
        }

        String username = null;
        String domain = null;

        if (token instanceof UPNCredentials) {
            username = ((UPNCredentials) token).getUPN();
            domain = "";
        } else if (token instanceof UserDomainPasswordCredentials) {
            UserDomainPasswordCredentials udp = (UserDomainPasswordCredentials)token;
            username = udp.getUser();
            domain = udp.getDomain();

            if (Strings.isEmpty(domain)) {
                return null;
            }
        }

        if (Strings.isEmpty(username)) {
            return null;
        }

        return new AGEWebServiceCredentials(ageSessionId, username, domain);
    }

    public static class AGEWebServiceCredentials {
        private String ageSessionId, username, domain;
        public AGEWebServiceCredentials(String ageSessionId, String username, String domain) {
            this.ageSessionId = ageSessionId;
            this.username = username;
            this.domain = domain;
        }
        public String getAGESessionId() {
            return ageSessionId;
        }
        public String getUsername() {
            return username;
        }
        public String getDomain() {
            return domain;
        }
    }
}
