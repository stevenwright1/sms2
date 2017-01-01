<%
// messagesStyle.inc
// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.
// Web Interface 5.4.0.0
%>
.messagesScreen {
    padding-bottom: 80px;
}

#messagesContent ul {
    margin-bottom:10px;
}

.messagesScreen h2 {
    margin: 10px 0 12px 0;
    padding: 3px 15px;
    font-size:<%=wiContext.getString("FontSizeBase")%>;
    font-weight: normal;
}

.messagesScreen h3 {
    font-size:<%=wiContext.getString("FontSizeLarge")%>;
    margin: 0 15px;
    padding: 4px 25px 4px 30px;
    background-color: #FAFAFA;
    background-repeat: no-repeat;
    background-position: 10px center;
}

.messagesScreen #errorMessages {
    background-image: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/Error16.png")%>);
    border: 1px solid #B05050;
}

.messagesScreen #warningMessages {
    background-image: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/WarningAlert.png")%>);
    border: 1px solid #F9C887;
}

.messagesScreen #infoMessages {
    background-image: url(<%=ClientInfoUtilities.getImageName(wiContext.getClientInfo(), "../media/Info16.png")%>);
    border: 1px solid #63A7D2;
}

.messageText { <% // Style for the individual message items %>
    margin: 15px;
}
