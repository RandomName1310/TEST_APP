using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Data;
using TEST_APP.Services;
namespace TEST_APP.Pages.Login;

public partial class SigninPage : ContentPage {
    public SigninPage() {
        InitializeComponent();
    }

    public async void CreateUser(object sender, EventArgs e) {
        ClickAnim(sender);
        var userData = new UserData 
        {
           Name = NameEntry.Text,
           Age = int.Parse(AgeEntry.Text),
           Email = EmailEntry.Text,
           Password = PasswordEntry.Text
        };
        await UserService.UService.SetCurrentUserAsync(userData);

        string query = "INSERT INTO Volunteer(name, age, email, password) VALUES(@name, @age, @email, @password);";
        var command = new SqlCommand(query);
        command.Parameters.AddWithValue("@name", userData.Name);
        command.Parameters.AddWithValue("@age", userData.Age);
        command.Parameters.AddWithValue("@email", userData.Email);
        command.Parameters.AddWithValue("@password", userData.Password);

        _ = DatabaseConnector.ExecuteNonQuery(command);

        // debugs
        await DisplayAlert(
            "Resultado de sign in",              
            "Conta criada com sucesso!", 
            "Continuar");                    
        Debug.WriteLine("New user added!");
    }

    async void ClickAnim(object sender) {
        if (sender is Button btn) {
            await btn.ScaleTo(0.9, 100, Easing.CubicOut);
            await btn.ScaleTo(1.0, 100, Easing.CubicOut);
        }
    }
}