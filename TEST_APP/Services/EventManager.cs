using System.Data;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using TEST_APP.Pages;
using TEST_APP.Cards;


namespace TEST_APP.Services {
    public class EventManager
    {
        private readonly ResourceDictionary _resources;
        private readonly Layout _eventStackLayout;
        private readonly INavigation _navigation;

        public class event_data {
            public int event_id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public string date { get; set; }
            public string time_begin { get; set; }
            public string time_end { get; set; }
            public string link { get; set; }
            public int number_limit { get; set; }
            public string color { get; set; }
        }

        public class role_data {
            public int role_id { get; set; }
            public string name { get; set; }
            public int number_limit { get; set; }
        }

        public EventManager(ResourceDictionary resources, Layout eventStackLayout, INavigation navigation)
        {
            _resources = resources;
            _eventStackLayout = eventStackLayout;
            _navigation = navigation;
        }

        public void add_event_enter(event_data ev_data)
        {
            EventAddCard card = new EventAddCard(ev_data);
            _eventStackLayout.Children.Add(card);
        }

        public void add_event_manage(event_data ev_data) {
            EventManageCard card = new EventManageCard(ev_data);
            _eventStackLayout.Children.Add(card);
        }
        public void add_role_manage(role_data r_data) {
            RoleManageCard card = new RoleManageCard(r_data);
            _eventStackLayout.Children.Add(card);
        }
    }
}
