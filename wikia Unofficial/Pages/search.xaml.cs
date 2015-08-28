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

        private void searchBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter && searchBox.Text.Length > 0)
            {
                //TODO: Check internet status before making the call
                //TODO: Create loading indicator and show it as soon as the call is made
                searchWikia();
            }
        }

        private async void searchWikia()
        {
            WikiSearchResults = new List<WikiSearchResult>();

            var client = new HttpClient();

            var searchString = searchBox.Text.Replace(" ", "+");
            var list = await client.GetAsync("http://www.wikia.com/api/v1/Wikis/ByString?string=" + searchString + "&limit=25&batch=1&includeDomain=true");
            var jsonString = await list.Content.ReadAsStringAsync();

            JObject searchResult = JObject.Parse(jsonString);

            IList<JToken> results = searchResult["items"].Children().ToList();

            foreach(JToken result in results)
            {
                WikiSearchResult wikiSearchResult = JsonConvert.DeserializeObject<WikiSearchResult>(result.ToString());
                WikiSearchResults.Add(wikiSearchResult);
            }

            searchList.ItemsSource = WikiSearchResults;

            //TODO: Show sad face message if nothing was found: WikiSearchResults.Count == 0

            //Close the keyboard programatically
            //TODO only make it on successful searches
            searchBox.IsEnabled = false;
            searchBox.IsEnabled = true;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            WikiSearchResults = new List<WikiSearchResult>();
        }
    }
}
