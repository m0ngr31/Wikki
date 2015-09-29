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
using Windows.System;
using Windows.UI.Xaml.Navigation;
using wikia_Unofficial.Models;
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
    public sealed partial class subdomain : Page
    {
        public subdomain()
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

        public WikiSearchResult wiki;

        private List<String> SearchIcons;

        private IList<ArticleSearchResult> PopularArticles;
        private IList<ArticleSearchResult> NewArticles;
        private IList<ArticleSearchResult> TopArticles;

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

        private void selectVisibility(string showMe = "")
        {
            errorMsg.Visibility = Visibility.Collapsed;
            nointernetMsg.Visibility = Visibility.Collapsed;
            loading.Visibility = Visibility.Collapsed;
            articlesView.Visibility = Visibility.Collapsed;

            if (showMe.Length > 0)
            {
                if (showMe == "errorMsg")
                    errorMsg.Visibility = Visibility.Visible;
                else if (showMe == "nointernetMsg")
                    nointernetMsg.Visibility = Visibility.Visible;
                else if (showMe == "loading")
                    loading.Visibility = Visibility.Visible;
                else if (showMe == "articlesView")
                    articlesView.Visibility = Visibility.Visible;
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
                title.Text = wiki.Prefered_Name;

                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                {
                    articlesView.Orientation = Orientation.Vertical;
                    PopularList.Width = Double.NaN;
                    TopList.Width = Double.NaN;
                    NewList.Width = Double.NaN;
                }
                    
                if (IsConnectedToInternet())
                    loadArticles();
                else
                    selectVisibility("nointernetMsg");
            } else
            {
                selectVisibility("errorMsg");
            }
        }

        private static bool IsConnectedToInternet()
        {
            ConnectionProfile connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            return (connectionProfile != null && connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
        }

        private async void loadArticles()
        {
            selectVisibility("loading");

            PopularArticles = new List<ArticleSearchResult>();
            NewArticles = new List<ArticleSearchResult>();
            TopArticles = new List<ArticleSearchResult>();

            try
            {
                var popularClient = new HttpClient();
                var popularList = await popularClient.GetAsync(wiki.Url + "api/v1/Articles/Popular?expand=1&limit=10");
                popularList.EnsureSuccessStatusCode();

                var newClient = new HttpClient();
                var newList = await newClient.GetAsync(wiki.Url + "api/v1/Articles/New?limit=25");
                newList.EnsureSuccessStatusCode();

                var topClient = new HttpClient();
                var topList = await topClient.GetAsync(wiki.Url + "api/v1/Articles/Top?expand=1&limit=25");
                topList.EnsureSuccessStatusCode();

                var jsonStringPopular = await popularList.Content.ReadAsStringAsync();
                JObject popularResult = JObject.Parse(jsonStringPopular);
                IList<JToken> popularResults = popularResult["items"].Children().ToList();

                var jsonStringNew = await newList.Content.ReadAsStringAsync();
                JObject newResult = JObject.Parse(jsonStringNew);
                IList<JToken> newResults = newResult["items"].Children().ToList();

                var jsonStringTop = await topList.Content.ReadAsStringAsync();
                JObject topResult = JObject.Parse(jsonStringTop);
                IList<JToken> topResults = topResult["items"].Children().ToList();

                foreach (JToken result in popularResults)
                {
                    ArticleSearchResult articleSearchResult = JsonConvert.DeserializeObject<ArticleSearchResult>(result.ToString());

                    if (Uri.IsWellFormedUriString(articleSearchResult.Thumbnail, UriKind.Absolute))
                        articleSearchResult.Image_Uri = new Uri(articleSearchResult.Thumbnail);
                    else
                    {
                        string rand = SearchIcons[RandomNumber(0, SearchIcons.Count)];
                        articleSearchResult.Image_Uri = new Uri(rand);
                    }

                    if (articleSearchResult.Type == "article" || articleSearchResult.Type == "Article")
                        PopularArticles.Add(articleSearchResult);
                }

                foreach (JToken result in newResults)
                {
                    ArticleSearchResult articleSearchResult = JsonConvert.DeserializeObject<ArticleSearchResult>(result.ToString());

                    if (Uri.IsWellFormedUriString(articleSearchResult.Thumbnail, UriKind.Absolute))
                        articleSearchResult.Image_Uri = new Uri(articleSearchResult.Thumbnail);
                    else
                    {
                        string rand = SearchIcons[RandomNumber(0, SearchIcons.Count)];
                        articleSearchResult.Image_Uri = new Uri(rand);
                    }

                    //if (articleSearchResult.Type == "article" || articleSearchResult.Type == "Article")
                    NewArticles.Add(articleSearchResult);
                }

                foreach (JToken result in topResults)
                {
                    ArticleSearchResult articleSearchResult = JsonConvert.DeserializeObject<ArticleSearchResult>(result.ToString());

                    if (Uri.IsWellFormedUriString(articleSearchResult.Thumbnail, UriKind.Absolute))
                        articleSearchResult.Image_Uri = new Uri(articleSearchResult.Thumbnail);
                    else
                    {
                        string rand = SearchIcons[RandomNumber(0, SearchIcons.Count)];
                        articleSearchResult.Image_Uri = new Uri(rand);
                    }

                    if(articleSearchResult.Type == "article" || articleSearchResult.Type == "Article")
                        TopArticles.Add(articleSearchResult);
                }

                PopularList.DataContext = PopularArticles;
                TopList.DataContext = TopArticles;
                NewList.DataContext = NewArticles;
                
                selectVisibility("articlesView");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                System.Diagnostics.Debug.WriteLine(wiki.Url);
                selectVisibility("errorMsg");
            }
        }

        override protected void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                wiki = (WikiSearchResult)e.Parameter;

                using (var db = new wikiaModels())
                {
                    if (db.Wikis.Any(a => a.WikiId == wiki.Id))
                    {
                        favoriteButton.Visibility = Visibility.Collapsed;
                    }
                }
            }
                
        }

        private void searchWiki_Click(object sender, RoutedEventArgs e)
        {
            if (wiki != null)
            {
                this.Frame.Navigate(typeof(subdomainSearch), wiki);
            }
        }

        async private void openInIE_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(wiki.Url));
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

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (wiki != null)
            {
                using (var db = new wikiaModels())
                {
                    if (!db.Wikis.Any(a => a.WikiId == wiki.Id))
                    {
                        var newFavorite = new Wiki { WikiId = wiki.Id, Url = wiki.Url, WikiName = wiki.Prefered_Name };

                        db.Wikis.Add(newFavorite);
                        db.SaveChanges();

                        var doc = new ToastNotification(content.GetXml());
                        doc.ExpirationTime = DateTime.Now.AddSeconds(2);
                        ToastNotificationManager.CreateToastNotifier().Show(doc);

                        favoriteButton.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }
    }
}
