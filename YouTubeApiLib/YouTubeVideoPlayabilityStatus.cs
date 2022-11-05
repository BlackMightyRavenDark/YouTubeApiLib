using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
    public class YouTubeVideoPlayabilityStatus
    {
        public string Status { get; private set; }
        public string Reason { get; private set; }
        public bool IsPlayable { get; private set; }
        public int ErrorCode { get; private set; }
        public JObject RawInfo { get; private set; }

        public YouTubeVideoPlayabilityStatus(string status, string reason, int errorCode, JObject rawInfo)
        {
            Status = status;
            Reason = reason;
            IsPlayable = status == "OK";
            ErrorCode = errorCode;
            RawInfo = rawInfo;
        }

        public static YouTubeVideoPlayabilityStatus Parse(JObject jPlayabilityStatus)
        {
            string status = jPlayabilityStatus.Value<string>("status");
            int errorCode = status == "OK" ? 200 : 403;
            string reason = errorCode != 200 ? GetReason(jPlayabilityStatus) : null;
            return new YouTubeVideoPlayabilityStatus(status, reason, errorCode, jPlayabilityStatus);
        }

        private static string GetReason(JObject jPlayabilityStatus)
        {
            JToken jt = jPlayabilityStatus.Value<JToken>("reason");
            if (jt != null)
            {
                return jt.Value<string>();
            }
            jt = jPlayabilityStatus.Value<JToken>("errorScreen");
            if (jt != null)
            {
                jt = jt.Value<JObject>().Value<JToken>("playerErrorMessageRenderer");
                if (jt != null)
                {
                    JObject jReason = jt.Value<JObject>().Value<JObject>("reason");
                    return jReason != null ? jReason.Value<string>("simpleText") : string.Empty;
                }
            }
            return string.Empty;
        }
    }
}
