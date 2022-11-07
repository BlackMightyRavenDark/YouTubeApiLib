
namespace YouTubeApiLib
{
    public class VideoId
    {
        public string Id { get; private set; }

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
