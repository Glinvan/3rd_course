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
using System.Data;
using System.Data.SqlClient;


namespace cafeteriaManager
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        DataTable regdt = new DataTable();
        DataTable product = new DataTable();
        SqlConnection con = new SqlConnection(@"Data Source=GLINVAN;Initial Catalog=cafeteria;Integrated Security=True");
        SqlCommand cmd;
        SqlCommand productCMD;
        SqlDataAdapter sda;
        SqlDataAdapter productDA;
        SqlCommandBuilder scb;
        SqlCommandBuilder productCB;

        public UserControl1()
        {
            InitializeComponent();
            cmd = new SqlCommand("SELECT id, name as 'Имя', surname as 'Фамилия', patronymic as 'Отчество', type as 'Тип', password as 'Пароль' FROM [user] WHERE type != 'admin'", con);
            sda = new SqlDataAdapter(cmd);
            scb = new SqlCommandBuilder(sda);
            regdt.Columns.Add("Имя", typeof(string));
            regdt.Columns.Add("Фамилия", typeof(string));
            regdt.Columns.Add("Отчество", typeof(string));
            regdt.Columns.Add("Тип", typeof(string));
            regdt.Columns.Add("Пароль", typeof(string));
            sda.Fill(regdt);
            users.ItemsSource = regdt.DefaultView;

            productCMD = new SqlCommand("SELECT name as 'Название',quantity as 'Количество',price as 'Цена, руб', id as 'Код товара' FROM [product]", con);
            productDA = new SqlDataAdapter(productCMD);
            productCB = new SqlCommandBuilder(productDA);
            product.Columns.Add("Название", typeof(string));
            product.Columns.Add("Количество", typeof(int));
            product.Columns.Add("Цена, руб", typeof(float));
            product.Columns.Add("Код товара", typeof(int));
            productDA.Fill(product);
            productlist.ItemsSource = product.DefaultView;
            //productlist.Columns[1].Width = 200;

        }

        private void AddCashier_Click(object sender, RoutedEventArgs e)
        {
            
            if (regName.Text == "" || regSurname.Text == "" || regPatronymic.Text == "" || regPassword.Text == "")
            {
                MessageBox.Show("Введите данные!");
                return;
            }
            foreach (DataRow r in regdt.Rows)
            {
                if (r[0].ToString().Trim(' ') == regName.Text && r[1].ToString().Trim(' ') == regSurname.Text && r[2].ToString().Trim(' ') == regPatronymic.Text && r[4].ToString().Trim(' ') == regPassword.Text)
                {
                    MessageBox.Show("Такой аккаунт уже существует!");
                    return;
                }                
            }
            regdt.Rows.Add(regName.Text, regSurname.Text, regPatronymic.Text, "cashier", regPassword.Text);
            dbsave(sda, scb, regdt, "SELECT id, name as 'Имя', surname as 'Фамилия', patronymic as 'Отчество', type as 'Тип', password as 'Пароль' FROM [user] WHERE type != 'admin'", cmd);
        }

        private void DeleteCashier_Click(object sender, RoutedEventArgs e)
        {
            DataRowView selrowview = (DataRowView)users.SelectedItem;
            DataRow selrow = selrowview.Row;            
            selrow.Delete();
            dbsave(sda, scb, regdt, "SELECT id, name as 'Имя', surname as 'Фамилия', patronymic as 'Отчество', type as 'Тип', password as 'Пароль' FROM [user] WHERE type != 'admin'", cmd);
        }

        private void dbsave(SqlDataAdapter sda, SqlCommandBuilder scb, DataTable dt, string cmdstring, SqlCommand cmd)
        {
            sda.InsertCommand = scb.GetInsertCommand();
            sda.DeleteCommand = scb.GetDeleteCommand();
            sda.UpdateCommand = scb.GetUpdateCommand();
            sda.Update(dt);
            dt.Rows.Clear();
            cmd = new SqlCommand(cmdstring, con);
            sda.Fill(dt);
        }

        private void logout_Click(object sender, RoutedEventArgs e)
        {
            new auth().Show();
            Window.GetWindow(this).Close();
        }

        private void AddProd_Click(object sender, RoutedEventArgs e)
        {
            if (pName.Text == "" || pQuant.Text == "" || pPrice.Text == "")
            {
                MessageBox.Show("Введите данные!");
                return;
            }

            try
            {
                foreach (DataRow r in product.Rows)
                {
                    if (r[0].ToString().ToLower().TrimEnd(' ') == pName.Text.ToLower().TrimEnd(' '))
                    {

                        r[1] = int.Parse(r[1].ToString()) + int.Parse(pQuant.Text);
                        r[2] = float.Parse(pPrice.Text);
                        dbsave(productDA, productCB, product, "SELECT name as 'Название',quantity as 'Количество',price as 'Цена', id FROM [product]", productCMD);
                        return;
                    }
                }

                product.Rows.Add(pName.Text, int.Parse(pQuant.Text), float.Parse(pPrice.Text));
                dbsave(productDA, productCB, product, "SELECT name as 'Название',quantity as 'Количество',price as 'Цена', id FROM [product]", productCMD);
            }
            catch
            {
                MessageBox.Show("Данные введены неверно!");
            }

        }

        private void DelProd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView selrowview = (DataRowView)productlist.SelectedItem;
                DataRow selrow = selrowview.Row;
                int sum = int.Parse(selrow[1].ToString()) - int.Parse(delQuant.Text);
                if (sum <= 0)
                {
                    selrow[1] = 0;
                }
                else
                {
                    selrow[1] = sum;
                }
                dbsave(productDA, productCB, product, "SELECT name as 'Название',quantity as 'Количество',price as 'Цена', id FROM [product]", productCMD);
            }
            catch
            {
                MessageBox.Show("Выберите продукт и введите количество");
            }
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (product.Rows.Count > 0)
            {
                DataView dv = new DataView(product);
                dv.RowFilter = "Название like '" + searchBar.Text + "%'";
                productlist.ItemsSource = dv;
            }
            else return;
        }

        private void Report_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (date1.SelectedDate.Value.Date > date2.SelectedDate.Value.Date)
                {
                    MessageBox.Show("Выбран некорректный период");
                    return;
                }
                DataRowView selrowview = (DataRowView)users.SelectedItem;
                DataRow selrow = selrowview.Row;
                Microsoft.Office.Interop.Word.Application app = new Microsoft.Office.Interop.Word.Application();
                object missing = System.Reflection.Missing.Value;
                Microsoft.Office.Interop.Word.Document doc = app.Documents.Add(ref missing, ref missing, ref missing, ref missing);

                SqlCommand cmdrep = new SqlCommand("SELECT id, user_id, overall_sum, date FROM [order] WHERE user_id = @user_id AND date >= @date1 AND date < @date2", con);
                cmdrep.Parameters.AddWithValue("@user_id", (int)selrow[5]);
                cmdrep.Parameters.AddWithValue("@date1", date1.SelectedDate);
                cmdrep.Parameters.AddWithValue("@date2", date2.SelectedDate);
                SqlDataAdapter sdarep = new SqlDataAdapter(cmdrep);
                DataTable dtrep = new DataTable();
                DataTable dtr = new DataTable();
                int records = sdarep.Fill(dtrep);
                decimal overallsum = 0;
                if (records == 0)
                {
                    MessageBox.Show("У данного пользователя нет записей за этот период");
                    return;
                }
                doc.Content.Text += ("Отчет работы кассира " + ((string)selrow[0]).TrimEnd(' ') + " " + ((string)selrow[1]).TrimEnd(' ') + " " + ((string)selrow[2]).TrimEnd(' ') + "\nза период от " + date1.SelectedDate.Value.Date.ToShortDateString() + " до " + date2.SelectedDate.Value.Date.ToShortDateString());
                
                foreach (DataRow r in dtrep.Rows)
                {
                    doc.Content.Text += ("Заказ номер " + r[0] + "; Дата: " + r[3] + "; Общая сумма заказа: " + r[2].ToString().TrimEnd('0','.') + " руб;\nСостав:");
                    overallsum += (decimal)r[2];
                    cmdrep = new SqlCommand("SELECT order_id, product.name, order_element.quantity, product.price FROM [order_element] JOIN [product] on product.id = [order_element].product_id WHERE order_id = " + r[0], con);
                    sdarep = new SqlDataAdapter(cmdrep);
                    sdarep.Fill(dtr);
                    foreach (DataRow oer in dtr.Rows)
                    {
                        doc.Content.Text += (((string)oer[1]).TrimEnd(' ') + " -- x" + oer[2] + " -- " + oer[3].ToString().TrimEnd('0','.') + " руб;");
                    }
                }

                doc.Content.Text += ("ИТОГ\nКоличество выполненных заказов: " + records + ";\nПрибыль за период: "+overallsum.ToString().TrimEnd('0','.'));
                app.Visible = true;
            }
            catch
            {
                MessageBox.Show("Выберите пользователя");
            }
        }
    }
}
