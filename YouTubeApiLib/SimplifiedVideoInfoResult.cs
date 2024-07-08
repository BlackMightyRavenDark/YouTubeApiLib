
namespace YouTubeApiLib
{
	public class SimplifiedVideoInfoResult
	{
		public SimplifiedVideoInfo SimplifiedVideoInfo { get; }
		public int ErrorCode { get; }

		public SimplifiedVideoInfoResult(SimplifiedVideoInfo simplifiedVideoInfo, int errorCode)
		{
			SimplifiedVideoInfo = simplifiedVideoInfo;
			ErrorCode = errorCode;
		}
	}
}
