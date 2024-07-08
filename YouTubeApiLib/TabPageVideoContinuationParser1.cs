using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
	internal class TabPageVideoContinuationParser1 : ITabPageParser
	{
		public JArray FindGridItems(JObject tabPage)
		{
			JObject j = tabPage.Value<JObject>("continuationContents");
			if (j == null)
			{
				System.Diagnostics.Debug.WriteLine($"{this}: \"continuationContents\" not found!");
				return null;
			}
			j = j.Value<JObject>("richGridContinuation");
			if (j == null)
			{
				System.Diagnostics.Debug.WriteLine($"{this}: \"richGridContinuation\" not found!");
				return null;
			}
			JArray ja = j.Value<JArray>("contents");
			if (ja == null || ja.Count == 0)
			{
				System.Diagnostics.Debug.WriteLine($"{this}: \"contents\" not found or empty!");
				return null;
			}

			return ja;
		}

		public override string ToString()
		{
			return "TabPageVideoContinuationParser1";
		}
	}
}
