using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
	internal class TabPageParserVideo2 : ITabPageParser
	{
		public JArray FindGridItems(JObject tabPage)
		{
			JObject j = tabPage.Value<JObject>("tabRenderer");
			if (j == null)
			{
				System.Diagnostics.Debug.WriteLine($"{this}: \"tabRenderer\" not found!");
				return null;
			}
			j = j.Value<JObject>("content");
			if (j == null)
			{
				System.Diagnostics.Debug.WriteLine($"{this}: \"content\" not found!");
				return null;
			}
			j = j.Value<JObject>("richGridRenderer");
			if (j == null)
			{
				System.Diagnostics.Debug.WriteLine($"{this}: \"richGridRenderer\" not found!");
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
			return "TabPageParserVideo2";
		}
	}
}
