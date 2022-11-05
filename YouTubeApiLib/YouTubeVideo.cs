using System;
using Newtonsoft.Json.Linq;

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
        public JObject RawInfo { get; private set; }
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
            JObject rawInfo,
            YouTubeVideoPlayabilityStatus status)
        {
            Title = title;
            Id = id;
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrWhiteSpace(id))
            {
                Url = $"https://www.youtube.com/watch?v={id}";
            }
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
            RawInfo = rawInfo;
            Status = status;
        }

        public static YouTubeVideo CreateEmpty(YouTubeVideoPlayabilityStatus status)
        {
            return new YouTubeVideo(null, null, TimeSpan.FromSeconds(0), DateTime.MaxValue, DateTime.MaxValue,
                null, null, null, 0L, null, false, false, false, false, null, status);
        }
    }
}
