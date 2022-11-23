using System;
using static YouTubeApiLib.Utils;

namespace YouTubeApiLib.ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Magical Underwater World 4K | 2160p | 7304448660 bytes
            //string videoUrl = "https://www.youtube.com/watch?v=7szcXCT-Oqw";

            //Trick or treat, kingdom hearts, and beat saber. | 18+ | 1080p | 197233776 bytes
            //string videoUrl = "https://www.youtube.com/watch?v=pQNRrnk63MQ";

            //коопим с Колясиком : Fighting Force | 1080p | DASH
            //string videoUrl = "https://www.youtube.com/watch?v=Z9c9SbfGvec";

            //NASA Live: Official Stream of NASA TV | HLS
            string videoUrl = "https://www.youtube.com/watch?v=21X5lGlDOfg";

            Console.WriteLine($"Video URL: {videoUrl}");

            VideoId videoId = ExtractVideoIdFromUrl(videoUrl);

            if (videoId != null)
            {
                YouTubeApi api = new YouTubeApi();
                YouTubeApi.getMediaTracksInfoImmediately = true;
                YouTubeVideo video = api.GetVideo(videoId);
                if (video != null)
                {
                    if (!YouTubeApi.getMediaTracksInfoImmediately)
                    {
                        video.UpdateMediaFormats(video.RawInfo);
                    }
                    Console.WriteLine($"Title: {video.Title}");
                    Console.WriteLine($"ID: {video.Id}");
                    Console.WriteLine($"URL: {video.Url}");
                    Console.WriteLine($"Uploaded: {video.DateUploaded:yyyy.MM.dd}");
                    Console.WriteLine($"Published: {video.DatePublished:yyyy.MM.dd}");
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
                    Console.Write("Downloadable formats: ");
                    if (video.MediaTracks != null && video.MediaTracks.Count > 0)
                    {
                        Console.WriteLine("");
                        // Quick drafted code may display incorrect field values!
                        // Some of the displayed values can be a big mistake!
                        // The displayed information can be also incomplete in current commit!
                        foreach (YouTubeMediaTrack track in video.MediaTracks)
                        {
                            string info;
                            if (track is YouTubeMediaTrackVideo)
                            {
                                YouTubeMediaTrackVideo videoTrack = track as YouTubeMediaTrackVideo;
                                string trackType;
                                if (track.IsHlsManifest)
                                {
                                    trackType = "HLS";
                                }
                                else
                                {
                                    trackType = videoTrack.IsDashManifest ? "DASH VIDEO" : "VIDEO";
                                }
                                info = $"{trackType} | ID {videoTrack.FormatId} | {videoTrack.VideoWidth}x{videoTrack.VideoHeight} | " +
                                    $"{videoTrack.FrameRate} fps | {videoTrack.ContentLength} bytes | {videoTrack.FileExtension}";
                            }
                            else if (track is YouTubeMediaTrackAudio)
                            {
                                YouTubeMediaTrackAudio audioTrack = track as YouTubeMediaTrackAudio;
                                string trackType = audioTrack.IsDashManifest ? "DASH AUDIO" : "AUDIO";
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
                            if (track.IsDashManifest)
                            {
                                string dashChunkCountString = track.DashUrls != null ? track.DashUrls.Count.ToString() : "null";
                                Console.WriteLine($"DASH chunk URL count: {dashChunkCountString}");
                            }
                            else
                            {
                                string url = string.IsNullOrEmpty(track.FileUrl) || string.IsNullOrWhiteSpace(track.FileUrl) ? "null" : track.FileUrl;
                                string t = track.IsHlsManifest ? "Playlist URL" : "URL";
                                Console.WriteLine($"{t}: {url}");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("null or empty");
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
    }
}
