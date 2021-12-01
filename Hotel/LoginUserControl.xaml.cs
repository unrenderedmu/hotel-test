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
    public partial class LoginUserControl : UserControl
    {
        bool dialogResult = false;

        DataTable datatable;
        //string str = Properties.Settings.Default.connString;
        string str = Properties.Settings.Default.connStringH;

        DispatcherTimer timer = new DispatcherTimer();

        string loginCheck = "";
        string passwordCheck = "";

        public LoginUserControl()
        {
            InitializeComponent();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            ShowClosingDialog();
        }

        private async void ShowClosingDialog()
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
            
            var closingDialogContent = new MessageBoxYesNo(dialogResult);
            object result = await DialogHost.Show(closingDialogContent, "ClosingDialog");
            if ((bool)result == true) Application.Current.Shutdown();
            else
            {
                var loginDialogContent = new LoginUserControl();
                object res = await DialogHost.Show(loginDialogContent, "LoginDialog");
            }
        }

        private void registerLink_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RegisterDialog();
        }

        private async void RegisterDialog()
        {
            DialogHost.CloseDialogCommand.Execute(null, null);

            var registerDialogContent = new RegisterUserControl();
            object result = await DialogHost.Show(registerDialogContent, "RegisterDialog");
        }

        private void loginBTN_Click(object sender, RoutedEventArgs e)
        {
            loginAttempt();
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        public bool FieldCheck()
        {
            // Уведомление об ошибке, если пользователь не ввёл логин и пароль одновременно
            if (string.IsNullOrEmpty(loginTB.Text) && string.IsNullOrEmpty(passwordTB.Password.ToString()))
            {
                MessageBox.Show("Для входа в систему необходимо ввести логин и пароль",
                    "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            // Уведомление об ошибке, если пользователь не ввёл логин
            if (string.IsNullOrEmpty(loginTB.Text))
            {
                MessageBox.Show("Для входа в систему необходимо ввести логин.",
                    "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            // Уведомление об ошибке, если пользователь не ввёл пароль
            if (string.IsNullOrEmpty(passwordTB.Password.ToString()))
            {
                MessageBox.Show("Для входа в систему необходимо ввести пароль.",
                    "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        public void loginAttempt()
        {
            if (FieldCheck()) authorization();
            else return;
        }

        public void authorization()
        {
            //Переменные для использования содержимого БД-----------------------------------------------------------
            SqlConnection connection = null;
            //SqlCommand command;
            datatable = new DataTable();

            if (loginTB.Text == "" || passwordTB.Password == "")
            {
                MessageBox.Show
                    ("Не все поля заполнены", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            else
            {
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
                    SqlCommand command = new SqlCommand("Auth", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@login", SqlDbType.NVarChar, 40));
                    command.Parameters[0].Value = loginTB.Text;
                    SqlDataReader reader = command.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        MessageBox.Show("Вы ввели неверный логин/пароль или такого аккаунта не существует, проверьте корректность данных.", "Ошибка входа",
                            MessageBoxButton.OK, MessageBoxImage.Error);

                        loginTB.Clear();
                        passwordTB.Clear();
                        return;
                    }

                    reader.Read();
                    loginCheck = reader.GetString(0);
                    passwordCheck = reader.GetString(1);

                    if (passwordTB.Password == passwordCheck)
                    {
                        DialogHost.CloseDialogCommand.Execute(loginCheck, null);
                    }
                    else
                    {
                        MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        loginTB.Clear();
                        passwordTB.Clear();
                        return;
                    }
                    reader.Close();
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
}
