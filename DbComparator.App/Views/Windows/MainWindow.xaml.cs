﻿using DbComparator.App.ViewModels;
using System.Windows;

namespace DbComparator.App.Views.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Close_Command(object sender, RoutedEventArgs e) =>
            this.Close();
    }
}
