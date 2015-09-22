using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using wikia_Unofficial.Pages;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace wikia_Unofficial.Templates
{
    public sealed partial class SplitPanel : UserControl
    {
        public SplitView MySplitView { get; set; }
        //public event EventHandler ButtonClicked;
        Frame rootFrame = Window.Current.Content as Frame;
        public SplitPanel()
        {
            this.InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
        }

        //private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        //{
        //    MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        //}

        private void goto_About(object sender, RoutedEventArgs e)
        {
            //this.Frame.Navigate(typeof(about));
            rootFrame.Navigate(typeof(about));
        }

        private void goto_Search(object sender, RoutedEventArgs e)
        {
            //this.Frame.Navigate(typeof(search));
            rootFrame.Navigate(typeof(search));
        }

        private void goto_Home(object sender, RoutedEventArgs e)
        {
            //this.Frame.Navigate(typeof(article));
            rootFrame.Navigate(typeof(home));
        }

        public static Brush ColorToBrush(string color)
        {
            color = color.Replace("#", "");
            if (color.Length == 6)
            {
                return new SolidColorBrush(ColorHelper.FromArgb(255,
                    byte.Parse(color.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(color.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(color.Substring(4, 2), System.Globalization.NumberStyles.HexNumber)));
            }
            else
            {
                return null;
            }
        }

        private void slideout_Loaded(object sender, RoutedEventArgs e)
        {
            var currentPage = rootFrame.Content as Page;

            if (currentPage.GetType().Name == "home")
                Home.Background = ColorToBrush("#A7A3A8");
            else if (currentPage.GetType().Name == "search")
                Search.Background = ColorToBrush("#A7A3A8");
            else if (currentPage.GetType().Name == "about")
                About.Background = ColorToBrush("#A7A3A8");
        }
    }
}
