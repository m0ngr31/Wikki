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
using NotificationsExtensions.Toasts;
using Windows.UI.Notifications;

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

        ToastContent content = new ToastContent()
        {
            Launch = "added",

            Visual = new ToastVisual()
            {
                TitleText = new ToastText()
                {
                    Text = "Added"
                },

                BodyTextLine1 = new ToastText()
                {
                    Text = "Added wiki to favorites."
                }
            }
        };

        private IList<WikiSearchResult> WikiSearchResults;

        private List<String> SearchIcons;

        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        public static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(min, max);
            }
        }

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
                var list = await client.GetAsync("http://www.wikia.com/api/v1/Wikis/ByString?expand=1&string=" + searchString + "&limit=25&batch=1&includeDomain=true");

                list.EnsureSuccessStatusCode();

                var jsonString = await list.Content.ReadAsStringAsync();
                JObject searchResult = JObject.Parse(jsonString);
                IList<JToken> results = searchResult["items"].Children().ToList();

                foreach (JToken result in results)
                {
                    WikiSearchResult wikiSearchResult = JsonConvert.DeserializeObject<WikiSearchResult>(result.ToString());

                    if (Uri.IsWellFormedUriString(wikiSearchResult.Image, UriKind.Absolute))
                        wikiSearchResult.Image_Uri = new Uri(wikiSearchResult.Image);
                    else
                    {
                        string rand = SearchIcons[RandomNumber(0, SearchIcons.Count)];
                        wikiSearchResult.Image_Uri = new Uri(rand);
                    }

                    if (wikiSearchResult.Headline != null)
                        wikiSearchResult.Prefered_Name = wikiSearchResult.Headline;
                    else
                        wikiSearchResult.Prefered_Name = wikiSearchResult.Title;

                    if (wikiSearchResult.Desc != null && wikiSearchResult.Desc.Length > 0)
                        wikiSearchResult.Desc = wikiSearchResult.Desc.Split('.')[0];
                    else
                        wikiSearchResult.Desc = "No Description";

                    if (wikiSearchResult.Hub == null || wikiSearchResult.Hub.Length == 0)
                        wikiSearchResult.Hub = "No Hub";

                    WikiSearchResults.Add(wikiSearchResult);
                }

                if (WikiSearchResults.Count > 0)
                {
                    selectVisibility("searchList");

                    searchList.ItemsSource = WikiSearchResults;
                    searchList.ScrollIntoView(searchList.Items[0]);

                    searchBox.IsEnabled = false;
                    searchBox.IsEnabled = true;
                }
                else
                    selectVisibility("noResults");
            }
            catch (Exception ex)
            {
                selectVisibility("errorMsg");
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void gotoWiki(object sender, RoutedEventArgs e)
        {
            var clicked = sender as MenuFlyoutItem;
            if(clicked != null)
            {
                var wiki = clicked.DataContext as WikiSearchResult;
                if(wiki != null)
                {
                    this.Frame.Navigate(typeof(subdomain), wiki);
                }
            }
        }

        private void addToFavorites(object sender, RoutedEventArgs e)
        {
            //TODO: Show message saying it was successful or not

            var menuFlyoutItem = sender as MenuFlyoutItem;
            if (menuFlyoutItem != null)
            {
                var wiki = menuFlyoutItem.DataContext as WikiSearchResult;
                if (wiki != null)
                {
                    using (var db = new wikiaModels())
                    {
                        if(!db.Wikis.Any(a => a.WikiId == wiki.Id))
                        {
                            var newFavorite = new Wiki { WikiId = wiki.Id, Url = wiki.Url, WikiName = wiki.Prefered_Name };

                            db.Wikis.Add(newFavorite);
                            db.SaveChanges();

                            var doc = new ToastNotification(content.GetXml());
                            doc.ExpirationTime = DateTime.Now.AddSeconds(2);
                            ToastNotificationManager.CreateToastNotifier().Show(doc);
                        }
                    }
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SearchIcons = new List<String>(new String[] {
                "ms-appx:///Assets/SearchIcons/blue.png",
                "ms-appx:///Assets/SearchIcons/green.png",
                "ms-appx:///Assets/SearchIcons/light-purple.png",
                "ms-appx:///Assets/SearchIcons/orange.png",
                "ms-appx:///Assets/SearchIcons/purple.png",
                "ms-appx:///Assets/SearchIcons/red.png"
            });
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var clicked = sender as Grid;
            if (clicked != null)
            {
                var wiki = clicked.DataContext as WikiSearchResult;
                if (wiki != null)
                {
                    this.Frame.Navigate(typeof(subdomain), wiki);
                }
            }
        }
    }
}
