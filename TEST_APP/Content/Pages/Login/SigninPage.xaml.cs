using System.Data;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using TEST_APP.Services;
namespace TEST_APP.Pages.Login;

public partial class SigninPage : ContentPage {
    private byte[]? user_image = null;
    public SigninPage() {
        InitializeComponent();
    }

    public async void CreateUser(object sender, EventArgs e) {
        ClickAnim(sender);

        // be sure that user_image is populated
        if (user_image == null) {
            if (UserImage.Source is FileImageSource fileSource) {
                byte[] buffer = await File.ReadAllBytesAsync(fileSource.File);
                user_image = buffer;
            }
        }

        string image_path = UserService.UserImgService.ToBase64(user_image);

        // Monta os dados do usuário
        var userData = new UserData {
            Name = NameEntry.Text,
            Age = int.Parse(AgeEntry.Text),
            Email = EmailEntry.Text,
            Password = PasswordEntry.Text,
            UserImg = image_path, 
        };

        await UserService.UService.SetCurrentUserAsync(userData);

        // Query SQL
        string query = "INSERT INTO Volunteer(name, age, email, password, user_img) " +
                       "VALUES(@name, @age, @email, @password, @user_img);";

        var command = new SqlCommand(query);
        command.Parameters.AddWithValue("@name", userData.Name);
        command.Parameters.AddWithValue("@age", userData.Age);
        command.Parameters.AddWithValue("@email", userData.Email);
        command.Parameters.AddWithValue("@password", userData.Password);
        command.Parameters.AddWithValue("@user_img", user_image);

        DatabaseConnector.ExecuteNonQuery(command);

        await DisplayAlert("Resultado de sign in", "Conta criada com sucesso!", "Continuar");

        await Navigation.PushAsync(new MainPage());
    }


    private async void OnPickImageClicked(object sender, EventArgs e) {
        try {
            var result = await FilePicker.PickAsync(new PickOptions {
                PickerTitle = "Selecione uma imagem",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null) {
                UserImage.Source = ImageSource.FromFile(result.FullPath);
                user_image = await UserService.UserImgService.ImgToBytesAsync(result);
            }
        } catch (Exception ex) {
            await DisplayAlert("Erro", $"Não foi possível carregar a imagem: {ex.Message}", "OK");
        }
    }

    async void ClickAnim(object sender) {
        if (sender is Button btn) {
            await btn.ScaleTo(0.9, 100, Easing.CubicOut);
            await btn.ScaleTo(1.0, 100, Easing.CubicOut);
        }
    }
}