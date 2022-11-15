﻿using System.Collections.Generic;
using System.Net;
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

                    //TODO: Implement other track types detection
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
                        bool isContainer = false;
                        string audioQuality = null;
                        int audioSampleRate = -1;
                        int audioChannelCount = -1;
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

                        YouTubeMediaTrack video = new YouTubeVideoTrack(
                            formatId, videoWidth, videoHeight, videoFrameRate, bitrate, averageBitrate,
                            lastModified, contentLength, quality, qualityLabel,
                            audioQuality, audioSampleRate, audioChannelCount, approxDurationMs,
                            projectionType, url, cipherSignatureEncrypted, cipherEncryptedUrl,
                            mimeType, mimeExt, mimeCodecs, fileExtension,
                            isContainer, isDash, isHls, isCiphered, null, null);
                        resList.AddLast(video);
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
                        (mimeExt.ToLower() == "mp4" ? "m4v" : "webm") : "dat";
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
                    bool isContainer = true;

                    YouTubeMediaTrack video = new YouTubeVideoTrack(
                        formatId, videoWidth, videoHeight, videoFrameRate, bitrate, averageBitrate,
                        lastModified, contentLength, quality, qualityLabel,
                        audioQuality, audioSampleRate, audioChannelCount, approxDurationMs,
                        projectionType, url, cipherSignatureEncrypted, cipherEncryptedUrl,
                        mimeType, mimeExt, mimeCodecs, fileExtension,
                        isContainer, isDash, isHls, isCiphered, null, null);
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
    }
}
