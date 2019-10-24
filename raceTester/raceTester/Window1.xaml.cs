using System;
using System.Configuration;
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
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Diagnostics;

namespace raceTester
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public static int Person_id { get; set; }
        public static string Person_Name { get; set; }
        public static string Person_Surname { get; set; }
        public static string Person_Patronymic { get; set; }
        public static int Test_id { get; set; }
        SqlCommand cmd;
        SqlConnection connection;
        SqlDataAdapter dataAdapter;

        public Window1()
        {
            InitializeComponent();
            var cstr = ConfigurationManager.ConnectionStrings["rtCS"].ConnectionString;
            connection = new SqlConnection(cstr);            
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            cmd = new SqlCommand("SELECT id,name,password, surname, patronymic FROM [user] WHERE name = @name AND password = @password", connection);
            cmd.Parameters.AddWithValue("@name", loginName.Text);
            cmd.Parameters.AddWithValue("@password", loginPass.Password);
            dataAdapter = new SqlDataAdapter(cmd);
            DataTable dtb = new DataTable();
            if (dataAdapter.Fill(dtb) == 1)
            {
                Person_id = int.Parse(dtb.Rows[0][0].ToString());
                Person_Name = dtb.Rows[0][1].ToString();
                Person_Surname = dtb.Rows[0][3].ToString();
                Person_Patronymic = dtb.Rows[0][4].ToString();
                new MainWindow().Show();
                connection.Close();
                Close();
                return;
            }
            else
            {
                MessageBox.Show("Данные введены неверно!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            connection.Close();

        }

        private void Reg_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            Regex names = new Regex(@"^[А-Я]{1}[а-я]{1,19}$");
            if (names.IsMatch(regName.Text) && names.IsMatch(regSurname.Text) && names.IsMatch(regPatronymic.Text) && regPass1.Password == regPass2.Password)
            {
                //name = regName.Text;
                //surname = regSurname.Text;
                //patronymic = regPatronymic.Text;
                //password = regPass1.Password;                
                cmd = new SqlCommand("INSERT INTO [user] (name,surname,patronymic,password) VALUES (@name,@surname,@patronymic,@password)", connection);
                cmd.Parameters.AddWithValue("@name", regName.Text);
                cmd.Parameters.AddWithValue("@surname", regSurname.Text);
                cmd.Parameters.AddWithValue("@patronymic", regPatronymic.Text);
                cmd.Parameters.AddWithValue("@password", regPass1.Password);
                cmd.ExecuteNonQuery();
            }
            else
            {
                MessageBox.Show("Данные введены неверно!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            connection.Close();

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                Process.Start("cafeteriamanager.chm");
            }
        }
    }    
        

}
