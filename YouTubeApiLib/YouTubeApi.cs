using static YouTubeApiLib.Utils;

namespace YouTubeApiLib
{
    public sealed class YouTubeApi
    {
        public static bool getMediaTracksInfoImmediately = false;
        internal static VideoInfoGettingMethod defaultVideoInfoGettingMethod = VideoInfoGettingMethod.HiddenApiEncryptedUrls;
        public static bool decryptMediaTrackUrlsAutomaticallyIfPossible = true;

        public YouTubeVideo GetVideo(VideoId videoId)
        {
            RawVideoInfoResult rawVideoInfoResult = GetRawVideoInfo(videoId);
            if (rawVideoInfoResult.ErrorCode == 200)
            {
                return MakeYouTubeVideo(rawVideoInfoResult.RawVideoInfo);
            }
            return YouTubeVideo.CreateEmpty(new YouTubeVideoPlayabilityStatus(null, null, rawVideoInfoResult.ErrorCode, null));
        }

        public RawVideoInfoResult GetRawVideoInfo(VideoId videoId)
        {
            return Utils.GetRawVideoInfo(videoId.Id, defaultVideoInfoGettingMethod);
        }

        public SimplifiedVideoInfoResult GetSimplifiedVideoInfo(string videoId, out StreamingData streamingData)
        {
            return Utils.GetSimplifiedVideoInfo(videoId, out streamingData);
        }

        public VideoListResult GetChannelVideoList(YouTubeChannel channel)
        {
            return Utils.GetChannelVideoList(channel.Id);
        }

        public VideoIdPageResult GetVideoIdPage(YouTubeChannel youTubeChannel, string continuationToken)
        {
            return GetVideoIdPage(youTubeChannel, null, continuationToken);
        }

        public VideoIdPageResult GetVideoIdPage(
            YouTubeChannel youTubeChannel, YouTubeChannelTabPage channelTabPage, string continuationToken)
        {
            return Utils.GetVideoIdPage(youTubeChannel?.Id, channelTabPage, continuationToken);
        }

        public VideoPageResult GetVideoPage(YouTubeChannel youTubeChannel, string continuationToken)
        {
            return GetVideoPage(youTubeChannel, null, continuationToken);
        }

        public VideoPageResult GetVideoPage(
            YouTubeChannel youTubeChannel, YouTubeChannelTabPage channelTabPage, string continuationToken)
        {
            return Utils.GetVideoPage(youTubeChannel?.Id, channelTabPage, continuationToken);
        }

        public YouTubeChannelTabResult GetChannelTab(
            YouTubeChannel youTubeChannel, YouTubeChannelTabPage youTubeChannelTabPage)
        {
            return Utils.GetChannelTab(youTubeChannel.Id, youTubeChannelTabPage);
        }
    }
}
