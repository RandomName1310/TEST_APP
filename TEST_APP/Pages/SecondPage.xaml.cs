using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using TEST_APP;

namespace TEST_APP.Pages;

public partial class ManagePage : ContentPage
{
    public ManagePage()
    {
        InitializeComponent();
        LoadEvents();
    }

    private async void LoadEvents()
    {
        var readerService = new XmlReaderService();
        string[,] events = await readerService.ReadXml("Events.xml");

        CreateEvents(events);
    }

    private void CreateEvents(string[,] events)
    {
        for(int i = 0; i < events.GetLength(0); i++)
        {
            string Title = events[i, 0];
            string Date = events[i, 1];
            string BorderColor = events[i, 2];

            AddEvent(Title, Date, BorderColor);
        }
    }

    public void AddEvent(string M_Text, string D_Text, string BorderColor)
    {
        var mainLabel = new Label
        {
            Style = (Style)Resources["MainText"],
            Text = M_Text,
            HorizontalOptions = LayoutOptions.Start
        };

        var dateLabel = new Label
        {
            Style = (Style)Resources["DateText"],
            Text = D_Text,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.End
        };

        var button = new Button
        {
            Style = (Style)Resources["GeneralButtonStyle"],
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Start, 
        };
        button.Clicked += ClickAnim;

        var textStack = new VerticalStackLayout();
        textStack.Children.Add(mainLabel);
        textStack.Children.Add(dateLabel);
        textStack.Children.Add(button); 


        var border = new Border
        {
            Style = (Style)Resources["BorderStyle"],
            BackgroundColor = Color.FromArgb(BorderColor),
            Content = textStack
        };

        EventStackLayout.Children.Add(border);
    }


    private async void ClickAnim(object sender, EventArgs e)
    {
        var button = (Button)sender;

        await button.ScaleTo(0.8, 60, Easing.Linear);
        await button.ScaleTo(1.0, 60, Easing.Linear);

        await Navigation.PushAsync(new DescriptPage());
    }


    /*private async void GoToDescription(object sender, EventArgs e)
      {
          await Navigation.PushAsync(new DescriptionPage());
      }*/

    private Color GetRandomColor()
    {
        var random = new Random();
        return Color.FromRgb(random.Next(100, 256), random.Next(100, 256), random.Next(100, 256));
    }
}
