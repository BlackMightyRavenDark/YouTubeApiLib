using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
	internal interface IYouTubeChannelTabPageParser
	{
		JArray FindGridItems(JObject tabPage);
	}
}
