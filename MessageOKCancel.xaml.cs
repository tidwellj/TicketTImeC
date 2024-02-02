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

        public string Toggle { get; private set; } // Use a property for encapsulat
        public event EventHandler<string> ClosingWithResult;


        public MessageOKCancel(string data)
        {
            InitializeComponent();
            



        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
           
            
           Toggle = "1";
            ClosingWithResult?.Invoke(this, Toggle); // Notify subscribers with the toggle value

            CloseWin();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            
            Toggle="2";
            ClosingWithResult?.Invoke(this, Toggle); // Notify subscribers with the toggle value


            CloseWin();
        }

        private void CloseWin()
        {
           Hide();
        
       
        }
    }   
}
