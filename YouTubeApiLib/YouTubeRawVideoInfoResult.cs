
namespace YouTubeApiLib
{
	public class YouTubeRawVideoInfoResult
	{
		public YouTubeRawVideoInfo RawVideoInfo { get; }
		public int ErrorCode { get; }

		public YouTubeRawVideoInfoResult(YouTubeRawVideoInfo rawVideoInfo, int errorCode)
		{
			RawVideoInfo = rawVideoInfo;
			ErrorCode = errorCode;
		}
	}
}
