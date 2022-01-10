using System;
using Newtonsoft.Json.Linq;

namespace YouTube_API
{
    public sealed class YouTubeApi
    {
        public const string YOUTUBE_URL = "https://www.youtube.com";
        public const string API_V1_KEY = "AIzaSyAO_FJ2SlqU8Q4STEHLGCilw_Y9_11qcW8";
        public const string API_V1_BROWSE_URL = "https://www.youtube.com/youtubei/v1/browse";
        public const string API_V1_PLAYER_URL = "https://www.youtube.com/youtubei/v1/player";
        public const string YOUTUBE_CLIENT_VERSION = "2.20211221.00.00";

        public JObject GenerateChannelVideoListRequestBody(string channelId, 
            SortingOrder sortingOrder, string continuationToken)
        {
            JObject jClient = new JObject();
            jClient["hl"] = "en";
            jClient["gl"] = "US";
            jClient["clientName"] = "WEB";
            jClient["clientVersion"] = YOUTUBE_CLIENT_VERSION;

            JObject jContext = new JObject();
            jContext.Add(new JProperty("client", jClient));

            JObject json = new JObject();
            json.Add(new JProperty("context", jContext));
            json["browseId"] = channelId;
            if (string.IsNullOrEmpty(continuationToken) || string.IsNullOrWhiteSpace(continuationToken))
            {
                switch (sortingOrder)
                {
                    case SortingOrder.Ascending:
                        json["params"] = "EgZ2aWRlb3MYAiAAMAE%3D";
                        break;

                    case SortingOrder.Descending:
                        json["params"] = "EgZ2aWRlb3MYAyAAMAE%3D";
                        break;

                    case SortingOrder.Popularity:
                        json["params"] = "EgZ2aWRlb3MYASAAMAE%3D";
                        break;
                }
            }
            else
            {
                json["continuation"] = continuationToken;
            }
            return json;
        }

        public JObject GenerateVideoInfoRequestBody(string videoId)
        {
            JObject jClient = new JObject();
            jClient["hl"] = "en";
            jClient["gl"] = "US";
            jClient["clientName"] = "WEB";
            jClient["clientVersion"] = YOUTUBE_CLIENT_VERSION;

            JObject jContext = new JObject();
            jContext.Add(new JProperty("client", jClient));

            JObject json = new JObject();
            json.Add(new JProperty("context", jContext));
            json["videoId"] = videoId;

            return json;
        }

        public VideoListResult GetChannelVideoList(YouTubeChannel channel, SortingOrder sortingOrder)
        {
            return GetChannelVideoList(channel.Id, sortingOrder);
        }

        public VideoListResult GetChannelVideoList(string channelId, SortingOrder sortingOrder)
        {
            JArray resList = new JArray();
            string continuationToken = null;
            int errorCode;
            string req = $"{API_V1_BROWSE_URL}?key={API_V1_KEY}";
            do
            {
                JObject body = GenerateChannelVideoListRequestBody(channelId, sortingOrder, continuationToken);
                errorCode = Utils.HttpsPost(req, body.ToString(), out string response);
                if (errorCode == 200)
                {
                    JObject json = JObject.Parse(response);
                    JArray jVideos;
                    bool tokenExists = !string.IsNullOrEmpty(continuationToken);
                    if (tokenExists)
                    {
                        jVideos = FindItemsArray(json, true);
                    }
                    else
                    {
                        JObject jTabVideos = FindVideosTab(json);
                        if (jTabVideos == null)
                        {
                            return new VideoListResult(resList, errorCode);
                        }
                        jVideos = FindItemsArray(jTabVideos, false);
                    }
                    if (jVideos == null)
                    {
                        return new VideoListResult(resList, errorCode);
                    }

                    JArray ja = ParseGridRendererItems(jVideos, out continuationToken);
                    foreach (JObject jo in ja)
                    {
                        resList.Add(jo);
                    }

                    if (!string.IsNullOrEmpty(continuationToken))
                    {
                        System.Diagnostics.Debug.WriteLine($"token = {continuationToken}");
                    }
                }
            }
            while (errorCode == 200 && !string.IsNullOrEmpty(continuationToken));
            
            return new VideoListResult(resList, errorCode);
        }

        private JObject FindVideosTab(JObject root)
        {
            JToken jt = root.Value<JToken>("contents");
            if (jt == null)
            {
                return null;
            }
            JObject jObject = jt.Value<JObject>().Value<JObject>("twoColumnBrowseResultsRenderer");
            JArray tabs = jObject.Value<JArray>("tabs");
            foreach (JObject jTab in tabs)
            {
                jt = jTab.Value<JToken>("tabRenderer");
                if (jt == null)
                {
                    jt = jTab.Value<JToken>("expandableTabRenderer");
                }
                if (jt != null)
                {
                    string tabTitle = jt.Value<JObject>().Value<string>("title");
                    if (tabTitle == "Videos")
                    {
                        return jTab;
                    }
                }
            }
            return null;
        }

        private JArray FindItemsArray(JObject json, bool token)
        {
            try
            {
                if (token)
                {
                    return json.Value<JArray>("onResponseReceivedActions")[0]
                        .Value<JObject>("appendContinuationItemsAction")
                        .Value<JArray>("continuationItems");
                }
                else
                {
                    return json.Value<JObject>("tabRenderer")
                        .Value<JObject>("content")
                        .Value<JObject>("sectionListRenderer")
                        .Value<JArray>("contents")[0]
                        .Value<JObject>("itemSectionRenderer")
                        .Value<JArray>("contents")[0]
                        .Value<JObject>("gridRenderer")
                        .Value<JArray>("items");

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                return null;
            }
        }

        private JArray ParseGridRendererItems(JArray items, out string continuationToken)
        {
            continuationToken = null;
            JArray simplifiedList = new JArray();
            foreach (JObject jObject in items)
            {
                JToken jt = jObject.Value<JToken>("gridVideoRenderer");
                if (jt != null)
                {
                    JObject jGridVideoRenderer = jt.Value<JObject>();
                    string videoId = jGridVideoRenderer.Value<string>("videoId");
                    if (GetSimplifyedVideoInfo(videoId, out JObject jVideoInfo) == 200)
                    {
                        simplifiedList.Add(jVideoInfo);
                    }
                }
                else
                {
                    JToken jt0 = jObject.Value<JToken>("continuationItemRenderer");
                    if (jt0 != null)
                    {
                        JObject jContinuationItemRenderer = jt0.Value<JObject>();
                        continuationToken = jContinuationItemRenderer.Value<JObject>("continuationEndpoint").Value<JObject>("continuationCommand").Value<string>("token");
                    }
                }
            }
            return simplifiedList;
        }

        public int GetVideoInfo(string videoId, out string infoString)
        {
            JObject body = GenerateVideoInfoRequestBody(videoId);
            string url = $"{API_V1_PLAYER_URL}?key={API_V1_KEY}";
            int errorCode = Utils.HttpsPost(url, body.ToString(), out infoString);
            return errorCode;
        }

        public int GetSimplifyedVideoInfo(string videoId, out JObject simplifyedVideoInfo)
        {
            int errorCode = GetVideoInfo(videoId, out string info);
            if (errorCode == 200)
            {
                JObject json = JObject.Parse(info);
                JObject jVideoDetails = json.Value<JObject>("videoDetails");
                JArray jThumbnails = jVideoDetails.Value<JObject>("thumbnail").Value<JArray>("thumbnails");
                string videoTitle = jVideoDetails.Value<string>("title");
                string shortDescription = jVideoDetails.Value<string>("shortDescription");
                int lengthSeconds = int.Parse(jVideoDetails.Value<string>("lengthSeconds"));
                string ownerChannelTitle = jVideoDetails.Value<string>("author");
                string ownerChannelId = jVideoDetails.Value<string>("channelId");
                int viewCount = int.Parse(jVideoDetails.Value<string>("viewCount"));
                bool isPrivate = jVideoDetails.Value<bool>("isPrivate");
                bool isLiveContent = jVideoDetails.Value<bool>("isLiveContent");
                JObject jMicroformatRenderer = json.Value<JObject>("microformat").Value<JObject>("playerMicroformatRenderer");
                string description = null;
                JToken jt = jMicroformatRenderer.Value<JToken>("description");
                if (jt != null)
                {
                    jt = jt.Value<JToken>("simpleText");
                    if (jt != null)
                    {
                        description = jt.Value<string>();
                    }
                }
                bool isFamilySafe = jMicroformatRenderer.Value<bool>("isFamilySafe");
                bool isUnlisted = jMicroformatRenderer.Value<bool>("isUnlisted");
                string category = jMicroformatRenderer.Value<string>("category");
                string datePublished = jMicroformatRenderer.Value<string>("publishDate");
                string dateUploaded = jMicroformatRenderer.Value<string>("uploadDate");

                simplifyedVideoInfo = new JObject();
                simplifyedVideoInfo["title"] = videoTitle;
                simplifyedVideoInfo["id"] = videoId;
                simplifyedVideoInfo["url"] = $"{YOUTUBE_URL}/watch?v={videoId}";
                simplifyedVideoInfo["lengthSeconds"] = lengthSeconds;
                simplifyedVideoInfo["ownerChannelTitle"] = ownerChannelTitle;
                simplifyedVideoInfo["ownerChannelId"] = ownerChannelId;
                simplifyedVideoInfo["viewCount"] = viewCount;
                simplifyedVideoInfo["category"] = category;
                simplifyedVideoInfo["isPrivate"] = isPrivate;
                simplifyedVideoInfo["isUnlisted"] = isUnlisted;
                simplifyedVideoInfo["isFamilySafe"] = isFamilySafe;
                simplifyedVideoInfo["isLiveContent"] = isLiveContent;
                simplifyedVideoInfo["datePublished"] = datePublished;
                simplifyedVideoInfo["dateUploaded"] = dateUploaded;
                simplifyedVideoInfo["description"] = description;
                simplifyedVideoInfo["shortDescription"] = shortDescription;
                simplifyedVideoInfo.Add(new JProperty("thumbnails", jThumbnails));
                jt = json.Value<JToken>("streamingData");
                if (jt != null)
                {
                    simplifyedVideoInfo.Add(new JProperty("streamingData", jt));
                }
            }
            else
            {
                simplifyedVideoInfo = null;
            }
            return errorCode;
        }
    }

    public sealed class YouTubeChannel
    {
        public string DisplayName { get; private set; }
        public string Id { get; private set; }

        public YouTubeChannel(string id, string displayName)
        {
            Id = id;
            DisplayName = displayName;
        }
    }

    public sealed class VideoListResult
    {
        public JArray List { get; private set; }
        public int ErrorCode { get; private set; }

        public VideoListResult(JArray list, int errorCode)
        {
            List = list;
            ErrorCode = errorCode;
        }
    }

    public enum SortingOrder 
    {
        /// <summary>
        /// Сортировка по дате. Старые видео сверху.
        /// </summary>
        Ascending,

        /// <summary>
        /// Сортировка по дате. Новые видео сверху.
        /// </summary>
        Descending,

        /// <summary>
        /// Сортировка по популярности.
        /// </summary>
        Popularity
    }
}
