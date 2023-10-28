using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
    public class YouTubeChannelTab
    {
        public string Title { get; private set; }
        public JObject Json { get; private set; }

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
    }
}
