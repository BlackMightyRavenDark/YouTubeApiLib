
namespace YouTubeApiLib
{
    public class YouTubeVideoWebPage
    {
        public string WebPageCode { get; private set; }
        public bool IsProvidedManually { get; private set; }

        private YouTubeVideoWebPage(string webPageCode, bool isProvidedManually)
        {
            WebPageCode = webPageCode;
            IsProvidedManually = isProvidedManually;
        }

        internal static YouTubeVideoWebPageResult Get(string videoId)
        {
            string url = Utils.GetVideoUrl(videoId);
            int errorCode = Utils.DownloadString(url, out string responseWebPageCode);
            YouTubeVideoWebPage webPage = errorCode == 200 ? new YouTubeVideoWebPage(responseWebPageCode, false) : null;
            return new YouTubeVideoWebPageResult(webPage, errorCode);
        }

        public static YouTubeVideoWebPageResult Get(VideoId videoId)
        {
            return videoId != null ? Get(videoId.Id) : new YouTubeVideoWebPageResult(null, 400);
        }

        public static YouTubeVideoWebPageResult FromCode(string webPageCode)
        {
            if (!string.IsNullOrEmpty(webPageCode) && !string.IsNullOrWhiteSpace(webPageCode))
            {
                return new YouTubeVideoWebPageResult(new YouTubeVideoWebPage(webPageCode, true), 200);
            }
            return new YouTubeVideoWebPageResult(null, 404);
        }
    }
}
