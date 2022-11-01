using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace YouTubeApiLib.GuiTest
{
    public partial class Form1 : Form
    {
        private JArray foundVideos = null;
        private string nextPageToken = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void listView1_Resize(object sender, EventArgs e)
        {
            columnHeaderTitle.Width = listView1.Width - columnHeaderId.Width - 30;
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            btnNextPage.Left = panel1.Width / 2 - btnNextPage.Width / 2;
        }

        private void btnSaveList_Click(object sender, EventArgs e)
        {
            btnSaveList.Enabled = false;
            if (foundVideos == null || foundVideos.Count == 0)
            {
                MessageBox.Show("Список пуст!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSaveList.Enabled = true;
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save ass...";
            sfd.Filter = "JSON|*.json";
            sfd.FileName = textBoxChannelName.Text;
            sfd.DefaultExt = ".json";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                JObject json = new JObject();
                json.Add(new JProperty("videos", foundVideos));
                System.IO.File.WriteAllText(sfd.FileName, json.ToString());
            }
            sfd.Dispose();

            btnSaveList.Enabled = true;
        }

        private async void btnOpenChannel_Click(object sender, EventArgs e)
        {
            btnOpenChannel.Enabled = false;
            btnNextPage.Enabled = false;
            btnSaveList.Enabled = false;
            listView1.Items.Clear();
            nextPageToken = null;
            foundVideos = new JArray();

            string channelName = textBoxChannelName.Text;
            if (string.IsNullOrEmpty(channelName) || string.IsNullOrWhiteSpace(channelName))
            {
                MessageBox.Show("Не введено название канала!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnOpenChannel.Enabled = true;
                btnSaveList.Enabled = true;
                return;
            }
            string channelId = textBoxChannelId.Text;
            if (string.IsNullOrEmpty(channelId) || string.IsNullOrWhiteSpace(channelId))
            {
                MessageBox.Show("Не введён ID канала!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnOpenChannel.Enabled = true;
                btnSaveList.Enabled = true;
                return;
            }

            YouTubeChannel youTubeChannel = new YouTubeChannel(channelId, channelName);
            YouTubeApi api = new YouTubeApi();
            VideoPageResult videoPageResult =
                await Task.Run(() => api.GetVideosPage(youTubeChannel.Id, null));
            if (videoPageResult.List == null || videoPageResult.List.Count == 0 || videoPageResult.ErrorCode != 200)
            {
                MessageBox.Show("Ничего не найдено!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnOpenChannel.Enabled = true;
                btnSaveList.Enabled = true;
                return;
            }

            foreach (JObject jInfo in videoPageResult.List)
            {
                foundVideos.Add(jInfo);

                string id = jInfo.Value<string>("id");
                string title = jInfo.Value<string>("title");

                ListViewItem item = new ListViewItem(id);
                item.SubItems.Add(title);
                listView1.Items.Add(item);
            }

            nextPageToken = videoPageResult.ContinuationToken;
            if (!string.IsNullOrEmpty(nextPageToken) && !string.IsNullOrWhiteSpace(nextPageToken))
            {
                btnNextPage.Enabled = true;
            }

            btnOpenChannel.Enabled = true;
            btnSaveList.Enabled = true;
        }

        private async void btnNextPage_Click(object sender, EventArgs e)
        {
            btnNextPage.Enabled = false;
            btnOpenChannel.Enabled = false;
            if (string.IsNullOrEmpty(nextPageToken) || string.IsNullOrWhiteSpace(nextPageToken))
            {
                MessageBox.Show("Дальше ничего нет! Дальше только мрак и пустота!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnOpenChannel.Enabled = true;
                return;
            }

            YouTubeApi api = new YouTubeApi();
            VideoPageResult videoPageResult = await Task.Run(() => api.GetVideosPage(null, nextPageToken));
            if (videoPageResult.List == null || videoPageResult.List.Count == 0)
            {
                MessageBox.Show("Ничего не найдено!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnOpenChannel.Enabled = true;
                btnSaveList.Enabled = true;
                return;
            }
            if (videoPageResult.ErrorCode == 200)
            {
                foreach (JObject jInfo in videoPageResult.List)
                {
                    foundVideos.Add(jInfo);

                    string id = jInfo.Value<string>("id");
                    string title = jInfo.Value<string>("title");

                    ListViewItem item = new ListViewItem(id);
                    item.SubItems.Add(title);
                    listView1.Items.Add(item);
                }

                nextPageToken = videoPageResult.ContinuationToken;
                if (!string.IsNullOrEmpty(nextPageToken))
                {
                    btnNextPage.Enabled = true;
                }
            }

            btnOpenChannel.Enabled = true;
        }

        private async void btnGetChannelPages_Click(object sender, EventArgs e)
        {
            btnGetChannelPages.Enabled = false;
            textBoxChannelPages.Clear();

            string channelName = textBoxChannelName.Text;
            if (string.IsNullOrEmpty(channelName) || string.IsNullOrWhiteSpace(channelName))
            {
                MessageBox.Show("Не введено название канала!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnGetChannelPages.Enabled = true;
                return;
            }
            string channelId = textBoxChannelId.Text;
            if (string.IsNullOrEmpty(channelId) || string.IsNullOrWhiteSpace(channelId))
            {
                MessageBox.Show("Не введён ID канала!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnGetChannelPages.Enabled = true;
                return;
            }

            JObject jResult = new JObject();

            int count = await Task.Run(() =>
            {
                int n = 0;
                YouTubeApi api = new YouTubeApi();
                for (int i = (int)ChannelTab.Home; i < (int)ChannelTab.About; ++i)
                {
                    ChannelTab tab = (ChannelTab)i;
                    YouTubeChannelTabResult channelTabResult = api.GetChannelTab(channelId, tab);
                    string tabTitle = tab.ToString();
                    if (channelTabResult.ErrorCode == 200)
                    {
                        n++;
                        jResult.Add(new JProperty(tabTitle, channelTabResult.Tab.Json));
                    }
                }
                return n;
            });

            if (count > 0)
            {
                textBoxChannelPages.Text = jResult.ToString();
            }
            else
            {
                MessageBox.Show("Ничего не найдено!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            btnGetChannelPages.Enabled = true;
        }
    }
}
