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
    public partial class UserControlCheckOut : UserControl
    {
        //Определение глобальных переменных-------------------------------------------------------------------------
        SqlDataReader reader;
        DataTable datatable;
        SqlDataAdapter adapter;
        //string str = Properties.Settings.Default.connString;
        string str = Properties.Settings.Default.connStringH;

        string passportCombo = "";
        public UserControlCheckOut()
        {
            InitializeComponent();
        }

        //-------------------------------------------------------------------------------------------------Фильтр ввода для Пробела
        private void SpacePreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) e.Handled = true;
        }
        //-------------------------------------------------------------------------------------------------Фильтр ввода только букв
        private void LetterPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(Char.IsLetter(e.Text, 0));
        }
        //-------------------------------------------------------------------------------------------------Фильтр ввода только цифр
        private void DigitPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(Char.IsDigit(e.Text, 0));
        }

        private void checkOutBTN_Click(object sender, RoutedEventArgs e)
        {
            FieldsCheck();
            ClearAllFields();
        }

        public void ClearAllFields()
        {
            surnameTB.Clear();
            givenNameTB.Clear();
            patronymicTB.Clear();
            passportSerialTB.Clear();
            passportNumTB.Clear();
            roomsCB.Items.Clear();
        }

        public void FieldsCheck()
        {
            if (surnameTB.Text == "" || givenNameTB.Text == "" || passportCombo == "" || roomsCB.Text == "" || passportCombo.Length != 11)
            {
                MessageBox.Show("Не все поля заполнены!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else CheckOut();
        }

        public void FillRooms()
        {
            //---------------------------------------------------------------------------------------------Некоторые переменные для использования содержимого БД
            SqlConnection connection = null;
            SqlCommand command;

            string sqlxpRoom = ($"select distinct rRoomID as [#] from rooms, status, guests, journal where jRoomID = rRoomID and jGuestID = gGuestID and rStatusID = sStatusID and gPassportID = '{passportCombo}'");
            //---------------------------------------------------------------------------------------------comboboxRoomIN
            try
            {
                connection = new SqlConnection(str);
                connection.Open();
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось подключиться к базе данных");
            }
            try
            {
                command = new SqlCommand(sqlxpRoom, connection);

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int roomid = reader.GetInt32(0);
                    roomsCB.Items.Add(roomid);
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
            //---------------------------------------------------------------------------------------------
        }

        public void CheckOut()
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
                command = new SqlCommand("CheckOut", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@passport", SqlDbType.NVarChar));
                command.Parameters[0].Value = passportCombo;

                command.Parameters.Add(new SqlParameter("@roomID", SqlDbType.Int));
                command.Parameters[1].Value = int.Parse(roomsCB.Text);

                MessageBoxResult res;
                res = MessageBox.Show
                ($"Подтвердите операцию:\nВыселить {surnameTB.Text + " " + givenNameTB.Text + " " + patronymicTB.Text} из номера {roomsCB.Text}? ", "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    command.ExecuteNonQuery();
                    MessageBox.Show($"{surnameTB.Text + " " + givenNameTB.Text + " " + patronymicTB.Text} успешно выселен(а) из номера {roomsCB.Text}", "Успех!");
                }
                else return;
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

        private void passportTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (passportNumTB.Text == "" || passportSerialTB.Text == "") passportCombo = "";
            else passportCombo = passportSerialTB.Text + "-" + passportNumTB.Text;

            if (passportCombo.Length == 11) roomsCB.IsEnabled = true;
            else roomsCB.IsEnabled = false;
        }

        private void clearBTN_Click(object sender, RoutedEventArgs e)
        {
            ClearAllFields();
        }

        private void roomsCB_DropDownOpened(object sender, EventArgs e)
        {
            roomsCB.Items.Clear();
            FillRooms();
        }
    }
}
