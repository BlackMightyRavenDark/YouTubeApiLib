﻿using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
    public class SimplifiedVideoInfo
    {
        public JObject Info { get; }
        public bool IsVideoInfoAvailable { get; }
        public bool IsMicroformatInfoAvailable { get; }

        /// <summary>
        /// The streaming data can be changed while parsing raw video info.
        /// So it's needed to be stored somewhere.
        /// A NULL value means that the source data has not been modified while parsing.
        /// </summary>
        public StreamingData StreamingData { get; } //TODO: This field must not to be here!

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
