using System.Data;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using TEST_APP.HelperClasses;

namespace TEST_APP.Pages
{
    public partial class DescriptPage : ContentPage
    {
        EventManager.event_data _data;
        public DescriptPage(EventManager.event_data data)
        {
            InitializeComponent();

            _data = data;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SetDescriptions();
        }

        private void SetDescriptions()
        {
            Title.Text = _data.name;
            Description.Text = _data.description;
            DateTime.Text = _data.date_time;
            Link.Text = _data.link;
        }

        public void AddClient(object sender, EventArgs e)
        {
            AddUser();
        }

        private async void AddUser()
        {
            string query = "INSERT INTO event_client (event_id, client_id) SELECT " + _data.event_id + " , 2 WHERE NOT EXISTS(SELECT 1 FROM event_client WHERE event_id = " + _data.event_id + " AND client_id = " + 2 + ");";
            var db = new DatabaseConnector();
            DataTable table = await db.ExecuteQueryAsync(query);
        }
    }
}
