using Newtonsoft.Json.Linq;
using static YouTubeApiLib.Utils;

namespace YouTubeApiLib
{
	public class YouTubeRawVideoInfo
	{
		public string RawData { get; }
		public YouTubeVideoInfoGettingMethod DataGettingMethod { get; }
		public YouTubeVideoPlayabilityStatus PlayabilityStatus => ExtractPlayabilityStatus();
		public YouTubeStreamingDataResult StreamingData => ExtractStreamingData();
		public JObject VideoDetails => ExtractVideoDetails();
		public JObject Microformat => ExtractMicroformat();

		private JObject _parsedData = null;

		public YouTubeRawVideoInfo(string rawData, YouTubeVideoInfoGettingMethod dataGettingMethod)
		{
			RawData = rawData;
			DataGettingMethod = dataGettingMethod;
		}

		public static YouTubeRawVideoInfoResult Get(YouTubeVideoId videoId, YouTubeVideoInfoGettingMethod method)
		{
			return GetRawVideoInfo(videoId.Id, method);
		}

		public static YouTubeRawVideoInfoResult Get(string videoId, YouTubeVideoInfoGettingMethod method)
		{
			YouTubeVideoId youTubeVideoId = new YouTubeVideoId(videoId);
			return Get(youTubeVideoId, method);
		}

		public static YouTubeRawVideoInfoResult Get(YouTubeVideoId videoId)
		{
			return Get(videoId.Id, YouTubeApi.defaultVideoInfoGettingMethod);
		}

		public static YouTubeRawVideoInfoResult Get(string videoId)
		{
			YouTubeVideoId youTubeVideoId = new YouTubeVideoId(videoId);
			return Get(youTubeVideoId, YouTubeApi.defaultVideoInfoGettingMethod);
		}

		public YouTubeSimplifiedVideoInfoResult Parse()
		{
			return ParseRawVideoInfo(this);
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
					YouTubeStreamingData streamingData = new YouTubeStreamingData(jStreamingData.ToString(), DataGettingMethod);
					return new YouTubeStreamingDataResult(streamingData, 200);
				}
			}

			return new YouTubeStreamingDataResult(null, 404);
		}

		private JObject ExtractVideoDetails()
		{
			if (_parsedData == null) { _parsedData = TryParseJson(RawData); }
			return _parsedData?.Value<JObject>("videoDetails");
		}

		private JObject ExtractMicroformat()
		{
			if (_parsedData == null) { _parsedData = TryParseJson(RawData); }
			return _parsedData?.Value<JObject>("microformat");
		}

		public override string ToString()
		{
			return RawData != null ? RawData.ToString() : "null";
		}
	}
}
