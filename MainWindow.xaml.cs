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
using System.Windows.Forms;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using Application = System.Windows.Application;

namespace TicketTime
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly NotifyIcon notifyIcon;

        public MainWindow()
        {
            InitializeComponent();

            notifyIcon = new NotifyIcon();
            

            notifyIcon.Icon = new System.Drawing.Icon("ticket.ico");
            notifyIcon.Visible = true;
            notifyIcon.Text = "Your tooltip text here";
           // notifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;
            notifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick1;




            var contextMenu = new ContextMenuStrip();
            var quitMenuItem = new ToolStripMenuItem("Quit");
            quitMenuItem.Click += QuitMenuItem_Click; // Event handler for quitting
            contextMenu.Items.Add(quitMenuItem);

            notifyIcon.ContextMenuStrip = contextMenu;








        }

        private void NotifyIcon_MouseDoubleClick1(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            // Cancel the closing of the application
            e.Cancel = true;

            // Minimize the window and hide it from the taskbar
            this.WindowState = WindowState.Minimized;
            this.ShowInTaskbar = false;
        }


        private void QuitMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false; // Hide the notify icon
            Application.Current.Shutdown(); // Shutdown the application
        }





    }
}
