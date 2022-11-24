using System;

namespace YouTubeApiLib.TestChannelPages
{
    internal class Program
    {
        static void Main(string[] args)
        {
            YouTubeChannel channel = new YouTubeChannel("UCSCHk4GbzMlKtxwpXPyYeMA", "Frozzen Fro");
            YouTubeApi api = new YouTubeApi();
            YouTubeApi.getMediaTracksInfoImmediately = false;
            VideoIdPageResult videoIdPageResult = api.GetVideoIdPage(channel, null);
            if (videoIdPageResult.ErrorCode == 200)
            {
                Console.WriteLine($"{channel.DisplayName} [{channel.Id}]: Videos tab page:");
                foreach (string id in videoIdPageResult.VideoIdPage.VideoIds)
                {
                    Console.WriteLine(id);
                }
                Console.WriteLine($"Continuation token: {videoIdPageResult.VideoIdPage.ContinuationToken}");
            }
            else
            {
                Console.WriteLine($"{channel.DisplayName} [{channel.Id}]: Error code = {videoIdPageResult.ErrorCode}");
            }

            Console.ReadLine();
        }
    }
}
