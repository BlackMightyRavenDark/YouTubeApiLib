
namespace YouTubeApiLib
{
    public class RawVideoInfoResult
    {
        public RawVideoInfo RawVideoInfo { get; private set; }
        public int ErrorCode;

        public RawVideoInfoResult(RawVideoInfo rawVideoInfo, int errorCode)
        {
            RawVideoInfo = rawVideoInfo;
            ErrorCode = errorCode;
        }
    }
}
