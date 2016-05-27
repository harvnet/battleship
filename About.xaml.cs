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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Battleship
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class About : Page
    {
        public About()
        {
            this.InitializeComponent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void HighScore_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HighScore));
        }

        private async void paulButton_Click(object sender, RoutedEventArgs e)
        {
            Uri email = new Uri("mailto:harv0116@algonquinlive.com");
            await Windows.System.Launcher.LaunchUriAsync(email);


        }

        private async void annaButton_Click(object sender, RoutedEventArgs e)
        {
            Uri email = new Uri("mailto:ioud0001@algonquinlive.com");
            await Windows.System.Launcher.LaunchUriAsync(email);
        }
    }
}
