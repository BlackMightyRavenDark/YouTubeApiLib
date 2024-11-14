﻿
namespace YouTubeApiLib
{
	public class YouTubeMediaTrackVideo : YouTubeMediaTrack
	{
		public int VideoWidth { get; protected set; }
		public int VideoHeight { get; protected set; }
		public int FrameRate { get; protected set; }

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
			YouTubeMediaTrackUrl fileUrl,
			string mimeType,
			string mimeExt,
			string mimeCodecs,
			string fileExtension,
			bool isDashManifest,
			bool isCiphered,
			string dashManifestUrl,
			YouTubeDashUrlList dashUrls)
			: base(formatId, bitrate, averageBitrate, lastModified, contentLength,
				  quality, qualityLabel, approxDurationMs, projectionType, fileUrl,
				  mimeType, mimeExt, mimeCodecs, fileExtension,
				  isDashManifest, isCiphered, dashManifestUrl, dashUrls)
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
			YouTubeDashUrlList dashUrls)
			: base(formatId, bitrate, bitrate, null, -1L,
				  null, null, -1, null, null,
				  mimeType, mimeExt, mimeCodecs, fileExtension,
				  true, false, dashManifestUrl, dashUrls)
		{
			VideoWidth = videoWidth;
			VideoHeight = videoHeight;
			FrameRate = frameRate;
		}

		//Simplified constructor for non-DASH and non-HLS video track
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
			YouTubeMediaTrackUrl fileUrl,
			string mimeType,
			string mimeExt,
			string mimeCodecs,
			string fileExtension,
			bool isCiphered)
			: base(formatId, bitrate, averageBitrate, lastModified, contentLength,
				  quality, qualityLabel, approxDurationMs, projectionType, fileUrl,
				  mimeType, mimeExt, mimeCodecs, fileExtension, false,
				  isCiphered, null, null)
		{
			VideoWidth = videoWidth;
			VideoHeight = videoHeight;
			FrameRate = frameRate;
		}
	}
}
