using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Timers;
//using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using Application = System.Windows.Application;
using Timer = System.Timers.Timer;
using System.Data.SQLite;
using System.Data;
using System.Data.Common;
using MessageBox = System.Windows.MessageBox;
using System.Linq.Expressions;
using System.Windows.Media.Animation;
using System.Xml.Linq;
using System.Windows.Controls;

namespace TicketTime
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly NotifyIcon notifyIcon;
        private Timer timer;
        private TimeSpan time;
        private bool isPaused;
        public string timerString;


        public MainWindow()



        {
            InitializeComponent();

            timerString = "";
            LoadDB();




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

        //Timer code ------------------------->

        public void ChangePause()
        {
            isPaused = !isPaused;
        }

        private void onTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (!isPaused)
            {
                time = time.Add(TimeSpan.FromSeconds(1));
                UpdateTimeDisplay();
            }
        }





        private void UpdateTimeDisplay()
        {
            Dispatcher.Invoke(() =>
            {
                timerString = time.ToString(@"hh\:mm\:ss");
                TimeLabel.Content = timerString;

            });
        }





        //Notificat tray code ---------------->




        private void NotifyIcon_MouseDoubleClick1(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            StateChanged += MainWindow_StateChanged;
            this.Show();
            this.WindowState = WindowState.Normal;

        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                Storyboard maximizeAnimation = (Storyboard)FindResource("maximizeAnimation");
                BeginStoryboard(maximizeAnimation);
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            // Cancel the closing of the application
            e.Cancel = true;

            // Minimize the window and hide it from the taskbar
            this.Hide();
            this.WindowState = WindowState.Minimized;
            this.ShowInTaskbar = false;
        }


        private void QuitMenuItem_Click(object sender, EventArgs e)
        {

            notifyIcon.Visible = false; // Hide the notify icon
            Application.Current.Shutdown(); // Shutdown the application
            notifyIcon.Dispose();

        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Console.WriteLine("Text Changed");
            
           
        }
        



        private void Clear_click(object sender, RoutedEventArgs e)
        {

            timerString = "00:00:00";
            TimeLabel.Content = timerString;
            TicketBox.Text = "";
            NameBox.Text = "";
            TypeCombo.SelectedIndex = 0;

        }


        private void Stop_Click(object sender, RoutedEventArgs e)
        {

            if (timer != null)
            {
                timer.Stop();
                // timerString = "";
                _ = MessageBox.Show("Do you want to save Ticket to Database?", "Save?", MessageBoxButton.OKCancel);
                string connectionString = "Data Source=C:\\Users\\jtidw\\OneDrive\\Documents\\GitHub\\TicketTImeC\\ticket_database.db;";
                string type = TypeCombo.Text;
                string ticket = TicketBox.Text;
                string name = NameBox.Text;
                DateTime now = DateTime.Now;
                string date = now.ToString("yyyy-MM-dd");
                string time = TimeLabel.Content.ToString();

                try
                {
                    using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                    {
                        conn.Open();
                        string sql = "INSERT INTO tickets (Type, Ticket, Name, Date, Time) VALUES (@type, @ticket, @name, @date, @time)";
                        using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                        {
                            command.Parameters.AddWithValue("@type", type);
                            command.Parameters.AddWithValue("@ticket", ticket);
                            command.Parameters.AddWithValue("@name", name);
                            command.Parameters.AddWithValue("@date", date);
                            command.Parameters.AddWithValue("@time", time);
                            command.ExecuteNonQuery();


                        }

                        conn.Close();
                    }




                }

                catch (Exception ex)
                {
                    MessageBox.Show("An error occured!" + ex.Message);
                }




            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save Ticket to Database?", "Save?", MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                    MessageBox.Show("Nothing to Save", "Error!", MessageBoxButton.OKCancel);

            }

            LoadDB();


        }





        private void Pause_Click(object sender, RoutedEventArgs e)
        {



            if (timer == null)
            {
                MessageBox.Show("Timer not running", "Error!", MessageBoxButton.OK);

            }
            else
            {


                ChangePause();



                if (Start.Content.ToString() == "Start")
                {
                    Start.Content = "Resume";
                }
            }
        }


        private void Set_Click(object sender, RoutedEventArgs e)
        {
            string type = TypeCombo.Text;
            string ticket = TicketBox.Text;
            string name = NameBox.Text;

            TrackingLabel.Content = "Tracking Ticket";

           

        }


            private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (isPaused == false)
            {
                isPaused = false;
                time = TimeSpan.Zero;
                timer = new Timer(1000);
                timer.Elapsed += onTimerElapsed;
                timer.Start();
            }

            else
            {
                if (Start.Content.ToString() == "Resume")
                {
                    Start.Content = "Start";
                }
                ChangePause();



            }





        }









        private void CloseButton_click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }




        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ChangePause();



        }

        private void TabSearch_Click(object sender, RoutedEventArgs e)
        {
            TabMaster.SelectedIndex = 0;
        }
        private void TabTicket_Click(object sender, RoutedEventArgs e)
        {
            TabMaster.SelectedIndex = 2;
        }


        private void TabSettings_Click(object sender, RoutedEventArgs e)
        {
            TabMaster.SelectedIndex = 1;
        }




        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void LoadDB()
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=C:\\Users\\jtidw\\OneDrive\\Documents\\GitHub\\TicketTImeC\\ticket_database.db;");
            System.Console.WriteLine(conn);
            conn.Open();
            string query = "Select * from tickets";
            SQLiteCommand cmd = new SQLiteCommand(query, conn);
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(cmd);
            DataTable dt = new DataTable();
            dataAdapter.Fill(dt);

            TicketGrid.ItemsSource = dt.DefaultView;
            // TicketGrid.Columns[1].Visibility = Visibility.Collapsed;

            conn.Close();


        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }

        }


        private void SearchBetween_Click(object sender, RoutedEventArgs e)
        {

            
                string connectionString = "Data Source=C:\\Users\\jtidw\\OneDrive\\Documents\\GitHub\\TicketTImeC\\ticket_database.db;";

            var startDate = From.SelectedDate;
                var endDate = To.SelectedDate;

                if (startDate == null || endDate == null)
                {

                    MessageBox.Show("Please select values for both dates.");
                }
                else if (startDate.Value.Date > endDate.Value.Date)
                {
                    MessageBox.Show("Please select a starting date bigger than the end date.");

                }
                else if (startDate.Value.Date == endDate.Value.Date)
                    {
                        try
                        {
                            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                            {
                                conn.Open();
                                string sql = "SELECT * FROM tickets WHERE DATE = @startDate";
                                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                                {
                                    command.Parameters.AddWithValue("@startDate", startDate.Value.ToString("yyy-MM-dd"));
                                    command.Parameters.AddWithValue("@endDate", startDate.Value.ToString("yyy-MM-dd"));
                                    command.ExecuteNonQuery();
                                    SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(command);
                                    DataTable dt = new DataTable();
                                    dataAdapter.Fill(dt);

                                    TicketGrid.ItemsSource = dt.DefaultView;


                                     conn.Close();
                                    



                        
                                }
                                
                            }




                        }


                        catch (Exception ex)
                        {
                        MessageBox.Show("An error occured!" + ex.Message);
                        }

            }
                else
                {

                    try
                    {
                        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                        {
                            conn.Open();
                            string sql = "SELECT * FROM TICKETS where DATE BETWEEN @startDate and @endDate";
                            using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                            {
                                command.Parameters.AddWithValue("@startDate", startDate.Value.ToString("yyy-MM-dd"));
                                command.Parameters.AddWithValue("@endDate", endDate.Value.ToString("yyy-MM-dd"));
                                command.ExecuteNonQuery ();
                                
                                SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(command);
                                DataTable dt = new DataTable();
                                dataAdapter.Fill(dt);

                                TicketGrid.ItemsSource = dt.DefaultView;


                                conn.Close();



                        }
                            

                        }




                    }


                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occured!" + ex.Message);

                    }













            }




















        }

        private void SearchName_Click(object sender, RoutedEventArgs e)
        {


            string connectionString = "Data Source=C:\\Users\\jtidw\\OneDrive\\Documents\\GitHub\\TicketTImeC\\ticket_database.db;";

            string nameValue = NameSearchBox.Text;
            //var endDate = To.SelectedDate;

            if (string.IsNullOrEmpty(nameValue))
            {

                MessageBox.Show("Please enter a ticket name.");
            }


            else
            {

                try
                {
                    using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                    {
                        conn.Open();
                        string sql = "SELECT * FROM TICKETS where name like @nameValue";
                        using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                        {
                            command.Parameters.AddWithValue("@nameValue", nameValue);
                            command.Parameters.AddWithValue("searchPattern", "%" + nameValue + "%");
                            command.ExecuteNonQuery();

                            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(command);
                            DataTable dt = new DataTable();
                            dataAdapter.Fill(dt);

                            TicketGrid.ItemsSource = dt.DefaultView;


                            conn.Close();



                        }


                    }




                }


                catch (Exception ex)
                {
                    MessageBox.Show("An error occured!" + ex.Message);

                }













            }





        }



        private void SearchType_Click(object sender, RoutedEventArgs e)
        {


            string connectionString = "Data Source=C:\\Users\\jtidw\\OneDrive\\Documents\\GitHub\\TicketTImeC\\ticket_database.db;";

            string typeValue = TypeSearchBox.Text;
            //var endDate = To.SelectedDate;

            if (string.IsNullOrEmpty(typeValue))
            {

                MessageBox.Show("Please enter a ticket type.");
            }


            else
            {

                try
                {
                    using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                    {
                        conn.Open();
                        string sql = "SELECT * FROM TICKETS where type like @typeValue";
                        using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                        {
                            command.Parameters.AddWithValue("@typeValue", typeValue);
                            //command.Parameters.AddWithValue("searchPattern", "%" + typeValue + "%");
                            command.ExecuteNonQuery();

                            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(command);
                            DataTable dt = new DataTable();
                            dataAdapter.Fill(dt);

                            TicketGrid.ItemsSource = dt.DefaultView;


                            conn.Close();



                        }


                    }




                }


                catch (Exception ex)
                {
                    MessageBox.Show("An error occured!" + ex.Message);

                }













            }





        }

        private void SearchTicket_Click(object sender, RoutedEventArgs e)
        {


            string connectionString = "Data Source=C:\\Users\\jtidw\\OneDrive\\Documents\\GitHub\\TicketTImeC\\ticket_database.db;";

            string ticketValue = TicketSearchBox.Text;
            //var endDate = To.SelectedDate;

            if (string.IsNullOrEmpty(ticketValue))
            {

                MessageBox.Show("Please enter a icket number.");
            }


            else
            {

                try
                {
                    using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                    {
                        conn.Open();
                        string sql = "SELECT * FROM TICKETS where ticket like @ticketValue";
                        using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                        {
                            command.Parameters.AddWithValue("@ticketValue", ticketValue);
                           // command.Parameters.AddWithValue("searchPattern", "%" + typeValue + "%");
                            command.ExecuteNonQuery();

                            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(command);
                            DataTable dt = new DataTable();
                            dataAdapter.Fill(dt);

                            TicketGrid.ItemsSource = dt.DefaultView;


                            conn.Close();



                        }


                    }




                }


                catch (Exception ex)
                {
                    MessageBox.Show("An error occured!" + ex.Message);

                }













            }





        }

        private void SearchClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TicketSearchBox.Text = "";
                NameSearchBox.Text = "";
                TypeSearchBox.Text = "";
                From.SelectedDate = null;
                To.SelectedDate = null;

                LoadDB();

            }


            catch (Exception ex)
            {
                MessageBox.Show("An error occured!" + ex.Message);

            }

        }

    }
}
       