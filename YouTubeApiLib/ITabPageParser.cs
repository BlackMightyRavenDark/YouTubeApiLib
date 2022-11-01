using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
    internal interface ITabPageParser
    {
        JArray FindGridItems(JObject tabPage);
    }
}
