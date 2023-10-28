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

            JObject json = JObject.Parse(RawData);
            JArray jaTabs = json.Value<JObject>("contents")?.Value<JObject>("twoColumnBrowseResultsRenderer")?.Value<JArray>("tabs");
            return jaTabs != null ? new YouTubeChannelTabPageContentTabs(jaTabs.ToString()) : null;
        }

        private JArray GetContinuationItems()
        {
            JObject json = JObject.Parse(RawData);
            return json.Value<JArray>("onResponseReceivedActions")[0]
                .Value<JObject>("appendContinuationItemsAction")
                .Value<JArray>("continuationItems");
        }

        public YouTubeVideosTabPage ParseAsVideosTabPage()
        {
            try
            {
                YouTubeChannelTabPageContentTabs tabs = GetTabs();
                if (tabs != null)
                {
                    if (!string.IsNullOrEmpty(tabs.RawData))
                    {
                        YouTubeChannelTab tabVideos = tabs.GetSelectedTab();
                        if (tabVideos != null && tabVideos.IsChannelTabPage(YouTubeChannelTabPages.Videos))
                        {
                            JObject jContent = tabVideos.Json.Value<JObject>("tabRenderer").Value<JObject>("content");
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
