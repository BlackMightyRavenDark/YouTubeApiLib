using System.Collections.Generic;
using System.Linq;

namespace YouTubeApiLib
{
	public class YouTubeMediaFormatList
	{
		public List<YouTubeMediaTrack> Tracks { get; }
		public IYouTubeClient Client { get; }
		public YouTubeMediaTrackUrlDecryptionData UrlDecryptionData { get; }

		public YouTubeMediaFormatList(IEnumerable<YouTubeMediaTrack> tracks,
			IYouTubeClient client, YouTubeMediaTrackUrlDecryptionData urlDecryptionData)
		{
			Tracks = tracks.ToList();
			Client = client;
			UrlDecryptionData = urlDecryptionData;
		}
	}
}
