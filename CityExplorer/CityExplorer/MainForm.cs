using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Windows.Forms;

namespace CityExplorer
{
    public partial class MainForm : Form
    {
        private List<City> citiesCache = new(); // Кеш для зберігання міст
        private readonly HttpClient httpClient = new();

        public MainForm()
        {
            InitializeComponent();
            cmbCriteria.Items.AddRange(new[] { "id", "Місто", "Населення" });
            cmbCriteria.SelectedIndex = 0; // Встановлення стандартного значення
            LoadCitiesAsync();
        }
        private async Task LoadCitiesAsync()
        {
            
            try
            {
                var response = await httpClient.GetStringAsync("http://localhost:5001/api/cities");
                var citiesData = JsonDocument.Parse(response);
                
                foreach (var cityElement in citiesData.RootElement.EnumerateArray())
                {
                    citiesCache.Add(new City
                    {
                        Id = cityElement.GetProperty("id").GetInt32(),
                        Name = cityElement.GetProperty("name").GetString(),
                        Uri = cityElement.GetProperty("uri").GetString()
                    });
                }

                dataGridViewCities.DataSource = citiesCache;

                // Видаляємо колонки, які не хочемо відображати
                if (dataGridViewCities.Columns.Contains("Population"))
                {
                    dataGridViewCities.Columns["Population"].Visible = false;
                }
                if (dataGridViewCities.Columns.Contains("Description"))
                {
                    dataGridViewCities.Columns["Description"].Visible = false;
                }
                if (dataGridViewCities.Columns.Contains("History"))
                {
                    dataGridViewCities.Columns["History"].Visible = false;
                }
                if (dataGridViewCities.Columns.Contains("ImageUrl"))
                {
                    dataGridViewCities.Columns["ImageUrl"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження даних: {ex.Message}");
            }
        }

        // Обробник натискання кнопки пошуку
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.ToLower();
            string criteria = cmbCriteria.SelectedItem.ToString();

            IEnumerable<City> filteredCities = citiesCache;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                switch (criteria)
                {
                    case "id":
                        filteredCities = citiesCache.FindAll(c => c.Id.ToString().ToLower().Contains(searchTerm));
                        break;
                    case "Місто":
                        filteredCities = citiesCache.FindAll(c => c.Name.ToLower().Contains(searchTerm));
                        break;
                    case "Населення":
                        filteredCities = citiesCache.FindAll(c => c.Population.HasValue && c.Population.Value.ToString().Contains(searchTerm));
                        break;
                }
            }

            dataGridViewCities.DataSource = filteredCities;
        }

        // Відображення деталей про місто
        private async void dataGridViewCities_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewCities.SelectedRows.Count > 0)
            {
                var selectedCity = (City)dataGridViewCities.SelectedRows[0].DataBoundItem;
                await LoadCityDetailsAsync(selectedCity);
            }
        }

        // Завантаження детальної інформації про місто
        private async Task LoadCityDetailsAsync(City city)
        {
            pictureBoxCity.ImageLocation = null;
            try
            {
                var response = await httpClient.GetStringAsync($"http://localhost:5001/api/cities/"+ city.Id);
                var cityDetails = JsonDocument.Parse(response).RootElement;

                lblName.Text = cityDetails.GetProperty("name").GetString();
                lblPopulation.Text = cityDetails.GetProperty("population").ToString();
                rtbHistory.Text = cityDetails.GetProperty("description").GetString();
                pictureBoxCity.ImageLocation = cityDetails.GetProperty("imageUrl").GetString(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження деталей: {ex.Message}");
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lblName.Text = "";
            lblPopulation.Text = "";
            lblHistory.Text = "";
            pictureBoxCity.ImageLocation = null;
            dataGridViewCities.DataSource = citiesCache;
        }
    }
}
