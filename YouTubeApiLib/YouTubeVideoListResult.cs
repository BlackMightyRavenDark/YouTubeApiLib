using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
	public class YouTubeVideoListResult
	{
		public JArray List { get; }
		public int ErrorCode { get; }

		public YouTubeVideoListResult(JArray list, int errorCode)
		{
			List = list;
			ErrorCode = errorCode;
		}
	}
}
