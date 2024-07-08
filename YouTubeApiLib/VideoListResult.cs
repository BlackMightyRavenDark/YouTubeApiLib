using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
	public class VideoListResult
	{
		public JArray List { get; }
		public int ErrorCode { get; }

		public VideoListResult(JArray list, int errorCode)
		{
			List = list;
			ErrorCode = errorCode;
		}
	}
}
