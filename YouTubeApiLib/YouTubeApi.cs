using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using static YouTubeApiLib.Utils;

namespace YouTubeApiLib
{
    public sealed class YouTubeApi
    {
        public const string YOUTUBE_URL = "https://www.youtube.com";
        public const string API_V1_KEY = "AIzaSyAO_FJ2SlqU8Q4STEHLGCilw_Y9_11qcW8";
        public const string API_V1_BROWSE_URL = "https://www.youtube.com/youtubei/v1/browse";
        public const string API_V1_PLAYER_URL = "https://www.youtube.com/youtubei/v1/player";
        public const string YOUTUBE_CLIENT_VERSION = "2.20211221.00.00";
        public const string TAB_HOME_PARAMS_ID = "EghmZWF0dXJlZA%3D%3D";
        public const string TAB_VIDEOS_DESCENDING_PARAMS_ID = "EgZ2aWRlb3MYAyAAMAE%3D";
        public const string TAB_COMMUNITY_PARAMS_ID = "Egljb21tdW5pdHk%3D";
        public const string TAB_CHANNELS_PARAMS_ID = "EghjaGFubmVscw%3D%3D";
        public const string TAB_ABOUT_PARAMS_ID = "EgVhYm91dA%3D%3D";

        public static string GetBrowseEndpointParams(ChannelTab channelTab)
        {
            switch (channelTab)
            {
                case ChannelTab.Home:
                    return TAB_HOME_PARAMS_ID;

                case ChannelTab.Videos:
                    return TAB_VIDEOS_DESCENDING_PARAMS_ID;

                case ChannelTab.Community:
                    return TAB_COMMUNITY_PARAMS_ID;

                case ChannelTab.Channels:
                    return TAB_CHANNELS_PARAMS_ID;

                case ChannelTab.About:
                    return TAB_ABOUT_PARAMS_ID;
            }

            return null;
        }

        public YouTubeChannelTabResult GetChannelTab(YouTubeChannel youTubeChannel, ChannelTab channelTab)
        {
            string browseParams = GetBrowseEndpointParams(channelTab);
            return GetChannelTab(youTubeChannel.Id, browseParams);
        }

        private YouTubeChannelTabResult GetChannelTab(string channelId, string browseEndpointParams)
        {
            string url = $"{API_V1_BROWSE_URL}?key={API_V1_KEY}";
            JObject body = GenerateChannelTabRequestBody(channelId, browseEndpointParams, null);
            int errorCode = HttpsPost(url, body.ToString(), out string response);
            if (errorCode == 200)
            {
                JObject json = JObject.Parse(response);
                if (json == null)
                {
                    return new YouTubeChannelTabResult(null, 404);
                }

                JObject j = json.Value<JObject>("contents");
                if (j == null)
                {
                    return new YouTubeChannelTabResult(null, 404);
                }
                j = j.Value<JObject>("twoColumnBrowseResultsRenderer");
                if (j == null)
                {
                    return new YouTubeChannelTabResult(null, 404);
                }
                JArray jaTabs = j.Value<JArray>("tabs");
                if (jaTabs == null || jaTabs.Count == 0)
                {
                    return new YouTubeChannelTabResult(null, 404);
                }

                JObject jSelectedTab = FindSelectedTab(jaTabs);
                if (jSelectedTab == null)
                {
                    return new YouTubeChannelTabResult(null, 404);
                }

                string tabTitle = jSelectedTab.Value<string>("title");
                return new YouTubeChannelTabResult(new YouTubeChannelTab(tabTitle, jSelectedTab), errorCode);
            }

            return new YouTubeChannelTabResult(null, errorCode);
        }

        public static JObject FindSelectedTab(JArray jaTabs)
        {
            foreach (JObject jObject in jaTabs)
            {
                JObject j = jObject.Value<JObject>("tabRenderer");
                if (j == null)
                {
                    j = jObject.Value<JObject>("expandableTabRenderer");
                }

                if (j != null)
                {
                    bool selected = j.Value<bool>("selected");
                    if (selected)
                    {
                        return j;
                    }
                }
            }
            return null;
        }

        public VideoIdPageResult GetVideoIdPage(YouTubeChannel youTubeChannel, string continuationToken)
        {
            return GetVideoIdPage(youTubeChannel?.Id, continuationToken);
        }

        private VideoIdPageResult GetVideoIdPage(string channelId, string continuationToken)
        {
            bool tokenExists = !string.IsNullOrEmpty(continuationToken) && !string.IsNullOrWhiteSpace(continuationToken);
            string browseParams = !tokenExists ? GetBrowseEndpointParams(ChannelTab.Videos) : null;
            JObject body = GenerateChannelTabRequestBody(channelId, browseParams, continuationToken);
            string url = $"{API_V1_BROWSE_URL}?key={API_V1_KEY}";
            int errorCode = HttpsPost(url, body.ToString(), out string response);
            if (errorCode == 200)
            {
                JObject json = JObject.Parse(response);
                VideoIdPage videoIdPage = new VideoIdPage(json, tokenExists);
                int count = videoIdPage.Parse();
                return new VideoIdPageResult(videoIdPage, count > 0 ? 200 : 400);
            }
            return new VideoIdPageResult(null, errorCode);
        }

        public JObject GenerateChannelVideoListRequestBody(string channelId, string continuationToken)
        {
            string browseParams = GetBrowseEndpointParams(ChannelTab.Videos);
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

        public VideoListResult GetChannelVideoList(YouTubeChannel channel)
        {
            return GetChannelVideoList(channel.Id);
        }

        private VideoListResult GetChannelVideoList(string channelId)
        {
            JArray resList = new JArray();
            string continuationToken = null;
            int errorCode;
            while (true)
            {
                VideoIdPageResult videoIdPageResult = GetVideoIdPage(channelId, continuationToken);

                errorCode = videoIdPageResult.ErrorCode;
                if (errorCode != 200)
                {
                    break;
                }

                foreach (string videoId in videoIdPageResult.VideoPage.VideoIds)
                {
                    if (GetSimplifiedVideoInfo(videoId, out JObject jInfo) == 200)
                    {
                        resList.Add(jInfo);
                    }
                }
    
                continuationToken = videoIdPageResult.VideoPage.ContinuationToken;
                bool continuationTokenExists = !string.IsNullOrEmpty(continuationToken) && !string.IsNullOrEmpty(continuationToken);
                if (!continuationTokenExists)
                {
                    break;
                }
            
                System.Diagnostics.Debug.WriteLine(continuationToken);
            }

            return new VideoListResult(resList, resList.Count > 0 ? 200 : errorCode);
        }

        private static JObject FindVideosTab(JObject root)
        {
            JObject j = root.Value<JObject>("contents");
            if (j == null)
            {
                return null;
            }
            j = j.Value<JObject>("twoColumnBrowseResultsRenderer");
            if (j == null)
            {
                return null;
            }
            JArray jaTabs = j.Value<JArray>("tabs");
            foreach (JObject jTab in jaTabs)
            {
                j = jTab.Value<JObject>("tabRenderer");
                if (j == null)
                {
                    j = jTab.Value<JObject>("expandableTabRenderer");
                }
                if (j != null)
                {
                    string tabTitle = j.Value<string>("title");
                    if (tabTitle == "Videos")
                    {
                        return jTab;
                    }
                }
            }
            return null;
        }

        internal static JArray FindItemsArray(JObject json, bool token)
        {
            try
            {
                if (token)
                {
                    List<ITabPageParser> continuationParsers = new List<ITabPageParser>()
                    {
                        new TabPageVideoContinuationParser1(),
                        new TabPageVideoContinuationParser2()
                    };
                    foreach (ITabPageParser continuationParser in continuationParsers)
                    {
                        JArray items = continuationParser.FindGridItems(json);
                        if (items != null && items.Count > 0)
                        {
                            return items;
                        }
                    }
                }
                else
                {
                    json = FindVideosTab(json);
                    if (json != null)
                    {
                        List<ITabPageParser> parsers = new List<ITabPageParser>() {
                            new TabPageParserVideo1(), new TabPageParserVideo2()
                        };
                        foreach (ITabPageParser parser in parsers)
                        {
                            JArray items = parser.FindGridItems(json);
                            if (items != null && items.Count > 0)
                            {
                                return items;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                return null;
            }

            return null;
        }

        internal static string ExtractVideoIDsFromGridRendererItem(JObject gridVideoRendererItem)
        {
            JObject j = gridVideoRendererItem.Value<JObject>("gridVideoRenderer");
            if (j != null)
            {
                return j.Value<string>("videoId");
            }
            else
            {
                j = gridVideoRendererItem.Value<JObject>("richItemRenderer");
                if (j != null)
                {
                    j = j.Value<JObject>("content");
                    if (j != null)
                    {
                        j = j.Value<JObject>("videoRenderer");
                        if (j != null)
                        {
                            return j.Value<string>("videoId");
                        }
                    }
                }
            }
            return null;
        }

        internal static List<string> ExtractVideoIDsFromGridRendererItems(
            JArray gridVideoRendererItems, out string continuationToken)
        {
            continuationToken = null;
            if (gridVideoRendererItems == null || gridVideoRendererItems.Count == 0)
            {
                return null;
            }

            List<string> idList = new List<string>();
            foreach (JObject jItem in gridVideoRendererItems)
            {
                string videoId = ExtractVideoIDsFromGridRendererItem(jItem);
                if (!string.IsNullOrEmpty(videoId) && !string.IsNullOrWhiteSpace(videoId))
                {
                    idList.Add(videoId);
                }
                else
                {
                    JObject jContinuationItemRenderer = jItem.Value<JObject>("continuationItemRenderer");
                    if (jContinuationItemRenderer != null)
                    {
                        JObject j = jContinuationItemRenderer.Value<JObject>("continuationEndpoint");
                        if (j != null)
                        {
                            j = j.Value<JObject>("continuationCommand");
                            if (j != null)
                            {
                                continuationToken = j.Value<string>("token");
                            }
                        }
                    }
                }
            }
            return idList;
        }

        public int GetRawVideoInfo(string videoId, out string rawInfoJsonString)
        {
            JObject body = GenerateVideoInfoRequestBody(videoId);
            string url = $"{API_V1_PLAYER_URL}?key={API_V1_KEY}";
            int errorCode = HttpsPost(url, body.ToString(), out rawInfoJsonString);
            return errorCode;
        }

        public YouTubeVideo GetVideo(string videoId)
        {
            int errorCode = GetRawVideoInfo(videoId, out string rawVideoInfo);
            if (errorCode == 200)
            {
                JObject json = JObject.Parse(rawVideoInfo);
                JObject jPlayabilityStatus = json.Value<JObject>("playabilityStatus");
                YouTubeVideoPlayabilityStatus videoStatus = YouTubeVideoPlayabilityStatus.Parse(jPlayabilityStatus);
                if (!videoStatus.IsPlayable)
                {
                    return YouTubeVideo.CreateEmpty(videoStatus);
                }

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
                        videoTitle, videoId, length, dateUploaded, datePublished, ownerChannelTitle,
                        ownerChannelId, description, viewCount, category, isPrivate, isUnlisted,
                        isFamilySafe, isLiveContent, json, videoStatus);
                    return youTubeVideo;
                }
                else
                {
                    errorCode = 400;
                }
            }
            return YouTubeVideo.CreateEmpty(new YouTubeVideoPlayabilityStatus(null, null, errorCode, null));
        }

        public int GetSimplifiedVideoInfo(string videoId, out JObject simplifiedVideoInfo)
        {
            int errorCode = GetRawVideoInfo(videoId, out string rawInfo);
            if (errorCode == 200)
            {
                JObject json = JObject.Parse(rawInfo);
                if (json == null)
                {
                    simplifiedVideoInfo = null;
                    return 404;
                }
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

    public enum ChannelTab { Home, Videos, Community, Channels, About }
}
