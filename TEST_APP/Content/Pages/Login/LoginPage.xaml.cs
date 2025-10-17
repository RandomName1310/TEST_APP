using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Data;
using TEST_APP.Services;
namespace TEST_APP.Pages.Login;

public partial class LoginPage : ContentPage {
    public LoginPage() {
        InitializeComponent();
    }

    public async void CreateUser(object sender, EventArgs e) {
        ClickAnim(sender);

        string query = "SELECT * FROM Volunteer WHERE email = @email AND password = @password";
        var command = new SqlCommand(query);
        command.Parameters.AddWithValue("@email", EmailEntry.Text);
        command.Parameters.AddWithValue("@password", PasswordEntry.Text);

        DataTable table = DatabaseConnector.ExecuteReadQuery(command);

        if (table.Rows.Count > 0) {
            var userData = new UserData {
                Name = table.Rows[0]["name"].ToString(),
                Age = Convert.ToInt32(table.Rows[0]["age"]),
                Email = table.Rows[0]["email"].ToString(),
                Password = table.Rows[0]["password"].ToString()
            };
            await UserService.UService.SetCurrentUserAsync(userData);

            // display sucess message
            await DisplayAlert(
            "Conectado com sucesso na conta!",
            $"Bem vindo(a) {table.Rows[0]["name"].ToString()}",
            "Concluir");
        }
        else {
            // display fail message
            await DisplayAlert(
            "Resultado de login",
            "Email ou senha incorretos!",
            "Continuar");
        }              
    }

    async void ClickAnim(object sender) {
        if (sender is Button btn) {
            await btn.ScaleTo(0.9, 100, Easing.CubicOut);
            await btn.ScaleTo(1.0, 100, Easing.CubicOut);
        }
    }
}