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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace wikia_Unofficial.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class search : Page
    {
        public double barHeight;
        public search()
        {
            this.InitializeComponent();
        }
 
        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // SystemControlBackgroundChromeWhiteBrush //
            searchGlyph.Foreground = (Application.Current.Resources["SystemControlHighlightAccentBrush"] as Brush);
            searchGrid.Background = (Application.Current.Resources["SystemControlBackgroundChromeWhiteBrush"] as Brush);
            //Close commandbar when there is a focus
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                commandBar.Height = 0;
        }

        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            searchBox.Focus(FocusState.Programmatic);
        }

        private void searchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                commandBar.Height = barHeight;

            //TextBoxButtonBackgroundThemeBrush
            searchGrid.Background = (Application.Current.Resources["TextBoxButtonBackgroundThemeBrush"] as Brush);
            searchGlyph.Foreground = (Application.Current.Resources["SystemControlPageTextBaseMediumBrush"] as Brush);
        }

        private void commandBar_Loaded(object sender, RoutedEventArgs e)
        {
            barHeight = commandBar.Height;
        }
    }
}
