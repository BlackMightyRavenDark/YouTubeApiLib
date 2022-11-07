using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
    public class VideoIdPage : IVideoPageParser
    {
        public JObject RawData { get; private set; }
        public List<string> VideoIds { get; private set; }
        public string ContinuationToken { get; private set; }
        private bool _isContinuationToken;

        public VideoIdPage(JObject rawData, bool isContinuationToken)
        {
            RawData = rawData;
            _isContinuationToken = isContinuationToken;
        }

        /// <summary>
        /// Parse contained data.
        /// </summary>
        /// <returns>Video ID count</returns>
        public int Parse()
        {
            JArray jaItems = Utils.FindItemsArray(RawData, _isContinuationToken);
            if (jaItems == null)
            {
                return 0;
            }
            VideoIds = Utils.ExtractVideoIDsFromGridRendererItems(jaItems, out string token);
            ContinuationToken = token;
            return VideoIds != null ? VideoIds.Count : 0;
        }
    }
}
