using System;
using Newtonsoft.Json.Linq;
using static YouTube_API.Utils;

namespace YouTube_API
{
    public sealed class YouTubeApi
    {
        public const string YOUTUBE_URL = "https://www.youtube.com";
        public const string API_V1_KEY = "AIzaSyAO_FJ2SlqU8Q4STEHLGCilw_Y9_11qcW8";
        public const string API_V1_BROWSE_URL = "https://www.youtube.com/youtubei/v1/browse";
        public const string API_V1_PLAYER_URL = "https://www.youtube.com/youtubei/v1/player";
        public const string YOUTUBE_CLIENT_VERSION = "2.20211221.00.00";
        public const string TAB_HOME_PARAMS_ID = "EghmZWF0dXJlZA%3D%3D";
        public const string TAB_VIDEOS_ASCENDING_PARAMS_ID = "EgZ2aWRlb3MYAiAAMAE%3D";
        public const string TAB_VIDEOS_DESCENDING_PARAMS_ID = "EgZ2aWRlb3MYAyAAMAE%3D";
        public const string TAB_VIDEOS_POPULARITY_PARAMS_ID = "EgZ2aWRlb3MYASAAMAE%3D";
        public const string TAB_PLAYLISTS_DESCENDING_PARAMS_ID = "EglwbGF5bGlzdHMYAyABMAE%3D";
        public const string TAB_PLAYLISTS_LAST_VIDEO_ADDED_PARAMS_ID = "EglwbGF5bGlzdHMYBCABMAE%3D";
        public const string TAB_COMMUNITY_PARAMS_ID = "Egljb21tdW5pdHk%3D";
        public const string TAB_CHANNELS_PARAMS_ID = "EghjaGFubmVscw%3D%3D";
        public const string TAB_ABOUT_PARAMS_ID = "EgVhYm91dA%3D%3D";

        public static string GetBrowseEndpointParams(ChannelTab channelTab, SortingOrder itemsSortingOrder)
        {
            switch (channelTab)
            {
                case ChannelTab.Home:
                    return TAB_HOME_PARAMS_ID;

                case ChannelTab.Videos:
                    switch (itemsSortingOrder)
                    {
                        case SortingOrder.Ascending:
                            return TAB_VIDEOS_ASCENDING_PARAMS_ID;

                        case SortingOrder.Descending:
                            return TAB_VIDEOS_DESCENDING_PARAMS_ID;

                        case SortingOrder.Popularity:
                            return TAB_VIDEOS_POPULARITY_PARAMS_ID;
                    }
                    break;

                case ChannelTab.Playlists:
                    switch (itemsSortingOrder)
                    {
                        case SortingOrder.Descending:
                            return TAB_PLAYLISTS_DESCENDING_PARAMS_ID;

                        default:
                            return TAB_PLAYLISTS_LAST_VIDEO_ADDED_PARAMS_ID;
                    }

                case ChannelTab.Community:
                    return TAB_COMMUNITY_PARAMS_ID;

                case ChannelTab.Channels:
                    return TAB_CHANNELS_PARAMS_ID;

                case ChannelTab.About:
                    return TAB_ABOUT_PARAMS_ID;
            }

            return null;
        }

        public YouTubeChannelTabResult GetChannelTab(string channelId, ChannelTab channelTab,
            SortingOrder itemsSortingOrder = SortingOrder.Descending)
        {
            string browseParams = GetBrowseEndpointParams(channelTab, itemsSortingOrder);
            return GetChannelTab(channelId, browseParams);
        }

        public YouTubeChannelTabResult GetChannelTab(string channelId, string browseEndpointParams)
        {
            string url = $"{API_V1_BROWSE_URL}?key={API_V1_KEY}";
            JObject body = GenerateChannelTabRequestBody(channelId, browseEndpointParams, null);
            int errorCode = Utils.HttpsPost(url, body.ToString(), out string response);
            if (errorCode == 200)
            {
                JObject json = JObject.Parse(response);
                JToken jt = json.Value<JToken>("contents");
                if (jt == null)
                {
                    return new YouTubeChannelTabResult(null, 404);
                }
                jt = jt.Value<JObject>().Value<JObject>("twoColumnBrowseResultsRenderer").Value<JToken>("tabs");
                if (jt == null)
                {
                    return new YouTubeChannelTabResult(null, 404);
                }

                JArray jTabs = jt.Value<JArray>();
                JObject jSelectedTab = FindSelectedTab(jTabs);
                if (jSelectedTab != null)
                {
                    string tabTitle = jSelectedTab.Value<string>("title");
                    return new YouTubeChannelTabResult(new YouTubeChannelTab(tabTitle, jSelectedTab), errorCode);
                }
            }
            return new YouTubeChannelTabResult(null, errorCode);
        }

        public static JObject FindSelectedTab(JArray jTabs)
        {
            foreach (JObject jObject in jTabs)
            {
                JToken jt = jObject.Value<JToken>("tabRenderer");
                if (jt == null)
                {
                    jt = jObject.Value<JToken>("expandableTabRenderer");
                }

                JObject j = jt.Value<JObject>();
                bool selected = j.Value<bool>("selected");
                if (selected)
                {
                    return j;
                }
            }
            return null;
        }

        public VideoPageResult GetVideosPage(string channelId, string pageToken, SortingOrder sortingOrder)
        {
            bool tokenExists = !string.IsNullOrEmpty(pageToken) && !string.IsNullOrWhiteSpace(pageToken);
            string browseParams = !tokenExists ? GetBrowseEndpointParams(ChannelTab.Videos, sortingOrder) : null;
            JObject body = GenerateChannelTabRequestBody(channelId, browseParams, pageToken);
            string url = $"{API_V1_BROWSE_URL}?key={API_V1_KEY}";
            int errorCode = Utils.HttpsPost(url, body.ToString(), out string response);
            if (errorCode == 200)
            {
                JObject json = JObject.Parse(response);
                JArray jVideos;
                if (!tokenExists)
                {
                    JObject jTabVideos = FindVideosTab(json);
                    if (jTabVideos == null)
                    {
                        return new VideoPageResult(null, null, 404);
                    }
                    jVideos = FindItemsArray(jTabVideos, false);
                }
                else
                {
                    jVideos = FindItemsArray(json, true);
                }
                if (jVideos == null || jVideos.Count == 0)
                {
                    return new VideoPageResult(null, null, 404);
                }

                JArray jParsedArray = ParseGridRendererItems(jVideos, out pageToken);
                if (jParsedArray == null || jParsedArray.Count == 0)
                {
                    return new VideoPageResult(null, null, 404);
                }
                return new VideoPageResult(jParsedArray, pageToken, errorCode);
            }
            return new VideoPageResult(null, null, errorCode);
        }

        public JObject GenerateChannelVideoListRequestBody(string channelId,
            SortingOrder videosSortingOrder, string continuationToken)
        {
            string browseParams = GetBrowseEndpointParams(ChannelTab.Videos, videosSortingOrder);
            return GenerateChannelTabRequestBody(channelId, browseParams, continuationToken);
        }

        public JObject GenerateChannelTabRequestBody(string channelId,
            string browseEndpointParams, string continuationToken)
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
                if (!string.IsNullOrEmpty(browseEndpointParams) && !string.IsNullOrWhiteSpace(browseEndpointParams))
                {
                    json["params"] = browseEndpointParams;
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
            while (true)
            {
                VideoPageResult videoPageResult = GetVideosPage(channelId, continuationToken, sortingOrder);
                errorCode = videoPageResult.ErrorCode;
                if (errorCode != 200)
                {
                    break;
                }

                foreach (JObject j in videoPageResult.List)
                {
                    resList.Add(j);
                }

                continuationToken = videoPageResult.ContinuationToken;
                bool continuationTokenExists = !string.IsNullOrEmpty(continuationToken) && !string.IsNullOrEmpty(continuationToken);
                if (!continuationTokenExists)
                {
                    break;
                }
            
                System.Diagnostics.Debug.WriteLine(continuationToken);
            }

            return new VideoListResult(resList, resList.Count > 0 ? 200 : errorCode);
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
                    if (GetSimplifiedVideoInfo(videoId, out JObject jVideoInfo) == 200)
                    {
                        simplifiedList.Add(jVideoInfo);
                    }
                }
                else
                {
                    jt = jObject.Value<JToken>("continuationItemRenderer");
                    if (jt != null)
                    {
                        JObject jContinuationItemRenderer = jt.Value<JObject>();
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

        public YouTubeVideo GetVideo(string videoId)
        {
            int errorCode = GetVideoInfo(videoId, out string info);
            if (errorCode == 200)
            {
                JObject json = JObject.Parse(info);
                JObject jVideoDetails = json.Value<JObject>("videoDetails");
                if (jVideoDetails != null)
                {
                    string videoTitle = jVideoDetails.Value<string>("title");
                    int lengthSeconds = int.Parse(jVideoDetails.Value<string>("lengthSeconds"));
                    TimeSpan length = TimeSpan.FromSeconds(lengthSeconds);
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
                    string published = jMicroformatRenderer.Value<string>("publishDate");
                    string uploaded = jMicroformatRenderer.Value<string>("uploadDate");
                    StringToDateTime(published, out DateTime datePublished);
                    StringToDateTime(uploaded, out DateTime dateUploaded);

                    YouTubeVideo youTubeVideo = new YouTubeVideo(
                        videoTitle, videoId, length, dateUploaded, datePublished, ownerChannelTitle, ownerChannelId,
                        description, viewCount, category, isPrivate, isUnlisted, isFamilySafe, isLiveContent, json, errorCode);
                    return youTubeVideo;
                }
                else
                {
                    errorCode = 400;
                }
            }
            return YouTubeVideo.CreateEmpty(errorCode);
        }

        public int GetSimplifiedVideoInfo(string videoId, out JObject simplifiedVideoInfo)
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

                simplifiedVideoInfo = new JObject();
                simplifiedVideoInfo["title"] = videoTitle;
                simplifiedVideoInfo["id"] = videoId;
                simplifiedVideoInfo["url"] = $"{YOUTUBE_URL}/watch?v={videoId}";
                simplifiedVideoInfo["lengthSeconds"] = lengthSeconds;
                simplifiedVideoInfo["ownerChannelTitle"] = ownerChannelTitle;
                simplifiedVideoInfo["ownerChannelId"] = ownerChannelId;
                simplifiedVideoInfo["viewCount"] = viewCount;
                simplifiedVideoInfo["category"] = category;
                simplifiedVideoInfo["isPrivate"] = isPrivate;
                simplifiedVideoInfo["isUnlisted"] = isUnlisted;
                simplifiedVideoInfo["isFamilySafe"] = isFamilySafe;
                simplifiedVideoInfo["isLiveContent"] = isLiveContent;
                simplifiedVideoInfo["datePublished"] = datePublished;
                simplifiedVideoInfo["dateUploaded"] = dateUploaded;
                simplifiedVideoInfo["description"] = description;
                simplifiedVideoInfo["shortDescription"] = shortDescription;
                simplifiedVideoInfo.Add(new JProperty("thumbnails", jThumbnails));
                jt = json.Value<JToken>("streamingData");
                if (jt != null)
                {
                    simplifiedVideoInfo.Add(new JProperty("streamingData", jt));
                }
            }
            else
            {
                simplifiedVideoInfo = null;
            }
            return errorCode;
        }
    }

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
        public int ErrorCode { get; private set; }

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
            int errorCode)
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
            ErrorCode = errorCode;
        }

        public static YouTubeVideo CreateEmpty(int errorCode)
        {
            return new YouTubeVideo(null, null, TimeSpan.FromSeconds(0), DateTime.MaxValue, DateTime.MaxValue,
                null, null, null, 0L, null, false, false, false, false, null, errorCode);
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

    public class YouTubeChannelTab
    {
        public string Title { get; private set; }
        public JObject Json { get; private set; }

        public YouTubeChannelTab(string title, JObject json)
        {
            Title = title;
            Json = json;
        }
    }

    public sealed class YouTubeChannelTabResult
    {
        public YouTubeChannelTab Tab { get; private set; }
        public int ErrorCode { get; private set; }

        public YouTubeChannelTabResult(YouTubeChannelTab channelTab, int errorCode)
        {
            Tab = channelTab;
            ErrorCode = errorCode;
        }
    }

    public class VideoListResult
    {
        public JArray List { get; private set; }
        public int ErrorCode { get; private set; }

        public VideoListResult(JArray list, int errorCode)
        {
            List = list;
            ErrorCode = errorCode;
        }
    }

    public sealed class VideoPageResult : VideoListResult
    {
        public string ContinuationToken { get; private set; }

        public VideoPageResult(JArray videoList, string continuationToken, int errorCode)
            : base(videoList, errorCode)
        {
            ContinuationToken = continuationToken;
        }
    }

    public enum ChannelTab { Home, Videos, Playlists, Community, Channels, About }

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
