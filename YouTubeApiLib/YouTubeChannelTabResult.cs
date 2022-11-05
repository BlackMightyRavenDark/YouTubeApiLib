
namespace YouTubeApiLib
{
    public sealed class YouTubeChannelTabResult
    {
        public YouTubeChannelTab Tab { get; private set; }
        public int ErrorCode { get; private set; }

        public YouTubeChannelTabResult(YouTubeChannelTab channelTab, int errorCode)
        {
            Tab = channelTab;
            ErrorCode = errorCode;
        }
    }
}
