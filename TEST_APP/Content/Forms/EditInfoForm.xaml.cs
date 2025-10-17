using Microsoft.Data.SqlClient;
using TEST_APP.Services;
using TEST_APP.Pages.Login;

namespace TEST_APP.Content.Forms;

public partial class EditInfoForm : ContentView
{
	string data;
	public EditInfoForm(string _data)
	{
		InitializeComponent();
		data = _data;
        InfoEntry.Placeholder = _data;
    }
    private async void OnEditClicked(object sender, EventArgs e) {
        var command = new SqlCommand($"UPDATE Volunteer SET {data} = @value WHERE email = @email AND password = @password");
        command.Parameters.AddWithValue("@value", InfoEntry.Text);
        command.Parameters.AddWithValue("@email", UserService.UService.currentUser.Email);
        command.Parameters.AddWithValue("@password", UserService.UService.currentUser.Password);

        int collumns_affected = DatabaseConnector.ExecuteNonQuery(command);
        if(collumns_affected > 1)
            await Application.Current.MainPage.DisplayAlert("Status da edição", "Edição feita com sucesso!","OK");
    }
    
    private void OnCloseClicked(object sender, EventArgs e) {
        if (this.Parent is Layout parentLayout) {
            parentLayout.Children.Remove(this);
        }
    }
}