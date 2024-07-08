
namespace YouTubeApiLib
{
	public class YouTubeChannel
	{
		public string DisplayName { get; }
		public string Id { get; }

		public YouTubeChannel(string id, string displayName)
		{
			Id = id;
			DisplayName = displayName;
		}

		public override string ToString()
		{
			return $"{DisplayName} ({Id})";
		}
	}
}
