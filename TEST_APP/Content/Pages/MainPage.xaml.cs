using System.Diagnostics;
using TEST_APP.Services;
using TEST_APP.Pages.Login;
using System.Buffers.Text;

namespace TEST_APP.Pages
{
    public partial class MainPage : ContentPage
    {
        static private bool hasChosenIp = false;

        public MainPage(){
            InitializeComponent();
        }

        private async Task ChooseIp() {
            string ip = await DisplayPromptAsync(
                "Configuração de IP",
                "Digite up IP válido:",
                accept: "OK",
                cancel: "Cancelar",
                placeholder: "IP aqui..."
            );
            DatabaseConnector.SetIp(ip);
        }

        protected override async void OnAppearing() {
            base.OnAppearing();
            // set user image
            if (UserService.UService.currentUser != null) {
                string user_img = UserService.UService.currentUser.UserImg;
                if (user_img != null) {
                    byte[] buffer = UserService.UserImgService.ToByteList(user_img);
                    string path = UserService.UserImgService.ByteToImage(buffer);
                    UserImage.Source = path;
                }
            }
            if (!hasChosenIp)
            {
                hasChosenIp = true;
                await Task.Delay(300);
                await ChooseIp();
            }
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
