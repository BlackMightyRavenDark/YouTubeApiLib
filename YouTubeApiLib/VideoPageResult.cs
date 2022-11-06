
namespace YouTubeApiLib
{
    public class VideoPageResult
    {
        public VideoPage VideoPage { get; private set; }
        public int ErrorCode { get; private set; }

        public VideoPageResult(VideoPage videoPage, int errorCode)
        {
            VideoPage = videoPage;
            ErrorCode = errorCode;
        }
    }
}
