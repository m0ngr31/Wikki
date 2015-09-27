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
using Windows.UI.Xaml.Documents;
using wikia_Unofficial.Common;

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

                object sectionTemplate;
                this.Resources.TryGetValue("sectionTemplate", out sectionTemplate);

                HubSection newSection = new HubSection();

                //System.Text.StringBuilder sectionContent = new System.Text.StringBuilder();

                List<ArticleSectionParagraph> sectionContent = new List<ArticleSectionParagraph>();

                UserControl bob3 = new UserControl();
                Common.RichTextColumns bob2 = new Common.RichTextColumns();

                DataTemplate bob4 = new DataTemplate();
                RichTextBlockOverflow bob5 = new RichTextBlockOverflow();

                foreach (JToken section in results)
                {
                    ArticleSection articleSection = JsonConvert.DeserializeObject<ArticleSection>(section.ToString());

                    ArticleSectionParagraph sectionParagraph = new ArticleSectionParagraph();
                    sectionParagraph.Size = 14;

                    if (articleSection.Level < 3)
                    {
                        newSection = new HubSection();
                        TextBlock headerText = new TextBlock();
                        headerText.FontSize = 28;
                        headerText.Text = articleSection.Title;
                        headerText.Margin = new Thickness { Left = 10, Top = 0, Right = 0, Bottom = 0 };
                        newSection.Header = headerText;
                        newSection.ContentTemplate = sectionTemplate as DataTemplate;

                        articleView.Sections.Add(newSection);

                        sectionContent = new List<ArticleSectionParagraph>();
                    }
                    else
                    {
                        ArticleSectionParagraph sectionHeader = new ArticleSectionParagraph();
                        sectionHeader.Text += articleSection.Title.ToString();
                        sectionHeader.Text += "\n\n";
                        sectionHeader.Size = 20;
                        sectionContent.Add(sectionHeader);
                    }

                    foreach(SectionContent content in articleSection.Content)
                    {
                        if (content.Type == "paragraph")
                        {
                            //sectionContent.AppendLine(content.Text.ToString());
                            //sectionContent.AppendLine();
                            sectionParagraph.Text += content.Text.ToString() + "\n\n";
                        } else if (content.Type == "list")
                        {
                            foreach(ListElement subcontent in content.Elements)
                            {
                                //sectionContent.AppendLine("   \x2022 " + subcontent.Text.ToString());
                                sectionParagraph.Text += "   \x2022 " + subcontent.Text.ToString() + "\n";

                                foreach (ListElement subList in subcontent.Elements)
                                {
                                    //sectionContent.AppendLine("      \x2023 " + subList.Text.ToString());
                                    sectionParagraph.Text += "      \x2023 " + subList.Text.ToString() + "\n";
                                }
                            }
                        }
                    }

                    RichTextBlock bob = new RichTextBlock();
                    Paragraph bob1 = new Paragraph();

                    bob.Blocks.Add(bob1);

                    sectionContent.Add(sectionParagraph);

                    //newSection.DataContext = sectionContent.ToString();
                    newSection.DataContext = sectionContent;
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
