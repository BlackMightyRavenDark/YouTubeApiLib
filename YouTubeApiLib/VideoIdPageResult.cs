
namespace YouTubeApiLib
{
    public class VideoIdPageResult
    {
        public VideoIdPage VideoIdPage { get; private set; }
        public int ErrorCode { get; private set; }

        public VideoIdPageResult(VideoIdPage videoIdPage, int errorCode)
        {
            VideoIdPage = videoIdPage;
            ErrorCode = errorCode;
        }
    }
}
