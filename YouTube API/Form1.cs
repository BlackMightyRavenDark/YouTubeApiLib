using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace YouTube_API
{
    public partial class Form1 : Form
    {
        private JArray searchResult = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void listView1_Resize(object sender, EventArgs e)
        {
            columnHeaderTitle.Width = listView1.Width - columnHeaderId.Width - 30;
        }

        private void btnSaveList_Click(object sender, EventArgs e)
        {
            btnSaveList.Enabled = false;
            if (searchResult == null || searchResult.Count == 0)
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
                json.Add(new JProperty("videos", searchResult));
                File.WriteAllText(sfd.FileName, json.ToString());
            }
            sfd.Dispose();

            btnSaveList.Enabled = true;
        }

        private async void btnGetChannelVideoList_Click(object sender, EventArgs e)
        {
            btnGetChannelVideoList.Enabled = false;
            btnSaveList.Enabled = false;
            listView1.Items.Clear();
            searchResult = null;

            string channelName = textBoxChannelName.Text;
            if (string.IsNullOrEmpty(channelName) || string.IsNullOrWhiteSpace(channelName))
            {
                MessageBox.Show("Не введено название канала!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnGetChannelVideoList.Enabled = true;
                btnSaveList.Enabled = true;
                return;
            }
            string channelId = textBoxChannelId.Text;
            if (string.IsNullOrEmpty(channelId) || string.IsNullOrWhiteSpace(channelId))
            {
                MessageBox.Show("Не введён ID канала!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnGetChannelVideoList.Enabled = true;
                btnSaveList.Enabled = true;
                return;
            }

            YouTubeChannel youTubeChannel = new YouTubeChannel(channelId, channelName);
            YouTubeApi api = new YouTubeApi();
            SortingOrder sortingOrder = SortingOrder.Descending;
            if (rbAscending.Checked)
            {
                sortingOrder = SortingOrder.Ascending;
            }
            else if (rbPopularity.Checked)
            {
                sortingOrder = SortingOrder.Popularity;
            }
            VideoListResult videoListResult =
                await Task.Run(() => api.GetChannelVideoList(youTubeChannel, sortingOrder));
            if (videoListResult.List == null || videoListResult.List.Count == 0)
            {
                MessageBox.Show("Ничего не найдено!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnGetChannelVideoList.Enabled = true;
                btnSaveList.Enabled = true;
                return;
            }
            if (videoListResult.ErrorCode == 200)
            {
                foreach (JObject jInfo in videoListResult.List)
                {
                    string id = jInfo.Value<string>("id");
                    string title = jInfo.Value<string>("title");

                    ListViewItem item = new ListViewItem(id);
                    item.SubItems.Add(title);
                    listView1.Items.Add(item);
                }

                searchResult = videoListResult.List;
            }

            btnGetChannelVideoList.Enabled = true;
            btnSaveList.Enabled = true;
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
                for (int i = (int)ChannelTab.Home; i < (int)ChannelTab.About; i++)
                {
                    ChannelTab tab = (ChannelTab)i;
                    YouTubeChannelTabResult channelTabResult =
                        api.GetChannelTab(channelId, tab, SortingOrder.Descending);
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
