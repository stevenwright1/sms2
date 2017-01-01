<%
WizardHelp help = wizardContext.getWizardHelp(PageID);
%>
<% if (help != null){ %>
        <div id="HelpDiv" class="SectionClose">
          <a id="HelpLink" href="#" onclick="toggleSection('HelpDiv', 'HelpLink');"><%=wizardContext.getString("Help")%></a>
          <div id="HelpContent">
                    <div class="PrintDiv">
                        <a id="printLink" href="javascript:window.print()" title="<%=wizardContext.getString("PrintTooltip")%>"
                           ><%=wizardContext.getString("Print")%></a>
                    </div>
                    <% Iterator sections = help.getSectionNumbers();
                       while(sections.hasNext()){
                            java.lang.Integer sectionNo = (java.lang.Integer) sections.next();
                            String title = help.getTitle(sectionNo);
                            if (title != null){
                    %>
                                <h4 class="HelpSectionTitle">
                                    <%=title%>
                                </h4>
                    <%      }
                            %><%=WizardUtil.getHelpStepsMarkup(wizardContext, help, sectionNo)%><%
                       } // end while(section.hasNext())
                    %>
              </div>
          </div>
    <% } %>
