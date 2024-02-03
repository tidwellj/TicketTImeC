// <copyright file="MessageOKCancel.xaml.cs" company="TidwellSoft">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Windows;

namespace TicketTime
{
    /// <summary>
    /// Interaction logic for MessageOKCancel.xaml
    /// </summary>
    ///

    public partial class MessageOKCancel : Window
    {
        public string toggled;

        private MainWindow _mainWindow;

        public MessageOKCancel(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.UpdateVariable("1");

            CloseWin();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.UpdateVariable("2");

            CloseWin();
        }

        private void CloseWin()
        {
            this.Close();
        }
    }
}
