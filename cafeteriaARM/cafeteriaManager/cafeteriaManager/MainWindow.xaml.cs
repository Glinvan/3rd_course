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

namespace cafeteriaManager
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int UId { get; set; }
        public string UaccountType { get; set; }
        public string UName { get; set; }
        public string USurname { get; set; }
        public string UPatronymic { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            //UserControl1 uc1 = new UserControl1();
            //Content = uc1;
            
        }

        public void ImportData(string id, string accountType, string name, string surname, string patronymic)
        {
            char[] trimmer = { ' ' };
            UId = int.Parse(id);
            UaccountType = accountType.Trim(trimmer);
            UName = name.Trim(trimmer);
            USurname = surname.Trim(trimmer);
            UPatronymic = patronymic.Trim(trimmer);
            if (UaccountType == "admin")
            {
                UserControl1 uc1 = new UserControl1();
                Content = uc1;
            }
            else if (UaccountType == "cashier")
            {
                cashier uc = new cashier();
                Content = uc;
            }
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
