using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
	public class YouTubeVideoIdPage : IVideoPageParser
	{
		public string RawData { get; }
		public List<string> VideoIds { get; private set; }
		public string ContinuationToken { get; private set; }
		private bool _isContinuationToken;

		public YouTubeVideoIdPage(string rawData, bool isContinuationToken)
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
			JObject json = Utils.TryParseJson(RawData);
			if (json == null) { return 0; }
			JArray jaItems = Utils.FindItemsArray(json, _isContinuationToken);
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
