using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml.Linq;
using Microsoft.Win32;

// using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using MessageBox = System.Windows.MessageBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using Timer = System.Timers.Timer;

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
        private string databasePath;
        private string conPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {

            this.InitializeComponent();

            this.timerString = string.Empty;
            this.LoadDB();

            this.notifyIcon = new NotifyIcon();

            this.notifyIcon.Icon = new System.Drawing.Icon("ticket.ico");
            this.notifyIcon.Visible = true;
            this.notifyIcon.Text = "Click to show";

            // notifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;
            this.notifyIcon.MouseDoubleClick += this.NotifyIcon_MouseDoubleClick1;

            var contextMenu = new ContextMenuStrip();
            var quitMenuItem = new ToolStripMenuItem("Quit");
            quitMenuItem.Click += this.QuitMenuItem_Click; // Event handler for quitting
            contextMenu.Items.Add(quitMenuItem);

            this.notifyIcon.ContextMenuStrip = contextMenu;
            //this.Start.IsEnabled = false;
            //this.Stop.IsEnabled = false;
            //this.Pause.IsEnabled = false;   
            //this.Clear.IsEnabled = false;


            //buttonToChange.Background = new SolidColorBrush(Colors.Green); // Change to your desired color

            //Start.Background = new SolidColorBrush(Colors.Red);


        }



        // Timer code ------------------------->
        public void ChangePause()
        {
            this.isPaused = !this.isPaused;
        }

        private void onTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (!this.isPaused)
            {
                this.time = this.time.Add(TimeSpan.FromSeconds(1));
                this.UpdateTimeDisplay();
            }
        }

        private void UpdateTimeDisplay()
        {
            this.Dispatcher.Invoke(() =>
            {
                this.timerString = this.time.ToString(@"hh\:mm\:ss");
                this.TimeLabel.Content = this.timerString;
            });
        }

        // Notificaton tray code ---------------->
        private void NotifyIcon_MouseDoubleClick1(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            this.StateChanged += this.MainWindow_StateChanged;
            this.Show();
            this.WindowState = WindowState.Normal;
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                Storyboard maximizeAnimation = (Storyboard)this.FindResource("maximizeAnimation");
                this.BeginStoryboard(maximizeAnimation);
            }
        }

        /// <inheritdoc/>
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

            this.notifyIcon.Visible = false; // Hide the notify icon
            Application.Current.Shutdown(); // Shutdown the application
            this.notifyIcon.Dispose();
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Console.WriteLine("Text Changed");
        }

        private void Clear_click(object sender, RoutedEventArgs e)
        {

            this.timerString = "00:00:00";
            this.TimeLabel.Content = this.timerString;
            this.TicketBox.Text = string.Empty;
            this.NameBox.Text = string.Empty;
            this.TypeCombo.SelectedIndex = 0;
            this.TrackingLabel.Content = string.Empty;
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            this.TrackingLabel.Content = string.Empty;
            LockBorder.Visibility = Visibility.Visible;
            LockBorder_2.Visibility = Visibility.Visible;
            LockBorder_3.Visibility = Visibility.Visible;

            this.timerString = "00:00:00";
            this.TimeLabel.Content = this.timerString;
            this.TicketBox.Text = string.Empty;
            this.NameBox.Text = string.Empty;
            this.TypeCombo.SelectedIndex = 0;
            this.TrackingLabel.Content = string.Empty;


            if (this.timer != null)
            {
                this.timer.Stop();



                MessageBoxResult result = MessageBox.Show("Do you want to save Ticket to Database?", "Save?", MessageBoxButton.OKCancel);

                // MessageBox.Show("Nothing to Save", "Error!", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {

                    if (this.TrackingLabel.Content != null)
                    {

                        // timerString = "";
                        // _ = MessageBox.Show("Do you want to save Ticket to Database?", "Save?", MessageBoxButton.OKCancel);
                        string connectionString = $"Data Source={this.databasePath};";
                        string type = this.TypeCombo.Text;

                        string ticket = this.TicketBox.Text;
                        string ticketValue = this.TicketSearchBox.Text;

                        // var endDate = To.SelectedDate;
                        string name = this.NameBox.Text;

                        DateTime now = DateTime.Now;
                        string date = now.ToString("yyyy-MM-dd");
                        string time = this.TimeLabel.Content.ToString();

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


                    else if (result == MessageBoxResult.Cancel)
                    {
                        // MessageBoxResult result = MessageBox.Show("Do you want to save Ticket to Database?", "Save?", MessageBoxButton.OKCancel);
                        this.timer.Stop();
                    }
                    else
                    {
                        MessageBox.Show("Timer not started");
                    }
                }
            }
            else
            {
                MessageBox.Show("Timer not started");
            }

            this.LoadDB();
            
        }
        
        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            var messageOK = new MessageOK();

            // MessageOKLoad();
            messageOK.Owner = this;
            messageOK.Left = this.Left + (this.ActualWidth - messageOK.Width) / 2;



            //messageOK.Left = this.Left;
            messageOK.Top = this.Top + (this.ActualHeight - messageOK.Height) / 2;
            // messageOK.Show();

            if (this.timer == null)
            {
                messageOK.MessageOKLabel.Text = "Timer is not running!";
                messageOK.Show();
               // MessageBox.Show("Timer not running", "Error!", MessageBoxButton.OK);
            }
            else
            {

                this.ChangePause();

                if (this.Start.Content.ToString() == "Start")
                {
                    this.Start.Content = "Resume";
                }
            }
        }

        private void Set_Click(object sender, RoutedEventArgs e)
        {

            string type = this.TypeCombo.Text;
            string ticket = this.TicketBox.Text;
            string name = this.NameBox.Text;
            //Create code for message window MessageOK.
            //var 
            var messageOK = new MessageOK();

           // MessageOKLoad();
            messageOK.Owner = this;
            messageOK.Left = this.Left + (this.ActualWidth - messageOK.Width) / 2;



            //messageOK.Left = this.Left;
            messageOK.Top = this.Top + (this.ActualHeight - messageOK.Height) / 2;
           // messageOK.Show();
            

            if (string.IsNullOrEmpty(ticket))
            {
                ticket = "None";
            }

            if (string.IsNullOrEmpty(name))
            {
                messageOK.MessageOKLabel.Text = "Please enter a name";
                messageOK.Show();
                //MessageBox.Show("Please enter a name");
            }
            else
            {
                this.TrackingLabel.Content = "Tracking Ticket";
                LockBorder.Visibility = Visibility.Collapsed;
                LockBorder_2.Visibility = Visibility.Collapsed;
                LockBorder_3.Visibility = Visibility.Collapsed;
               


            }
            //Activate timer buttons
            // this.Start.IsEnabled = true;
            //this.Stop.IsEnabled = true;
            //this.Pause.IsEnabled = true;
            //this.Clear.IsEnabled = true;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            var messageOK = new MessageOK();
           

            // messageOK.Owner = this;
            messageOK.Owner = this;
            messageOK.Left = this.Left + (this.ActualWidth - messageOK.Width) / 2;
            messageOK.Top = this.Top + (this.ActualHeight - messageOK.Height) / 2;

            string name = this.NameBox.Text;
            string track = this.TrackingLabel.Content.ToString();
            if (string.IsNullOrEmpty(name))
            {
                messageOK.MessageOKLabel.Text = "Please enter a name";
                messageOK.Show();
               // MessageBox.Show("Please enter a name");
            }
            else if (string.IsNullOrEmpty(track))
            {
                messageOK.MessageOKLabel.Text = "Please add ticket info and press enter";
                messageOK.Show();
               // MessageBox.Show("Please add ticket info and press enter");
            }
            else
            {

                if (this.isPaused == false)
                {
                    this.isPaused = false;
                    this.time = TimeSpan.Zero;
                    this.timer = new Timer(1000);
                    this.timer.Elapsed += this.onTimerElapsed;
                    this.timer.Start();
                }
                else
                {
                    if (this.Start.Content.ToString() == "Resume")
                    {
                        this.Start.Content = "Start";
                    }

                    this.ChangePause();
                }
            }
        }

        private void Show(string v)
        {
            throw new NotImplementedException();
        }

        private void CloseButton_click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.ChangePause();
        }

        private void TabSearch_Click(object sender, RoutedEventArgs e)
        {
            this.TabMaster.SelectedIndex = 0;
        }

        private void TabTicket_Click(object sender, RoutedEventArgs e)
        {
            this.TabMaster.SelectedIndex = 2;
        }

        private void TabSettings_Click(object sender, RoutedEventArgs e)
        {
            this.TabMaster.SelectedIndex = 1;
        }

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
        }

        private void LoadDB()
        {
            var messageOK = new MessageOK();

            // messageOK.Owner = this;
           // messageOK.Owner = this;
            messageOK.Left = this.Left + (this.ActualWidth - messageOK.Width) / 2;
            messageOK.Top = this.Top + (this.ActualHeight - messageOK.Height) / 2;

            try
            {
                this.databasePath = Directory.GetCurrentDirectory() + "\\ticket_database.db";

                // MessageBox.Show(databasePath);
                SQLiteConnection conn = new SQLiteConnection($"Data Source={this.databasePath};");
                System.Console.WriteLine(conn);
                conn.Open();
                string query = "Select * from tickets";
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(cmd);
                DataTable dt = new DataTable();
                dataAdapter.Fill(dt);

                this.TicketGrid.ItemsSource = dt.DefaultView;

                // TicketGrid.Columns[1].Visibility = Visibility.Collapsed;
                conn.Close();
            }
            catch (Exception ex)
            {
                messageOK.MessageOKLabel.Text = "An error occured!" + ex.Message;
                messageOK.Show();
               // MessageBox.Show("An error occured!" + ex.Message);
            }
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
            var messageOK = new MessageOK();

            // messageOK.Owner = this;
            messageOK.Owner = this;
            messageOK.Left = this.Left + (this.ActualWidth - messageOK.Width) / 2;
            messageOK.Top = this.Top + (this.ActualHeight - messageOK.Height) / 2;
            string connectionString = $"Data Source={this.databasePath};";

            var startDate = this.From.SelectedDate;
            var endDate = this.To.SelectedDate;

            if (startDate == null || endDate == null)
            {
                messageOK.MessageOKLabel.Text = "Please select values for both dates";
                messageOK.Show();
               //MessageBox.Show("Please select values for both dates.");
            }
            else if (startDate.Value.Date > endDate.Value.Date)
            {
                messageOK.MessageOKLabel.Text = "Please select a starting date biggr thn the end date";
                messageOK.Show();
               // MessageBox.Show("Please select a starting date bigger than the end date.");
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

                            this.TicketGrid.ItemsSource = dt.DefaultView;

                            conn.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    messageOK.MessageOKLabel.Text ="An error occured!" + ex.Message;
                    messageOK.Show();
                   // MessageBox.Show("An error occured!" + ex.Message);
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
                            command.ExecuteNonQuery();

                            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(command);
                            DataTable dt = new DataTable();
                            dataAdapter.Fill(dt);

                            this.TicketGrid.ItemsSource = dt.DefaultView;

                            conn.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    messageOK.MessageOKLabel.Text = "An error occured!";
                    messageOK.Show();
                    //MessageBox.Show("An error occured!" + ex.Message);
                }
            }
        }

        private void SearchName_Click(object sender, RoutedEventArgs e)
        {
            var messageOK = new MessageOK();

            // messageOK.Owner = this;
            messageOK.Owner = this;
            messageOK.Left = this.Left + (this.ActualWidth - messageOK.Width) / 2;
            messageOK.Top = this.Top + (this.ActualHeight - messageOK.Height) / 2;
            string connectionString = $"Data Source={this.databasePath};";

            string nameValue = this.NameSearchBox.Text;

            // var endDate = To.SelectedDate;
            if (string.IsNullOrEmpty(nameValue))
            {
                messageOK.MessageOKLabel.Text = "Please enter a ticket name";
                messageOK.Show();
               // MessageBox.Show("Please enter a ticket name.");
            }
            else
            {

                try
                {
                    using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                    {
                        conn.Open();
                        string sql = $"SELECT * FROM TICKETS where name like '%{nameValue}%'";
                        using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                        {
                            //command.Parameters.AddWithValue("@nameValue", nameValue);
                            //command.Parameters.AddWithValue("searchPattern", "'%" + nameValue + "%'");
                            command.ExecuteNonQuery();

                            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(command);
                            DataTable dt = new DataTable();
                            dataAdapter.Fill(dt);

                            this.TicketGrid.ItemsSource = dt.DefaultView;

                            conn.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    messageOK.MessageOKLabel.Text = "An error occured!";
                    messageOK.Show();
                   // MessageBox.Show("An error occured!" + ex.Message);
                }
            }
        }

        private void SearchType_Click(object sender, RoutedEventArgs e)
        {
            var messageOK = new MessageOK();

            // MessageOKLoad();
            messageOK.Owner = this;
            messageOK.Left = this.Left + (this.ActualWidth - messageOK.Width) / 2;



            //messageOK.Left = this.Left;
            messageOK.Top = this.Top + (this.ActualHeight - messageOK.Height) / 2;
            // messageOK.Show();
            string connectionString = $"Data Source={this.databasePath};";

            string typeValue = this.TypeSearchBox.Text;
            // var endDate = To.SelectedDate;
            if (string.IsNullOrEmpty(typeValue))
            {
                messageOK.MessageOKLabel.Text = "Please enter a ticket type";
                messageOK.Show();
               // MessageBox.Show("Please enter a ticket type.");
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

                            // command.Parameters.AddWithValue("searchPattern", "%" + typeValue + "%");
                            command.ExecuteNonQuery();

                            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(command);
                            DataTable dt = new DataTable();
                            dataAdapter.Fill(dt);

                            this.TicketGrid.ItemsSource = dt.DefaultView;

                            conn.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    messageOK.MessageOKLabel.Text = "An error occured!";
                    messageOK.Show();
                    //MessageBox.Show("An error occured!" + ex.Message);
                }
            }
        }

        private void SearchTicket_Click(object sender, RoutedEventArgs e)
        {
            var messageOK = new MessageOK();

            // MessageOKLoad();
            //messageOK.Owner = this;
            messageOK.Left = this.Left + (this.ActualWidth - messageOK.Width) / 2;



            //messageOK.Left = this.Left;
            messageOK.Top = this.Top + (this.ActualHeight - messageOK.Height) / 2;
            // messageOK.Show();
            string connectionString = $"Data Source={this.databasePath};";

            string ticketValue = this.TicketSearchBox.Text;

            // var endDate = To.SelectedDate;
            if (string.IsNullOrEmpty(ticketValue))
            {
                messageOK.MessageOKLabel.Text = "Please enter a ticket number";
                messageOK.Show();
               // MessageBox.Show("Please enter a icket number.");
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

                            this.TicketGrid.ItemsSource = dt.DefaultView;

                            conn.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    messageOK.MessageOKLabel.Text = "An error occured!";
                    messageOK.Show();
                    //MessageBox.Show("An error occured!" + ex.Message);
                }
            }
        }

        private void SearchClear_Click(object sender, RoutedEventArgs e)
        {
            var messageOK = new MessageOK();

            // MessageOKLoad();
            messageOK.Owner = this;
            messageOK.Left = this.Left + (this.ActualWidth - messageOK.Width) / 2;



            //messageOK.Left = this.Left;
            messageOK.Top = this.Top + (this.ActualHeight - messageOK.Height) / 2;
            // messageOK.Show();
            try
            {
                this.TicketSearchBox.Text = string.Empty;
                this.NameSearchBox.Text = string.Empty;
                this.TypeSearchBox.Text = string.Empty;
                this.From.SelectedDate = null;
                this.To.SelectedDate = null;
                this.TrackingLabel.Content = string.Empty;

                this.LoadDB();
            }
            catch (Exception ex)
            {
                messageOK.MessageOKLabel.Text = "An error occured!";
                messageOK.Show();
                //MessageBox.Show("An error occured!" + ex.Message);
            }
           
        }

        private void CSVWriter_Click(object sender, RoutedEventArgs e)
        {
            var messageOK = new MessageOK();

            // MessageOKLoad();
            //messageOK.Owner = this;
            messageOK.Left = this.Left + (this.ActualWidth - messageOK.Width) / 2;



            //messageOK.Left = this.Left;
            messageOK.Top = this.Top + (this.ActualHeight - messageOK.Height) / 2;
            // messageOK.Show();
            string connectionString = $"Data Source={this.databasePath};";

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT * FROM tickets";
                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    {

                        command.ExecuteNonQuery();

                        List<string[]> data = new List<string[]>();

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string[] row = new string[reader.FieldCount];
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[i] = reader[i].ToString();
                                }

                                data.Add(row);
                            }

                            SaveFileDialog saveFileDialog = new SaveFileDialog();
                            saveFileDialog.Filter = "CSV file (*.csv)|*.csv";
                            saveFileDialog.DefaultExt = "csv";
                            if (saveFileDialog.ShowDialog() == true)
                            {
                                this.WriteDataToCsv(data, saveFileDialog.FileName);
                            }
                        }

                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                messageOK.MessageOKLabel.Text = "An error occured!";
                messageOK.Show();
               // MessageBox.Show("An error occured!" + ex.Message);
            }
        }

        private void WriteDataToCsv(List<string[]> data, string filePath)
        {
            using (StreamWriter file = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                // Assuming the first row of data contains the headers
                for (int i = 0; i < data[0].Length; i++)
                {
                    file.Write(data[0][i] + (i < data[0].Length - 1 ? "," : string.Empty));
                }

                file.WriteLine();

                // Writing the data
                for (int i = 1; i < data.Count; i++)
                {
                    for (int j = 0; j < data[i].Length; j++)
                    {
                        string value = data[i][j];
                        if (value.Contains(","))
                        {
                            value = "\"" + value + "\""; // Handle commas within fields
                        }

                        file.Write(value + (j < data[i].Length - 1 ? "," : string.Empty));
                    }

                    file.WriteLine();
                }
            }
        }
        private void ClearDB_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = $"Data Source={this.databasePath};";
            MessageBoxResult result = MessageBox.Show("Do you want to clear the Database? Tis is not reversable", "Clear?", MessageBoxButton.OKCancel);

            // MessageBox.Show("Nothing to Save", "Error!", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                try
                {
                    using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                    {
                        conn.Open();
                        string sql = "DELETE FROM TICKETS";
                        using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                        {
                            //command.Parameters.AddWithValue("@typeValue", typeValue);

                            // command.Parameters.AddWithValue("searchPattern", "%" + typeValue + "%");
                            command.ExecuteNonQuery();

                            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(command);
                            DataTable dt = new DataTable();
                            dataAdapter.Fill(dt);

                            this.TicketGrid.ItemsSource = dt.DefaultView;

                            conn.Close();
                        }

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occured!" + ex.Message);
                }
            }
            else if (result == MessageBoxResult.Cancel)
            {
                MessageBox.Show("Canceled.");
            }




        }

        private MessageBoxResult Show(string v1, string v2, MessageBoxButton oKCancel)
        {
            throw new NotImplementedException();
        }


        private void MyDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() == "ID")
            {
                e.Cancel = true; // Cancel the column generation
            }
        }






    }

   
}
