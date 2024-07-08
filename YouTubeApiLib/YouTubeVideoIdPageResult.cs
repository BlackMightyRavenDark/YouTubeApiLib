
namespace YouTubeApiLib
{
	public class YouTubeVideoIdPageResult
	{
		public YouTubeVideoIdPage VideoIdPage { get; }
		public int ErrorCode { get; }

		public YouTubeVideoIdPageResult(YouTubeVideoIdPage videoIdPage, int errorCode)
		{
			VideoIdPage = videoIdPage;
			ErrorCode = errorCode;
		}
	}
}
