
namespace YouTubeApiLib
{
	public class YouTubeVideoId
	{
		public string Id { get; }

		public YouTubeVideoId(string id)
		{
			Id = id;
		}

		public override string ToString()
		{
			return Id;
		}
	}
}
