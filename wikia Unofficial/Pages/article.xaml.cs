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
using Windows.UI.Xaml.Markup;
using System.Text.RegularExpressions;

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

        private String beginningTemplate()
        {
            System.Text.StringBuilder hubTemplate = new System.Text.StringBuilder();

            hubTemplate.Append("<DataTemplate ");
            hubTemplate.Append("xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" ");
            hubTemplate.Append("xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" ");
            hubTemplate.Append("xmlns:local=\"using:wikia_Unofficial.Pages\" ");
            hubTemplate.Append("xmlns:common=\"using:wikia_Unofficial.Common\" ");
            hubTemplate.Append("xmlns:triggers=\"using:WindowsStateTriggers\">");
            hubTemplate.Append("<UserControl>");
            hubTemplate.Append("<common:RichTextColumns>");
            hubTemplate.Append("<VisualStateManager.VisualStateGroups>");
            hubTemplate.Append("<VisualStateGroup>");
            hubTemplate.Append("<VisualState x:Name=\"desktop\">");
            hubTemplate.Append("<VisualState.StateTriggers>");
            hubTemplate.Append("<triggers:DeviceFamilyStateTrigger DeviceFamily=\"Desktop\" />");
            hubTemplate.Append("</VisualState.StateTriggers>");
            hubTemplate.Append("<VisualState.Setters>");
            hubTemplate.Append("<Setter Target=\"richTB.Width\" Value=\"400\" />");
            hubTemplate.Append("<Setter Target=\"richTB.Margin\" Value=\"10,0,0,10\" />");
            hubTemplate.Append("</VisualState.Setters>");
            hubTemplate.Append("</VisualState>");
            hubTemplate.Append("<VisualState x:Name=\"mobile\">");
            hubTemplate.Append("<VisualState.StateTriggers>");
            hubTemplate.Append("<triggers:DeviceFamilyStateTrigger DeviceFamily=\"Mobile\" />");
            hubTemplate.Append("</VisualState.StateTriggers>");
            hubTemplate.Append("<VisualState.Setters>");
            hubTemplate.Append("<Setter Target=\"richTB.Width\" Value=\"Auto\" />");
            hubTemplate.Append("<Setter Target=\"richTB.Margin\" Value=\"10,0,10,10\" />");
            hubTemplate.Append("</VisualState.Setters>");
            hubTemplate.Append("</VisualState>");
            hubTemplate.Append("</VisualStateGroup>");
            hubTemplate.Append("</VisualStateManager.VisualStateGroups>");
            hubTemplate.Append("<common:RichTextColumns.ColumnTemplate>");
            hubTemplate.Append("<DataTemplate>");
            hubTemplate.Append("<RichTextBlockOverflow Width=\"400\" Margin=\"20,0,0,10\" />");
            hubTemplate.Append("</DataTemplate>");
            hubTemplate.Append("</common:RichTextColumns.ColumnTemplate>");
            hubTemplate.Append("<RichTextBlock TextAlignment=\"Justify\" TextWrapping=\"WrapWholeWords\" Name=\"richTB\">");
            
            return hubTemplate.ToString();
        }

        private String endingTemplate()
        {
            System.Text.StringBuilder hubTemplate = new System.Text.StringBuilder();

            hubTemplate.Append("</RichTextBlock>");
            hubTemplate.Append("</common:RichTextColumns>");
            hubTemplate.Append("</UserControl>");
            hubTemplate.Append("</DataTemplate>");

            return hubTemplate.ToString();
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

        static string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings.
            try
            {
                return Regex.Replace(strIn, @"[^\w\.@-@ ]", "",
                                     RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            // If we timeout when replacing invalid characters, 
            // we should return Empty.
            catch (RegexMatchTimeoutException)
            {
                return String.Empty;
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
                System.Text.StringBuilder sectionContent = new System.Text.StringBuilder();

                foreach (JToken section in results)
                {
                    ArticleSection articleSection = JsonConvert.DeserializeObject<ArticleSection>(section.ToString());

                    if (articleSection.Level < 3)
                    {
                        newSection = new HubSection();
                        TextBlock headerText = new TextBlock();
                        headerText.FontSize = 28;
                        headerText.Text = CleanInput(articleSection.Title);
                        headerText.Margin = new Thickness { Left = 10, Top = 0, Right = 0, Bottom = 0 };
                        newSection.Header = headerText;

                        if(articleSection.Title.ToString() != "References" && articleSection.Title.ToString() != "See also" && articleSection.Title.ToString() != "External links")
                            articleView.Sections.Add(newSection);
                        
                        sectionContent = new System.Text.StringBuilder();
                    }
                    else
                    {
                        sectionContent.Append("<Paragraph FontSize=\"20\" FontWeight=\"Bold\"><Run>" + CleanInput(articleSection.Title.ToString()) + "\n</Run></Paragraph>");
                    }

                    foreach(SectionContent content in articleSection.Content)
                    {
                        if (content.Type == "paragraph")
                        {
                            sectionContent.Append("<Paragraph><Run>" + CleanInput(content.Text.ToString()) + "</Run></Paragraph><Paragraph></Paragraph>");
                        } else if (content.Type == "list")
                        {
                            foreach(ListElement subcontent in content.Elements)
                            {
                                sectionContent.Append("<Paragraph Margin=\"10,0,0,0\"><Run>\x2022 " + CleanInput(subcontent.Text.ToString()) + "</Run></Paragraph>");

                                foreach (ListElement subList in subcontent.Elements)
                                {
                                    sectionContent.Append("<Paragraph Margin=\"20,0,0,0\"><Run>\x2023 " + CleanInput(subList.Text.ToString()) + "</Run></Paragraph>");
                                }
                            }
                        }
                    }

                    newSection.ContentTemplate = (DataTemplate)XamlReader.Load(beginningTemplate() + sectionContent.ToString() + endingTemplate());
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
