using System.Data;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using TEST_APP.Services;

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

        private async void AddUser(object sender, EventArgs e)
        {
            ClickAnim(sender);
            UserData? userData = await UserService.UService.GetCurrentUserAsync();

            if (userData != null) {
                var selectCmd = new SqlCommand("SELECT volunteer_id FROM Volunteer WHERE email = @email");
                selectCmd.Parameters.AddWithValue("@email", userData.Email);

                DataTable dt = DatabaseConnector.ExecuteReadQuery(selectCmd);
                if (dt.Rows.Count == 0) {
                    Debug.WriteLine("User not found in database!");
                    return;
                }

                int volunteerId = Convert.ToInt32(dt.Rows[0]["volunteer_id"]);

                var command = new SqlCommand("INSERT INTO Volunteer_Event(event_id, volunteer_id) VALUES(@event_id, @volunteer_id)");
                command.Parameters.AddWithValue("@event_id", _data.event_id);
                command.Parameters.AddWithValue("@volunteer_id", volunteerId);

                _ = DatabaseConnector.ExecuteNonQuery(command);

                Debug.WriteLine("USER CONNECTED TO EVENT!");
            }
        }

        async void ClickAnim(object sender) {
            if(sender is Button btn) {
                await btn.ScaleTo(0.9, 100, Easing.CubicOut);
                await btn.ScaleTo(1.0, 100, Easing.CubicOut);
            }
        }
    }
}
