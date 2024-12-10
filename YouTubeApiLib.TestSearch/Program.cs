using System;

namespace YouTubeApiLib.TestSearch
{
	internal class Program
	{
		static void Main(string[] args)
		{
			const string searchQuery = "coding train";
			Console.WriteLine($"Searching for \"{searchQuery}\"... Please wait!");

			YouTubeApi api = new YouTubeApi();
			YouTubeApiV1SearchResults searchResults =
				api.Search(searchQuery, null, YouTubeApiV1SearchResultFilters.Video);
			if (searchResults == null || searchResults.RawData == null)
			{
				Console.WriteLine("Error! The search result object is NULL!");
			}
			else if (searchResults.ErrorCode != 200)
			{
				Console.WriteLine($"There is error with code {searchResults.ErrorCode}!");
			}
			else
			{
				// Warning! A raw info may be too long for console output!
				// TODO: Write the mighty search results parsers.
				Console.WriteLine(searchResults.RawData.ToString());
			}

			Console.ReadLine();
		}
	}
}
