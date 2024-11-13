﻿using System.Collections.Specialized;
using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
	public interface IYouTubeClient
	{
		string DisplayName { get; }
		JObject GenerateRequestBody(string videoId, YouTubeConfig youTubeConfig = null);
		NameValueCollection GenerateRequestHeaders(string videoId, YouTubeConfig youTubeConfig = null);
		YouTubeRawVideoInfoResult GetRawVideoInfo(YouTubeVideoId videoId, out string errorMessage);
		int GetRawVideoInfo(string videoId, out YouTubeRawVideoInfo rawVideoInfo, out string errorMessage);
	}
}
