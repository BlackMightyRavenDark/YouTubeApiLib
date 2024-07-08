
namespace YouTubeApiLib
{
	public static class YouTubeApiV1SearchResultFilters
	{
		public static readonly YouTubeApiV1SearchResultFilter None = new YouTubeApiV1SearchResultFilter("None", null);
		public static readonly YouTubeApiV1SearchResultFilter Video = new YouTubeApiV1SearchResultFilter("Video", "EgIQAQ%3D%3D");
		public static readonly YouTubeApiV1SearchResultFilter Channel = new YouTubeApiV1SearchResultFilter("Channel", "EgIQAg%3D%3D");
		public static readonly YouTubeApiV1SearchResultFilter Playlist = new YouTubeApiV1SearchResultFilter("Playlist", "EgIQAw%3D%3D");
	}

	public class YouTubeApiV1SearchResultFilter
	{
		public string Name { get; }
		public string ParamsId { get; }

		public YouTubeApiV1SearchResultFilter(string name, string paramsId)
		{
			Name = name;
			ParamsId = paramsId;
		}
	}
}
