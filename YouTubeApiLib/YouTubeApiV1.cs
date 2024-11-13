using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using static YouTubeApiLib.Utils;

namespace YouTubeApiLib
{
	internal static class YouTubeApiV1
	{
		internal const string API_V1_KEY = "AIzaSyAO_FJ2SlqU8Q4STEHLGCilw_Y9_11qcW8";
		internal const string API_V1_BROWSE_URL = "https://www.youtube.com/youtubei/v1/browse";
		internal const string API_V1_PLAYER_URL = "https://www.youtube.com/youtubei/v1/player";
		internal const string API_V1_SEARCH_URL = "https://www.youtube.com/youtubei/v1/search";

		public static JObject GenerateSearchQueryRequestBody(
			string searchQuery, string continuationToken, YouTubeApiV1SearchResultFilter filter)
		{
			const string CLIENT_NAME = "WEB";
			const string CLIENT_VERSION = "2.20210408.08.00";

			JObject jClient = GenerateYouTubeClientBody(CLIENT_NAME, CLIENT_VERSION);
			jClient["utcOffsetMinutes"] = 0;

			JObject jContext = new JObject()
			{
				["client"] = jClient
			};

			JObject json = new JObject()
			{
				["context"] = jContext
			};

			if (!string.IsNullOrEmpty(searchQuery) && !string.IsNullOrWhiteSpace(searchQuery))
			{
				json["query"] = searchQuery;
				if (filter != YouTubeApiV1SearchResultFilters.None)
				{
					json["params"] = filter.ParamsId;
				}
			}
			else if (!string.IsNullOrEmpty(continuationToken) && !string.IsNullOrWhiteSpace(continuationToken))
			{
				json["continuation"] = continuationToken;
			}

			return json;
		}

		public static JObject GenerateChannelTabRequestBody(string channelId,
			YouTubeChannelTabPage youTubeChannelTabPage, string continuationToken)
		{
			const string CLIENT_NAME = "WEB";
			const string CLIENT_VERSION = "2.20241029.07.00";

			JObject jClient = GenerateYouTubeClientBody(CLIENT_NAME, CLIENT_VERSION);
			JObject jContext = new JObject()
			{
				["client"] = jClient
			};

			JObject json = new JObject()
			{
				["context"] = jContext,
				["browseId"] = channelId
			};

			bool tokenExists = !string.IsNullOrEmpty(continuationToken) && !string.IsNullOrWhiteSpace(continuationToken);
			if (tokenExists)
			{
				json["continuation"] = continuationToken;
			}
			else if (youTubeChannelTabPage != null)
			{
				json["params"] = youTubeChannelTabPage.ParamsId;
			}

			return json;
		}

		public static JObject GenerateYouTubeClientBody(string clientName, string clientVersion, string hl, string gl)
		{
			return new JObject()
				{
					["clientName"] = clientName,
					["clientVersion"] = clientVersion,
					["hl"] = hl,
					["gl"] = gl
				};
		}

		public static JObject GenerateYouTubeClientBody(string clientName, string clientVersion)
		{
			return GenerateYouTubeClientBody(clientName, clientVersion, "en", "US");
		}

		public static JObject GenerateChannelVideoListRequestBody(string channelId, string continuationToken)
		{
			return GenerateChannelTabRequestBody(channelId, YouTubeChannelTabPages.Videos, continuationToken);
		}

		public static string GetBrowseRequestUrl()
		{
			return $"{API_V1_BROWSE_URL}?key={API_V1_KEY}";
		}

		public static string GetPlayerRequestUrl()
		{
			return $"{API_V1_PLAYER_URL}?key={API_V1_KEY}";
		}

		public static string GetSearchRequestUrl()
		{
			return $"{API_V1_SEARCH_URL}?key={API_V1_KEY}";
		}

		internal static YouTubeRawVideoInfoResult GetRawVideoInfo(YouTubeVideoId videoId)
		{
			IYouTubeClient client = YouTubeApi.GetYouTubeClient("video_info");
			return client.GetRawVideoInfo(videoId, out _);
		}

		internal static YouTubeRawVideoInfoResult GetRawVideoInfo(string videoId)
		{
			return GetRawVideoInfo(new YouTubeVideoId(videoId));
		}

		internal static YouTubeVideoPageResult GetVideoPage(string channelId, YouTubeChannelTabPage tabPage, string continuationToken)
		{
			YouTubeVideoIdPageResult videoIdPageResult = GetVideoIdPage(channelId, tabPage, continuationToken);
			if (videoIdPageResult.ErrorCode == 200)
			{
				List<YouTubeVideo> videos = new List<YouTubeVideo>();
				foreach (string videoId in videoIdPageResult.VideoIdPage.VideoIds)
				{
					YouTubeRawVideoInfoResult rawVideoInfoResult = GetRawVideoInfo(videoId);
					if (rawVideoInfoResult.ErrorCode == 200)
					{
						YouTubeVideo video = rawVideoInfoResult.RawVideoInfo.ToVideo();
						if (video != null && video.Status != null)
						{
							videos.Add(video);
						}
					}
				}
				return new YouTubeVideoPageResult(new YouTubeVideoPage(videos, videoIdPageResult.VideoIdPage.ContinuationToken), 200);
			}
			return new YouTubeVideoPageResult(null, videoIdPageResult.ErrorCode);
		}

		internal static YouTubeVideoIdPageResult GetVideoIdPage(string channelId, YouTubeChannelTabPage tabPage, string continuationToken)
		{
			JObject body = GenerateChannelTabRequestBody(channelId, tabPage, continuationToken);
			bool tokenExists = !string.IsNullOrEmpty(continuationToken) && !string.IsNullOrWhiteSpace(continuationToken);
			return GetVideoIdPage(body, tokenExists);
		}

		internal static YouTubeVideoIdPageResult GetVideoIdPage(JObject requestBody, bool continuationTokenExists)
		{
			string url = GetBrowseRequestUrl();
			string body = requestBody != null ? requestBody.ToString() : string.Empty;
			int errorCode = YouTubeHttpPost(url, body, out string response);
			if (errorCode == 200)
			{
				YouTubeVideoIdPage videoIdPage = new YouTubeVideoIdPage(response, continuationTokenExists);
				int count = videoIdPage.Parse();
				return new YouTubeVideoIdPageResult(videoIdPage, count > 0 ? 200 : 400);
			}
			return new YouTubeVideoIdPageResult(null, errorCode);
		}

		internal static YouTubeVideoListResult GetChannelVideoList(string channelId, IYouTubeClient client)
		{
			JArray resList = new JArray();
			string continuationToken = null;
			int errorCode;
			while (true)
			{
				YouTubeVideoIdPageResult videoIdPageResult = GetVideoIdPage(channelId, YouTubeChannelTabPages.Videos, continuationToken);

				errorCode = videoIdPageResult.ErrorCode;
				if (errorCode != 200)
				{
					break;
				}

				foreach (string videoId in videoIdPageResult.VideoIdPage.VideoIds)
				{
					YouTubeSimplifiedVideoInfoResult simplifiedVideoInfoResult = GetSimplifiedVideoInfo(videoId, client);
					if (simplifiedVideoInfoResult.ErrorCode == 200)
					{
						resList.Add(simplifiedVideoInfoResult.SimplifiedVideoInfo.Info);
					}
				}

				continuationToken = videoIdPageResult.VideoIdPage.ContinuationToken;
				bool continuationTokenExists = !string.IsNullOrEmpty(continuationToken) && !string.IsNullOrEmpty(continuationToken);
				if (!continuationTokenExists)
				{
					break;
				}

				System.Diagnostics.Debug.WriteLine(continuationToken);
			}

			return new YouTubeVideoListResult(resList, resList.Count > 0 ? 200 : errorCode);
		}

		internal static YouTubeChannelTabResult GetChannelTab(string channelId, YouTubeChannelTabPage channelTabPage)
		{
			string url = $"{API_V1_BROWSE_URL}?key={API_V1_KEY}";
			JObject body = GenerateChannelTabRequestBody(channelId, channelTabPage, null);
			int errorCode = YouTubeHttpPost(url, body.ToString(), out string response);
			if (errorCode == 200)
			{
				JObject json = TryParseJson(response);
				if (json == null)
				{
					return new YouTubeChannelTabResult(null, 404);
				}

				YouTubeChannelTab selectedTab = FindSelectedChannelTab(json);
				if (selectedTab == null)
				{
					return new YouTubeChannelTabResult(null, 404);
				}

				if (selectedTab.Title != channelTabPage.Title)
				{
					// If the requested tab is not exists,
					// API returns the "Home" tab content.
					return new YouTubeChannelTabResult(null, 404);
				}

				return new YouTubeChannelTabResult(selectedTab, errorCode);
			}

			return new YouTubeChannelTabResult(null, errorCode);
		}

		public static YouTubeChannelTabPageContentResult GetChannelTabContentRawData(
			string channelId, YouTubeChannelTabPage channelTabPage, string pageToken)
		{
			string url = $"{API_V1_BROWSE_URL}?key={API_V1_KEY}";
			JObject body = GenerateChannelTabRequestBody(channelId, channelTabPage, pageToken);
			int errorCode = YouTubeHttpPost(url, body.ToString(), out string response);
			return new YouTubeChannelTabPageContentResult(
				new YouTubeChannelTabPageContent(channelTabPage, response), errorCode);
		}

		internal static YouTubeApiV1SearchResults SearchYouTube(
			string searchQuery, string continuationToken,
			YouTubeApiV1SearchResultFilter searchResultFilter)
		{
			string url = GetSearchRequestUrl();
			JObject body = GenerateSearchQueryRequestBody(
				searchQuery, continuationToken, searchResultFilter);
			int errorCode = YouTubeHttpPost(url, body.ToString(), out string response);
			if (errorCode == 200)
			{
				bool isContinuationToken = !string.IsNullOrEmpty(continuationToken) && !string.IsNullOrWhiteSpace(continuationToken);
				return new YouTubeApiV1SearchResults(response, searchResultFilter, isContinuationToken, errorCode);
			}
			return new YouTubeApiV1SearchResults(null, searchResultFilter, false, errorCode);
		}
	}
}
