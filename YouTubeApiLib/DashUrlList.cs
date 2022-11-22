using System.Collections.Generic;

namespace YouTubeApiLib
{
    public class DashUrlList
    {
        private readonly string _BaseUrl;
        private readonly List<string> _PartialUrlList;

        public int Count => _PartialUrlList != null ? _PartialUrlList.Count : -1;
        public string this[int id] { get { return _BaseUrl + _PartialUrlList[id]; } }

        public DashUrlList(string baseUrl, List<string> partialUrlList)
        {
            _BaseUrl = baseUrl;
            _PartialUrlList = partialUrlList;
        }
    }
}
