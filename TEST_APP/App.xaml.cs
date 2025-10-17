using TEST_APP.Services;
namespace TEST_APP
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            InitUserAsync();
        }

        private async void InitUserAsync() {
            UserData u_data = await UserService.UService.GetCurrentUserAsync();

            if (u_data != null)
                await UserService.UService.SetCurrentUserAsync(u_data);
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {  
            return new Window(new AppShell());
        }
    }
}