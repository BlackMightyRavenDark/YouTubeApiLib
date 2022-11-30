using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using static YouTubeApiLib.Utils;

namespace YouTubeApiLib
{
    public class StreamingData
    {
        public JObject RawData { get; private set; }
        public VideoInfoGettingMethod DataGettingMethod { get; private set; }

        public StreamingData(JObject rawData, VideoInfoGettingMethod dataGettingMethod)
        {
            RawData = rawData;
            DataGettingMethod = dataGettingMethod;
        }

        public LinkedList<YouTubeMediaTrack> Parse()
        {
            return YouTubeMediaFormatsParser.Parse(this);
        }
    }
}
