using System.Collections.Generic;

namespace YouTubeApiLib
{
    public class YouTubeMediaTrackAudio : YouTubeMediaTrack
    {
        public string AudioQuality { get; private set; }
        public int SampleRate { get; private set; }
        public int ChannelCount { get; private set; }
        public double LoudnessDb { get; private set; }

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
            List<string> dashUrls,
            List<string> hlsUrls)
            : base(formatId, bitrate, averageBitrate, lastModified, contentLength,
                  quality, qualityLabel, approxDurationMs, null, fileUrl,
                  cipherSignatureEncrypted, cipherEncryptedFileUrl,
                  mimeType, mimeExt, mimeCodecs, fileExtension,
                  isDash, isHls, isCiphered, dashManifestUrl, dashUrls, hlsUrls)
        {
            AudioQuality = audioQuality;
            SampleRate = sampleRate;
            ChannelCount = channelCount;
            LoudnessDb = loudnessDb;
        }
    }
}
