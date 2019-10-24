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
using System.Data.Sql;
using System.Data.SqlClient;
using System.IO;

namespace cafeteriaManager
{
    /// <summary>
    /// Логика взаимодействия для cashier.xaml
    /// </summary>
    public partial class cashier : UserControl
    {
        DataTable order = new DataTable();
        DataTable product = new DataTable();
        SqlConnection con;
        SqlDataAdapter sda;
        SqlCommandBuilder scb;
        float sum;
        public cashier()
        {
            InitializeComponent();
            con = new SqlConnection(@"Data Source=GLINVAN;Initial Catalog=cafeteria;Integrated Security=True");
            sum = 0;
        }

        void fillDataGrid()
        {
            
            SqlCommand cmd = new SqlCommand("SELECT id as 'Код товара',name as 'Название',quantity as 'Количество',price as 'Цена, руб' FROM [product]", con);
            
            sda = new SqlDataAdapter(cmd);
            scb = new SqlCommandBuilder(sda);

            order.Columns.Add("Код товара", typeof(int));
            order.Columns.Add("Название", typeof(string));
            order.Columns.Add("Количество", typeof(int));
            order.Columns.Add("Цена, руб", typeof(float));

            product.Columns.Add("Код товара", typeof(int));
            product.Columns.Add("Название", typeof(string));
            product.Columns.Add("Количество", typeof(int));
            product.Columns.Add("Цена, руб", typeof(float));

            sda.Fill(product);
            productlist.ItemsSource = product.DefaultView;
            checklist.ItemsSource = order.DefaultView;
            productlist.Columns[1].Width = 200;            
            
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            fillDataGrid();
        }

        private void Addtoorder_Click(object sender, RoutedEventArgs e)
        {
            try
            {                
                DataRowView selrowview = (DataRowView)productlist.SelectedItem;
                DataRow selrow = selrowview.Row;
                if ((int)selrow[2] > 0)
                {
                    if (checklist.Items.Count > 0)
                    {
                        foreach (DataRow r in order.Rows)
                        {
                            System.Diagnostics.Debug.Write(r[1]);
                            if (r[1].ToString() == selrow[1].ToString())
                            {
                                r[2] = int.Parse(r[2].ToString()) + 1;
                                r[3] = (float)(r[3]) + (float)(selrow[3]);
                                sum += (float)selrow[3];
                                sumLabel.Content = ("Сумма: " + sum + " руб");
                                if (int.Parse(selrow[2].ToString()) > 1)
                                {
                                    selrow[2] = int.Parse(selrow[2].ToString()) - 1;
                                }
                                else
                                {
                                    selrow[2] = 0;
                                    product.AcceptChanges();
                                }
                                return;
                            }
                        }
                        selrow[2] = int.Parse(selrow[2].ToString()) - 1;
                        order.ImportRow(selrow);
                        order.Rows[order.Rows.Count - 1][2] = 1;
                        sum += (float)order.Rows[order.Rows.Count - 1][3];
                        sum = (float)Math.Round(sum, 2);
                        sumLabel.Content = ("Сумма: " + sum + " руб");

                    }
                }
                else
                {
                    MessageBox.Show("Данный продукт закончился");
                }
            }
            catch
            {
                MessageBox.Show("Не выделен элемент!");
            }
            

            
        }

        private void deleterow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView selrowview = (DataRowView)checklist.SelectedItem;
                DataRow selrow = selrowview.Row;
                foreach (DataRow r in product.Rows)
                {
                    if (r[1].ToString() == selrow[1].ToString())
                    {
                        r[2] = int.Parse(r[2].ToString()) + 1;
                        sum -= (float)selrow[3] / (int)selrow[2];
                        sumLabel.Content = ("Сумма: " + sum + " руб");
                        if (int.Parse(selrow[2].ToString()) > 1)
                        {
                            selrow[2] = int.Parse(selrow[2].ToString()) - 1;
                            selrow[3] = float.Parse(selrow[3].ToString()) - float.Parse(r[3].ToString());
                            
                        }
                        else
                        {
                            selrow.Delete();
                            order.AcceptChanges();
                        }
                        return;
                    }
                }
                selrow[2] = int.Parse(selrow[2].ToString()) - 1;
                product.ImportRow(selrow);
                product.Rows[product.Rows.Count - 1][2] = 1;
                sum -= (float)product.Rows[product.Rows.Count - 1][3];
                sum = (float)Math.Round(sum, 2);
                sumLabel.Content = ("Сумма: " + sum + " руб");
            }
            catch
            {
                MessageBox.Show("Не выделен элемент!");
            }

        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            new auth().Show();
            Window.GetWindow(this).Close();            
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            if (order.Rows.Count > 0)
            {
                SqlCommand checkcmd = new SqlCommand("INSERT INTO [order] (user_id, overall_sum, date) VALUES (@user_id, @overall_sum, @date) SELECT SCOPE_IDENTITY()", con);
                checkcmd.Parameters.AddWithValue("@user_id", ((MainWindow)Window.GetWindow(this)).UId);
                checkcmd.Parameters.AddWithValue("@overall_sum", Convert.ToDecimal(sum));
                checkcmd.Parameters.AddWithValue("@date", DateTime.Now);
                FileStream fs = new FileStream("check.txt", FileMode.Create);
                StreamWriter sr = new StreamWriter(fs);
                MainWindow mw = (MainWindow)Window.GetWindow(this);

                con.Open();
                var insId = Convert.ToInt32(checkcmd.ExecuteScalar());
                con.Close();
                checkcmd.Parameters.Clear();
                sr.WriteLine("Чек");
                sr.WriteLine("Имя кассира: " + mw.UName + " " + mw.USurname + " " + mw.UPatronymic);
                sr.WriteLine("Выдан: " + DateTime.Now);
                foreach (DataRow r in order.Rows)
                {
                    sr.WriteLine(r[1].ToString().TrimEnd(' ') + " -- x" + r[2] + " -- " + (float)(r[3]) / (int)(r[2]) + " руб");
                    checkcmd = new SqlCommand("INSERT INTO [order_element] (order_id, product_id, quantity) VALUES (@order_id, @product_id, @quantity)", con);
                    checkcmd.Parameters.AddWithValue("@order_id", insId);
                    checkcmd.Parameters.AddWithValue("@product_id", r[0]);
                    checkcmd.Parameters.AddWithValue("@quantity", r[2]);
                    con.Open();
                    checkcmd.ExecuteNonQuery();
                    con.Close();
                }
                sr.WriteLine("Сумма: " + sum + " руб");
                sr.Close();
                System.Diagnostics.Process.Start("check.txt");

                sda.UpdateCommand = scb.GetUpdateCommand();
                sda.Update(product);
                order.Rows.Clear();
                sum = 0;
                sumLabel.Content = ("Сумма: 0 руб");
            }
            else
            {
                MessageBox.Show("В заказе ничего нет!");

            }
            
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (product.Rows.Count > 0)
            {
                DataView dv = new DataView(product);
                dv.RowFilter = "name like '" + searchBar.Text + "%'";
                productlist.ItemsSource = dv;
            }
            else return;
        }
         
    }
}
