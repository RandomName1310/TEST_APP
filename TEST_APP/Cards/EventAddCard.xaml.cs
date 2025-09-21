using System.Diagnostics.Tracing;
using TEST_APP.Services;
using TEST_APP.Pages;
namespace TEST_APP.Cards;

public partial class EventAddCard : ContentView {
	private EventManager.event_data data;
	public EventAddCard(EventManager.event_data evData) {
		InitializeComponent();
		data = evData;
		BindingContext = evData;
	}
    public async void GoToEvent(object sender, EventArgs e) {
        var button = (Button)sender;

        await button.ScaleTo(0.8, 60, Easing.Linear);
        await button.ScaleTo(1.0, 60, Easing.Linear);

        // go to next page
        await Navigation.PushAsync(new DescriptPage(data));
    }
}