﻿using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
	public class YouTubeVideoDetails
	{
		public string RawData { get; }

		/// <summary>
		/// The internal YouTube client used for getting this data.
		/// </summary>
		public IYouTubeClient Client { get; }

		private JObject _parsedData;

		public YouTubeVideoDetails(string rawData, IYouTubeClient client)
		{
			RawData = rawData;
			Client = client;
		}

		public JObject Parse()
		{
			if (_parsedData == null) { _parsedData = Utils.TryParseJson(RawData); }
			return _parsedData;
		}
	}
}
