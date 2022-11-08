
namespace YouTubeApiLib
{
    public sealed class YouTubeChannelTabResult
    {
        public YouTubeChannelTab ChannelTab { get; private set; }
        public int ErrorCode { get; private set; }

        public YouTubeChannelTabResult(YouTubeChannelTab channelTab, int errorCode)
        {
            ChannelTab = channelTab;
            ErrorCode = errorCode;
        }
    }
}
