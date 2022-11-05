
namespace YouTubeApiLib
{
    public class SimplifiedVideoInfoResult
    {
        public SimplifiedVideoInfo SimplifiedVideoInfo { get; private set; }
        public int ErrorCode { get; private set; }

        public SimplifiedVideoInfoResult(SimplifiedVideoInfo simplifiedVideoInfo, int errorCode)
        {
            SimplifiedVideoInfo = simplifiedVideoInfo;
            ErrorCode = errorCode;
        }
    }
}
