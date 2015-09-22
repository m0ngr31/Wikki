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
    public sealed partial class home : Page
    {
        public home()
        {
            this.InitializeComponent();
        }

        private List<String> SearchIcons;
        private IList<WikiSearchResult> WikiSearchResults;

        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        public static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(min, max);
            }
        }

        private void selectVisibility(string showMe = "")
        {
            noWikis.Visibility = Visibility.Collapsed;
            errorMsg.Visibility = Visibility.Collapsed;
            nointernetMsg.Visibility = Visibility.Collapsed;
            showWikis.Visibility = Visibility.Collapsed;
            loading.Visibility = Visibility.Collapsed;

            if (showMe.Length > 0)
            {
                if (showMe == "noWikis")
                    noWikis.Visibility = Visibility.Visible;
                else if (showMe == "errorMsg")
                    errorMsg.Visibility = Visibility.Visible;
                else if (showMe == "nointernetMsg")
                    nointernetMsg.Visibility = Visibility.Visible;
                else if (showMe == "showWikis")
                    showWikis.Visibility = Visibility.Visible;
                else if (showMe == "loading")
                    loading.Visibility = Visibility.Visible;
            }
        }

        private static bool IsConnectedToInternet()
        {
            ConnectionProfile connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            return (connectionProfile != null && connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
        }

        private void checkSize()
        {
            using (var db = new wikiaModels())
            {
                bool isWikis = db.Wikis.Any();

                if (isWikis)
                {
                    selectVisibility("loading");
                    List<Wiki> wikiList = db.Wikis.ToList();
                    if (IsConnectedToInternet())
                        parseWikis(wikiList);
                    else
                        selectVisibility("nointernetMsg");
                } else
                {
                    selectVisibility("noWikis");
                }
            }
        }

        private async void parseWikis(List<Wiki> wikiList)
        {
            try {
                List<int> wikiIdsList = new List<int>();
                WikiSearchResults = new List<WikiSearchResult>();

                foreach (Wiki result in wikiList)
                {
                    wikiIdsList.Add(result.WikiId);
                }

                var wikiIds = string.Join(",", wikiIdsList.ToArray());
                var urlString = new Uri("http://www.wikia.com/api/v1/Wikis/Details?ids=" + wikiIds);

                var client = new HttpClient();
                var favoriteWikis = await client.GetAsync(urlString);

                favoriteWikis.EnsureSuccessStatusCode();

                var jsonString = await favoriteWikis.Content.ReadAsStringAsync();
                JObject searchResult = JObject.Parse(jsonString);
                IList<JToken> results = searchResult["items"].Values().ToList();

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

                showWikis.ItemsSource = WikiSearchResults;
                showWikis.ScrollIntoView(showWikis.Items[0]);

                selectVisibility("showWikis");
            }
            catch (Exception ex)
            {
                selectVisibility("errorMsg");
                System.Diagnostics.Debug.WriteLine(ex);
            }

        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;

            if(MySplitView.IsPaneOpen)
                title.Visibility = Visibility.Collapsed;
            else
                title.Visibility = Visibility.Visible;
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

            checkSize();
        }

        private void goto_Search(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(search));
        }

        private void delete_Wiki(object sender, RoutedEventArgs e)
        {
            var menuFlyoutItem = sender as Button;
            if (menuFlyoutItem != null)
            {
                var wiki = menuFlyoutItem.DataContext as WikiSearchResult;
                if (wiki != null)
                {
                    using (var db = new wikiaModels())
                    {
                        var deleteMe = db.Wikis.Single(a => a.WikiId == wiki.Id);
                        
                        db.Wikis.Remove(deleteMe);
                        db.SaveChanges();
                    }
                    //TODO Add check to see if this is the last one, and if it is, fire off checkSize(). If not, just remove it from the list.
                    checkSize();
                }
            }
        }

        private void goto_Wiki(object sender, TappedRoutedEventArgs e)
        {
            var clicked = sender as Grid;
            if(clicked != null)
            {
                var wiki = clicked.DataContext as WikiSearchResult;
                if(wiki != null)
                {
                    this.Frame.Navigate(typeof(subdomain), wiki);
                }
            }
        }
    }
}
