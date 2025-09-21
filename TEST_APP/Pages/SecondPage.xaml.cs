using System.Data;
using System.Diagnostics;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Data.SqlClient;
using TEST_APP.Cards;
using TEST_APP.Services;
using Microsoft.Extensions.Logging;
namespace TEST_APP.Pages;

public partial class SecondPage : ContentPage {
    public SecondPage() {
        InitializeComponent();
    }

    protected override void OnAppearing() {
        base.OnAppearing();
        ClearEvents();
        ShowEvents();
    }

    private void ShowEvents() {
        // convert query to command
        var command = new SqlCommand("SELECT * FROM events");
        var ev_manager = new EventManager(Resources, EventStackLayout, Navigation);
        DataTable table = DatabaseConnector.ExecuteReadQuery(command);

        foreach (DataRow row in table.Rows) {
            var event_data = new EventManager.event_data {
                event_id = Convert.ToInt32(row["event_id"]),
                name = row["name"].ToString() ?? "none",
                description = row["description"].ToString() ?? "none",
                date_time = row["date_time"].ToString() ?? "none",
                link = row["link"].ToString() ?? "none",
                number_limit = Convert.ToInt32(row["number_limit"]),
                color = GetRandomColor().ToHex()
            };

            ev_manager.add_event_enter(event_data);
        }
    }

    void ClearEvents() {
        foreach (var child in EventStackLayout.Children.ToList()) {
            EventStackLayout.Children.Remove(child);
        }
    }

    private Color GetRandomColor() {
        var random = new Random();
        return Color.FromRgb(random.Next(100, 256), random.Next(100, 256), random.Next(100, 256));
    }
}