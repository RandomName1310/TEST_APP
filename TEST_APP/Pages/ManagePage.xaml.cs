using System.Data;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Data.SqlClient;
using TEST_APP;
using TEST_APP.Services;


namespace TEST_APP.Pages;

public partial class ManagePage : ContentPage
{
    public ManagePage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ClearEvents();
        ShowEvents();
    }

    public void ShowEvents()
    {
        var ev_manager = new EventManager(Resources, EventStackLayout, Navigation);

        var command = new SqlCommand(@"
            SELECT E.event_id, E.name, E.description, E.date_time, E.link, E.number_limit
            FROM Volunteer V
            JOIN Volunteer_Event VE ON VE.volunteer_id = V.volunteer_id
            JOIN Events E ON E.event_id = VE.event_id
            WHERE V.email = @email;
        ");
        command.Parameters.AddWithValue("@email", UserService.UService.currentUser.Email);
        DataTable Table = DatabaseConnector.ExecuteReadQuery(command);

        foreach (DataRow row in Table.Rows)
        {
            var event_data = new EventManager.event_data {
                event_id = Convert.ToInt32(row["event_id"]),
                name = row["name"].ToString() ?? "None",
                description = row["description"].ToString() ?? "None",
                date_time = row["date_time"].ToString() ?? "None",
                link = row["link"].ToString() ?? "None",
                number_limit = Convert.ToInt32(row["number_limit"]),
                color = GetRandomColor().ToHex()
            };

            ev_manager.add_event_manage(event_data);
        }
    }

    public void ClearEvents() {
        foreach (var child in EventStackLayout.Children.ToList()) {
            EventStackLayout.Children.Remove(child);
        }
    }

    private Color GetRandomColor()
    {
        var random = new Random();
        return Color.FromRgb(random.Next(100, 256), random.Next(100, 256), random.Next(100, 256));
    }
}
