using System.Net;
using Newtonsoft.Json.Linq;
using MultiThreadedDownloaderLib;

namespace YouTubeApiLib.ConsoleTest
{
	internal class YouTubeClientVisionOs : IYouTubeClient
	{
		public string DisplayName => "Vision OS";
		public YouTubeVideoWebPage WebPage { get; private set; }
		public FileDownloader Downloader { get; set; }

		internal const string CLIENT_VERSION = "1.02";
		internal const string OS_NAME = "visionOS";
		internal const string OS_VERSION = "26.5.23O471";

		private const string USER_AGENT = "Mozilla/5.0 (Macintosh; Intel Mac OS X 15_7_3) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/26.0 Safari/605.1.15";

		public JObject GenerateRequestBody(string videoId, YouTubeConfig youTubeConfig = null)
		{
			if (youTubeConfig == null) { return null; }

			JObject jClient = new JObject()
			{
				["clientName"] = "VISIONOS",
				["clientVersion"] = CLIENT_VERSION,
				["deviceMake"] = "Apple",
				["deviceModel"] = "RealityDevice17,1",
				["userAgent"] = USER_AGENT,
				["osName"] = OS_NAME,
				["osVersion"] = OS_VERSION,
				["hl"] = "en",
				["timeZone"] = "UTC",
				["utcOffsetMinutes"] = 0
			};
			JObject jPlaybackContext = new JObject()
			{
				["contentPlaybackContext"] = new JObject()
				{
					["html5Preference"] = "HTML5_PREF_WANTS",
					["signatureTimestamp"] = youTubeConfig.SignatureTimestamp
				}
			};
			JObject json = new JObject()
			{
				["context"] = new JObject() { ["client"] = jClient },
				["playbackContext"] = jPlaybackContext,
				["videoId"] = videoId,
				["contentCheckOk"] = true,
				["racyCheckOk"] = true
			};

			return json;
		}

		public WebHeaderCollection GenerateRequestHeaders(string videoId, YouTubeConfig youTubeConfig = null)
		{
			return youTubeConfig == null ? null : new WebHeaderCollection()
			{
				{ "Origin", YouTubeApiLib.Utils.YOUTUBE_URL },
				{ "X-Goog-Visitor-Id", youTubeConfig.VisitorData },
				{ "X-YouTube-Client-Name", "101" },
				{ "X-YouTube-Client-Version", CLIENT_VERSION },
				{ "User-Agent", USER_AGENT }
			};
		}

		public YouTubeRawVideoInfoResult GetRawVideoInfo(YouTubeVideoId videoId, out string errorMessage)
		{
			int errorCode = GetRawVideoInfo(videoId.Id, out YouTubeRawVideoInfo rawVideoInfo, out errorMessage);
			return new YouTubeRawVideoInfoResult(rawVideoInfo, errorCode);
		}

		public int GetRawVideoInfo(string videoId, out YouTubeRawVideoInfo rawVideoInfo, out string errorMessage)
		{
			if (WebPage == null)
			{
				YouTubeVideoWebPageResult webPageResult = YouTubeVideoWebPage.Get(new YouTubeVideoId(videoId), Downloader);
				if (webPageResult.ErrorCode == 200) { WebPage = webPageResult.VideoWebPage; }
			}
			if (WebPage != null)
			{
				YouTubeConfig youTubeConfig = WebPage.ExtractYouTubeConfig();
				if (youTubeConfig == null)
				{
					rawVideoInfo = null;
					errorMessage = "YouTubeConfig is not found";
					return 404;
				}

				JObject body = GenerateRequestBody(videoId, youTubeConfig);
				WebHeaderCollection headers = GenerateRequestHeaders(videoId, youTubeConfig);
				int errorCode = YouTubeApiLib.Utils.YouTubeHttpPost(YouTubeApiV1.API_V1_PLAYER_URL, body.ToString(), headers, out string response);
				if (errorCode == 200)
				{
					rawVideoInfo = YouTubeRawVideoInfo.MakeFromRaw(response, this, System.DateTime.UtcNow);
					errorMessage = null;
				}
				else
				{
					rawVideoInfo = null;
					errorMessage = response;
				}

				return errorCode;
			}

			rawVideoInfo = null;
			errorMessage = null;
			return 404;
		}

		public void SetWebPage(YouTubeVideoWebPage webPage)
		{
			WebPage = webPage;
		}
	}
}
