
namespace YouTubeApiLib
{
	public class VideoIdPageResult
	{
		public VideoIdPage VideoIdPage { get; }
		public int ErrorCode { get; }

		public VideoIdPageResult(VideoIdPage videoIdPage, int errorCode)
		{
			VideoIdPage = videoIdPage;
			ErrorCode = errorCode;
		}
	}
}
