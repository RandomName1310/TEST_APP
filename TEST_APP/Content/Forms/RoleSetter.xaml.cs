using System;
using System.Data;
using System.Xml.Linq;
using Microsoft.Maui.Storage;
using TEST_APP.Services;
using TEST_APP.Cards;
using TEST_APP.Pages;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;

namespace TEST_APP.Content.Forms;

public partial class RoleSetter : ContentView {
    EventManager.event_data ev_data;

    public RoleSetter(EventManager.event_data _ev_data) {
        InitializeComponent();
        ev_data = _ev_data;

        InitEventPicker();
        InitDatePicker();
    }

    private void InitEventPicker() {
        // clear picker before init
        RolePicker.Items.Clear();

        string query = @"
            SELECT r.name
            FROM Event_Role er
            INNER JOIN Roles r ON er.role_ID = r.role_ID
            WHERE er.event_ID = @event_id;
        ";
        var command = new SqlCommand(query);
        command.Parameters.AddWithValue("@event_id", ev_data.event_id);
        DataTable table = DatabaseConnector.ExecuteReadQuery(command);

        if (table == null) return;

        foreach (DataRow row in table.Rows) {
            if (row != null) {
                RolePicker.Items.Add(row["name"].ToString());
            }
        }
    }

    private void InitDatePicker() {
        // clear picker before init
        DataPicker.Items.Clear();

        string query = @"
            SELECT date FROM Events WHERE event_ID = @event_id
        ";

        var command = new SqlCommand(query);
        command.Parameters.AddWithValue("@event_id", ev_data.event_id);
        DateOnly? _date = DateOnly.FromDateTime(Convert.ToDateTime(DatabaseConnector.ExecuteScalarQuery(command)));

        if (_date == null)
            return;

        DataPicker.Items.Add(_date.ToString());
    }

    private void OnPickerChanged(object sender, EventArgs e) {
        // get role_id by name
        string role_query = "SELECT role_ID FROM Roles WHERE name = @role_name;";
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
        var ev_manager = new EventManager(Resources, RoleStack, Navigation);
        ev_manager.add_role_manage(_r_data);
    }

    private async void AddUser(object sender, EventArgs e) {
        UserData userData = await UserService.UService.GetCurrentUserAsync();

        if (userData != null) {
            var selectCmd = new SqlCommand("SELECT volunteer_ID FROM Volunteer WHERE email = @email");
            selectCmd.Parameters.AddWithValue("@email", userData.Email);

            int? volunteerId = Convert.ToInt32(DatabaseConnector.ExecuteScalarQuery(selectCmd));
            if (volunteerId == null) {
                Debug.WriteLine("User not found in database!");
                return;
            }

            foreach (var child in RoleStack.Children) {
                if (child is RoleManageCard roleCard) {
                    // getting role_id 
                    var role_command = new SqlCommand(@"SELECT role_ID FROM Roles WHERE name = @name");
                    role_command.Parameters.AddWithValue("@name", roleCard.RoleName);

                    int role_id = Convert.ToInt32(DatabaseConnector.ExecuteScalarQuery(role_command));

                    //adding user
                    var command = new SqlCommand(@"INSERT INTO Volunteer_Event(event_ID, volunteer_ID, role_ID, time_begin, time_end, date) 
                                               VALUES(@event_id, @volunteer_id, @role_id, @time_begin, @time_end, @date)");

                    command.Parameters.AddWithValue("@event_id", ev_data.event_id);
                    command.Parameters.AddWithValue("@volunteer_id", volunteerId);
                    command.Parameters.AddWithValue("@role_id", role_id);
                    command.Parameters.AddWithValue("@time_begin", roleCard.TimeBeginPicker);
                    command.Parameters.AddWithValue("@time_end", roleCard.TimeEndPicker);
                    command.Parameters.AddWithValue("@date", DataPicker.Items[0]);

                    DatabaseConnector.ExecuteNonQuery(command);

                    // reduce number of volunteers in role by 1
                    var delete_command = new SqlCommand(@"UPDATE Event_Role SET number_limit = number_limit - 1 WHERE event_ID = @event_id AND role_ID = @role_id");
                    delete_command.Parameters.AddWithValue("@event_id", ev_data.event_id);
                    delete_command.Parameters.AddWithValue("@role_id", role_id);

                    DatabaseConnector.ExecuteNonQuery(delete_command);
                }
            }

            // debugs
            await Application.Current.MainPage.DisplayAlert("Status de inscrição", "Inscrição efetuada com sucesso!", "OK");
            Debug.WriteLine("USER CONNECTED TO EVENT!");

            // close
            if (this.Parent is Layout parentLayout) {
                parentLayout.Children.Remove(this);
            }
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