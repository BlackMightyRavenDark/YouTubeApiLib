using System;
using static YouTubeApiLib.Utils;

namespace YouTubeApiLib.ConsoleTest
{
	internal class Program
	{
		static void Main(string[] args)
		{
			//11 HOURS of 4K Underwater Wonders + Relaxing Music - Coral Reefs & Colorful Sea Life in UHD
			//| 2160p | 30 fps | 78583477005 bytes
			string videoUrl = "https://www.youtube.com/watch?v=843Rpqza_6o";

			//Magical Underwater World 4K | 2160p | 24 fps | 7304448660 bytes
			//string videoUrl = "https://www.youtube.com/watch?v=7szcXCT-Oqw";

			//date night what's in my bag, removing trees, and celebrating mothers everywhere ep. 147
			//| 18+ | 1080p | 60 fps | 951661536 bytes
			//string videoUrl = "https://www.youtube.com/watch?v=EUKa5G7TNI0";

			//Live High-Definition Views from the International Space Station (Official NASA Stream) | HLS
			//string videoUrl = "https://www.youtube.com/watch?v=O9mYwRlucZY";

			Console.WriteLine($"Video URL: {videoUrl}");

			YouTubeVideoId videoId = ExtractVideoIdFromUrl(videoUrl);

			if (videoId != null)
			{
				YouTubeApi.getMediaTracksInfoImmediately = true;
				YouTubeVideo video = YouTubeVideo.GetById(videoId, null);
				if (video != null)
				{
					if (video.IsInfoAvailable)
					{
						if (!YouTubeApi.getMediaTracksInfoImmediately ||
							(YouTubeApi.getMediaTracksInfoImmediately && video.MediaTracks.Count == 0))
						{
							IYouTubeClient client = YouTubeApi.GetYouTubeClient("ios");
							video.UpdateMediaFormats(client);
						}
						Console.WriteLine($"Title: {video.Title}");
						Console.WriteLine($"ID: {video.Id}");
						Console.WriteLine($"URL: {video.Url}");
						if (video.SimplifiedInfo.IsMicroformatInfoAvailable)
						{
							Console.WriteLine($"Uploaded: {DateTimeToString(video.DateUploaded)}");
							Console.WriteLine($"Published: {DateTimeToString(video.DatePublished)}");
						}
						if (video.Length > TimeSpan.Zero)
						{
							Console.WriteLine($"Length: {video.Length}");
						}
						Console.WriteLine($"Channel title: {video.OwnerChannelTitle}");
						Console.WriteLine($"Channel ID: {video.OwnerChannelId}");
						Console.WriteLine($"Description: {video.Description}");
						Console.WriteLine($"View count: {video.ViewCount}");
						Console.WriteLine($"Category: {video.Category}");
						Console.WriteLine($"Private: {video.IsPrivate}");
						Console.WriteLine($"Unlisted: {video.IsUnlisted}");
						Console.WriteLine($"Family safe: {video.IsFamilySafe}");
						Console.WriteLine($"Live content: {video.IsLiveContent}");
						if (video.IsLiveContent)
						{
							Console.WriteLine($"Is broadcasting now: {video.IsLiveNow}");
							if (video.IsLiveNow)
							{
								Console.WriteLine($"HLS manifest URL: {video.HlsManifestUrl}");
							}
						}
						Console.WriteLine($"Is DASH manifest present: {video.IsDashed}");
						if (video.IsDashed)
						{
							Console.WriteLine($"DASH manifest URL: {video.DashManifestUrl}");
						}
						Console.Write("Thumbnails: ");
						if (video.ThumbnailUrls != null && video.ThumbnailUrls.Count > 0)
						{
							Console.WriteLine("");
							foreach (YouTubeVideoThumbnail videoThumbnail in video.ThumbnailUrls)
							{
								Console.WriteLine(videoThumbnail.ToString());
							}
						}
						else
						{
							Console.WriteLine("null");
						}
					}
					else
					{
						Console.WriteLine("Video info is unavailable!");
					}

					Console.Write("Downloadable formats: ");
					if (video.IsPlayable)
					{
						if (video.MediaTracks.Count > 0)
						{
							foreach (var dictItem in video.MediaTracks)
							{
								// Quick drafted code may display incorrect field values!
								// Some of the displayed values can be a big mistake!
								// The displayed information can be also incomplete in current commit!

								Console.WriteLine("");
								if (dictItem.Value.Client is YouTubeClientVideoInfo)
								{
									Console.WriteLine("The simple video info client has broken format URLs! Skipping it!");

									continue;
								}

								Console.WriteLine($"Track list for client [{dictItem.Value.Client.DisplayName}]:");
								foreach (YouTubeMediaTrack track in dictItem.Value.Tracks)
								{
									string info;
									if (track is YouTubeMediaTrackVideo)
									{
										YouTubeMediaTrackVideo videoTrack = track as YouTubeMediaTrackVideo;
										string trackType;
										if (track.IsHlsManifestPresent)
										{
											trackType = "HLS";
										}
										else
										{
											trackType = videoTrack.IsDashManifestPresent ? "DASH VIDEO" : "VIDEO";
										}
										info = $"{trackType} | ID {videoTrack.FormatId} | {videoTrack.VideoWidth}x{videoTrack.VideoHeight} | " +
											$"{videoTrack.FrameRate} fps | {videoTrack.ContentLength} bytes | {videoTrack.FileExtension}";
									}
									else if (track is YouTubeMediaTrackAudio)
									{
										YouTubeMediaTrackAudio audioTrack = track as YouTubeMediaTrackAudio;
										string trackType = audioTrack.IsDashManifestPresent ? "DASH AUDIO" : "AUDIO";
										info = $"{trackType} | ID {audioTrack.FormatId} | {audioTrack.SampleRate} Hz | " +
											$"{audioTrack.ChannelCount} ch | {audioTrack.AudioQuality} | {audioTrack.FileExtension}";
									}
									else if (track is YouTubeMediaTrackContainer)
									{
										YouTubeMediaTrackContainer container = track as YouTubeMediaTrackContainer;
										info = $"CONTAINER | ID {container.FormatId} | {container.VideoWidth}x{container.VideoHeight} | " +
											$"{container.VideoFrameRate} fps | {container.FileExtension}";
									}
									else
									{
										Console.WriteLine("ERROR! Unknown track type!");
										continue;
									}
									Console.WriteLine(info);
									if (track.IsDashManifestPresent)
									{
										string dashChunkCountString = track.DashUrls != null ? track.DashUrls.Count.ToString() : "null";
										Console.WriteLine($"DASH chunk URL count: {dashChunkCountString}");
									}
									else
									{
										string url = string.IsNullOrEmpty(track.FileUrl.ToString()) || string.IsNullOrWhiteSpace(track.FileUrl.ToString()) ? "null" : track.FileUrl.ToString();
										string t = track.IsHlsManifestPresent ? "Playlist URL" : "URL";
										Console.WriteLine($"{t}: {url}");
										if (!track.IsHlsManifestPresent)
										{
											if (track.FileUrl.SplitUrl() && track.FileUrl.QueryUrl.ContainsKey("n"))
											{
												Console.WriteLine($"'n'-param: {track.FileUrl.QueryUrl["n"]}");
											}
										}
									}
								}
								YouTubeConfig youTubeConfig = dictItem.Value.UrlDecryptionData?.VideoWebPage.ExtractYouTubeConfig();
								string playerUrl = youTubeConfig != null ? youTubeConfig.PlayerUrl : "<Not available>";
								Console.WriteLine($"Player URL: {playerUrl}");
							}
						}
						else
						{
							Console.WriteLine("null or empty");
						}
					}
					else
					{
						Console.WriteLine("Not found!");
						Console.WriteLine($"Is playable: {video.Status.IsPlayable}");
						Console.WriteLine($"Is private: {video.Status.IsPrivate}");
						Console.WriteLine($"Is adult: {video.Status.IsAdult}");
						Console.WriteLine($"Status: {video.Status.Status}");
						Console.WriteLine($"Reason: {video.Status.Reason}");
						if (!string.IsNullOrEmpty(video.Status.ReasonDetails))
						{
							Console.WriteLine($"Reason details: {video.Status.ReasonDetails}");
						}

						string thumbnailUrl =
							string.IsNullOrEmpty(video.Status.ThumbnailUrl) ||
							string.IsNullOrWhiteSpace(video.Status.ThumbnailUrl) ?
							"null or empty" : video.Status.ThumbnailUrl;
						Console.WriteLine($"Thumbnail URL: {thumbnailUrl}");
					}
					Console.Write($"Raw info: ");
					Console.WriteLine(video.RawInfo != null ? $"\n{video.RawInfo}" : "null");
				}
				else
				{
					Console.WriteLine("ERROR! Video is null!");
				}
			}
			else
			{
				Console.WriteLine("Video ID: <ERROR>");
			}

			Console.ReadLine();
		}

		private static string DateTimeToString(DateTime dateTime)
		{
			string s = dateTime.ToString("yyyy.MM.dd HH:mm:ss");
			return dateTime.Kind != DateTimeKind.Utc ? s : $"{s} GMT";
		}
	}
}
