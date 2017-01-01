using System;
using System.Net;
using System.IO;
using System.Text;

using AuthGateway.Shared.Tracking;


namespace AuthGateway.Helpers.Pexels
{
    public class PexelsParser
    {
        private string JSONstring;    
   
        private const string PerPageAttrName = "per_page";
        private const string TotalResultsAttrName = "total_results";
        private const string PhotosAttrName = "photos";
        private const string SrcAttrName = "src";
        private const string TinyAttrName = "tiny";
        private const string NextPageAttrName = "next_page";
        
        public PexelsParser(string pexelsJSON)
        {
            JSONstring = pexelsJSON;
        }

        private string GetAttrString(string attrName)
        {
            return string.Format("\"{0}\":", attrName);
        }

        public int PerPage
        {
            get
            {
                try {                
                    string cut = JSONstring.Substring(JSONstring.IndexOf(GetAttrString(PerPageAttrName)) + GetAttrString(PerPageAttrName).Length);
                
                    string num = cut.Substring(0, cut.IndexOf(','));

                    return int.Parse(num);
                }
                catch (Exception ex) {
                    Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
                    throw new Exception("Wrong Pexels response format.");
                }

            }
        }

        public int TotalResults
        {
            get
            {
                try {
                    string cut = JSONstring.Substring(JSONstring.IndexOf(GetAttrString(TotalResultsAttrName)) + GetAttrString(TotalResultsAttrName).Length);

                    string num = cut.Substring(0, cut.IndexOf(','));

                    return int.Parse(num);
                }
                catch (Exception ex){
                    Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
                    throw new Exception("Wrong Pexels response format.");
                }

            }
        }

        public string NextPageUrl
        {
            get
            {
                try {
                    string nextPage = "";
                    if (JSONstring.IndexOf(GetAttrString(NextPageAttrName)) >= 0) {
                        string cut = JSONstring.Substring(JSONstring.IndexOf(GetAttrString(NextPageAttrName)) + GetAttrString(NextPageAttrName).Length);

                        nextPage = cut.Substring(0, cut.IndexOf(',')).Split(new char[] { '"' }, StringSplitOptions.RemoveEmptyEntries)[0];
                    }
                    return nextPage;
                }
                catch (Exception ex){
                    Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
                    throw new Exception("Wrong Pexels response format.");
                }
            }
        }

        private bool IsLastPage
        {
            get
            {                
                return string.IsNullOrEmpty(NextPageUrl);
            }
        }
        public string[] GetTinyPhotos()
        {                        
            try {
                int resultsCount = IsLastPage? TotalResults % PerPage : PerPage;
                string[] tinyPhotos = new string[resultsCount];

                string cut = JSONstring;
                for (int i = 0; i < resultsCount; i++) {
                    int index = cut.IndexOf(GetAttrString(TinyAttrName)) + GetAttrString(TinyAttrName).Length;
                    cut = cut.Substring(index);
                    tinyPhotos[i] = cut.Substring(0, cut.IndexOf('}')).Split(new char[] { '"' }, StringSplitOptions.RemoveEmptyEntries)[0];
                }

                return tinyPhotos;
            }
            catch (Exception ex) {
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
                throw new Exception("Wrong Pexels response format." + " Total: " + TotalResults + "next: " + NextPageUrl);
            }
        }

        //public static int IdFromUrl(string url)
        //{
        //    int id;
        //    string cut = url.Substring("https://static.pexels.com/photos/".Length);
        //    string number = cut.Substring(0, cut.IndexOf('/'));
        //    id = int.Parse(number);
        //    return id;
        //}
    }
    public class PexelsAPI
    {
        private const string AuthKey = "563492ad6f917000010000010d7738f7efa140d74d80aaeac2623b40";

        private const string Path = @"http://api.pexels.com/v1/search";

        public static string CreateQuery(string category, int perPage = 15)
        {
            return string.Format(@"{0}?query={1}&per_page={2}", Path, category, perPage);
        }
        public static string QueryImages(string queryString)
        {            
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(queryString);
            req.Headers.Add("Authorization", AuthKey);

            StreamReader str = new StreamReader(req.GetResponse().GetResponseStream());
            string response = str.ReadToEnd();

            return response;
        }
    }
}