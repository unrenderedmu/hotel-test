using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Hotel
{
    public partial class MainWindow : Window
    {
        bool dialogResult = false;
        public MainWindow()
        {
            InitializeComponent();
            
        }
        private void ButtonHide_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            ShowClosingDialog();
        }
        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void ButtonLogout_Click(object sender, RoutedEventArgs e)
        {
            ShowClosingDialog();
        }
        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Visible;
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
        }
        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            ButtonOpenMenu.Visibility = Visibility.Visible;
        }

        private void ButtonAbout_Click(object sender, RoutedEventArgs e)
        {
            ShowAboutDialog();
        }

        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            ShowHelpDialog();
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ListViewMenu.SelectedIndex;
            ellipse.Visibility = Visibility.Visible;
            switch (index)
            {
                case 0:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(new UserControlRooms());
                    break;
                case 1:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(new UserControlCheckIn());
                    break;
                case 2:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(new UserControlCheckOut());
                    break;
                case 3:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(new UserControlClients());
                    break;
                default:
                    break;
            }
        }

        private async void ShowAboutDialog()
        {
            var aboutDialogContent = new UserControlAbout();
            await DialogHost.Show(aboutDialogContent, "AboutDialog");
        }
        private async void ShowHelpDialog()
        {
            var helpDialogContent = new UserControlHelp();
            await DialogHost.Show(helpDialogContent, "HelpDialog");
        }


        private async void ShowClosingDialog()
        {
            var closingDialogContent = new MessageBoxYesNo(dialogResult);
            object result = await DialogHost.Show(closingDialogContent, "ClosingDialog");
            if ((bool)result == true) Application.Current.Shutdown();
        }

        private async void LoginDialog()
        {
            ListViewMenu.SelectedIndex = -1;
            ellipse.Visibility = Visibility.Hidden;
            GridPrincipal.Children.Clear();

            var loginDialogContent = new LoginUserControl();
            object result = await DialogHost.Show(loginDialogContent, "LoginDialog");
        }

        private void ButtonRelog_Click(object sender, RoutedEventArgs e)
        {
            LoginDialog();
        }

        private void ShowLoginDialog(object sender, RoutedEventArgs e)
        {
            LoginDialog();
        }
    }
}