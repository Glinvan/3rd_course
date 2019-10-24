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
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

namespace cafeteriaManager
{
    /// <summary>
    /// Логика взаимодействия для auth.xaml
    /// </summary>
    public partial class auth : Window
    {
        SqlCommand cmd;
        SqlConnection connection;
        SqlDataAdapter dataAdapter;
        public static Window AuthWindow;
        public auth()
        {
            InitializeComponent();
            var cstr = ConfigurationManager.ConnectionStrings["cafCS"].ConnectionString;
            connection = new SqlConnection(cstr);
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();          
            cmd = new SqlCommand("SELECT name,surname,patronymic,type,password,id FROM [user] WHERE name = @name AND surname = @surname AND patronymic = @patronymic AND password COLLATE Latin1_General_CS_AS = @password ", connection);
            cmd.Parameters.AddWithValue("@name", loginName.Text);
            cmd.Parameters.AddWithValue("@surname", loginSurname.Text);
            cmd.Parameters.AddWithValue("@patronymic", LoginPatronymic.Text);
            cmd.Parameters.AddWithValue("@password", loginPass.Password);
            dataAdapter = new SqlDataAdapter(cmd);           
            DataTable dtb = new DataTable();
            if (dataAdapter.Fill(dtb) == 1)
            {
                MainWindow main = new MainWindow();
                main.ImportData(dtb.Rows[0][5].ToString(),dtb.Rows[0][3].ToString(), dtb.Rows[0][0].ToString(), dtb.Rows[0][1].ToString(), dtb.Rows[0][2].ToString());
                main.Show();
                connection.Close();
                Close();    
            }
            else
            {
                MessageBox.Show("Данные введены неверно!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            connection.Close();

        }

        private void LoginName_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                System.Diagnostics.Process.Start("cafeteriamanager.chm");
            }
        }
    }
}
