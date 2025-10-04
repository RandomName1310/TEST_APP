using System.Data;
using TEST_APP.Services;
using Microsoft.Data.SqlClient;
namespace TEST_APP.Cards;

public partial class RoleManageCard : ContentView {
    EventManager.role_data role_data;

    public RoleManageCard(EventManager.role_data _role_data) {
        InitializeComponent();
        role_data = _role_data;
        BindingContext = _role_data;
    }
    public string TimeBeginPicker => TimeBegin.Time.ToString();
    public string TimeEndPicker => TimeEnd.Time.ToString();

    private void OnCloseClicked(object sender, EventArgs e) {
        string query = $"DELETE FROM roles WHERE role_ID = @role_id;";
        var command = new SqlCommand(query);
        command.Parameters.AddWithValue("@email", UserService.UService.currentUser.Email);
        DatabaseConnector.ExecuteNonQuery(command);

        if (this.Parent is Layout parentLayout) {
            parentLayout.Children.Remove(this);
        }
    }
}