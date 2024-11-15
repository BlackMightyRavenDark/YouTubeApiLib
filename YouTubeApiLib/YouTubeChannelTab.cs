using System.Linq;
using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
	public class YouTubeChannelTab
	{
		public string Title { get; }
		public JObject Json { get; }

		public YouTubeChannelTab(string title, JObject json)
		{
			Title = title;
			Json = json;
		}

		public bool IsChannelTabPage(YouTubeChannelTabPage channelTabPage)
		{
			return !string.IsNullOrEmpty(Title) &&
				Title.Equals(channelTabPage.Title, System.StringComparison.OrdinalIgnoreCase);
		}

		public static JArray FindTabList(JObject megaRoot)
		{
			JObject j = megaRoot?.Value<JObject>("contents")?.Value<JObject>("twoColumnBrowseResultsRenderer");
			return j?.Value<JArray>("tabs");
		}

		public static YouTubeChannelTab FindSelectedTab(JArray tabList)
		{
			foreach (JObject jObject in tabList.Cast<JObject>())
			{
				JObject j = jObject.Value<JObject>("tabRenderer") ?? jObject.Value<JObject>("expandableTabRenderer");
				if (j != null)
				{
					bool selected = j.Value<bool>("selected");
					if (selected)
					{
						string tabTitle = j.Value<string>("title");
						return new YouTubeChannelTab(tabTitle, jObject);
					}
				}
			}

			return null;
		}

		public static YouTubeChannelTab FindSelectedTab(JObject megaRoot)
		{
			JArray jaTabs = FindTabList(megaRoot);
			return jaTabs == null || jaTabs.Count == 0 ? null : FindSelectedTab(jaTabs);
		}
	}
}
