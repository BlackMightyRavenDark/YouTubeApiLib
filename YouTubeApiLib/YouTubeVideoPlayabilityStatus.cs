using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
    public class YouTubeVideoPlayabilityStatus
    {
        public string Status { get; }
        public string Reason { get; }
        public string ThumbnailUrl { get; }
        public bool IsPlayable { get; }
        public bool IsPrivate => GetIsPrivate();
        public bool IsAdult => GetIsAdult();
        public int ErrorCode { get; }
        public string RawInfo { get; }

        public YouTubeVideoPlayabilityStatus(string status, string reason, string thumbnailUrl,
            int errorCode, string rawInfo)
        {
            Status = status;
            Reason = reason;
            ThumbnailUrl = thumbnailUrl;
            ErrorCode = errorCode;
            RawInfo = rawInfo;
            IsPlayable = status == "OK";
        }

        public static YouTubeVideoPlayabilityStatus Parse(JObject jPlayabilityStatus)
        {
            string status = jPlayabilityStatus.Value<string>("status");
            string reason = jPlayabilityStatus.Value<string>("reason");
            string thumbnailUrl = null;
            JObject jErrorScreen = jPlayabilityStatus.Value<JObject>("errorScreen");
            if (jErrorScreen != null)
            {
                JObject jPlayerErrorMessageRenderer = jErrorScreen.Value<JObject>("playerErrorMessageRenderer");
                if (string.IsNullOrEmpty(reason) || string.IsNullOrWhiteSpace(reason))
                {
                    JObject jReason = jPlayerErrorMessageRenderer.Value<JObject>("reason");
                    if (jReason != null)
                    {
                        reason = jReason.Value<string>("simpleText");
                        if (string.IsNullOrEmpty(reason) || string.IsNullOrWhiteSpace(reason))
                        {
                            reason = "<UNKNOWN>";
                        }
                    }
                }
                JObject jThumbnail = jPlayerErrorMessageRenderer.Value<JObject>("thumbnail");
                if (jThumbnail != null)
                {
                    JArray jaThumbnails = jThumbnail.Value<JArray>("thumbnails");
                    if (jaThumbnails != null && jaThumbnails.Count > 0)
                    {
                        thumbnailUrl = $"https:{(jaThumbnails[0] as JObject).Value<string>("url")}";
                    }
                }
            }
            int errorCode = status == "OK" ? 200 : 403;
            return new YouTubeVideoPlayabilityStatus(status, reason, thumbnailUrl, errorCode, jPlayabilityStatus.ToString());
        }

        private bool GetIsPrivate()
        {
            return !string.IsNullOrEmpty(Reason) && !string.IsNullOrWhiteSpace(Reason) &&
                Reason.ToLower().Contains("private");
        }

        private bool GetIsAdult()
        {
            return !string.IsNullOrEmpty(Reason) && !string.IsNullOrWhiteSpace(Reason) &&
                Reason.ToLower().Contains("age");
        }
    }
}
