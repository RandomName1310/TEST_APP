using System.Diagnostics;

namespace TEST_APP.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(){
            InitializeComponent();
            Debug.WriteLine("DAWG");
        }

        private async void NavigateToSecondPage(object sender, EventArgs e){
            await Navigation.PushAsync(new SecondPage());
        }
        private async void NavigateToManagePage(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ManagePage());
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}
