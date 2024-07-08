
namespace YouTubeApiLib
{
	public class VideoIdPageResult
	{
		public YouTubeVideoIdPage VideoIdPage { get; }
		public int ErrorCode { get; }

		public VideoIdPageResult(YouTubeVideoIdPage videoIdPage, int errorCode)
		{
			VideoIdPage = videoIdPage;
			ErrorCode = errorCode;
		}
	}
}
