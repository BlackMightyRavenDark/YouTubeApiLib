﻿
namespace YouTubeApiLib
{
	public class YouTubeMediaTrackVideo : YouTubeMediaTrack
	{
		public int VideoWidth { get; }
		public int VideoHeight { get; }
		public int FrameRate { get; }

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
			bool isHlsManifest,
			bool isCiphered,
			string dashManifestUrl,
			YouTubeDashUrlList dashUrls,
			string hlsManifestUrl,
			YouTubeBroadcast broadcast)
			: base(formatId, bitrate, averageBitrate, lastModified, contentLength,
				  quality, qualityLabel, approxDurationMs, projectionType, fileUrl,
				  mimeType, mimeExt, mimeCodecs, fileExtension,
				  isDashManifest, isHlsManifest, isCiphered, dashManifestUrl, dashUrls, hlsManifestUrl, broadcast)
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
				  true, false, false, dashManifestUrl, dashUrls, null, null)
		{
			VideoWidth = videoWidth;
			VideoHeight = videoHeight;
			FrameRate = frameRate;
		}

		//Simplified constructor for HLS track (broadcast aka stream)
		public YouTubeMediaTrackVideo(YouTubeBroadcast broadcast, string hlsManifestUrl)
			: base(broadcast.FormatId, broadcast.Bandwidth, broadcast.Bandwidth, null, -1L,
				  null, null, -1, null, broadcast.PlaylistUrl,
				  "video/ts", "ts", broadcast.Codecs, "ts",
				  false, true, false, null, null, hlsManifestUrl, broadcast)
		{
			VideoWidth = broadcast.VideoWidth;
			VideoHeight = broadcast.VideoHeight;
			FrameRate = broadcast.FrameRate;
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
				  mimeType, mimeExt, mimeCodecs, fileExtension, false, false,
				  isCiphered, null, null, null, null)
		{
			VideoWidth = videoWidth;
			VideoHeight = videoHeight;
			FrameRate = frameRate;
		}
	}
}
