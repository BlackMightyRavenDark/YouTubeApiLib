using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using static YouTubeApiLib.Utils;

namespace YouTubeApiLib
{
	public class StreamingData
	{
		public string RawData { get; }
		public VideoInfoGettingMethod DataGettingMethod { get; }

		private JObject _parsedData = null;

		public StreamingData(string rawData, VideoInfoGettingMethod dataGettingMethod)
		{
			RawData = rawData;
			DataGettingMethod = dataGettingMethod;
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
