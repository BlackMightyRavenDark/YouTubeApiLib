using System;
using System.Collections.Generic;

namespace YouTubeApiLib
{
	public class YouTubeVideo
	{
		public string Title { get; }
		public string Id { get; }
		public string Url { get; }
		public DateTime DateUploaded { get; }
		public DateTime DatePublished { get; }
		public TimeSpan Length { get; }
		public string OwnerChannelTitle { get; }
		public string OwnerChannelId { get; }
		public string Description { get; }
		public long ViewCount { get; }
		public string Category { get; }
		public bool IsPrivate { get; }
		public bool IsUnlisted { get; }
		public bool IsFamilySafe { get; }
		public bool IsLiveContent { get; }

		/// <summary>
		/// Warning! This value can be always TRUE for some videos!
		/// E.G. when the broadcast is finished, but not yet fully processed by YouTube.
		/// This is a YouTube API bug.
		/// </summary>
		public bool IsLiveNow { get; }

		public bool IsDashed { get; }
		public string DashManifestUrl { get; }
		public string HlsManifestUrl { get; }
		public List<YouTubeVideoThumbnail> ThumbnailUrls { get; }
		public LinkedList<YouTubeMediaTrack> MediaTracks { get; private set; }
		public YouTubeRawVideoInfo RawInfo { get; private set; }
		public YouTubeSimplifiedVideoInfo SimplifiedInfo { get; }
		public YouTubeVideoPlayabilityStatus Status { get; }
		public bool IsInfoAvailable => GetIsInfoAvailable();

		public YouTubeVideo(
			string title,
			string id,
			TimeSpan length,
			DateTime dateUploaded,
			DateTime datePublished,
			string ownerChannelTitle,
			string ownerChannelId,
			string description,
			long viewCount,
			string category,
			bool isPrivate,
			bool isUnlisted,
			bool isFamilySafe,
			bool isLiveContent,
			List<YouTubeVideoThumbnail> thumbnailUrls,
			LinkedList<YouTubeMediaTrack> mediaTracks,
			YouTubeRawVideoInfo rawInfo,
			YouTubeSimplifiedVideoInfo simplifiedInfo,
			YouTubeVideoPlayabilityStatus status)
		{
			Title = title;
			Id = id;
			Url = !string.IsNullOrEmpty(id) && !string.IsNullOrWhiteSpace(id) ?
				Utils.GetVideoUrl(id) : null;
			Length = length;
			DateUploaded = dateUploaded;
			DatePublished = datePublished;
			OwnerChannelTitle = ownerChannelTitle;
			OwnerChannelId = ownerChannelId;
			Description = description;
			ViewCount = viewCount;
			Category = category;
			IsPrivate = isPrivate;
			IsUnlisted = isUnlisted;
			IsFamilySafe = isFamilySafe;
			IsLiveContent = isLiveContent;
			ThumbnailUrls = thumbnailUrls;
			MediaTracks = mediaTracks;
			RawInfo = rawInfo;
			SimplifiedInfo = simplifiedInfo;
			Status = status;

			if (MediaTracks != null)
			{
				foreach (YouTubeMediaTrack track in MediaTracks)
				{
					if (!IsDashed)
					{
						IsDashed = track.IsDashManifest;
						DashManifestUrl = track.DashManifestUrl;
					}
					if (!IsLiveNow && track.Broadcast != null)
					{
						HlsManifestUrl = track.HlsManifestUrl;
						IsLiveNow = true;
					}
					if (IsDashed && IsLiveNow)
					{
						break;
					}
				}
			}
			else
			{
				YouTubeStreamingData streamingData = rawInfo?.StreamingData;
				if (streamingData != null)
				{
					DashManifestUrl = streamingData.GetDashManifestUrl();
					IsDashed = !string.IsNullOrEmpty(DashManifestUrl) && !string.IsNullOrWhiteSpace(DashManifestUrl);
					HlsManifestUrl = streamingData.GetHlsManifestUrl();
					IsLiveNow = !string.IsNullOrEmpty(HlsManifestUrl) && !string.IsNullOrWhiteSpace(HlsManifestUrl);
				}
				else
				{
					DashManifestUrl = null;
					IsDashed = false;
					HlsManifestUrl = null;
					IsLiveNow = false;
				}
			}
		}

		public static YouTubeVideo CreateEmpty(YouTubeVideoPlayabilityStatus status)
		{
			return new YouTubeVideo(null, null, TimeSpan.FromSeconds(0), DateTime.MaxValue, DateTime.MaxValue,
				null, null, null, 0L, null, false, false, false, false, null, null, null, null, status);
		}

		/// <summary>
		/// Reparse the downloadable formats info.
		/// Warming!!! You will lost the current media track list!
		/// </summary>
		public void UpdateMediaFormats(YouTubeRawVideoInfo rawVideoInfo)
		{
			MediaTracks = Utils.ParseMediaTracks(rawVideoInfo);
		}

		/// <summary>
		/// Redownload and reparse the downloadable formats info.
		/// Warming!!! You will lost the current media track list!
		/// </summary>
		/// <returns>HTTP error code.</returns>
		public int UpdateMediaFormats(Utils.YouTubeVideoInfoGettingMethod method)
		{
			MediaTracks = null;
			YouTubeRawVideoInfoResult rawVideoInfoResult = Utils.GetRawVideoInfo(Id, method);
			if (rawVideoInfoResult != null)
			{
				RawInfo = rawVideoInfoResult.RawVideoInfo;
				if (rawVideoInfoResult.ErrorCode == 200)
				{
					UpdateMediaFormats(rawVideoInfoResult.RawVideoInfo);
					return 200;
				}
				return rawVideoInfoResult.ErrorCode;
			}
			return 404;
		}

		/// <summary>
		/// Redownload and reparse the downloadable formats info.
		/// Warming!!! You will lost the current media track list!
		/// </summary>
		/// <returns>HTTP error code.</returns>
		public int UpdateMediaFormats()
		{
			Utils.YouTubeVideoInfoGettingMethod method =
				RawInfo != null ? RawInfo.DataGettingMethod : Utils.YouTubeVideoInfoGettingMethod.HiddenApiEncryptedUrls;
			return UpdateMediaFormats(method);
		}

		private bool GetIsInfoAvailable()
		{
			bool status = Status == null || (Status != null && !Status.IsPrivate);
			return status || RawInfo?.VideoDetails != null &&
				(SimplifiedInfo.IsVideoInfoAvailable || SimplifiedInfo.IsMicroformatInfoAvailable);
		}
	}
}
