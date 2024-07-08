using System.Collections.Generic;

namespace YouTubeApiLib
{
	public class VideoPage
	{
		public List<YouTubeVideo> Videos { get; }
		public string ContinuationToken { get; }

		public VideoPage(List<YouTubeVideo> videos, string continuationToken)
		{
			Videos = videos;
			ContinuationToken = continuationToken;
		}
	}
}
