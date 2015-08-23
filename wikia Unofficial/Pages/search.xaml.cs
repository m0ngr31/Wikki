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
    }
}
