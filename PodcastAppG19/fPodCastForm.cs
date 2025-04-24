using PodcastAppG19.BLL;
using System;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Data;
using PodcastAppG19.ExceptionHandling;
using System.Security.Policy;
using PodcastAppG19.PodcastAppG19;
using PodcastAppG19.DAL;
using PodcastAppG19.Models;

namespace PodcastAppG19
{
    public partial class fPodCast : Form
    {
        FeedController feedController;
        private List<Feed> feeds;
        CategoryController categoryController;
        private bool validationPassed = false; // En flagga som indikerar om valideringen har passerat.

        public fPodCast()
        {
            InitializeComponent();
            feeds = new List<Feed>();
            feedController = new FeedController();
            categoryController = new CategoryController();
            UpdateContentforCatagory();
            UpdateFeedsList();
        }

        private void btnTaBort_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int selectedRowIndex = dataGridView1.SelectedRows[0].Index;
                Feed selectedFeed = feeds[selectedRowIndex];

                try
                {
                    // Försök ta bort feeden och dess innehåll
                    feedController.DeleteFeedAndContents(selectedFeed);

                    // Ta bort feeden från listan av feeds
                    feeds.Remove(selectedFeed);

                    // Ta bort raden från dataGridView1
                    dataGridView1.Rows.RemoveAt(selectedRowIndex);

                    // Rensa innehållet i dataGridView2 och episodeDescriptionBox "txtbINFO"
                    dataGridView2.Rows.Clear();
                    txtbINFO.Clear();
                }
                catch (Exception ex)
                {
                    // Hanterar undantaget
                    MessageBox.Show("Ett fel uppstod vid borttagning av feeden: " + ex.Message);
                }

            }
            else
            {
                MessageBox.Show("Välj en feed att ta bort.");
            }
        }

        private async void btnLaggTill_Click(object sender, EventArgs e)
        {
            string name = txtbNamn.Text;
            string url = textBoxURL.Text;
            string stringFrequency = cbBFrekvens.Text;
            int frequency = 0;
            string categoryTitle = cbBKategori.Text;

            // Validation: Ensure that the user enters valid data
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(url) || string.IsNullOrEmpty(stringFrequency) || string.IsNullOrEmpty(categoryTitle))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            frequency = UpdateFrequency(stringFrequency);

            Category category = new Category(categoryTitle);

            Feed feed = new Feed(name, url, frequency, category);
            feedController.Create(feed);

            int r = dataGridView1.Rows.Add();
            dataGridView1.Rows[r].Cells[1].Value = name;
            dataGridView1.Rows[r].Cells[2].Value = await feed.GetFeedTitleAsync();
            dataGridView1.Rows[r].Cells[3].Value = stringFrequency;
            dataGridView1.Rows[r].Cells[4].Value = categoryTitle;
            int episodeCount = feed.GetEpisodeNumber();

            // Check if the number of episodes is -1 and handle it appropriately
            if (episodeCount == -1)
            {
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    dataGridView1.Rows[r].Cells[i].Value = "";
                }
            }
            else
            {
                dataGridView1.Rows[r].Cells[0].Value = episodeCount;
            }

            // Add the new feed to the list of feeds
            feeds.Add(feed);

            // Clear and repopulate dataGridView2 with episodes
            UpdateEpisodeList(feed);

            // Serialize the updated list of feeds
            feedController.FeedSerailizer(feeds);
        }

        private void UpdateEpisodeList(Feed feed)
        {
            dataGridView2.Rows.Clear();
            foreach (Episode episode in feed.episodes)
            {
                int row = dataGridView2.Rows.Add();
                dataGridView2.Rows[row].Cells[0].Value = episode.name;
            }
        }


        private void txtbNamn_TextChanged(object sender, EventArgs e)
        {

            bool valideringResultat = Validation.NamnKontroll(txtbNamn.Text, RutaNamn);
            validationPassed = valideringResultat;

        }

        private async void cbBFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbBFilter.SelectedItem != null)
            {
                string selectedCategory = (string)cbBFilter.SelectedItem;

                // Now, update your DataGridView with the filtered feeds
                dataGridView1.Rows.Clear();

                // Filter the feeds based on the selected category
                List<Feed> filteredFeeds = feeds.Where(feed => feed.category.Title == selectedCategory).ToList();

                foreach (var feed in filteredFeeds)
                {
                    string stringFrequency = cbBFrekvens.Text;

                    int antalAvsnitt = feed.GetEpisodeNumber();
                    int r = dataGridView1.Rows.Add();
                    dataGridView1.Rows[r].Cells[0].Value = antalAvsnitt;
                    dataGridView1.Rows[r].Cells[1].Value = feed.name;
                    dataGridView1.Rows[r].Cells[2].Value = await feed.GetFeedTitleAsync();
                    dataGridView1.Rows[r].Cells[3].Value = stringFrequency;
                    dataGridView1.Rows[r].Cells[4].Value = feed.category.Title;
                }
            }
        }

        private void dataGridView2_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked cell is valid
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                // Clear the rows in dataGridView2
                dataGridView2.Rows.Clear();

                // Get the selected feed based on the clicked cell in dataGridView1
                Feed selectedFeed = feeds[e.RowIndex];

                // Loop through the episodes and add them to dataGridView2
                foreach (Episode episode in selectedFeed.episodes)
                {
                    int r = dataGridView2.Rows.Add();
                    dataGridView2.Rows[r].Cells[0].Value = episode.name;

                    txtbINFO.Text = episode.description;
                }
            }
        }


        private void Catagory_Click(object sender, EventArgs e)
        {
            categoryController.Create(kategoritxtb.Text);
            int r = dataGridView3.Rows.Add();
            dataGridView3.Rows[r].Cells[0].Value = kategoritxtb.Text;
            UpdateContentforCatagory();
        }

        private async void UpdateFeedsList()
        {
            dataGridView1.Rows.Clear();
            int feedRow = 0;
            feeds = feedController.Getallapodcast();
            foreach (Feed feed in feeds)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[feedRow].Cells[0].Value = feed.GetEpisodeNumber();
                dataGridView1.Rows[feedRow].Cells[1].Value = feed.name;
                dataGridView1.Rows[feedRow].Cells[2].Value = await feed.GetFeedTitleAsync();
                dataGridView1.Rows[feedRow].Cells[3].Value = UpdateFrequency(feed.frequency);
                dataGridView1.Rows[feedRow].Cells[4].Value = feed.category.Title;
                feedRow++;
            }
        }

        private string UpdateFrequency(int frequency)
        {
            return frequency + " min";
        }

        private int UpdateFrequency(string frequency) 
        {
            frequency = frequency.Replace(" min", "");
            return int.Parse(frequency);
        }

        private void UpdateContentforCatagory()
        {
            cbBKategori.Items.Clear();
            dataGridView3.Rows.Clear();
            cbBFilter.Items.Clear(); // Clear the filter combo box
            int categoryRow = 0;
            foreach (Category category in categoryController.GetallaCatagory())
            {
                cbBKategori.Items.Add(category.Title);
                cbBFilter.Items.Add(category.Title); // Add categories to the filter combo box
                dataGridView3.Rows.Add();
                dataGridView3.Rows[categoryRow].Cells[0].Value = category.Title;
                categoryRow++;
            }

            if (cbBKategori.Items.Count == 0)
            {
                return;
            }

            cbBKategori.SelectedIndex = 0;
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            UpdateContentforCatagory();
        }

        private void btnTaBort1_Click(object sender, EventArgs e)
        {
            string categoryName;
            try
            {
                categoryName = (string)dataGridView3.SelectedRows[0].Cells[0].Value;
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Ingen kategori var vald!" + Environment.NewLine + "Se till att markera en kategori genom att klicka på rutan bredvid kategorinamnet!");
                return;
            }
            Category category = categoryController.GetCategoryByName(categoryName);
            DialogResult dialogResult = MessageBox.Show("Radera kategorin " + category.Title, "Varning!", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                feedController.DeleteOnCategory(category);
                categoryController.Remove(category);

                categoryController.RemoveCategory(categoryName);

                UpdateContentforCatagory();
            }
            else if (dialogResult == DialogResult.No)
            {
                MessageBox.Show("Ingen kategori har raderats");
            }
        }

        private void kategoritxtb_TextChanged(object sender, EventArgs e)
        {
            bool valideringResultat = Validation.NamnKontroll(kategoritxtb.Text, KategoriNamn);
            validationPassed = valideringResultat;
        }

        private void btnAndra1_Click(object sender, EventArgs e)
        {
            string oldCategory;
            string newCategory;
            try
            {
                oldCategory = (string)dataGridView3.SelectedRows[0].Cells[0].Value;
                newCategory = kategoritxtb.Text;
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Ingen kategori var vald!" + Environment.NewLine + "Se till att markera en kategori genom att klicka på rutan bredvid kategorinamnet!");
                return;
            }

            if (string.IsNullOrWhiteSpace(newCategory))
            {
                MessageBox.Show("Kategori namnet får inte vara tomt!");
                return;
            }

            DialogResult dialogResult = MessageBox.Show("Ändra namn på " + oldCategory + " till " + newCategory, "Varning!", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                categoryController.UppdateKatagory(oldCategory, newCategory);
                UpdateContentforCatagory();
            }
        }

        private void btnAndra_Click(object sender, EventArgs e)
        {

            if (dataGridView1.SelectedRows.Count > 0)
            {
                int selectedRowIndex = dataGridView1.SelectedRows[0].Index;
                Feed selectedFeed = feeds[selectedRowIndex];
                string newFeedName = txtbNamn.Text; // Hamta namn fran anvandaren

                // Kontrollera att det nya namnet inte ar tomt eller ogiltigt
                if (!string.IsNullOrWhiteSpace(newFeedName))
                {
                    feedController.UpdateFeedName(selectedFeed, newFeedName);

                    // Uppdatera anvandargranssnittet med det nya namnet
                    dataGridView1.Rows[selectedRowIndex].Cells[1].Value = newFeedName;

                    MessageBox.Show("Namnet pa feeden har uppdaterats.");
                }
                else
                {
                    MessageBox.Show("Ange ett giltigt nytt namn for feeden.");
                }
            }
            else
            {
                MessageBox.Show("Valj en feed att uppdatera namnet for.");
            }
        }



        private void btnAterstall_Click(object sender, EventArgs e)
        {

            MessageBox.Show("Button clicked!");

            // Ta bort eventuell filtrering i DataGridView
            dataGridView1.Rows.Clear();

            // Uppdatera DataGridView med alla rader
            UpdateFeedsList();

            txtbNamn.Text = string.Empty;
            textBoxURL.Text = string.Empty;


            if (cbBFilter.SelectedItem != null)
            {
                cbBFilter.Text = "Filtrera...";
            }

            // Återställ ComboBox för kategori
            cbBKategori.Text = "Kategori...";

            // Återställ ComboBox för uppdateringsfrekvens
            if (cbBFrekvens.SelectedItem != null)
            {
                cbBFrekvens.SelectedIndex = -1;
            }
        }

        private void dataGridView1_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            try
            {
                string feedTitle = (string)e.Row.Cells[2].Value;
                getEpisodesAsync(feedTitle);
            }
            catch (System.InvalidCastException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void dataGridView1_CellStateChanged(object sender, DataGridViewCellStateChangedEventArgs e)
        {
            try
            {
                string feedTitle = (string)dataGridView1.Rows[e.Cell.RowIndex].Cells[2].Value;
                getEpisodesAsync(feedTitle);
            }
            catch (System.InvalidCastException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //Hämtar avsnitten från vald podcast


        private async void getEpisodesAsync(string feedTitle)
        {
            foreach (Feed feed in feeds)
            {
                if (await feed.GetFeedTitleAsync() == feedTitle)
                {
                    UpdateEpisodeList(feed);
                }
            }
        }

        private void dataGridView2_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            string episodeName = (string)e.Row.Cells[0].Value;

            foreach (Feed feed in feeds)
            {
                foreach (Episode episode in feed.episodes)
                {
                    if (episode.name == episodeName)

                        txtbINFO.Text = episode.description;
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int selectedRowIndex = dataGridView1.SelectedRows[0].Index;
                Feed selectedFeed = feeds[selectedRowIndex];

                if (cbBKategori.SelectedItem != null)
                {
                    // Hämta den valda kategorin från ComboBox
                    Category selectedCategory = new Category();

                    foreach (Category category in categoryController.GetallaCatagory()) 
                    {
                        if (category.Title.Contains((string)cbBKategori.SelectedItem)) 
                        { 
                            selectedCategory = category;
                            break;
                        }
                    }

                    // Uppdatera feedens kategori med den valda kategorin från ComboBox
                    feedController.UpdateFeedCategory(selectedRowIndex, selectedCategory);
                    // Uppdatera användargränssnittet för den valda feeden med den nya kategorin
                    dataGridView1.Rows[selectedRowIndex].Cells[4].Value = selectedCategory.Title;

                    // Alternativt: Anropa en metod i FeedController för att uppdatera kategorin för den valda feeden i databasen eller lagringen
                    // feedcontoller.UpdateFeedCategory(selectedFeed, selectedCategory);
                }
                else
                {
                    MessageBox.Show("Välj en kategori att tilldela till feeden.");
                }
            }
            else
            {
                MessageBox.Show("Välj en feed för att uppdatera kategorin.");
            }
        }
    }
}