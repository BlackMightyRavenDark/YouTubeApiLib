using System;
using System.Collections.Generic;

namespace YouTubeApiLib.TestChannelPages
{
    internal class Program
    {
        static void Main(string[] args)
        {
            YouTubeChannel channel = new YouTubeChannel("UCSCHk4GbzMlKtxwpXPyYeMA", "Frozzen Fro");
            YouTubeApi api = new YouTubeApi();
            YouTubeApi.getMediaTracksInfoImmediately = false;
            YouTubeApi.decryptMediaTrackUrlsAutomaticallyIfPossible = false;
            List<YouTubeChannelTabPage> pages = new List<YouTubeChannelTabPage>()
            {
                YouTubeChannelTabPages.Videos,
                YouTubeChannelTabPages.Shorts,
                YouTubeChannelTabPages.Live
            };
            foreach (YouTubeChannelTabPage page in pages)
            {
                VideoIdPageResult videoIdPageResult = api.GetVideoIdPage(channel, page, null);
                if (videoIdPageResult.ErrorCode == 200)
                {
                    Console.WriteLine($"{channel} {page.Title} tab page:");
                    foreach (string id in videoIdPageResult.VideoIdPage.VideoIds)
                    {
                        YouTubeVideo video = api.GetVideo(new VideoId(id));
                        string t = video != null ? $"{video.Id} > {video.Title}" : $"{id} > null";
                        Console.WriteLine(t);
                    }
                    string token = !string.IsNullOrEmpty(videoIdPageResult.VideoIdPage.ContinuationToken) &&
                        !string.IsNullOrWhiteSpace(videoIdPageResult.VideoIdPage.ContinuationToken) ?
                    videoIdPageResult.VideoIdPage.ContinuationToken : "null";
                    Console.WriteLine($"Continuation token: {token}");
                }
                else
                {
                    Console.WriteLine($"{channel} {page.Title} tab page: null");
                }
            }
            Console.ReadLine();
        }
    }
}
