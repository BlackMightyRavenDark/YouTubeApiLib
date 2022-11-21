using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
    public class StreamingData
    {
        public JObject RawData { get; private set; }

        public StreamingData(JObject rawData)
        {
            RawData = rawData;
        }

        public LinkedList<YouTubeMediaTrack> Parse()
        {
            return YouTubeMediaFormatsParser.Parse(this);
        }
    }
}
