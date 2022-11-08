
namespace YouTubeApiLib
{
    public static class YouTubeChannelTabPages
    {
        public static readonly TabPageHome Home = new TabPageHome("Home", "EghmZWF0dXJlZA%3D%3D");
        public static readonly TabPageVideos Videos = new TabPageVideos("Videos", "EgZ2aWRlb3MYAyAAMAE%3D");
        public static readonly TabPageCommunity Community = new TabPageCommunity("Community", "Egljb21tdW5pdHk%3D");
        public static readonly TabPageChannels Channels = new TabPageChannels("Channels", "EghjaGFubmVscw%3D%3D");
        public static readonly TabPageAbout About = new TabPageAbout("About", "EgVhYm91dA%3D%3D");
    }

    public class TabPageHome : YouTubeChannelTabPage
    {
        public TabPageHome(string title, string paramsId)
        {
            Title = title;
            ParamsId = paramsId;
        }
    }

    public class TabPageVideos : YouTubeChannelTabPage
    {
        public TabPageVideos(string title, string paramsId)
        {
            Title = title;
            ParamsId = paramsId;
        }
    }

    public class TabPageCommunity : YouTubeChannelTabPage
    {
        public TabPageCommunity(string title, string paramsId)
        {
            Title = title;
            ParamsId = paramsId;
        }
    }

    public class TabPageChannels : YouTubeChannelTabPage
    {
        public TabPageChannels(string title, string paramsId)
        {
            Title = title;
            ParamsId = paramsId;
        }
    }

    public class TabPageAbout : YouTubeChannelTabPage
    {
        public TabPageAbout(string title, string paramsId)
        {
            Title = title;
            ParamsId = paramsId;
        }
    }
}
