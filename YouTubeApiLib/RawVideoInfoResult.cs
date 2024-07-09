
namespace YouTubeApiLib
{
	public class RawVideoInfoResult
	{
		public YouTubeRawVideoInfo RawVideoInfo { get; }
		public int ErrorCode { get; }

		public RawVideoInfoResult(YouTubeRawVideoInfo rawVideoInfo, int errorCode)
		{
			RawVideoInfo = rawVideoInfo;
			ErrorCode = errorCode;
		}
	}
}
