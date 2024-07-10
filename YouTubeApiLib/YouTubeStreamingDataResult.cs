
namespace YouTubeApiLib
{
	public class YouTubeStreamingDataResult
	{
		public YouTubeStreamingData Data { get; }
		public int ErrorCode { get; }

		public YouTubeStreamingDataResult(YouTubeStreamingData data, int errorCode)
		{
			Data = data;
			ErrorCode = errorCode;
		}
	}
}
