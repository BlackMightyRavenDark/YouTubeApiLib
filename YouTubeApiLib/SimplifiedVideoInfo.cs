using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
    public class SimplifiedVideoInfo
    {
        public JObject Info { get; private set; }
        public bool IsVideoInfoAvailable { get; private set; }
        public bool IsMicroformatInfoAvailable { get; private set; }

        /// <summary>
        /// The streaming data can be changed while parsing raw video info.
        /// So it's needed to be stored somewhere.
        /// A NULL value means that the source data has not been modified while parsing.
        /// </summary>
        public StreamingData StreamingData { get; private set; } //TODO: This field must not to be here!

        public SimplifiedVideoInfo(JObject simplifiedVideoInfo,
            bool isVideoInfoAvailable, bool isMicroformatInfoAvailable,
            StreamingData streamingData)
        {
            Info = simplifiedVideoInfo;
            IsVideoInfoAvailable = isVideoInfoAvailable;
            IsMicroformatInfoAvailable = isMicroformatInfoAvailable;
            StreamingData = streamingData;
        }
    }
}
