
namespace YouTubeApiLib
{
	public class YouTubeChannelTabPageContentResult
	{
		public YouTubeChannelTabPageContent Content { get; }
		public int ErrorCode { get; }

		public YouTubeChannelTabPageContentResult(YouTubeChannelTabPageContent content, int errorCode)
		{
			Content = content;
			ErrorCode = errorCode;
		}
	}
}
