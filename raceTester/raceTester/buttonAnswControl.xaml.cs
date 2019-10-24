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

namespace raceTester
{
    /// <summary>
    /// Логика взаимодействия для UserControl3.xaml
    /// </summary>
    public partial class UserControl3 : UserControl
    {
        public int Qnum { get; set; }
        public int Type { get; set; }
        public string TrueA { get; set; }
        public bool isTrue { get; set; }
        public UserControl3(Question q, int num)
        {
            InitializeComponent();
            Qnum = num;
            qNum.Content = ("Вопрос №" + (Qnum+1));
            Type = q.type;
            qText.Text = (q.text);
            if (Type == 1)
            {
                answ1.Content = q.ansarr[0].text;
                answ2.Content = q.ansarr[1].text;
                answ3.Content = q.ansarr[2].text;
                answ4.Content = q.ansarr[3].text;
                Image1.Visibility = Visibility.Collapsed;
                Image2.Visibility = Visibility.Collapsed;
                Image3.Visibility = Visibility.Collapsed;
                Image4.Visibility = Visibility.Collapsed;
                foreach (Answer a in q.ansarr)
                {
                    if (a.isTrue)
                    {
                        TrueA = a.text;
                        break;
                    }
                }
            }
            else if (Type == 3)
            {
                Image1.Source = new BitmapImage(new Uri(q.ansarr[0].text, UriKind.Relative));
                Image2.Source = new BitmapImage(new Uri(q.ansarr[1].text, UriKind.Relative));
                Image3.Source = new BitmapImage(new Uri(q.ansarr[2].text, UriKind.Relative));
                Image4.Source = new BitmapImage(new Uri(q.ansarr[3].text, UriKind.Relative));
                for (int i = 0; i < q.ansarr.Capacity-1; i++)
                {
                    if (q.ansarr[i].isTrue)
                    {
                        TrueA = (i + 1).ToString();
                    }
                }
            }
            
            isTrue = false;
        }      

        private void RBChecked(object sender, RoutedEventArgs e)
        {
            if (Type == 1)
            {
                if (((RadioButton)sender).Content.ToString() == TrueA)
                {
                    isTrue = true;
                }
                else
                {
                    isTrue = false;
                }
            }
            else if (Type == 3)
            {
                System.Diagnostics.Debug.Write(((RadioButton)sender).Name.Substring(((RadioButton)sender).Name.Length - 1, 1) + " " + TrueA);
                if (((RadioButton)sender).Name.Substring(((RadioButton)sender).Name.Length-1,1) == TrueA)
                {
                    isTrue = true;
                }
                else
                {
                    isTrue = false;
                }
            }
        }
    }
}
