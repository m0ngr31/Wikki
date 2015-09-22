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

        public WikiSearchResult wiki;

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void selectVisibility(string showMe = "")
        {
            errorMsg.Visibility = Visibility.Collapsed;
            nointernetMsg.Visibility = Visibility.Collapsed;
            loading.Visibility = Visibility.Collapsed;

            if (showMe.Length > 0)
            {
                if (showMe == "errorMsg")
                    errorMsg.Visibility = Visibility.Visible;
                else if (showMe == "nointernetMsg")
                    nointernetMsg.Visibility = Visibility.Visible;
                else if (showMe == "loading")
                    loading.Visibility = Visibility.Visible;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (wiki != null)
            {
                title.Text = wiki.Prefered_Name;
            } else
            {
                selectVisibility("errorMsg");
            }
                
        }

        override protected void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
                wiki = (WikiSearchResult)e.Parameter;
        }

        private void searchWiki_Click(object sender, RoutedEventArgs e)
        {

        }

        async private void openInIE_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(wiki.Url));
        }
    }
}
