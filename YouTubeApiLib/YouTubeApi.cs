using static YouTubeApiLib.Utils;

namespace YouTubeApiLib
{
	public sealed class YouTubeApi
	{
		public static bool getMediaTracksInfoImmediately = false;
		internal static YouTubeVideoInfoGettingMethod defaultVideoInfoGettingMethod = YouTubeVideoInfoGettingMethod.HiddenApiEncryptedUrls;
		public static bool decryptMediaTrackUrlsAutomaticallyIfPossible = true;

		public YouTubeVideo GetVideo(YouTubeVideoId youTubeVideoId)
		{
			YouTubeRawVideoInfoResult rawVideoInfoResult = YouTubeRawVideoInfo.Get(youTubeVideoId);
			if (rawVideoInfoResult.ErrorCode == 200)
			{
				return MakeYouTubeVideo(rawVideoInfoResult.RawVideoInfo);
			}
			return YouTubeVideo.CreateEmpty(new YouTubeVideoPlayabilityStatus(
				null, null, null, rawVideoInfoResult.ErrorCode, null));
		}

		public YouTubeVideo GetVideo(YouTubeVideoWebPage videoWebPage)
		{
			return YouTubeVideo.GetByWebPage(videoWebPage);
		}

		public YouTubeVideo GetVideo(string webPageCode)
		{
			return YouTubeVideo.GetByWebPage(webPageCode);
		}

		public YouTubeSimplifiedVideoInfoResult GetSimplifiedVideoInfo(string videoId)
		{
			return Utils.GetSimplifiedVideoInfo(videoId);
		}

		public YouTubeVideoListResult GetChannelVideoList(YouTubeChannel channel)
		{
			return YouTubeApiV1.GetChannelVideoList(channel.Id);
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
	}
}
