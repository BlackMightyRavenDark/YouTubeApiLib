using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
	public class YouTubeVideoThumbnail
	{
		public string Id { get; }
		public string Url { get; }

		public YouTubeVideoThumbnail(string id, string url)
		{
			Id = id;
			Url = url;
		}

		public JObject ToJson()
		{
			JObject json = new JObject() { ["id"] = Id, ["url"] = Url };
			return json;
		}

		public override string ToString()
		{
			return $"{Id}: {Url}";
		}
	}
}
