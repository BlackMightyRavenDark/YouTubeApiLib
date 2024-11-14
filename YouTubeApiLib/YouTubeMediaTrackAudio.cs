﻿
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
			YouTubeMediaTrackUrl fileUrl,
			string mimeType,
			string mimeExt,
			string mimeCodecs,
			string fileExtension,
			bool isDash,
			bool isCiphered,
			string dashManifestUrl,
			YouTubeDashUrlList dashUrls)
			: base(formatId, bitrate, averageBitrate, lastModified, contentLength,
				  quality, qualityLabel, approxDurationMs, null, fileUrl,
				  mimeType, mimeExt, mimeCodecs, fileExtension,
				  isDash, isCiphered, dashManifestUrl, dashUrls)
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
			YouTubeDashUrlList dashUrls)
			: base(formatId, bitrate, bitrate, null, -1L, null, null, -1, null, null,
				  mimeType, mimeExt, mimeCodecs, fileExtension,
				  true, false, dashManifestUrl, dashUrls)
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
			YouTubeMediaTrackUrl fileUrl,
			string mimeType,
			string mimeExt,
			string mimeCodecs,
			string fileExtension,
			bool isCiphered)
			: base(formatId, bitrate, averageBitrate, lastModified, contentLength,
				  quality, qualityLabel, approxDurationMs, null, fileUrl,
				  mimeType, mimeExt, mimeCodecs, fileExtension,
				  false, isCiphered, null, null)
		{
			AudioQuality = audioQuality;
			SampleRate = sampleRate;
			ChannelCount = channelCount;
			LoudnessDb = loudnessDb;
		}
	}
}
