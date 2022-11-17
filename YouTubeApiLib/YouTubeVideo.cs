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
        public bool IsDashed { get; private set; }
        public string DashManifestUrl { get; private set; }
        public List<YouTubeVideoThumbnail> ThumbnailUrls { get; private set; }
        public LinkedList<YouTubeMediaTrack> MediaTracks { get; private set; }
        public RawVideoInfo RawInfo { get; private set; }
        public SimplifiedVideoInfo SimplifiedInfo { get; private set; }
        public YouTubeVideoPlayabilityStatus Status { get; private set; }

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

            YouTubeMediaTrack track = MediaTracks != null && MediaTracks.Count > 0 ? MediaTracks.First.Value : null;
            if (track != null)
            {
                IsDashed = track.IsDashManifest;
                DashManifestUrl = track.DashManifestUrl;
            }
            else
            {
                IsDashed = false;
                DashManifestUrl = null;
            }
        }

        public static YouTubeVideo CreateEmpty(YouTubeVideoPlayabilityStatus status)
        {
            return new YouTubeVideo(null, null, TimeSpan.FromSeconds(0), DateTime.MaxValue, DateTime.MaxValue,
                null, null, null, 0L, null, false, false, false, false, null, null, null, null, status);
        }
    }
}
