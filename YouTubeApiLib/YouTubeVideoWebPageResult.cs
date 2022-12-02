
namespace YouTubeApiLib
{
    public class YouTubeVideoWebPageResult
    {
        public YouTubeVideoWebPage VideoWebPage { get; private set; }
        public int ErrorCode { get; private set; }

        public YouTubeVideoWebPageResult(YouTubeVideoWebPage videoWebPage, int errorCode)
        {
            VideoWebPage = videoWebPage;
            ErrorCode = errorCode;
        }
    }
}
