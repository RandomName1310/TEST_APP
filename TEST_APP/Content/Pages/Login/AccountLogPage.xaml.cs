using Microsoft.Maui.Controls;
namespace TEST_APP.Pages.Login;

public partial class AccountLogPage : ContentPage {
    public AccountLogPage() {
        InitializeComponent();
    }

    public async void GoToLogin(object sender, EventArgs e) {
        await Navigation.PushAsync(new LoginPage());
    }

    public async void GoToSignin(object sender, EventArgs e) {
        await Navigation.PushAsync(new SigninPage());
    }
}