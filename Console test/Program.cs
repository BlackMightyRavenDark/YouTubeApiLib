using System;
using YouTube_API;
using static YouTube_API.Utils;

namespace Console_test
{
    class Program
    {
        static void Main(string[] args)
        {
            //Magical Underwater World 4K | 2160p | 7304448660 bytes
            string videoUrl = "https://www.youtube.com/watch?v=7szcXCT-Oqw";

            //Trick or treat, kingdom hearts, and beat saber. | 18+ | 1080p | 197233776 bytes
            //string videoUrl = "https://www.youtube.com/watch?v=pQNRrnk63MQ";
            
            string videoId = ExtractVideoIdFromUrl(videoUrl);

            Console.WriteLine($"Video URL: {videoUrl}");

            if (!string.IsNullOrEmpty(videoId))
            {
                YouTubeApi api = new YouTubeApi();
                YouTubeVideo video = api.GetVideo(videoId);
                if (video.Status.IsPlayable)
                {
                    Console.WriteLine($"Title: {video.Title}");
                    Console.WriteLine($"ID: {video.Id}");
                    Console.WriteLine($"URL: {video.Url}");
                    Console.WriteLine($"Uploaded: {video.DateUploaded:yyyy.MM.dd}");
                    Console.WriteLine($"Published: {video.DatePublished:yyyy.MM.dd}");
                    Console.WriteLine($"Length: {video.Length}");
                    Console.WriteLine($"Channel title: {video.OwnerChannelTitle}");
                    Console.WriteLine($"Channel ID: {video.OwnerChannelId}");
                    Console.WriteLine($"Description: {video.Description}");
                    Console.WriteLine($"View count: {video.ViewCount}");
                    Console.WriteLine($"Category: {video.Category}");
                    Console.WriteLine($"Private: {video.IsPrivate}");
                    Console.WriteLine($"Unlisted: {video.IsUnlisted}");
                    Console.WriteLine($"Family safe: {video.IsFamilySafe}");
                    Console.WriteLine($"Live content: {video.IsLiveContent}");
                    Console.WriteLine($"Raw info:\n{video.RawInfo}");
                }
                else
                {
                    Console.WriteLine($"Video ID: {videoId}");
                    Console.WriteLine(video.Status.Reason);
                }
            }
            else
            {
                Console.WriteLine("Video ID: <ERROR>");
            }
            Console.ReadLine();
        }
    }
}
