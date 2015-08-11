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
using wikia_Unofficial.Pages;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace wikia_Unofficial
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                title.Margin = new Thickness(0, 0, 0, 400);
                main_button.Margin = new Thickness(0, 400, 0, 0);
            } else
            {
                title.Margin = new Thickness(0, 0, 0, 500);
                main_button.Margin = new Thickness(0, 500, 0, 0);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //main_button.Foreground = new SolidColorBrush(Colors.White);
            this.Frame.Navigate(typeof(home));
        }
    }
}