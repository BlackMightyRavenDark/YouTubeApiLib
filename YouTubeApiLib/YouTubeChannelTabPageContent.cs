using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
	public class YouTubeChannelTabPageContent
	{
		public YouTubeChannelTabPage TabPage { get; }
		public string RawData { get; }

		public YouTubeChannelTabPageContent(YouTubeChannelTabPage tabPage, string rawData)
		{
			TabPage = tabPage;
			RawData = rawData;
		}

		public static YouTubeChannelTabPageContentResult Get(
			string channelId, YouTubeChannelTabPage tabPage, string pageToken = null)
		{
			return YouTubeApiV1.GetChannelTabContentRawData(channelId, tabPage, pageToken);
		}

		public static YouTubeChannelTabPageContentResult Get(string pageToken)
		{
			return Get(null, null, pageToken);
		}

		public YouTubeChannelTabPageContentTabs GetTabs()
		{
			if (string.IsNullOrEmpty(RawData) || string.IsNullOrWhiteSpace(RawData))
			{
				return null;
			}

			JObject json = Utils.TryParseJson(RawData);
			JArray jaTabs = json?.Value<JObject>("contents")?.Value<JObject>("twoColumnBrowseResultsRenderer")?.Value<JArray>("tabs");
			return jaTabs != null ? new YouTubeChannelTabPageContentTabs(jaTabs.ToString()) : null;
		}

		private JArray GetContinuationItems()
		{
			try
			{
				JObject json = JObject.Parse(RawData);
				return json.Value<JArray>("onResponseReceivedActions")[0]
					.Value<JObject>("appendContinuationItemsAction")
					.Value<JArray>("continuationItems");
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
				return null;
			}
		}

		public YouTubeVideosTabPage ParseAsVideosOrShortsOrLiveTabPage()
		{
			try
			{
				YouTubeChannelTabPageContentTabs tabs = GetTabs();
				if (tabs != null)
				{
					if (!string.IsNullOrEmpty(tabs.RawData))
					{
						YouTubeChannelTab channelTab = tabs.GetSelectedTab();
						if (channelTab != null && (channelTab.IsChannelTabPage(YouTubeChannelTabPages.Videos) ||
							channelTab.IsChannelTabPage(YouTubeChannelTabPages.Live) ||
							channelTab.IsChannelTabPage(YouTubeChannelTabPages.Shorts)))
						{
							JObject jContent = channelTab.Json.Value<JObject>("tabRenderer").Value<JObject>("content");
							JArray jaItems = jContent.Value<JObject>("richGridRenderer").Value<JArray>("contents");
							List<string> ids = Utils.ExtractVideoIDsFromGridRendererItems(jaItems, out string token);

							return new YouTubeVideosTabPage(ids, token);
						}
					}
				}
				else
				{
					JArray items = GetContinuationItems();
					List<string> ids = Utils.ExtractVideoIDsFromGridRendererItems(items, out string token);

					return new YouTubeVideosTabPage(ids, token);
				}
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
			return null;
		}
	}
}
