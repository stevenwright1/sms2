  <script type="text/javascript" src="<%=UIUtils.getJavascriptURL(wiContext)%>"></script>
  <script type="text/javascript">
    <!--
    // Ensure every page has an onLoadLayout function. If a page defines its own
    // onLoadLayout function, it will override this one.
    function onLoadLayout() {
      return;
    }
    function getFrameSuffix() {
      return "<%=Include.getFrameSuffix(wiContext.getUserEnvironmentAdaptor())%>";
    }
    function getSessionToken() {
      return "<%=SessionToken.get(wiContext)%>";
    }
    // -->
  </script>