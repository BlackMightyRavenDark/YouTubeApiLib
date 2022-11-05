using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
    public class YouTubeChannel
    {
        public string DisplayName { get; private set; }
        public string Id { get; private set; }

        public YouTubeChannel(string id, string displayName)
        {
            Id = id;
            DisplayName = displayName;
        }
    }

    public class YouTubeChannelTab
    {
        public string Title { get; private set; }
        public JObject Json { get; private set; }

        public YouTubeChannelTab(string title, JObject json)
        {
            Title = title;
            Json = json;
        }
    }
}
