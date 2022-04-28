using System;
using static YouTube_API.Utils;

namespace Console_test
{
    class Program
    {
        static void Main(string[] args)
        {
            //Magical Underwater World 4K | 2160p | 7304448660 bytes
            string videoUrl = "https://www.youtube.com/watch?v=7szcXCT-Oqw";

            string videoId = ExtractVideoIdFromUrl(videoUrl);

            Console.WriteLine($"Video URL: {videoUrl}");

            string t = $"Video ID: {(string.IsNullOrEmpty(videoId) ? "<ERROR>" : videoId)}";
            Console.WriteLine(t);

            Console.ReadLine();
        }
    }
}
