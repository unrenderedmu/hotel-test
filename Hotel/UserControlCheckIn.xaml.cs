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
    public partial class UserControlCheckIn : UserControl
    {
        //Определение глобальных переменных-------------------------------------------------------------------------
        SqlDataReader reader;
        DataTable datatable;
        //string str = Properties.Settings.Default.connString;
        string str = Properties.Settings.Default.connStringH;

        string passportCombo = "", phoneCombo = "";

        public UserControlCheckIn()
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
        private void addBTN_Click(object sender, RoutedEventArgs e)
        {
            FieldsCheck();
            ClearAllFields();
        }
        private void clearBTN_Click(object sender, RoutedEventArgs e)
        {
            ClearAllFields();
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
        private void categoryCB_DropDownOpened(object sender, EventArgs e)
        {
            categoryCB.Items.Clear();
            FillCategory();
        }
        private void bedsCB_DropDownOpened(object sender, EventArgs e)
        {
            bedsCB.Items.Clear();
            FillBeds();
        }
        private void roomsCB_DropDownOpened(object sender, EventArgs e)
        {
            roomsCB.Items.Clear();
            FillRooms();
        }
        private void CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            roomsCB.Items.Clear();
            IndexCheck();
        }

        public void ClearAllFields()
        {
            surnameTB.Clear();
            givenNameTB.Clear();
            patronymicTB.Clear();
            passportSerialTB.Clear();
            passportNumTB.Clear();
            phoneTB.Clear();
            categoryCB.Items.Clear();
            bedsCB.Items.Clear();
            roomsCB.Items.Clear();
            dateFromDP.SelectedDate = null;
            dateToDP.SelectedDate = null;
        }
        public int GetDateDifference()
        {
            DateTime dateFrom = dateFromDP.SelectedDate.Value.Date;
            DateTime dateTo = dateToDP.SelectedDate.Value.Date;
            TimeSpan ts = dateFrom - dateTo;
            int daysDiff = Math.Abs(ts.Days);
            return daysDiff;
        }
        public void FillCategory()
        {
            //Переменные для использования содержимого БД-----------------------------------------------------------
            SqlConnection connection = null;
            SqlCommand command;

            string sqlxpCat = "select distinct rCategory from rooms";
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
            //Заполнение выпадающего списка-------------------------------------------------------------------------
            try
            {
                command = new SqlCommand(sqlxpCat, connection);

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string category = reader.GetString(0);
                    categoryCB.Items.Add(category);
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
            //------------------------------------------------------------------------------------------------------
        }
        public void FillBeds()
        {
            //Переменные для использования содержимого БД-----------------------------------------------------------
            SqlConnection connection = null;
            SqlCommand command;

            string sqlxpCat = "select distinct rBeds from rooms";
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
            //Заполнение выпадающего списка-------------------------------------------------------------------------
            try
            {
                command = new SqlCommand(sqlxpCat, connection);

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int beds = reader.GetInt32(0);
                    bedsCB.Items.Add(beds);
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
            //------------------------------------------------------------------------------------------------------
        }
        public void FillRooms()
        {
            //---------------------------------------------------------------------------------------------Некоторые переменные для использования содержимого БД
            SqlConnection connection = null;
            SqlCommand command;

            string sqlxpRoom = ($"select rRoomID from rooms where rCategory = '{categoryCB.Text}' and rBeds = {int.Parse(bedsCB.Text)} and rStatusID = 1");
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
            if (categoryCB.SelectedIndex != -1 && bedsCB.SelectedIndex != -1 && roomsCB.Items.Count < 1)
            {
                MessageBox.Show("Нет свободных номеров с выбранными критериями :(\nВыберите другие критерии.", "Увы");
                ClearAllFields();
            }
            //---------------------------------------------------------------------------------------------
        }
        public void IndexCheck()
        {
            if (categoryCB.SelectedIndex != -1 && bedsCB.SelectedIndex != -1)
            {
                roomsCB.IsEnabled = true;
            }
            else roomsCB.IsEnabled = false;
        }

        public void FieldsCheck()
        {
            if (surnameTB.Text == "" || givenNameTB.Text == "" || passportCombo == "" || roomsCB.Text == "" || dateFromDP.Text == "" || dateToDP.Text == "" || passportCombo.Length != 11)
            {
                MessageBox.Show("Не все поля заполнены!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else GuestCheck();
        }

        public void GuestCheck()
        {
            //---------------------------------------------------------------------------------------------Некоторые переменные для использования содержимого БД
            SqlConnection connection = null;
            SqlCommand command;

            string sqlxpCheck = ($"select gGuestID from guests where gPassportID = '{passportCombo}'");
            int check = -1;
            
            //---------------------------------------------------------------------------------------------Проверка наличия клиента в БД
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
                command = new SqlCommand(sqlxpCheck, connection);

                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    check = reader.GetInt32(0);
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
            //---------------------------------------------------------------------------------------------Заселение
            if (check == -1) CheckInWithAdd();
            else CheckIn(check);
        }

        public void CheckIn(int check)
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
                MessageBox.Show("Не удалось подключиться к базе данных");
            }
            try
            {
                using (connection)
                {
                    command = new SqlCommand("CheckIn", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@guestID", SqlDbType.Int));
                    command.Parameters[0].Value = check;

                    command.Parameters.Add(new SqlParameter("@roomID", SqlDbType.Int));
                    command.Parameters[1].Value = int.Parse(roomsCB.Text);

                    command.Parameters.Add(new SqlParameter("@dateFrom", SqlDbType.Date));
                    command.Parameters[2].Value = dateFromDP.Text;

                    command.Parameters.Add(new SqlParameter("@diff", SqlDbType.Int));
                    command.Parameters[3].Value = GetDateDifference();
                    //---------------------------------------------------------------------------------------------
                    MessageBoxResult res;
                    res = MessageBox.Show
                    ($"Подтвердите операцию:\nЗаселить {surnameTB.Text + " " + givenNameTB.Text + " " + patronymicTB.Text} в номер {roomsCB.Text}? ", "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                    if (res == MessageBoxResult.Yes)
                    {
                        command.ExecuteNonQuery();
                        MessageBox.Show($"{surnameTB.Text + " " + givenNameTB.Text + " " + patronymicTB.Text} успешно заселен(а) в номер {roomsCB.Text}", "Успех!");
                    }
                    else return;
                }
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

        public void CheckInWithAdd()
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
                command = new SqlCommand("CheckInWithAdd", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@passport", SqlDbType.NVarChar));
                command.Parameters[0].Value = passportCombo;

                command.Parameters.Add(new SqlParameter("@phone", SqlDbType.NVarChar));
                command.Parameters[1].Value = phoneCombo;

                command.Parameters.Add(new SqlParameter("@surname", SqlDbType.NVarChar));
                command.Parameters[2].Value = surnameTB.Text;

                command.Parameters.Add(new SqlParameter("@givenName", SqlDbType.NVarChar));
                command.Parameters[3].Value = givenNameTB.Text;

                command.Parameters.Add(new SqlParameter("@patronymic", SqlDbType.NVarChar));
                command.Parameters[4].Value = patronymicTB.Text;

                command.Parameters.Add(new SqlParameter("@roomID", SqlDbType.Int));
                command.Parameters[5].Value = int.Parse(roomsCB.Text);

                command.Parameters.Add(new SqlParameter("@dateFrom", SqlDbType.Date));
                command.Parameters[6].Value = dateFromDP.Text;

                command.Parameters.Add(new SqlParameter("@diff", SqlDbType.Int));
                command.Parameters[7].Value = GetDateDifference();

                MessageBoxResult res;
                res = MessageBox.Show
                ($"Подтвердите операцию:\nЗаселить {surnameTB.Text + " " +givenNameTB.Text + " " + patronymicTB.Text} в номер {roomsCB.Text}? ", "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    command.ExecuteNonQuery();
                    MessageBox.Show($"{surnameTB.Text + " " +givenNameTB.Text + " " + patronymicTB.Text} успешно заселен(а) в номер {roomsCB.Text}", "Успех!");
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
    }
}
