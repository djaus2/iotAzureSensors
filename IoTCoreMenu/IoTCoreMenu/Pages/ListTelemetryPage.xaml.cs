using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IoTCoreMenu
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ListTelemetryPage : Page
    {
        public ListBox ListItems;
        public ListTelemetryPage()
        {
            //this.OnNavigatedTo += OnNavigatedTo;
            this.NavigationCacheMode =
                Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;
            this.InitializeComponent();
            Globals.LTP = this;
            ListItems = this.listItems;
            TelemetryAPI.SetListTelemetryPage(this);
        }
        protected async override void OnNavigatedTo(
            NavigationEventArgs e)
        {
            string typ = e.Parameter.GetType().ToString();
            if (typ == "string".GetType().ToString())
            {
                string param = (string)e.Parameter;
                if (param == "Current")
                {
                    ShowProgress(true);
                    await Globals.TelemetryApi.RefreshTelemetryItems();
                    ShowProgress(false);
                }
                else if (param == "")
                {
                    // This is just a return
                }
            }
            else if (typ == 137.GetType().ToString())
            {
                int cmdIndex = (int)e.Parameter;
                string cmd = Commands.GetCommand("MainMenu", cmdIndex + 1, 4).name;
                ShowProgress(true);
                await Globals.TelemetryApi.RefreshTelemetryHistory(cmd);
                ShowProgress(false);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage), null);
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            ShowProgress(true);
            await Globals.TelemetryApi.RefreshTelemetryItems();
            ShowProgress(false);
        }

        private void ShowProgress(bool show)
        {
            if (show)
            {
                Progress1.IsActive = true;
                Progress1.Visibility = Visibility.Visible;
            }

            else
            {
                Progress1.IsActive = false;
                Progress1.Visibility = Visibility.Collapsed;
            }
        }
    }
}
