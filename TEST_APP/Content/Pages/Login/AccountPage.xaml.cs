using Microsoft.Data.SqlClient;
using System.Data;
using TEST_APP.Services;
using TEST_APP.Content.Forms;
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

    private async void InitFields() {
        var _currentUser = UserService.UService.currentUser;

        var command = new SqlCommand("SELECT * FROM Volunteer WHERE email = @email AND password = @password");
        command.Parameters.AddWithValue("@email", _currentUser.Email);
        command.Parameters.AddWithValue("@password", _currentUser.Password);

        DataTable table = DatabaseConnector.ExecuteReadQuery(command);

        // set up fields
        if (table != null && table.Rows.Count > 0) {
            NameLabel.Text = $"Nome: {table.Rows[0]["name"].ToString()}";
            AgeLabel.Text = $"Idade: {table.Rows[0]["age"].ToString()}";
            EmailLabel.Text = $"Email: {table.Rows[0]["email"].ToString()}";
            PasswordLabel.Text = $"Senha: {table.Rows[0]["password"].ToString()}";

            if (table.Rows[0]["user_img"] != DBNull.Value) {
                string user_img = table.Rows[0]["user_img"].ToString();
                // Cria um Stream para a imagem
                UserImage.Source = ImageSource.FromFile(user_img);
            }
        }
        else {
            UserService.UService.LogoutUser();
        }
    }
    public Command OnEditCommand => new Command((param) => {
        string value = param.ToString();
        Console.WriteLine($"Parameter received: {value}");
    });

    private async void OnPickImageClicked(object sender, EventArgs e) {
        try {
            var result = await FilePicker.PickAsync(new PickOptions {
                PickerTitle = "Selecione uma imagem",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null) {
                var _currentUser = UserService.UService.currentUser;
                string path = await UserService.UService.GetUserImgPathAsync(result);

                var command = new SqlCommand("UPDATE Volunteer SET user_img = @image WHERE email = @email AND password = @password");

                command.Parameters.AddWithValue("@image", path);
                command.Parameters.AddWithValue("@email", _currentUser.Email);
                command.Parameters.AddWithValue("@password", _currentUser.Password);

                DatabaseConnector.ExecuteNonQuery(command);

                // update user data
                _currentUser.UserImgPath = path;
                UserImage.Source = ImageSource.FromFile(path);
                await UserService.UService.SetCurrentUserAsync(_currentUser);

            }
        }
        catch (Exception ex) {
            await DisplayAlert("Erro", $"Não foi possível carregar a imagem: {ex.Message}", "OK");
        }
    }

    public async void LogoutUser(object sender, EventArgs e) {
        UserService.UService.LogoutUser();
        await Application.Current.MainPage.DisplayAlert("Status de logout", "Conta desconectada com sucesso!", "OK");

        // voltar para MainPage
        await Navigation.PushAsync(new MainPage());
    }
}