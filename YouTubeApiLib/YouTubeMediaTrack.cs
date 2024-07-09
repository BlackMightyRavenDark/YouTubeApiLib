
namespace YouTubeApiLib
{
	public abstract class YouTubeMediaTrack
	{
		public int FormatId { get; }
		public string MimeType { get; }
		public string MimeExt { get; }
		public string MimeCodecs { get; }
		public int Bitrate { get; }
		public int AverageBitrate { get; }
		public string LastModified { get; }
		public long ContentLength { get; }
		public string Quality { get; }
		public string QualityLabel { get; }
		public int ApproxDurationMs { get; }
		public string ProjectionType { get; }
		public string FileUrl { get; }
		public string CipherSignatureEncrypted { get; }
		public string CipherEncryptedFileUrl { get; }
		public string FileExtension { get; }
		public bool IsDashManifest { get; }
		public bool IsHlsManifest { get; }
		public bool IsCiphered { get; }
		public string DashManifestUrl { get; }
		public YouTubeDashUrlList DashUrls { get; }
		public string HlsManifestUrl { get; }
		public YouTubeBroadcast Broadcast { get; }

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
			string dashManifestUrl,
			YouTubeDashUrlList dashUrls,
			string hlsManifestUrl,
			YouTubeBroadcast broadcast)
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
			DashManifestUrl = dashManifestUrl;
			DashUrls = dashUrls;
			HlsManifestUrl = hlsManifestUrl;
			Broadcast = broadcast;
		}

		public virtual string GetShortInfo()
		{
			return null;
		}
	}
}
