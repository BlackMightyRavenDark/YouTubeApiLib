using Newtonsoft.Json.Linq;
using static YouTubeApiLib.Utils;

namespace YouTubeApiLib
{
	public class RawVideoInfo
	{
		public string RawData { get; }
		public VideoInfoGettingMethod DataGettingMethod { get; }
		public YouTubeVideoPlayabilityStatus PlayabilityStatus => ExtractPlayabilityStatus();
		public YouTubeStreamingData StreamingData => ExtractStreamingData();
		public JObject VideoDetails => ExtractVideoDetails();
		public JObject Microformat => ExtractMicroformat();

		private JObject _parsedData = null;

		public RawVideoInfo(string rawData, VideoInfoGettingMethod dataGettingMethod)
		{
			RawData = rawData;
			DataGettingMethod = dataGettingMethod;
		}

		private YouTubeVideoPlayabilityStatus ExtractPlayabilityStatus()
		{
			if (_parsedData == null) { _parsedData = TryParseJson(RawData); }
			JObject jPlayabilityStatus = _parsedData?.Value<JObject>("playabilityStatus");
			return jPlayabilityStatus != null ? YouTubeVideoPlayabilityStatus.Parse(jPlayabilityStatus) : null;
		}

		private YouTubeStreamingData ExtractStreamingData()
		{
			if (_parsedData == null) { _parsedData = TryParseJson(RawData); }
			if (_parsedData != null)
			{
				JObject jStreamingData = _parsedData.Value<JObject>("streamingData");
				if (jStreamingData != null)
				{
					return new YouTubeStreamingData(jStreamingData.ToString(), DataGettingMethod);
				}
			}
			return null;
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
