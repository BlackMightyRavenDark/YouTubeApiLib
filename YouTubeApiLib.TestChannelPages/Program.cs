﻿using System;
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
			List<YouTubeChannelTabPage> pages = new List<YouTubeChannelTabPage>()
			{
				YouTubeChannelTabPages.Videos,
				YouTubeChannelTabPages.Shorts,
				YouTubeChannelTabPages.Live
			};
			foreach (YouTubeChannelTabPage page in pages)
			{
				YouTubeVideoIdPageResult videoIdPageResult = api.GetVideoIdPage(channel, page);
				if (videoIdPageResult.ErrorCode == 200)
				{
					Console.WriteLine($"{channel} {page.Title} tab page:");
					foreach (string videoId in videoIdPageResult.VideoIdPage.VideoIds)
					{
						YouTubeVideo video = YouTubeVideo.GetById(videoId);
						string t = video != null ? $"{video.Id} > {video.Title}" : $"{videoId} > null";
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
