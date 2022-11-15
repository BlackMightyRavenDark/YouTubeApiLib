using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
    public static class Utils
    {
        public const string YOUTUBE_URL = "https://www.youtube.com";
        public const string API_V1_KEY = "AIzaSyAO_FJ2SlqU8Q4STEHLGCilw_Y9_11qcW8";
        public const string API_V1_BROWSE_URL = "https://www.youtube.com/youtubei/v1/browse";
        public const string API_V1_PLAYER_URL = "https://www.youtube.com/youtubei/v1/player";
        public const string YOUTUBE_CLIENT_VERSION = "2.20211221.00.00";

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
                json["params"] = youTubeChannelTabPage.ParamsId;
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

        public static JObject GenerateVideoInfoRequestBody(string videoId)
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
            json["videoId"] = videoId;

            return json;
        }

        internal static RawVideoInfoResult GetRawVideoInfo(string videoId)
        {
            JObject body = GenerateVideoInfoRequestBody(videoId);
            string url = $"{API_V1_PLAYER_URL}?key={API_V1_KEY}";
            int errorCode = HttpsPost(url, body.ToString(), out string rawInfoJsonString);
            if (errorCode == 200)
            {
                JObject j = JObject.Parse(rawInfoJsonString);
                if (j != null)
                {
                    return new RawVideoInfoResult(new RawVideoInfo(j), 200);
                }
            }
            return new RawVideoInfoResult(null, errorCode);
        }

        internal static SimplifiedVideoInfoResult GetSimplifiedVideoInfo(string videoId)
        {
            RawVideoInfoResult rawVideoInfoResult = GetRawVideoInfo(videoId);
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

            string videoTitle = jVideoDetails.Value<string>("title");
            string videoId = jVideoDetails.Value<string>("videoId");
            string shortDescription = jVideoDetails.Value<string>("shortDescription");
            int lengthSeconds = int.Parse(jVideoDetails.Value<string>("lengthSeconds"));
            string ownerChannelTitle = jVideoDetails.Value<string>("author");
            string ownerChannelId = jVideoDetails.Value<string>("channelId");
            int viewCount = int.Parse(jVideoDetails.Value<string>("viewCount"));
            bool isPrivate = jVideoDetails.Value<bool>("isPrivate");
            bool isLiveContent = jVideoDetails.Value<bool>("isLiveContent");
            string description = null;
            JObject jDescription = jMicroformatRenderer.Value<JObject>("description");
            if (jDescription != null)
            {
                description = jDescription.Value<string>("simpleText");
            }
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

            JObject jStreamingData = rawVideoInfo.RawData.Value<JObject>("streamingData");
            simplifiedVideoInfo["streamingData"] = jStreamingData;

            return new SimplifiedVideoInfoResult(new SimplifiedVideoInfo(simplifiedVideoInfo), 200);
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

            JObject jStreamingData = rawVideoInfo.RawData.Value<JObject>("streamingData");
            LinkedList<YouTubeMediaTrack> mediaTracks = YouTubeMediaFormatsParser.Parse(jStreamingData);

            JObject jPlayabilityStatus = rawVideoInfo.RawData.Value<JObject>("playabilityStatus");
            YouTubeVideoPlayabilityStatus videoStatus = YouTubeVideoPlayabilityStatus.Parse(jPlayabilityStatus);

            YouTubeVideo youTubeVideo = new YouTubeVideo(
                videoTitle, videoId, length, dateUploaded, datePublished, ownerChannelTitle,
                ownerChannelId, description, viewCount, category, isPrivate, isUnlisted,
                isFamilySafe, isLiveContent, videoThumbnails, mediaTracks,
                rawVideoInfo, simplifiedVideoInfo, videoStatus);
            return youTubeVideo;
        }

        internal static VideoPageResult GetVideoPage(string channelId, string continuationToken)
        {
            VideoIdPageResult videoIdPageResult = GetVideoIdPage(channelId, continuationToken);
            if (videoIdPageResult.ErrorCode == 200)
            {
                List<YouTubeVideo> videos = new List<YouTubeVideo>();
                foreach (string videoId in videoIdPageResult.VideoIdPage.VideoIds)
                {
                    RawVideoInfoResult rawVideoInfoResult = GetRawVideoInfo(videoId);
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

        internal static VideoIdPageResult GetVideoIdPage(string channelId, string continuationToken)
        {
            JObject body = GenerateChannelTabRequestBody(channelId, YouTubeChannelTabPages.Videos, continuationToken);
            string url = $"{API_V1_BROWSE_URL}?key={API_V1_KEY}";
            int errorCode = HttpsPost(url, body.ToString(), out string response);
            if (errorCode == 200)
            {
                JObject json = JObject.Parse(response);
                bool tokenExists = !string.IsNullOrEmpty(continuationToken) && !string.IsNullOrWhiteSpace(continuationToken);
                VideoIdPage videoIdPage = new VideoIdPage(json, tokenExists);
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
                VideoIdPageResult videoIdPageResult = GetVideoIdPage(channelId, continuationToken);

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
                    YouTubeChannelTab tabVideos = FindChannelTab(YouTubeChannelTabPages.Videos, json);
                    if (tabVideos != null)
                    {
                        List<ITabPageParser> parsers = new List<ITabPageParser>() {
                            new TabPageParserVideo1(), new TabPageParserVideo2()
                        };
                        foreach (ITabPageParser parser in parsers)
                        {
                            JArray items = parser.FindGridItems(tabVideos.Json);
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
