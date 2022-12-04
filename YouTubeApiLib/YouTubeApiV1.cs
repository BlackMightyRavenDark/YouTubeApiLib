using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using static YouTubeApiLib.Utils;

namespace YouTubeApiLib
{
    internal static class YouTubeApiV1
    {
        internal const string API_V1_KEY = "AIzaSyAO_FJ2SlqU8Q4STEHLGCilw_Y9_11qcW8";
        internal const string API_V1_BROWSE_URL = "https://www.youtube.com/youtubei/v1/browse";
        internal const string API_V1_PLAYER_URL = "https://www.youtube.com/youtubei/v1/player";
        internal const string API_V1_SEARCH_URL = "https://www.youtube.com/youtubei/v1/search";

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

        public static JObject GenerateSearchQueryRequestBody(
            string searchQuery, string continuationToken, YouTubeApiV1SearchResultFilter filter)
        {
            const string CLIENT_NAME = "WEB";
            const string CLIENT_VERSION = "2.20210408.08.00";

            JObject jClient = new JObject();
            jClient["clientName"] = CLIENT_NAME;
            jClient["clientVersion"] = CLIENT_VERSION;
            jClient["hl"] = "en";
            jClient["gl"] = "US";
            jClient["utcOffsetMinutes"] = 0;

            JObject jContext = new JObject();
            jContext.Add(new JProperty("client", jClient));

            JObject json = new JObject();
            json.Add(new JProperty("context", jContext));
            if (!string.IsNullOrEmpty(searchQuery) && !string.IsNullOrWhiteSpace(searchQuery))
            {
                json["query"] = searchQuery;
                if (filter != YouTubeApiV1SearchResultFilters.None)
                {
                    json["params"] = filter.ParamsId;
                }
            }
            else if (!string.IsNullOrEmpty(continuationToken) && !string.IsNullOrWhiteSpace(continuationToken))
            {
                json["continuation"] = continuationToken;
            }
            return json;
        }

        public static JObject GenerateChannelTabRequestBody(string channelId,
            YouTubeChannelTabPage youTubeChannelTabPage, string continuationToken)
        {
            const string CLIENT_NAME = "WEB";
            const string CLIENT_VERSION = "2.20211221.00.00";

            JObject jClient = new JObject();
            jClient["hl"] = "en";
            jClient["gl"] = "US";
            jClient["clientName"] = CLIENT_NAME;
            jClient["clientVersion"] = CLIENT_VERSION;

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

        public static JObject GenerateChannelVideoListRequestBody(string channelId, string continuationToken)
        {
            return GenerateChannelTabRequestBody(channelId, YouTubeChannelTabPages.Videos, continuationToken);
        }

        public static string GetSearchRequestUrl()
        {
            return $"{API_V1_SEARCH_URL}?key={API_V1_KEY}";
        }

        internal static RawVideoInfoResult GetRawVideoInfo(
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
            int errorCode = HttpsPost(url, body.ToString(), out string rawVideoInfoJsonString);
            if (errorCode == 200)
            {
                JObject j = JObject.Parse(rawVideoInfoJsonString);
                if (j != null)
                {
                    return new RawVideoInfoResult(new RawVideoInfo(j, method), 200);
                }
            }
            return new RawVideoInfoResult(null, errorCode);
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

        internal static YouTubeApiV1SearchResults SearchYouTube(
            string searchQuery, string continuationToken,
            YouTubeApiV1SearchResultFilter searchResultFilter)
        {
            string url = GetSearchRequestUrl();
            JObject body = GenerateSearchQueryRequestBody(
                searchQuery, continuationToken, searchResultFilter);
            int errorCode = HttpsPost(url, body.ToString(), out string response);
            if (errorCode == 200)
            {
                JObject j = JObject.Parse(response);
                if (j != null)
                {
                    bool isContinationToken = !string.IsNullOrEmpty(continuationToken) && !string.IsNullOrWhiteSpace(continuationToken);
                    return new YouTubeApiV1SearchResults(j, searchResultFilter, isContinationToken, errorCode);
                }
            }
            return new YouTubeApiV1SearchResults(null, searchResultFilter, false, errorCode);
        }
    }
}
