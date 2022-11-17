using System.Collections.Generic;

namespace YouTubeApiLib
{
    public class YouTubeMediaTrackContainer : YouTubeMediaTrack
    {
        public int VideoWidth { get; private set; }
        public int VideoHeight { get; private set; }
        public int VideoFrameRate { get; private set; }
        public string AudioQuality { get; private set; }
        public int AudioSampleRate { get; private set; }
        public int AudioChannelCount { get; private set; }

        public YouTubeMediaTrackContainer(
            int formatId,
            int videoWidth, int videoHeight,
            int videoFrameRate,
            int bitrate,
            int averageBitrate,
            string lastModified,
            long contentLength,
            string quality,
            string qualityLabel,
            string audioQuality,
            int audioSampleRate,
            int audioChannelCount,
            int approxDurationMs,
            string projectionType,
            string fileUrl,
            string cipherSignatureEncrypted,
            string cipherEncryptedFileUrl,
            string mimeType,
            string mimeExt,
            string mimeCodecs,
            string fileExtension,
            bool isDashManifest,
            bool isHlsManifest,
            bool isCiphered,
            string dashManifestUrl,
            List<string> dashUrls,
            List<string> hlsUrls)
            : base(formatId, bitrate, averageBitrate, lastModified, contentLength,
                  quality, qualityLabel, approxDurationMs, projectionType, fileUrl,
                  cipherSignatureEncrypted, cipherEncryptedFileUrl,
                  mimeType, mimeExt, mimeCodecs, fileExtension,
                  isDashManifest, isHlsManifest, isCiphered, dashManifestUrl, dashUrls, hlsUrls)
        {
            VideoWidth = videoWidth;
            VideoHeight = videoHeight;
            VideoFrameRate = videoFrameRate;
            AudioQuality = audioQuality;
            AudioSampleRate = audioSampleRate;
            AudioChannelCount = audioChannelCount;
        }
    }
}
