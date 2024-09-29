using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
	public class YouTubeVideoWebPage
	{
		public string WebPageCode { get; }
		public bool IsProvidedManually { get; }

		private YouTubeVideoWebPage(string webPageCode, bool isProvidedManually)
		{
			WebPageCode = webPageCode;
			IsProvidedManually = isProvidedManually;
		}

		internal static YouTubeVideoWebPageResult Get(string videoId)
		{
			string url = Utils.GetYouTubeVideoUrl(videoId);
			int errorCode = Utils.DownloadString(url, out string responseWebPageCode);
			YouTubeVideoWebPage webPage = errorCode == 200 ? new YouTubeVideoWebPage(responseWebPageCode, false) : null;
			return new YouTubeVideoWebPageResult(webPage, errorCode);
		}

		public static YouTubeVideoWebPageResult Get(YouTubeVideoId youTubeVideoId)
		{
			return youTubeVideoId != null ? Get(youTubeVideoId.Id) : new YouTubeVideoWebPageResult(null, 400);
		}

		public static YouTubeVideoWebPageResult FromCode(string webPageCode)
		{
			if (!string.IsNullOrEmpty(webPageCode) && !string.IsNullOrWhiteSpace(webPageCode))
			{
				return new YouTubeVideoWebPageResult(new YouTubeVideoWebPage(webPageCode, true), 200);
			}
			return new YouTubeVideoWebPageResult(null, 404);
		}

		public JObject ExtractYouTubeConfig(string pattern)
		{
			return !string.IsNullOrEmpty(WebPageCode) ?
				Utils.ExtractYouTubeConfigFromWebPageCode(WebPageCode, pattern) : null;
		}

		public JObject ExtractYouTubeConfig()
		{
			return !string.IsNullOrEmpty(WebPageCode) ?
				Utils.ExtractYouTubeConfigFromWebPageCode(WebPageCode) : null;
		}
	}
}
