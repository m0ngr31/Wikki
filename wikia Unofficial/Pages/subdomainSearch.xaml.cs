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
    public sealed partial class subdomainSearch : Page
    {
        public subdomainSearch()
        {
            this.InitializeComponent();
        }

        public WikiSearchResult wiki;
        private IList<ArticleSearchResult> SearchResults;

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
                if (showMe == "defaultMsg")
                    defaultMsg.Visibility = Visibility.Visible;
                else if (showMe == "errorMsg")
                    errorMsg.Visibility = Visibility.Visible;
                else if (showMe == "nointernetMsg")
                    nointernetMsg.Visibility = Visibility.Visible;
                else if (showMe == "noResults")
                    noResults.Visibility = Visibility.Visible;
                else if (showMe == "searchList")
                    searchList.Visibility = Visibility.Visible;
                else if (showMe == "loading")
                    loading.Visibility = Visibility.Visible;
            }
        }

        private async void searchWikia()
        {
            selectVisibility("loading");

            SearchResults = new List<ArticleSearchResult>();

            try
            {
                var searchString = Uri.EscapeDataString(searchBox.Text);

                var client = new HttpClient();
                var list = await client.GetAsync(wiki.Url + "api/v1/Search/List/?query=" + searchString + "&limit=25&batch=1");

                list.EnsureSuccessStatusCode();

                var jsonString = await list.Content.ReadAsStringAsync();
                JObject searchResult = JObject.Parse(jsonString);
                IList<JToken> results = searchResult["items"].Children().ToList();

                //System.Diagnostics.Debug.WriteLine(searchResult["items"]);

                foreach (JToken result in results)
                {
                    ArticleSearchResult wikiSearchResult = JsonConvert.DeserializeObject<ArticleSearchResult>(result.ToString());

                    if (Uri.IsWellFormedUriString(wikiSearchResult.Thumbnail, UriKind.Absolute))
                        wikiSearchResult.Image_Uri = new Uri(wikiSearchResult.Thumbnail);
                    else
                    {
                        string rand = SearchIcons[RandomNumber(0, SearchIcons.Count)];
                        wikiSearchResult.Image_Uri = new Uri(rand);
                    }

                    SearchResults.Add(wikiSearchResult);
                }

                if (SearchResults.Count > 0)
                {
                    selectVisibility("searchList");

                    searchList.ItemsSource = SearchResults;
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

            if (wiki != null)
            {
                searchBox.PlaceholderText = "search " + wiki.Prefered_Name;
            } else
            {
                selectVisibility("errorMsg");
                searchGrid.Visibility = Visibility.Collapsed;
            }
        }

        override protected void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
                wiki = (WikiSearchResult)e.Parameter;
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var clicked = sender as Grid;
            if (clicked != null)
            {
                var article = clicked.DataContext as ArticleSearchResult;
                if (article != null)
                {
                    var passedArticle = new ArticlePage(article, wiki);
                    this.Frame.Navigate(typeof(article), passedArticle);
                }
            }
        }
    }
}
