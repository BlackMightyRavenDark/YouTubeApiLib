using Newtonsoft.Json.Linq;
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

        public YouTubeVideo GetVideo(string webPageCode)
        {
            string videoInfo = ExtractVideoInfoFromWebPage(webPageCode);
            if (!string.IsNullOrEmpty(videoInfo) && !string.IsNullOrWhiteSpace(videoInfo))
            {
                JObject j = JObject.Parse(videoInfo);
                if (j != null)
                {
                    RawVideoInfo rawVideoInfo = new RawVideoInfo(j, VideoInfoGettingMethod.Manual);
                    SimplifiedVideoInfoResult simplifiedVideoInfoResult = ParseRawVideoInfo(rawVideoInfo);
                    if (simplifiedVideoInfoResult.ErrorCode == 200)
                    {
                        return MakeYouTubeVideo(rawVideoInfo, simplifiedVideoInfoResult.SimplifiedVideoInfo);
                    }
                }
            }
            return null;
        }

        public RawVideoInfoResult GetRawVideoInfo(VideoId videoId)
        {
            return Utils.GetRawVideoInfo(videoId.Id, defaultVideoInfoGettingMethod);
        }

        public SimplifiedVideoInfoResult GetSimplifiedVideoInfo(string videoId)
        {
            return Utils.GetSimplifiedVideoInfo(videoId);
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
