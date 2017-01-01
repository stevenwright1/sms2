<%
// tabPaneFoot.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<%// The two closing div's are from tabPaneHead.inc %>

  </div> <% // closes the selectedTabContent div %>

<% if(viewControl.showHint) { %>
  <div id="hintAreaLeft">
    <div id="hintAreaRight">
      <div id="hintArea">
        <table cellspacing="0" cellpadding="0">
          <tr>
          <td>
          <p>
         <%=viewControl.hintText%>
           </p>
          </td>
          <% if (viewControl.showCloseHint) { %>
          <td>
          <p>
           <a title='<%=wiContext.getString("CloseHintsArea")%>'  onmouseover="changeCloseImage(this, true);" onmouseout="changeCloseImage(this, false);"
              id="closeHintsLink" href="<%=Constants.PAGE_APPLIST%>?<%=Constants.QSTR_CLOSE_HINTS_AREA%>=<%=Constants.CONST_CLOSE_HINTS_AREA%>"
              ><img alt='<%=wiContext.getString("CloseHintsArea")%>' src="../media/CloseHint.gif"></a>
          </p>
          </td>
          <% } %>
          </tr>
        </table>
      </div>
    </div>
  </div>
<% } %>
</div> <% // closes the mainContainer div %>