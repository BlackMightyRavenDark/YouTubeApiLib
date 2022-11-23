
namespace YouTubeApiLib
{
    public class YouTubeBroadcast
    {
        public int FormatId { get; private set; }
        public int VideoWidth { get; private set; }
        public int VideoHeight { get; private set; }
        public int FrameRate { get; private set; }
        public int Bandwidth { get; private set; }
        public string Codecs { get; private set; }
        public string PlaylistUrl { get; private set; }

        public YouTubeBroadcast(int formatId, int videoWidth, int videoHeight,
            int frameRate, int bandwidth, string codecs, string playlistUrl)
        {
            FormatId = formatId;
            VideoWidth = videoWidth;
            VideoHeight = videoHeight;
            FrameRate = frameRate;
            Bandwidth = bandwidth;
            Codecs = codecs;
            PlaylistUrl = playlistUrl;
        }
    }
}
