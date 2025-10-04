using System;
using System.Data;
using System.Xml.Linq;
using Microsoft.Maui.Storage;
using TEST_APP.Services;
using TEST_APP.Cards;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;

namespace TEST_APP.Content.Forms;

public partial class RoleSetter : ContentView {
    EventManager.event_data ev_data;
    EventManager.role_data r_data;

    public RoleSetter(EventManager.event_data _ev_data) {
        InitializeComponent();
        ev_data = _ev_data;

        InitEventPicker();
    }

    private void InitEventPicker() {
        // clear picker before init
        RolePicker.Items.Clear();

        // TODO: pegando Roles globais, deve pegar apenas as do evento
        string query = @"
            SELECT r.name
            FROM Event_Role er
            INNER JOIN Roles r ON er.role_ID = r.role_ID
            WHERE er.event_ID = @event_id;
        ";
        var command = new SqlCommand(query);
        command.Parameters.AddWithValue("@event_id", ev_data.event_id);
        DataTable? table = DatabaseConnector.ExecuteReadQuery(command);

        if (table == null) return;

        foreach (DataRow row in table.Rows) {
            if (row != null) {
                RolePicker.Items.Add(row["name"].ToString());
            }
        }
    }
    private void OnPickerChanged(object sender, EventArgs e) {
        // get role_id by name
        string role_query = "SELECT role_id FROM Roles WHERE name = @role_name;";
        var role_command = new SqlCommand(role_query);
        role_command.Parameters.AddWithValue("@role_name", RolePicker.Items[RolePicker.SelectedIndex]);
        int? r_id = Convert.ToInt32(DatabaseConnector.ExecuteScalarQuery(role_command));

        string query = "SELECT number_limit FROM Event_Role WHERE event_ID = @event_id AND role_ID = @role_id;";
        var command = new SqlCommand(query);
        command.Parameters.AddWithValue("@event_id", ev_data.event_id);
        command.Parameters.AddWithValue("@role_id", r_id.Value);
        int? num_limit = Convert.ToInt32(DatabaseConnector.ExecuteScalarQuery(command));

        EventManager.role_data _r_data = new EventManager.role_data {
            role_id = r_id.Value,
            name = RolePicker.Items[RolePicker.SelectedIndex],
            number_limit = num_limit.Value,
        };
        r_data = _r_data;

        var ev_manager = new EventManager(Resources, RoleStack, Navigation);
        ev_manager.add_role_manage(_r_data);
    }

    private async void AddUser(object sender, EventArgs e) {
        UserData userData = await UserService.UService.GetCurrentUserAsync();

        if (userData != null) {
            var selectCmd = new SqlCommand("SELECT volunteer_id FROM Volunteer WHERE email = @email");
            selectCmd.Parameters.AddWithValue("@email", userData.Email);

            int? volunteerId = Convert.ToInt32(DatabaseConnector.ExecuteScalarQuery(selectCmd));
            if (volunteerId == null) {
                Debug.WriteLine("User not found in database!");
                return;
            }

            foreach (var child in RoleStack.Children) {
                if (child is RoleManageCard roleCard) {
                    var command = new SqlCommand(@"INSERT INTO Volunteer_Event(event_id, volunteer_id, role_id, time_begin, time_end) 
                                               VALUES(@event_id, @volunteer_id, @role_id, @time_begin, @time_end)");

                    command.Parameters.AddWithValue("@event_id", ev_data.event_id);
                    command.Parameters.AddWithValue("@volunteer_id", volunteerId);
                    command.Parameters.AddWithValue("@role_id", r_data.role_id);
                    command.Parameters.AddWithValue("@time_begin", roleCard.TimeBeginPicker);
                    command.Parameters.AddWithValue("@time_end", roleCard.TimeEndPicker);

                    Debug.WriteLine(roleCard.TimeBeginPicker.ToString());
                    _ = DatabaseConnector.ExecuteNonQuery(command);
                }
            }

            Debug.WriteLine("USER CONNECTED TO EVENT!");
        }
    }

    private void OnCloseClicked(object sender, EventArgs e) {
        if (this.Parent is Layout parentLayout) {
            parentLayout.Children.Remove(this);
        }
    }

    private Color GetRandomColor() {
        var random = new Random();
        return Color.FromRgb(random.Next(100, 256), random.Next(100, 256), random.Next(100, 256));
    }
}