using Newtonsoft.Json.Linq;

namespace YouTubeApiLib
{
    public class SimplifiedVideoInfo
    {
        public JObject Info { get; private set; }

        public SimplifiedVideoInfo(JObject simplifiedVideoInfo)
        {
            Info = simplifiedVideoInfo;
        }
    }
}
