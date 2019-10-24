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
using System.Windows.Shapes;
using System.Threading;

namespace raceTester
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        bool[] answers;
        public List<UserControl> questions;
        int Qnum;
        int timerMin;
        int timerSec;
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();    
        public TestWindow(int n)
        {
            InitializeComponent();

            answers = new bool[n];
            questions = new List<UserControl>();
            for(int j = 0; j < n; j++)
            {
                answers[j] = false;
            }
            Qnum = 0;
            timerMin = 5;
            timerSec = 0;
            timerLabel.Content = string.Format("{0:00}:{1:00}", timerMin, timerSec);
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Start();
        }

        public void Start()
        {
            testGrid.Children.Add(questions[0]);
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (timerMin == 0 && timerSec <= 1)
            {
                MessageBox.Show("Время вышло!");
                Content = new resultControl(answers, questions);
                ((System.Windows.Threading.DispatcherTimer)sender).Stop();
            }
            timerSec--;
            if (timerSec <= 0)
            {
                timerMin--;
                timerSec = 59;
            }
            timerLabel.Content = string.Format("{0:00}:{1:00}", timerMin, timerSec);
            

        }

        private void TestClosed(object sender, EventArgs e)
        {
            Application.Current.MainWindow.Visibility = Visibility.Visible;
        }

        private void nextAnsw_Click(object sender, RoutedEventArgs e)
        {
            if (testGrid.Children[0] is UserControl3)
            {
                if (((UserControl3)testGrid.Children[0]).isTrue)
                {
                    answers[Qnum] = true;
                }
                else
                {
                    answers[Qnum] = false;
                }
            }
            else if (testGrid.Children[0] is UserControl2)
            {
                if (((UserControl2)testGrid.Children[0]).answTB.Text.Trim(' ') == ((UserControl2)testGrid.Children[0]).True.Trim(' '))
                    {
                        answers[Qnum] = true;
                    }
                else
                {
                    answers[Qnum] = false;
                }
            }
                     
            if (Qnum == answers.Length-1)
            {
                Content = new resultControl(answers, questions);
            }
            else
            {
                testGrid.Children.Clear();
                testGrid.Children.Add(questions[++Qnum]);
            }
            foreach (bool a in answers)
            {
                System.Diagnostics.Debug.WriteLine(a);
            }

            if (Qnum == 1)
            {
                prevAnsw.IsEnabled = true;
            }
            
        }

        private void prevAnsw_Click(object sender, RoutedEventArgs e)
        {
            if (Qnum == 0)
            {
                Close();
                return;
            }
            testGrid.Children.Clear();
            testGrid.Children.Add(questions[--Qnum]);
            if (Qnum == 0)
            {
                ((Button)sender).IsEnabled = false;
            }
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
