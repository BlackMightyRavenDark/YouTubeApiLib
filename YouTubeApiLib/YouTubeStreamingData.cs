using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using static YouTubeApiLib.Utils;

namespace YouTubeApiLib
{
	public class YouTubeStreamingData
	{
		public string RawData { get; }
		public YouTubeVideoInfoGettingMethod DataGettingMethod { get; }

		private JObject _parsedData = null;

		public YouTubeStreamingData(string rawData, YouTubeVideoInfoGettingMethod dataGettingMethod)
		{
			RawData = rawData;
			DataGettingMethod = dataGettingMethod;
		}

		public static YouTubeStreamingDataResult Get(YouTubeVideoId videoId, YouTubeVideoInfoGettingMethod method)
		{
			return Get(videoId.Id, method);
		}

		public static YouTubeStreamingDataResult Get(string videoId, YouTubeVideoInfoGettingMethod method)
		{
			return GetStreamingData(videoId, method);
		}

		public static YouTubeStreamingDataResult Get(YouTubeVideoId videoId)
		{
			return Get(videoId, YouTubeApi.defaultVideoInfoGettingMethod);
		}

		public static YouTubeStreamingDataResult Get(string videoId)
		{
			return Get(videoId, YouTubeApi.defaultVideoInfoGettingMethod);
		}

		public LinkedList<YouTubeMediaTrack> Parse()
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
	}
}
