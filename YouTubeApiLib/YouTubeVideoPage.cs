using System.Collections.Generic;
using System.Linq;

namespace YouTubeApiLib
{
	public class YouTubeVideoPage
	{
		public List<YouTubeVideo> Videos { get; }
		public string ContinuationToken { get; }

		public YouTubeVideoPage(IEnumerable<YouTubeVideo> videos, string continuationToken)
		{
			Videos = videos.ToList();
			ContinuationToken = continuationToken;
		}
	}
}
