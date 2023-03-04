using Newtonsoft.Json.Linq;
using static YouTubeApiLib.Utils;

namespace YouTubeApiLib
{
    public class RawVideoInfo
    {
        public JObject RawData { get; private set; }
        public VideoInfoGettingMethod DataGettingMethod { get; private set; }
        public YouTubeVideoPlayabilityStatus PlayabilityStatus => ExtractPlayabilityStatus();
        public StreamingData StreamingData => ExtractStreamingData();
        public JObject VideoDetails => ExtractVideoDetails();
        public JObject Microformat => ExtractMicroformat();

        public RawVideoInfo(JObject rawData, VideoInfoGettingMethod dataGettingMethod)
        {
            RawData = rawData;
            DataGettingMethod = dataGettingMethod;
        }

        private YouTubeVideoPlayabilityStatus ExtractPlayabilityStatus()
        {
            JObject jPlayabilityStatus = RawData?.Value<JObject>("playabilityStatus");
            return jPlayabilityStatus != null ? YouTubeVideoPlayabilityStatus.Parse(jPlayabilityStatus) : null;
        }

        private StreamingData ExtractStreamingData()
        {
            JObject jStreamingData = RawData?.Value<JObject>("streamingData");
            if (jStreamingData != null)
            {
                return new StreamingData(jStreamingData, DataGettingMethod);
            }
            return null;
        }

        private JObject ExtractVideoDetails()
        {
            return RawData?.Value<JObject>("videoDetails");
        }

        private JObject ExtractMicroformat()
        {
            return RawData?.Value<JObject>("microformat");
        }

        public override string ToString()
        {
            return RawData != null ? RawData.ToString() : "null";
        }
    }
}
