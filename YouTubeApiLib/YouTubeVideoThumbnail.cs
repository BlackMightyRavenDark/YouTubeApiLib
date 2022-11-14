using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
    public class YouTubeVideoThumbnail
    {
        public string Id { get; private set; }
        public string Url { get; private set; }

        public YouTubeVideoThumbnail(string id, string url)
        {
            Id = id;
            Url = url;
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["id"] = Id;
            json["url"] = Url;
            return json;
        }

        public override string ToString()
        {
            return $"{Id}: {Url}";
        }
    }
}
