using System.Diagnostics.Tracing;
using TEST_APP.Services;
using TEST_APP.Pages;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Maui.ApplicationModel.DataTransfer;
namespace TEST_APP.Cards;

public partial class EventManageCard : ContentView {
	private EventManager.event_data data;
	public EventManageCard(EventManager.event_data evData) {
		InitializeComponent();
		data = evData;
		BindingContext = evData;
	}
    public async void ExitEvent(object sender, EventArgs e) {
        //animate button 
        if (sender is Button btn) {
            await btn.ScaleTo(0.9, 100, Easing.CubicOut);
            await btn.ScaleTo(1.0, 100, Easing.CubicOut);
        }

        var id_command = new SqlCommand("SELECT volunteer_ID FROM Volunteer WHERE email = @email;");
        id_command.Parameters.AddWithValue("@email", UserService.UService.currentUser.Email);
        DataTable id_table = DatabaseConnector.ExecuteReadQuery(id_command);

        var command = new SqlCommand("DELETE FROM Volunteer_Event WHERE volunteer_ID = @vol_id AND event_ID = @ev_id");
        command.Parameters.AddWithValue("@vol_id", id_table.Rows[0]["volunteer_id"]);
        command.Parameters.AddWithValue("@ev_id", data.event_id);
        _ = DatabaseConnector.ExecuteNonQuery(command);
        
        // recreate events on screen
        var currentPage = (ManagePage)Shell.Current.CurrentPage;
        currentPage.ClearEvents();
        currentPage.ShowEvents();
    }
}