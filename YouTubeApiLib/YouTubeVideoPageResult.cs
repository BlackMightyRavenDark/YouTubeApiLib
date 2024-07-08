
namespace YouTubeApiLib
{
	public class YouTubeVideoPageResult
	{
		public YouTubeVideoPage VideoPage { get; }
		public int ErrorCode { get; }

		public YouTubeVideoPageResult(YouTubeVideoPage videoPage, int errorCode)
		{
			VideoPage = videoPage;
			ErrorCode = errorCode;
		}
	}
}
