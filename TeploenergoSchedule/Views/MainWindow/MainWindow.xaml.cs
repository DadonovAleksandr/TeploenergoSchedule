﻿using System.Windows;
using TeploenergoSchedule.ViewModels.MainWindowVm;

namespace TeploenergoSchedule
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Closing += (sender, args) =>
            {
                if (!(DataContext is MainWindowViewModel mainVm)) return;
                mainVm.OnExit();
            };
        }
    }
}