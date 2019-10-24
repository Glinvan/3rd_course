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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using System.Media;

namespace raceTester
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        SqlConnection con; 
        SqlCommand cmd;
        DataTable dt;
        Test curtest;
        SoundPlayer player;

        public UserControl1()
        {
            InitializeComponent();
            var cstr = ConfigurationManager.ConnectionStrings["rtCS"].ConnectionString;
            con = new SqlConnection(cstr);
            greet.Content = string.Format("Добро пожаловать, {0}", Window1.Person_Name);
            player = new SoundPlayer(@"wavs\meow.wav");
            player.Load();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (menuTestStart.Visibility == Visibility.Hidden)
            {
                menuTestStart.Content = "Начать";
                menuCombo.Visibility = Visibility.Visible;
                menuText.Visibility = Visibility.Visible;
                textScroll.Visibility = Visibility.Visible;
                menuList.Visibility = Visibility.Hidden;
                menuTestStart.Visibility = Visibility.Visible;           
            }
            else if (menuTestStart.Content.ToString() == "Сохранить")
            {
                menuTestStart.Content = "Начать";
                menuText.Visibility = Visibility.Visible;
                textScroll.Visibility = Visibility.Visible;
                menuList.Visibility = Visibility.Hidden;
            }
            else if (menuTestStart.Visibility == Visibility.Visible)
            {
                menuCombo.Visibility = Visibility.Hidden;
                menuText.Visibility = Visibility.Hidden;
                textScroll.Visibility = Visibility.Hidden;
                menuList.Visibility = Visibility.Hidden;
                menuTestStart.Visibility = Visibility.Hidden;
            }
            menuCombo.Items.Clear();
            cmd = new SqlCommand(@"SELECT * FROM test", con);
            fillDT(cmd, ref dt);
            foreach (DataRow r in dt.Rows)
            {
                menuCombo.Items.Add(r[1].ToString());
            }

        }

        private void result_Click(object sender, RoutedEventArgs e)
        {
            if (menuTestStart.Visibility == Visibility.Hidden)
            {
                menuTestStart.Content = "Сохранить";
                menuCombo.Visibility = Visibility.Visible;
                menuText.Visibility = Visibility.Hidden;
                textScroll.Visibility = Visibility.Hidden;
                menuList.Visibility = Visibility.Visible;
                menuTestStart.Visibility = Visibility.Visible;
            }
            else if (menuTestStart.Visibility == Visibility.Visible && menuTestStart.Content.ToString() == "Начать")
            {
                menuTestStart.Content = "Сохранить";
                menuText.Visibility = Visibility.Hidden;
                textScroll.Visibility = Visibility.Hidden;
                menuList.Visibility = Visibility.Visible;
            }
            else if (menuTestStart.Visibility == Visibility.Visible)
            {
                menuCombo.Visibility = Visibility.Hidden;
                menuText.Visibility = Visibility.Hidden;
                textScroll.Visibility = Visibility.Hidden;
                menuList.Visibility = Visibility.Hidden;
                menuTestStart.Visibility = Visibility.Hidden;
            }
            menuCombo.Items.Clear();
            cmd = new SqlCommand(@"SELECT result.id, user_id, test.name, precent, date FROM result JOIN test ON result.test_id = test.id", con);
            fillDT(cmd, ref dt);
            foreach (DataRow r in dt.Rows)
            {
                if (int.Parse(r[1].ToString()) == Window1.Person_id)
                    menuCombo.Items.Add(r[2].ToString().TrimEnd(' ') + " - " + r[4].ToString());
            }

        }

        private void userChange_Click(object sender, RoutedEventArgs e)
        {
            new Window1().Show();
            Window.GetWindow(this).Close();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }


        private void MenuTestStart_Click(object sender, RoutedEventArgs e)
        {
            if ((string)menuTestStart.Content == "Начать")
            {
                if (menuCombo.SelectedItem != null)
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        if (menuCombo.SelectedItem.ToString() == r[1].ToString())
                        {
                            curtest = new Test(int.Parse(r[0].ToString()), r[1].ToString(), con);
                            foreach (Question q in curtest.qarr)
                            {
                                q.ansarr.Shuffle();
                            }
                            curtest.qarr.Shuffle();
                            
                            Window1.Test_id = int.Parse(r[0].ToString());
                            break;
                        }
                    }

                }
                else
                {
                    MessageBox.Show("Выберите тест!");
                }
                TestWindow a = new TestWindow(curtest.qarr.Count);
                a.Show();
                Window.GetWindow(this).Visibility = Visibility.Collapsed;
                int i = 0;
                foreach (Question q in curtest.qarr)
                {   
                    
                    if (q.type==1 || q.type==3)
                    {
                       a.questions.Add(new UserControl3(q, i));
                    }
                    if (q.type == 2)
                    {
                        a.questions.Add(new UserControl2(q, i));
                    }
                    i++;

                }
                a.Start();
                //a.Close();
                //Window.GetWindow(this).Visibility = Visibility.Visible;
            }
            else
            {
                if (menuCombo.SelectedItem != null)
                {
                    Microsoft.Office.Interop.Word.Application application = new Microsoft.Office.Interop.Word.Application();
                    object missing = System.Reflection.Missing.Value;
                    Microsoft.Office.Interop.Word.Document doc = application.Documents.Add(ref missing, ref missing, ref missing, ref missing);
                    doc.Content.Text += (menuCombo.SelectedItem.ToString());
                    doc.Content.Text += (Window1.Person_Name + " " + Window1.Person_Patronymic + " " + Window1.Person_Surname);
                    foreach (string s in menuList.Items)
                    {
                        doc.Content.Text += s;
                    }

                    application.Visible = true;
                }
            }
           


        }

        private void MenuCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            menuList.Items.Clear();
            menuText.Text = "";
            if (menuCombo.SelectedItem != null)
            {
                if ((string)menuTestStart.Content == "Сохранить")
                {
                    cmd = new SqlCommand(@"SELECT result.id, user_id, test.name, precent, date FROM result JOIN test ON result.test_id = test.id", con);
                    fillDT(cmd, ref dt);
                    string selection = ((string)((ComboBox)sender).SelectedValue);
                    foreach (DataRow r in dt.Rows)
                    {
                        if (selection == (r[2].ToString().TrimEnd(' ') + " - " + r[4].ToString()))
                        {
                            SqlCommand tbcmd = new SqlCommand("SELECT result_id, question_text, istrue FROM result_answers WHERE result_id = @result_id", con);
                            tbcmd.Parameters.AddWithValue("@result_id", int.Parse(r[0].ToString()));
                            DataTable tbtable = null;
                            fillDT(tbcmd, ref tbtable);
                            foreach (DataRow dr in tbtable.Rows)
                            {
                                menuList.Items.Add(dr[1].ToString().TrimEnd(' ') + " - " + dr[2].ToString());
                            }
                            menuList.Items.Add("Процент правильности: " + r[3].ToString() + "%");
                            break;
                        }
                    }
                }
                else if ((string)menuTestStart.Content == "Начать")
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        if (((string)((ComboBox)sender).SelectedValue) == (string)r[1])
                        {

                            menuText.Text = r[2].ToString();
                        }
                    }
                }
            }

        }

        private void fillDT(SqlCommand sqlcmd, ref DataTable DT)
        {
            SqlDataAdapter tbadapter = new SqlDataAdapter(sqlcmd);
            DT = new DataTable();
            tbadapter.Fill(DT);
        }

        private void TestBlock()
        {

        }

        private void easteregg_MouseEnter(object sender, MouseEventArgs e)
        {
            easterImage.Visibility = Visibility.Visible;
            player.Play();
        }

        private void easteregg_MouseLeave(object sender, MouseEventArgs e)
        {
            easterImage.Visibility = Visibility.Collapsed;
        }
    }

    public class Answer
    {
        int id;
        public string text;
        public bool isTrue;

        public Answer(int id, string text, bool isTrue)
        {
            this.id = id;
            this.text = text;
            this.isTrue = isTrue;
        }
    }

    public class Question
    {
        int id;
        public string text;
        public int type;
        public List<Answer> ansarr;

        public Question(int id, int type, string text, SqlConnection con)
        {
            this.id = id;
            this.type = type;
            this.text = text;
            ansarr = new List<Answer>();
            SqlCommand cmd = new SqlCommand("SELECT * FROM answer WHERE question_id = @question_id", con);
            cmd.Parameters.AddWithValue("@question_id", id);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            foreach (DataRow r in dt.Rows)
            {
                ansarr.Add(new Answer(int.Parse(r[0].ToString()), r[2].ToString(), bool.Parse(r[3].ToString())));
            }
        }
    }

    public class Test
    {
        int id;        
        public string name;
        public List<Question> qarr;

        public Test(int id, string name, SqlConnection con)
        {
            this.id = id;
            this.name = name;
            qarr = new List<Question>();
            SqlCommand cmd = new SqlCommand("SELECT * FROM question WHERE test_id = @test_id", con);
            cmd.Parameters.AddWithValue("@test_id", id);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            foreach(DataRow r in dt.Rows)
            {
                qarr.Add(new Question(int.Parse(r[0].ToString()),int.Parse(r[2].ToString()),r[1].ToString(), con));
            }

        }
    }

    static public class MyExtensions
    {
        private static Random rng = new Random();
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
   
}
