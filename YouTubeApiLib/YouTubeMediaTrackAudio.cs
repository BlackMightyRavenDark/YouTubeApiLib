
namespace YouTubeApiLib
{
    public class YouTubeMediaTrackAudio : YouTubeMediaTrack
    {
        public string AudioQuality { get; }
        public int SampleRate { get; }
        public int ChannelCount { get; }
        public double LoudnessDb { get; }

        public YouTubeMediaTrackAudio(
            int formatId,
            int bitrate,
            int averageBitrate,
            string lastModified,
            long contentLength,
            string quality,
            string qualityLabel,
            string audioQuality,
            int sampleRate,
            int channelCount,
            double loudnessDb,
            int approxDurationMs,
            string fileUrl,
            string cipherSignatureEncrypted,
            string cipherEncryptedFileUrl,
            string mimeType,
            string mimeExt,
            string mimeCodecs,
            string fileExtension,
            bool isDash,
            bool isHls,
            bool isCiphered,
            string dashManifestUrl,
            DashUrlList dashUrls)
            : base(formatId, bitrate, averageBitrate, lastModified, contentLength,
                  quality, qualityLabel, approxDurationMs, null, fileUrl,
                  cipherSignatureEncrypted, cipherEncryptedFileUrl,
                  mimeType, mimeExt, mimeCodecs, fileExtension,
                  isDash, isHls, isCiphered, dashManifestUrl, dashUrls, null, null)
        {
            AudioQuality = audioQuality;
            SampleRate = sampleRate;
            ChannelCount = channelCount;
            LoudnessDb = loudnessDb;
        }

        //Simplified constructor for DASH audio track
        public YouTubeMediaTrackAudio(
            int formatId,
            int bitrate,
            int sampleRate,
            int channelCount,
            string mimeType,
            string mimeExt,
            string mimeCodecs,
            string fileExtension,
            string dashManifestUrl,
            DashUrlList dashUrls)
            : base(formatId, bitrate, bitrate, null, -1L, null, null, -1,
                  null, null, null, null,
                  mimeType, mimeExt, mimeCodecs, fileExtension,
                  true, false, false, dashManifestUrl, dashUrls, null, null)
        {
            AudioQuality = null;
            SampleRate = sampleRate;
            ChannelCount = channelCount;
            LoudnessDb = 0.0;
        }

        //Simplified constructor for non-DASH audio track
        public YouTubeMediaTrackAudio(
            int formatId,
            int bitrate,
            int averageBitrate,
            string lastModified,
            long contentLength,
            string quality,
            string qualityLabel,
            string audioQuality,
            int sampleRate,
            int channelCount,
            double loudnessDb,
            int approxDurationMs,
            string fileUrl,
            string cipherSignatureEncrypted,
            string cipherEncryptedFileUrl,
            string mimeType,
            string mimeExt,
            string mimeCodecs,
            string fileExtension,
            bool isCiphered)
            : base(formatId, bitrate, averageBitrate, lastModified, contentLength,
                  quality, qualityLabel, approxDurationMs, null, fileUrl,
                  cipherSignatureEncrypted, cipherEncryptedFileUrl,
                  mimeType, mimeExt, mimeCodecs, fileExtension,
                  false, false, isCiphered, null, null, null, null)
        {
            AudioQuality = audioQuality;
            SampleRate = sampleRate;
            ChannelCount = channelCount;
            LoudnessDb = loudnessDb;
        }
    }
}
