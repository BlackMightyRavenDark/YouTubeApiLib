
namespace YouTubeApiLib
{
	public class YouTubeApiV1SearchResults
	{
		public string RawData { get; }
		public YouTubeApiV1SearchResultFilter UsedFilter { get; }
		public bool IsContinuationItem { get; }
		public int ErrorCode { get; }

		public YouTubeApiV1SearchResults(
			string rawData,
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
