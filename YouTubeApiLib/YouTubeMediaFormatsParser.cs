using System.Collections.Generic;
using System.Net;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
    public static class YouTubeMediaFormatsParser
    {
        public static LinkedList<YouTubeMediaTrack> Parse(JObject jStreamingData)
        {
            if (jStreamingData == null)
            {
                return null;
            }

            //TODO: Complete parsing

            LinkedList<YouTubeMediaTrack> resList = new LinkedList<YouTubeMediaTrack>();
            bool isDash = false;
            string dashManifestUrl = null;
            JToken jtDash = jStreamingData.Value<JToken>("dashManifestUrl");
            if (jtDash != null)
            {
                dashManifestUrl = jtDash.Value<string>();
                if (Utils.DownloadString(dashManifestUrl, out string dashManifest) == 200)
                {
                    LinkedList<YouTubeMediaTrack> dashList = ParseDashManifest(dashManifest, dashManifestUrl);
                    if (dashList != null)
                    {
                        foreach (YouTubeMediaTrack track in dashList)
                        {
                            resList.AddLast(track);
                        }
                    }
                }
                return resList;
            }
            bool isHls = false;

            JArray jaAdaptiveFormats = jStreamingData.Value<JArray>("adaptiveFormats");
            if (jaAdaptiveFormats != null)
            {
                foreach (JObject jFormat in jaAdaptiveFormats)
                {
                    string mimeType = jFormat.Value<string>("mimeType");
                    if (string.IsNullOrEmpty(mimeType) || string.IsNullOrWhiteSpace(mimeType))
                    {
                        System.Diagnostics.Debug.WriteLine("The \"mimeType\" field read error!");
                        continue;
                    }

                    if (mimeType.Contains("video"))
                    {
                        int formatId = jFormat.Value<int>("itag");
                        ParseMime(mimeType, out string mimeCodecs, out string mimeExt);
                        string fileExtension = !string.IsNullOrEmpty(mimeExt) && !string.IsNullOrWhiteSpace(mimeExt) ?
                            (mimeExt.ToLower() == "mp4" ? "m4v" : "webm") : "dat";
                        int bitrate = jFormat.Value<int>("bitrate");
                        int averageBitrate = jFormat.Value<int>("averageBitrate");
                        int videoWidth = jFormat.Value<int>("width");
                        int videoHeight = jFormat.Value<int>("height");
                        string quality = jFormat.Value<string>("quality");
                        string qualityLabel = jFormat.Value<string>("qualityLabel");
                        int videoFrameRate = jFormat.Value<int>("fps");
                        string projectionType = jFormat.Value<string>("projectionType");
                        string lastModified = jFormat.Value<string>("lastModified");
                        long contentLength = -1L;
                        JToken jt = jFormat.Value<JToken>("contentLength");
                        if (jt != null)
                        {
                            string contentLengthString = jt.Value<string>();
                            if (!long.TryParse(contentLengthString, out contentLength))
                            {
                                contentLength = -1;
                            }
                        }
                        int approxDurationMs = -1;
                        string cipherSignatureEncrypted = null;
                        string cipherEncryptedUrl = null;
                        bool isCiphered = false;
                        string url = null;
                        jt = jFormat.Value<JToken>("signatureCipher");
                        if (jt != null)
                        {
                            string t = jt.Value<string>();
                            Dictionary<string, string> dict = Utils.SplitStringToKeyValues(t, '&', '=');
                            cipherSignatureEncrypted = WebUtility.UrlDecode(dict["s"]);
                            cipherEncryptedUrl = WebUtility.UrlDecode(dict["url"]);
                            isCiphered = true;
                        }
                        else
                        {
                            url = jFormat.Value<string>("url");
                        }

                        YouTubeMediaTrack video = new YouTubeMediaTrackVideo(
                            formatId, videoWidth, videoHeight, videoFrameRate, bitrate, averageBitrate,
                            lastModified, contentLength, quality, qualityLabel, approxDurationMs,
                            projectionType, url, cipherSignatureEncrypted, cipherEncryptedUrl,
                            mimeType, mimeExt, mimeCodecs, fileExtension,
                            isDash, isHls, isCiphered, dashManifestUrl, null, null);
                        resList.AddLast(video);
                    }
                    else if (mimeType.Contains("audio"))
                    {
                        int formatId = jFormat.Value<int>("itag");
                        ParseMime(mimeType, out string mimeCodecs, out string mimeExt);
                        string fileExtension = !string.IsNullOrEmpty(mimeExt) && !string.IsNullOrWhiteSpace(mimeExt) ?
                            (mimeExt.ToLower() == "mp4" ? "m4a" : "weba") : "dat";
                        int bitrate = jFormat.Value<int>("bitrate");
                        int averageBitrate = jFormat.Value<int>("averageBitrate");
                        string quality = jFormat.Value<string>("quality");
                        string qualityLabel = jFormat.Value<string>("qualityLabel");
                        string lastModified = jFormat.Value<string>("lastModified");
                        long contentLength = -1L;
                        JToken jt = jFormat.Value<JToken>("contentLength");
                        if (jt != null)
                        {
                            string contentLengthString = jt.Value<string>();
                            if (!long.TryParse(contentLengthString, out contentLength))
                            {
                                contentLength = -1;
                            }
                        }
                        int approxDurationMs = -1;
                        string cipherSignatureEncrypted = null;
                        string cipherEncryptedUrl = null;
                        bool isCiphered = false;
                        string url = null;
                        jt = jFormat.Value<JToken>("signatureCipher");
                        if (jt != null)
                        {
                            string t = jt.Value<string>();
                            Dictionary<string, string> dict = Utils.SplitStringToKeyValues(t, '&', '=');
                            cipherSignatureEncrypted = WebUtility.UrlDecode(dict["s"]);
                            cipherEncryptedUrl = WebUtility.UrlDecode(dict["url"]);
                            isCiphered = true;
                        }
                        else
                        {
                            url = jFormat.Value<string>("url");
                        }

                        string audioQuality = jFormat.Value<string>("audioQuality");
                        int audioSampleRate = int.Parse(jFormat.Value<string>("audioSampleRate"));
                        int audioChannelCount = jFormat.Value<int>("audioChannels");
                        double loudnessDb = jFormat.Value<double>("loudnessDb");

                        YouTubeMediaTrack audio = new YouTubeMediaTrackAudio(
                            formatId, bitrate, averageBitrate, lastModified, contentLength,
                            quality, qualityLabel, audioQuality, audioSampleRate,
                            audioChannelCount, loudnessDb, approxDurationMs, url,
                            cipherSignatureEncrypted, cipherEncryptedUrl,
                            mimeType, mimeExt, mimeCodecs, fileExtension,
                            isDash, isHls, isCiphered, dashManifestUrl, null, null);
                        resList.AddLast(audio);
                    }
                }
            }

            JArray jaFormats = jStreamingData.Value<JArray>("formats");
            if (jaFormats != null)
            {
                foreach (JObject jFormat in jaFormats)
                {
                    string mimeType = jFormat.Value<string>("mimeType");
                    ParseMime(mimeType, out string mimeCodecs, out string mimeExt);
                    string fileExtension = !string.IsNullOrEmpty(mimeExt) && !string.IsNullOrWhiteSpace(mimeExt) ?
                        mimeExt.ToLower() : "mp4"; //It's possible to be wrong for some videos.
                    int formatId = jFormat.Value<int>("itag");
                    int bitrate = jFormat.Value<int>("bitrate");
                    int averageBitrate = jFormat.Value<int>("averageBitrate");
                    int videoWidth = jFormat.Value<int>("width");
                    int videoHeight = jFormat.Value<int>("height");
                    string quality = jFormat.Value<string>("quality");
                    string qualityLabel = jFormat.Value<string>("qualityLabel");
                    int videoFrameRate = jFormat.Value<int>("fps");
                    string projectionType = jFormat.Value<string>("projectionType");
                    string lastModified = jFormat.Value<string>("lastModified");
                    long contentLength = -1L;
                    JToken jt = jFormat.Value<JToken>("contentLength");
                    if (jt != null)
                    {
                        string contentLengthString = jt.Value<string>();
                        if (!long.TryParse(contentLengthString, out contentLength))
                        {
                            contentLength = -1;
                        }
                    }
                    string audioQuality = jFormat.Value<string>("audioQuality");
                    jt = jFormat.Value<JToken>("audioSampleRate");
                    int audioSampleRate = jt != null ? int.Parse(jt.Value<string>()) : -1;
                    jt = jFormat.Value<JToken>("audioChannels");
                    int audioChannelCount = jt != null ? int.Parse(jt.Value<string>()) : -1;
                    jt = jFormat.Value<JToken>("approxDurationMs");
                    int approxDurationMs = jt != null ? int.Parse(jt.Value<string>()) : -1;
                    string cipherSignatureEncrypted = null;
                    string cipherEncryptedUrl = null;
                    bool isCiphered = false;
                    string url = null;
                    jt = jFormat.Value<JToken>("signatureCipher");
                    if (jt != null)
                    {
                        string t = jt.Value<string>();
                        Dictionary<string, string> dict = Utils.SplitStringToKeyValues(t, '&', '=');
                        cipherSignatureEncrypted = WebUtility.UrlDecode(dict["s"]);
                        cipherEncryptedUrl = WebUtility.UrlDecode(dict["url"]);
                        isCiphered = true;
                    }
                    else
                    {
                        url = jFormat.Value<string>("url");
                    }

                    YouTubeMediaTrack video = new YouTubeMediaTrackContainer(
                        formatId, videoWidth, videoHeight, videoFrameRate, bitrate, averageBitrate,
                        lastModified, contentLength, quality, qualityLabel,
                        audioQuality, audioSampleRate, audioChannelCount, approxDurationMs,
                        projectionType, url, cipherSignatureEncrypted, cipherEncryptedUrl,
                        mimeType, mimeExt, mimeCodecs, fileExtension,
                        isDash, isHls, isCiphered, dashManifestUrl, null, null);
                    resList.AddLast(video);
                }
            }
            return resList;
        }

        private static void ParseMime(string mime, out string codecs, out string mimeExt)
        {
            string[] t = mime.Split(';', '/', '=');
            if (t.Length > 3)
            {
                codecs = !string.IsNullOrEmpty(t[3]) && !string.IsNullOrWhiteSpace(t[3]) ?
                    t[3].Replace("\"", string.Empty) : null;
            }
            else
            {
                codecs = null;
            }
            mimeExt = t.Length > 1 ? t[1] : null;
        }

        private static LinkedList<YouTubeMediaTrack> ParseDashManifest(string dashManifest, string dashManifestUrl)
        {
            if (string.IsNullOrEmpty(dashManifest) || string.IsNullOrWhiteSpace(dashManifest))
            {
                return null;
            }

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(dashManifest);
            XmlNode nodePeriod = xml.DocumentElement.SelectSingleNode("//*[local-name()='Period']");
            if (nodePeriod == null)
            {
                return null;
            }

            bool isDash = true;
            LinkedList<YouTubeMediaTrack> resList = new LinkedList<YouTubeMediaTrack>();

            foreach (XmlNode nodeAdaptationSet in nodePeriod)
            {
                if (nodeAdaptationSet == null || nodeAdaptationSet.Name != "AdaptationSet")
                {
                    continue;
                }
                XmlAttribute attrMimeType = nodeAdaptationSet.Attributes["mimeType"];
                if (attrMimeType == null)
                {
                    continue;
                }
                
                string mimeType = attrMimeType.Value;
                string[] mimeTypeSplitted = mimeType.Split('/');
                if (mimeTypeSplitted == null || mimeTypeSplitted.Length < 2)
                {
                    continue;
                }
                string splittedMimeTypeLowecased = mimeTypeSplitted[0].ToLower();
                if (splittedMimeTypeLowecased == "video")
                {
                    foreach (XmlNode node in nodeAdaptationSet)
                    {
                        if (node.Name == "Representation")
                        {
                            string formatIdString = node.Attributes["id"]?.Value;
                            if (!int.TryParse(formatIdString, out int formatId))
                            {
                                formatId = 0;
                            }
                            string videoWidthString = node.Attributes["width"]?.Value;
                            if (!int.TryParse(videoWidthString, out int videoWidth))
                            {
                                videoWidth = 0;
                            }
                            string videoHeightString = node.Attributes["height"]?.Value;
                            if (!int.TryParse(videoHeightString, out int videoHeight))
                            {
                                videoHeight = 0;
                            }
                            string videoFrameRateString = node.Attributes["frameRate"]?.Value;
                            if (!int.TryParse(videoFrameRateString, out int videoFrameRate))
                            {
                                videoFrameRate = 0;
                            }
                            string videoBitrateString = node.Attributes["bandwidth"]?.Value;
                            if (!int.TryParse(videoBitrateString, out int videoBitrate))
                            {
                                videoBitrate = 0;
                            }
                            string videoCodecs = node.Attributes["codecs"]?.Value;

                            string mimeExtLowerCased = mimeTypeSplitted[1].ToLower();
                            string fileExtension = mimeExtLowerCased == "mp4" ? "m4v" : mimeExtLowerCased;

                            XmlNode nodeBaseUrl = node.SelectSingleNode("./*[local-name()='BaseURL']");
                            if (nodeBaseUrl == null)
                            {
                                System.Diagnostics.Debug.WriteLine("ERROR! The \"nodeBaseUrl\" is NULL!");
                                continue;
                            }
                            string baseUrl = nodeBaseUrl.InnerText;

                            XmlNode nodeSegmentList = node.SelectSingleNode("./*[local-name()='SegmentList']");
                            if (nodeSegmentList == null)
                            {
                                System.Diagnostics.Debug.WriteLine("ERROR! The \"nodeSegmentList\" is NULL!");
                                continue;
                            }

                            List<string> dashChunkUrls = new List<string>();

                            XmlNode nodeInitialization = nodeSegmentList.SelectSingleNode("./*[local-name()='Initialization']");
                            if (nodeInitialization != null)
                            {
                                XmlAttribute attrSourceUrl = nodeInitialization.Attributes["sourceURL"];
                                if (attrSourceUrl != null)
                                {
                                    dashChunkUrls.Add(baseUrl + attrSourceUrl.Value);
                                }
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("Warning! The \"nodeInitialization\" is NULL! The first track chunk may be lost!");
                            }
                            foreach (XmlNode nodeSegment in nodeSegmentList)
                            {
                                if (nodeSegment.Name == "SegmentURL")
                                {
                                    string segmentUrl = baseUrl + nodeSegment.Attributes["media"]?.Value;
                                    dashChunkUrls.Add(segmentUrl);
                                }
                            }

                            YouTubeMediaTrack video = new YouTubeMediaTrackVideo(
                                formatId, videoWidth, videoHeight, videoFrameRate, videoBitrate, videoBitrate,
                                null, -1, null, null, -1, null, null, null, null,
                                mimeType, mimeExtLowerCased, videoCodecs, fileExtension,
                                isDash, false, false, dashManifestUrl, dashChunkUrls, null);
                            resList.AddLast(video);
                        }
                    }
                }
                else if (splittedMimeTypeLowecased == "audio")
                {
                    foreach (XmlNode node in nodeAdaptationSet)
                    {
                        if (node.Name == "Representation")
                        {
                            string formatIdString = node.Attributes["id"]?.Value;
                            if (!int.TryParse(formatIdString, out int formatId))
                            {
                                formatId = 0;
                            }
                            string audioSamplingRateString = node.Attributes["audioSamplingRate"]?.Value;
                            if (!int.TryParse(audioSamplingRateString, out int audioSampleRate))
                            {
                                audioSampleRate = 0;
                            }
                            string audioBitrateString = node.Attributes["bandwidth"]?.Value;
                            if (!int.TryParse(audioBitrateString, out int audioBitrate))
                            {
                                audioBitrate = 0;
                            }
                            string audioCodecs = node.Attributes["codecs"]?.Value;

                            int audioChannelCount;
                            XmlNode nodeAudio = FindChildNodeByName(node, "AudioChannelConfiguration");
                            if (nodeAudio != null)
                            {
                                string val = nodeAudio.Attributes["value"]?.Value;
                                if (!int.TryParse(val, out audioChannelCount))
                                {
                                    audioChannelCount = -1;
                                }
                            }
                            else
                            {
                                audioChannelCount = -1;
                            }

                            string mimeExtLowerCased = mimeTypeSplitted[1].ToLower();
                            string fileExtension;
                            if (mimeExtLowerCased == "mp4")
                            {
                                fileExtension = "m4a";
                            }
                            else if (mimeExtLowerCased == "webm")
                            {
                                fileExtension = "weba";
                            }
                            else
                            {
                                fileExtension = mimeExtLowerCased;
                            }

                            XmlNode nodeBaseUrl = node.SelectSingleNode("./*[local-name()='BaseURL']");
                            if (nodeBaseUrl == null)
                            {
                                System.Diagnostics.Debug.WriteLine("ERROR! The \"nodeBaseUrl\" is NULL!");
                                continue;
                            }
                            string baseUrl = nodeBaseUrl.InnerText;

                            XmlNode nodeSegmentList = node.SelectSingleNode("./*[local-name()='SegmentList']");
                            if (nodeSegmentList == null)
                            {
                                System.Diagnostics.Debug.WriteLine("ERROR! The \"nodeSegmentList\" is NULL!");
                                continue;
                            }

                            List<string> dashChunkUrls = new List<string>();

                            XmlNode nodeInitialization = nodeSegmentList.SelectSingleNode("./*[local-name()='Initialization']");
                            if (nodeInitialization != null)
                            {
                                XmlAttribute attrSourceUrl = nodeInitialization.Attributes["sourceURL"];
                                if (attrSourceUrl != null)
                                {
                                    dashChunkUrls.Add(baseUrl + attrSourceUrl.Value);
                                }
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("Warning! The \"nodeInitialization\" is NULL! The first track chunk may be lost!");
                            }

                            foreach (XmlNode nodeSegment in nodeSegmentList)
                            {
                                if (nodeSegment.Name == "SegmentURL")
                                {
                                    string segmentUrl = baseUrl + nodeSegment.Attributes["media"]?.Value;
                                    dashChunkUrls.Add(segmentUrl);
                                }
                            }

                            YouTubeMediaTrack audio = new YouTubeMediaTrackAudio(
                                formatId, audioBitrate, audioBitrate, null, -1, null, null, null,
                                audioSampleRate, audioChannelCount, 0.0, -1, null, null, null,
                                mimeType, mimeExtLowerCased, audioCodecs, fileExtension,
                                isDash, false, false, dashManifestUrl, dashChunkUrls, null);
                            resList.AddLast(audio);
                        }
                    }
                }
            }

            return resList;
        }

        private static XmlNode FindChildNodeByName(XmlNode parentNode, string nodeName)
        {
            foreach (XmlNode node in parentNode.ChildNodes)
            {
                if (node.Name == nodeName)
                {
                    return node;
                }
            }
            return null;
        }
    }
}
