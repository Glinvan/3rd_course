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
    /// Логика взаимодействия для UserControl2.xaml
    /// </summary>
    public partial class UserControl2 : UserControl
    {
        public int Qnum { get; set; }
        public int Type { get; set; }
        public string True { get; set; }
        public UserControl2(Question q, int num)
        {
            InitializeComponent();
            Qnum = num;
            qNum.Content = ("Вопрос №" + (Qnum + 1));
            Type = q.type;
            qText.Text = (q.text);
            True = q.ansarr[0].text;
        }     
    }
}
