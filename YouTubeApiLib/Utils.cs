using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using MultiThreadedDownloaderLib;

namespace YouTubeApiLib
{
    public static class Utils
    {
        public const string YOUTUBE_URL = "https://www.youtube.com";
        public const string API_V1_KEY = "AIzaSyAO_FJ2SlqU8Q4STEHLGCilw_Y9_11qcW8";
        public const string API_V1_BROWSE_URL = "https://www.youtube.com/youtubei/v1/browse";
        public const string API_V1_PLAYER_URL = "https://www.youtube.com/youtubei/v1/player";
        public const string YOUTUBE_CLIENT_VERSION = "2.20211221.00.00";

        public enum VideoInfoGettingMethod
        {
            HiddenApiEncryptedUrls,
            HiddenApiDecryptedUrls,

            /// <summary>
            /// This way will download and parse the video web page.
            /// Warning!!! YouTube may ban your IP address if you make requests too often!
            /// You will get the "HTTP error 429" for some days or weeks (maybe forever!).
            /// You should not abuse this method!
            /// </summary>
            WebPage,

            /// <summary>
            /// Used when user provide the web page code or video info data manually.
            /// </summary>
            Manual
        }

        public static JObject GenerateChannelVideoListRequestBody(string channelId, string continuationToken)
        {
            return GenerateChannelTabRequestBody(channelId, YouTubeChannelTabPages.Videos, continuationToken);
        }

        public static JObject GenerateChannelTabRequestBody(string channelId,
            YouTubeChannelTabPage youTubeChannelTabPage, string continuationToken)
        {
            JObject jClient = new JObject();
            jClient["hl"] = "en";
            jClient["gl"] = "US";
            jClient["clientName"] = "WEB";
            jClient["clientVersion"] = YOUTUBE_CLIENT_VERSION;

            JObject jContext = new JObject();
            jContext["client"] = jClient;

            JObject json = new JObject();
            json["context"] = jContext;
            json["browseId"] = channelId;
            if (string.IsNullOrEmpty(continuationToken) || string.IsNullOrWhiteSpace(continuationToken))
            {
                if (youTubeChannelTabPage != null)
                {
                    json["params"] = youTubeChannelTabPage.ParamsId;
                }
            }
            else
            {
                json["continuation"] = continuationToken;
            }
            return json;
        }

        public static string GetVideoUrl(string videoId)
        {
            return $"{YOUTUBE_URL}/watch?v={videoId}";
        }

        /// <summary>
        /// Генерирует тело POST-запроса для получения информации о видео.
        /// Ответ будет содержать всё необходимое, кроме ссылок для скачивания.
        /// Ссылки будут зашифрованы (Cipher, ограничение скорости и т.д.).
        /// </summary>
        /// <param name="videoId">ID видео</param>
        /// <returns>Тело запроса</returns>
        public static JObject GenerateVideoInfoEncryptedRequestBody(string videoId)
        {
            const string CLIENT_NAME = "WEB";
            const string CLIENT_VERSION = "2.20201021.03.00";

            JObject jClient = new JObject();
            jClient["hl"] = "en";
            jClient["gl"] = "US";
            jClient["clientName"] = CLIENT_NAME;
            jClient["clientVersion"] = CLIENT_VERSION;

            JObject jContext = new JObject();
            jContext.Add(new JProperty("client", jClient));

            JObject json = new JObject();
            json.Add(new JProperty("context", jContext));
            json["videoId"] = videoId;

            return json;
        }

        /// <summary>
        /// Генерирует тело POST-запроса для получения информации о видео.
        /// Ответ будет содержать уже расшифрованные ссылки для скачивания
        /// без ограничения скорости, но остальная информация будет не полной.
        /// Используйте этот запрос только для получения ссылок.
        /// Внимание! Этот запрос не работает для видео с пометкой "18+"!
        /// </summary>
        /// <param name="videoId">ID видео</param>
        /// <returns>Тело запроса</returns>
        public static JObject GenerateVideoInfoDecryptedRequestBody(string videoId)
        {
            const string CLIENT_NAME = "ANDROID";
            const string CLIENT_VERSION = "16.46.37";

            JObject jClient = new JObject();
            jClient["hl"] = "en";
            jClient["gl"] = "US";
            jClient["clientName"] = CLIENT_NAME;
            jClient["clientVersion"] = CLIENT_VERSION;
            jClient["clientScreen"] = null;
            jClient["utcOffsetMinutes"] = 0;

            JObject jContext = new JObject();
            jContext.Add(new JProperty("client", jClient));

            JObject json = new JObject();
            json.Add(new JProperty("context", jContext));
            json["contentCheckOk"] = true;
            json["racyCheckOk"] = true;
            json["videoId"] = videoId;

            return json;
        }

        public static RawVideoInfoResult GetRawVideoInfo(
            string videoId, VideoInfoGettingMethod method)
        {
            switch (method)
            {
                case VideoInfoGettingMethod.HiddenApiEncryptedUrls:
                case VideoInfoGettingMethod.HiddenApiDecryptedUrls:
                    return GetRawVideoInfoViaApi(videoId, method);

                case VideoInfoGettingMethod.WebPage:
                    return GetRawVideoInfoViaWebPage(videoId);
            }
            return new RawVideoInfoResult(null, 400);
        }

        internal static RawVideoInfoResult GetRawVideoInfoViaWebPage(string videoId)
        {
            YouTubeVideoWebPageResult videoWebPageResult = DownloadVideoWebPage(videoId);
            if (videoWebPageResult.ErrorCode == 200)
            {
                return ExtractRawVideoInfoFromWebPage(videoWebPageResult.VideoWebPage);
            }
            return new RawVideoInfoResult(null, videoWebPageResult.ErrorCode);
        }

        internal static RawVideoInfoResult GetRawVideoInfoViaApi(
            string videoId, VideoInfoGettingMethod method)
        {
            if (method == VideoInfoGettingMethod.WebPage)
            {
                return GetRawVideoInfoViaWebPage(videoId);
            }

            JObject body = method == VideoInfoGettingMethod.HiddenApiEncryptedUrls ?
                GenerateVideoInfoEncryptedRequestBody(videoId) :
                GenerateVideoInfoDecryptedRequestBody(videoId);
            string url = $"{API_V1_PLAYER_URL}?key={API_V1_KEY}";
            int errorCode = HttpsPost(url, body.ToString(), out string rawInfoJsonString);
            if (errorCode == 200)
            {
                JObject j = JObject.Parse(rawInfoJsonString);
                if (j != null)
                {
                    return new RawVideoInfoResult(new RawVideoInfo(j, method), 200);
                }
            }
            return new RawVideoInfoResult(null, errorCode);
        }

        internal static SimplifiedVideoInfoResult GetSimplifiedVideoInfo(string videoId)
        {
            RawVideoInfoResult rawVideoInfoResult = GetRawVideoInfo(videoId, YouTubeApi.defaultVideoInfoGettingMethod);
            if (rawVideoInfoResult.ErrorCode == 200)
            {
                return ParseRawVideoInfo(rawVideoInfoResult.RawVideoInfo);
            }
            return new SimplifiedVideoInfoResult(null, rawVideoInfoResult.ErrorCode);
        }

        internal static YouTubeChannelTabResult GetChannelTab(string channelId, YouTubeChannelTabPage channelTabPage)
        {
            string url = $"{API_V1_BROWSE_URL}?key={API_V1_KEY}";
            JObject body = GenerateChannelTabRequestBody(channelId, channelTabPage, null);
            int errorCode = HttpsPost(url, body.ToString(), out string response);
            if (errorCode == 200)
            {
                JObject json = JObject.Parse(response);
                if (json == null)
                {
                    return new YouTubeChannelTabResult(null, 404);
                }

                YouTubeChannelTab selectedTab = FindSelectedChannelTab(json);
                if (selectedTab == null)
                {
                    return new YouTubeChannelTabResult(null, 404);
                }

                if (selectedTab.Title != channelTabPage.Title)
                {
                    // If the requested tab is not exists,
                    // API returns the "Home" tab content.
                    return new YouTubeChannelTabResult(null, 404);
                }

                return new YouTubeChannelTabResult(selectedTab, errorCode);
            }

            return new YouTubeChannelTabResult(null, errorCode);
        }

        public static SimplifiedVideoInfoResult ParseRawVideoInfo(RawVideoInfo rawVideoInfo)
        {
            JObject jVideoDetails = rawVideoInfo.RawData.Value<JObject>("videoDetails");
            if (jVideoDetails == null)
            {
                return new SimplifiedVideoInfoResult(null, 404);
            }

            JObject jMicroformat = rawVideoInfo.RawData.Value<JObject>("microformat");
            if (jMicroformat == null)
            {
                return new SimplifiedVideoInfoResult(null, 404);
            }

            JObject jMicroformatRenderer = jMicroformat.Value<JObject>("playerMicroformatRenderer");
            if (jMicroformatRenderer == null)
            {
                return new SimplifiedVideoInfoResult(null, 404);
            }

            string videoId = jVideoDetails.Value<string>("videoId");
            string videoTitle = jVideoDetails.Value<string>("title");
            int lengthSeconds = int.Parse(jVideoDetails.Value<string>("lengthSeconds"));
            string ownerChannelTitle = jVideoDetails.Value<string>("author");
            string ownerChannelId = jVideoDetails.Value<string>("channelId");
            int viewCount = int.Parse(jVideoDetails.Value<string>("viewCount"));
            bool isPrivate = jVideoDetails.Value<bool>("isPrivate");
            bool isLiveContent = jVideoDetails.Value<bool>("isLiveContent");
            JObject jDescription = jMicroformatRenderer.Value<JObject>("description");
            string description = jDescription?.Value<string>("simpleText");
            string shortDescription = jVideoDetails.Value<string>("shortDescription");
            bool isFamilySafe = jMicroformatRenderer.Value<bool>("isFamilySafe");
            bool isUnlisted = jMicroformatRenderer.Value<bool>("isUnlisted");
            string category = jMicroformatRenderer.Value<string>("category");
            string datePublished = jMicroformatRenderer.Value<string>("publishDate");
            string dateUploaded = jMicroformatRenderer.Value<string>("uploadDate");

            JObject simplifiedVideoInfo = new JObject();
            simplifiedVideoInfo["title"] = videoTitle;
            simplifiedVideoInfo["id"] = videoId;
            simplifiedVideoInfo["url"] = GetVideoUrl(videoId);
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

            List<YouTubeVideoThumbnail> videoThumbnails = GetThumbnailUrls(jMicroformat, videoId);
            simplifiedVideoInfo["thumbnails"] = ThumbnailsToJson(videoThumbnails);

            StreamingData streamingData = null;
            if (rawVideoInfo.DataGettingMethod != VideoInfoGettingMethod.HiddenApiDecryptedUrls &&
                YouTubeApi.decryptMediaTrackUrlsAutomaticallyIfPossible)
            {
                streamingData = GetStreamingData(videoId, VideoInfoGettingMethod.HiddenApiDecryptedUrls);
            }
            else
            {
                JObject jStreamingData = rawVideoInfo.RawData.Value<JObject>("streamingData");
                if (jStreamingData != null)
                {
                    streamingData = new StreamingData(jStreamingData, rawVideoInfo.DataGettingMethod);
                }
            }
            simplifiedVideoInfo["streamingData"] = streamingData?.RawData;

            return new SimplifiedVideoInfoResult(new SimplifiedVideoInfo(simplifiedVideoInfo, streamingData), 200);
        }

        public static YouTubeVideo MakeYouTubeVideo(RawVideoInfo rawVideoInfo)
        {
            SimplifiedVideoInfoResult simplifiedVideoInfoResult = ParseRawVideoInfo(rawVideoInfo);
            if (simplifiedVideoInfoResult.ErrorCode != 200)
            {
                return YouTubeVideo.CreateEmpty(new YouTubeVideoPlayabilityStatus(null, "Not parsed", 400, null));
            }

            return MakeYouTubeVideo(rawVideoInfo, simplifiedVideoInfoResult.SimplifiedVideoInfo);
        }

        public static YouTubeVideo MakeYouTubeVideo(RawVideoInfo rawVideoInfo, SimplifiedVideoInfo simplifiedVideoInfo)
        {
            string videoTitle = simplifiedVideoInfo.Info.Value<string>("title");
            string videoId = simplifiedVideoInfo.Info.Value<string>("id");
            int lengthSeconds = int.Parse(simplifiedVideoInfo.Info.Value<string>("lengthSeconds"));
            TimeSpan length = TimeSpan.FromSeconds(lengthSeconds);
            string ownerChannelTitle = simplifiedVideoInfo.Info.Value<string>("ownerChannelTitle");
            string ownerChannelId = simplifiedVideoInfo.Info.Value<string>("ownerChannelId");
            int viewCount = int.Parse(simplifiedVideoInfo.Info.Value<string>("viewCount"));
            bool isPrivate = simplifiedVideoInfo.Info.Value<bool>("isPrivate");
            bool isLiveContent = simplifiedVideoInfo.Info.Value<bool>("isLiveContent");
            string description = simplifiedVideoInfo.Info.Value<string>("description");
            if (string.IsNullOrEmpty(description))
            {
                description = simplifiedVideoInfo.Info.Value<string>("shortDescription");
            }
            bool isFamilySafe = simplifiedVideoInfo.Info.Value<bool>("isFamilySafe");
            bool isUnlisted = simplifiedVideoInfo.Info.Value<bool>("isUnlisted");
            string category = simplifiedVideoInfo.Info.Value<string>("category");
            string published = simplifiedVideoInfo.Info.Value<string>("datePublished");
            string uploaded = simplifiedVideoInfo.Info.Value<string>("dateUploaded");
            StringToDateTime(published, out DateTime datePublished);
            StringToDateTime(uploaded, out DateTime dateUploaded);

            List<YouTubeVideoThumbnail> videoThumbnails = null;
            JArray jaThumbnails = simplifiedVideoInfo.Info.Value<JArray>("thumbnails");
            if (jaThumbnails != null && jaThumbnails.Count > 0)
            {
                videoThumbnails = new List<YouTubeVideoThumbnail>();
                foreach (JObject jThumbnail in jaThumbnails)
                {
                    string id = jThumbnail.Value<string>("id");
                    string url = jThumbnail.Value<string>("url");
                    videoThumbnails.Add(new YouTubeVideoThumbnail(id, url));
                }
            }

            LinkedList<YouTubeMediaTrack> mediaTracks = null;
            if (YouTubeApi.getMediaTracksInfoImmediately)
            {
                mediaTracks = simplifiedVideoInfo.StreamingData != null ?
                    ParseMediaTracks(simplifiedVideoInfo.StreamingData) :
                    ParseMediaTracks(rawVideoInfo);
            }

            JObject jPlayabilityStatus = rawVideoInfo.RawData.Value<JObject>("playabilityStatus");
            YouTubeVideoPlayabilityStatus videoStatus = YouTubeVideoPlayabilityStatus.Parse(jPlayabilityStatus);

            YouTubeVideo youTubeVideo = new YouTubeVideo(
                videoTitle, videoId, length, dateUploaded, datePublished, ownerChannelTitle,
                ownerChannelId, description, viewCount, category, isPrivate, isUnlisted,
                isFamilySafe, isLiveContent, videoThumbnails, mediaTracks,
                rawVideoInfo, simplifiedVideoInfo, videoStatus);
            return youTubeVideo;
        }

        internal static VideoPageResult GetVideoPage(string channelId, YouTubeChannelTabPage tabPage, string continuationToken)
        {
            VideoIdPageResult videoIdPageResult = GetVideoIdPage(channelId, tabPage, continuationToken);
            if (videoIdPageResult.ErrorCode == 200)
            {
                List<YouTubeVideo> videos = new List<YouTubeVideo>();
                foreach (string videoId in videoIdPageResult.VideoIdPage.VideoIds)
                {
                    RawVideoInfoResult rawVideoInfoResult = GetRawVideoInfo(videoId, YouTubeApi.defaultVideoInfoGettingMethod);
                    if (rawVideoInfoResult.ErrorCode == 200)
                    {
                        YouTubeVideo video = MakeYouTubeVideo(rawVideoInfoResult.RawVideoInfo);
                        if (video != null && video.Status != null)
                        {
                            videos.Add(video);
                        }
                    }
                }
                return new VideoPageResult(new VideoPage(videos, videoIdPageResult.VideoIdPage.ContinuationToken), 200);
            }
            return new VideoPageResult(null, videoIdPageResult.ErrorCode);
        }

        internal static VideoIdPageResult GetVideoIdPage(string channelId, YouTubeChannelTabPage tabPage, string continuationToken)
        {
            JObject body = GenerateChannelTabRequestBody(channelId, tabPage, continuationToken);
            bool tokenExists = !string.IsNullOrEmpty(continuationToken) && !string.IsNullOrWhiteSpace(continuationToken);
            return GetVideoIdPage(body, tokenExists);
        }

        internal static VideoIdPageResult GetVideoIdPage(JObject requestBody, bool continuationTokenExists)
        {
            string url = $"{API_V1_BROWSE_URL}?key={API_V1_KEY}";
            string body = requestBody != null ? requestBody.ToString() : string.Empty;
            int errorCode = HttpsPost(url, body, out string response);
            if (errorCode == 200)
            {
                JObject json = JObject.Parse(response);
                VideoIdPage videoIdPage = new VideoIdPage(json, continuationTokenExists);
                int count = videoIdPage.Parse();
                return new VideoIdPageResult(videoIdPage, count > 0 ? 200 : 400);
            }
            return new VideoIdPageResult(null, errorCode);
        }

        internal static VideoListResult GetChannelVideoList(string channelId)
        {
            JArray resList = new JArray();
            string continuationToken = null;
            int errorCode;
            while (true)
            {
                VideoIdPageResult videoIdPageResult = GetVideoIdPage(channelId, YouTubeChannelTabPages.Videos, continuationToken);

                errorCode = videoIdPageResult.ErrorCode;
                if (errorCode != 200)
                {
                    break;
                }

                foreach (string videoId in videoIdPageResult.VideoIdPage.VideoIds)
                {
                    SimplifiedVideoInfoResult simplifiedVideoInfoResult = GetSimplifiedVideoInfo(videoId);
                    if (simplifiedVideoInfoResult.ErrorCode == 200)
                    {
                        resList.Add(simplifiedVideoInfoResult.SimplifiedVideoInfo.Info);
                    }
                }

                continuationToken = videoIdPageResult.VideoIdPage.ContinuationToken;
                bool continuationTokenExists = !string.IsNullOrEmpty(continuationToken) && !string.IsNullOrEmpty(continuationToken);
                if (!continuationTokenExists)
                {
                    break;
                }

                System.Diagnostics.Debug.WriteLine(continuationToken);
            }

            return new VideoListResult(resList, resList.Count > 0 ? 200 : errorCode);
        }

        public static StreamingData GetStreamingData(string videoId, VideoInfoGettingMethod method)
        {
            RawVideoInfoResult rawVideoInfoResult = GetRawVideoInfo(videoId, method);
            if (rawVideoInfoResult.ErrorCode == 200)
            {
                JObject jStreamingData = rawVideoInfoResult.RawVideoInfo.RawData.Value<JObject>("streamingData");
                if (jStreamingData != null)
                {
                    return new StreamingData(jStreamingData, method);
                }
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
                        JObject jRenderer = j.Value<JObject>("videoRenderer");
                        if (jRenderer == null)
                        {
                            jRenderer = j.Value<JObject>("reelItemRenderer");
                        }
                        if (jRenderer != null)
                        {
                            return jRenderer.Value<string>("videoId");
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

        public static JArray FindTabItems(JObject megaRoot)
        {
            JObject j = megaRoot.Value<JObject>("contents");
            if (j == null)
            {
                return null;
            }
            j = j.Value<JObject>("twoColumnBrowseResultsRenderer");
            if (j == null)
            {
                return null;
            }
            return j.Value<JArray>("tabs");
        }

        public static YouTubeChannelTab FindSelectedChannelTab(JObject megaRoot)
        {
            JArray jaTabs = FindTabItems(megaRoot);
            if (jaTabs == null || jaTabs.Count == 0)
            {
                return null;
            }
            return FindSelectedChannelTab(jaTabs);
        }
        
        public static YouTubeChannelTab FindSelectedChannelTab(JArray jaTabs)
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
                        string tabTitle = j.Value<string>("title");
                        return new YouTubeChannelTab(tabTitle, jObject);
                    }
                }
            }
            return null;
        }

        private static YouTubeChannelTab FindChannelTab(
            YouTubeChannelTabPage channelTabPage, JObject megaRoot)
        {
            JArray jaTabs = FindTabItems(megaRoot);
            if (jaTabs == null || jaTabs.Count == 0)
            {
                return null;
            }
            foreach (JObject jTab in jaTabs)
            {
                JObject j = jTab.Value<JObject>("tabRenderer");
                if (j == null)
                {
                    j = jTab.Value<JObject>("expandableTabRenderer");
                }
                if (j != null)
                {
                    string tabTitle = j.Value<string>("title");
                    if (tabTitle == channelTabPage.Title)
                    {
                        return new YouTubeChannelTab(tabTitle, jTab);
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
                    YouTubeChannelTab selectedTab = FindSelectedChannelTab(json);
                    if (selectedTab != null)
                    {
                        List<ITabPageParser> parsers = new List<ITabPageParser>() {
                            new TabPageParserVideo1(), new TabPageParserVideo2()
                        };
                        foreach (ITabPageParser parser in parsers)
                        {
                            JArray items = parser.FindGridItems(selectedTab.Json);
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

        internal static List<YouTubeVideoThumbnail> GetThumbnailUrls(JObject jMicroformat, string videoId)
        {
            List<YouTubeVideoThumbnail> possibleThumbnails = new List<YouTubeVideoThumbnail>()
            {
                new YouTubeVideoThumbnail("maxresdefault", $"https://i.ytimg.com/vi/{videoId}/maxresdefault.jpg"),
                new YouTubeVideoThumbnail("hqdefault", $"https://i.ytimg.com/vi/{videoId}/hqdefault.jpg"),
                new YouTubeVideoThumbnail("mqdefault", $"https://i.ytimg.com/vi/{videoId}/mqdefault.jpg"),
                new YouTubeVideoThumbnail("sddefault", $"https://i.ytimg.com/vi/{videoId}/sddefault.jpg"),
                new YouTubeVideoThumbnail("default", $"https://i.ytimg.com/vi/{videoId}/default.jpg")
            };
            List<YouTubeVideoThumbnail> thumbnails = ExtractThumbnailsFromMicroformat(jMicroformat);
            foreach (YouTubeVideoThumbnail thumbnail in thumbnails)
            {
                bool found = false;
                foreach (YouTubeVideoThumbnail thumbnail2 in possibleThumbnails)
                {
                    if (thumbnail.Url == thumbnail2.Url)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    possibleThumbnails.Add(thumbnail);
                }
            }

            List<YouTubeVideoThumbnail> resList = new List<YouTubeVideoThumbnail>();
            foreach (YouTubeVideoThumbnail thumbnail in possibleThumbnails)
            {
                //TODO: Check the URL availability
                //But it's extremely slow operation :(
                resList.Add(thumbnail);
            }

            return resList;
        }

        private static List<YouTubeVideoThumbnail> ExtractThumbnailsFromMicroformat(JObject jMicroformat)
        {
            if (jMicroformat != null)
            {
                JObject jMicroformatRenderer = jMicroformat.Value<JObject>("playerMicroformatRenderer");
                return ExtractThumbnailsFromMicroformatRenderer(jMicroformatRenderer);
            }
            return null;
        }

        private static List<YouTubeVideoThumbnail> ExtractThumbnailsFromMicroformatRenderer(JObject jMicroformatRenderer)
        {
            List<YouTubeVideoThumbnail> resList = new List<YouTubeVideoThumbnail>();
            if (jMicroformatRenderer != null)
            {
                JObject jThumbnail = jMicroformatRenderer.Value<JObject>("thumbnail");
                if (jThumbnail != null)
                {
                    JArray jaThumbnails = jThumbnail.Value<JArray>("thumbnails");
                    if (jaThumbnails != null && jaThumbnails.Count > 0)
                    {
                        foreach (JObject j in jaThumbnails)
                        {
                            string url = j.Value<string>("url");
                            if (!string.IsNullOrEmpty(url) && !string.IsNullOrWhiteSpace(url))
                            {
                                if (url.Contains("?"))
                                {
                                    url = url.Substring(0, url.IndexOf("?"));
                                }
                                if (url.Contains("vi_webp"))
                                {
                                    url = url.Replace("vi_webp", "vi").Replace(".webp", ".jpg");
                                }

                                bool found = false;
                                foreach (YouTubeVideoThumbnail thumbnail in resList)
                                {
                                    if (thumbnail.Url == url)
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                                if (!found)
                                {
                                    resList.Add(new YouTubeVideoThumbnail("Unnamed", url));
                                }
                            }
                        }
                    }
                }
            }
            return resList;
        }

        private static JArray ThumbnailsToJson(IEnumerable<YouTubeVideoThumbnail> videoThumbnails)
        {
            JArray jsonArr = new JArray();
            foreach (YouTubeVideoThumbnail thumbnail in videoThumbnails)
            {
                jsonArr.Add(thumbnail.ToJson());
            }
            return jsonArr;
        }

        internal static LinkedList<YouTubeMediaTrack> ParseMediaTracks(StreamingData streamingData)
        {
            return streamingData != null ? streamingData.Parse() : null;
        }

        internal static LinkedList<YouTubeMediaTrack> ParseMediaTracks(RawVideoInfo rawVideoInfo)
        {
            if (rawVideoInfo == null || rawVideoInfo.RawData == null)
            {
                return null;
            }
            JObject jStreamingData = rawVideoInfo.RawData.Value<JObject>("streamingData");
            StreamingData streamingData = new StreamingData(jStreamingData, rawVideoInfo.DataGettingMethod);
            return ParseMediaTracks(streamingData);
        }

        public static int HttpsPost(string url, string body, out string responseString)
        {
            responseString = "Client error";
            int res = 400;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentLength = body.Length;
            httpWebRequest.Host = "www.youtube.com";
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3591.2 Safari/537.36";
            httpWebRequest.Method = "POST";
            StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            try
            {
                streamWriter.Write(body);
                streamWriter.Dispose();
            }
            catch
            {
                if (streamWriter != null)
                {
                    streamWriter.Dispose();
                }
                return res;
            }
            try
            {
                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());
                try
                {
                    responseString = streamReader.ReadToEnd();
                    streamReader.Dispose();
                    res = (int)httpResponse.StatusCode;
                }
                catch
                {
                    if (streamReader != null)
                    {
                        streamReader.Dispose();
                    }
                    return 400;
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse httpWebResponse = (HttpWebResponse)ex.Response;
                    responseString = ex.Message;
                    res = (int)httpWebResponse.StatusCode;
                }
            }
            return res;
        }

        public static RawVideoInfoResult ExtractRawVideoInfoFromWebPage(YouTubeVideoWebPage webPage)
        {
            if (webPage != null)
            {
                string videoInfo = ExtractRawVideoInfoFromWebPageCode(webPage.WebPageCode);
                if (!string.IsNullOrEmpty(videoInfo) && !string.IsNullOrWhiteSpace(videoInfo))
                {
                    JObject j = JObject.Parse(videoInfo);
                    if (j != null)
                    {
                        return new RawVideoInfoResult(new RawVideoInfo(j, VideoInfoGettingMethod.WebPage), 200);
                    }
                    else
                    {
                        return new RawVideoInfoResult(null, 400);
                    }
                }
            }
            return new RawVideoInfoResult(null, 404);
        }

        internal static string ExtractRawVideoInfoFromWebPageCode(string webPageCode)
        {
            //TODO: Replace this shit with a cool web page parser!
            try
            {
                int n = webPageCode.IndexOf("var ytInitialPlayerResponse");
                if (n > 0)
                {
                    int n2 = webPageCode.IndexOf("}};var meta =");
                    if (n2 > 0)
                    {
                        return webPageCode.Substring(n + 30, n2 - n - 28);
                    }

                    n2 = webPageCode.IndexOf("};\nvar meta =");
                    if (n2 > 0)
                    {
                        return webPageCode.Substring(n + 29, n2 - n - 28);
                    }

                    n2 = webPageCode.IndexOf("}};var head =");
                    if (n2 > 0)
                    {
                        return webPageCode.Substring(n + 30, n2 - n - 28);
                    }

                    n2 = webPageCode.IndexOf("};\nvar head =");
                    if (n2 > 0)
                    {
                        return webPageCode.Substring(n + 29, n2 - n - 28);
                    }

                    n2 = webPageCode.IndexOf(";</script><div");
                    if (n2 > 0)
                    {
                        return webPageCode.Substring(n + 30, n2 - n - 30);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return null;
        }

        public static VideoId ExtractVideoIdFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            Uri uri;
            try
            {
                uri = new Uri(url);
            }
            catch (Exception ex)
            {
                //подразумевается, что юзер ввёл ID видео, а не ссылку.
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return new VideoId(url);
            }

            if (!uri.Host.EndsWith("youtube.com", StringComparison.OrdinalIgnoreCase) &&
                !uri.Host.EndsWith("youtu.be", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (string.IsNullOrEmpty(uri.Query))
            {
                if (!string.IsNullOrEmpty(uri.AbsolutePath) && !string.IsNullOrWhiteSpace(uri.AbsolutePath))
                {
                    string videoId = uri.AbsolutePath;
                    if (videoId.StartsWith("/shorts/", StringComparison.OrdinalIgnoreCase))
                    {
                        videoId = videoId.Substring(8);
                    }
                    else if (videoId.StartsWith("/embed/", StringComparison.OrdinalIgnoreCase))
                    {
                        videoId = videoId.Substring(7);
                    }

                    if (videoId.StartsWith("/"))
                    {
                        videoId = videoId.Remove(0, 1);
                    }

                    if (!string.IsNullOrEmpty(videoId) && videoId.Length > 11)
                    {
                        videoId = videoId.Substring(0, 11);
                    }

                    return new VideoId(videoId);
                }
                return null;
            }

            Dictionary<string, string> dict = SplitUrlQueryToDictionary(uri.Query);
            if (dict == null || !dict.ContainsKey("v"))
            {
                return null;
            }

            return new VideoId(dict["v"]);
        }

        public static int DownloadString(string url, out string response)
        {
            FileDownloader d = new FileDownloader();
            d.Url = url;
            return d.DownloadString(out response);
        }

        public static YouTubeVideoWebPageResult DownloadVideoWebPage(VideoId videoId)
        {
            return videoId != null ? DownloadVideoWebPage(videoId.Id) : null;
        }

        internal static YouTubeVideoWebPageResult DownloadVideoWebPage(string videoId)
        {
            string url = GetVideoUrl(videoId);
            int errorCode = DownloadString(url, out string responseWebPageCode);
            YouTubeVideoWebPage webPage = errorCode == 200 ? new YouTubeVideoWebPage(responseWebPageCode) : null;
            return new YouTubeVideoWebPageResult(webPage, errorCode);
        }

        public static Dictionary<string, string> SplitUrlQueryToDictionary(string urlQuery)
        {
            if (string.IsNullOrEmpty(urlQuery) || string.IsNullOrWhiteSpace(urlQuery))
            {
                return null;
            }
            if (urlQuery[0] == '?')
            {
                urlQuery = urlQuery.Remove(0, 1);
            }
            return SplitStringToKeyValues(urlQuery, '&', '=');
        }

        public static Dictionary<string, string> SplitStringToKeyValues(
            string inputString, char keySeparaor, char valueSeparator)
        {
            if (string.IsNullOrEmpty(inputString) || string.IsNullOrWhiteSpace(inputString))
            {
                return null;
            }
            string[] keyValues = inputString.Split(keySeparaor);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            for (int i = 0; i < keyValues.Length; i++)
            {
                if (!string.IsNullOrEmpty(keyValues[i]) && !string.IsNullOrWhiteSpace(keyValues[i]))
                {
                    string[] t = keyValues[i].Split(valueSeparator);
                    dict.Add(t[0], t[1]);
                }
            }
            return dict;
        }

        public static bool StringToDateTime(string inputString, out DateTime resDateTime, string format = "yyyy-MM-dd")
        {
            bool res = DateTime.TryParseExact(inputString, format,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out resDateTime);
            if (!res)
            {
                resDateTime = DateTime.MaxValue;
            }
            return res;
        }
    }
}
