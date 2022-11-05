
namespace YouTubeApiLib
{
    public class VideoIdPageResult
    {
        public VideoIdPage VideoPage { get; private set; }
        public int ErrorCode { get; private set; }

        public VideoIdPageResult(VideoIdPage videoPage, int errorCode)
        {
            VideoPage = videoPage;
            ErrorCode = errorCode;
        }
    }
}
