
namespace YouTubeApiLib
{
	public class RawVideoInfoResult
	{
		public RawVideoInfo RawVideoInfo { get; }
		public int ErrorCode { get; }

		public RawVideoInfoResult(RawVideoInfo rawVideoInfo, int errorCode)
		{
			RawVideoInfo = rawVideoInfo;
			ErrorCode = errorCode;
		}
	}
}
