
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
		public YouTubeMediaTrackUrl FileUrl { get; }
		public string FileExtension { get; }
		public bool IsDashManifestPresent { get; }
		public bool IsHlsManifestPresent { get; }
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
			YouTubeMediaTrackUrl fileUrl,
			string mimeType,
			string mimeExt,
			string mimeCodecs,
			string fileExtension,
			bool isDashManifestPresent,
			bool isHlsManifestPresent,
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
			MimeType = mimeType;
			MimeExt = mimeExt;
			MimeCodecs = mimeCodecs;
			FileExtension = fileExtension;
			IsDashManifestPresent = isDashManifestPresent;
			IsHlsManifestPresent = isHlsManifestPresent;
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
