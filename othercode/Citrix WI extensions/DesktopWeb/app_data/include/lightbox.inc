<%
// lightbox.inc
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<div id="lightbox">
  <table class="lightBox" cellpadding="0" cellspacing="0" align="center">
    <tr>
      <td class="lightBoxTop lightBoxLeft lightBoxTopLeft"></td>
      <td class="lightBoxTop lightBoxTopMid">
        <p id="lightboxTitle"><%= wiContext.getString("MessageHeaderWarning")%></p>
        <a href="javascript:hideLightbox(false);"
           tabIndex="3"><img id="closeButton" src="<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/LightboxCloseButton.png")%>" alt="" /></a>
      </td>
      <td class="lightBoxTop lightBoxRight lightBoxTopRight"></td>
    </tr>
    <tr>
      <td class="lightBoxLeft lightBoxMidLeft"></td>
      <td class="lightBoxMid">
        <img id="logo" width="48" height="48" src="<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/AlertIconLarge.png")%>" alt="">
        <div id="lightboxText">
          <div id="lightboxHeading"><%= wiContext.getString("RestartLightboxTitle") %></div>
          <p id="lightboxMessage"><%= wiContext.getString("RestartDesktopConfirmationText") %></p>
        </div>
      </td>
      <td class="lightBoxRight lightBoxMidRight"></td>
    </tr>
    <tr>
      <td class="lightBoxFooter lightBoxLeft lightBoxFooterLeft"></td>
      <td class="lightBoxFooter lightBoxFooterMid">
        <div id="cancelButtonPane" class="lightboxButtonPane">
          <div class="customButton">
            <a href="javascript:hideLightbox(false);"
               class="leftDoor"
               tabIndex="2"><span class="rightDoor"><%=wiContext.getString("Cancel")%></span></a>
          </div> 
        </div>
        <div id="okButtonPane" class="lightboxButtonPane">
          <div class="customButton">
            <a href="javascript:hideLightbox(true);"
               class="leftDoor"
               tabIndex="1"
               ><span class="rightDoor"><%=wiContext.getString("Restart")%></span></a>
           </div>
        </div></td>
      <td class="lightBoxFooter lightBoxRight lightBoxFooterRight"></td>
    </tr>
  </table>
</div>

<div id="lightboxMask"></div>
