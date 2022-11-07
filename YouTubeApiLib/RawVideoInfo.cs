using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
    public class RawVideoInfo
    {
        public JObject RawData { get; private set; }

        public RawVideoInfo(JObject rawData)
        {
            RawData = rawData;
        }

        public override string ToString()
        {
            return RawData != null ? RawData.ToString() : "null";
        }
    }
}
