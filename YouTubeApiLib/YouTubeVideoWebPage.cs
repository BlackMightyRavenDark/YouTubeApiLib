
namespace YouTubeApiLib
{
    public class YouTubeVideoWebPage
    {
        public string WebPageCode { get; private set; }
        public bool IsProvidedManually { get; private set; }

        public YouTubeVideoWebPage(string webPageCode, bool isProvidedManually)
        {
            WebPageCode = webPageCode;
            IsProvidedManually = isProvidedManually;
        }

        internal static YouTubeVideoWebPage Get(string videoId)
        {
            string url = Utils.GetVideoUrl(videoId);
            int errorCode = Utils.DownloadString(url, out string pageCode);
            return errorCode == 200 ? new YouTubeVideoWebPage(pageCode, false) : null;
        }

        public static YouTubeVideoWebPage Get(VideoId videoId)
        {
            return videoId != null ? Get(videoId.Id) : null;
        }
    }
}
