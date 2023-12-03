
namespace YouTubeApiLib
{
    public class VideoId
    {
        public string Id { get; }

        public VideoId(string id)
        {
            Id = id;
        }

        public override string ToString()
        {
            return Id;
        }
    }
}
