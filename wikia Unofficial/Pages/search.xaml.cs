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
using wikia_Unofficial.Models;
using Windows.System;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.Networking.Connectivity;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace wikia_Unofficial.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class search : Page
    {
        public search()
        {
            this.InitializeComponent();
        }

        private IList<WikiSearchResult> WikiSearchResults;
 
        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            searchGlyph.Foreground = (Application.Current.Resources["SystemControlPageTextChromeBlackMediumLowBrush"] as Brush);
            borderLine.Height = 10;
            borderLine.Margin = new Thickness(15, -17, 10, 0);

            //Close commandBar when there is a focus
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                commandBar.Visibility = Visibility.Collapsed;
        }

        private void searchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            searchGlyph.Foreground = (Application.Current.Resources["SystemControlPageTextBaseMediumBrush"] as Brush);
            borderLine.Height = 4;
            borderLine.Margin = new Thickness(15, -10, 10, 0);

            //Open commandBar back up
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
               commandBar.Visibility = Visibility.Visible;
        }

        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            searchBox.Focus(FocusState.Programmatic);
        }

        private static bool IsConnectedToInternet()
        {
            ConnectionProfile connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            return (connectionProfile != null && connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
        }

        private void searchBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter && searchBox.Text.Length > 0)
            {
                if (IsConnectedToInternet())
                    searchWikia();
                else
                    selectVisibility("nointernetMsg");

            }
        }

        private void selectVisibility(string showMe = "")
        {
            defaultMsg.Visibility = Visibility.Collapsed;
            errorMsg.Visibility = Visibility.Collapsed;
            nointernetMsg.Visibility = Visibility.Collapsed;
            noResults.Visibility = Visibility.Collapsed;
            searchList.Visibility = Visibility.Collapsed;
            loading.Visibility = Visibility.Collapsed;

            if (showMe.Length > 0)
            {
               if(showMe == "defaultMsg")
                    defaultMsg.Visibility = Visibility.Visible;
               else if(showMe == "errorMsg")
                    errorMsg.Visibility = Visibility.Visible;
               else if(showMe == "nointernetMsg")
                    nointernetMsg.Visibility = Visibility.Visible;
               else if(showMe == "noResults")
                    noResults.Visibility = Visibility.Visible;
               else if(showMe == "searchList")
                    searchList.Visibility = Visibility.Visible;
                else if (showMe == "loading")
                    loading.Visibility = Visibility.Visible;
            }
        }

        private async void searchWikia()
        {
            selectVisibility("loading");

            WikiSearchResults = new List<WikiSearchResult>();

            try {
                var searchString = Uri.EscapeDataString(searchBox.Text);

                var client = new HttpClient();
                var list = await client.GetAsync("http://www.wikia.com/api/v1/Wikis/ByString?string=" + searchString + "&limit=25&batch=1&includeDomain=true");

                list.EnsureSuccessStatusCode();

                var jsonString = await list.Content.ReadAsStringAsync();

                JObject searchResult = JObject.Parse(jsonString);

                IList<JToken> results = searchResult["items"].Children().ToList();

                foreach (JToken result in results)
                {
                    WikiSearchResult wikiSearchResult = JsonConvert.DeserializeObject<WikiSearchResult>(result.ToString());
                    WikiSearchResults.Add(wikiSearchResult);
                }

                searchList.ItemsSource = WikiSearchResults;

                if (WikiSearchResults.Count > 0)
                {
                    selectVisibility("searchList");
                    searchBox.IsEnabled = false;
                    searchBox.IsEnabled = true;
                    searchList.ScrollIntoView(searchList.Items[0]);
                }
                else
                    selectVisibility("noResults");
            }
            catch (Exception ex)
            {
                selectVisibility("errorMsg");
            }
        }

        private void TextBlock_Holding(object sender, HoldingRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var menuFlyoutItem = sender as MenuFlyoutItem;
            if (menuFlyoutItem != null)
            {
                var wiki = menuFlyoutItem.DataContext as WikiSearchResult;
                if(wiki != null)
                {
                    System.Diagnostics.Debug.WriteLine(wiki.Name);
                }
            }
        }

        private void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            var menuFlyoutItem = sender as MenuFlyoutItem;
            if (menuFlyoutItem != null)
            {
                var wiki = menuFlyoutItem.DataContext as WikiSearchResult;
                if (wiki != null)
                {
                    System.Diagnostics.Debug.WriteLine(wiki.Name);
                }
            }
        }
    }
}
