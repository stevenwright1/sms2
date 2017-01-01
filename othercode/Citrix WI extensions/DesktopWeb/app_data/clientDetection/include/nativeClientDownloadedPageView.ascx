<!--#include file="~/app_data/clientDetection/include/includeBottom.aspxf" -->
<%
NativeDownloadedViewModel viewModel = (NativeDownloadedViewModel)wizardContext.getWebAbstraction().getRequestContextAttribute(WizardConstants.VIEW_MODEL);
if (viewModel.IEPollingMode) {
%>
<!--#include file="~/app_data/clientDetection/include/nativeClientDownloadedIE.inc" -->
<%
} else {
%>
<!--#include file="~/app_data/clientDetection/include/nativeClientDownloaded.inc" -->
<%
}
%>
