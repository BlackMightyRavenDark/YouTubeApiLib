
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

        public override string ToString()
        {
            return $"{DisplayName} ({Id})";
        }
    }
}
