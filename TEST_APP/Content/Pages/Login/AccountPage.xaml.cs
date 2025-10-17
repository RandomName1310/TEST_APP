using System.Diagnostics;
using Microsoft.Data.SqlClient;
using System.Data;
using TEST_APP.Services;
using TEST_APP.Content.Forms;
using System.ComponentModel.Design.Serialization;
namespace TEST_APP.Pages.Login;

public partial class AccountPage : ContentPage {
    public AccountPage() {
        InitializeComponent();
        InitEditFunctions();
        InitFields();
    }
    private void InitEditFunctions() {
        NameButton.Clicked += (s, e) => EditInfo("name");
        AgeButton.Clicked += (s, e) => EditInfo("age");
        EmailButton.Clicked += (s, e) => EditInfo("email");
        PasswordButton.Clicked += (s, e) => EditInfo("password");
    }

    private void EditInfo(string data) {
        EditInfoForm role_setter = new EditInfoForm(data);
        ContentStack.Add(role_setter);
    }

    private void InitFields() {
        var _currentUser = UserService.UService.currentUser;

        var command = new SqlCommand("SELECT * FROM Volunteer WHERE email = @email AND password = @password");
        command.Parameters.AddWithValue("@email", _currentUser.Email);
        command.Parameters.AddWithValue("@password", _currentUser.Password);

        DataTable table = DatabaseConnector.ExecuteReadQuery(command);

        // set up fields
        if (table != null) {
            NameLabel.Text = $"Name: {table.Rows[0]["name"].ToString()}";
            AgeLabel.Text = $"Age: {table.Rows[0]["age"].ToString()}";
            EmailLabel.Text = $"Email: {table.Rows[0]["email"].ToString()}";
            PasswordLabel.Text = $"Password: {table.Rows[0]["password"].ToString()}";
        }
    }
    public Command OnEditCommand => new Command((param) =>
    {
        string value = param.ToString();
        Console.WriteLine($"Parameter received: {value}");
    });

    public async void GoToLogin(object sender, EventArgs e) {
        await Navigation.PushAsync(new LoginPage());
    }

    public async void GoToSignin(object sender, EventArgs e) {
        await Navigation.PushAsync(new SigninPage());
    }
}