using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;

using java.util;

using Citrix.DeliveryServices.ProtocolTransition.ClientProxy;
using Citrix.DeliveryServices.Security.Claims;
using Citrix.DeliveryServices.Security.Claims.Abstractions;
using Citrix.DeliveryServices.Security.Claims.Interfaces;
using Citrix.DeliveryServices.Security.Messages;
using Citrix.DeliveryServices.Security.Tokens.Interfaces;

using com.citrix.authentication.tokens;
using com.citrix.authentication.tokens.age;
using com.citrix.wi.config;
using com.citrix.wi.config.auth;
using com.citrix.wi.types;
using com.citrix.wi.controls;
using com.citrix.wi.mvc;
using com.citrix.wi.pageutils;
using com.citrix.wi.util;
using com.citrix.wing;
using com.citrix.wing.types;
using com.citrix.wing.webpn;

namespace com.citrix.wi.pages.auth.age
{
    /// <summary>
    /// Implements smart card authentication from Access Gateway.
    /// </summary>
    public class Certificate : StandardPage
    {
        #region Protocol Transition Service support

        /// <summary>
        /// The token service generating tokens suitable for sending to the
        /// Protocol Transition Service.
        /// </summary>
        private readonly ITokenService tokenService;

        /// <summary>
        /// The calling endpoint for the Protocol Transition Service, as defined
        /// in web.config.
        /// </summary>
        private readonly string ptsEndpoint;

        /// <summary>
        /// The URL for the the Protocol Transition Service.  This should match
        /// the allowed hosts directive in the PTS app.config file, but doesn't 
        /// need to be read from that file.  
        /// </summary>
        private const string SOURCE_HOST = "http://localhost";

        /// <summary>
        /// The configuration key defining the Protocol Transition Service endpoint.
        /// </summary>
        private const string CONF_KEY_AG_PTS_ENDPOINT = "AG_PTS_ENDPOINT";

        #endregion

        private readonly bool useProtocolTransition;

        public Certificate(WIContext wiContext) : base(wiContext)
        {
            WIConfiguration config = wiContext.getConfiguration();
            AuthenticationConfiguration authConfig = config.getAuthenticationConfiguration();
            AGAuthPoint authPoint = authConfig.getAuthPoint() as AGAuthPoint;

            if (authPoint != null)
            {
                useProtocolTransition = authPoint.getAuthenticationMethod() ==
                                        AGAuthenticationMethod.SMART_CARD_KERBEROS;
            }

            if (useProtocolTransition)
            {
                WebAbstraction web = wiContext.getWebAbstraction();
                ptsEndpoint = web.getConfigurationAttribute(CONF_KEY_AG_PTS_ENDPOINT);

                StaticEnvironmentAdaptor envAdaptor = wiContext.getStaticEnvironmentAdaptor();
                tokenService = envAdaptor.getApplicationAttribute(
                    AppAttributeKey.PTS_TOKEN_SERVICE_KEY) as ITokenService;
            }
        }

        public override bool performImp()
        {
            if (AGEUtilities.isAGEIntegrationEnabled(wiContext) && tryLoginUser())
            {
                // Auth successful, redirect user
                LaunchUtilities.transferLaunchDataToSession(wiContext);
                Authentication.redirectToCurrentAuthPage(wiContext);
            }
            else
            {
                // Auth failed, clean up
                WebAbstraction web = wiContext.getWebAbstraction();
                web.abandonSession();
                web.setResponseStatus(WebAbstraction.SC_UNAUTHORIZED);
                web.writeToResponse(wiContext.getString(AGEUtilities.KEY_AG_AUTH_ERROR));
            }

            return false;
        }

        private bool tryLoginUser()
        {
            AGUserIdentityToken credentialsFromAG = getCredentialsFromAG();
            if (credentialsFromAG == null)
            {
                return false;
            }

            SIDBasedToken accessToken = useProtocolTransition ?
                getAccessTokenUsingProtocolTransition(credentialsFromAG) :
                getValidAccessToken(credentialsFromAG);

            if (accessToken == null)
            {
                return false;
            }

            return tryLoginUserToWI(accessToken);
        }

        private AGUserIdentityToken getCredentialsFromAG()
        {
            WebAbstraction web = wiContext.getWebAbstraction();
            
            Map parameters = Authentication.getAuthenticationState(web).getParameters() as Map;
            if (parameters == null)
            {
                return null;
            }

            return parameters.get(Constants.AGE_ACCESS_TOKEN) as AGUserIdentityToken;
        }

        private AGUserIdentityToken getValidAccessToken(AGUserIdentityToken credentialsFromAG)
        {
            if (testForValidCredentials(credentialsFromAG))
            {
                return credentialsFromAG;
            }

            return null;
        }

        private SIDBasedToken getAccessTokenUsingProtocolTransition(AGUserIdentityToken credentialsFromAG)
        {
            if (tokenService == null)
            {
                return null;
            }

            WindowsIdentity windowsIdentity = doProtocolTransition(credentialsFromAG);
            if (windowsIdentity == null)
            {
                return null;
            }

            return new WindowsToken(windowsIdentity, true);
        }

        private bool testForValidCredentials(AGUserIdentityToken credentials)
        {
            // Creates an object for pre authenticated user.
            AccessTokenValidity validAccessToken = Authentication.validateAccessTokenWithExpiry(
                wiContext.getConfiguration(), null, credentials);
            AccessTokenValidationResult result = validAccessToken.getValidationResult();

            return result.isSuccess();
        }

        private bool tryLoginUserToWI(SIDBasedToken accessToken)
        {
            // If the roaming user feature is enabled, the access token needs to be populated with the SIDs of the AD groups
            // the user belongs to.
            StatusMessage statusMessage = DisasterRecoveryUtils.fillInUserIdentityInfoIfNeeded(wiContext, accessToken);
            if (statusMessage != null)
            {
                logStatusMessage(statusMessage);
                return false;
            }

            WebAbstraction web = wiContext.getWebAbstraction();
            Authentication.getAuthenticationState(web).setAuthenticated(accessToken);

            // Create a UserContext to hold user-specific app state; this
            // is null if the creation failed, which indicates some horrible
            // problem has arisen somewhere.
            UserContext userContext = SessionUtils.createNewUserContext(wiContext);
            return (userContext != null);
        }

        /// <summary>
        /// Handles the provided status message and then kills the session.
        /// </summary>
        /// <param name="statusMessage">The status message to log and/or display</param>
        private void logStatusMessage(StatusMessage statusMessage)
        {
            // Log the message
            object[] logMessageArgs = statusMessage.getLogMessageArgs();
            MessageType logMessageType = statusMessage.getType();
            string logMessageKey = statusMessage.getLogMessageKey();

            if (logMessageArgs != null)
            {
                wiContext.log(logMessageType, logMessageKey, logMessageArgs);
            }
            else
            {
                wiContext.log(logMessageType, logMessageKey);
            }
        }

        private WindowsIdentity doProtocolTransition(AGUserIdentityToken token)
        {
            ProtocolTransitionServiceProxy proxy = new ProtocolTransitionServiceProxy();
            try
            {
                proxy.Open();

                WindowsIdentity identity = proxy.GetIdentity(getSerializedPTSTokenFromUserIdentityToken(token));

                proxy.Close();

                return identity;
            }
            catch (SecurityAccessDeniedException ex)
            {
                // May occur if an admin has not restarted the machine, or if
                // the user is not specified on the PTSAccess.txt file.  Potentially also
                // if the app pool identity is not added to the CitrixPTServiceUsers group.
                logProtocolTransitionFailedMessage(token.getUserIdentity(), ex.Message);
                return null;
            }
            catch (CryptographicException ex)
            {
                // May occur if an admin has not restarted the machine, or if the 
                // web server certificate's private key is unavailable to the 
                // app pool identity somehow.
                logProtocolTransitionFailedMessage(token.getUserIdentity(), ex.Message);
                return null;
            }
            finally
            {
                if (proxy.State != CommunicationState.Closed)
                {
                    proxy.Abort();
                }
            }
        }

        private void logProtocolTransitionFailedMessage(string userIdentity, string exceptionMessage)
        {
            wiContext.log(MessageType.ERROR, "ProtocolTransitionFailed",
                    new object[] { userIdentity, exceptionMessage });
        }

        /// <summary>
        /// This method converts the username into a serialized token that can be
        /// given to the protocol transition service to assert that we have 
        /// authenticated the user.
        /// </summary>
        /// <param name="token">The AGUserIdentityToken for the user for whom the 
        /// serialized token should be returned.</param>
        /// <returns>The serialized token</returns>
        private string getSerializedPTSTokenFromUserIdentityToken(AGUserIdentityToken token)
        {
            if (token == null)
            {
                throw new ArgumentNullException("token");
            }

            if (tokenService == null)
            {
                return null;
            }

            IClaimsPrincipalFactory claimsPrincipalFactory = tokenService.ClaimsPrincipalFactory;
            if (claimsPrincipalFactory == null)
            {
                return null;
            }

            IIdentity identity = new IdentityBase(token.getUserIdentity(), "Identify", true);
            IClaimsPrincipal principal = claimsPrincipalFactory.CreatePrincipal(identity, new List<Claim>());
            if (principal == null)
            {
                return null;
            }

            RequestToken requestToken = buildRequestToken();
            IToken tokenObj = tokenService.TokenForService(principal, requestToken);

            if (tokenObj == null)
            {
                return null;    
            }

            return tokenObj.Serialize();
        }

        private RequestToken buildRequestToken()
        {
            RequestToken requestToken = new RequestToken();
            requestToken.RequestedLifetime = new TimeSpan(1, 0, 0).ToString(); // TTL: 1h
            requestToken.RequestTokenTemplate = "";
            requestToken.ServiceId = ptsEndpoint;
            requestToken.ServiceUrl = SOURCE_HOST;

            return requestToken;
        }
    }
}