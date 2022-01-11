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
            if (searchResult == null)
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
            if (videoListResult.ErrorCode == 200)
            {
                if (videoListResult.List.Count == 0)
                {
                    MessageBox.Show("Ничего не найдено!", "Ошибка!",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnGetChannelVideoList.Enabled = true;
                    btnSaveList.Enabled = true;
                    return;
                }
                searchResult = videoListResult.List;
                foreach (JObject jInfo in videoListResult.List)
                {
                    string id = jInfo.Value<string>("id");
                    string title = jInfo.Value<string>("title");

                    ListViewItem item = new ListViewItem(id);
                    item.SubItems.Add(title);
                    listView1.Items.Add(item);
                }
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

            YouTubeChannelTabsResult tabsResult = await Task.Run(() =>
            {
                YouTubeApi api = new YouTubeApi();
                return api.GetChannelTabs(channelId, SortingOrder.Descending);
            });
            if (tabsResult.ErrorCode == 200)
            {
                JObject j = new JObject();
                foreach (YouTubeChannelTab tab in tabsResult.Tabs)
                {
                    System.Diagnostics.Debug.WriteLine(tab.Title);
                    j.Add(new JProperty(tab.Title, tab.Json));
                }
                string t = j.ToString();
                textBoxChannelPages.Text = t;
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
