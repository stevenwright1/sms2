// Copyright (c) 2000 - 2010 Citrix Systems, Inc. All Rights Reserved.

using System;
using System.Web;

using com.citrix.wi.metrics;
using com.citrix.wi.mvc;
using com.citrix.wing;

using com.citrix.wi.config;
using com.citrix.wi.types;

using System.ServiceModel;
using System.ServiceModel.Security;
using System.Runtime.Serialization;
using System.Net.Security;
using System.Text;
using Citrix.DeliveryServices.ICASign.ClientProxy;
using Citrix.DeliveryServices.ICASign.Contract;

using System.Xml;



namespace com.citrix.wi.pages.site {

    public class LaunchAsp : LaunchShared {

        private bool IFSEnabled = false;
        private bool LegacyClientSupportEnabled = false;
        private string icaFileThumbPrint = null;
        private IcaFileSigningHashAlgorithm ifsHashAlgorithm = null;

        public LaunchAsp(WIContext wiContext, PerformanceMetrics metrics) : base(wiContext, metrics)
        {
            WIConfiguration config = wiContext.getConfiguration();
            IFSEnabled = config.getIcaFileSigningEnabled();
            icaFileThumbPrint = config.getIcaFileSigningThumbPrint();
            ifsHashAlgorithm = config.getIcaFileSigningHashAlgorithm();
            LegacyClientSupportEnabled = config.getClientDeploymentConfiguration().getLegacyClientSupportEnabled();
        }

        public override string getRequestPageUrl()
        {
            return getRequest().ServerVariables["SCRIPT_NAME"];
        }

        public override void writeICAFileContents(ICAFile icaFile)
        {
            HttpResponse response = getResponse();
            System.Text.Encoding responseEncoding = System.Text.Encoding.UTF8;
            icaFile.setICAEncoding( "UTF8" );
            string icaFileContent = icaFile.ToString();

            if (LegacyClientSupportEnabled)
            {
                if (wiContext.getConfiguration().getDefaultLocale().getLanguage() == "ja")
                {
                    icaFile.setICAEncoding("SJIS");
                    responseEncoding = System.Text.Encoding.GetEncoding("sjis");
                }
                else
                {
                    icaFile.setICAEncoding("ISO8859_1");
                    responseEncoding = System.Text.Encoding.GetEncoding("iso-8859-1");
                }
            }

            if (IFSEnabled)
            {
               
                SigningServiceProxy proxy = new SigningServiceProxy();
                bool Success = false;

                //Need to increase maxStringContentLength for lager key size certificate which 
                //can make the returned signed ICA file bigger than default limit size 8192
                NetNamedPipeBinding binding = (NetNamedPipeBinding)proxy.Endpoint.Binding;
                XmlDictionaryReaderQuotas quotas = binding.ReaderQuotas;
                quotas.MaxStringContentLength = 65535;
                
                try
                {
                    proxy.Open();
                    icaFileContent = proxy.SignICAFile(icaFile.ToString(), icaFileThumbPrint, ifsHashAlgorithm.ToString()).ToString();
                    proxy.Close();
                    Success = true;
                }
                catch (SecurityAccessDeniedException ex)
                {
                    logSecurityAccessDeniedMessage(ex.Message);
                }
                catch (FaultException<InvalidParameterFault> ipf)
                {
                    logIcaFileSigningFailedMessage(ipf.Detail.Message);
                }
                catch (FaultException<CryptographicFault> cf)
                {
                    logIcaFileSigningFailedMessage(cf.Detail.Message);
                }
                catch (Exception e)
                {
                    logIcaFileSigningFailedMessage(e.Message);
                }
                finally
                {
                    if (!Success)
                        proxy.Abort();
                }

            }
            
            // From ASP.NET we can suppress the charset element in the Content-Type header
            // We should continue to do this to support legacy clients even though it
            // is different behaviour to the JSPs
            response.Charset = null;
            response.ContentType = "application/x-ica";
            // Large negative value for expires to ensure timeout in the past even if
            // local client and server have their clocks skewed.
            response.Expires = -10000;

            writeIcaFileContent(icaFileContent, responseEncoding);  
        }

        private void logIcaFileSigningFailedMessage(string exceptionMessage)
        {
            wiContext.log(MessageType.WARNING, "ICAFileSigningFailed",
                    new object[] { exceptionMessage } );
        }

        private void logSecurityAccessDeniedMessage(string exceptionMessage)
        {
            wiContext.log(MessageType.WARNING, "ICAFileSigningSecurityAccessDenied",
                    new object[] { exceptionMessage });
        }

        public override void writeRadFileContents(RADFile radFile, ICAFile icaFile) {
            HttpResponse response = getResponse();
            response.Charset = null;
            response.ContentType = "application/x-ctxrade";
            response.Expires = -10000;
            if (icaFile != null){
                icaFile.setICAEncoding("UTF8");
            }
            writeIcaFileContent(radFile.toString( icaFile ), System.Text.Encoding.UTF8);
        }

        public override string getAutoRadeUrl() {
            return getApplicationURL() + "/rade.aspx";
        }

        /**
         * Gets the URL for the web app servicing the current Request.
         *
         * If a "Host" header is present, this method uses it to construct
         * the web app URL, as it is more likely to be correct in the
         * presence of proxies.
         */
        private string getApplicationURL() {

            // Try and get the host and port from the request header
            string hostAndPort = getHostAndPort(getRequest());

            // Construct the URL for this webapp
            String urlBase = "";
            if (hostAndPort != null && hostAndPort.Length != 0) {
                urlBase = "" + getRequest().Url.Scheme + "://" + hostAndPort;
            } else {
                // rely on the Request.Url property (which may not be correct for proxied requests)
                urlBase = getRequest().Url.GetLeftPart(UriPartial.Authority);
            }

            return urlBase + getRequest().ApplicationPath;
        }

        private string getHostAndPort(HttpRequest request) {
            // Try and get the host and port from the request header
            string hostAndPort = request.Headers["Host"];
            // Try again with a lower case "h"
            if (hostAndPort == null || hostAndPort.Length == 0) {
                hostAndPort = request.Headers["host"];
            }
            return hostAndPort;
        }

        private void writeIcaFileContent(String content, System.Text.Encoding responseEncoding) {
            getResponse().BinaryWrite(responseEncoding.GetBytes(content));
        }

        private HttpContext getContext()
        {
            return ((com.citrix.wi.mvc.asp.AspWebAbstraction)wiContext.getWebAbstraction()).Context;
        }

        private HttpRequest getRequest()
        {
            return getContext().Request;
        }

        private HttpResponse getResponse()
        {
            return getContext().Response;
        }

    }
}
