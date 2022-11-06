using System.Collections.Generic;

namespace YouTubeApiLib
{
    public class VideoPage
    {
        public List<YouTubeVideo> Videos { get; private set; }
        public string ContinuationToken { get; private set; }

        public VideoPage(List<YouTubeVideo> videos, string continuationToken)
        {
            Videos = videos;
            ContinuationToken = continuationToken;
        }
    }
}
