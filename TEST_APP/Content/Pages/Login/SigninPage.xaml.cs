using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Data;
using TEST_APP.Services;
using Microsoft.Maui.Storage;
namespace TEST_APP.Pages.Login;

public partial class SigninPage : ContentPage {
    public SigninPage() {
        InitializeComponent();
    }

    public async void CreateUser(object sender, EventArgs e) {
        ClickAnim(sender);

        string imagePath = null;

        try {
            if (UserImage.Source is FileImageSource fileSource && File.Exists(fileSource.File)) {
                // User selected an image — copy it to app directory
                var result = new FileResult(fileSource.File);
                imagePath = await UserService.UService.GetUserImgPathAsync(result);
            }
            else {
                // No custom image — copy the default bundled one
                using var stream = await FileSystem.OpenAppPackageFileAsync("user_icon.png");
                string destPath = Path.Combine(FileSystem.AppDataDirectory, "default_user_icon.png");
                using var destStream = File.Create(destPath);
                await stream.CopyToAsync(destStream);
                imagePath = destPath;
            }
        }
        catch (Exception ex) {
            await DisplayAlert("Erro", $"Falha ao carregar imagem de perfil! ({ex.Message})", "Continuar");
            return;
        }

        if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath)) {
            await DisplayAlert("Erro", "Falha ao salvar imagem de perfil!", "Continuar");
            return;
        }

        // Monta os dados do usuário
        var userData = new UserData {
            Name = NameEntry.Text,
            Age = int.Parse(AgeEntry.Text),
            Email = EmailEntry.Text,
            Password = PasswordEntry.Text,
            UserImgPath = imagePath, 
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
        command.Parameters.AddWithValue("@user_img", imagePath);

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
            }
        }
        catch (Exception ex) {
            await DisplayAlert("Erro", $"Não foi possível carregar a imagem: {ex.Message}", "OK");
        }
    }

    private byte[] GetImageBytesAsync(ImageButton imageButton) {
        if (imageButton?.Source is FileImageSource fileSource) {
            return File.ReadAllBytes(fileSource.File);
        }
        else {
            return null;
        }
    }


    async void ClickAnim(object sender) {
        if (sender is Button btn) {
            await btn.ScaleTo(0.9, 100, Easing.CubicOut);
            await btn.ScaleTo(1.0, 100, Easing.CubicOut);
        }
    }
}