using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
	internal class TabPageParserVideo1 : ITabPageParser
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
			j = j.Value<JObject>("sectionListRenderer");
			if (j == null)
			{
				System.Diagnostics.Debug.WriteLine($"{this}: \"sectionListRenderer\" not found!");
				return null;
			}
			JArray ja = j.Value<JArray>("contents");
			if (ja == null || ja.Count == 0)
			{
				System.Diagnostics.Debug.WriteLine($"{this}: \"contents\" not found or empty!");
				return null;
			}
			j = (ja[0] as JObject).Value<JObject>("itemSectionRenderer");
			if (j == null)
			{
				System.Diagnostics.Debug.WriteLine($"{this}: \"itemSectionRenderer\" not found!");
				return null;
			}
			ja = j.Value<JArray>("contents");
			if (ja == null || ja.Count == 0)
			{
				System.Diagnostics.Debug.WriteLine($"{this}: \"contents\" not found or empty!");
				return null;
			}
			j = (ja[0] as JObject).Value<JObject>("gridRenderer");
			if (j == null)
			{
				System.Diagnostics.Debug.WriteLine($"{this}: \"gridRenderer\" not found!");
				return null;
			}
			ja = j.Value<JArray>("items");
			if (ja == null || ja.Count == 0)
			{
				System.Diagnostics.Debug.WriteLine($"{this}: \"items\" not found or empty!");
				return null;
			}
			return ja;
		}

		public override string ToString()
		{
			return "TabPageParserVideo1";
		}
	}
}
