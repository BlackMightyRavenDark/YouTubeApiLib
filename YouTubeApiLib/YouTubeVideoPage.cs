using System.Collections.Generic;

namespace YouTubeApiLib
{
	public class YouTubeVideoPage
	{
		public List<YouTubeVideo> Videos { get; }
		public string ContinuationToken { get; }

		public YouTubeVideoPage(List<YouTubeVideo> videos, string continuationToken)
		{
			Videos = videos;
			ContinuationToken = continuationToken;
		}
	}
}
