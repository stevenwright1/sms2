<%
// messageScreen.inc
// Copyright (c) 2007 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
<% if( viewControl.getNumMessages() == 0 ) { %>
<div class="commonInnerBox messagesScreen">
  <p class="secondParagraph"><%=wiContext.getString("MessagesNone")%></p>
</div>

<% } else { %>

<div class="messagesScreen">

<h2 id="messagesScreenHeader"><%=wiContext.getString("MessageHeader", viewControl.getNumMessagesAsString())%></h2>
  <div id="messagesContent">

  <%
    // ERROR messages
    int numErrorMessages = viewControl.getNumMessages(MessageType.ERROR);
    if(numErrorMessages > 0) {
      MessageScreenMessage[] errorMessages = viewControl.getMessages(MessageType.ERROR);
  %>
  <ul>
    <li class="headerMessage"><h3 id="errorMessages"><%=wiContext.getString("MessageHeaderError")%></h3></li>

      <% for(int ix = 0; ix < numErrorMessages; ++ix) { %>
<% if (ix < numErrorMessages - 1){%>
        <li class="messageText messageRowSeparator"
<% } else {%>
        <li class="messageText"
<% }%>
        ><%=errorMessages[ix].getBody()%></li>
      <%
        }
      %>
    </ul>
  <% } %>

  <%
    // WARNING messages
    int numWarningMessages = viewControl.getNumMessages(MessageType.WARNING);
    if(numWarningMessages > 0) {
      MessageScreenMessage[] warningMessages = viewControl.getMessages(MessageType.WARNING);
  %>
  <ul>
    <li class="headerMessage"><h3 id="warningMessages"><%=wiContext.getString("MessageHeaderWarning")%></h3></li>

      <% for(int ix = 0; ix < numWarningMessages; ++ix) { %>
<% if (ix < numWarningMessages - 1){%>
        <li class="messageText messageRowSeparator"
<% } else {%>
        <li class="messageText"
<% }%>
        ><%=warningMessages[ix].getBody()%></li>
      <%
        }
      %>
    </ul>
  <% } %>

  <%
    // INFORMATION messages
    int numInfoMessages = viewControl.getNumMessages(MessageType.INFORMATION);
    if(numInfoMessages > 0) {
      MessageScreenMessage[] infoMessages = viewControl.getMessages(MessageType.INFORMATION);
  %>
  <ul>
    <li class="headerMessage"><h3 id="infoMessages"><%=wiContext.getString("MessageHeaderInformation")%></h3></li>

      <% for(int ix = 0; ix < numInfoMessages; ++ix) { %>
<% if (ix < numInfoMessages - 1){%>
        <li class="messageText messageRowSeparator"
<% } else {%>
        <li class="messageText"
<% }%>
        ><%=infoMessages[ix].getBody()%></li>
      <%
        }
      %>
    </ul>
  <% } %>

  </div>
</div>

<% } %>
