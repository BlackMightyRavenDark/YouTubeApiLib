
namespace YouTubeApiLib
{
	public class YouTubeSimplifiedVideoInfoResult
	{
		public YouTubeSimplifiedVideoInfo SimplifiedVideoInfo { get; }
		public int ErrorCode { get; }

		public YouTubeSimplifiedVideoInfoResult(YouTubeSimplifiedVideoInfo simplifiedVideoInfo, int errorCode)
		{
			SimplifiedVideoInfo = simplifiedVideoInfo;
			ErrorCode = errorCode;
		}
	}
}
