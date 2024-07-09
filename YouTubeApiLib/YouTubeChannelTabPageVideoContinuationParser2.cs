using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
	internal class YouTubeChannelTabPageVideoContinuationParser2 : IYouTubeChannelTabPageParser
	{
		public JArray FindGridItems(JObject tabPage)
		{
			JArray ja = tabPage.Value<JArray>("onResponseReceivedActions");
			if (ja == null || ja.Count == 0)
			{
				System.Diagnostics.Debug.WriteLine($"{this}: \"onResponseReceivedActions\" not found or empty!");
				return null;
			}
			JObject j = (ja[0] as JObject).Value<JObject>("appendContinuationItemsAction");
			if (j == null)
			{
				System.Diagnostics.Debug.WriteLine($"{this}: \"appendContinuationItemsAction\" not found!");
				return null;
			}
			ja = j.Value<JArray>("continuationItems");
			if (ja == null || ja.Count == 0)
			{
				System.Diagnostics.Debug.WriteLine($"{this}: \"continuationItems\" not found or empty!");
				return null;
			}

			return ja;
		}

		public override string ToString()
		{
			return "TabPageVideoContinuationParser2";
		}
	}
}
