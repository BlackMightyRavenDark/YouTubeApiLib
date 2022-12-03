using Newtonsoft.Json.Linq;
using static YouTubeApiLib.Utils;

namespace YouTubeApiLib
{
    public class RawVideoInfo
    {
        public JObject RawData { get; private set; }
        public VideoInfoGettingMethod DataGettingMethod { get; private set; }
        public StreamingData StreamingData => GetStreamingData();

        public RawVideoInfo(JObject rawData, VideoInfoGettingMethod dataGettingMethod)
        {
            RawData = rawData;
            DataGettingMethod = dataGettingMethod;
        }

        private StreamingData GetStreamingData()
        {
            JObject jStreamingData = RawData?.Value<JObject>("streamingData");
            if (jStreamingData != null)
            {
                return new StreamingData(RawData, DataGettingMethod);
            }
            return null;
        }

        public override string ToString()
        {
            return RawData != null ? RawData.ToString() : "null";
        }
    }
}
