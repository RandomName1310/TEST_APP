using System.Diagnostics;
using TEST_APP.Services;
using TEST_APP.Pages.Login;

namespace TEST_APP.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(){
            InitializeComponent();
        }

        private async void GoToSecondPage(object sender, EventArgs e){
            await Navigation.PushAsync(new SecondPage());
        }
        private async void GoToManagePage(object sender, EventArgs e)
        {
            if(UserService.UService.currentUser == null) {
                await DisplayAlert(
                    "Usuário desconhecido",
                    "Crie uma conta para prosseguir",
                    "Continuar");
                return;
            }
            await Navigation.PushAsync(new ManagePage());
        }
        private async void GoToLoginPage(object sender, EventArgs e) {
            if (UserService.UService.currentUser != null) {
                await Navigation.PushAsync(new AccountPage());
            }
            else {
                await Navigation.PushAsync(new AccountLogPage());
            }
        }
    }
}
