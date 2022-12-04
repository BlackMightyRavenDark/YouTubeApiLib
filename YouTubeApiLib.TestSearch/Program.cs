using System;

namespace YouTubeApiLib.TestSearch
{
    internal class Program
    {
        static void Main(string[] args)
        {
            YouTubeApi api = new YouTubeApi();
            YouTubeApiV1SearchResults searchResults =
                api.Search("coding train", null, YouTubeApiV1SearchResultFilters.Video);
            if (searchResults != null)
            {
                // A raw info may be too long for console output!
                //TODO: Write the mighty search results parsers.
                Console.WriteLine(searchResults.RawData.ToString());
            }
            Console.ReadLine();
        }
    }
}
