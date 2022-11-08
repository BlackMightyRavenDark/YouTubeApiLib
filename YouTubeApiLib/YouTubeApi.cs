using static YouTubeApiLib.Utils;

namespace YouTubeApiLib
{
    public sealed class YouTubeApi
    {
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
            return Utils.GetRawVideoInfo(videoId.Id);
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
            return Utils.GetVideoIdPage(youTubeChannel?.Id, continuationToken);
        }

        public VideoPageResult GetVideoPage(YouTubeChannel youTubeChannel, string continuationToken)
        {
            return Utils.GetVideoPage(youTubeChannel?.Id, continuationToken);
        }

        public YouTubeChannelTabResult GetChannelTab(
            YouTubeChannel youTubeChannel, YouTubeChannelTabPage youTubeChannelTabPage)
        {
            return Utils.GetChannelTab(youTubeChannel.Id, youTubeChannelTabPage);
        }
    }
}
