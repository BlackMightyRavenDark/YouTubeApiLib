using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
    public class VideoListResult
    {
        public JArray List { get; private set; }
        public int ErrorCode { get; private set; }

        public VideoListResult(JArray list, int errorCode)
        {
            List = list;
            ErrorCode = errorCode;
        }
    }
}
