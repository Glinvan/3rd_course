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
using System.Data.Sql;
using System.Data.SqlClient;

namespace raceTester
{
    /// <summary>
    /// Логика взаимодействия для resultControl.xaml
    /// </summary>
    public partial class resultControl : UserControl
    {
        double precents;
        int trueCount;
        int falseCount;
        List<string> listContent;
        SqlCommand cmd;
        SqlConnection connection;
        public resultControl(bool[] answers, List<UserControl> questions)
        {
            InitializeComponent();
            connection = new SqlConnection(@"Data Source=GLINVAN;Initial Catalog=race_test;Integrated Security=True");
            listContent = new List<string>();
            int i = 0;
            trueCount = 0;
            falseCount = 0;
            precents = 0;           
            foreach(bool b in answers)
            {
                if (b)
                {
                    trueCount++;
                }
                else
                {
                    falseCount++;
                }
            }
            listContent.Add("Количество правильных ответов: "+trueCount);
            listContent.Add("Количество неправильных ответов: " + falseCount);
            precents = trueCount;
            precents /= answers.Length;
            precents *= 100;
            precents = Math.Round(precents, 0);
            listContent.Add("Процент правильности: " + precents + "%");           
            cmd = new SqlCommand("INSERT INTO [result] (user_id, test_id, precent, date) VALUES (@user_id, @test_id, @precent, @date); SELECT SCOPE_IDENTITY()", connection);
            cmd.Parameters.AddWithValue("@user_id", Window1.Person_id);
            cmd.Parameters.AddWithValue("@test_id", Window1.Test_id);
            cmd.Parameters.AddWithValue("@precent", precents);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);
            connection.Open();
            int resid = Convert.ToInt32(cmd.ExecuteScalar());
            connection.Close();
            
            cmd = new SqlCommand("INSERT INTO [result_answers] (result_id, question_text, istrue) VALUES (@result_id, @question_text, @istrue)", connection);
            cmd.Parameters.AddWithValue("@result_id", resid);                       
            string restext = null;
            bool curranswer = true;
            cmd.Parameters.AddWithValue("@question_text", restext);
            cmd.Parameters.AddWithValue("@istrue", curranswer);
            connection.Open();
            foreach (UserControl u in questions)
            {
                if (u is UserControl3)
                {
                    listContent.Add(((UserControl3)u).qText.Text.ToString().TrimEnd(' ') + " - " + answers[i]);
                    restext = ((UserControl3)u).qText.Text.ToString();                    
                }
                else if (u is UserControl2)
                {
                    listContent.Add(((UserControl2)u).qText.Text.ToString().TrimEnd(' ') + " - " + answers[i]);
                    restext = ((UserControl2)u).qText.Text.ToString();
                }
                curranswer = answers[i];
                i++;
                cmd.Parameters["@istrue"].Value = curranswer;
                cmd.Parameters["@question_text"].Value = restext;
                cmd.ExecuteNonQuery();
            }
            connection.Close();
            foreach (string s in listContent)
            {
                resultList.Items.Add(s);
            }

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Office.Interop.Word.Application application = new Microsoft.Office.Interop.Word.Application();
            object missing = System.Reflection.Missing.Value;
            Microsoft.Office.Interop.Word.Document doc = application.Documents.Add(ref missing, ref missing, ref missing, ref missing);
            doc.Content.Text += (DateTime.Now);
            doc.Content.Text += (Window1.Person_Name + " " + Window1.Person_Patronymic + " " + Window1.Person_Surname);
            foreach (string s in resultList.Items)
            {
                doc.Content.Text += s;
            }

            application.Visible = true;
        }
    }
}
