using System;
using static YouTubeApiLib.Utils;

namespace YouTubeApiLib.ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Magical Underwater World 4K | 2160p | 7304448660 bytes
            string videoUrl = "https://www.youtube.com/watch?v=7szcXCT-Oqw";

            //Trick or treat, kingdom hearts, and beat saber. | 18+ | 1080p | 197233776 bytes
            //string videoUrl = "https://www.youtube.com/watch?v=pQNRrnk63MQ";

            Console.WriteLine($"Video URL: {videoUrl}");

            VideoId videoId = ExtractVideoIdFromUrl(videoUrl);

            if (videoId != null)
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
                    Console.Write($"Raw info: ");
                    Console.WriteLine(video.RawInfo != null ? $"\n{video.RawInfo}" : "null");
                }
                else
                {
                    Console.WriteLine($"Video ID: {videoId}");
                    Console.WriteLine("ERROR! Video is not playable!");
                    string reason = !string.IsNullOrEmpty(video.Status.Reason) && !string.IsNullOrWhiteSpace(video.Status.Reason) ?
                        video.Status.Reason : "Unknown";
                    Console.WriteLine($"Reason: {reason}");
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
