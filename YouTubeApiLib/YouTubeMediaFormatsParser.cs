using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
	public static class YouTubeMediaFormatsParser
	{
		public static LinkedList<YouTubeMediaTrack> Parse(StreamingData streamingData)
		{
			if (streamingData == null || streamingData.RawData == null)
			{
				return null;
			}

			LinkedList<YouTubeMediaTrack> resList = new LinkedList<YouTubeMediaTrack>();

			bool isHls = false;
			string hlsManifestUrl = streamingData.GetHlsManifestUrl();
			if (!string.IsNullOrEmpty(hlsManifestUrl) && !string.IsNullOrWhiteSpace(hlsManifestUrl) &&
				Utils.DownloadString(hlsManifestUrl, out string hlsManifest) == 200)
			{
				isHls = true;
				YouTubeHlsManifestParser parser = new YouTubeHlsManifestParser(hlsManifest);
				LinkedList<YouTubeBroadcast> broadcasts = parser.Parse();
				if (broadcasts != null)
				{
					foreach (YouTubeBroadcast broadcast in broadcasts)
					{
						YouTubeMediaTrack hlsStream = new YouTubeMediaTrackVideo(broadcast, hlsManifestUrl);
						resList.AddLast(hlsStream);
					}
				}
			}

			bool isDash = false;
			string dashManifestUrl = streamingData.GetDashManifestUrl();
			if (!string.IsNullOrEmpty(dashManifestUrl) && !string.IsNullOrWhiteSpace(dashManifestUrl) &&
				Utils.DownloadString(dashManifestUrl, out string dashManifest) == 200)
			{
				isDash = true;
				YouTubeDashManifestParser parser = new YouTubeDashManifestParser(dashManifest, dashManifestUrl);
				LinkedList<YouTubeMediaTrack> dashList = parser.Parse();
				if (dashList != null)
				{
					foreach (YouTubeMediaTrack track in dashList)
					{
						resList.AddLast(track);
					}
				}
			}

			if (!isDash && !isHls)
			{
				JArray jaAdaptiveFormats = streamingData.GetAdaptiveFormats();
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
								mimeType, mimeExt, mimeCodecs, fileExtension, isCiphered);
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
							if (!int.TryParse(jFormat.Value<string>("audioSampleRate"), out int audioSampleRate))
							{
								audioSampleRate = -1;
							}
							int audioChannelCount = jFormat.Value<int>("audioChannels");
							double loudnessDb = jFormat.Value<double>("loudnessDb");

							YouTubeMediaTrack audio = new YouTubeMediaTrackAudio(
								formatId, bitrate, averageBitrate, lastModified, contentLength,
								quality, qualityLabel, audioQuality, audioSampleRate,
								audioChannelCount, loudnessDb, approxDurationMs, url,
								cipherSignatureEncrypted, cipherEncryptedUrl,
								mimeType, mimeExt, mimeCodecs, fileExtension, isCiphered);
							resList.AddLast(audio);
						}
					}
				}

				JArray jaFormats = streamingData.GetFormats();
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
							mimeType, mimeExt, mimeCodecs, fileExtension, isCiphered);
						resList.AddLast(video);
					}
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
