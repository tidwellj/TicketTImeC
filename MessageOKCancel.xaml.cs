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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TicketTime
{
    /// <summary>
    /// Interaction logic for MessageOKCancel.xaml
    /// </summary>
    /// 



    public partial class MessageOKCancel : Window
    {

        public string toggled;  //{ get; set; } // Use a property for encapsulat

        private MainWindow _mainWindow;



        public MessageOKCancel(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;




        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            //MainWindow mainWindow = new MainWindow();


            //mainWindow.toggled = "1";
           // _mainWindow = mainWindow;
            _mainWindow.UpdateVariable("1");

            CloseWin();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
           // MainWindow mainWindow = new MainWindow();
           // _mainWindow = mainWindow;

            _mainWindow.UpdateVariable("2");



            CloseWin();
        }

        private void CloseWin()
        {
            Hide();


        }
    }
}
