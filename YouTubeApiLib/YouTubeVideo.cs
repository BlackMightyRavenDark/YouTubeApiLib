using System;
using System.Collections.Generic;

namespace YouTubeApiLib
{
    public class YouTubeVideo
    {
        public string Title { get; private set; }
        public string Id { get; private set; }
        public string Url { get; private set; }
        public DateTime DateUploaded { get; private set; }
        public DateTime DatePublished { get; private set; }
        public TimeSpan Length { get; private set; }
        public string OwnerChannelTitle { get; private set; }
        public string OwnerChannelId { get; private set; }
        public string Description { get; private set; }
        public long ViewCount { get; private set; }
        public string Category { get; private set; }
        public bool IsPrivate { get; private set; }
        public bool IsUnlisted { get; private set; }
        public bool IsFamilySafe { get; private set; }
        public bool IsLiveContent { get; private set; }

        /// <summary>
        /// Warning! This value can be always TRUE for some videos!
        /// E.G. when the broadcast is finished, but not yet fully processed by YouTube.
        /// This is a YouTube API bug.
        /// </summary>
        public bool IsLiveNow { get; private set; }

        public bool IsDashed { get; private set; }
        public string DashManifestUrl { get; private set; }
        public string HlsManifestUrl { get; private set; }
        public List<YouTubeVideoThumbnail> ThumbnailUrls { get; private set; }
        public LinkedList<YouTubeMediaTrack> MediaTracks { get; private set; }
        public RawVideoInfo RawInfo { get; private set; }
        public SimplifiedVideoInfo SimplifiedInfo { get; private set; }
        public YouTubeVideoPlayabilityStatus Status { get; private set; }
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
            RawVideoInfo rawInfo,
            SimplifiedVideoInfo simplifiedInfo,
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
                StreamingData streamingData = rawInfo?.StreamingData;
                if (streamingData != null)
                {
                    DashManifestUrl = streamingData.RawData.Value<string>("dashManifestUrl");
                    IsDashed = !string.IsNullOrEmpty(DashManifestUrl) && !string.IsNullOrWhiteSpace(DashManifestUrl);
                    HlsManifestUrl = streamingData.RawData.Value<string>("hlsManifestUrl");
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
        public void UpdateMediaFormats(RawVideoInfo rawVideoInfo)
        {
            MediaTracks = Utils.ParseMediaTracks(rawVideoInfo);
        }

        /// <summary>
        /// Redownload and reparse the downloadable formats info.
        /// Warming!!! You will lost the current media track list!
        /// </summary>
        /// <returns>HTTP error code.</returns>
        public int UpdateMediaFormats(Utils.VideoInfoGettingMethod method)
        {
            MediaTracks = null;
            RawVideoInfoResult rawVideoInfoResult = Utils.GetRawVideoInfo(Id, method);
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
            Utils.VideoInfoGettingMethod method =
                RawInfo != null ? RawInfo.DataGettingMethod : Utils.VideoInfoGettingMethod.HiddenApiEncryptedUrls;
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
