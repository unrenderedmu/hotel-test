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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MaterialDesignThemes.Wpf;

namespace Hotel
{
    public partial class MessageBoxYesNo : UserControl
    {
        bool dialogResult = false;
        public MessageBoxYesNo(bool result)
        {
            InitializeComponent();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            dialogResult = true;
            DialogHost.CloseDialogCommand.Execute(dialogResult, null);
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            dialogResult = false;
            DialogHost.CloseDialogCommand.Execute(dialogResult, null);
        }
    }
}
