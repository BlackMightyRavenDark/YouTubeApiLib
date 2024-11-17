using System.Collections.Generic;

namespace YouTubeApiLib
{
	public sealed class YouTubeApi
	{
		public static bool getMediaTracksInfoImmediately = false;
		private static string _defaultYouTubeClientId = "ios";
		private static Dictionary<string, IYouTubeClient> _clients = new Dictionary<string, IYouTubeClient>()
		{
			["video_info"] = new YouTubeClientVideoInfo(),
			["web_page"] = new YouTubeClientWebPage(),
			["ios"] = new YouTubeClientIos()
		};

		public YouTubeVideo GetVideo(YouTubeVideoId youTubeVideoId, IYouTubeClient client)
		{
			return YouTubeVideo.GetById(youTubeVideoId, client);
		}

		public YouTubeVideo GetVideo(YouTubeVideoId youTubeVideoId)
		{
			IYouTubeClient client = GetYouTubeClient("video_info");
			return client != null ? GetVideo(youTubeVideoId, client) : null;
		}

		public YouTubeVideo GetVideo(YouTubeVideoWebPage videoWebPage)
		{
			return YouTubeVideo.GetByWebPage(videoWebPage);
		}

		public YouTubeVideo GetVideo(string webPageCode)
		{
			return YouTubeVideo.GetByWebPage(webPageCode);
		}

		public YouTubeSimplifiedVideoInfoResult GetSimplifiedVideoInfo(string videoId, IYouTubeClient client)
		{
			return Utils.GetSimplifiedVideoInfo(videoId, client);
		}

		public YouTubeVideoListResult GetChannelVideoList(YouTubeChannel channel)
		{
			return YouTubeApiV1.GetChannelVideoList(channel.Id, null);
		}

		public YouTubeVideoIdPageResult GetVideoIdPage(YouTubeChannel youTubeChannel, string continuationToken)
		{
			return GetVideoIdPage(youTubeChannel, null, continuationToken);
		}

		public YouTubeVideoIdPageResult GetVideoIdPage(
			YouTubeChannel youTubeChannel, YouTubeChannelTabPage channelTabPage, string continuationToken)
		{
			return YouTubeApiV1.GetVideoIdPage(youTubeChannel?.Id, channelTabPage, continuationToken);
		}

		public YouTubeVideoPageResult GetVideoPage(YouTubeChannel youTubeChannel, string continuationToken)
		{
			return GetVideoPage(youTubeChannel, null, continuationToken);
		}

		public YouTubeVideoPageResult GetVideoPage(
			YouTubeChannel youTubeChannel, YouTubeChannelTabPage channelTabPage, string continuationToken)
		{
			return YouTubeApiV1.GetVideoPage(youTubeChannel?.Id, channelTabPage, continuationToken);
		}

		public YouTubeChannelTabResult GetChannelTab(
			YouTubeChannel youTubeChannel, YouTubeChannelTabPage youTubeChannelTabPage)
		{
			return YouTubeApiV1.GetChannelTab(youTubeChannel.Id, youTubeChannelTabPage);
		}

		public YouTubeApiV1SearchResults Search(string searchQuery, string continuationToken,
			YouTubeApiV1SearchResultFilter searchResultFilter)
		{
			return YouTubeApiV1.SearchYouTube(searchQuery, continuationToken, searchResultFilter);
		}

		public static IYouTubeClient GetYouTubeClient(string clientId)
		{
			lock (_clients)
			{
				return _clients.ContainsKey(clientId) ? _clients[clientId] : null;
			}
		}

		public static string GetDefaultYouTubeClientId()
		{
			lock (_defaultYouTubeClientId)
			{
				return _defaultYouTubeClientId;
			}
		}

		public static void SetDefaultYouTubeClientId(string clientId)
		{
			lock (_defaultYouTubeClientId)
			{
				_defaultYouTubeClientId = clientId;
			}
		}
	}
}
