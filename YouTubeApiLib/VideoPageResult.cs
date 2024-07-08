
namespace YouTubeApiLib
{
	public class VideoPageResult
	{
		public YouTubeVideoPage VideoPage { get; }
		public int ErrorCode { get; }

		public VideoPageResult(YouTubeVideoPage videoPage, int errorCode)
		{
			VideoPage = videoPage;
			ErrorCode = errorCode;
		}
	}
}
