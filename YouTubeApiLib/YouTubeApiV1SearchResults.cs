using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
    public class YouTubeApiV1SearchResults
    {
        public JObject RawData { get; private set; }
        public YouTubeApiV1SearchResultFilter UsedFilter { get; private set; }
        public bool IsContinuationItem { get; private set; }
        public int ErrorCode { get; private set; }

        public YouTubeApiV1SearchResults(
            JObject rawData,
            YouTubeApiV1SearchResultFilter usedFilter,
            bool isContinuationItem,
            int errorCode)
        {
            RawData = rawData;
            UsedFilter = usedFilter;
            IsContinuationItem = isContinuationItem;
            ErrorCode = errorCode;
        }
    }
}
