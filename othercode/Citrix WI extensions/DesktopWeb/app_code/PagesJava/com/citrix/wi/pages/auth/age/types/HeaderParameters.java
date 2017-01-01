/**
 *
 */
package com.citrix.wi.pages.auth.age.types;

import java.io.UnsupportedEncodingException;
import java.util.HashMap;

import com.citrix.wi.pageutils.Constants;
import com.citrix.wing.util.Base64Codec;
import com.citrix.wing.util.Strings;

/**
 * Represents the set of parameters returned in an AG response
 */
public class HeaderParameters {
    private String username;
    private String password;
    private String domain;
    private String ageSessionId;
    private Integer ageProtocolRevision;

    private HeaderParameters(String username, String password, String domain, String ageSessionId, Integer ageProtocolRevision) {
       this.username = username;
       this.password = password;
       this.domain = domain;
       this.ageSessionId = ageSessionId;
       this.ageProtocolRevision = ageProtocolRevision;
    }

    public static HeaderParameters fromHashMap(HashMap params, boolean isPasswordRequiredFromAG) {
        String username = (String)params.get(Constants.AGE_USERNAME);
        String domain = (String)params.get(Constants.AGE_DOMAIN);
        String password = (isPasswordRequiredFromAG ? (String)params.get(Constants.AGE_PASSWORD) : "");
        String ageSessionId = (String)params.get(Constants.AGE_SESSION_ID);

        Integer protocolRevision = null;
        String protocolRevisionStr = (String) params.get(Constants.AGE_PROTOCOL_REVISION);
        if (protocolRevisionStr != null) {
            try {
                protocolRevision = new Integer(protocolRevisionStr);
            } catch (NumberFormatException ignore) { }
        }

        return new HeaderParameters(username, password, domain, ageSessionId, protocolRevision);
    }

    public static HeaderParameters fromAGBasicChallenge(String headerString, boolean passwordRequired) {
        String[] parameters = Strings.split(headerString, ';');
        HashMap table = new HashMap();

        for (int i = 0; i < parameters.length; i++) {
            String trimmed = parameters[i].trim();
            int pos = trimmed.indexOf('=');
            if (pos > 0) {
                table.put(trimmed.substring(0, pos), decodeHeaderValue(trimmed.substring(pos + 1)));
            }
        }

        return HeaderParameters.fromHashMap(table, passwordRequired);
    }

    /**
     * @return the username
     */
    public String getUsername() {
        return username;
    }

    /**
     * @param username the username to set
     */
    public void setUsername(String username) {
        this.username = username;
    }

    /**
     * @return the password
     */
    public String getPassword() {
        return password;
    }

    /**
     * @param password the password to set
     */
    public void setPassword(String password) {
        this.password = password;
    }

    /**
     * @return the domain
     */
    public String getDomain() {
        return domain;
    }

    /**
     * @param domain the domain to set
     */
    public void setDomain(String domain) {
        this.domain = domain;
    }

    /**
     * @return the ageSessionId
     */
    public String getAgeSessionId() {
        return ageSessionId;
    }

    /**
     * @param ageSessionId the ageSessionId to set
     */
    public void setAgeSessionId(String ageSessionId) {
        this.ageSessionId = ageSessionId;
    }

    /**
     * @return the ageProtocolRevision
     */
    public Integer getAgeProtocolRevision() {
        return ageProtocolRevision;
    }

    /**
     * @param ageProtocolRevision the ageProtocolRevision to set
     */
    public void setAgeProtocolRevision(Integer ageProtocolRevision) {
        this.ageProtocolRevision = ageProtocolRevision;
    }

    // Decode a value passed in via the CitrixAGBasic header
    private static String decodeHeaderValue(String encoded) {
        if (!encoded.startsWith("\"") || !encoded.endsWith("\"")) {
            return null;
        }

        // Strip off the double quotes and decode the base64 value
        byte[] decodedBytes = Base64Codec.decode(encoded.substring(1, encoded.length() - 1));

        String decodedString = null;
        try {
            decodedString = new String(decodedBytes, "UTF8");
        } catch (UnsupportedEncodingException uee) {
            decodedString = new String(decodedBytes);
        }

        return decodedString;
    }
}
