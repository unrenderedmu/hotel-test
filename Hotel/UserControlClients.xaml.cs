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
using System.Data;
using System.Data.SqlClient;
using System.Windows.Threading;

namespace Hotel
{
    public partial class UserControlClients : UserControl
    {
        //Определение глобальных переменных-------------------------------------------------------------------------
        SqlDataAdapter adapter;
        DataTable datatable;
        //string str = Properties.Settings.Default.connString;
        string str = Properties.Settings.Default.connStringH;
        bool dialogResult = false;

        public UserControlClients()
        {
            InitializeComponent();
        }

        private void guestsGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(System.DateTime))
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd/MM/yyyy";
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FillTable();
        }

        public void FillTable()
        {
            //Переменные для использования содержимого БД-----------------------------------------------------------
            SqlConnection connection = null;
            SqlCommand command;
            datatable = new DataTable();
            //Подключение к базе данных-----------------------------------------------------------------------------
            try
            {
                connection = new SqlConnection(str.ToString());
                connection.Open();
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось подключиться к базе данных");
            }
            //Загрузка данных---------------------------------------------------------------------------------------
            try
            {
                command = new SqlCommand("viewGuests", connection);
                command.CommandType = CommandType.StoredProcedure;
                adapter = new SqlDataAdapter(command);
                adapter.Fill(datatable);
                guestsGrid.ItemsSource = datatable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection != null) connection.Close();
            }
            //------------------------------------------------------------------------------------------------------
        }

        private void refreshBTN_Click(object sender, RoutedEventArgs e)
        {
            FillTable();
        }
    }
}
