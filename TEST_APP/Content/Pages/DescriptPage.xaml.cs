using System.Data;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using TEST_APP.Services;
using TEST_APP.Content.Forms;
using System.ComponentModel.Design.Serialization;

namespace TEST_APP.Pages
{
    public partial class DescriptPage : ContentPage
    {
        EventManager.event_data _data;
        public DescriptPage(EventManager.event_data data)
        {
            InitializeComponent();

            _data = data;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SetDescriptions();
        }

        private void SetDescriptions()
        {
            Title.Text = _data.name;
            Description.Text = _data.description;
            DateTime.Text = _data.date;
            Link.Text = _data.link;
        }

        private void OpenRoleSetter(object sender, EventArgs e) {
            RoleSetter role_setter = new RoleSetter(_data);
            ContentStack.Add(role_setter);
        }

        async void ClickAnim(object sender) {
            if(sender is Button btn) {
                await btn.ScaleTo(0.9, 100, Easing.CubicOut);
                await btn.ScaleTo(1.0, 100, Easing.CubicOut);
            }
        }
    }
}
