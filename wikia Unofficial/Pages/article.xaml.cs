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
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.Networking.Connectivity;
using Windows.System;
using Windows.Graphics.Display;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace wikia_Unofficial.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class article : Page
    {
        public article()
        {
            this.InitializeComponent();
        }

        public ArticlePage page;
        private IList<ArticleSection> Article;

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void searchWiki_Click(object sender, RoutedEventArgs e)
        {
            if (page.Wiki != null)
            {
                this.Frame.Navigate(typeof(subdomainSearch), page.Wiki);
            }
        }

        async private void openInIE_Click(object sender, RoutedEventArgs e)
        {
            var url = page.Wiki.Url + page.Article.Url;
            await Launcher.LaunchUriAsync(new Uri(url));
        }

        private static bool IsConnectedToInternet()
        {
            ConnectionProfile connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            return (connectionProfile != null && connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
        }

        private void selectVisibility(string showMe = "")
        {
            errorMsg.Visibility = Visibility.Collapsed;
            nointernetMsg.Visibility = Visibility.Collapsed;
            loading.Visibility = Visibility.Collapsed;
            articleView.Visibility = Visibility.Collapsed;

            if (showMe.Length > 0)
            {
                if (showMe == "errorMsg")
                    errorMsg.Visibility = Visibility.Visible;
                else if (showMe == "nointernetMsg")
                    nointernetMsg.Visibility = Visibility.Visible;
                else if (showMe == "loading")
                    loading.Visibility = Visibility.Visible;
                else if (showMe == "articleView")
                    articleView.Visibility = Visibility.Visible;
            }
        }

        async private void loadArticle()
        {
            selectVisibility("loading");

            Article = new List<ArticleSection>();

            try
            {
                var articleClient = new HttpClient();
                var articleList = await articleClient.GetAsync(page.Wiki.Url + "api/v1/Articles/AsSimpleJson?id=" + page.Article.Id);
                articleList.EnsureSuccessStatusCode();

                var jsonString = await articleList.Content.ReadAsStringAsync();
                JObject result = JObject.Parse(jsonString);
                IList<JToken> results = result["sections"].Children().ToList();

                foreach(JToken section in results)
                {
                    ArticleSection articleSection = JsonConvert.DeserializeObject<ArticleSection>(section.ToString());

                    HubSection newSection = new HubSection();

                    TextBlock headerText = new TextBlock();
                    headerText.FontSize = 28;
                    headerText.Text = articleSection.Title;
                    headerText.Margin = new Thickness {Left = 10, Top = 0, Right = 0, Bottom = 0};
                    newSection.Header = headerText;

                    List<TextBlock> sectionContent = new List<TextBlock>();

                    foreach(SectionContent content in articleSection.Content)
                    {
                        if (content.Type == "paragraph")
                        {
                            TextBlock newParagraph = new TextBlock();
                            newParagraph.Text = content.Text;
                            sectionContent.Add(newParagraph);
                        }
                        //System.Diagnostics.Debug.WriteLine(content.Text);
                    }

                    //DataTemplate sectionTemplate = new DataTemplate() { VisualTree = sectionContent };
                    String text1 = "There are also several multiplayer improvements. Bleszinski promised Gears of War 3 will include better region filtering to allow for less laggy games. Fans also saw the return of Gridlock. The biggest addition to multiplayer is 4-player online co-op in the story mode.\n\nGears of War 3 includes a mode called Beast.It is the opposite of Horde Mode. In Horde Mode, you are the COG combating the Locust, but in Beast, you are the Locust combating the COG.In Beast mode, you earn money for killing your enemies.Once you get enough money, you are able to upgrade your character to a specific class, such as Boomers, Corpsers and even Berserkers. A new Multiplayer mode called \"Capture The Leader\" is a combination of Guardian and Submission.The Leader has an improved Tat-com which can see their enemies no matter where they are.There are no capture points in this new game, just hold the enemy's leader as the timer counts down. There is a \"newbie\" playlist where if you have no Gears achievements or little xp, you will play in a playlist with other \"newbies\". After a little while of playing you cannot play in this playlist anymore. Gears of War 3 also features unlockables by achievements similar to Gears of War 2. Achievement unlocks include completing Gears 1 campaign and Gears 2 campaign and reaching level 100 in multiplayer.[5] this game also have update.";

                    object sectionTemplate;
                    this.Resources.TryGetValue("sectionTemplate", out sectionTemplate);
                    newSection.ContentTemplate = sectionTemplate as DataTemplate;
                    newSection.DataContext = text1;

                    articleView.Sections.Add(newSection);
                    Article.Add(articleSection);
                }

                
                selectVisibility("articleView");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                selectVisibility("errorMsg");
            }
        }

        override protected void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
                page = (ArticlePage)e.Parameter;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (page != null)
            {
                title.Text = page.Article.Title;

                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                    articleView.Orientation = Orientation.Vertical;

                if (IsConnectedToInternet())
                    loadArticle();
                else
                    selectVisibility("nointernetMsg");
            } else
            {
                selectVisibility("errorMsg");
            }
        }
    }
}
