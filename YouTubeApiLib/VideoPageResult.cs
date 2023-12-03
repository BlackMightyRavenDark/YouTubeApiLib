
namespace YouTubeApiLib
{
    public class VideoPageResult
    {
        public VideoPage VideoPage { get; }
        public int ErrorCode { get; }

        public VideoPageResult(VideoPage videoPage, int errorCode)
        {
            VideoPage = videoPage;
            ErrorCode = errorCode;
        }
    }
}
