<%
// styleVarsWizard.inc
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<%
    String fontFace = wiContext.getString("FontFace");
    String fontSizeXSmall = wiContext.getString("FontSizeXSmall");
    String fontSizeSmall = wiContext.getString("FontSizeSmall");
    String fontSizeBase = wiContext.getString("FontSizeBase");
    String fontSizeLarge = wiContext.getString("FontSizeLarge");
    String fontSizeXLarge = "130%";
    String brandingColor = Branding.getBrandingColor(wiContext.getConfiguration());
    String buttonWidth_Apply_Cancel_Ok = wiContext.getString("ButtonWidth_Apply_Cancel_OK");
%>
