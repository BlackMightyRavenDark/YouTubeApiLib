
namespace YouTubeApiLib
{
    public class YouTubeVideoWebPageResult
    {
        public YouTubeVideoWebPage VideoWebPage { get; }
        public int ErrorCode { get; }

        public YouTubeVideoWebPageResult(YouTubeVideoWebPage videoWebPage, int errorCode)
        {
            VideoWebPage = videoWebPage;
            ErrorCode = errorCode;
        }
    }
}
