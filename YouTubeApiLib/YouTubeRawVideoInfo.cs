﻿using Newtonsoft.Json.Linq;
using static YouTubeApiLib.Utils;

namespace YouTubeApiLib
{
	public class YouTubeRawVideoInfo
	{
		public string RawData { get; private set; }

		/// <summary>
		/// The YouTube client which used for getting info.
		/// </summary>
		public IYouTubeClient Client { get; }

		public YouTubeMediaTrackUrlDecryptionData UrlDecryptionData { get; }

		public YouTubeVideoPlayabilityStatus PlayabilityStatus => ExtractPlayabilityStatus();
		public YouTubeStreamingDataResult StreamingData => ExtractStreamingData();
		public YouTubeVideoDetails VideoDetails => ExtractVideoDetails();
		public JObject Microformat => ExtractMicroformat();

		private JObject _parsedData = null;

		public YouTubeRawVideoInfo(string rawData, IYouTubeClient client, YouTubeMediaTrackUrlDecryptionData urlDecryptionData)
		{
			RawData = rawData;
			Client = client;
			UrlDecryptionData = urlDecryptionData;
		}

		public static YouTubeRawVideoInfoResult Get(YouTubeVideoId videoId, IYouTubeClient client)
		{
			return GetRawVideoInfo(videoId.Id, client);
		}

		public static YouTubeRawVideoInfoResult Get(string videoId, IYouTubeClient client)
		{
			int errorCode = client.GetRawVideoInfo(videoId, out YouTubeRawVideoInfo rawVideoInfo, out _);
			return new YouTubeRawVideoInfoResult(rawVideoInfo, errorCode);
		}

		public static YouTubeRawVideoInfoResult Get(YouTubeVideoId videoId)
		{
			IYouTubeClient client = YouTubeApi.GetYouTubeClient(YouTubeApi.GetDefaultYouTubeClientId());
			return Get(videoId.Id, client);
		}

		public static YouTubeRawVideoInfoResult Get(string videoId)
		{
			IYouTubeClient client = new YouTubeClientIos(null);
			return Get(videoId, client);
		}

		public YouTubeSimplifiedVideoInfoResult Simplify(YouTubeStreamingData customStreamingData = null)
		{
			return SimplifyRawVideoInfo(this, customStreamingData);
		}

		private YouTubeVideoPlayabilityStatus ExtractPlayabilityStatus()
		{
			if (_parsedData == null) { _parsedData = TryParseJson(RawData); }
			JObject jPlayabilityStatus = _parsedData?.Value<JObject>("playabilityStatus");
			return jPlayabilityStatus != null ? YouTubeVideoPlayabilityStatus.Parse(jPlayabilityStatus) : null;
		}

		private YouTubeStreamingDataResult ExtractStreamingData()
		{
			if (_parsedData == null) { _parsedData = TryParseJson(RawData); }
			if (_parsedData != null)
			{
				JObject jStreamingData = _parsedData.Value<JObject>("streamingData");
				if (jStreamingData != null)
				{
					YouTubeStreamingData streamingData = new YouTubeStreamingData(
						jStreamingData.ToString(), Client, UrlDecryptionData);
					return new YouTubeStreamingDataResult(streamingData, 200);
				}
			}

			return new YouTubeStreamingDataResult(null, 404);
		}

		private YouTubeVideoDetails ExtractVideoDetails()
		{
			if (_parsedData == null) { _parsedData = TryParseJson(RawData); }
			JObject j = _parsedData?.Value<JObject>("videoDetails");
			return j != null ? new YouTubeVideoDetails(j.ToString(), Client) : null;
		}

		private JObject ExtractMicroformat()
		{
			if (_parsedData == null) { _parsedData = TryParseJson(RawData); }
			return _parsedData?.Value<JObject>("microformat");
		}

		public YouTubeVideo ToVideo()
		{
			return MakeYouTubeVideo(this);
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
