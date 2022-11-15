using System.Collections.Generic;

namespace YouTubeApiLib
{
    public class YouTubeVideoTrack : YouTubeMediaTrack
    {
        public int VideoWidth { get; private set; }
        public int VideoHeight { get; private set; }
        public int FrameRate { get; private set; }

        public YouTubeVideoTrack(
            int formatId,
            int videoWidth, int videoHeight,
            int frameRate,
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
            string url,
            string cipherSignatureEncrypted,
            string cipherEncryptedUrl,
            string mimeType,
            string mimeExt,
            string mimeCodecs,
            string fileExtension,
            bool isContainer,
            bool isDashManifest,
            bool isHlsManifest,
            bool isCiphered,
            List<string> dashUrls,
            List<string> hlsUrls
            )
            : base(formatId, bitrate, averageBitrate, lastModified, contentLength,
                  quality, qualityLabel, audioQuality, audioSampleRate, audioChannelCount,
                  approxDurationMs, projectionType, url, cipherSignatureEncrypted,
                  cipherEncryptedUrl, mimeType, mimeExt, mimeCodecs, fileExtension,
                  isContainer, isDashManifest, isHlsManifest, isCiphered, dashUrls, hlsUrls)
        {
            VideoWidth = videoWidth;
            VideoHeight = videoHeight;
            FrameRate = frameRate;
        }
    }
}
