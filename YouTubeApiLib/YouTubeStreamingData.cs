﻿using Newtonsoft.Json.Linq;
using static YouTubeApiLib.Utils;

namespace YouTubeApiLib
{
	public class YouTubeStreamingData
	{
		public string RawData { get; private set; }

		/// <summary>
		/// The internal YouTube client used for getting this data.
		/// </summary>
		public IYouTubeClient Client { get; }

		public YouTubeMediaTrackUrlDecryptionData UrlDecryptionData { get; }

		private JObject _parsedData = null;

		public YouTubeStreamingData(string rawData, IYouTubeClient client,
			YouTubeMediaTrackUrlDecryptionData urlDecryptionData)
		{
			RawData = rawData;
			Client = client;
			UrlDecryptionData = urlDecryptionData;
		}

		public static YouTubeStreamingDataResult Get(YouTubeVideoId videoId, IYouTubeClient client)
		{
			return Get(videoId.Id, client);
		}

		public static YouTubeStreamingDataResult Get(string videoId, IYouTubeClient client)
		{
			int errorCode = client.GetRawVideoInfo(videoId, out YouTubeRawVideoInfo rawVideoInfo, out _);
			return errorCode == 200 ? rawVideoInfo.StreamingData :
				new YouTubeStreamingDataResult(null, errorCode);
		}

		public static YouTubeStreamingDataResult Get(YouTubeVideoId videoId)
		{
			return Get(videoId.Id);
		}

		public static YouTubeStreamingDataResult Get(string videoId)
		{
			IYouTubeClient client = YouTubeApi.GetYouTubeClient(YouTubeApi.GetDefaultYouTubeClientId());
			return client != null ? Get(videoId, client) :
				new YouTubeStreamingDataResult(null, 400);
		}

		public YouTubeMediaFormatList Parse()
		{
			return YouTubeMediaFormatsParser.Parse(this);
		}

		public JArray GetFormats()
		{
			if (_parsedData == null) { _parsedData = TryParseJson(RawData); }
			return _parsedData?.Value<JArray>("formats");
		}

		public JArray GetAdaptiveFormats()
		{
			if (_parsedData == null) { _parsedData = TryParseJson(RawData); }
			return _parsedData?.Value<JArray>("adaptiveFormats");
		}

		public string GetDashManifestUrl()
		{
			if (_parsedData == null) { _parsedData = TryParseJson(RawData); }
			return _parsedData?.Value<string>("dashManifestUrl");
		}

		public string GetHlsManifestUrl()
		{
			if (_parsedData == null) { _parsedData = TryParseJson(RawData); }
			return _parsedData?.Value<string>("hlsManifestUrl");
		}

		public void FormatRawData()
		{
			JObject j = TryParseJson(RawData);
			if (j != null)
			{
				if (_parsedData == null) { _parsedData = j; }
				RawData = j.ToString();
			}
		}

		public override string ToString()
		{
			return RawData ?? "null";
		}
	}
}
