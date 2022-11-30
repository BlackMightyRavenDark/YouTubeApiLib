using Newtonsoft.Json.Linq;
using static YouTubeApiLib.Utils;

namespace YouTubeApiLib
{
    public class RawVideoInfo
    {
        public JObject RawData { get; private set; }
        public VideoInfoGettingMethod DataGettingMethod { get; private set; }

        public RawVideoInfo(JObject rawData, VideoInfoGettingMethod dataGettingMethod)
        {
            RawData = rawData;
            DataGettingMethod = dataGettingMethod;
        }

        public override string ToString()
        {
            return RawData != null ? RawData.ToString() : "null";
        }
    }
}
