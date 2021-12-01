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
    public partial class RegisterUserControl : UserControl
    {
        bool dialogResult = false;
        string passportCombo = "", phoneCombo = "";

        DataTable datatable;
        //string str = Properties.Settings.Default.connString;
        string str = Properties.Settings.Default.connStringH;

        public RegisterUserControl()
        {
            InitializeComponent();
        }
        //Фильтр ввода для Пробела------------------------------------------------------------------
        private void SpacePreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) e.Handled = true;
        }
        //Фильтр ввода только букв------------------------------------------------------------------
        private void LetterPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(Char.IsLetter(e.Text, 0));
        }
        //Фильтр ввода только цифр-------------------------------------------------------------------
        private void DigitPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(Char.IsDigit(e.Text, 0));
        }
        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            OpenLoginDialog();
        }

        private void registerBTN_Click(object sender, RoutedEventArgs e)
        {
            FieldCheck();
        }
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            ShowClosingDialog();
        }

        private async void OpenLoginDialog()
        {
            DialogHost.CloseDialogCommand.Execute(null, null);

            var loginDialogContent = new LoginUserControl();
            object result = await DialogHost.Show(loginDialogContent, "LoginDialog");
        }
        private async void ShowClosingDialog()
        {
            DialogHost.CloseDialogCommand.Execute(null, null);

            var closingDialogContent = new MessageBoxYesNo(dialogResult);
            object result = await DialogHost.Show(closingDialogContent, "ClosingDialog");
            if ((bool)result == true) Application.Current.Shutdown();
            else OpenLoginDialog();

        }

        private void passportTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (passportNumTB.Text == "" || passportSerialTB.Text == "") passportCombo = "";
            else passportCombo = passportSerialTB.Text + "-" + passportNumTB.Text;
        }

        private void phoneTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (phoneTB.Text == "") phoneCombo = "";
            else phoneCombo = "8-" + phoneTB.Text;
        }

        public void FieldCheck()
        {
            //Уведомление об ошибке, если пользователь не заполнил все нужные поля
            if (string.IsNullOrEmpty(loginTB.Text) || string.IsNullOrEmpty(passwordTB.Password.ToString()) || string.IsNullOrEmpty(surnameTB.Text) || string.IsNullOrEmpty(givenNameTB.Text) || string.IsNullOrEmpty(passportCombo))
            {
                MessageBox.Show("Не все поля заполнены",
                    "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else availabilityCheck();
        }
        public void availabilityCheck()
        {
            //Переменные для использования содержимого БД-----------------------------------------------------------
            SqlConnection connection = null;
            //SqlCommand command;
            datatable = new DataTable();

            //Подключение к базе данных-----------------------------------------------------------------------------
            try
            {
                connection = new SqlConnection(str);
                connection.Open();
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось подключиться к базе данных");
            }
            //Загрузка данных---------------------------------------------------------------------------------------
            try
            {
                SqlCommand command = new SqlCommand("availabilityCheck", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@passport", SqlDbType.NVarChar, 40));
                command.Parameters[0].Value = passportCombo;
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    MessageBox.Show("Вы уже зарегистрированы. Используйте другие паспортные данные или выполните вход.", "Ошибка ввода",
                        MessageBoxButton.OK, MessageBoxImage.Error);

                    loginTB.Clear();
                    passwordTB.Clear();
                    return;
                }
                else RegisterUser();
            }
            finally
            {
                if (connection != null) connection.Close();
            }
        }
        public void RegisterUser()
        {
            SqlConnection connection = null;
            SqlCommand command;

            try
            {
                connection = new SqlConnection(str);
                connection.Open();
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось подключиться к базе данных!");
            }
            try
            {
                command = new SqlCommand("Register", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@surname", SqlDbType.NVarChar));
                command.Parameters[0].Value = surnameTB.Text;

                command.Parameters.Add(new SqlParameter("@givenName", SqlDbType.NVarChar));
                command.Parameters[1].Value = givenNameTB.Text;

                command.Parameters.Add(new SqlParameter("@patronymic", SqlDbType.NVarChar));
                command.Parameters[2].Value = patronymicTB.Text;

                command.Parameters.Add(new SqlParameter("@phone", SqlDbType.NVarChar));
                command.Parameters[3].Value = phoneCombo;

                command.Parameters.Add(new SqlParameter("@passport", SqlDbType.NVarChar));
                command.Parameters[4].Value = passportCombo;

                command.Parameters.Add(new SqlParameter("@login", SqlDbType.NVarChar));
                command.Parameters[5].Value = loginTB.Text;

                command.Parameters.Add(new SqlParameter("@password", SqlDbType.NVarChar));
                command.Parameters[6].Value = passwordTB.Password;

                command.ExecuteNonQuery();

                OpenLoginDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection != null) connection.Close();
            }
        }
    }
}
