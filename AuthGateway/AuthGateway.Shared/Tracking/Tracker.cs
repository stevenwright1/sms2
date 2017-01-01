using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using AuthGateway.Shared.Log;
using Trackerbird.Tracker;

namespace AuthGateway.Shared.Tracking
{
    public class Tracker
    {
        private static Tracker instance;

        private const string TrackerbirdCallhomeUrl = @"http://14041.tbnet1.com";

        private const string TrackerbirdProductId = "2385108563";

        private const string Version = "16101701";

        private const string Build = "beta";

        private bool trackingAllowed = false;

        //private EventCollector eventCollector = new EventCollector();

        private Tracker()
        {            
        }

        public static Tracker Instance
        {
            get
            {
                if (instance == null) {
                    instance = new Tracker();
                }
                return instance;
            }
        }

        public string DefaultEventCategory
        {
            get;
            private set;
        }

        public void StartTracking(string defaultEventCategory = null)
        {
            try {
                trackingAllowed = true;

                Logger.Instance.WriteToLog("Tracker.StartTracking", LogLevel.Debug);

                DefaultEventCategory = defaultEventCategory;

                Assembly assembly = Assembly.GetExecutingAssembly();
                string path = Directory.GetParent(Directory.GetParent(assembly.Location).FullName).FullName.TrimEnd(Path.DirectorySeparatorChar);
                string configPath = path + @"\Service\";

                TBConfig.SetFilePath(configPath);

                TBConfig.CreateConfig(TrackerbirdCallhomeUrl, TrackerbirdProductId, Version, Build, false);

                TBApp.Start();
                //TBApp.StartAutoSync(false);
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("Tracker.StartTracking ERROR: " + ex.Message, LogLevel.Debug);
            }
        }

        public void StopTracking()
        {
            if (!trackingAllowed)
                return;

            try {
                Logger.Instance.WriteToLog("Tracker.StopTracking", LogLevel.Debug);
                //TBApp.StopAutoSync();
                TBApp.Stop();
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("Tracker.StopTracking ERROR: " + ex.Message, LogLevel.Debug);
            }
        }

        public void ForceSync()
        {
            if (!trackingAllowed)
                return;

            try {
                TBApp.Sync(false); // NOTE: Trackerbird can ignore Sync if it is abused 
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("Tracker.ForceSync ERROR: " + ex.Message, LogLevel.Debug);
            }
        }

        public void TrackException(string className, string methodName, Exception exception)
        {
            if (!trackingAllowed)
                return;

            try {
                TBApp.ExceptionTrack(className, methodName, exception);
                ForceSync();
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("Tracker.TrackException ERROR: " + ex.Message, LogLevel.Debug);
            }
        }

        public void TrackEvent(string eventName, string categoryName = null)
        {
            if (!trackingAllowed)
                return;

            try {
                TBApp.EventTrack(categoryName, eventName, null);
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("Tracker.TrackEvent ERROR: " + ex.Message, LogLevel.Debug);
            }
        }

        public void TrackNumberEvent(string eventName, string categoryName, double value)
        {
            if (!trackingAllowed)
                return;

            try {
                TBApp.EventTrackNum(categoryName, eventName, value, null);
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("Tracker.TrackNumberEvent ERROR: " + ex.Message, LogLevel.Debug);
            }
        }

        public void TrackCustomEvent(string eventName, string categoryName, string customString)
        {
            if (!trackingAllowed)
                return;

            try {
                TBApp.EventTrackTxt(categoryName, eventName, customString, null);
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("Tracker.TrackCustomEvent ERROR: " + ex.Message, LogLevel.Debug);
            }
        }

       /* public void CollectEvent(string eventName, string categoryName = null)
        {
            if (!trackingAllowed)
                return;

            try {
                eventCollector.Collect(categoryName, eventName);
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("Tracker.CollectEvent ERROR: " + ex.Message, LogLevel.Debug);
                Logger.Instance.WriteToLog("Tracker.CollectEvent STACK: " + ex.StackTrace, LogLevel.Debug);
            }
        }

        public void TrackCollectedEvents(string category)
        {
            if (!trackingAllowed)
                return;

            try {
                List<EventCollector.EventCount> events = eventCollector.GetEvents(category);

                foreach (var e in events) {
                    TrackNumberEvent(e.EventName, category, e.Count);
                }
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("Tracker.TrackCollectedEvents ERROR: " + ex.Message, LogLevel.Debug);
                Logger.Instance.WriteToLog("Tracker.TrackCollectedEvents STACK: " + ex.StackTrace, LogLevel.Debug);
            }
        }

        public void TrackCollectedEvents()
        {
            try {
                foreach (string category in eventCollector.Categories) {
                    TrackCollectedEvents(category);
                }
            }
            catch (Exception ex) {
                Logger.Instance.WriteToLog("Tracker.TrackCollectedEvents ERROR: " + ex.Message, LogLevel.Debug);
                Logger.Instance.WriteToLog("Tracker.TrackCollectedEvents STACK: " + ex.StackTrace, LogLevel.Debug);
            }
        }

        private class EventCollector
        {
            public class EventCount
            {
                public EventCount(string eventName)
                {
                    EventName = eventName;
                    Count = 1;
                }

                public string EventName { get; set; }
                public int Count { get; private set; }

                public void Increment()
                {
                    Count++;
                }
            }

            private Dictionary<string, List<EventCount>> collectedEvents = new Dictionary<string, List<EventCount>>();

            public string[] Categories
            {
                get
                {
                    string[] categories = new string[collectedEvents.Keys.Count];
                    collectedEvents.Keys.CopyTo(categories, 0);
                    return categories;
                }
            }

            public List<EventCount> GetEvents(string category)
            {
                if (!collectedEvents.ContainsKey(category))
                    return null;

                return collectedEvents[category];
            }

            public void Collect(string eventCategory, string eventName)
            {
                Logger.Instance.WriteToLog("Collect event " + eventCategory + " " + eventName, LogLevel.Debug);         

                if (!collectedEvents.ContainsKey(eventCategory)) {
                    collectedEvents.Add(eventCategory, new List<EventCount>());
                    Logger.Instance.WriteToLog("Added category" + eventCategory, LogLevel.Debug);         
                }

                int index = collectedEvents[eventCategory].FindIndex(ec => ec.EventName == eventName);

                if (index < 0) {
                    EventCount eventCount = new EventCount(eventName);
                    collectedEvents[eventCategory].Add(eventCount);
                    Logger.Instance.WriteToLog("New event " + eventName + " count: " + collectedEvents[eventCategory].FindLast(e => true).Count, LogLevel.Debug);         
                }
                else {
                    collectedEvents[eventCategory][index].Increment();
                    Logger.Instance.WriteToLog("Repeated event " + eventName + " count: " + collectedEvents[eventCategory][index].Count, LogLevel.Debug);         
                }                
            }
        }*/
    }
}
