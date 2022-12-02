
namespace YouTubeApiLib
{
    public class YouTubeVideoWebPage
    {
        public string WebPageCode { get; private set; }

        public YouTubeVideoWebPage(string webPageCode)
        {
            WebPageCode = webPageCode;
        }

        internal static YouTubeVideoWebPage Get(string videoId)
        {
            string url = Utils.GetVideoUrl(videoId);
            int errorCode = Utils.DownloadString(url, out string pageCode);
            return errorCode == 200 ? new YouTubeVideoWebPage(pageCode) : null;
        }

        public static YouTubeVideoWebPage Get(VideoId videoId)
        {
            return videoId != null ? Get(videoId.Id) : null;
        }
    }
}
