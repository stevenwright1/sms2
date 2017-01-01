package com.citrix.wi.pageutils;

import java.util.List;

import com.citrix.authentication.tokens.SIDBasedToken;
import com.citrix.wi.mvc.StatusMessage;
import com.citrix.wi.mvc.WIContext;
import com.citrix.wi.util.AppAttributeKey;
import com.citrix.wing.MessageType;
import com.citrix.wing.SourceIOException;
import com.citrix.wing.SourceUnavailableException;
import com.citrix.wing.webpn.WebPN;

/**
 * A collection of useful methods for working with the Disaster Recovery feature.
 */
public class DisasterRecoveryUtils {

    /**
     * Checks whether the Roaming User feature is enabled, and calls
     * <code>fillInUserIdentityInfo</code> on the user's primary WebPN.  Handles
     * any exceptions thrown and returns a <code>StatusMessage</code> indicating
     * the failure, or null if the call was successful.
     * @param wiContext The WIContext object
     * @param sidBasedToken The user's access token.
     * @return <code>null</code> on success, otherwise a
     * <code>StatusMessage</code> for displaying to the user.
     */
    public static StatusMessage fillInUserIdentityInfoIfNeeded(WIContext wiContext, SIDBasedToken sidBasedToken)
    {
        if (wiContext.getConfiguration().isRoamingUserEnabled())
        {
            List webPNs = getWebPNs(wiContext);

            // Roaming user only applies to normal (non-recovery) farms, so don't
            // loop through the entire list.
            try
            {
                WebPN webPN = (WebPN)webPNs.get(Constants.INDEX_PRIMARY_WEB_PN);
                webPN.fillInUserIdentityInfo(sidBasedToken);
            }
            catch (SourceUnavailableException sue)
            {
                // If home farm is unavailable, save a few calls by pointing the
                // user context to the first disaster recovery farm.
                if (webPNs.size() > 1)
                {
                    DisasterRecoveryUtils.setWebPNIndex(wiContext, Constants.INDEX_FIRST_RECOVERY_WEB_PN);
                }
                else
                {
                    return new StatusMessage(MessageType.ERROR, "GeneralCredentialsFailure");
                }
            }
            catch (SourceIOException sioe)
            {
                return new StatusMessage(MessageType.ERROR, "GeneralCredentialsFailure");
            }
        }

        return null;
    }

    /**
     * Gets the WebPN in use by the user.  If no WebPN has been set, then it
     * will return the primary WebPN for the user.
     * @param wiContext The WIContext object.
     * @return The current WebPN.
     */
    public static WebPN getWebPN(WIContext wiContext) {
        List webPNs = DisasterRecoveryUtils.getWebPNs(wiContext);
        int farmIndex = DisasterRecoveryUtils.getWebPNIndex(wiContext);
        return (WebPN)webPNs.get(farmIndex);
    }

    /**
     * Retrieve the list of all WebPNs configured for use (including both
     * "normal" mode farms and disaster recovery farms).
     * @param wiContext The WIContext object.
     * @return A List of WebPNs.
     */
    public static List getWebPNs(WIContext wiContext) {
        return (List)wiContext.getWebAbstraction().getApplicationAttribute(AppAttributeKey.WEBPN_LIST);
    }

    /**
     * Indicates whether disaster recovery is enabled and activated.
     * @param wiContext The WIContext object.
     * @return <code>true</code> if a recovery farm is in use, else
     * <code>false</code>.
     */
    public static boolean isDisasterRecoveryInUse(WIContext wiContext) {
        int farmIndex = DisasterRecoveryUtils.getWebPNIndex(wiContext);
        return farmIndex != Constants.INDEX_PRIMARY_WEB_PN;
    }

    /**
     * Caches the details of the WebPN currently in use by the user.
     * @param wiContext The WIContext object.
     * @param webPN The user's current WebPN.
     */
    public static void setWebPN(WIContext wiContext, WebPN webPN) {
        List webPNs = getWebPNs(wiContext);
        setWebPNIndex(wiContext, webPNs.indexOf(webPN));
    }

    private static int getWebPNIndex(WIContext wiContext) {
        Integer farmIndexFromSession = (Integer)wiContext.getWebAbstraction().getSessionAttribute(Constants.SV_WEB_PN);
        return (farmIndexFromSession == null) ? Constants.INDEX_PRIMARY_WEB_PN : farmIndexFromSession.intValue();
    }

    private static void setWebPNIndex(WIContext wiContext, int webPNIndex) {
        wiContext.getWebAbstraction().setSessionAttribute(
                                        Constants.SV_WEB_PN,
                                        (webPNIndex >= 0 ? new Integer(webPNIndex) : null));
    }
}
