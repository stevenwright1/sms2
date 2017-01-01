using System;
using System.Timers;
using System.Threading;
using System.Drawing;

using AuthGateway.Helpers.Pexels;
using AuthGateway.Shared;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Tracking;

namespace AuthGateway.MutualAuthImages
{
    public class Server
    {
        private System.Timers.Timer requestImagesTimer;
        private const double _requestImagesTimerInterval = 24 * 60 * 60 * 1000;

        private AuthEngineProxy aeProxy;
        private SystemConfiguration sc;

        public Server()
        {            
        }

        private void timerElapsed(object sender, ElapsedEventArgs e)
        {
            try {
                Logger.Instance.WriteToLog("Timer: Request Images", LogLevel.Debug);
                var ret = aeProxy.GetImagesPollingMasterStatus();
                if (!ret.IsImagesPollingMaster) {
                    Logger.Instance.WriteToLog("Timer: not a polling master, return", LogLevel.Debug);
                }
                else {
                    string[] categories = GetCategories();
                    RequestImages(categories);
                }
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("Timer ERROR: " + ex.Message, LogLevel.Debug);
                Logger.Instance.WriteToLog("Timer TRACE: " + ex.StackTrace, LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
            }
        }

        private string[] GetCategories()
        {
            Logger.Instance.WriteToLog("GetCategories:", LogLevel.Debug);
            string[] categories = aeProxy.GetImageCategories().Categories;
            Logger.Instance.WriteToLog("GetCategories: count = " + categories.Length, LogLevel.Debug);
            return categories;
        }

        private void StoreImage(string url, string category)
        {
            try {
                byte[] bytes;
                Bitmap image = (Bitmap)ImagingHelper.GetImageFromUrl(url, out bytes);
                aeProxy.StoreImage(url, category, bytes);
            }
            catch (Exception ex){
                Logger.Instance.WriteToLog(string.Format("Failed to store image {0}: {1}", url, ex.Message), LogLevel.Debug);
                //Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
            }
            
        }
        private void RequestImages(string[] categories)
        {
            foreach (string category in categories) {
                try {
                    Logger.Instance.WriteToLog("RequestImages: start requesting category: " + category, LogLevel.Debug);
                    string imagesResponse = PexelsAPI.QueryImages(PexelsAPI.CreateQuery(category, 150));
                    PexelsParser parser = new PexelsParser(imagesResponse);
                    string[] imagesPortion = parser.GetTinyPhotos();
                    foreach (string imageUrl in imagesPortion) {
                        StoreImage(imageUrl, category);
                    }

                    while (!string.IsNullOrEmpty(parser.NextPageUrl)) {
                        imagesResponse = PexelsAPI.QueryImages(parser.NextPageUrl);
                        parser = new PexelsParser(imagesResponse);
                        imagesPortion = parser.GetTinyPhotos();
                        foreach (string imageUrl in imagesPortion) {
                            StoreImage(imageUrl, category);
                        }
                    }
                    Logger.Instance.WriteToLog("RequestImages: end requesting category: " + category, LogLevel.Debug);
                }
                catch (Exception ex) {
                    Logger.Instance.WriteToLog(string.Format("RequestImages: error requesting category {0}: {1}", category, ex.Message), LogLevel.Debug);
                    Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
                }
            }
        }
        public void StartService(object data)
        {
            try {                
                Logger.Instance.WriteToLog("Server.StartService", LogLevel.Info);                                

                var parms = (object[])data;
                sc = (SystemConfiguration)parms[0];
                
                aeProxy = new AuthEngineProxy(sc);

                new Thread(() => {
                                timerElapsed(null, null);
                            }).Start();

                if (requestImagesTimer == null) {                    
                    requestImagesTimer = new System.Timers.Timer();
                    requestImagesTimer.Elapsed += new ElapsedEventHandler(timerElapsed);
                    requestImagesTimer.Enabled = true;
                    requestImagesTimer.Interval = _requestImagesTimerInterval;
                }
                else
                    requestImagesTimer.Start();
            }
            catch (Exception ex){
                Logger.Instance.WriteToLog("Server.StartService ERROR: " + ex.Message, LogLevel.Debug);
                Logger.Instance.WriteToLog("Server.StartService TRACE: " + ex.StackTrace, LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
            }
            finally {
                StopService();
            }
        }

        public void StopService()
        {
            
        }
    }
}