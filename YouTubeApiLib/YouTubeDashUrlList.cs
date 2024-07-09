using System.Collections.Generic;

namespace YouTubeApiLib
{
	public class YouTubeDashUrlList
	{
		public string BaseUrl { get; }
		private readonly List<string> _partialUrlList;

		public int Count => _partialUrlList != null ? _partialUrlList.Count : -1;
		public string this[int id] => BaseUrl + _partialUrlList[id];

		public YouTubeDashUrlList(string baseUrl, List<string> partialUrlList)
		{
			BaseUrl = baseUrl;
			_partialUrlList = partialUrlList;
		}

		public string GetPartialUrl(int id)
		{
			return id >= 0 && id < Count ? _partialUrlList[id] : null;
		}
	}
}
