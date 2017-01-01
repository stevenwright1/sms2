<%@ Import Namespace="com.citrix.wi" %>
<%@ Import Namespace="com.citrix.wi.mvc" %>
<%@ Import Namespace="com.citrix.wi.mvc.asp" %>
<%@ Import Namespace="com.citrix.wi.pageutils" %>
<%@ Import Namespace="com.citrix.wi.util" %>
<%@ Import Namespace="com.citrix.wing" %>
<%@ Import Namespace="com.citrix.wing.util" %>
<%
// icons.aspx
// Copyright (c) 2003 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0

WebAbstraction web = AspWebAbstraction.getInstance(Context);

int cacheDuration = UIUtils.getAppIconCacheDuration(web);
if (cacheDuration > 0) {
    web.setResponseCacheDuration(cacheDuration);
        
    // Icons can be cached in shared caches
    web.setResponseCacheControl(WebAbstraction.CACHE_CONTROL_PUBLIC);
} else {
    web.setNoCaching();
}

string imageId = web.getRequestParameter("id");

try {
    string decodedId = WebUtilities.decodeId(imageId);
    string size = web.getRequestParameter("size");
    IconCache iconCache = (IconCache)web.getApplicationAttribute(AppAttributeKey.ICON_CACHE);
    Icon icon = iconCache.getIcon(decodedId);

    if (size != null) {
        size = size.ToLower();
    }

    if (icon != null && icon.getImage() != null) {
        sbyte[] sgif = ("small" == size) ? icon.getSmallImage() : icon.getImage();

        // need to convert sbyte[] to byte[]
        // this is only done once as the page should be cached
        byte[] ugif = new byte[sgif.Length];
        for(int i=0; i<sgif.Length; i++) {
            ugif[i] = (byte)sgif[i];
        }
        web.setResponseContentType(icon.getImageMimeType());
        Response.BinaryWrite(ugif);
    } else {
        Response.Write("Icon not found");
    }
} catch (ParseException) {
    Response.Write("Icon not found");
}
%>