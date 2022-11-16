using System.Collections.Generic;

namespace YouTubeApiLib
{
    public abstract class YouTubeMediaTrack
    {
        public int FormatId { get; private set; }
        public string MimeType { get; private set; }
        public string MimeExt { get; private set; }
        public string MimeCodecs { get; private set; }
        public int Bitrate { get; private set; }
        public int AverageBitrate { get; private set; }
        public string LastModified { get; private set; }
        public long ContentLength { get; private set; }
        public string Quality { get; private set; }
        public string QualityLabel { get; private set; }
        public int ApproxDurationMs { get; private set; }
        public string ProjectionType { get; private set; }
        public string FileUrl { get; private set; }
        public string CipherSignatureEncrypted { get; private set; }
        public string CipherEncryptedFileUrl { get; private set; }
        public string FileExtension { get; private set; }
        public bool IsDashManifest { get; private set; }
        public bool IsHlsManifest { get; private set; }
        public bool IsCiphered { get; private set; }
        public List<string> DashUrls { get; private set; }
        public List<string> HlsUrls { get; private set; }

        public YouTubeMediaTrack(
            int formatId,
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
            List<string> dashUrls,
            List<string> hlsUrls)
        {
            FormatId = formatId;
            Bitrate = bitrate;
            AverageBitrate = averageBitrate;
            LastModified = lastModified;
            ContentLength = contentLength;
            Quality = quality;
            QualityLabel = qualityLabel;
            ApproxDurationMs = approxDurationMs;
            ProjectionType = projectionType;
            FileUrl = fileUrl;
            CipherSignatureEncrypted = cipherSignatureEncrypted;
            CipherEncryptedFileUrl = cipherEncryptedFileUrl;
            MimeType = mimeType;
            MimeExt = mimeExt;
            MimeCodecs = mimeCodecs;
            FileExtension = fileExtension;
            IsDashManifest = isDashManifest;
            IsHlsManifest = isHlsManifest;
            IsCiphered = isCiphered;
            DashUrls = dashUrls;
            HlsUrls = hlsUrls;
        }

        public virtual string GetShortInfo()
        {
            return null;
        }
    }
}
