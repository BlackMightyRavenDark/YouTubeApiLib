using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
	public class YouTubeConfig
	{
		public string VideoId { get; }
		public JObject Data { get; }
		public string RawData { get; }

		public int SignatureTimestamp => Data != null ? Data.Value<int>("STS") : -1;
		public string VisitorData => Data?.Value<string>("VISITOR_DATA");
		public string PlayerUrl => FindPlayerUrl();

		public YouTubeConfig(string videoId, string rawData)
		{
			VideoId = videoId;
			RawData = rawData;
			Data = Utils.TryParseJson(rawData);
		}

		private string FindPlayerUrl()
		{
			if (Data != null)
			{
				const string pattern = @"""jsUrl"":\s*""([^""]*)""";
				Regex regex = new Regex(pattern);
				string dataString = Data.ToString();
				MatchCollection matches = regex.Matches(dataString);
				if (matches.Count > 0 && matches[0].Groups.Count > 0)
				{
					string url = Utils.YOUTUBE_URL + matches[0].Groups[1].Value;
					return url;
				}
			}

			return null;
		}
	}
}
