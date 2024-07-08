using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
	public class YouTubeChannelTabPageContentTabs
	{
		public string RawData { get; }

		public YouTubeChannelTabPageContentTabs(string rawData)
		{
			RawData = rawData;
		}

		public YouTubeChannelTab GetSelectedTab()
		{
			JArray jaTabs = Utils.TryParseJsonArray(RawData);
			return jaTabs != null ? Utils.FindSelectedChannelTab(jaTabs) : null;
		}
	}
}
