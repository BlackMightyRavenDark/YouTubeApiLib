﻿using System.Collections.Generic;

namespace YouTubeApiLib
{
    public class YouTubeMediaTrackVideo : YouTubeMediaTrack
    {
        public int VideoWidth { get; private set; }
        public int VideoHeight { get; private set; }
        public int FrameRate { get; private set; }

        public YouTubeMediaTrackVideo(
            int formatId,
            int videoWidth, int videoHeight,
            int frameRate,
            int bitrate,
            int averageBitrate,
            string lastModified,
            long contentLength,
            string quality,
            string qualityLabel,
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
            DashUrlList dashUrls,
            List<string> hlsUrls)
            : base(formatId, bitrate, averageBitrate, lastModified, contentLength,
                  quality, qualityLabel, approxDurationMs, projectionType, fileUrl,
                  cipherSignatureEncrypted, cipherEncryptedFileUrl,
                  mimeType, mimeExt, mimeCodecs, fileExtension,
                  isDashManifest, isHlsManifest, isCiphered, dashManifestUrl, dashUrls, hlsUrls)
        {
            VideoWidth = videoWidth;
            VideoHeight = videoHeight;
            FrameRate = frameRate;
        }

        //Simplified constructor for DASH video track
        public YouTubeMediaTrackVideo(
            int formatId,
            int videoWidth, int videoHeight,
            int frameRate,
            int bitrate,
            string mimeType,
            string mimeExt,
            string mimeCodecs,
            string fileExtension,
            string dashManifestUrl,
            DashUrlList dashUrls)
            : base(formatId, bitrate, bitrate, null, -1L,
                  null, null, -1, null, null, null, null,
                  mimeType, mimeExt, mimeCodecs, fileExtension,
                  true, false, false, dashManifestUrl, dashUrls, null)
        {
            VideoWidth = videoWidth;
            VideoHeight = videoHeight;
            FrameRate = frameRate;
        }
    }
}
