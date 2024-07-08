
namespace YouTubeApiLib
{
	public sealed class YouTubeChannelTabResult
	{
		public YouTubeChannelTab ChannelTab { get; }
		public int ErrorCode { get; }

		public YouTubeChannelTabResult(YouTubeChannelTab channelTab, int errorCode)
		{
			ChannelTab = channelTab;
			ErrorCode = errorCode;
		}
	}
}
