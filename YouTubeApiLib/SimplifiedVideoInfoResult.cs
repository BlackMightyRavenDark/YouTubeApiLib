
namespace YouTubeApiLib
{
	public class SimplifiedVideoInfoResult
	{
		public YouTubeSimplifiedVideoInfo SimplifiedVideoInfo { get; }
		public int ErrorCode { get; }

		public SimplifiedVideoInfoResult(YouTubeSimplifiedVideoInfo simplifiedVideoInfo, int errorCode)
		{
			SimplifiedVideoInfo = simplifiedVideoInfo;
			ErrorCode = errorCode;
		}
	}
}
